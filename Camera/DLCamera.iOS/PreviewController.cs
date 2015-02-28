using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using AVFoundation;
using CoreGraphics;

namespace DLCamera.iOS
{
    [Register("UniversalView")]
    public class UniversalView : UIView
    {
        public UniversalView()
        {
            Initialize();
        }

        public UniversalView(RectangleF bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.Red;
        }
    }

    [Register("PreviewController")]
    public class PreviewController : UIViewController
    {
        AVCaptureSession session;
        DispatchQueue queue;
        OutputRecorder outputRecorder;
        AVAudioPlayer audioPlayer;

        public static UIImageView ImageView;
        public static UIImageView ImageViewFrame;
        public static UIImage ImageComposite;
        public static SizeF size;

        public static UIButton ButtonTake;

        public PreviewController()
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
            View = new UniversalView();

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // キャプチャー情報の設定
            SetupCaptureSession();

            // 画面レイアウトの設定
            CreateLayout();
        }

        void CreateLayout()
        {
            CGPoint center = new PointF((float)UIScreen.MainScreen.Bounds.Width / 2, (float)UIScreen.MainScreen.Bounds.Height / 2);
            // カメラ画像用のViewを作成、サイズは何でも良い
            ImageView = new UIImageView(new RectangleF(0, 0, 480, 360));
            ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageView.Center = center;

            // フレーム表示用のViewを作成する
            ImageViewFrame = new UIImageView(new RectangleF(0, 0, 480, 360));
            ImageViewFrame.ContentMode = UIViewContentMode.ScaleAspectFit;
            ImageViewFrame.Image = UIImage.FromBundle("waku1.png");
            ImageViewFrame.Center = center;

            // 撮影用のボタンの作成
            ButtonTake = UIButton.FromType(UIButtonType.RoundedRect);
            UIImage button = UIImage.FromBundle("button_blue.png");
            ButtonTake.SetBackgroundImage(button, UIControlState.Normal);
            float height = UIScreen.MainScreen.Bounds.Width / (float)4;
            ButtonTake.Frame = new RectangleF(height * 3 / 2, UIScreen.MainScreen.Bounds.Height - height, height, height);
            ButtonTake.ContentMode = UIViewContentMode.ScaleAspectFit;
            ButtonTake.SetTitle("Take", UIControlState.Normal);
            ButtonTake.TouchUpInside += (object sender, EventArgs e) =>
            {
                audioPlayer.Play();
                CreateComposite();
                AppDelegate.NaviController.PushViewController(new SaveView(), false);
            };

            // フレーム画像名
            string[] images = {
                                  "waku1.png",
                                  "waku2.png",
                                  "waku3.png",
                                  "waku4.png",
                              };

            // フレーム画像選択用のボタンを作成
            for (int i = 0; i < images.Length; i++)
                this.View.AddSubview(CreateButton(images[i], i, images.Length));

            // サブビューに作成したビューを追加
            this.View.AddSubview(ImageView);
            this.View.AddSubview(ImageViewFrame);
            this.View.AddSubview(ButtonTake);
        }

        /// <summary>
        /// セッションの設定
        /// </summary>
        /// <returns></returns>
        bool SetupCaptureSession()
        {
            // configure the capture session for low resolution, change this if your code
            // can cope with more data or volume
            session = new AVCaptureSession()
            {
                SessionPreset = AVCaptureSession.PresetMedium
            };

            // create a device input and attach it to the session
            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            if (captureDevice == null)
            {
                Console.WriteLine("No captureDevice - this won't work on the simulator, try a physical device");
                return false;
            }
            //Configure for 15 FPS. Note use of LockForConigfuration()/UnlockForConfiguration()
            NSError error = null;
            captureDevice.LockForConfiguration(out error);
            if (error != null)
            {
                Console.WriteLine(error);
                captureDevice.UnlockForConfiguration();
                return false;
            }
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                captureDevice.ActiveVideoMinFrameDuration = new CMTime(1, 15);
            captureDevice.UnlockForConfiguration();


            var input = AVCaptureDeviceInput.FromDevice(captureDevice);
            if (input == null)
            {
                Console.WriteLine("No input - this won't work on the simulator, try a physical device");
                return false;
            }
            session.AddInput(input);

            // create a VideoDataOutput and add it to the sesion
            var output = new AVCaptureVideoDataOutput()
            {
                VideoSettings = new AVVideoSettings(CVPixelFormatType.CV32BGRA),
            };


            // configure the output
            queue = new MonoTouch.CoreFoundation.DispatchQueue("myQueue");
            outputRecorder = new OutputRecorder();
            output.SetSampleBufferDelegate(outputRecorder, queue);
            session.AddOutput(output);

            session.StartRunning();
            return true;
        }
    }
}