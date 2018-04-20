using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ArabicParallelThinningAlgorithm
    {
        public static bool[][] ArabicParallelThinningAlg(bool[][] s)                // http://www.ancient-asia-journal.com/articles/10.5334/aa.06114/
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = stepArabicParallelThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepArabicParallelThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
                count += stepArabicParallelThinning(3, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
                count += stepArabicParallelThinning(4, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }



        static int stepArabicParallelThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                    if (ArabicParallelThinningAlg(a, b, temp, stepNo))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool ArabicParallelThinningAlg(int x, int y, bool[][] s, int stepNo)
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

            if (bp1 >= 2 && bp1 <= 6) //2nd condition
            {
                if (ThinningHelper.NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
                {
                    if (stepNo == 1)
                    if (!((p2 && p4) && p6))
                    {
                        if (!((p4 && p6) && p8))
                        {
                            return true;
                        }
                    }

                    if (stepNo == 2)
                    if (!((p2 && p6) && p8))
                    {
                        if (!((p4 && p6) && p8))
                        {
                            return true;
                        }
                    }

                    if (stepNo == 3)
                    if (!((p2 && p4) && p8))
                    {
                        if (!((p2 && p6) && p8))
                        {
                            return true;
                        }
                    }

                    if (stepNo == 4)
                    if (!((p2 && p4) && p6))
                    {
                        if (!((p2 && p4) && p8))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}

