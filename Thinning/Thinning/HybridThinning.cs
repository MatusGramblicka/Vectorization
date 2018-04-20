using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class HybridThinning
    {
        public static bool[][] HybridAlg(bool[][] s)            //https://www.scribd.com/document/116907201/A-Novel-Embedded-Hybrid-Thinning-Algorithm-For
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 

            int count = 0;
            do  // the missing iteration
            {
                count = HybridAlgThinning(1, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += HybridAlgThinning(2, temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!                       
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }

        static int HybridAlgThinning(int stepNo, bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                        if (HybridAlgThinningAlg(a, b, temp, stepNo))
                        {
                            // still changes happening?
                            if (s[a][b]) count++;
                            s[a][b] = false;
                        }
                }
            }
            return count;
        }    
     

        static bool HybridAlgThinningAlg(int x, int y, bool[][] s, int stepNo)
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
                if (removePixel(x, y, s))
                {
                    if (stepNo == 1)
                        if ((p8 && !p4) | (!p2 && p6) | (!p8 && p4))
                        {
                            if ((!p2 && p6) | (p2 && !p6) | (!p8 && p4))
                            {
                                return true;
                            }
                        }

                    if (stepNo == 2)
                        if ((p8 && !p4) | (!p2 && p6) | (!p6 && p2))
                        {
                            if ((p8 && !p4) | (p2 && !p6) | (!p8 && p4))
                            {
                                return true;
                            }
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
    }
}
