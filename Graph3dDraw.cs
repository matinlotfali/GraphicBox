using System;
using System.Collections.Generic;
using System.Drawing;

using System.Threading;

namespace GraphDLL
{
    public static class Graph3dDraw
    {
        public static int FillLines = 10;
        public static AnaglyphMethod Method = AnaglyphMethod.Optimized;

        public static void _to2Da(double xm, double ym, double zm, out double x1p, out double x2p, out double yp)
        {
            double t = zm == Graph3d._ze ? 1.0 : (Graph3d._ze) / (Graph3d._ze - zm);
            x1p = (xm - Graph3d._x1e) * t + Graph3d._x1e;
            x2p = (xm - Graph3d._x2e) * t + Graph3d._x2e;
            yp = (ym - Graph3d._ye) * t + Graph3d._ye;
        }

        public static void _to2D(double xm, double ym, double zm, out double xp, out double yp)
        {
            double t = zm == Graph3d._ze ? 1.0 : (Graph3d._ze) / (Graph3d._ze - zm);
            xp = (xm - Graph3d._x1e) * t + Graph3d._x1e;
            yp = (ym - Graph3d._ye) * t + Graph3d._ye;
        }

        #region shapes
        private static void _line(int x1, int y1, int x2, int y2, bool Left, byte[] dest)
        {
            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
            {
                if (x1 > x2) { int t = x2; x2 = x1; x1 = t; t = y2; y2 = y1; y1 = t; }
                for (int i = x1; i < x2; i++)
                {
                    int j = Convert.ToInt32(y1 + (y2 - y1) * (i - x1) / Convert.ToDouble(x2 - x1));
                    _pixel(Left, i, j, dest);
                }
            }
            else
            {
                if (y1 > y2) { int t = y2; y2 = y1; y1 = t; t = x2; x2 = x1; x1 = t; }
                for (int j = y1; j < y2; j++)
                {
                    int i = Convert.ToInt32(x1 + (x2 - x1) * (j - y1) / Convert.ToDouble(y2 - y1));
                    _pixel(Left, i, j, dest);
                }
            }
        }

        private static void _pixel(bool Left, int i, int j, byte[] dest)
        {
            switch (Graph3d.glass)
            {
                case Glass.None:
                    Bitmap.SetPixel(i, j, Graph.color, dest, Graph.width, Graph.height);
                    break;
                case Glass.Anaglyph:
                    Color anaglyph = Left ?
                        AnaglyphLeft(Graph.color, j, dest, i) :
                        AnaglyphRight(Graph.color, j, dest, i);
                    Bitmap.SetPixel(i, j, anaglyph, dest, Graph.width, Graph.height);
                    break;
                case Glass.TV3D:
                    if (!Left)
                    {
                        if (i < Graph.width)
                            Bitmap.SetPixel(i / 2, j, Graph.color, dest, Graph.width, Graph.height);
                    }
                    else
                        if (i >= 0)
                            Bitmap.SetPixel((i + Graph.width) / 2, j, Graph.color, dest, Graph.width, Graph.height);
                    break;
            }
        }

        public static void line(int x1m, int y1m, int z1m, int x2m, int y2m, int z2m, byte[] dest)
        {
            double x1p1, x2p1, yp1, x1p2, x2p2, yp2;
            if (Graph3d.glass == Glass.None)
            {
                _to2D(x1m, y1m, z1m, out x1p1, out yp1);
                _to2D(x2m, y2m, z2m, out x1p2, out yp2);
                _line((int)x1p1, (int)yp1, (int)x1p2, (int)yp2, false, dest);
            }
            else
            {
                _to2Da(x1m, y1m, z1m, out x1p1, out x2p1, out yp1);
                _to2Da(x2m, y2m, z2m, out x1p2, out x2p2, out yp2);
                _line((int)x2p1, (int)yp1, (int)x2p2, (int)yp2, false, dest);
                _line((int)x1p1, (int)yp1, (int)x1p2, (int)yp2, true, dest);
            }
        }

