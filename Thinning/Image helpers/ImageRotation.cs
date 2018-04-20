using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Thinning
{
    public static class ImageRotation
    {
        public static Image RotateImage(PictureBox pb, Image img, float angle)     //http://www.codeproject.com/Articles/58815/C-Image-PictureBox-Rotations
        {
            if (img == null || pb.Image == null)
                return null;

            Image oldImage = pb.Image;

            pb.Image = ImageRotation.RotateImg((Bitmap)img, angle, Color.White);          //  pb.Image = ImageExtension.RotateImage(img, angle);
            if (oldImage != null)
            {
                oldImage.Dispose();
            }

            return pb.Image;
        }

        public static Bitmap RotateImg(Bitmap bmp, float angle, Color bkColor)         // http://stackoverflow.com/questions/14184700/how-to-rotate-image-x-degrees-in-c
        {
            angle = angle % 360;
            if (angle > 180)
                angle -= 360;

            System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
            if (bkColor == Color.Transparent)
            {
                pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            float sin = (float)Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
            float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;

            if (angle > 0)
            {
                if (angle <= 90)
                    originX = sin * bmp.Height;
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angle >= -90)
                    originY = sin * bmp.Width;
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }

            Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
            Graphics g = Graphics.FromImage(newImg);
            g.Clear(bkColor);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
            g.Dispose();

            return newImg;
        }


        /*
       public static Bitmap RotateImage(Image image, float angle)          //http://www.codeproject.com/Articles/58815/C-Image-PictureBox-Rotations
       {
           // center of the image
           float rotateAtX = image.Width / 2;
           float rotateAtY = image.Height / 2;
           bool bNoClip = false;
           return RotateImage(image, rotateAtX, rotateAtY, angle, bNoClip);
       }
       */
        /*
        public static Bitmap RotateImage(Image image, float angle, bool bNoClip)
        {
            // center of the image
            float rotateAtX = image.Width / 2;
            float rotateAtY = image.Height / 2;
            return RotateImage(image, rotateAtX, rotateAtY, angle, bNoClip);
        }
        */
        /*
        public static Bitmap RotateImage(Image image, float rotateAtX, float rotateAtY, float angle, bool bNoClip)
        {
            int W, H, X, Y;
            if (bNoClip)
            {
                double dW = (double)image.Width;
                double dH = (double)image.Height;

                double degrees = Math.Abs(angle);
                if (degrees <= 90)
                {
                    double radians = 0.0174532925 * degrees;
                    double dSin = Math.Sin(radians);
                    double dCos = Math.Cos(radians);
                    W = (int)(dH * dSin + dW * dCos);
                    H = (int)(dW * dSin + dH * dCos);
                    X = (W - image.Width) / 2;
                    Y = (H - image.Height) / 2;
                }
                else
                {
                    degrees -= 90;
                    double radians = 0.0174532925 * degrees;
                    double dSin = Math.Sin(radians);
                    double dCos = Math.Cos(radians);
                    W = (int)(dW * dSin + dH * dCos);
                    H = (int)(dH * dSin + dW * dCos);
                    X = (W - image.Width) / 2;
                    Y = (H - image.Height) / 2;
                }
            }
            else
            {
                W = image.Width;
                H = image.Height;
                X = 0;
                Y = 0;
            }

            //create a new empty bitmap to hold rotated image
            Bitmap bmpRet = new Bitmap(W, H);
            bmpRet.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(bmpRet);

            //Put the rotation point in the "center" of the image
            g.TranslateTransform(rotateAtX + X, rotateAtY + Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-rotateAtX - X, -rotateAtY - Y);


            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;


            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0 + X, 0 + Y));

            return bmpRet;
        }
        */
    }
}
