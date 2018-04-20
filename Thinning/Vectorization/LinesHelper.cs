using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using netDxf;
using LineDXF = netDxf.Entities;

namespace Thinning
{
    public class LinesHelper
    {
        public static LineDXF.Line[] setWidth(List<coordinatesVect> CoordinatesDXF, List<double> linesWidth, LineDXF.Line[] line, double width)
        {
            for (int j = 0; j < CoordinatesDXF.Count / 2; j++)      // priradujem hrubku ciaram
            {
                if (linesWidth[j] != 0.0)
                {
                    short s = (short)linesWidth[j];

                    if (s > /*2.0*/width)                                   // here you can change threshold of line width
                    {
                        line[j].Lineweight.Value = 30;
                    }
                }
            }

            return line;
        }

        public static LineDXF.Line[] createLines(List<coordinatesVect> CoordinatesDXF, LineDXF.Line[] line, int picHeight)
        {
            for (int i = 0, j = 0; i < CoordinatesDXF.Count/* - 1*/; j++, i = i + 2)        // vytvaram z bodov ciary
            {
                line[j] = new LineDXF.Line();
                line[j].StartPoint = new Vector3(CoordinatesDXF[i].X, picHeight - CoordinatesDXF[i].Y, 0);
                line[j].EndPoint = new Vector3(CoordinatesDXF[i + 1].X, picHeight - CoordinatesDXF[i + 1].Y, 0);
            }

            return line;
        }
    }
}