        public static void rectangle(int x0, int y0, int z0, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            int num = Convert.ToInt32(r1 * Math.Sin(f1 * Math.PI / 180.0) * Math.Cos(-t1 * Math.PI / 180.0));
            int num2 = Convert.ToInt32(r1 * Math.Sin(f1 * Math.PI / 180.0) * Math.Sin(-t1 * Math.PI / 180.0));
            int num3 = Convert.ToInt32(r1 * Math.Cos(f1 * Math.PI / 180.0));
            int num4 = Convert.ToInt32(r2 * Math.Sin(f2 * Math.PI / 180.0) * Math.Cos(-t2 * Math.PI / 180.0));
            int num5 = Convert.ToInt32(r2 * Math.Sin(f2 * Math.PI / 180.0) * Math.Sin(-t2 * Math.PI / 180.0));
            int num6 = Convert.ToInt32(r2 * Math.Cos(f2 * Math.PI / 180.0));
            line(x0, y0, z0, x0 + num, y0 + num2, z0 + num3, Graph.bitmap.Pixels);
            line(x0, y0, z0, x0 + num4, y0 + num5, z0 + num6, Graph.bitmap.Pixels);
            line(x0 + num, y0 + num2, z0 + num3, x0 + num + num4, y0 + num2 + num5, z0 + num3 + num6, Graph.bitmap.Pixels);
            line(x0 + num4, y0 + num5, z0 + num6, x0 + num + num4, y0 + num2 + num5, z0 + num3 + num6, Graph.bitmap.Pixels);
        }

        public static void fillrectangle(int x0, int y0, int z0, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            double T1 = -t1 * Math.PI / 180;
            double T2 = -t2 * Math.PI / 180;
            double F1 = f1 * Math.PI / 180;
            double F2 = f2 * Math.PI / 180;
            int x1 = Convert.ToInt32(r1 * Math.Sin(F1) * Math.Cos(T1));
            int y1 = Convert.ToInt32(r1 * Math.Sin(F1) * Math.Sin(T1));
            int z1 = Convert.ToInt32(r1 * Math.Cos(F1));
            int x2 = Convert.ToInt32(r2 * Math.Sin(F2) * Math.Cos(T2));
            int y2 = Convert.ToInt32(r2 * Math.Sin(F2) * Math.Sin(T2));
            int z2 = Convert.ToInt32(r2 * Math.Cos(F2));
            for (double i = 0; i < 1; i += 1.0 / FillLines)
            {
                int x = Convert.ToInt32(x1 * i + x0);
                int y = Convert.ToInt32(y1 * i + y0);
                int z = Convert.ToInt32(z1 * i + z0);
                line(x, y, z, x + x2, y + y2, z + z2, Graph.bitmap.Pixels);
                x = Convert.ToInt32(x2 * i + x0);
                y = Convert.ToInt32(y2 * i + y0);
                z = Convert.ToInt32(z2 * i + z0);
                line(x, y, z, x + x1, y + y1, z + z1, Graph.bitmap.Pixels);
            }
        }

        public static void ellipse(int x0, int y0, int z0, int startangle, int endangle, int r1, int f1, int t1, int r2, int f2, int t2, byte[] pixels)
        {
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            double num16 = -t1 * Math.PI / 180;
            double num17 = -t2 * Math.PI / 180;
            double num18 = f1 * Math.PI / 180;
            double num19 = f2 * Math.PI / 180;
            double num20 = r1 * Math.Sin(num18) * Math.Cos(num16);
            double num21 = r1 * Math.Sin(num18) * Math.Sin(num16);
            double num22 = r1 * Math.Cos(num18);
            double num23 = r2 * Math.Sin(num19) * Math.Cos(num17);
            double num24 = r2 * Math.Sin(num19) * Math.Sin(num17);
            double num25 = r2 * Math.Cos(num19);
            int num26 = 0;
            for (double i = startangle; i <= endangle; i++)
            {
                double num7;
                double num8;
                if ((i == 90.0) || (i == 450.0))
                {
                    num7 = 0f;
                    num8 = r2;
                }
                else if ((i == -90.0) || (i == 270.0))
                {
                    num7 = 0f;
                    num8 = -r2;
                }
                else
                {
                    double num9 = Math.Tan(i * Math.PI / 180.0);
                    num7 = r1 * r2 / Math.Sqrt(r2 * r2 + (((num9 * num9) * r1) * r1));
                    num8 = num7 * num9;
                    if ((i > 90.0) && (i < 270.0))
                    {
                        num7 = -num7;
                        num8 = -num8;
                    }
                }
                double num = (num7 / r1) * num20;
                double num2 = (num8 / r2) * num23;
                int num13 = Convert.ToInt32(num + num2);
                double num3 = (num7 / r1) * num21;
                double num4 = (num8 / r2) * num24;
                int num14 = Convert.ToInt32(num3 + num4);
                double num5 = (num7 / r1) * num22;
                double num6 = (num8 / r2) * num25;
                int num15 = Convert.ToInt32(num5 + num6);
                if (num26 == 1)
                {
                    line(x0 + num13, y0 + num14, z0 + num15, x0 + num10, y0 + num11, z0 + num12, pixels);
                }
                else
                {
                    num26 = 1;
                }
                num10 = num13;
                num11 = num14;
                num12 = num15;
            }
        }

