using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class PolygonalApproximationHelper
    {
        public static List<List<coordinatesVect>> polygonalApproximation(List<List<coordinatesVect>> Full_amerenPoints)
        {
            List<coordinatesVect> reducedCoordinates = new List<coordinatesVect>();
            List<List<coordinatesVect>> FullreducedCoordinates = new List<List<coordinatesVect>>();
            /*
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            */
            foreach (var sublist in Full_amerenPoints)
            {
                List<System.Drawing.Point> drawingPoints = new List<System.Drawing.Point>();
                foreach (coordinatesVect point in sublist)
                {
                    drawingPoints.Add(new System.Drawing.Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)));
                }
                if (drawingPoints.Count > 2)
                {
                  //  g.DrawLines(new Pen(Brushes.Black, 1), drawingPoints.ToArray());

                    List<coordinatesVect> points = PolygonalApproximation.DouglasPeuckerReduction(sublist, Convert.ToDouble(4));

                    drawingPoints = new List<System.Drawing.Point>();
                    foreach (coordinatesVect point in points)
                    {
                        drawingPoints.Add(new System.Drawing.Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)));
                        reducedCoordinates.Add(point);
                    }
                    /*
                    Random randomGen = new Random();
                    KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));

                    for (int i = 0; i < drawingPoints.Count - 1; i++)
                    {
                        KnownColor randomColorName = names[randomGen.Next(names.Length)];
                        Color randomColor = Color.FromKnownColor(randomColorName);

                        g.DrawLine(new Pen(randomColor, 1), drawingPoints[i].X + 200, drawingPoints[i].Y + 200, drawingPoints[i + 1].X + 200, drawingPoints[i + 1].Y + 200);
                        g.DrawLine(new Pen(Brushes.Red, 1), drawingPoints[i].X + 200, drawingPoints[i].Y + 200, drawingPoints[i + 1].X + 200, drawingPoints[i + 1].Y + 200);
                    }
                    */
                }

                FullreducedCoordinates.Add(reducedCoordinates);
                reducedCoordinates = new List<coordinatesVect>();  //recreate           
            }

           // pictureBox2.Image = bmp;

           // g.Dispose();

            return FullreducedCoordinates;
        }

        public static List<List<coordinatesVect>> selectpointsForPolApproximation(List<coordinatesVect> _amerenPoints)        // http://www.dotnetperls.com/nested-list
        {
            List<List<coordinatesVect>> Full_amerenPointsLocal = new List<List<coordinatesVect>>();
            List<coordinatesVect> Part_amerenPoints = new List<coordinatesVect>();

            bool zapisPrvy = false;

            Part_amerenPoints.Add(_amerenPoints[0]);

            for (int i = 1; i < _amerenPoints.Count; i++)
            {
                if (i % 2 != 0)
                {
                    if ((i + 1) == _amerenPoints.Count)             // v pripade, ked uz som na konci, a aby mi nehadzalo, ze som mimo pola, zapisujem posledne dva body a koncim
                    {
                        Part_amerenPoints.Add(_amerenPoints[i]);

                        Full_amerenPointsLocal.Add(Part_amerenPoints);
                        break;
                    }
                    if (_amerenPoints[i].X == _amerenPoints[i + 1].X && _amerenPoints[i].Y == _amerenPoints[i + 1].Y)
                    {
                        if (zapisPrvy == true)
                        {
                            Part_amerenPoints.Add(_amerenPoints[i - 1]);
                            zapisPrvy = false;
                        }

                        Part_amerenPoints.Add(_amerenPoints[i]);
                        Part_amerenPoints.Add(_amerenPoints[i + 1]);
                    }
                    else
                    {
                        Part_amerenPoints.Add(_amerenPoints[i]);
                        Full_amerenPointsLocal.Add(Part_amerenPoints);           // http://www.dotnetperls.com/nested-list

                        Part_amerenPoints = new List<coordinatesVect>();  //recreate

                        zapisPrvy = true;

                        continue;
                    }
                }
            }
            return Full_amerenPointsLocal;
        }

        public static List<coordinatesVect> vytvorCiary(List<List<coordinatesVect>> reducedCoor)
        {
            List<coordinatesVect> reducedLines = new List<coordinatesVect>();

            foreach (var sublist in reducedCoor)
            {
                if (sublist.Count != 0)
                {
                    reducedLines.Add(sublist[0]);

                    for (int u = 1; u < sublist.Count - 1; u++)
                    {
                        reducedLines.Add(sublist[u]);
                        reducedLines.Add(sublist[u]);
                    }

                    reducedLines.Add(sublist[sublist.Count - 1]);
                }
            }

            return reducedLines;
        }     
    }
}
