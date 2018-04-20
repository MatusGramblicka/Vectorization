using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class Shift
    { 
        private int x;
        private int y;
        private int posun;

        public Shift(int X, int Y, int Posun)
        {
            this.x = X;
            this.y = Y;
            this.posun = Posun;
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
       
        public int Posun
        {
            get
            {
                return posun;
            }
            set
            {
                posun = value;
            }
        }
    }
}
