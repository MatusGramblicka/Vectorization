using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ModifiedAlgorithmZhangWangThinning
    {
        public static bool[][] ModifiedThinningalgorithmZhangWangThinningAlg(bool[][] s) //https://gist.github.com/anonymous/fe757151d37a7f229386#file-thinning-algs
      {                                                                                  // http://www.uel.br/pessoal/josealexandre/stuff/thinning/ftp/zhang-wang.pdf
          Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = stepZhangWangThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepZhangWangThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }



        public static int stepZhangWangThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 0; a < s.Length; a++)
            {
                for (int b = 0; b < s[0].Length; b++)
                {
                    if (s[a][b])
                    if (ZhangWangThinningAlg(a, b, temp))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        // temp[a][b] = false;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        static bool ZhangWangThinningAlg(int x, int y, bool[][] s)
        {

            if (x > 0 && x < s.Length - 2 && y < s[0].Length - 1 && y > 1 && s[x][y])//bounds and 1st condition p1 = 1
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

                if (bp1 >= 2 && bp1 <= 6)//2nd condition
                {
                    if (ThinningHelper.NumberOfZeroToOneTransitionFromP9(x, y, s) == 1)
                    {
                        if (!((p2 && p4) && p8) || s[x][y - 2])
                        {
                            if (!((p2 && p4) && p6) || s[x + 2][y])
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
