using System;
using System.Collections.Generic;

using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Management;
using System.Drawing.Text;

namespace GraphDLL
{
    public static partial class Graph
    {
        internal static GForm2 form;
        internal static Color color = Color.White;
        internal static SolidBrush brush = new SolidBrush(System.Drawing.Color.White);
        internal static Pen pen = new Pen(color.SysDraw);
        internal static Font font = new Font("Tahoma", 10, System.Drawing.FontStyle.Regular);
        internal static bool imediateDrawing = false;
        internal static bool showmouse = true;
        private static double widthInMM, heightInMM;
        public static Bitmap bitmap { get; internal set; }
        public static int FPS { get { return ((int)(1000.0 / MSPF)); } }
        public static int width { get; private set; }
        public static int height { get; private set; }
        public static int ScreenWidth { get { return SystemInformation.PrimaryMonitorSize.Width; } }
        public static int ScreenHeight { get { return SystemInformation.PrimaryMonitorSize.Height; } }
        public static double MonitorWidthMillimeter { get { return widthInMM; } }
        public static double MonitorHeightMillimeter { get { return heightInMM; } }



        #region Initialize
        private static Thread myThread = new Thread(MakeForm);
        private static void MakeForm()
        {
            form = new GForm2();
            Application.Run(form);
        }

        public static void initgraph(int Width, int Height)
        {
            Graph.width = Width;
            Graph.height = Height;
            initdraw();
            if (skipLogo)
                initwithlogo();

            myThread.Start();
            while (!GForm2.answered) ;
            form.Invoke((MethodInvoker)delegate { form.Activate(); });
            GForm2.MouseX = Width / 2;
            GForm2.MouseY = Height / 2;
            if (!skipLogo)
                showLogo();

            lastRefresh = DateTime.Now;
        }

        public static void initgraph(int Width, int Height, bool fullScreen, Glass glass, AnaglyphMethod method)
        {
            initgraph(Width, Height);
            Graph3d.set3DGlass(glass);
            Graph3d.SetAnaglyphMethod(method);
        }

        private static void initdraw()
        {
            Graph3d._ye = Graph.height / 2;
            Graph3d._x1e = Graph3d._xe - Graph.width / 20;
            Graph3d._x2e = Graph3d._xe + Graph.width / 20;

            Graph.bitmap = new Bitmap(Graph.width, Graph.height);
            cleardevice();
        }

        private static bool skipLogo = false;
        private static void showLogo()
        {
            DateTime t;
            int x = 0, y = 0;
            Bitmap logo = new Bitmap(GraphDLL.Properties.Resources._13cforGraph);

            x = width / 2 - logo.Width / 2;
            y = height / 2 - logo.Height / 2;
            if (y < 0) y = 0;

            hidemouse();
            if (GForm2.fullScreen)
                delay(1500);
            else
                delay(250);
            t = DateTime.Now;
            double alpha = 0;
            do
            {
                alpha = (DateTime.Now - t).TotalMilliseconds;
                if (alpha > 255)
                    alpha = 255;
                cleardevice();
                putimage(logo.Opacity(alpha * 100 / 255), x, y);
                delay(0);
            } while (alpha < 255);

            initwithlogo();
            delay(1500);
            t = DateTime.Now;
            do
            {
                alpha = (DateTime.Now - t).TotalMilliseconds;
                if (alpha > 255)
                    alpha = 255;
                cleardevice();
                putimage(logo.Opacity((255 - alpha) * 100 / 255), x, y);
                delay(0);
            } while (alpha < 255);
            showMouse();
        }

        private static void initwithlogo()
        {
            try
            {
                MeasureMM();
            }
            catch { }
            try
            {
                initsound();
            }
            catch { }
        }

        #endregion

        #region Delay
        private static DateTime lastRefresh;
        public static double MSPF { get; private set; }

        public static void refresh()
        {
            delay(0);
        }

