using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class KwonWoongKangThinning              //http://shodhganga.inflibnet.ac.in/bitstream/10603/3466/10/10_chapter%202.pdf   str. 13
    {
        public static bool[][] KwonWoongKangThinningAlg(bool[][] s)
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the first pass
            {
                count = stepKwonWoongKangThinningThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepKwonWoongKangThinningThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);

            // pokracovanie druha podmienka

            do  // the second pass
            {
                count = passKwonWoongKangThinningThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..              
            }
            while (count > 0);
            Benchmark.End();

            return s;
        }

        private static int passKwonWoongKangThinningThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int x = 1; x < temp.Length - 1; x++)
            {
                for (int y = 1; y < temp[0].Length - 1; y++)
                {
                    if (s[x][y])
                    {
                        bool p2 = s[x][y - 1];
                        bool p3 = s[x + 1][y - 1];
                        bool p4 = s[x + 1][y];
                        bool p5 = s[x + 1][y + 1];
                        bool p6 = s[x][y + 1];
                        bool p7 = s[x - 1][y + 1];
                        bool p8 = s[x - 1][y];
                        bool p9 = s[x - 1][y - 1];

                        if (((p9 == true) & (p8 == true) & (p6 == true) & (p3 == false)) | ((p3 == true) & (p4 == true) & (p6 == true) & (p9 == false)) | ((p5 == true) & (p6 == true) & (p8 == true) & (p3 == false)) | ((p4 == true) & (p6 == true) & (p7 == true) & (p9 == false)))
                        {
                            // still changes happening?
                            if (s[x][y]) count++;
                            s[x][y] = false;
                        }
                    }
                }
            }
            return count;
        }

        static int stepKwonWoongKangThinningThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                    if (KwonWoongKangThinningThinningAlg(a, b, temp, stepNo == 2))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool KwonWoongKangThinningThinningAlg(int x, int y, bool[][] s, bool even)
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

            if (ThinningHelper.NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
            {
                if (even)
                {
                    if (bp1 >= 3 && bp1 <= 6)
                    {
                        if (!((p2 && p4) && p8))
                        {
                            if (!((p2 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (bp1 >= 2 && bp1 <= 6)
                    {
                        if (!((p2 && p4) && p6))
                        {
                            if (!((p4 && p6) && p8))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }
    }
}
