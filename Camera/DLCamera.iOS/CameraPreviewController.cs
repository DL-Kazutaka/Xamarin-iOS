
using System;
using System.Drawing;

using Foundation;
using UIKit;
using AVFoundation;
using CoreMedia;
using CoreVideo;

namespace DLCamera.iOS
{
	public partial class CameraPreviewController : UIViewController
	{
		static bool UserInterfaceIdiomIsPhone
		{
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public CameraPreviewController()
			: base(UserInterfaceIdiomIsPhone ? "CameraPreviewController_iPhone" : "CameraPreviewController_iPad", null)
		{
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            // キャプチャーセッションを設定
            SetupCaptureSesseion();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		bool SetupCaptureSesseion()
		{
			// セッションを作成
			var session = new AVCaptureSession()
			{
				// 解像度を設定
				SessionPreset = AVCaptureSession.PresetMedium
			};

			// メディアの種類からキャプチャデバイスを作成
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			if (captureDevice == null)
			{
				// メディアが取得できない場合
				Console.WriteLine("No captureDevice - this won't work on the simulator, try a physical device");
				return false;
			}

			NSError error = null;
			// デバイスをロックする
			captureDevice.LockForConfiguration(out error);
			if (error != null)
			{
				// キャプチャのロックに失敗した場合
				Console.WriteLine(error);
				captureDevice.UnlockForConfiguration();
				return false;
			}

			// FPSを設定
			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				captureDevice.ActiveVideoMinFrameDuration = new CMTime(1, 15);

			// デバイスのロックを解除する
			captureDevice.UnlockForConfiguration();

			// デバイスからのインプットを取得
			var input = AVCaptureDeviceInput.FromDevice(captureDevice);
			if (input == null)
			{
				Console.WriteLine("No input - this won't work on the simulator, try a physical device");
				return false;
			}
			// セッションにインプットを追加
			session.AddInput(input);

			// 出力先を作成
			var output = new AVCaptureVideoDataOutput()
			{
				WeakVideoSettings =  new CVPixelBufferAttributes () {
										PixelFormatType = CVPixelFormatType.CV32BGRA
									 }.Dictionary,
			};


			// 出力設定
			// 画像取得時のコールバック用のキューを作成
			var queue = new CoreFoundation.DispatchQueue("myQueue");
			// 画像取得時のコールバック用のメソッドを作成
			var outputRecorder = new OutputRecorder();
			// 画像取得時のデリゲートを設定
			output.SetSampleBufferDelegate(outputRecorder, queue);
			session.AddOutput(output);

			// セッション開始
			session.StartRunning();
			return true;
		}
	}
}