        static Semaphore semaphore = new Semaphore(0, 1);
        public static void delay(int milisecound)
        {
            if (bitmap != null)
            {
                byte[] bytesToShow = (byte[])bitmap.Pixels.Clone();

                Bitmap temp = null;
                if (Graph3d.mouse3da && Graph3d.MouseX != -1)
                {
                    temp = new Bitmap(width, height);
                    temp.Pixels = bytesToShow;
                    Graph3d.DrawMouse(bitmap);
                    bytesToShow = (byte[])bitmap.Pixels.Clone();
                }

                form.BeginInvoke((MethodInvoker)delegate
                {
                    Image p = form.pictureBox1.Image;

                    Bitmap bitmapToShow = new Bitmap(width, height);
                    bitmapToShow.Pixels = bytesToShow;
                    form.pictureBox1.Image = bitmapToShow.SysDraw;

                    if (p != null)
                        p.Dispose();
                    if (imediateDrawing)
                    {
                        form.pictureBox1.Update();
                        form.Activate();
                        semaphore.Release();
                    }
                });

                if (temp != null)
                    bitmap.Pixels = temp.Pixels;

                if (imediateDrawing)
                    semaphore.WaitOne();
            }


            if (lastRefresh.Year == 1)
                lastRefresh = DateTime.Now;

            if (milisecound > 0)
            {
                TimeSpan temp;
                DateTime until = lastRefresh.AddMilliseconds(milisecound);
                temp = until - DateTime.Now;
                if (temp > TimeSpan.Zero)
                    Thread.Sleep(temp);
                //do
                //temp = until - DateTime.Now;
                //while (temp.Ticks > 5000);
            }
            MSPF = (DateTime.Now - lastRefresh).TotalMilliseconds;
            lastRefresh = DateTime.Now;
        }
        #endregion

        #region RegularMethods
        private static void MeasureMM()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\root\\wmi", "SELECT * FROM WmiMonitorBasicDisplayParams");
            foreach (ManagementObject mo in searcher.Get())
            {
                widthInMM = (byte)mo["MaxHorizontalImageSize"] * 10;
                heightInMM = (byte)mo["MaxVerticalImageSize"] * 10;
                break;
            }
            double inch = Math.Sqrt(widthInMM * widthInMM + heightInMM * heightInMM) / 25.4;
            inch = Math.Round(inch);
            inch *= 25.4;
            double c = (double)widthInMM / heightInMM;
            heightInMM = Math.Sqrt(inch * inch / (c * c + 1));
            widthInMM = Math.Sqrt(inch * inch - heightInMM * heightInMM);
        }

        public static void trace()
        {
            Graph.imediateDrawing = true;
            while (!GForm2.answered) ;
        }
        public static void notrace()
        {
            Graph.imediateDrawing = false;
            while (!GForm2.answered) ;
        }

