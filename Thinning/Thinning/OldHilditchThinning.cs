using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    class OldHilditchThinning
    {
        public static int[][] HilditchThinningAlg(int[][] binaryImage) //https://nayefreza.wordpress.com/2013/05/11/hilditchs-thinning-algorithm-java-implementation/
        {
            Benchmark.Start();
            int a, b;
            bool hasChange;
            do
            {
                hasChange = false;
                for (int y = 1; y + 1 < binaryImage.Length; y++)
                {
                    for (int x = 1; x + 1 < binaryImage[y].Length; x++)
                    {
                        if (binaryImage[y][x]==1)
                        {
                        a = getA(binaryImage, y, x);
                        b = getB(binaryImage, y, x);
                        if (binaryImage[y][x] == 1 & 2 <= b & b <= 6 & a == 1 & 
                            ((binaryImage[y - 1][x] * binaryImage[y][x + 1] * binaryImage[y][x - 1] == 0) || (getA(binaryImage, y - 1, x) != 1)) &
                            ((binaryImage[y - 1][x] * binaryImage[y][x + 1] * binaryImage[y + 1][x] == 0) || (getA(binaryImage, y, x + 1) != 1)))
                        {
                            binaryImage[y][x] = 0;
                            hasChange = true;
                        }
                    }
                    }
                }
            } while (hasChange);
            Benchmark.End();
            return binaryImage;
        }

        private static int getA(int[][] binaryImage, int y, int x)
        {

            int count = 0;
            //p2 p3
            if (y - 1 >= 0 && x + 1 < binaryImage[y].Length && binaryImage[y - 1][x] == 0 && binaryImage[y - 1][x + 1] == 1)
            {
                count++;
            }
            //p3 p4
            if (y - 1 >= 0 && x + 1 < binaryImage[y].Length && binaryImage[y - 1][x + 1] == 0 && binaryImage[y][x + 1] == 1)
            {
                count++;
            }
            //p4 p5
            if (y + 1 < binaryImage.Length && x + 1 < binaryImage[y].Length && binaryImage[y][x + 1] == 0 && binaryImage[y + 1][x + 1] == 1)
            {
                count++;
            }
            //p5 p6
            if (y + 1 < binaryImage.Length && x + 1 < binaryImage[y].Length && binaryImage[y + 1][x + 1] == 0 && binaryImage[y + 1][x] == 1)
            {
                count++;
            }
            //p6 p7
            if (y + 1 < binaryImage.Length && x - 1 >= 0 && binaryImage[y + 1][x] == 0 && binaryImage[y + 1][x - 1] == 1)
            {
                count++;
            }
            //p7 p8
            if (y + 1 < binaryImage.Length && x - 1 >= 0 && binaryImage[y + 1][x - 1] == 0 && binaryImage[y][x - 1] == 1)
            {
                count++;
            }
            //p8 p9
            if (y - 1 >= 0 && x - 1 >= 0 && binaryImage[y][x - 1] == 0 && binaryImage[y - 1][x - 1] == 1)
            {
                count++;
            }
            //p9 p2
            if (y - 1 >= 0 && x - 1 >= 0 && binaryImage[y - 1][x - 1] == 0 && binaryImage[y - 1][x] == 1)
            {
                count++;
            }

            return count;
        }

        private static int getB(int[][] binaryImage, int y, int x)
        {
            return binaryImage[y - 1][x] + binaryImage[y - 1][x + 1] + binaryImage[y][x + 1] + binaryImage[y + 1][x + 1] + binaryImage[y + 1][x] + binaryImage[y + 1][x - 1] + binaryImage[y][x - 1] + binaryImage[y - 1][x - 1];
        }
    }
}
