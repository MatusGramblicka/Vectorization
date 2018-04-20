using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class GuoHallThinning : Abstract3x3NeighbourhoodThinning
    {
        //http://www.programcreek.com/java-api-examples/index.php?source_dir=knip-master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/GuoHallAlgorithm.java
        //https://github.com/knime-ip/knip/blob/master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/GuoHallAlgorithm.java
        private int iteration = 0;

        /**
         * Create a new Guo-Hall thinning strategy. The passed boolean will represent the foreground-value of the image. 
         * 
         * @param foreground Value determining the boolean value of foreground pixels. 
         */

        public GuoHallThinning(bool foreground) : base(foreground)
        {
            //  super(foreground); 
        }

        /**
         * {@inheritDoc} 
         */

        public bool[][] GuoHallThinningAlg(bool[][] s)
        {
            Benchmark.Start();
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 

            int count = 0;
            do  // the missing iteration
            {
                count = stepGuoHallThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepGuoHallThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
                iteration++;
            }
            while (count > 0);
            Benchmark.End();
            return s;
        }

        public int stepGuoHallThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                    if (removePixel(a, b, temp)/*(ZhangSuenThinningAlg(a, b, temp, stepNo == 2)*/)
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        public bool removePixel(int x, int y, bool[][] temp/*,long[] position, RandomAccessible<BitType> accessible*/)
        {
            /*// Setup
           RandomAccess<BitType> access = accessible.randomAccess();
           access.setPosition(position);
           */

            // bool[] vals = getNeighbourhood(access);
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

            // Check for foreground pixels in each sector. 
            int sectorsOne = countSectors(vals, -1);
            int sectorsTwo = countSectors(vals, 1);
            int minSectors = Math.Min(sectorsOne, sectorsTwo);
            if (!((2 <= minSectors) && (minSectors <= 3)))
            {
                return false;
            }

            bool oddEvenCheck;

            // Depending on the step in the cycle, we need to perform different calculations. 
            if (iteration % 2 == 1)
            {
                oddEvenCheck = (vals[1] == m_foreground || vals[2] == m_foreground || vals[4] == m_background) &&/* ||*/ vals[3] == m_foreground;
            }
            else
            {
                oddEvenCheck = (vals[5] == m_foreground || vals[6] == m_foreground || vals[8] == m_background) && vals[7] == m_foreground;
            }
            if (oddEvenCheck)
            {
                return false;
            }
            return true;
        }

        // Count the foreground pixels in each of the four sectors. Depending on the offset, the sector borders are 
        // rotated by 45 degrees. 
        private int countSectors(bool[] vals, int offset)
        {
            int res = 0;
            for (int i = 1; i < vals.Length - 1; i = i + 2)
            {
                if (i + offset == 0)
                {
                    if (vals[1] == m_foreground || vals[8] == m_foreground)
                    {
                        ++res;
                    }
                }
                else if (vals[i] == m_foreground || vals[i + offset] == m_foreground)
                {
                    ++res;

                }
            }
            return res;
        }


        /**
         * {@inheritDoc} 
         */

        public void afterCycle()
        {
            ++iteration;
        }

        public int getIterationsPerCycle()
        {
            return 2;
        }

        /**
         * {@inheritDoc} 
         */
        /*
        public ThinningStrategy copy()
        {
            return new GuoHallAlgorithm(m_foreground);
        }
        */
        /*
        public static T[][] ArrayClone<T>(T[][] A)
        {
            return A.Select(a => a.ToArray()).ToArray();
        }
        */
    }
}

