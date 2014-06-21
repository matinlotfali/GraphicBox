using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace GraphDLL
{
    static class Graph3dStereoImage
    {
        static Bitmap _image;
        static byte[] dest, src;
        static int x0, y0, z0;
        static double AX, BX, CX, AY, BY, CY, _fill_lines;

        public static void image(int x0, int y0, int z0, int Xsize, int Xfi, int Xteta, int Ysize, int Yfi, int Yteta, Bitmap image)
        {
            image = Queues.SearchPicture(image, Xsize, Ysize);
            _drawImage(x0, y0, z0, Xsize, Xfi, Xteta, Ysize, Yfi, Yteta, image, Graph.bitmap.Pixels);
        }

        public static void _drawImage(int x0, int y0, int z0, int Xsize, int Xfi, int Xteta, int Ysize, int Yfi, int Yteta, Bitmap image, byte[] dest)
        {
            double x1p0, x2p0, yp0, x1p1, x2p1, yp1;
            double XTeta = -Xteta * Math.PI / 180;
            double YTeta = -Yteta * Math.PI / 180;
            double XFi = Xfi * Math.PI / 180;
            double YFi = Yfi * Math.PI / 180;
            AX = Math.Sin(XFi) * Math.Cos(XTeta) * Xsize;
            BX = Math.Sin(XFi) * Math.Sin(XTeta) * Xsize;
            CX = Math.Cos(XFi) * Xsize;
            AY = Math.Sin(YFi) * Math.Cos(YTeta) * Ysize;
            BY = Math.Sin(YFi) * Math.Sin(YTeta) * Ysize;
            CY = Math.Cos(YFi) * Ysize;

            if (Xfi <= 90)
            {
                Graph3dDraw._to2Da((double)x0, (double)y0, (double)z0, out x1p0, out x2p0, out yp0);
                Graph3dDraw._to2Da((double)x0 + AY, (double)y0 + BY, (double)z0 + CY, out x1p1, out x2p1, out yp1);
            }
            else
            {
                Graph3dDraw._to2Da(x0 + AX, y0 + BX, z0 + CX, out x1p0, out x2p0, out yp0);
                Graph3dDraw._to2Da(x0 + (AX + AY), y0 + (BX + BY), z0 + (CX + CY), out x1p1, out x2p1, out yp1);
            }
            double dy = Math.Pow(yp1 - yp0, 2);
            double dx1 = Math.Sqrt(Math.Pow(x1p1 - x1p0, 2) + dy);
            double dx2 = Math.Sqrt(Math.Pow(x2p1 - x2p0, 2) + dy);
            _fill_lines = (dx1 > dx2) ? dx1 : dx2;
            _fill_lines *= 1.2;

            Graph3dStereoImage._image = image;
            Graph3dStereoImage.src = image.Pixels;
            Graph3dStereoImage.dest = dest;
            Graph3dStereoImage.x0 = x0;
            Graph3dStereoImage.y0 = y0;
            Graph3dStereoImage.z0 = z0;
            int threadCount = Environment.ProcessorCount;
            double delta = _fill_lines / threadCount;
            Thread[] thread = new Thread[threadCount];
            int i = 0;
            for (; i < threadCount - 1; i++)
            {
                thread[i] = new Thread(testThread);
                thread[i].Start(new object[] { i * delta, (i + 1) * delta });
            }
            testThread(new object[] { i * delta, (i + 1) * delta });

            for (i = 0; i < threadCount - 1; i++)
                while (thread[i].ThreadState == ThreadState.Running) ;
        }

        private static void testThread(object param)
        {
            object[] o = (object[])param;
            double from = (double)o[0], to = (double)o[1];
            for (double t = from; t < to; t++)
            {
                double x = AY * t / _fill_lines + x0;
                double y = BY * t / _fill_lines + y0;
                double z = CY * t / _fill_lines + z0;
                lineImage(x, y, z, x + AX, y + BX, z + CX, t);
            }
        }


        private static void lineImage(double x1m, double y1m, double z1m, double x2m, double y2m, double z2m, double time)
        {
            double x1p1, x2p1, yp1, x1p2, x2p2, yp2;
            if (Graph3d.glass == Glass.None)
            {
                Graph3dDraw._to2D(x1m, y1m, z1m, out x1p1, out yp1);
                Graph3dDraw._to2D(x2m, y2m, z2m, out x1p2, out yp2);
                lineImageDraw(x1p1, yp1, x1p2, yp2, false, time);
            }
            else
            {
                Graph3dDraw._to2Da(x1m, y1m, z1m, out x1p1, out x2p1, out yp1);
                Graph3dDraw._to2Da(x2m, y2m, z2m, out x1p2, out x2p2, out yp2);
                lineImageDraw(x2p1, yp1, x2p2, yp2, false, time);
                lineImageDraw(x1p1, yp1, x1p2, yp2, true, time);
            }
        }

        private static void lineImageDraw(double x1, double y1, double x2, double y2, bool Left, double time)
        {
            int Xsize = _image.Width / 2;
            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
            {
                if (x1 > x2) { double t = x2; x2 = x1; x1 = t; t = y2; y2 = y1; y1 = t; }
                for (double i = x1; i < x2; i++)
                {
                    double j = y1 + (y2 - y1) * (i - x1) / (x2 - x1);

                    double z = (i - x1) * Xsize / (x2 - x1);
                    double k = time * _image.Height / _fill_lines;
                    Color c = Left ?
                        Bitmap.GetPixel((int)z, (int)k, src, _image.Width, _image.Height) :
                        Bitmap.GetPixel((int)z + Xsize, (int)k, src, _image.Width, _image.Height);
                    drawPixel(Left, i, j, c);
                }
            }
            else
            {
                if (y1 > y2) { double t = y2; y2 = y1; y1 = t; t = x2; x2 = x1; x1 = t; }
                for (double j = y1; j < y2; j++)
                {
                    double i = x1 + (x2 - x1) * (j - y1) / (y2 - y1);

                    double z = time * _image.Height / _fill_lines;
                    double k = (j - y1) * _image.Width / (y2 - y1);
                    Color c = Left ?
                        Bitmap.GetPixel((int)k, (int)z, src, _image.Width, _image.Height) :
                        Bitmap.GetPixel((int)k + Xsize, (int)z, src, _image.Width, _image.Height);
                    drawPixel(Left, i, j, c);
                }
            }
        }

        private static void drawPixel(bool Left, double i, double j, Color c)
        {
            if (c.a > 0)
                switch (Graph3d.glass)
                {
                    case Glass.None:
                        Color back = Graph.getpixel((int)i, (int)j);
                        Bitmap.SetPixel((int)i, (int)j, Color.PutAonB(c, back), dest, Graph.width, Graph.height);
                        break;
                    case Glass.Anaglyph:
                        Color anaglyph = Left ?
                            Graph3dDraw.AnaglyphLeft(c, (int)j, Graph.bitmap.Pixels, (int)i) :
                            Graph3dDraw.AnaglyphRight(c, (int)j, Graph.bitmap.Pixels, (int)i);
                        Bitmap.SetPixel((int)i, (int)j, anaglyph, dest, Graph.width, Graph.height);
                        break;
                    case Glass.TV3D:
                        if (!Left)
                        {
                            if (i < Graph.width)
                            {
                                back = Graph.getpixel((int)(i / 2), (int)j);
                                Bitmap.SetPixel((int)(i / 2), (int)j, Color.PutAonB(c, back), dest, Graph.width, Graph.height);
                            }
                        }
                        else
                        {
                            if (i >= 0)
                            {
                                back = Graph.getpixel((int)((i + Graph.width) / 2), (int)j);
                                Bitmap.SetPixel((int)((i + Graph.width) / 2), (int)j, Color.PutAonB(c, back), dest, Graph.width, Graph.height);
                            }
                        }
                        break;
                }
        }
    }
}
