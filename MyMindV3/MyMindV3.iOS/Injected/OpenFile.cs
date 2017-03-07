using System;
using System.IO;
using System.Linq;
using Foundation;
using MyMindV3.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpenFile))]
namespace MyMindV3.iOS
{
    public class OpenFile : IFile
    {
        public void OpenFileExternally(string fileName)
        {
            try
            {
                Stream stream = File.OpenRead(fileName);

                if (!NSFileManager.DefaultManager.FileExists(fileName))
                {
                    var imgData = NSData.FromStream(stream);
                    NSError err;
                    imgData.Save(fileName, false, out err);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    var firstController = UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First();
					var navcontroller = firstController as UIViewController;
                    var uidic = UIDocumentInteractionController.FromUrl(new NSUrl(fileName, true));
                    uidic.Delegate = new DocInteractionC(navcontroller);
                    uidic.PresentPreview(true);

                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown - {0}::{1}", e.Message, e.InnerException);
            }
        }

        public class DocInteractionC : UIDocumentInteractionControllerDelegate
        {
			readonly UIViewController navigationController;

			public DocInteractionC(UIViewController controller)
            {
                navigationController = controller;
            }

            public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
            {
                return navigationController;
            }

            public override UIView ViewForPreview(UIDocumentInteractionController controller)
            {
                return navigationController.View;
            }
        }
    }
}