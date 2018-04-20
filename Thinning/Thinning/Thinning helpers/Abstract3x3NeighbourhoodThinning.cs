using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public abstract class Abstract3x3NeighbourhoodThinning      //http://www.programcreek.com/java-api-examples/index.php?source_dir=knip-master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/Abstract3x3NeighbourhoodThinning.java
    {                                                           //https://github.com/knime-ip/knip/blob/master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/Abstract3x3NeighbourhoodThinning.java
        /**
      * Boolean value of the foreground.
      */
        protected bool m_foreground;

        /**
         * Boolean value of the background.
         */
        protected bool m_background;

        /**
         * Create a new abstract thinning strategy. The passed boolean will represent the foreground-value of the image.
         *
         * @param foreground Value determining the boolean value of foreground pixels.
         */
        protected Abstract3x3NeighbourhoodThinning(bool foreground)
        {
            m_foreground = foreground;
            m_background = !foreground;
        }
        /**
         * {@inheritDoc}
         */
        /*
        public bool removePixel(long[] position, RandomAccessible<BitType> access)
        {
            // TODO Auto-generated method stub
            return false;
        }
        */
        /**
         * Returns all booleans in a 3x3 neighbourhood of the pixel the RandomAccess points to.
         * These booleans are stored in an Array in the following order: <br>
         *
         *  8   1   2 <br>
         *  7   0   3 <br>
         *  6   5   4 <br>
         *
         * @param access A RandomAccess pointing to a pixel of the image
         * @return A boolean Array holding the values of the neighbourhood in clockwise order.
         */
        protected bool[] getNeighbourhood(/*RandomAccess<BitType> access*/int x, int y, bool[][] temp)
        {
            bool[][] s = temp;

            bool[] vals = new bool[9];

            // vals[0] = access.get().get();// tento je zbytočný
            vals[0] = s[x][y];


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

        /**
         * Returns the amount of switches from foreground to background occurring in the circle around vals[1]
         * @param vals Boolean Array holding the neighbourhood.
         * @return Amount of true-false switches in the neighbourhood.
         */
        protected int findPatternSwitches(bool[] vals)
        {
            int res = 0;
            for (int i = 1; i < vals.Length - 1; ++i)
            {
                if (vals[i] == m_foreground && vals[i + 1] == m_background)
                {
                    ++res;
                }
                if (vals[vals.Length - 1] == m_foreground && vals[1] == m_background)
                {
                    ++res;
                }
            }
            return res;
        }


        public void afterCycle()
        {
            // Intentionally left blank.
        }



        public int getIterationsPerCycle()
        {
            return 1;
        }


    }
}
