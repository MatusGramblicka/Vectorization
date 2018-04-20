using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ProposedThinning
    {
        public static bool[][] ProposedKwonWoongKangThinningAlg(bool[][] s)
        {
            Benchmark.Start();      // time counting
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 
            int count = 0;

            do  // the first pass 
            {
                count = stepProposedKwonWoongKangThinningThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepProposedKwonWoongKangThinningThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);

            do  // the second pass 
            {
                count = pass2ProposedKwonWoongKangThinningThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..              
            }
            while (count > 0);

            do  // the third pass
            {
                count = pass3ProposedKwonWoongKangThinningThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..    
                count = pass3ProposedKwonWoongKangThinningThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..   
                count = pass3ProposedKwonWoongKangThinningThinning(3, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..   
                count = pass3ProposedKwonWoongKangThinningThinning(4, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..   
            }
            while (count > 0);

            Benchmark.End();

            return s;
        }

        private static int pass3ProposedKwonWoongKangThinningThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 2; a < temp.Length - 2; a++)
            {
                for (int b = 2; b < temp[0].Length - 2; b++)
                {
                    if (s[a][b])            // Am I on black pixel?
                        if (ProposedskuskaAlgThinningAlg(a, b, temp, stepNo))
                        {
                            if (s[a][b]) count++;       // still changes happening?
                            s[a][b] = false;
                        }
                }
            }
            return count;
        }

        static bool ProposedskuskaAlgThinningAlg(int x, int y, bool[][] s, int stepNo)
        {
            bool p2 = s[x][y - 1];
            bool p3 = s[x + 1][y - 1];
            bool p4 = s[x + 1][y];
            bool p5 = s[x + 1][y + 1];
            bool p6 = s[x][y + 1];
            bool p7 = s[x - 1][y + 1];
            bool p8 = s[x - 1][y];
            bool p9 = s[x - 1][y - 1];
            bool p11 = s[x][y - 2];
            bool p15 = s[x + 2][y];
            bool p19 = s[x][y + 2];
            bool p23 = s[x - 2][y];

            if (stepNo == 1)
                if (!p11 && p2 && p4 && !p15 && !p7)    //south west corner
                {
                    return true;
                }

            if (stepNo == 2)
                if (!p15 && p4 && p6 && !p19 && !p9)    //north west corner
                {
                    return true;
                }

            if (stepNo == 3)
                if (!p19 && p6 && p8 && !p23 && !p3)    //north east corner
                {
                    return true;
                }

            if (stepNo == 4)
                if (!p23 && p8 && p2 && !p11 && !p5)    //south east corner    
                {
                    return true;
                }

            return false;
        }

        private static int pass2ProposedKwonWoongKangThinningThinning(bool[][] temp, bool[][] s)
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
                            if (s[x][y]) count++;           // still changes happening?
                            s[x][y] = false;
                        }
                    }
                }
            }
            return count;
        }

        static int stepProposedKwonWoongKangThinningThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])            // Am I on black pixel?
                        if (ProposedKwonWoongKangThinningThinningAlg(a, b, temp, stepNo == 2))
                        {
                            if (s[a][b]) count++;       // still changes happening?
                            s[a][b] = false;
                        }
                }
            }
            return count;
        }

        static bool ProposedKwonWoongKangThinningThinningAlg(int x, int y, bool[][] s, bool even)
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
