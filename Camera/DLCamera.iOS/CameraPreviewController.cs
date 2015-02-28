
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

            // �L���v�`���[�Z�b�V������ݒ�
            SetupCaptureSesseion();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		bool SetupCaptureSesseion()
		{
			// �Z�b�V�������쐬
			var session = new AVCaptureSession()
			{
				// �𑜓x��ݒ�
				SessionPreset = AVCaptureSession.PresetMedium
			};

			// ���f�B�A�̎�ނ���L���v�`���f�o�C�X���쐬
			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			if (captureDevice == null)
			{
				// ���f�B�A���擾�ł��Ȃ��ꍇ
				Console.WriteLine("No captureDevice - this won't work on the simulator, try a physical device");
				return false;
			}

			NSError error = null;
			// �f�o�C�X�����b�N����
			captureDevice.LockForConfiguration(out error);
			if (error != null)
			{
				// �L���v�`���̃��b�N�Ɏ��s�����ꍇ
				Console.WriteLine(error);
				captureDevice.UnlockForConfiguration();
				return false;
			}

			// FPS��ݒ�
			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				captureDevice.ActiveVideoMinFrameDuration = new CMTime(1, 15);

			// �f�o�C�X�̃��b�N����������
			captureDevice.UnlockForConfiguration();

			// �f�o�C�X����̃C���v�b�g���擾
			var input = AVCaptureDeviceInput.FromDevice(captureDevice);
			if (input == null)
			{
				Console.WriteLine("No input - this won't work on the simulator, try a physical device");
				return false;
			}
			// �Z�b�V�����ɃC���v�b�g��ǉ�
			session.AddInput(input);

			// �o�͐���쐬
			var output = new AVCaptureVideoDataOutput()
			{
				WeakVideoSettings =  new CVPixelBufferAttributes () {
										PixelFormatType = CVPixelFormatType.CV32BGRA
									 }.Dictionary,
			};


			// �o�͐ݒ�
			// �摜�擾���̃R�[���o�b�N�p�̃L���[���쐬
			var queue = new CoreFoundation.DispatchQueue("myQueue");
			// �摜�擾���̃R�[���o�b�N�p�̃��\�b�h���쐬
			var outputRecorder = new OutputRecorder();
			// �摜�擾���̃f���Q�[�g��ݒ�
			output.SetSampleBufferDelegate(outputRecorder, queue);
			session.AddOutput(output);

			// �Z�b�V�����J�n
			session.StartRunning();
			return true;
		}
	}
}