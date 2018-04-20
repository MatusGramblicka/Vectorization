using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class MorphologicalThinning : Abstract3x3NeighbourhoodThinning
    {
        private int iteration = 0;

        /**
         * Create a new morphological thinning strategy. The passed boolean will represent the foreground-value of the image. 
         * 
         * @param foreground Value determining the boolean value of foreground pixels. 
         */
        public MorphologicalThinning(bool foreground) : base(foreground)
        {
            //super(foreground); 
        }

        public bool[][] MorphologicalThinningAlg(bool[][] s)
        {
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 


            int count = 0;
            do  // the missing iteration
            {
                count = stepMorphologicalThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += stepMorphologicalThinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!

                iteration++;
            }
            while (count > 0);

            return s;
        }

        public int stepMorphologicalThinning(bool[][] temp, bool[][] s)
        {
            int count = 0;

            for (int a = 1; a < temp.Length - 1; a++)
            {
                for (int b = 1; b < temp[0].Length - 1; b++)
                {
                    if (s[a][b])
                    if (removePixel(a, b, temp))
                    {
                        // still changes happening?
                        if (s[a][b]) count++;
                        s[a][b] = false;
                    }
                }
            }
            return count;
        }

        public bool removePixel(int x, int y, bool[][] temp) //http://www.programcreek.com/java-api-examples/index.php?source_dir=knip-master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/MorphologicalThinning.java
        {                                                    //https://github.com/knime-ip/knip/blob/master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/MorphologicalThinning.java
            /*
                   // Setup 
                   RandomAccess<BitType> access = accessible.randomAccess(); 
                   access.setPosition(position); 
            */
            // boolean[] vals = getNeighbourhood(access); 
            bool[] vals = getNeighbourhood(x, y, temp);

            // Depending on the current step of the cycle, we rotate the two Filters by 0, 90, 180 or 270 Degrees. 
            if (iteration % 4 == 0)
            {
                return top(vals);
               // if (top(vals) | right(vals) | bottom(vals) | left(vals))    // tieto 2 davajú asi najlepší výsledok, avšak sledované len na jednom obrázku
               //     return true;                
            }

            // toto neprebehneoh
            if (iteration % 4 == 1)
            {
                return right(vals);
            }

            if (iteration % 4 == 2)
            {
                return bottom(vals);
            }

            if (iteration % 4 == 3)
            {
                return left(vals);
            }

            return false;
        }

        /*
         * This method applies the filters without rotating. The actual filters are given by: 
         * 
         *  0   0   0           0   0 
         *      1           1   1   0 
         *  1   1   1           1 
         * 
         *  (Zero stands for background, 1 stands for foreground, no value represents a wildcard.) 
         *  Since the ThinningOp only checks pixels which are in the foreground, the center pixel is alway 1. 
         */
        private bool top(bool[] vals)
        {
            if (vals[1] == m_background && vals[2] == m_background && vals[8] == m_background && vals[4] == m_foreground && vals[5] == m_foreground && vals[6] == m_foreground)
            {
                return true;
            }
            if (vals[1] == m_background && vals[2] == m_background && vals[3] == m_background && vals[5] == m_foreground && vals[7] == m_foreground)
            {
                return true;
            }

            return false;
        }

        // Rotated by 90 degrees RIGHT 
        private bool right(bool[] vals)
        {
            if (vals[2] == m_background && vals[3] == m_background && vals[4] == m_background && vals[6] == m_foreground && vals[7] == m_foreground && vals[8] == m_foreground)
            {
                return true;
            }
            if (vals[3] == m_background && vals[4] == m_background && vals[5] == m_background && vals[1] == m_foreground && vals[7] == m_foreground)
            {
                return true;
            }

            return false;
        }

        // Rotated by 180 degrees 
        private bool bottom(bool[] vals)
        {
            if (vals[4] == m_background && vals[5] == m_background && vals[6] == m_background && vals[1] == m_foreground && vals[2] == m_foreground && vals[8] == m_foreground)
            {
                return true;
            }
            if (vals[5] == m_background && vals[6] == m_background && vals[7] == m_background && vals[1] == m_foreground && vals[3] == m_foreground)
            {
                return true;
            }

            return false;
        }

        // Rotated by 270 degrees RIGHT or 90 degrees LEFT. 
        private bool left(bool[] vals)
        {
            if (vals[8] == m_background && vals[7] == m_background && vals[6] == m_background && vals[2] == m_foreground && vals[3] == m_foreground && vals[4] == m_foreground)
            {
                return true;
            }
            if (vals[7] == m_background && vals[8] == m_background && vals[1] == m_background && vals[5] == m_foreground && vals[3] == m_foreground)
            {
                return true;
            }

            return false;
        }

        /**
         * {@inheritDoc} 
         */

        public void afterCycle()
        {
            // Keep track of current step in the cycle. 
            ++iteration;
        }


        /**
         * {@inheritDoc} 
         */

        public int getIterationsPerCycle()
        {
            // To ensure correct order of filter applications and correct termination, we need at least 4 iterations per cycle. 
            // This guarantees that each filter is checked before terminating. 
            return 4;
        }

        /**
         * {@inheritDoc} 
         */
        /*
        public ThinningStrategy copy()
        {
            return new MorphologicalThinning(m_foreground);
        }
        */
    }
}
