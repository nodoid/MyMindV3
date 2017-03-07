using System;
using QuickLook;
using System.IO;
using Foundation;
namespace MyMindV3.iOS
{
    public class PreviewItemBundle : QLPreviewItem
    {
        string filename, filepath;

        public override string ItemTitle
        {
            get
            {
                return filename;
            }
        }
        public PreviewItemBundle(string fn, string fp)
        {
            filename = fn;
            filepath = fp;
        }

        public override NSUrl ItemUrl
        {
            get
            {
                var lib = Path.Combine(NSBundle.MainBundle.BundlePath, filepath);
                return NSUrl.FromFilename(lib);
            }
        }
    }
}
