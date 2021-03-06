﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    class ZhangSuenThinning2 : Abstract3x3NeighbourhoodThinning     //http://www.programcreek.com/java-api-examples/index.php?source_dir=knip-master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/ZhangSuenAlgorithm.java
    {                                                               //https://github.com/knime-ip/knip/blob/master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/ZhangSuenAlgorithm.java
        private int iteration = 0;

        /**
         * Create a new Zhang-Suen thinning strategy. The passed boolean will represent the foreground-value of the image. 
         * 
         * @param foreground Value determining the boolean value of foreground pixels. 
         */
        public ZhangSuenThinning2(bool foreground) : base(foreground)
        {
            // super(foreground); 
        }

        /**
         * {@inheritDoc} 
         */

        public bool[][] ZhangSuen2ThinningAlg(bool[][] s)
        {
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 


            int count = 0;
            do  // the missing iteration
            {
                count = stepZhangSuen2Thinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepZhangSuen2Thinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);

            return s;
        }

        public int stepZhangSuen2Thinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
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



        public bool removePixel(int x, int y, bool[][] temp /*,long[] position, final RandomAccessible<BitType> accessible*/)
        {
            /*
                   // Setup 
                   RandomAccess<BitType> access = accessible.randomAccess(); 
                   access.setPosition(position); 
            */
            // boolean[] vals = getNeighbourhood(access); 
            bool[] vals = getNeighbourhood(x, y, temp);

            // First two conditions are similar to Hilditch-Thinning. 
            int numForeground = 0;

            for (int i = 1; i < vals.Length; ++i)
            {
                if (vals[i] == m_foreground)
                {
                    ++numForeground;
                }
            }

            if (!(2 <= numForeground && numForeground <= 6))
            {
                return false;
            }
            int numPatternSwitches = findPatternSwitches(vals);
            if (!(numPatternSwitches == 1))
            {
                return false;
            }

            // Currently, this thinning algorithm runs as 1-iteration-per-cycle, since the order of operations is not important. 
            if ((iteration % 2) != 1)
            {
                return evenIteration(vals);
            }
            else
            {
                return oddIteration(vals);
            }

        }

        // Check for background pixels in the vicinity. 
        private bool evenIteration(bool[] vals)
        {
            if (!(vals[1] == m_background || vals[3] == m_background || vals[5] == m_background))
            {
                return false;
            }

            if (!(vals[3] == m_background || vals[5] == m_background || vals[7] == m_background))
            {
                return false;
            }

            return true;
        }

        // Variation of the checks in an even iteration. 
        private bool oddIteration(bool[] vals)
        {
            if (!(vals[1] == m_background || vals[3] == m_background || vals[7] == m_background))
            {
                return false;
            }

            if (!(vals[1] == m_background || vals[5] == m_background || vals[7] == m_background))
            {
                return false;
            }
            return true;
        }

        /**
         * {@inheritDoc} 
         */
        /*
       public void afterCycle() 
         { 
           ++iteration; 
       } 
    */
        /**
         * {@inheritDoc} 
         */
        /*
       public ThinningStrategy copy()
       { 
           return new ZhangSuenAlgorithm(m_foreground); 
       } 
           */
    }
}
