using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class HilditchThinning2 : Abstract3x3NeighbourhoodThinning //http://www.programcreek.com/java-api-examples/index.php?source_dir=knip-master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/HilditchAlgorithm.java
    {                                                                 //https://github.com/knime-ip/knip/blob/master/org.knime.knip.base/src/org/knime/knip/base/nodes/proc/thinning/strategies/HilditchAlgorithm.java  
        /**
    * Create a new hilditch strategy. The passed boolean will represent the foreground-value of the image. 
    * 
    * @param foreground Value determining the boolean value of foreground pixels. 
    */
        public HilditchThinning2(bool foreground) : base(foreground)
        {
            //super(foreground); 
        }

        /**
         * {@inheritDoc} 
         */


        public bool[][] Hilditch2ThinningAlg(bool[][] s)
        {
            bool[][] temp = ThinningHelper.ArrayClone(s);  // make a deep copy to start.. 


            int count = 0;
            do  // the missing iteration
            {
                count = Hilditch2Thinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..and on each..
                count += Hilditch2Thinning(temp, s);
                temp = ThinningHelper.ArrayClone(s);      // ..call!
            }
            while (count > 0);

            return s;
        }

        public int Hilditch2Thinning(bool[][] temp, bool[][] s)
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


        public bool removePixel(int x, int y, bool[][] temp/*, long[] position, RandomAccessible<BitType> accessible*/)
        {
            /*
            RandomAccess<BitType> access = accessible.randomAccess(); 
            access.setPosition(position); 
     */
            //     bool[] vals = getNeighbourhood(access); 
            bool[] vals = getNeighbourhood(x, y, temp);

            // First condition is to ensure there are at least 2 and at most 6 neighbouring foreground pixels. 
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

            // Second condition checks for transitions between foreground and background. Exactly 1 such transition 
            // is required. 
            int numPatterns = findPatternSwitches(vals);
            if (!(numPatterns == 1))
            {
                return false;
            }

            // The third and fourth conditions require neighbourhoods of adjacent pixels. 

            // Access has to be reset to current image-position before moving it, since 
            // the getNeighbourhood() method moves it to the top-left of the initial pixel. 
            /*
            access.setPosition(position);
            access.move(-1, 1);
            */
            int p2Patterns = findPatternSwitches((getNeighbourhood(x, y, temp)));
            if (!((vals[1] == m_background || vals[3] == m_background || vals[7] == m_background) || p2Patterns != 1))
            {
                return false;
            }
            /*
            access.setPosition(position);
            access.move(1, 0);
            */
            int p4Patterns = findPatternSwitches((getNeighbourhood(x, y, temp)));
            if (!((vals[1] == m_background || vals[3] == m_background || vals[5] == m_background) || p4Patterns != 1))
            {
                return false;
            }

            // If all conditions are met, we can safely remove the pixel. 
            return true;
        }

        /**
         * {@inheritDoc} 
         */
        /*
         public ThinningStrategy copy()
     { 
             return new HilditchAlgorithm(m_foreground); 
         } 
     */
    }
}