        public static void fillellipse(int x0, int y0, int z0, int startangle, int endangle, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            int num10 = 0;
            int num11 = 0;
            int num12 = 0;
            int num16 = r1;
            int num17 = r2;
            double num18 = -t1 * Math.PI / 180.0;
            double num19 = -t2 * Math.PI / 180.0;
            double num20 = f1 * Math.PI / 180.0;
            double num21 = f2 * Math.PI / 180.0;
            for (double i = 1.0 / FillLines; i < 1.0 + 1.0 / FillLines; i += 1.0 / FillLines)
            {
                r1 = Convert.ToInt32(i * num16);
                r2 = Convert.ToInt32(i * num17);
                double num23 = r1 * Math.Sin(num20) * Math.Cos(num18);
                double num24 = r1 * Math.Sin(num20) * Math.Sin(num18);
                double num25 = r1 * Math.Cos(num20);
                double num26 = r2 * Math.Sin(num21) * Math.Cos(num19);
                double num27 = r2 * Math.Sin(num21) * Math.Sin(num19);
                double num28 = r2 * Math.Cos(num21);
                int num29 = 0;
                for (double j = startangle; j <= endangle; j++)
                {
                    double num7;
                    double num8;
                    if ((j == 90.0) || (j == 450.0))
                    {
                        num7 = 0.0;
                        num8 = r2;
                    }
                    else if ((j == -90.0) || (j == 270.0))
                    {
                        num7 = 0.0;
                        num8 = -r2;
                    }
                    else
                    {
                        double num9 = Math.Tan(j * Math.PI / 180.0);
                        num7 = (r1 * r2) / Math.Sqrt((r2 * r2) + num9 * num9 * r1 * r1);
                        num8 = num7 * num9;
                        if ((j > 90.0) && (j < 270.0))
                        {
                            num7 = -num7;
                            num8 = -num8;
                        }
                    }
                    double num = num7 / r1 * num23;
                    double num2 = num8 / r2 * num26;
                    int num13 = Convert.ToInt32(num + num2);
                    double num3 = num7 / r1 * num24;
                    double num4 = num8 / r2 * num27;
                    int num14 = Convert.ToInt32(num3 + num4);
                    double num5 = num7 / r1 * num25;
                    double num6 = num8 / r2 * num28;
                    int num15 = Convert.ToInt32(num5 + num6);
                    if (num29 == 1)
                    {
                        line(x0 + num13, y0 + num14, z0 + num15, x0 + num10, y0 + num11, z0 + num12, Graph.bitmap.Pixels);
                    }
                    else
                    {
                        num29 = 1;
                    }
                    num10 = num13;
                    num11 = num14;
                    num12 = num15;
                }
            }
        }

        public static void circle(int x0, int y0, int z0, int r)
        {
            int threadCount = 10;
            Thread[] threads = new Thread[threadCount - 1];
            byte[] pixels = Graph.bitmap.Pixels;
            double delta = 180 / threadCount;
            int i = 0;
            for (; i < threadCount - 1; i++)
            {
                threads[i] = new Thread(circleDraw);
                threads[i].Start(new object[] { x0, y0, z0, r, (int)(180 - i * delta), (int)(180 - (i + 1) * delta), pixels });
            }
            circleDraw(new object[] { x0, y0, z0, r, (int)(180 - i * delta), (int)(180 - (i + 1) * delta), pixels });
            for (i = 0; i < threadCount - 1; i++)
                while (threads[i].ThreadState == ThreadState.Running) ;
        }

        static void circleDraw(object param)
        {
            object[] o = (object[])param;
            int x0 = (int)o[0], y0 = (int)o[1], z0 = (int)o[2], r = (int)o[3], from = (int)o[4], to = (int)o[5];
            byte[] pixels = (byte[])o[6];
            for (int i = from; i >= to; i -= 750 / r)
            {
                ellipse(x0, y0, z0, 0, 180, r, 90, 0, r, 90 + i, -90, pixels);
                ellipse(x0, y0, z0, -90, 90, r, 90 + i, 0, r, 90, -90, pixels);
            }
        }
        #endregion