        public static void closegraph()
        {
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Graph.form.Close();
            });
        }

        public static void fullscreen()
        {
            GForm2.fullScreen = true;
        }

        public static void windowscreen()
        {
            GForm2.fullScreen = false;
        }

        public static void skiplogo()
        {
            skipLogo = true;
        }

        public static string intextxy(int x, int y, bool RightToLeft)
        {
            DateTime t = DateTime.Now;
            StringFormat f = (RightToLeft) ? new StringFormat(StringFormatFlags.DirectionRightToLeft) : new StringFormat();
            System.Drawing.Bitmap bg = (System.Drawing.Bitmap)Graph.bitmap.SysDraw.Clone();
            string r = "";
            char cursor = '\0';
            GForm2.ch = '\0';
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Graph.form.Activate();
            });
            while (true)
            {
                char ch = GForm2.ch;
                switch (ch)
                {
                    case '\0':
                    case (char)27:
                        break;
                    case '\r':
                        _intextxy(x, y, r, '\0', bg, f);
                        return r;
                    case '\b':
                        if (r.Length > 0)
                        {
                            r = r.Remove(r.Length - 1);
                            _intextxy(x, y, r, cursor, bg, f);
                        }
                        break;
                    default:
                        r += ch;
                        _intextxy(x, y, r, cursor, bg, f);
                        break;
                }
                GForm2.ch = '\0';

                if ((DateTime.Now - t).TotalMilliseconds > 500)
                {
                    cursor = (cursor == '\0') ? '|' : '\0';
                    _intextxy(x, y, r, cursor, bg, f);
                    t = DateTime.Now;
                }

            }
        }

        private static void _intextxy(int x, int y, string r, char cursor, System.Drawing.Bitmap bg, StringFormat f)
        {
            Graph.bitmap.SysDraw = (System.Drawing.Bitmap)bg.Clone();
            Graphics.FromImage(bitmap.SysDraw).DrawString(r + cursor, Graph.font, Graph.brush, x, y, f);
            delay(0);
        }
        #endregion

        #region Draw

        public static bool insidebitmap(int x, int y)
        {
            return ((((x >= 0) && (y >= 0)) && (x < Graph.width)) && (y < Graph.height));
        }

        public static void line(int x1, int y1, int x2, int y2)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawLine(Graph.pen, x1, y1, x2, y2);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void rectangle(int x1, int y1, int x2, int y2)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawRectangle(Graph.pen, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void bar(int x1, int y1, int x2, int y2)
        {
            Graphics.FromImage(bitmap.SysDraw).FillRectangle(Graph.brush, x1, y1, x2 - x1 + 1, y2 - y1 + 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void circle(int x, int y, int radius)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawEllipse(Graph.pen, x - radius, y - radius, radius + radius, radius + radius);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void fillcircle(int x, int y, int radius)
        {
            Graphics.FromImage(bitmap.SysDraw).FillEllipse(Graph.brush, x - radius, y - radius, radius + radius, radius + radius);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void arc(int x, int y, int stangle, int endangle, int radius)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawArc(Graph.pen, x - radius, y - radius, radius + radius, radius + radius, -stangle, -endangle + stangle - 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void ellipse(int x, int y, int stangle, int endangle, int xradius, int yradius)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawArc(Graph.pen, x - xradius, y - yradius, 2 * xradius, 2 * yradius, stangle, -endangle + stangle - 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void fillellipse(int x, int y, int xradius, int yradius)
        {
            Graphics.FromImage(bitmap.SysDraw).FillPie(Graph.brush, x - xradius, y - yradius, 2 * xradius, 2 * yradius, 0, 360);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void sector(int x, int y, int stangle, int endangle, int radius)
        {
            Graphics.FromImage(bitmap.SysDraw).FillPie(Graph.brush, x - radius, y - radius, radius + radius, radius + radius, -stangle, -endangle + stangle - 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void pieslice(int x, int y, int stangle, int endangle, int radius)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawPie(Graph.pen, x - radius, y - radius, radius + radius, radius + radius, -stangle, -endangle + stangle - 1);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void cleardevice()
        {
            Graphics.FromImage(bitmap.SysDraw).Clear(System.Drawing.Color.Black);
            if (Graph.imediateDrawing) delay(0);
        }

        public static void floodfill(int x, int y, Color border)
        {
            if (insidebitmap(x, y))
            {
                bool imediateDrawing = Graph.imediateDrawing;
                Graph.imediateDrawing = false;
                Bitmap bitmap = Graph.bitmap;
                Queue<Point> queue = new Queue<Point>(Graph.width * Graph.height);
                queue.Enqueue(new Point(x, y));
                do
                {
                    Point p = queue.Dequeue();
                    x = p.X;
                    y = p.Y;
                    if (bitmap.GetPixel(x, y) != Graph.color)
                    {
                        int num = x;
                        int num2 = x;
                        while (num > 0)
                        {
                            if (bitmap.GetPixel(num - 1, y) != border)
                                num--;
                            else break;
                        }
                        while (num2 + 1 < Graph.width)
                        {
                            if (bitmap.GetPixel(num2 + 1, y) != border)
                                num2++;
                            else break;
                        }
                        for (int i = num; i <= num2; i++)
                        {
                            bitmap.SetPixel(i, y, Graph.color);
                            if (!((y <= 0) || bitmap.GetPixel(i, y - 1) == border))
                                queue.Enqueue(new Point(i, y - 1));
                            if (!(((y + 1) >= Graph.height) || bitmap.GetPixel(i, y - 1) == border))
                                queue.Enqueue(new Point(i, y + 1));
                        }
                    }
                }
                while (queue.Count != 0);
                Graph.imediateDrawing = imediateDrawing;
                if (Graph.imediateDrawing) delay(0);
            }
        }

        public static void setcolor(Color c)
        {
            Graph.color = c;
            Graph.pen.Color = c.SysDraw;
            Graph.brush.Color = c.SysDraw;
        }
        public static void setcolor(Color c, byte transparency)
        {
            c.a = transparency;
            setcolor(c);
        }

        static FontFamily f;
        public static void setfont(string name, FontStyle style, float size)
        {
            string FontFile = name + ".ttf";
            if (File.Exists(FontFile))
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(FontFile);
                f = pfc.Families[0];
                Graph.font = new Font(f, size, (System.Drawing.FontStyle)style, GraphicsUnit.Pixel);
            }
            else
                Graph.font = new Font(name, size, (System.Drawing.FontStyle)style, GraphicsUnit.Pixel);
        }

        public static void outtextxy(int x, int y, string s)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawString(s, Graph.font, Graph.brush, x, y);
            if (Graph.imediateDrawing) delay(0);
        }
        public static void outtextxy(int x, int y, string s, bool center)
        {
            if (center)
            {
                SizeF size = Graphics.FromImage(bitmap.SysDraw).MeasureString(s, font);
                StringFormat f = new StringFormat() { Alignment = StringAlignment.Center };
                Graphics.FromImage(bitmap.SysDraw).DrawString(s, Graph.font, Graph.brush, x, y - size.Height / 2, f);
            }
            else
                Graphics.FromImage(bitmap.SysDraw).DrawString(s, Graph.font, Graph.brush, x, y);
            if (Graph.imediateDrawing) delay(0);
        }
        public static void outtextxy(int x, int y, string s, int angle)
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap.SysDraw);
            g.TranslateTransform(x, y);
            g.RotateTransform(-angle);
            g.DrawString(s, font, brush, 0, 0);
            g.Dispose();
            if (Graph.imediateDrawing) delay(0);
        }

        public static void image(string filepath, int x, int y)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            //node f = Queues.SearchFile(filepath);
            //if (f.data == null)
            //    f.data = new Bitmap(filepath);
            //Bitmap pic = (Bitmap)f.data;
            Bitmap pic = new Bitmap(filepath);
            Graphics.FromImage(bitmap.SysDraw).DrawImage(pic.SysDraw, x, y);
            if (Graph.imediateDrawing) delay(0);
        }
        public static void image(string filepath, int x1, int y1, int x2, int y2)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException();

            int w = x2 - x1 + 1;
            int h = y2 - y1 + 1;

            //node f = Queues.SearchFile(filepath);
            //if (f.data == null)
            //    f.data = new Bitmap(filepath);

            //putimage((Bitmap)f.data, x1, y1, w, h);
            Bitmap pic = new Bitmap(filepath);
            putimage(pic, x1, y1, w, h);
        }
        public static void image(string filePath, int x, int y, int width, int height, int angle)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            //node f = Queues.SearchFile(filePath);
            //if (f.data == null)
            //    f.data = new Bitmap(filePath);
            //Bitmap pic = (Bitmap)f.data;
            Bitmap pic = new Bitmap(filePath);

            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap.SysDraw);
            g.TranslateTransform(x, y);
            g.RotateTransform(-angle);
            g.DrawImage(pic.SysDraw, 0, 0, width, height);
            g.Dispose();
            if (Graph.imediateDrawing) delay(0);
        }

        public static void putimage(Bitmap image, int x, int y)
        {
            Graphics.FromImage(bitmap.SysDraw).DrawImage(image.SysDraw, x, y);
            if (Graph.imediateDrawing) delay(0);
        }
        public static void putimage(Bitmap image, int x, int y, int width, int height)
        {
            //Bitmap pic = Queues.SearchPicture(image, width, height);            
            Graphics.FromImage(bitmap.SysDraw).DrawImage(image.SysDraw, x, y, width, height);
            if (Graph.imediateDrawing) delay(0);
        }

        public static Bitmap getimage(int x1, int y1, int x2, int y2)
        {
            Bitmap source = new Bitmap((x2 - x1) + 1, (y2 - y1) + 1);
            for (int i = x1; i <= x2; i++)
                for (int j = y1; j <= y2; j++)
                    source.SetPixel(i - x1, j - y1, Graph.bitmap.GetPixel(i, j));
            return source;
        }

        public static void putpixel(int x, int y, Color color)
        {
            Graph.bitmap.SetPixel(x, y, color);
            if (Graph.imediateDrawing) delay(0);
        }
        public static void putpixel(int x, int y)
        {
            Graph.bitmap.SetPixel(x, y);
            if (Graph.imediateDrawing) delay(0);
        }

        public static Color getpixel(int x, int y)
        {
            return Graph.bitmap.GetPixel(x, y);
        }

        static System.Drawing.Bitmap background;
        public static void copybackground()
        {
            background = (System.Drawing.Bitmap)Graph.bitmap.SysDraw.Clone();
        }

        public static void pastebackground()
        {
            if (background != null)
            {
                Graph.bitmap.SysDraw = (System.Drawing.Bitmap)background.Clone();
                if (Graph.imediateDrawing) delay(0);
            }
            else
                throw new Exception("No background is copied!");
        }
        #endregion
    }
}
