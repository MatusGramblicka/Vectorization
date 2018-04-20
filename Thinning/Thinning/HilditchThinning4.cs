using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class HilditchThinning4       //http://cis.k.hosei.ac.jp/~wakahara/Hilditch.c
    {
        public const int BLACK = 0;
        public const int WHITE = 255;
        public const int GRAY = 128;

       static int func_nc8(int[] b)     /* connectivity detection for each point */
        {
            int[] n_odd = new int[4] { 1, 3, 5, 7 };  /* odd-number neighbors */
            int i, j, sum;           /* control variable */
            int[] d = new int[10];    /* control variable */

            for (i = 0; i </*=*/ 9; i++)
            {
                j = i;
                if (i == 8)
                    j = 1;

                if (Math.Abs(b[/*i*/j]) == 1) // if (Math.Abs((b + j)) == 1)
                {
                    d[i] = 1;
                }
                else
                {
                    d[i] = 0;
                }
            }
            sum = 0;
            for (i = 0; i < 4; i++)
            {
                j = n_odd[i];
                sum = sum + d[j] - d[j] * d[j + 1] * d[j + 2];
            }
            return (sum);
        }

       static public /*Bitmap*/bool[][] hilditch4(/*Bitmap image1,*/ /*Bitmap*/bool[][] image2)     /* thinning of binary image by Hilditch's algorithm */ /* WHITE --> 0, GRAY --> -1, BLACK --> 1 */ /* input image1[y][x] ===> output image2[y][x] */
        {
            int[,] offset = new int[9, 2] { { 0, 0 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } }; /* offsets for neighbors */
            int[] n_odd = new int[4] { 1, 3, 5, 7 };      /* odd-number neighbors */
            int px, py;                         /* X/Y coordinates  */
            int[] b = new int[9];                           /* gray levels for 9 neighbors */
            bool[] condition = new bool[6];                   /* valid for conditions 1-6 */
            int counter;                        /* number of changing points  */
            int i, x, y, copy, sum;             /* control variable          */
            /*
                        int x_size1 = image1.Width;
                        int y_size1 = image1.Height;
                        */
           // int y_size1 = image1.Width;
           // int x_size1 = image1.Height;

            int x_size1 = image2.Length;
            int y_size1 = image2[0].Length;


            Console.WriteLine(" Hilditch's thinning starts now.\n");
            /* initialization of arrays */
            /*
             int x_size2 = x_size1;
             int y_size2 = y_size1;
             for (y = 0; y < y_size2; y++)
             {
                 for (x = 0; x < x_size2; x++)
                 {
                     image2[y][x] = image1[y][x];
                 }
             }*/
            /* processing starts */
            do
            {
                counter = 0;
                for (y = 0; y < y_size1; y++)
                {
                    for (x = 0; x < x_size1; x++)
                    {
                        /* substitution of 9-neighbor gray values */
                        for (i = 0; i < 9; i++)
                        {
                            b[i] = 0;
                            px = x + offset[i, 0];
                            py = y + offset[i, 1];
                            if (px >= 0 && px < x_size1 && py >= 0 && py < y_size1)
                            {
                                //  if (image1.GetPixel(py, px) == Color.Black)
                                // if ((image2.GetPixel(py, px).R == BLACK & image2.GetPixel(py, px).G == BLACK & image2.GetPixel(py, px).B == BLACK))
                                if (image2[px][py] == true)
                                {
                                    b[i] = 1;
                                }
                                /*
                                                                else if (image2.GetPixel(py, px) == Color.Gray)
                                                                {
                                                                    b[i] = -1;
                                                                }
                                                                */
                            }
                        }
                        for (i = 0; i < 6; i++)
                        {
                            condition[i] = false;
                        }

                        /* condition 1: figure point */
                        if (b[0] == 1)
                            condition[0] = true;

                        /* condition 2: boundary point */
                        sum = 0;
                        for (i = 0; i < 4; i++)
                        {
                            sum = sum + 1 - Math.Abs(b[n_odd[i]]);
                        }
                        if (sum >= 1)
                            condition[1] = true;

                        /* condition 3: endpoint conservation */
                        sum = 0;
                        for (i = 1; i <= 8; i++)
                        {
                            sum = sum + Math.Abs(b[i]);
                        }
                        if (sum >= 2)
                            condition[2] = true;

                        /* condition 4: isolated point conservation */
                        sum = 0;
                        for (i = 1; i <= 8; i++)
                        {
                            if (b[i] == 1) sum++;
                        }
                        if (sum >= 1)
                            condition[3] = true;

                        /* condition 5: connectivity conservation */
                        if (func_nc8(b) == 1)
                            condition[4] = true;

                        /* condition 6: one-side elimination for line-width of two */
                        sum = 0;
                        for (i = 1; i <= 8; i++)
                        {
                            if (b[i] != -1)
                            {
                                sum++;
                            }
                            else
                            {
                                copy = b[i];
                                b[i] = 0;
                                if (func_nc8(b) == 1) sum++;
                                b[i] = copy;
                            }
                        }
                        if (sum == 8)
                            condition[5] = true;

                        /* final decision */
                        if (condition[0] && condition[1] && condition[2] && condition[3] && condition[4] && condition[5])
                        {
                            // image2.SetPixel(y, x, Color.Gray); /* equals -1 */
                            //image2.SetPixel(y, x, Color.White); 
                            image2[x][y] = false;
                            counter++;
                        }
                    } /* end of x */
                } /* end of y */
                /*
                if (counter != 0)
                {
                    for (y = 0; y < y_size1; y++)
                    {
                        for (x = 0; x < x_size1; x++)
                        {
                            if (image2.GetPixel(y, x) == Color.Gray)                               
                            image2.SetPixel(y, x, Color.White);
                        }
                    }
                }*/
            } while (counter != 0);

            return image2;
        }
    }
}