        public static void putpixel(int xm, int ym, int zm, Color color, byte[] pixels, int w, int h)
        {
            double x1p, x2p, yp;
            switch (Graph3d.glass)
            {
                case Glass.None:
                    _to2D(xm, ym, zm, out x1p, out yp);
                    Bitmap.SetPixel((int)x1p, (int)yp, color, pixels, w, h);
                    return;

                case Glass.Anaglyph:
                    _to2Da(xm, ym, zm, out x1p, out x2p, out yp);
                    Color anaglyph = AnaglyphLeft(color, (int)yp, pixels, (int)x1p);
                    Bitmap.SetPixel((int)x1p, (int)yp, anaglyph, pixels, w, h);

                    anaglyph = AnaglyphRight(color, (int)yp, pixels, (int)x2p);
                    Bitmap.SetPixel((int)x2p, (int)yp, anaglyph, pixels, w, h);
                    return;

                case Glass.TV3D:
                    _to2Da(xm, ym, zm, out x1p, out x2p, out yp);
                    if (x1p < Graph.width)
                        Bitmap.SetPixel((int)(x2p / 2), (int)yp, color, pixels, w, h);
                    if (x2p >= 0)
                        Bitmap.SetPixel((int)((x1p + Graph.width) / 2), (int)yp, color, pixels, w, h);
                    break;
            }
        }

        public static Color AnaglyphLeft(Color color, int y, byte[] background, int i)
        {
            Color back = Bitmap.GetPixel(i, y, background, Graph.width, Graph.height);
            switch (Method)
            {
                case AnaglyphMethod.Color:
                    return Color.PutAonB(Color.FromARGB(color.a, color.r, back.g, back.b), back);
                case AnaglyphMethod.Optimized:
                    return Color.PutAonB(Color.FromARGB(color.a, (byte)(0.7 * color.g + 0.3 * color.b), back.g, back.b), back);
                default:
                    return Color.PutAonB(Color.FromARGB(color.a, (byte)(0.299 * color.r + 0.587 * color.g + 0.114 * color.b), back.g, back.b), back);
            }
        }

        public static Color AnaglyphRight(Color color, int y, byte[] background, int i)
        {
            Color back = Bitmap.GetPixel(i, y, background, Graph.width, Graph.height);
            switch (Method)
            {
                case AnaglyphMethod.True:
                    return Color.PutAonB(Color.FromARGB(color.a, back.r, back.g, (byte)(0.299 * color.r + 0.587 * color.g + 0.114 * color.b)), back);
                case AnaglyphMethod.Gray:
                    return Color.PutAonB(Color.FromARGB(color.a, back.r, (byte)(0.299 * color.r + 0.587 * color.g + 0.114 * color.b), (byte)(0.299 * color.r + 0.587 * color.g + 0.114 * color.b)), back);
                default:
                    return Color.PutAonB(Color.FromARGB(color.a, back.r, color.g, color.b), back);
            }
        }

        public static void putpixel(int xm, int ym, int zm, byte[] pixels, int w, int h)
        {
            putpixel(xm, ym, zm, Graph.color, pixels, w, h);
        }


        public static void _outtextxy(int x, int y, int z, string s, int angle)
        {
            double xp1, xp2, yp, yp2;
            _to2Da(x, y, z, out xp1, out xp2, out yp);
            _to2Da(x, y + (int)Graph.font.Size, z, out xp1, out xp2, out yp2);
            Font font = new Font(Graph.font.Name, (int)(yp2 - yp) + 1, Graph.font.Style, GraphicsUnit.Pixel);

            //node p = Queues.SearchTexts(s, font);
            //Bitmap pic = null;
            //if (p.data == null)
            //{
            //    SizeF size = Graphics.FromImage(Graph.bitmap.SysDraw).MeasureString(s, font);
            //    p.data = new Bitmap((int)size.Width, (int)size.Height);
            //    pic = (Bitmap)p.data;
            //    Graphics.FromImage(pic.SysDraw).DrawString(s, font, Graph.brush, 0, 0);
            //}
            //else
            //    pic = (Bitmap)p.data;

            SizeF size = Graphics.FromImage(Graph.bitmap.SysDraw).MeasureString(s, font);
            Bitmap pic = new Bitmap((int)size.Width, (int)size.Height);
            Graphics.FromImage(pic.SysDraw).DrawString(s, font, Graph.brush, 0, 0);
            Graph3dImage._drawImage(x, y, z, pic.Width, 90, angle, pic.Height, 90, angle - 90, pic, Graph.bitmap.Pixels);
        }

        public static void outtextxy(int x, int y, int z, string s)
        {
            _outtextxy(x, y, z, s, 0);
        }

    }
}
