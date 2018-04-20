using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tessnet2;
using netDxf;
using LineDXF = netDxf.Entities;
using netDxf.Header;

namespace Thinning
{
    public partial class Form1 : Form
    {
        # region variables
        private Bitmap OriginalImage, OriginalImage2; // The original image. // cropping
        private Bitmap CroppedImage, CroppedImage2; // The currently cropped image.
        private Bitmap DisplayImage, DisplayImage2;          // The cropped image with the selection rectangle.
        Bitmap figure1, figure2;
        private Bitmap imageForAngle = null;
        private Graphics DisplayGraphics, DisplayGraphics2;
        Image drawing;
        private Point StartPoint, EndPoint, StartPoint2, EndPoint2;

        private bool cropping = false;
        private bool Drawing = false;                // Let the user select an area.
        private bool Drawing2 = false;
        bool[][] t = null;

        int[][] r = null;
        int xDimension, yDimension;     // pre účely spätného určenia a priradenia hodnoty kóty k čiare
        private float angle = 0.0f;

        static string fileName;     // for the temporary image file for OCR use 
        # endregion

        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;

            toolStripStatusLabel1.Text = "";
        }

        # region Component behaviour
        private void loadTSButon_Click(object sender, EventArgs e)
        {
            string input_file_name = null;

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                input_file_name = openFileDialog1.FileName.ToString();
            }

