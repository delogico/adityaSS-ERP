using System;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;


namespace RMERP
{
    public static class Extends
    {
        public static Image Resize(this Image current, int maxWidth, int maxHeight)
        {
            int width, height;
            #region reckon size
            if(maxWidth < current.Width || maxHeight< current.Height)
            {
                if (current.Width > current.Height)
                {
                    width = maxWidth;
                    height = Convert.ToInt32(current.Height * maxHeight / (double)current.Width);
                }
                else
                {
                    width = Convert.ToInt32(current.Width * maxWidth / (double)current.Height);
                    height = maxHeight;
                }
            }
            else
            {
                width = current.Width;
                height = current.Height;
            }
            
            #endregion

            #region get resized bitmap
            var canvas = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.CompositingQuality = CompositingQuality.Default;
                //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.InterpolationMode = InterpolationMode.Low;              
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(current, 0, 0, width, height);
            }

            return canvas;
            #endregion
        }

        public static byte[] ToByteArray(this Image current)
        {
            using (var stream = new MemoryStream())
            {
                current.Save(stream,System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
    }
}