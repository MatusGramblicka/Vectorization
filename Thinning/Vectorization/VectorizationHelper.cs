using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class VectorizationHelper
    {
        public static bool equalShift(List<Shift> previousShifts, List<Shift> nextShifts)
        {
            for (int i = 0; i < nextShifts.Count; i++)
            {
                if ((previousShifts[0].X == nextShifts[i].X) && (previousShifts[0].Y == nextShifts[i].Y))        // predchadzajuci posun je rovnaky ako nasledujuci, teda idem idem po čiare
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Shift> shiftRemove(List<Shift> nextShifts) // uprednostnovanie 90° posuny namiesto diagonalnych, diagonalne sa nezapocitavaju, teda su zmazane, ak existuje 90° posun
        {
            for (int w = 0; w < nextShifts.Count; w++) //  
            {
                if (nextShifts[w].Posun % 2 != 0)
                {
                    if (nextShifts[w].Posun == 1)
                    {
                        if (w > 0)
                        {
                            if (nextShifts[w - 1].Posun == 2)
                            {
                                nextShifts.RemoveAt(w - 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                        if (nextShifts.Count > (w + 1))
                        {
                            if (nextShifts[w + 1].Posun == 0)
                            {
                                nextShifts.RemoveAt(w + 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                    }

                    if (nextShifts[w].Posun == 3)
                    {
                        if (w > 0)
                        {
                            if (nextShifts[w - 1].Posun == 4)
                            {
                                nextShifts.RemoveAt(w - 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                        if (nextShifts.Count > (w + 1))
                        {
                            if (nextShifts[w + 1].Posun == 2)
                            {
                                nextShifts.RemoveAt(w + 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                    }

                    if (nextShifts[w].Posun == 5)
                    {
                        if (w > 0)
                        {
                            if (nextShifts[w - 1].Posun == 6)
                            {
                                nextShifts.RemoveAt(w - 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                        if (nextShifts.Count > (w + 1))
                        {
                            if (nextShifts[w + 1].Posun == 4)
                            {
                                nextShifts.RemoveAt(w + 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                    }

                    if (nextShifts[w].Posun == 7)
                    {
                        if (nextShifts.Count > 1)
                        {
                            if (nextShifts[w + 1].Posun == 6)      // 0ovy sused bude na poslednom mieste, toto je trochu tricky 
                            {
                                nextShifts.RemoveAt(w + 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                        if (nextShifts.Count > 1)
                        {
                            if (nextShifts[nextShifts.Count - 1].Posun == 0)
                            {
                                nextShifts.RemoveAt(nextShifts.Count - 1);
                                w = -1;     // zacinam od zaciatku
                                continue;
                            }
                        }
                    }
                }
            }
            return nextShifts;
        }

        public static bool getShiftOdd(int i, int posun)
        {
            int koef = 0;

            if (posun == 3 & i == 0) i = 8;      // 0 treba zmenit na 8, v pripade ked je posun 3, aby vychadzali pocty

            if (posun > i)
            {
                koef = posun - 4;
            }
            else
            {
                koef = posun + 4;
            }

            if (i >= koef - 1 & i <= koef + 1)
                return true;
            else
                return false;
        }

        public static bool getShiftEven(int i, int posun)     // zatial len opozitny bod nebude sa brat do uvahy, neskor podla testov mozno aj ine
        {
            if (posun - i == Math.Abs(4))       // su opozitne, napr smer 4 opozitny je 0, 0 nemozem brat do uvahy
            {
                return true;
            }
            else return false;
        }
    }
}
