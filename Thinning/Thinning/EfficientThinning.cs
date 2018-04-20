using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class EfficientThinning
    {
        public static bool[][] EfficientThinningAlg(bool[][] s)      // http://www.academia.edu/2075831/An_Efficient_Parallel_Thinning_Algorithm_using_One_and_Two_Sub-Iterations
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = stepEfficientThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepEfficientThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }



        static int stepEfficientThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                        if (EfficientThinningAlg(a, b, temp, stepNo == 2))
                        {
                            // still changes happening?
                            if (s[a][b]) count++;
                            s[a][b] = false;
                        }
                }
            }
            return count;
        }

        static bool EfficientThinningAlg(int x, int y, bool[][] s, bool even)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];

            if (even)
            {
                if (((p4 & !p8 & ((!p9 & !p2 & p6) | (p2 & !p6 & !p7))) | (p2 & p3 & p4 & ((p5 & p6 & !p8) | (p9 & !p6 & p8))) | (!p6 & !p7 & !p8 & (((!p9 & !p2 & p4) & (p5 | p3)) | ((p2 & !p4 & !p5) & (p3 | p9))))))
                {
                    return true;
                }
            }
            else
            {
                if (((!p4 & p8 & ((p2 & !p5 & !p6) | (!p2 & !p3 & p6))) | (p6 & p7 & p8 & ((p9 & p2 & !p4) | (!p2 & p4 & p5))) | (!p2 & !p3 & !p4 & (((!p5 & !p6 & p8) & (p9 | p7)) | ((!p9 & p6 & !p8) & (p7 | p5))))))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
