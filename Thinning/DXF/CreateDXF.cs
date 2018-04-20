using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public static class CreateDXF
    {
        static double x_scale = 1.0, y_scale = 1.0;
        static string layer_name = "VecGram";
        static int color = 0;

        public static void CreateDXF_File(List<coordinatesVect> startCoordinatesXY, int picHeight)
        {
           // FileStream fp_dxf = new FileStream(@"C:\Users\Skuska\Desktop\dxf_file_name.dxf", FileMode.Create);
            FileStream fp_dxf = new FileStream(@"C:\Users\Administrator\Desktop\dxf_file_name.dxf", FileMode.Create);     // cesta na skolskom PC

            StreamWriter sw = new StreamWriter(fp_dxf);

            sw.Write("0\nSECTION\n2\nHEADER\n0\nENDSEC\n0\nSECTION\n2\nTABLES\n0\nENDSEC\n0\nSECTION\n2\nBLOCKS\n0\nENDSEC\n0\nSECTION\n2\nENTITIES\n");

            for (int i = 0; i < startCoordinatesXY.Count; i = i + 2)
            {
                sw.Write("0\nPOLYLINE\n8\n{0}\n62\n{1}\n66\n1\n70\n8\n", layer_name, color);

                int startX = startCoordinatesXY[i].X;
                int startY = picHeight - startCoordinatesXY[i].Y;

                int endX = startCoordinatesXY[i + 1].X;
                int endY = picHeight - startCoordinatesXY[i + 1].Y;

                sw.Write("0\nVERTEX\n8\n{0}\n62\n{1}\n70\n32\n10\n" + startX + "\n20\n" + startY + "\n30\n0\n", layer_name, color);
                sw.Write("0\nVERTEX\n8\n{0}\n62\n{1}\n70\n32\n10\n" + endX + "\n20\n" + endY + "\n30\n0\n", layer_name, color);

                sw.Write("0\nSEQEND\n");
            }

            sw.Write("0\nENDSEC\n0\nEOF\n");

            sw.Close();
        }
    }
}
