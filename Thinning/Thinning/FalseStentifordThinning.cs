using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class FalseStentifordThinning  //http://www.programcreek.com/java-api-examples/index.php?source_dir=ellipsedetector-master/src/robo/vision/StentifordThinningOpImage.java
    {
       static int width;
       static int height;
       static int WHITE = 255;       
        //int BLACK = 0;
        Color BLACK = Color.Black;

        public static Bitmap StentifordThinningAlg(Bitmap src, Bitmap dst)
        {
            // Raster src  = srcarr[0]; 
            width = src.Width;
            height = src.Height;

            bool[][] t = ThinningHelper.Image2Bool(src);

            return thinning(src, t, dst);
        }

        // Return the k indexed neighbour of a given pixel (x, y). 
        // The k indexed neighbour table is given as below: 
        // 4 3 2 
        // 5 0 1 
        // 6 7 8 
        private static int neighbour(int x, int y, int k,/*Raster*/ Bitmap src)
        {            
            switch (k)
            {
                case 0:
                    break;
                case 1:
                    x++;
                    break;
                case 2:
                    x++;
                    y--;
                    break;
                case 3:
                    y--;
                    break;
                case 4:
                    x--;
                    y--;
                    break;
                case 5:
                    x--;
                    break;
                case 6:
                    x--;
                    y++;
                    break;
                case 7:
                    y++;
                    break;
                case 8:
                    x++;
                    y++;
                    break;

            }
            if (x < 0) x = 0;
            else if (x >= width) x = width - 1;

            if (y < 0) y = 0;
            else if (y >= height) y = height - 1;
                       
            return get(src, x, y, 0);
        }

        private static int connectivity(int x, int y,/*Raster*/Bitmap src)
        {            
            // As in paper http://www.eng.fiu.edu/me/robotics/elib/am_st_fiu_ppr_2000.pdf, 0 is object value (foreground) and 1 is background value.
            // This is opposite  from the binarize operation in JAI where equation dst(x, y) = src(x, y) >= threshold ? 1 : 0; is used. Hence, we will just invert the value to fit JAI. 
            int p1 = neighbour(x, y, 1, src) == 1 ? 0 : 1;
            int p2 = neighbour(x, y, 2, src) == 1 ? 0 : 1;
            int p3 = neighbour(x, y, 3, src) == 1 ? 0 : 1;
            int p4 = neighbour(x, y, 4, src) == 1 ? 0 : 1;
            int p5 = neighbour(x, y, 5, src) == 1 ? 0 : 1;
            int p6 = neighbour(x, y, 6, src) == 1 ? 0 : 1;
            int p7 = neighbour(x, y, 7, src) == 1 ? 0 : 1;
            int p8 = neighbour(x, y, 8, src) == 1 ? 0 : 1;

           // if ((p1 - (p1 * p2 * p3) + p2 - (p2 * p3 * p4) + p3 - (p3 * p4 * p5) + p4 - (p4 * p5 * p6) + p5 - (p5 * p6 * p7) + p6 - (p6 * p7 * p8) + p7 - (p7 * p8 * p1) + p8 - (p8 * p1 * p2)) == 4)

           // return p1 - (p1 * p2 * p3) + p2 - (p2 * p3 * p4) + p3 - (p3 * p4 * p5) + p4 - (p4 * p5 * p6) + p5 - (p5 * p6 * p7) + p6 - (p6 * p7 * p8) + p7 - (p7 * p8 * p1) + p8 - (p8 * p1 * p2);

            return p1 * (1 - p2 * p3) + p3 * (1 - p4 * p5) + p5 * (1 - p6 * p7) + p7 * (1 - p8 * p1);       // vzorec je spravny ale ak n razi na bielu malo by byt 1 a cierna 0
        }

        private static bool endPoint(int x, int y,/*Raster*/Bitmap src)
        {         
            int pixel = get(src, x, y, 0);
            int count = 0;

            if (pixel == neighbour(x, y, 1, src)) count++;
            if (pixel == neighbour(x, y, 2, src)) count++;
            if (pixel == neighbour(x, y, 3, src)) count++;
            if (pixel == neighbour(x, y, 4, src)) count++;
            if (pixel == neighbour(x, y, 5, src)) count++;
            if (pixel == neighbour(x, y, 6, src)) count++;
            if (pixel == neighbour(x, y, 7, src)) count++;
            if (pixel == neighbour(x, y, 8, src)) count++;

            return count == 1;
        }

        private static Bitmap thinning(Bitmap src, bool[][] t, Bitmap dst)
        {
            int[] foregroundNeighbour = { 7, 1, 3, 5 };
            int[] backgroundNeighbour = { 3, 5, 7, 1 };
          
            bool[][] m = new bool[width][];
            
            for (int x = 0; x < m.Length; x++)
            {
                m[x] = new bool[height];
            }

            bool remain = true;

            for (int i = 0; i < m.Length; i++)
            {
                for (int j = 0; j < m[i].Length; j++)
                {                   
                    m[i][j] = false;  // uneraseable 
                }
            }

            while (remain)
            {
                remain = false;
                for (int k = 0; k < foregroundNeighbour.Length; k++)
                {
                    // We are not interest at the image boundary. 
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {                          
                            int p = get(src, x, y, 0);
                                                      
                            if (p == 1)
                            {
                                int p1 = neighbour(x, y, foregroundNeighbour[k], src);
                                int p2 = neighbour(x, y, backgroundNeighbour[k], src);

                                // Pattern matched.                               
                                if (p1 == 1 && p2 == 0)
                                {
                                    if (!endPoint(x, y, src) && connectivity(x, y, src) == 1)
                                    {
                                        // Mark for deletion.                                        
                                        m[x][y] = true; // eraseable 
                                        //  remain = true;
                                    }
                                }
                            }
                        }
                    }

                    // We are not interest at the image boundary. 
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                          
                            if (m[x][y])
                            {                                
                                setBlack(dst, x, y);                               
                                m[x][y] = false;
                            }
                            else
                            {
                                setWhite(dst, x, y);
                            }
                        }
                    }
                }
            }
            //pictureBox2.Image = dst;
            return dst;
        }

        private static void setWhite(/*WritableRaster*/Bitmap dst, int i, int j)
        {
            dst.SetPixel(i, j, Color.White);
        }

        private static void setBlack(/*WritableRaster*/Bitmap dst, int i, int j)
        {
            dst.SetPixel(i, j, Color.Black);
        }

        private static int get(/*Raster*/Bitmap src, int x, int y, int b)
        {
           // Color t = src.GetPixel(x, y);           

            if ((src.GetPixel(x, y).R == WHITE & src.GetPixel(x, y).G == WHITE & src.GetPixel(x, y).B == WHITE))
                return 1;
            else
                return 0;
        }
    }
}
