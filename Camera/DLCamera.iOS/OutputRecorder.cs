using AVFoundation;

namespace DLCamera.iOS
{

    public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            try
            {
                var image = ImageFromSampleBuffer(sampleBuffer);

                // Do something with the image, we just stuff it in our main view.
                CameraPreview.ImageView.BeginInvokeOnMainThread(delegate
                {
                    //// プレビュー表示中
                    CameraPreview.ImageView.Image = image;
                    CameraPreview.ImageView.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);

                    CameraPreview.ImageViewFrame.Transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);

                    ////// ベースのサイズの取得
                    CameraPreview.size = image.Size;
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


                    float scaleWidth = UIScreen.MainScreen.Bounds.Width / (float)image.Size.Height;
                    float scaleHeight = UIScreen.MainScreen.Bounds.Height / (float)image.Size.Width;
                    float scale = Math.Min(scaleWidth, scaleHeight);

                    //// フレームサイズを変更
                    CameraPreview.ImageView.Frame = new RectangleF(0, 0, size.Height * scale, size.Width * scale);
                    //// 表示位置センターを修正
                    CameraPreview.ImageView.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 2);

                    //// フレームサイズを変更
                    CameraPreview.ImageViewFrame.Frame = new RectangleF(0, 100, size.Height * scale, size.Width * scale);
                    //// 表示位置センターを修正
                    CameraPreview.ImageViewFrame.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height / 2);

                    ////// 合成画像をImageに設定する
                    ////AppDelegate.ImageView.Image = dstImage;

                    // 画像に合わせてボタンサイズも変更
                    float height = (UIScreen.MainScreen.Bounds.Height - CameraPreview.ImageView.Frame.Height) / 2;
                    CameraPreview.ButtonTake.Frame = new RectangleF((UIScreen.MainScreen.Bounds.Width - height) / 2, UIScreen.MainScreen.Bounds.Height - height, height, height);
                    // 表示位置センターを修正
                    CameraPreview.ButtonTake.Center = new PointF(UIScreen.MainScreen.Bounds.Width / 2, UIScreen.MainScreen.Bounds.Height - height / 2);
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
                // Lock the base address
                pixelBuffer.Lock(0);
                // Get the number of bytes per row for the pixel buffer
                var baseAddress = pixelBuffer.BaseAddress;
                int bytesPerRow = pixelBuffer.BytesPerRow;
                int width = pixelBuffer.Width;
                int height = pixelBuffer.Height;
                var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;
                // Create a CGImage on the RGB colorspace from the configured parameter above
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