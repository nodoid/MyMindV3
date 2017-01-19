using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using Java.IO;

namespace MyMindV3.Droid
{
    public static class UIUtils
    {
        public static Android.Graphics.Color ToAndroidGraphicsColor(this Xamarin.Forms.Color color)
        {
            var newColor = new Android.Graphics.Color()
            {
                R = Convert.ToByte(color.R),
                G = Convert.ToByte(color.G),
                B = Convert.ToByte(color.B),
                A = Convert.ToByte(color.A)
            };
            return newColor;
        }

        public static Xamarin.Forms.Color ToXFColor(this Android.Graphics.Color color)
        {
            var newColor = Xamarin.Forms.Color.FromRgba(color.R, color.G, color.B, color.A);
            return newColor;
        }

        public static async Task CreateBMPAndSave(this Bitmap image, string filename)
        {
            if (!System.IO.File.Exists(filename))
                System.IO.File.Create(filename);
            using (var ms = new MemoryStream())
            {
                await image.CompressAsync(Bitmap.CompressFormat.Jpeg, 90, ms);
                var buffer = ms.GetBuffer();
                using (var output = new FileOutputStream(filename))
                    output.Write(buffer);
                //await Torrent.CopyAndMove(filename, false);
            }
        }

        private static Size GetImageSizeFromArray(byte[] imgBuffer)
        {
            var options = new BitmapFactory.Options()
            {
                InJustDecodeBounds = true
            };

            BitmapFactory.DecodeByteArray(imgBuffer, 0, imgBuffer.Length, options);

            return new Size(options.OutWidth, options.OutHeight);
        }

        public static int CalculateSampleSizePower2(Size originalSize, int reqWidth, int reqHeight)
        {
            int height = originalSize.Height, width = originalSize.Width, inSampleSize = 1;
            int IMAGE_MAX_SIZE = reqWidth >= reqHeight ? reqWidth : reqHeight;

            if (height > IMAGE_MAX_SIZE || width > IMAGE_MAX_SIZE)
            {
                inSampleSize = (int)Math.Pow(2, (int)Math.Round(Math.Log(IMAGE_MAX_SIZE /
                            (double)Math.Max(height, width)) / Math.Log(0.5)));
            }

            return inSampleSize;
        }

        private static int CalculateSampleSize(Size originalSize, int reqWidth, int reqHeight)
        {
            int sampleSize = 1;

            if (originalSize.Height > reqHeight || originalSize.Width > reqWidth)
                sampleSize = Convert.ToInt32(originalSize.Width > originalSize.Height ?
                    (double)originalSize.Height / (double)reqHeight : (double)originalSize.Width / (double)reqWidth);

            return sampleSize;
        }