            OriginalImage = LoadBitmapUnlocked(openFileDialog1.FileName);// pre účely cropovania
            CroppedImage = OriginalImage.Clone() as Bitmap;
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);

            figure1 = (Bitmap)Image.FromFile(input_file_name);
            figure2 = (Bitmap)Image.FromFile(input_file_name);
        
            pictureBox1.Image = DisplayImage;
            drawing = Image.FromFile(input_file_name);    // tento predávam thinning algoritmom           
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (fileName != null)
            {
                System.Diagnostics.Process.Start(fileName);
            }
        }

        private void pictureBox2_Layout(object sender, LayoutEventArgs e)
        {
            fileName = ImageCapture.SaveImageCapture(pictureBox2.Image);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = pictureBox1.Image;
        }
        # endregion

        # region Cropping
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Crop();
        }

        private void Crop() // Crop the image
        {
            if (!cropping)
            {
                cropping = true;  // turn on
           //     this.Cursor = Cursors.Cross;
            }
            else
            {
                cropping = false; // turn off
                this.Cursor = Cursors.Default;
            }
        }                

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (cropping)
            {
                Drawing = true;
                StartPoint = e.Location;

                DrawSelectionBox(e.Location); // Draw the area selected.
            }
        }
        //---------------------------------------------------------------------
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (cropping)
            {
                this.Cursor = Cursors.Cross;

                if (!Drawing)
                    return;

                DrawSelectionBox(e.Location); // Draw the area selected.
            }
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (cropping)
            {
                this.Cursor = Cursors.Default;
            }
        }
        //---------------------------------------------------------------------
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Drawing)
                return;

            Drawing = false;

            int x = Math.Min(StartPoint.X, EndPoint.X); // Crop. // Get the selected area's dimensions.            
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(StartPoint.X - EndPoint.X);
            int height = Math.Abs(StartPoint.Y - EndPoint.Y);
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);

            DisplayImage = new Bitmap(width, height);   // Copy that part of the image to a new bitmap.
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            DisplayGraphics.DrawImage(CroppedImage, dest_rect, source_rect, GraphicsUnit.Pixel);

            CroppedImage = DisplayImage;     // Display the new bitmap.
            DisplayImage = CroppedImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);

            imageForAngle = DisplayImage;
            pictureBox2.Image = (Bitmap)DisplayImage.Clone(); ;

            fileName = ImageCapture.SaveImageCapture(pictureBox2.Image);

            inicializePicBox();    // je potrebné vždy brať obrázok z pictureboxu ako originál, volať pri každej zmene pictureboxu, pre správne cropovanie

            reset();        // musím resetnúť inak to nebude poriaden fungovať            

            xDimension = x + width / 2;       // pre účely spätného určenia a priradenia hodnoty kóty k čiare
            yDimension = y + height / 2;
        }

        public void inicializePicBox()   // je potrebné vždy brať obrázok z pictureboxu ako originál, volať pri každej zmene pictureboxu, pre správne cropovanie
        {
            OriginalImage2 = LoadBitmapUnlocked2(/*ofdPicture.FileName*/pictureBox2.Image);
            CroppedImage2 = OriginalImage2.Clone() as Bitmap;
            DisplayImage2 = CroppedImage2.Clone() as Bitmap;
            DisplayGraphics2 = Graphics.FromImage(DisplayImage2);

            pictureBox2.Image = DisplayImage2;
        }

        private void DrawSelectionBox(Point end_point)      // Draw the area selected.
        {
            EndPoint = end_point;         // Save the end point.
            if (EndPoint.X < 0) EndPoint.X = 0;
            if (EndPoint.X >= CroppedImage.Width) EndPoint.X = CroppedImage.Width - 1;
            if (EndPoint.Y < 0) EndPoint.Y = 0;
            if (EndPoint.Y >= CroppedImage.Height) EndPoint.Y = CroppedImage.Height - 1;
            
            DisplayGraphics.DrawImageUnscaled(CroppedImage, 0, 0);      // Reset the image.
            
            int x = Math.Min(StartPoint.X, EndPoint.X);       // Draw the selection area.
            int y = Math.Min(StartPoint.Y, EndPoint.Y);
            int width = Math.Abs(StartPoint.X - EndPoint.X);
            int height = Math.Abs(StartPoint.Y - EndPoint.Y);
            DisplayGraphics.DrawRectangle(Pens.Red, x, y, width, height);
            pictureBox1.Refresh();
        }

        private void reset()
        {
            CroppedImage = OriginalImage.Clone() as Bitmap;
            DisplayImage = OriginalImage.Clone() as Bitmap;
            DisplayGraphics = Graphics.FromImage(DisplayImage);
            pictureBox1.Image = DisplayImage;
        }

        private void DrawSelectionBox2(Point end_point)
        {
            EndPoint2 = end_point;        // Save the end point.
            if (EndPoint2.X < 0) EndPoint2.X = 0;
            if (EndPoint2.X >= CroppedImage2.Width) EndPoint2.X = CroppedImage2.Width - 1;
            if (EndPoint2.Y < 0) EndPoint2.Y = 0;
            if (EndPoint2.Y >= CroppedImage2.Height) EndPoint2.Y = CroppedImage2.Height - 1;

            DisplayGraphics2.DrawImageUnscaled(CroppedImage2, 0, 0);        // Reset the image.
            
            int x = Math.Min(StartPoint2.X, EndPoint2.X);        // Draw the selection area.
            int y = Math.Min(StartPoint2.Y, EndPoint2.Y);
            int width = Math.Abs(StartPoint2.X - EndPoint2.X);
            int height = Math.Abs(StartPoint2.Y - EndPoint2.Y);
            DisplayGraphics2.DrawRectangle(Pens.Red, x, y, width, height);
            pictureBox2.Refresh();
        }

        private void reset2()
        {
            CroppedImage2 = OriginalImage2.Clone() as Bitmap;
            DisplayImage2 = OriginalImage2.Clone() as Bitmap;
            DisplayGraphics2 = Graphics.FromImage(DisplayImage2);
            pictureBox2.Image = DisplayImage2;           
        }

        private Bitmap LoadBitmapUnlocked(string file_name)    // Load the image into a Bitmap, clone it, and set the PictureBox's Image property to the Bitmap.     
        {
            using (Bitmap bm = new Bitmap(file_name))
            {
                Bitmap new_bitmap = new Bitmap(bm.Width, bm.Height);
                using (Graphics gr = Graphics.FromImage(new_bitmap))
                {
                    gr.DrawImage(bm, 0, 0);
                }
                return new_bitmap;
            }
        }
       
        private Bitmap LoadBitmapUnlocked2(Image file_name)     // Load the image into a Bitmap, clone it, and set the PictureBox's Image property to the Bitmap.     
        {
            using (Bitmap bm = new Bitmap(file_name))
            {
                Bitmap new_bitmap = new Bitmap(bm.Width, bm.Height);
                using (Graphics gr = Graphics.FromImage(new_bitmap))
                {
                    gr.DrawImage(bm, 0, 0);
                }
                return new_bitmap;
            }
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (cropping)
            {
                Drawing2 = true;
                StartPoint2 = e.Location;

                DrawSelectionBox2(e.Location);        // Draw the area selected.
            }
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (cropping)
            {
                this.Cursor = Cursors.Cross;

                if (!Drawing2)
                    return;

                DrawSelectionBox2(e.Location);       // Draw the area selected.
            }
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Drawing2) return;
            Drawing2 = false;

            int x = Math.Min(StartPoint2.X, EndPoint2.X);        // Crop.    // Get the selected area's dimensions.
            int y = Math.Min(StartPoint2.Y, EndPoint2.Y);
            int width = Math.Abs(StartPoint2.X - EndPoint2.X);
            int height = Math.Abs(StartPoint2.Y - EndPoint2.Y);
            Rectangle source_rect = new Rectangle(x, y, width, height);
            Rectangle dest_rect = new Rectangle(0, 0, width, height);

            DisplayImage2 = new Bitmap(width, height);        // Copy that part of the image to a new bitmap.
            DisplayGraphics2 = Graphics.FromImage(DisplayImage2);
            DisplayGraphics2.DrawImage(CroppedImage2, dest_rect, source_rect, GraphicsUnit.Pixel);

            CroppedImage2 = DisplayImage2;       // Display the new bitmap.
            DisplayImage2 = CroppedImage2.Clone() as Bitmap;
            DisplayGraphics2 = Graphics.FromImage(DisplayImage2);
            pictureBox2.Image = DisplayImage2;
            pictureBox2.Refresh();

            imageForAngle = DisplayImage2;  // kvôli rotácii, aby rotoval vždy nový obrázok z premennej imageForAngle        
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            if (cropping)
            {
                this.Cursor = Cursors.Default;
            }
        }
        # endregion

        # region Thinning
        private void showThinImage(bool[][] t)
        {           
            pictureBox2.Image = ThinningHelper.Bool2Image(t);          

            double seconds = Benchmark.GetSeconds();

            textBox4.Text = seconds.ToString();
            toolStripStatusLabel1.Text = seconds.ToString();
        }

        private void showThinImage(int[][] r)
        {
            pictureBox2.Image = ThinningHelper.Int2Image(r);

            Benchmark.End();
            double seconds = Benchmark.GetSeconds();

            textBox4.Text = seconds.ToString();
            toolStripStatusLabel1.Text = seconds.ToString();
        }

        private void zhangSuenThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);            

            t = ZhangSuenThinning.ZhangSuenThinningAlg(t);
            showThinImage(t);
        }

        private void zhangWangThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = ModifiedAlgorithmZhangWangThinning.ModifiedThinningalgorithmZhangWangThinningAlg(t);
            showThinImage(t);
        }

        private void stentifordThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            pictureBox2.Image = FalseStentifordThinning.StentifordThinningAlg(figure1, figure2);

            Benchmark.End();

            double seconds = Benchmark.GetSeconds();

            toolStripStatusLabel1.Text = seconds.ToString();
        }

        private void guoHallThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing); 
      
            GuoHallThinning GuoHall = new GuoHallThinning(true);

            t = GuoHall.GuoHallThinningAlg(t);
            showThinImage(t);
        }

        private void hilditchThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            r = ThinningHelper.Image2Int(drawing);

            r = OldHilditchThinning.HilditchThinningAlg(r);
            showThinImage(r);
        }

        private void morphologicalThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            MorphologicalThinning Morphological = new MorphologicalThinning(true);

            t = Morphological.MorphologicalThinningAlg(t);
            showThinImage(t);
        }

        private void hilditchThinning2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            HilditchThinning2 Hilditch2 = new HilditchThinning2(true);

            t = Hilditch2.Hilditch2ThinningAlg(t);
            showThinImage(t);
        }

        private void zhangSuen2ThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            ZhangSuenThinning2 ZhangSuen = new ZhangSuenThinning2(true);

            t = ZhangSuen.ZhangSuen2ThinningAlg(t);
            showThinImage(t);
        }

        private void hilditchThinning3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            HilditchThinning3 Hilditch3 = new HilditchThinning3(figure1);

            Bitmap outputImage = Hilditch3.thin();
            pictureBox2.Image = outputImage;
        }

        private void hilditchThinning4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = HilditchThinning4.hilditch4(t);
            showThinImage(t);
        }

        private void zhangSuenThinning3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = ZhangSuenThinning3.thinImage(t);
            showThinImage(t);
        }
       
        private void luWangThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = LuWangThinning.LuWangThinningAlg(t);
            showThinImage(t);
        }

        private void kwonWoongKangThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = KwonWoongKangThinning.KwonWoongKangThinningAlg(t);
            showThinImage(t);
        }

        private void wangZhangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = WangZhangThinning.WangZhangThinningAlg(t);
            showThinImage(t);
        }

        private void arabicParallelThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = ArabicParallelThinningAlgorithm.ArabicParallelThinningAlg(t);
            showThinImage(t);
        }

        private void novelImageThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = NovelImageThinning.NovelImageThinningAlg(t);
            showThinImage(t);
        }

        private void skuskaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = StentifordThinning.skuskaAlg(t);
            showThinImage(t);
        }

        private void hybridThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = HybridThinning.HybridAlg(t);
            showThinImage(t);
        }

        private void efficientThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = EfficientThinning.EfficientThinningAlg(t);
            showThinImage(t);
        }

        private void hilditchThinningToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = HilditchThinning.HilditchThinningAlg(t);
            showThinImage(t);
        }

        private void proposedThinningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Benchmark.Start();

            t = ThinningHelper.Image2Bool(drawing);

            t = ProposedThinning.ProposedKwonWoongKangThinningAlg(t);
            showThinImage(t);
        }
        # endregion       

        # region Rotation
        private void button1_Click(object sender, EventArgs e)
        {
            angle = (float)numUDAngle.Value;
            pictureBox2.Image = ImageRotation.RotateImage(pictureBox2, imageForAngle, angle);

            fileName = ImageCapture.SaveImageCapture(pictureBox2.Image);

            inicializePicBox();    // je potrebné vždy brať obrázok z pictureboxu ako originál, volať pri každej zmene pictureboxu, pre správne cropovanie
        }
        # endregion

        # region OCR
        private void btnOCR_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            ocr_demo();

            textBox3.Text = "X: " + xDimension + " Y: " + yDimension;     // súradnice kóty
        }

        public void ocr_demo()
        {
            if (fileName == null)
                return; 
         
            var image = new Bitmap(fileName);
            var ocr = new tessnet2.Tesseract();

            // ocr.SetVariable("tessedit_char_whitelist", "0123456789");    // if only digits          

            string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            ocr.Init(wanted_path + "\\tessdata", "eng", false);

            var result = ocr.DoOCR(image, Rectangle.Empty);
            foreach (tessnet2.Word word in result)
            {               
                textBox1.Text += word.Text;
            }        
        }
        # endregion

        # region Zoom
        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap zoomBitmap = (Bitmap)pictureBox2.Image;
           
            string zoomString = textBox2.Text;
            int zoomFactor;

            if (zoomBitmap == null)
                return; 
            
            if (int.TryParse(zoomString, out zoomFactor))
            {
                Size newSize = new Size((int)(zoomBitmap.Width * zoomFactor), (int)(zoomBitmap.Height * zoomFactor));   // http://stackoverflow.com/questions/10915958/how-to-zoom-inout-an-image-in-c-sharp
                Bitmap bmp = new Bitmap(zoomBitmap, newSize);

                pictureBox2.Image = bmp;
            }

            fileName = ImageCapture.SaveImageCapture(pictureBox2.Image);

            inicializePicBox();    // je potrebné vždy brať obrázok z pictureboxu ako originál, volať pri každej zmene pictureboxu, pre správne cropovanie
        }
        # endregion            

        # region oldVectorizationAlg
        private void btnVectorization1_Click(object sender, EventArgs e)
        {
            /**
             * <p><code>[ 0 ][ 1 ][ 2 ]</code></p>
             * <p><code>[ 7 ][ P ][ 3 ]</code></p>
             * <p><code>[ 6 ][ 5 ][ 4 ]</code></p> 
            **/
            int[][] matica = new int[8][] { new int[] { -4, -1 }, new int[] { 0, -1 }, new int[] { 4, -1 }, new int[] { 4, 0 }, new int[] { 4, 1 }, new int[] { 0, 1 }, new int[] { -4, 1 }, new int[] { -4, 0 }/*, new int[] { 0, -1 } */};

            int countNeighborhoodPoints = 0;
            int countOldNeighborhoodPoints = 0;

            int countReturned = 0;
            bool returned = false;

            List<coordinatesVect> CoordinatesDXF = new List<coordinatesVect>();

            int picHeight = pictureBox2.Height;

            if (pictureBox2.Image != null)
            {
                unsafe
                {
                    Bitmap bmp = new Bitmap(pictureBox2.Image);
                    Color clr = Color.Empty;

                    List<Shift> previousShifts = new List<Shift>();
                    List<Shift> nextShifts = new List<Shift>();

                    BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                    int heightInPixels = bitmapData.Height;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                    byte* currentLine2 = null;
                    int x3 = 0;

                    byte* currentLine3 = null;
                    int x4 = 0;

                    int NeighborhoodCount = 0;
                    int sumTotalNeighborhood = 0;

                    bool maz = false;
                    bool pokracuj = false;

                    for (int r = 0; r < 4; r++)
                        for (int y = 0; y < bmp.Height; y++)
                        {
                            byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                            for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                            {
                                int oldBlue = currentLine[x];
                                int oldGreen = currentLine[x + 1];
                                int oldRed = currentLine[x + 2];

                                if (oldRed == 0 & oldGreen == 0 & oldBlue == 0)           // som na súčiastke
                                {
                                    for (int i = 0; i < matica.Length; i++)         // prezerám okolie cez maticu vyššie uvedenú
                                    {
                                        currentLine2 = ptrFirstPixel + ((y + matica[i][1]) * bitmapData.Stride);     // musím sa posunúť v Y-ovom smere o dany smer matice
                                        x3 = x + matica[i][0];                                                      // musím sa posunúť v X-ovom smere o dany smer matice

                                        int oldBlue2 = currentLine2[x3];
                                        int oldGreen2 = currentLine2[x3 + 1];
                                        int oldRed2 = currentLine2[x3 + 2];

                                        if (oldRed2 == 0 & oldGreen2 == 0 & oldBlue2 == 0)  // narazil som na susedny bod
                                        {
                                            /*
                                            nextShifts.Insert(0, new Shift { X = matica[i][0], Y = matica[i][1], Posun = i });      // ukladam smery posunu v 8 okoli matice
                                            previousShifts.Insert(0, new Shift { X = matica[i][0], Y = matica[i][1], Posun = i });
                                            */
                                            nextShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i));      // ukladam smery posunu v 8 okoli matice
                                            previousShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i));

                                            countNeighborhoodPoints++;    // kolko je susednych bodov 
                                        }
                                    }

                                    if (returned)
                                    {
                                        countOldNeighborhoodPoints = countOldNeighborhoodPoints - countNeighborhoodPoints;   // zmenil sa pocet susednych bodov, po opakovani z toho isteho bodu?

                                        if (countOldNeighborhoodPoints == 1)      // ak bola zmena v pocte, resp. znizenie poctu susednych bodov, treba znizit premennu, aby som spravne urcil kam sa posunut dalej
                                            countReturned--;
                                    }

                                    countOldNeighborhoodPoints = countNeighborhoodPoints;   // kvoli porovnavaniu stareho poctu bodov a noveho, ak sa opakuje povodny bod, pretoze mal viacero susedov

                                    if (countNeighborhoodPoints == 0)               // Mazem bod na ktorom som nenasiel ziadny susedny bod 
                                    {
                                        currentLine[x] = 255;
                                        currentLine[x + 1] = 255;
                                        currentLine[x + 2] = 255;

                                        continue;  // treba sa posunut dalej v cykle, ked nema ziadnych susedov
                                        // break;      // treba sa posunut dalej v cykle, ked nema ziadnych susedov
                                    }

                                    if (countNeighborhoodPoints == 1)               // Mazem bod na ktorom som nasiel len 1 susedny bod 
                                    {
                                        currentLine[x] = 255;
                                        currentLine[x + 1] = 255;
                                        currentLine[x + 2] = 255;

                                        countReturned = 0;   // ak je uz len jeden sused a predtym sa opakovalo tak sa resetnu returned premenne, ktore ovladaju opakovania
                                        returned = false;
                                    }

                                    if (countNeighborhoodPoints > 1 & countReturned != 0)               // ak uz som sa raz vracal do toho isteho bodu, treba zmenit smer posunu
                                    {
                                        if (countReturned == countNeighborhoodPoints)    // ak sa aj napriek opakovaniu z ineho smeru nepodari odstranit body, chod dalej bez toho aby sme vyriesili dane problematicke miesta
                                        {
                                            countReturned = 0;
                                            countNeighborhoodPoints = 0;
                                            returned = false;

                                            // tu by bolo mozno dobre ho zmazat

                                            continue;
                                        }

                                        previousShifts[0].X = nextShifts[countReturned].X;       ///potocit nextpoint na zaklade hodnoty returned
                                        previousShifts[0].Y = nextShifts[countReturned].Y;
                                        previousShifts[0].Posun = nextShifts[countReturned].Posun;
                                    }

                                    int x2 = x;         // poloha bodu do novych premennych
                                    int y2 = y;

                                    //  CoordinatesDXF.Add(new coordinatesVect { X = x2 / 4, Y = y2 }); //Start point x2, y2
                                    CoordinatesDXF.Add(new coordinatesVect(x2 / 4, y2)); //Start point x2, y2

                                    do
                                    {
                                        pokracuj = false;

                                        x2 += previousShifts[0].X;            // opat sa posuvam na susedny bod, z ktoreho prezriem 8-okolie
                                        y2 += previousShifts[0].Y;

                                        nextShifts.RemoveRange(0, nextShifts.Count);// treba vymazat list nextShift

                                        currentLine2 = ptrFirstPixel + (y2 * bitmapData.Stride);         // bod na ktorom prezeram 8-okolie a kt. eventuelne zmazem                               
                                        x3 = x2;

                                        for (int i = 0; i < matica.Length; i++)         // prezerám okolie cez maticu vyššie uvedenú
                                        {
                                            currentLine3 = ptrFirstPixel + ((y2 + matica[i][1]) * bitmapData.Stride);  // prezeram okolie v posunutom bode   // musím sa posunúť v Y-ovom smere                                       
                                            x4 = x3 + matica[i][0];                                                                                         // musím sa posunúť v X-ovom smere

                                            int oldBlue2 = currentLine3[x4];
                                            int oldGreen2 = currentLine3[x4 + 1];
                                            int oldRed2 = currentLine3[x4 + 2];

                                            if (oldRed2 == 0 & oldGreen2 == 0 & oldBlue2 == 0)  // narazil som na susedny bod
                                            {
                                                /*
                                                nextShifts.Insert(0, new Shift { X = matica[i][0], Y = matica[i][1], Posun = i }); // ukladam susedne body v 8 okoli matice
                                                */
                                                nextShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i)); // ukladam susedne body v 8 okoli matice
                                                NeighborhoodCount++;
                                                sumTotalNeighborhood++;

                                                if ((i % 2 != previousShifts[0].Posun % 2) & (previousShifts[0].Posun - i != Math.Abs(4)))      // ak narazim na diagonálneho suseda, mozem zmazat sam seba 
                                                {                                                                                               // a predchadzajuci posun nie je protilahly k danemu posunu, napr 3 a 7
                                                    maz = true;
                                                }
                                            }

                                            if (NeighborhoodCount > 0)      // ma susedny bod, kontrolujem, ci je mozne nastavit predchadzajuci posun
                                            {
                                                if ((previousShifts[0].X == nextShifts[0].X) & (previousShifts[0].Y == nextShifts[0].Y))    // predchadzajuci posun je rovnaky ako nasledujuci, teda idem idem po čiare
                                                {
                                                    previousShifts[0].X = nextShifts[0].X;      // predchadzajuci posun sa stane sucasnym
                                                    previousShifts[0].Y = nextShifts[0].Y;

                                                    NeighborhoodCount = 0;
                                                }
                                            }                                            //if (isNeighborhoodCount > 1)    // toto je zrejme zbytocne??  // ma viac susednych bodov{if ((previousShifts[0].X == nextShifts[0].X) & (previousShifts[0].Y == nextShifts[0].Y))    // predchadzajuci posun  je rovnaky ako nasledujuci, teda idem idem po čiare                                                {currentLine2[x3] = 255;         // mozem ho zmazat, ak ma len jeden susedny je zbytocne ho ponechavat, ulahci sa tym dalsia praca                                                    currentLine2[x3 + 1] = 255;                                                    currentLine2[x3 + 2] = 255;isNeighborhoodCount = 0;previousShifts[0].X = nextShifts[0].X;      // predchadzajuci posun sa stane sucasnym                                                    previousShifts[0].Y = nextShifts[0].Y;}}}
                                        }

                                        for (int i = 0; i < nextShifts.Count; i++) //  tu treba prejst vsetky next shifty, porovnat ich a snad aj zmazat sucasny bod, z ktoreho prezeram
                                        {
                                            if ((sumTotalNeighborhood == 1) & (previousShifts[0].X == nextShifts[i].X) & (previousShifts[0].Y == nextShifts[i].Y))
                                            {
                                                currentLine2[x3] = 255;         // mozem ho zmazat, ak ma len jeden susedny je zbytocne ho ponechavat, ulahci sa tym dalsia praca
                                                currentLine2[x3 + 1] = 255;
                                                currentLine2[x3 + 2] = 255;
                                            }
                                            if ((sumTotalNeighborhood > 1) & (previousShifts[0].X == nextShifts[i].X) & (previousShifts[0].Y == nextShifts[i].Y) & maz)
                                            {
                                                currentLine2[x3] = 255;         // mozem ho zmazat, posun je v smere rovnakom, a navyse aj ked ma viac susedov mozem ho zmazat, lebo niektore su diagonalne
                                                currentLine2[x3 + 1] = 255;
                                                currentLine2[x3 + 2] = 255;
                                            }
                                        }

                                        if (NeighborhoodCount == 0 & previousShifts != null)    // mazem posledny bod, ktory by inak zostal, kedze uz nema suseda
                                        {
                                            currentLine2[x3] = 255;     // mozno skor sum, alebo obe?
                                            currentLine2[x3 + 1] = 255;
                                            currentLine2[x3 + 2] = 255;
                                        }

                                        if (sumTotalNeighborhood == 0)     // nema susedne body, je zbytočne sa posuvat dalej
                                            break;

                                        for (int i = 0; i < nextShifts.Count; i++) //  porovnat vsetky posuny ci sa nerovnaju predchadzajucemu a ak ano pokracovat v cykle
                                        {
                                            if ((previousShifts[0].X == nextShifts[i].X) & (previousShifts[0].Y == nextShifts[i].Y))
                                            {
                                                pokracuj = true;
                                            }
                                        }

                                        NeighborhoodCount = 0;         // resetujem premenne
                                        sumTotalNeighborhood = 0;
                                        maz = false;

                                    } while (pokracuj);      // kym sa rovnaju, znamena ze idem po ciare, pokracujem v cykle  

                                    //  CoordinatesDXF.Add(new coordinatesVect { X = x3 / 4, Y = y2 });    //End point x3, y2
                                    CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2));    //End point x3, y2

                                    nextShifts.RemoveRange(0, nextShifts.Count);    // treba vymazat list nextShift a previousShifts
                                    previousShifts.RemoveRange(0, previousShifts.Count);
                                }

                                if (countNeighborhoodPoints > 1)        // ak som mal na prvom bode viac ako jedneho suseda, treba sa tam vratit
                                {
                                    x = x - 4;          //vraciam ho na povodny bod

                                    countReturned++;
                                    returned = true;
                                }

                                countNeighborhoodPoints = 0;
                            }
                        }

                    bmp.UnlockBits(bitmapData);

                    pictureBox2.Image = bmp;
                }

                textBox3.Text = countNeighborhoodPoints.ToString();
            }

            if (CoordinatesDXF.Count > 1)
            {
                CreateDXF.CreateDXF_File(CoordinatesDXF, picHeight);
            }
        }
        # endregion

        # region VectorizationAlg
        private void btnVectorization2_Click(object sender, EventArgs e)
        {
            # region Variables vectorization
            /**
             * <p><code>[ 0 ][ 1 ][ 2 ]</code></p>
             * <p><code>[ 7 ][ P ][ 3 ]</code></p>
             * <p><code>[ 6 ][ 5 ][ 4 ]</code></p> 
            **/
            int[][] matica = new int[8][] { new int[] { -4, -1 }, new int[] { 0, -1 }, new int[] { 4, -1 }, new int[] { 4, 0 }, new int[] { 4, 1 }, new int[] { 0, 1 }, new int[] { -4, 1 }, new int[] { -4, 0 }/*, new int[] { 0, -1 } */};

         //   string fileName = @"C:\Users\Skuska\Desktop\" + Guid.NewGuid().ToString() + ".dxf";      //http://stackoverflow.com/questions/581570/how-can-i-create-a-temp-file-with-a-specific-extension-with-net
                string fileName = @"C:\Users\Administrator\Desktop\" + Guid.NewGuid().ToString() + ".dxf";     // umiestnenie na skolskom PC

            int xOrigin = 0;
            int yOrigin = 0;

            bool notGetShift = false;
            bool equalSh = false;
            bool returned = false;
            bool originReturn = true;
            bool wasReturn = false;

            List<coordinatesVect> CoordinatesDXF = new List<coordinatesVect>();
            List<coordinatesVect> returnPoint = new List<coordinatesVect>();
            List<coordinatesVect> reducedLines = new List<coordinatesVect>();

            List<List<coordinatesVect>> Full_amerenPoints = new List<List<coordinatesVect>>();
            List<List<coordinatesVect>> reducedCoor = new List<List<coordinatesVect>>();

            List<Shift> previousShifts = new List<Shift>();
            List<Shift> nextShifts = new List<Shift>();

            List<double> linesWidth = new List<double>();

            int picHeight = pictureBox2.Height;

            DxfDocument dxf = new DxfDocument();                    // inicializuje NETDXF
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;

            LineDXF.Line[] line = new LineDXF.Line[50000];              // premenna kam ukladam jednotlive ciary

            Bitmap drawing = new Bitmap(pictureBox1.Image);
            # endregion

            if (pictureBox2.Image != null)
            {
                unsafe
                {
                    Bitmap bmp = new Bitmap(pictureBox2.Image);
                    Color clr = Color.Empty;

                    BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                    int heightInPixels = bitmapData.Height;
                    int widthInBytes = bitmapData.Width * bytesPerPixel;
                    byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                    byte* currentLine2 = null;
                    int x3 = 0;

                    byte* currentLine3 = null;
                    int x4 = 0;

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                        for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                        {
                            if (returnPoint.Count != 0)     // mazem navratovy bod
                            {
                                returnPoint.RemoveAt(0);
                            }

                            int oldBlue = currentLine[x];
                            int oldGreen = currentLine[x + 1];
                            int oldRed = currentLine[x + 2];

                            if (oldRed == 0 && oldGreen == 0 && oldBlue == 0)           // som na súčiastke
                            {
                                if (originReturn)               // prvy krat som narazil na cierny bod, ukladam si poziciu
                                {
                                    xOrigin = x;
                                    yOrigin = y;

                                    originReturn = false;
                                }

                                returned = false;

                                for (int i = 0; i < matica.Length; i++)         // prezerám okolie cez maticu vyššie uvedenú
                                {
                                    currentLine2 = ptrFirstPixel + ((y + matica[i][1]) * bitmapData.Stride);     // musím sa posunúť v Y-ovom smere o dany smer matice
                                    x3 = x + matica[i][0];                                                      // musím sa posunúť v X-ovom smere o dany smer matice

                                    int oldBlue2 = currentLine2[x3];
                                    int oldGreen2 = currentLine2[x3 + 1];
                                    int oldRed2 = currentLine2[x3 + 2];

                                    if (oldRed2 == 0 && oldGreen2 == 0 && oldBlue2 == 0)  // narazil som na susedny bod
                                    {
                                        nextShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i));      // ukladam smery posunu v 8 okoli matice
                                        previousShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i));
                                    }
                                }

                                if (nextShifts.Count > 1)
                                    nextShifts = VectorizationHelper.shiftRemove(nextShifts);       // uprednostnovanie 90° posuny namiesto diagonalnych, diagonalne sa nezapocitavaju, teda su zmazane, ak existuje 90° posun

                                if (nextShifts.Count == 0)               // Mazem bod na ktorom som nenasiel ziadny susedny bod 
                                {
                                    currentLine[x] = 255;
                                    currentLine[x + 1] = 255;
                                    currentLine[x + 2] = 255;

                                    continue;      // treba sa posunut dalej v cykle, ked nema ziadnych susedov   //break;    
                                }

                                if (nextShifts.Count == 1)               // Mazem bod na ktorom som nasiel len 1 susedny bod 
                                {
                                    currentLine[x] = 255;
                                    currentLine[x + 1] = 255;
                                    currentLine[x + 2] = 255;
                                }

                                if (nextShifts.Count > 1)               // musim ulozit pociatocny bod kedze je tam viacero susedov, aby som sa ptm mohol do neho vratit
                                {
                                    returnPoint.Add(new coordinatesVect(x, y));    // treba si ulozit pozíciu tohto bodu, do neho sa bude treba vracat, -4 preto, lebo sa v iteracii pricita 4, takze aby to vyslo                                          

                                    wasReturn = true;       // existoval navratov bod, toto znamena, ze sa bude treba vracat na uplny zaciatok, po vycerpaní navratovych bodov
                                }

                                int x2 = x;         // poloha bodu do novych premennych
                                int y2 = y;

                                CoordinatesDXF.Add(new coordinatesVect(x2 / 4, y2)); //Start point x2, y2

                                do
                                {
                                    equalSh = false;

                                    x2 += previousShifts[0].X;                      // opat sa posuvam na susedny bod podla posunu, z ktoreho prezriem 8-okolie
                                    y2 += previousShifts[0].Y;

                                    nextShifts.RemoveRange(0, nextShifts.Count);    // treba vymazat list nextShift

                                    currentLine2 = ptrFirstPixel + (y2 * bitmapData.Stride);         // bod na ktorom prezeram 8-okolie a kt. eventuelne zmazem                               
                                    x3 = x2;

                                    for (int i = 0; i < matica.Length; i++)         // prezerám okolie cez maticu vyššie uvedenú
                                    {
                                        currentLine3 = ptrFirstPixel + ((y2 + matica[i][1]) * bitmapData.Stride);  // prezeram okolie v posunutom bode   // musím sa posunúť v Y-ovom smere                                       
                                        x4 = x3 + matica[i][0];                                                                                         // musím sa posunúť v X-ovom smere

                                        int oldBlue2 = currentLine3[x4];
                                        int oldGreen2 = currentLine3[x4 + 1];
                                        int oldRed2 = currentLine3[x4 + 2];

                                        if (oldRed2 == 0 && oldGreen2 == 0 && oldBlue2 == 0)  // narazil som na susedny bod
                                        {
                                            if (previousShifts[0].Posun % 2 != 0)              // len neparne posuny, kontrola predchadzajucich pixelov, vodorovne a zvisle posuny
                                                notGetShift = VectorizationHelper.getShiftOdd(i, previousShifts[0].Posun);     // zistujem ci mam zapisovat posun, treba ignorovat susedov, ktory sa nachadzaju za smerom posunu
                                            else                                                   // len parne posuny, kontrola predchadzajucich pixelov
                                            {
                                                notGetShift = VectorizationHelper.getShiftEven(i, previousShifts[0].Posun); // diagonalne posuny
                                            }

                                            if (!notGetShift)           // ukladam len nadchadzajuce posuny
                                            {
                                                nextShifts.Insert(0, new Shift(matica[i][0], matica[i][1], i)); // ukladam susedne body v 8 okoli matice  
                                            }
                                            notGetShift = false;
                                        }
                                    }

                                    if (nextShifts.Count > 0)                // ma susedny bod, kontrolujem, ci je mozne nastavit predchadzajuci posun
                                    {
                                        equalSh = VectorizationHelper.equalShift(previousShifts, nextShifts);  // je predchadzajuci posun je rovnaky ako nasledujuci, teda idem idem po čiare
                                    }

                                    if (nextShifts.Count > 1)
                                        nextShifts = VectorizationHelper.shiftRemove(nextShifts);       // uprednostnovanie 90° posuny namiesto diagonalnych, diagonalne sa nezapocitavaju, teda su zmazane, ak existuje 90° posun vedal diagonalneho

                                    if ((nextShifts.Count == 3) & equalSh) // treba rozdelit ak su tri susedne a je tam smer, nech sa meni smer inak niektore ciary ostavali v celku aj ked by mali byt rozdelene
                                    {
                                        equalSh = false;
                                    }

                                    if ((!equalSh) & (nextShifts.Count != 0))       // je iny posun, ale da sa pokracovat v inom smere, ukoncim jednu ciaru (end point) a zacinam inu (start point)
                                    {
                                        equalSh = true;

                                        previousShifts[0].X = nextShifts[0].X;              // nastavujem iny smer
                                        previousShifts[0].Y = nextShifts[0].Y;
                                        previousShifts[0].Posun = nextShifts[0].Posun;

                                        CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2)); //end point                                        
                                        CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2)); //start point
                                    }

                                    if ((nextShifts.Count == 1) && (equalSh)) // mozem ho zmazat, ak ma len jeden susedny a ma totozny posun je zbytocne ho ponechavat, ulahci sa tym dalsia praca
                                    {
                                        currentLine2[x3] = 255;
                                        currentLine2[x3 + 1] = 255;
                                        currentLine2[x3 + 2] = 255;
                                    }

                                    if (nextShifts.Count == 0 && previousShifts != null)    // mazem posledny bod, ktory by inak zostal, kedze uz nema suseda
                                    {
                                        currentLine2[x3] = 255;     // mozno skor sum, alebo obe?
                                        currentLine2[x3 + 1] = 255;
                                        currentLine2[x3 + 2] = 255;
                                    }

                                    if (nextShifts.Count > 1)       // ak je viac susedných bodov, treba tu zastavit a vytvorit endpoint a zaroven start point, teda rozdelit ciaru
                                    {
                                        CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2)); //end point                                         
                                        CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2)); //start point

                                        returnPoint.Add(new coordinatesVect(x3, y2));    // treba si ulozit pozíciu tohto bodu, do neho sa bude treba vracat, -4 preto, lebo sa v iteracii pricita 4, takze aby to vyslo                                          

                                        wasReturn = true;       // existoval navratov bod, toto znamena, ze sa bude treba vracat na uplny zaciatok, po vycerpaní navratovych bodov
                                    }

                                    if (nextShifts.Count == 0)     // nema susedne body, je zbytočne sa posuvat dalej
                                        break;

                                } while (equalSh);                  // kym sa rovnaju, znamena ze idem po ciare, pokracujem v cykle  

                                CoordinatesDXF.Add(new coordinatesVect(x3 / 4, y2));    //End point x3, y2        

                                nextShifts.RemoveRange(0, nextShifts.Count);            // treba vymazat list nextShift a previousShifts
                                previousShifts.RemoveRange(0, previousShifts.Count);

                                if (returnPoint.Count != 0)             // musi existovat miesto , kde bolo viac susedov
                                {
                                    x = returnPoint[0].X - 4;                // nastavujem suradnice do bodu, kde bolo viac susedov
                                    y = returnPoint[0].Y;
                                    currentLine = ptrFirstPixel + (y * bitmapData.Stride);      // musim menit aj pointer nie len y

                                    returned = true;
                                }
                            }
                            else
                            {
                                if (returned && returnPoint.Count != 0)                           //vratil som sa na ulozený navratovy bod, ktory uz neexistuje, idem do dalsieho
                                {
                                    x = returnPoint[0].X - 4;                        // nastavujem suradnice do bodu, kde bolo viac susedov
                                    y = returnPoint[0].Y;
                                    currentLine = ptrFirstPixel + (y * bitmapData.Stride);      // musim menit aj pointer nie len y

                                    returned = true;
                                }
                                else returned = false;
                            }

                            if (returnPoint.Count == 0 && wasReturn) // ak sa uz nemam kam vratit, musim zacat od uplne prveho bodu kde som narazil na suciastku, aby som dokazal najst aj dalsie pripadne oddelene casti suciastky
                            {
                                x = xOrigin - 4;
                                y = yOrigin;
                                currentLine = ptrFirstPixel + (y * bitmapData.Stride);      // musim menit aj pointer nie len y

                                originReturn = true;
                                wasReturn = false;
                            }
                        }
                    }
                    bmp.UnlockBits(bitmapData);

                    pictureBox2.Image = bmp;
                }
            }
            //-----------------------------------------------------------------------------------------------------------------------
            Full_amerenPoints = PolygonalApproximationHelper.selectpointsForPolApproximation(CoordinatesDXF);       // rozdelujem ciary podla toho ci su spojene. Ak nie suspojene vytvaram novy list. Vysledok je list v liste urceny pre polygonálnu aproximáciu

            reducedCoor = PolygonalApproximationHelper.polygonalApproximation(Full_amerenPoints);        // polygonalna aproximacia počítam a vykreslujem výsledok
            //-----------------------------------------------------------------------------------------------------------------------
            linesWidth = WidthPreserving.ComputeThickness(CoordinatesDXF, drawing);      // zistujem hrubky ciar

            line = LinesHelper.createLines(CoordinatesDXF, line, picHeight);        // vytvaram z bodov ciary, pre ucely vytvorenia DXF suboru

            double width = double.Parse(TBWidth.Text);        // hrúbka akú chcem, aby sa zaznamenala hrubšie na vektore  

            line = LinesHelper.setWidth(CoordinatesDXF, linesWidth, line, width);          // priradujem hrubku ciaram
            
            // tu by sa dala spravit polygon. aprox., podla toho ci maju rovnaku hrubku

            /*
            foreach (var L in line)
            {
                if (L !=null && L.Lineweight.Value == 30)                   // zrejme iba hrubé čiary chcem ukladať
                    dxf.AddEntity(L);
            }
            */
            dxf.AddEntity(line);
            dxf.Save(fileName);                                         // creating DXF file         
            //-----------------------------------------------------------------------------------------------------------------------
            reducedLines = PolygonalApproximationHelper.vytvorCiary(reducedCoor);                    //dostal som v premennej body po polygonalnej aproximacii, ide o list v liste, treba ho prerobit na ciary podla toho ci su spojene alebo nie

            if (reducedLines.Count > 1)
            {
                CreateDXF.CreateDXF_File(reducedLines, picHeight);      // vytvaram DXF subor, zatial pre ucely polygonalnej aproximacie
            }
        } 
        # endregion

        // toto by malo ist neskor prec, je to tu len kvoli malovaniu do picturebox2
        private List<List<coordinatesVect>> polygonalApproximation(List<List<coordinatesVect>> Full_amerenPoints)
        {
            List<coordinatesVect> reducedCoordinates = new List<coordinatesVect>();
            List<List<coordinatesVect>> FullreducedCoordinates = new List<List<coordinatesVect>>();

            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);

            foreach (var sublist in Full_amerenPoints)
            {
                List<System.Drawing.Point> drawingPoints = new List<System.Drawing.Point>();
                foreach (coordinatesVect point in sublist)
                {
                    drawingPoints.Add(new System.Drawing.Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)));
                }
                if (drawingPoints.Count > 2)
                {
                    g.DrawLines(new Pen(Brushes.Black, 1), drawingPoints.ToArray());

                    List<coordinatesVect> points = PolygonalApproximation.DouglasPeuckerReduction(sublist, Convert.ToDouble(4));

                    drawingPoints = new List<System.Drawing.Point>();
                    foreach (coordinatesVect point in points)
                    {
                        drawingPoints.Add(new System.Drawing.Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)));
                        reducedCoordinates.Add(point);
                    }

                    Random randomGen = new Random();
                    KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));

                    for (int i = 0; i < drawingPoints.Count - 1; i++)
                    {
                        KnownColor randomColorName = names[randomGen.Next(names.Length)];
                        Color randomColor = Color.FromKnownColor(randomColorName);

                        g.DrawLine(new Pen(randomColor, 1), drawingPoints[i].X + 200, drawingPoints[i].Y + 200, drawingPoints[i + 1].X + 200, drawingPoints[i + 1].Y + 200);
                        g.DrawLine(new Pen(Brushes.Red, 1), drawingPoints[i].X + 200, drawingPoints[i].Y + 200, drawingPoints[i + 1].X + 200, drawingPoints[i + 1].Y + 200);
                    }
                }

                FullreducedCoordinates.Add(reducedCoordinates);
                reducedCoordinates = new List<coordinatesVect>();  //recreate           
            }

            pictureBox2.Image = bmp;

            g.Dispose();

            return FullreducedCoordinates;
        }      
        
        # region Benchmarking
        private void button5_Click(object sender, EventArgs e)          // performance measurement of thinning for article purposes
        {          
            ResultsForPaper results = new ResultsForPaper();

            long totalThinned = results.pixelCount(pictureBox2.Image);

            double thinness = results.Thinness(pictureBox2.Image);
                      
            long totalOriginal = results.pixelCount(pictureBox1.Image);

            double reduction = (double)totalThinned / (double)totalOriginal;

            double thinningRatio = (1 - reduction) * 100;

            int sensitivity = results.Sensitivity(pictureBox2.Image);

            int connectivity = results.Connectivity(pictureBox2.Image);    
         }
        # endregion

            
    }
}
