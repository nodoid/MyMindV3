using System.ComponentModel;
using System.Linq;
using Android.Graphics;
using MyMindV3;
using MyMindV3.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System;

[assembly: ExportRenderer(typeof(CustomImage), typeof(CustomImageRenderer))]
namespace MyMindV3.Droid
{
    public class CustomImageRenderer : ImageRenderer
    {
        string lastFilename;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var img = e.NewElement as CustomImage;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var largeImage = (CustomImage)Element;

            //if (e.PropertyName == "ImageSource")
            //{
            if (lastFilename != largeImage.ImageSource)
            {
                if (Element.Width > 0 && Element.Height > 0)
                {
                    try
                    {
                        using (var options = new BitmapFactory.Options { InJustDecodeBounds = true })
                        {

                            //Get the resource id for the image
                            if (largeImage.ImageSource != null)
                            {
                                var field = typeof(Resource.Drawable).GetField(largeImage.ImageSource.Split('.').First());
                                if (field != null)
                                {
                                    var value = (int)field.GetRawConstantValue();

                                    //BitmapFactory.DecodeResource(Context.Resources, value, options);

                                    //The with and height of the elements (LargeImage) will be used to decode the image
                                    var width = (int)Element.Width;
                                    var height = (int)Element.Height;
                                    options.InSampleSize = CalculateInSampleSize(options, width, height);

                                    options.InJustDecodeBounds = false;
                                    using (var bitmap = BitmapFactory.DecodeResource(Context.Resources, value, options))
                                        Control.SetImageBitmap(bitmap);
                                }
                                lastFilename = largeImage.ImageSource;
                            }
                        }
                    }
                    catch (Java.Lang.OutOfMemoryError f)
                    {
#if DEBUG
                        Console.WriteLine("Out of memory exception thrown - {0}::{1}", f.Message, f.InnerException);
#endif
                    }
                    GC.Collect();
                }
            }
            //}
        }

        public int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }
    }
}
