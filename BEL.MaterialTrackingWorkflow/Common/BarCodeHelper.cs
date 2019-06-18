namespace BEL.MaterialTrackingWorkflow.Common
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public static class BarCodeHelper
    {
        /// <summary>
        /// Draws the text image.
        /// </summary>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="font">The font.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="backColor">Color of the back.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <returns></returns>
        public static string DrawTextImage(String currencyCode, Font font, Color textColor, Color backColor, Size minSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(currencyCode, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }

            //create a new image of the right size
            Image retImg = new Bitmap((int)textSize.Width, (int)textSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    //drawing.DrawString(currencyCode, font, textBrush, 45, 35);
                    drawing.DrawString(currencyCode, font, textBrush, 4, 25);
                    drawing.Save();
                }
            }
            //===================================
            MemoryStream MemStream = new MemoryStream();
            retImg.Save(MemStream, ImageFormat.Jpeg);
            Byte[] barcodeimg = MemStream.ToArray();
            var imageUrl = string.Empty;
            if (barcodeimg != null && barcodeimg.Length > 0)
            {
                imageUrl = "data:image/jpg;base64," + Convert.ToBase64String(barcodeimg);
            }
            return imageUrl;
        }

    }
}