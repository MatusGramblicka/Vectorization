using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{                               //http://stackoverflow.com/questions/9622211/how-to-make-correct-clone-of-the-listmyobject
    public interface ICloneable<T> //http://www.codinghelmet.com/?path=howto/implement-icloneable-or-not
    {
        T Clone();
    }

    public class coordinatesVect : ICloneable<coordinatesVect>
    {
        private  int x;
        private int y;

        public coordinatesVect(int X, int Y)
        {
            this.x= X;
            this.y= Y;
        }
       
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
               
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public virtual coordinatesVect Clone()
        {
            return new coordinatesVect(this.x, this.y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)     //http://www.codeproject.com/Articles/18936/A-Csharp-Implementation-of-Douglas-Peucker-Line-Ap?loginkey=false&fid=422038&df=90&mpp=25&sort=Position&spc=Relaxed&prof=False&view=Normal&fr=1#xx0xx
        {
            if (obj is coordinatesVect)
            {
                coordinatesVect p = (coordinatesVect)obj;
                return x == p.x && +y == p.y;
            }
            else
                return false;
        }
    }
}
