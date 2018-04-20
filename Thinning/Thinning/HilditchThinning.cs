using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class HilditchThinning
    {
        public static bool[][] HilditchThinningAlg(bool[][] s)      //   http://stackoverflow.com/questions/20245003/zhang-suen-thinning-algorithm-c-sharp
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = stepHilditchThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepHilditchThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }



        static int stepHilditchThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 2; a < temp.Length - 1; a++)
            {
                for (int b = 2; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                        if (HilditchThinningAlg(a, b, temp))
                        {
                            // still changes happening?
                            if (s[a][b]) count++;
                            s[a][b] = false;
                        }
                }
            }
            return count;
        }

        static bool HilditchThinningAlg(int x, int y, bool[][] s)
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
                    if (!((p2 && p4) && p8) | (ThinningHelper.NumberOfZeroToOneTransitionFromP9(x, y - 1, s) != 1))
                    {
                        if (!((p2 && p4) && p6) | (ThinningHelper.NumberOfZeroToOneTransitionFromP9(x + 1, y, s) != 1))
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
