using System;
using QuickLook;
using Foundation;
namespace MyMindV3.iOS
{
    public class PreviewItemFS : QLPreviewItem
    {
        string filename, filepath;

        public PreviewItemFS(string fn, string fp)
        {
            filename = fn;
            filepath = fp;
        }

        public override string ItemTitle
        {
            get
            {
                return filename;
            }
        }

        public override Foundation.NSUrl ItemUrl
        {
            get
            {
                return NSUrl.FromFilename(filepath);
            }
        }
    }
}
