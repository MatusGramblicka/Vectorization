using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinning
{
    public class ImageCapture
    {
        public static string SaveImageCapture(System.Drawing.Image image)
        {    
            if (image != null)
            {     
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".jpg";      //http://stackoverflow.com/questions/581570/how-can-i-create-a-temp-file-with-a-specific-extension-with-net

                using (System.IO.FileStream fstream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                {
                    image.Save(fstream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fstream.Close();
                }
                return fileName;
            }
            return null;
        }     
    }
}
