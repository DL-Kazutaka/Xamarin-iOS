using System;
using System.Drawing;

using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using UIKit;

namespace DLCamera.iOS
{

    public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            try
            {
                // 取得したバッファーからイメージを作成
                var image = ImageFromSampleBuffer(sampleBuffer);

                // プレビュー用のビューに画像を表示
                CameraPreviewController.previewView.BeginInvokeOnMainThread(delegate
                {
                    //// プレビューにイメージを挿入
                    CameraPreviewController.previewView.Image = image;
                    // 90度回転させる
                    CameraPreviewController.previewView.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);
                    // 90度回転させる
                    CameraPreviewController.frameView.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);

                    ////// ベースのサイズの取得
                    //CameraPreviewController.previewView size = image.Size;
                    var size = image.Size;
                    ////// ビットマップ形式のグラフィックスコンテキストの生成
                    //UIGraphics.BeginImageContextWithOptions(size, false, UIScreen.MainScreen.Scale);
                    ////// 領域を決めて塗りつぶす
                    ////AppDelegate.ImageView.Image.DrawAsPatternInRect(new RectangleF(0, 0, size.Width, size.Height));
                    ////// フレーム画像を取得
                    ////// 領域を決めて塗りつぶす(画像サイズが塗りつぶす領域に満たない場合はループする)
                    ////AppDelegate.ImageFrame.DrawAsPatternInRect(new RectangleF(0, 0, AppDelegate.ImageFrame.Size.Width, AppDelegate.ImageFrame.Size.Height));
                    ////// 現在のグラフィックスコンテキストの画像を取得する
                    //var dstImage = UIGraphics.GetImageFromCurrentImageContext();
                    ////// 現在のグラフィックスコンテキストへの編集を終了
                    ////// (スタックの先頭から削除する)
                    //UIGraphics.EndImageContext();

                    //// 画面に最大限表示されるように、スケールを算出する
                    //float scaleWidth = UIScreen.MainScreen.Bounds.Width / (float)dstImage.Size.Height;
                    //float scaleHeight = UIScreen.MainScreen.Bounds.Height / (float)dstImage.Size.Width;
                    //float scale = Math.Min(scaleWidth, scaleHeight);

                    float screenWidth = (float)UIScreen.MainScreen.Bounds.Width;
                    float screenHeight = (float)UIScreen.MainScreen.Bounds.Height;
                    // 画面に最大限表示されるように、スケールを算出する
                    float scaleWidth = screenWidth / (float)image.Size.Height;
                    float scaleHeight = screenHeight / (float)image.Size.Width;
                    float scale = Math.Min(scaleWidth, scaleHeight);

                    //// プレビューのサイズを変更
                    CameraPreviewController.previewView.Frame = new CGRect(0, 0, size.Height * scale, size.Width * scale);
                    //// 表示位置センターを修正
                    CameraPreviewController.previewView.Center = new CGPoint(screenWidth / 2, screenHeight / 2);

                    //// フレームサイズを変更
                    CameraPreviewController.frameView.Frame = new CGRect(0, 100, size.Height * scale, size.Width * scale);
                    //// 表示位置センターを修正
                    CameraPreviewController.frameView.Center = new CGPoint(screenWidth / 2, screenHeight / 2);

                    ////// 合成画像をImageに設定する
                    ////AppDelegate.ImageView.Image = dstImage;

                    // 画像に合わせてボタンサイズも変更
                    //float height = (UIScreen.MainScreen.Bounds.Height - CameraPreview.ImageView.Frame.Height) / 2;
                    //CameraPreview.ButtonTake.Frame = new RectangleF((UIScreen.MainScreen.Bounds.Width - height) / 2, UIScreen.MainScreen.Bounds.Height - height, height, height);
                    // 表示位置センターを修正
                    //CameraPreview.ButtonTake.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height - height / 2);
                });

                //
                // Although this looks innocent "Oh, he is just optimizing this case away"
                // this is incredibly important to call on this callback, because the AVFoundation
                // has a fixed number of buffers and if it runs out of free buffers, it will stop
                // delivering frames. 
                //	
                sampleBuffer.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        UIImage ImageFromSampleBuffer(CMSampleBuffer sampleBuffer)
        {
            // Get the CoreVideo image
            using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
            {
                // バッファーをロックする
                pixelBuffer.Lock(0);
                // ピクセルバッファーの行あたりのバイト数を取得する
                var baseAddress = pixelBuffer.BaseAddress;
                int bytesPerRow = (int)pixelBuffer.BytesPerRow;
                int width = (int)pixelBuffer.Width;
                int height = (int)pixelBuffer.Height;
                var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
                // 上記のように構成されたパラメータからRGB色空間上のCGImageを作成
                using (var cs = CGColorSpace.CreateDeviceRGB())
                using (var context = new CGBitmapContext(baseAddress, width, height, 8, bytesPerRow, cs, (CGImageAlphaInfo)flags))
                using (var cgImage = context.ToImage())
                {
                    pixelBuffer.Unlock(0);
                    return UIImage.FromImage(cgImage);
                }
            }
        }
    }
}