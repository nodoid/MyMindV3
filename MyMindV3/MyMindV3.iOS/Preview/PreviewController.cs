using System;
using QuickLook;
namespace MyMindV3.iOS
{
    public class PreviewController : QLPreviewControllerDataSource
    {
        QLPreviewItem item;
        public PreviewController(QLPreviewItem i)
        {
            item = i;
        }
        public override nint PreviewItemCount(QLPreviewController controller)
        {
            return 1;
        }
        public override IQLPreviewItem GetPreviewItem(QLPreviewController controller, nint index)
        {
            return item;
        }
    }
}
