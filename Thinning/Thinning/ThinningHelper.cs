using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
   public class ThinningHelper
    {/*     Stará verzia
       public static bool[][] Image2Bool(Image img) //http://stackoverflow.com/questions/20245003/zhang-suen-thinning-algorithm-c-sharp
       {
           Bitmap bmp = new Bitmap(img);
           LockBitmap lockBitmap = new LockBitmap(bmp);
           lockBitmap.LockBits();


           bool[][] s = new bool[bmp.Height][];
           for (int y = 0; y < bmp.Height; y++)
           {
               s[y] = new bool[bmp.Width];
               for (int x = 0; x < /*bmp*//*lockBitmap.Width; x++)
                   s[y][x] = /*bmp*//*lockBitmap.GetPixel(x, y).GetBrightness() < 0.3;
           }
           lockBitmap.UnlockBits();
           return s;
       }
*/


       public static bool[][] Image2Bool(Image img) //http://csharpexamples.com/fast-image-processing-c/
       {
           unsafe
           {                                         // V podstate prahovanie
               Bitmap bmp = new Bitmap(img);
               Color clr = Color.Empty;
               /*
               LockBitmap lockBitmap = new LockBitmap(bmp);
               lockBitmap.LockBits();
               */
               BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
               int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
               int heightInPixels = bitmapData.Height;
               int widthInBytes = bitmapData.Width * bytesPerPixel;
               byte* ptrFirstPixel = (byte*)bitmapData.Scan0;


               bool[][] s = new bool[bmp.Height][];
               for (int y = 0; y < bmp.Height; y++)
               {
                   s[y] = new bool[bmp.Width];
                   byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                   for (int x = 0, x2 =0; /*x < bmp/*lockBitmap*//*.Width; x++*/x < widthInBytes; x = x + bytesPerPixel, x2++)
                   {
                       int oldBlue = currentLine[x];
                       int oldGreen = currentLine[x + 1];
                       int oldRed = currentLine[x + 2];

                       clr = Color.FromArgb(oldRed, oldGreen, oldBlue);

                       s[y][x2] = /*bmp/*lockBitmap*//*.GetPixel(x, y)*/clr.GetBrightness() < 0.3;
                   }                     
               }
               //lockBitmap.UnlockBits();
               bmp.UnlockBits(bitmapData);
               return s;
           }
        }

       /*       Stará verzia
       public static Image Bool2Image(bool[][] s)
       {
           Bitmap bmp = new Bitmap(s[0].Length, s.Length);

           using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);

           LockBitmap lockBitmap = new LockBitmap(bmp);
           lockBitmap.LockBits();

           for (int y = 0; y < bmp.Height; y++)
               for (int x = 0; x < bmp.Width; x++)
                   if (s[y][x]) /*bmp*//*lockBitmap.SetPixel(x, y, Color.Black);

           lockBitmap.UnlockBits();
           return (Bitmap)bmp;
       }
    */
        public static Image Bool2Image(bool[][] s)    //  http://csharpexamples.com/fast-image-processing-c/
        {
            unsafe
            {
                Bitmap bmp = new Bitmap(s[0].Length, s.Length);

                using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);
                /*
                LockBitmap lockBitmap = new LockBitmap(bmp);
                lockBitmap.LockBits();
                */
                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < bmp.Height; y++)
                {
                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    //  for (int x = 0; x < bmp.Width; x++)
                    for (int x = 0, x2 = 0; /*x < bmp/*lockBitmap*//*.Width; x++*/x < widthInBytes; x = x + bytesPerPixel, x2++)
                    {
                        if (s[y][x2])
                        {
                            currentLine[x] = 0;//(byte)oldBlue;
                            currentLine[x + 1] = 0;// (byte)oldGreen;
                            currentLine[x + 2] = 0;// (byte)oldRed;
                        }
                        // if (s[y][x]) /*bmp*/lockBitmap.SetPixel(x, y, Color.Black);
                    }
                }
                // lockBitmap.UnlockBits();
                bmp.UnlockBits(bitmapData);
                return (Bitmap)bmp;
            }
        }

        public static T[][] ArrayClone<T>(T[][] A)
        {
            return A.Select(a => a.ToArray()).ToArray();
        }

        public static int NumberOfZeroToOneTransitionFromP9(int x, int y, bool[][] s)       //ZhangWang, ZhangSuen
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];

            int A = Convert.ToInt32((!p2 && p3)) + Convert.ToInt32((!p3 && p4)) +
                    Convert.ToInt32((!p4 && p5)) + Convert.ToInt32((!p5 && p6)) +
                    Convert.ToInt32((!p6 && p7)) + Convert.ToInt32((!p7 && p8)) +
                    Convert.ToInt32((!p8 && p9)) + Convert.ToInt32((!p9 && p2));
            return A;
        }
        public static int NumberOfNonZeroNeighbors(int x, int y, bool[][] s) //ZhangWang, ZhangSuen
        {
            int count = 0;
            if (s[x - 1][y]) count++;
            if (s[x - 1][y + 1]) count++;
            if (s[x - 1][y - 1]) count++;
            if (s[x][y + 1]) count++;
            if (s[x][y - 1]) count++;
            if (s[x + 1][y]) count++;
            if (s[x + 1][y + 1]) count++;
            if (s[x + 1][y - 1]) count++;
            return count;
        }



        public static int[][] Image2Int(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            int[][] s = new int[bmp.Height][];
            for (int y = 0; y < bmp.Height; y++)
            {
                s[y] = new int[bmp.Width];
                for (int x = 0; x < bmp.Width; x++)
                    if (bmp.GetPixel(x, y).GetBrightness() < 0.3)
                        s[y][x] = 1;
                    else
                        s[y][x] = 0;
            }
            return s;        
        }

        public static Image Int2Image(int[][] r)
        {
            Bitmap bmp = new Bitmap(r[0].Length, r.Length);
            using (Graphics g = Graphics.FromImage(bmp)) g.Clear(Color.White);
            for (int y = 0; y < bmp.Height; y++)
                for (int x = 0; x < bmp.Width; x++)
                    if (r[y][x] == 1) bmp.SetPixel(x, y, Color.Black);

            return (Bitmap)bmp;
        }
    }
}
