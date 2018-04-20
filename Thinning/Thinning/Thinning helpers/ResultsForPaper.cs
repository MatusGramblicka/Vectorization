using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ResultsForPaper
    {
        public long pixelCount(Image image)
        {
            Bitmap bmp = new Bitmap(image);

            long count = 0;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (bmp.GetPixel(x, y).R == 0 & bmp.GetPixel(x, y).G == 0 & bmp.GetPixel(x, y).B == 0)
                    {
                         count++;                        
                    }
                }
            }
            return count;
        }

        public int Connectivity(Image image)
        {   
            Bitmap bmp = new Bitmap(image);

            int count = 0;

            for (int y = 1; y < image.Height; y++)
            {
                for (int x = 1; x < image.Width; x++)
                {
                    if (bmp.GetPixel(x, y).R == 0 & bmp.GetPixel(x, y).G == 0 & bmp.GetPixel(x, y).B == 0)      // black pixel
                    {
                        int BP = 0;

                        if (bmp.GetPixel(x - 1, y).R == 0 & bmp.GetPixel(x - 1, y).G == 0 & bmp.GetPixel(x - 1, y).B == 0) BP++;
                        if (bmp.GetPixel(x - 1, y + 1).R == 0 & bmp.GetPixel(x - 1, y + 1).G == 0 & bmp.GetPixel(x - 1, y + 1).B == 0) BP++;
                        if (bmp.GetPixel(x - 1, y - 1).R == 0 & bmp.GetPixel(x - 1, y - 1).G == 0 & bmp.GetPixel(x - 1, y - 1).B == 0) BP++;
                        if (bmp.GetPixel(x, y + 1).R == 0 & bmp.GetPixel(x, y + 1).G == 0 & bmp.GetPixel(x, y + 1).B == 0) BP++;
                        if (bmp.GetPixel(x, y - 1).R == 0 & bmp.GetPixel(x, y - 1).G == 0 & bmp.GetPixel(x, y - 1).B == 0) BP++;
                        if (bmp.GetPixel(x + 1, y).R == 0 & bmp.GetPixel(x + 1, y).G == 0 & bmp.GetPixel(x + 1, y).B == 0) BP++;
                        if (bmp.GetPixel(x + 1, y + 1).R == 0 & bmp.GetPixel(x + 1, y + 1).G == 0 & bmp.GetPixel(x + 1, y + 1).B == 0) BP++;
                        if (bmp.GetPixel(x + 1, y - 1).R == 0 & bmp.GetPixel(x + 1, y - 1).G == 0 & bmp.GetPixel(x + 1, y - 1).B == 0) BP++;

                        if (BP < 2)
                            count++;
                    }
                }
            }
            return count;
        }

        public int Sensitivity(Image image)
        {
            Bitmap bmp = new Bitmap(image);

            int count = 0;
            int P2, P3, P4, P5, P6, P7, P8, P9 = 0;

            for (int y = 1; y < image.Height; y++)
            {
                for (int x = 1; x < image.Width; x++)
                {
                    if (bmp.GetPixel(x, y).R == 0 & bmp.GetPixel(x, y).G == 0 & bmp.GetPixel(x, y).B == 0)      // black pixel
                    {

                        if (bmp.GetPixel(x - 1, y).R == 0 & bmp.GetPixel(x - 1, y).G == 0 & bmp.GetPixel(x - 1, y).B == 0) P8 = 1; else P8 = 0;
                        if (bmp.GetPixel(x - 1, y + 1).R == 0 & bmp.GetPixel(x - 1, y + 1).G == 0 & bmp.GetPixel(x - 1, y + 1).B == 0) P7 = 1; else P7 = 0;
                        if (bmp.GetPixel(x - 1, y - 1).R == 0 & bmp.GetPixel(x - 1, y - 1).G == 0 & bmp.GetPixel(x - 1, y - 1).B == 0) P9 = 1; else P9 = 0;
                        if (bmp.GetPixel(x, y + 1).R == 0 & bmp.GetPixel(x, y + 1).G == 0 & bmp.GetPixel(x, y + 1).B == 0) P6 = 1; else P6 = 0;
                        if (bmp.GetPixel(x, y - 1).R == 0 & bmp.GetPixel(x, y - 1).G == 0 & bmp.GetPixel(x, y - 1).B == 0) P2 = 1; else P2 = 0;
                        if (bmp.GetPixel(x + 1, y).R == 0 & bmp.GetPixel(x + 1, y).G == 0 & bmp.GetPixel(x + 1, y).B == 0) P4 = 1; else P4 = 0;
                        if (bmp.GetPixel(x + 1, y + 1).R == 0 & bmp.GetPixel(x + 1, y + 1).G == 0 & bmp.GetPixel(x + 1, y + 1).B == 0) P5 = 1; else P5 = 0;
                        if (bmp.GetPixel(x + 1, y - 1).R == 0 & bmp.GetPixel(x + 1, y - 1).G == 0 & bmp.GetPixel(x + 1, y - 1).B == 0) P3 = 1; else P3 = 0;

                        bool p2 = Convert.ToBoolean(P2); bool p3 = Convert.ToBoolean(P3); bool p4 = Convert.ToBoolean(P4); bool p5 = Convert.ToBoolean(P5);
                        bool p6 = Convert.ToBoolean(P6); bool p7 = Convert.ToBoolean(P7); bool p8 = Convert.ToBoolean(P8); bool p9 = Convert.ToBoolean(P9);

                        int A = Convert.ToInt32((!p2 && p3)) + Convert.ToInt32((!p3 && p4)) + Convert.ToInt32((!p4 && p5)) + Convert.ToInt32((!p5 && p6)) +
                                Convert.ToInt32((!p6 && p7)) + Convert.ToInt32((!p7 && p8)) + Convert.ToInt32((!p8 && p9)) + Convert.ToInt32((!p9 && p2));

                        if (A > 2)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        public double Thinness(Image image)
        {
            Bitmap bmp = new Bitmap(image);           

            int CP = 0;
            int Pi, P2, P3, P4, P5, P6, P7, P8, P9 = 0;
            double T = 0;

            for (int y = 1; y < image.Height-1; y++)
            {
                for (int x = 1; x < image.Width - 1; x++)
                {
                    if (bmp.GetPixel(x, y).R == 0 & bmp.GetPixel(x, y).G == 0 & bmp.GetPixel(x, y).B == 0) Pi = 1; else Pi = 0;
                    if (bmp.GetPixel(x, y - 1).R == 0 & bmp.GetPixel(x, y - 1).G == 0 & bmp.GetPixel(x, y - 1).B == 0) P2 = 1; else P2 = 0;
                    if (bmp.GetPixel(x+1, y - 1).R == 0 & bmp.GetPixel(x+1, y - 1).G == 0 & bmp.GetPixel(x+1, y - 1).B == 0) P3 = 1; else P3 = 0;
                    if (bmp.GetPixel(x + 1, y).R == 0 & bmp.GetPixel(x + 1, y).G == 0 & bmp.GetPixel(x + 1, y).B == 0) P4 = 1; else P4 = 0;
                    if (bmp.GetPixel(x + 1, y + 1).R == 0 & bmp.GetPixel(x + 1, y + 1).G == 0 & bmp.GetPixel(x + 1, y + 1).B == 0) P5 = 1; else P5 = 0;
                    if (bmp.GetPixel(x, y + 1).R == 0 & bmp.GetPixel(x, y + 1).G == 0 & bmp.GetPixel(x, y + 1).B == 0) P6 = 1; else P6 = 0;
                    if (bmp.GetPixel(x - 1, y + 1).R == 0 & bmp.GetPixel(x - 1, y + 1).G == 0 & bmp.GetPixel(x - 1, y + 1).B == 0) P7 = 1; else P7 = 0;
                    if (bmp.GetPixel(x - 1, y).R == 0 & bmp.GetPixel(x - 1, y).G == 0 & bmp.GetPixel(x - 1, y).B == 0) P8 = 1; else P8 = 0;
                    if (bmp.GetPixel(x - 1, y - 1).R == 0 & bmp.GetPixel(x - 1, y - 1).G == 0 & bmp.GetPixel(x - 1, y - 1).B == 0) P9 = 1; else P9 = 0;
                    
                     // CP += (Pi * P9 * P2) + (Pi * P9 * P8) + (Pi * P8 * P7) + (Pi * P7 * P6);   
                    CP += (Pi * P2 * P9) + (Pi * P8 * P9) + (Pi * P7 * P8) + (Pi * P6 * P7) + (Pi * P2 * P3) + (Pi * P3 * P4) + (Pi * P4 * P5) + (Pi * P5 * P6);
                }
            }

            double TM2 = /*4 **/ Math.Pow(Math.Max(image.Width, image.Height) - 1, 2)/4;

            return T = 1 - CP / TM2;
        }
    }
}
