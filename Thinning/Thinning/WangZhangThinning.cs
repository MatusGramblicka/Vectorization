using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class WangZhangThinning      // http://shodhganga.inflibnet.ac.in/bitstream/10603/3466/10/10_chapter%202.pdf  str. 7
    {                                   //  Zhang, Y. Y.;  Wang P. S. P. A fast and flexible thinning algorithm. IEEE Transactions on Computers Year: 1989, Volume: 38, Issue: 5 Pages: 741 - 745, DOI: 10.1109/12.24276       
        public static bool[][] WangZhangThinningAlg(bool[][] s) 
        {
            Benchmark.Start();                                   
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;
            do  // the missing iteration
            {
                count = stepWangZhangThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepWangZhangThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }



        public static int stepWangZhangThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < s.Length-1; a++)
            {
                for (int b = 1; b < s[0].Length-1; b++)
                {
                    if (s[a][b])
                    if (WangZhangThinningAlg(a, b, temp))
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

        static bool WangZhangThinningAlg(int x, int y, bool[][] s)
        {

           // if (x > 0 && x < s.Length - 2 && y < s[0].Length - 1 && y > 1 && s[x][y])//bounds and 1st condition p1 = 1
         //   {
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
                    if ((ThinningHelper.NumberOfZeroToOneTransitionFromP9(x, y, s) == 1) | (p2 == false & p3 == false & p4 == false & p7 == false & p6 == true & p8 == true) | (p4 == false & p5 == false & p6 == false & p9 == false & p8 == true & p2 == true))
                    {
                        if ((p2 == false & (p8 == false | p4 == false | p6 == false)) | (p4 == false & (p6 == false | p2 == false | p8 == false)))
                        {                           
                                return true;                            
                        }
                    }
                }

           // }
            return false;
        }
    }
}
