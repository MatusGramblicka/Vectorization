using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class StentifordThinning
    {
        public static bool[][] skuskaAlg(bool[][] s)
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 

            int count = 0;
            do  // the missing iteration
            {
                count = skuskaAlgThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += skuskaAlgThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
                count += skuskaAlgThinning(3, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
                count += skuskaAlgThinning(4, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!              
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }
        
        static int skuskaAlgThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                    if (skuskaAlgThinningAlg(a, b, temp, stepNo))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool skuskaAlgThinningAlg(int x, int y, bool[][] s, int stepNo)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];

            int bp1 = ThinningHelper.NumberOfNonZeroNeighbors(x, y, s);

            if (/*!endPoint(x, y, s)*/ bp1 != 1) //2nd condition
            {
                if (removePixel(x, y, s))
                {
                    if (stepNo == 1)
                        if (!p2 && p6)
                        {
                            return true;
                        }

                    if (stepNo == 2)
                        if (!p8 && p4)
                        {
                            return true;
                        }

                    if (stepNo == 3)
                        if (!p6 && p2)
                        {
                            return true;
                        }

                    if (stepNo == 4)
                        if (!p4 && p8)
                        {
                            return true;
                        }
                }
            }
            return false;
        }
        
        public static bool removePixel(int x, int y, bool[][] temp)
        {
            bool m_background = false;
            bool m_foreground = true;

            bool[] vals = getNeighbourhood(x, y, temp);

            // First we need to count the amount of connected neighbours in the vicinity. 
            int simpleConnectedNeighbours = 0;
            if (vals[1] == m_background && (vals[2] == m_foreground || vals[3] == m_foreground))
            {
                ++simpleConnectedNeighbours;
            }
            if (vals[3] == m_background && (vals[4] == m_foreground || vals[5] == m_foreground))
            {
                ++simpleConnectedNeighbours;
            }
            if (vals[5] == m_background && (vals[6] == m_foreground || vals[7] == m_foreground))
            {
                ++simpleConnectedNeighbours;
            }
            if (vals[7] == m_background && (vals[8] == m_foreground || vals[1] == m_foreground))
            {
                ++simpleConnectedNeighbours;
            }
            // First condition: Exactly one simple connected neighbour. 
            if (simpleConnectedNeighbours != 1)
            {
                return false;
            }
            else
                return true;
        }       

        protected static bool[] getNeighbourhood(/*RandomAccess<BitType> access*/int x, int y, bool[][] temp)
        {
            bool[][] s = temp;

            bool[] vals = new bool[9];

            // vals[0] = access.get().get();// tento je zbytočný

            vals[1] = s[x][y - 1];
            /*
            access.move(-1, 1);
            vals[1] = access.get().get();
            */
            vals[2] = s[x + 1][y - 1];
            /*
            access.move(1, 0);
            vals[2] = access.get().get();
            */
            vals[3] = s[x + 1][y];
            /*
            access.move(1, 1);
            vals[3] = access.get().get();
            */
            vals[4] = s[x + 1][y + 1];
            /*
            access.move(1, 1);
            vals[4] = access.get().get();
            */
            vals[5] = s[x][y + 1];
            /*
            access.move(-1, 0);
            vals[5] = access.get().get();
            */
            vals[6] = s[x - 1][y + 1];
            /*
            access.move(-1, 0);
            vals[6] = access.get().get();
            */
            vals[7] = s[x - 1][y];
            /*
            access.move(-1, 1);
            vals[7] = access.get().get();
            */
            vals[8] = s[x - 1][y - 1];
            /*
            access.move(-1, 1);
            vals[8] = access.get().get();
            */
            return vals;
        }



        private static bool endPoint(int x, int y,/*Raster*/bool[][] src)
        {
            int pixel = get(src, x, y);
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

        private static int get(/*Raster*/bool[][] src, int x, int y)
        {
            // Color t = src.GetPixel(x, y);           

            if (src[x][y] == false)
                return 1;
            else
                return 0;
        }

        // Return the k indexed neighbour of a given pixel (x, y). 
        // The k indexed neighbour table is given as below: 
        // 4 3 2 
        // 5 0 1 
        // 6 7 8 
        private static int neighbour(int x, int y, int k,/*Raster*/ bool[][] src)
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
            else if (x >= src.Length) x = src.Length - 1;

            if (y < 0) y = 0;
            else if (y >= src[0].Length) y = src[0].Length - 1;

            return get(src, x, y);
        }
    }
}
