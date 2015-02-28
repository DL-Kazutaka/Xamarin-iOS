// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DLCamera.iOS
{
	[Register ("CameraPreviewController")]
	partial class CameraPreviewController
	{
		[Outlet]
		public static UIKit.UIImageView frameView { get; set; }

		[Outlet]
		public static UIKit.UIImageView previewView { get; set; }

		[Action ("takePhoto:")]
		partial void takePhoto (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (previewView != null) {
				previewView.Dispose ();
				previewView = null;
			}

			if (frameView != null) {
				frameView.Dispose ();
				frameView = null;
			}
		}
	}
}