        public static Bitmap CreateImageForDisplay(byte[] userImage, int width, int height, Resources res)
        {
            if (userImage.Length > 0 && userImage.Length != 2)
            {
                var imgSize = GetImageSizeFromArray(userImage);

                var options = new BitmapFactory.Options()
                {
                    InSampleSize = CalculateSampleSizePower2(imgSize, width, height)
                };
                var scaledUserImage = BitmapFactory.DecodeByteArray(userImage, 0, userImage.Length, options);

                int scaledWidth = scaledUserImage.Width, scaledHeight = scaledUserImage.Height;

                var resultImage = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(res, Resource.Drawable.dummy), scaledWidth, scaledHeight, true);
                using (var canvas = new Canvas(resultImage))
                {
                    using (var paint = new Paint(PaintFlags.AntiAlias)
                    {
                        Dither = false,
                        FilterBitmap = true
                    })
                    {
                        paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstIn));
                        canvas.DrawBitmap(scaledUserImage, 0, 0, null);
                        scaledUserImage.Recycle();

                        using (var maskImage = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(res, Resource.Drawable.emptybackground), scaledWidth, scaledHeight, true))
                        {
                            canvas.DrawBitmap(maskImage, 0, 0, paint);
                            maskImage.Recycle();
                        }
                    }
                }
                return resultImage;
            }
            else
                return null;
        }


        public static void resizeLayout(LinearLayout layout, Context c)
        {
            int[] newSize = new int[2];
            using (LinearLayout.LayoutParams layParams = new LinearLayout.LayoutParams(newSize[0], newSize[1]))
            {
                layout.LayoutParameters = layParams;
            }
        }

        public async static Task<Bitmap> rotateImage(string fileName, bool left = false)
        {
            //var oldMode = GCSettings.LatencyMode;

            // Make sure we can always go to the catch block, 
            // so we can set the latency mode back to `oldMode`
            //RuntimeHelpers.PrepareConstrainedRegions();

            Bitmap toReturn = null;
            /*try
            {*/
            //GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            using (var bmp = await BitmapFactory.DecodeFileAsync(fileName))
            {
                if (bmp != null)
                    using (var matrix = new Matrix())
                    {
                        var scaledBitmap = Bitmap.CreateScaledBitmap(bmp, bmp.Width, bmp.Height, true);
                        matrix.PostRotate(!left ? 90 : -90);
                        toReturn = Bitmap.CreateBitmap(scaledBitmap, 0, 0, bmp.Width, bmp.Height, matrix, true);
                        using (var stream = new MemoryStream())
                        {
                            await toReturn.CompressAsync(Bitmap.CompressFormat.Png, 90, stream);
                            var bitmapData = stream.ToArray();
                            System.IO.File.WriteAllBytes(fileName, bitmapData);
                        }
                    }
            }
            /*}
            finally
            {
                GCSettings.LatencyMode = oldMode;
            }*/
            return toReturn;
        }

        static void rotateMatrix(int[][] a)
        {
            int m = 0;
            for (int i = 0; i < a.Length; ++i)
            {
                for (int j = m; j < a[0].Length; ++j)
                {
                    int tmp = a[i][j];
                    a[i][j] = a[j][i];
                    a[j][i] = tmp;
                }
                m++;
            }

            for (int i = 0; i < a.Length; ++i)
            {
                int end = a.Length - 1;
                for (int j = 0; j < a[0].Length; j++)
                {
                    if (j >= end)
                        break;
                    int tmp = a[i][j];
                    a[i][j] = a[i][end];
                    a[i][end] = tmp;
                    end--;
                }
            }
        }

        private static float resizeFont(string text, float size, float btnWidth, Context context)
        {
            var paint = new Paint(PaintFlags.AntiAlias)
            {
                TextSize = size
            };
            paint.SetTypeface(Typeface.DefaultBold);
            float width = convertDpToPixel(paint.MeasureText(text), context);
            float pxWidth = convertDpToPixel(btnWidth, context);
            while (width <= pxWidth)
            {
                size += .5f;
                paint.TextSize = size;
                width = convertDpToPixel(paint.MeasureText(text), context);
            }

            return size;
        }

        public static byte[] convColToByteArray(Android.Graphics.Color color)
        {
            var toReturn = new byte[] { color.R, color.G, color.B, color.A };
            return toReturn;
        }

        public static float getNewFontSize(float size, Context c)
        {
            float fSize = convertDpToPixel(size, c);
            float calc = fSize + (fSize * .1f);
            return convertPixelToDp(calc, c);
        }

        public static float convertDpToPixel(float dp, Context context)
        {
            var metrics = context.Resources.DisplayMetrics;
            return dp * ((float)metrics.DensityDpi / 160f);
        }

        public static float convertPixelToDp(float px, Context context)
        {
            var metrics = context.Resources.DisplayMetrics;
            return (px * 160f) / (float)metrics.DensityDpi;
        }

        public static byte[] CreateByteImageFromResources(this ImageView img, Context context, string filename)
        {
            byte[] userImage = null;
            var imgId = GetResourceIdFromFilename(context, filename);
            try
            {
                using (var bmp = BitmapFactory.DecodeResource(context.Resources, imgId))
                {
                    if (bmp != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            bmp.Compress(Bitmap.CompressFormat.Jpeg, 80, ms);
                            if (userImage == null)
                                userImage = new byte[ms.Length];
                            userImage = ms.ToArray();
                        }
                    }
                }
            }
            catch (Java.Lang.OutOfMemoryError)
            {
                System.Console.WriteLine("Out of memory error caught in create byte image from resource");
            }

            return userImage;
        }

        public static byte[] CreateByteImage(this ImageView img, string filename)
        {
            byte[] userImage = null;
            try
            {
                if (System.IO.File.Exists(filename))
                {
                    using (var bmp = BitmapFactory.DecodeFile(filename))
                    {
                        if (bmp != null)
                        {
                            using (var ms = new MemoryStream())
                            {
                                bmp.Compress(Bitmap.CompressFormat.Jpeg, 80, ms);
                                if (userImage == null)
                                    userImage = new byte[ms.Length];
                                userImage = ms.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Java.Lang.OutOfMemoryError)
            {
                System.Console.WriteLine("Out of memory error caught in create byte image");
            }
            return userImage;
        }

        public static Bitmap CreateBitmapImage(Context context, Resources res, byte[] image, int x = 120, int y = 120)
        {
            if (image != null)
            {
                if (image.Length > 0)
                {
                    var myBitmap = CreateImageForDisplay(image, (int)convertDpToPixel(x, context),
                                       (int)convertDpToPixel(y, context), res);
                    return myBitmap;
                }
                else
                    return Bitmap.CreateBitmap((int)convertDpToPixel(x, context),
                        (int)convertDpToPixel(y, context), Bitmap.Config.Argb4444);
            }
            else
                return Bitmap.CreateBitmap((int)convertDpToPixel(x, context),
                    (int)convertDpToPixel(y, context), Bitmap.Config.Argb4444);
        }

        public static int GetResourceIdFromFilename(Context context, string filename)
        {
            var fn2 = filename.Replace('-', '_');
            fn2 = fn2.Split('.').ToArray()[0];
            var res = context.Resources.GetIdentifier(fn2, "drawable", context.PackageName);
            return res;
        }
    }
}
