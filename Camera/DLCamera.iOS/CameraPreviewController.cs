
using System;
using System.Drawing;

using Foundation;
using UIKit;
using AVFoundation;

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

            // Perform any additional setup after loading the view, typically from a nib.
        }

        void SetupCaptureSesseion()
        {
        }
    }
}