using System;
using System.Collections.Generic;

namespace GraphDLL
{
    public static partial class Graph3d
    {
        internal static Glass glass = Glass.None;
        internal static bool mouse3da = false;
        internal static int _ye = 240, _ze = -1050, _x1e = 290, _x2e = 350;
        internal static int _xe
        {
            get
            {
                return (_x1e + _x2e) / 2;
            }
        }


        public static void SetEyeDistnce(int d)
        {
            _x1e = _xe - d;
            _x2e = _xe + d;
        }
        public static int GetEyeDistnce()
        {
            return (_x2e - _x1e) / 2;
        }

        public static void setfillstyle(int x)
        {
            Graph3dDraw.FillLines = x;
        }
        public static int getfillstyle()
        {
            return Graph3dDraw.FillLines;
        }

        public static void set3DGlass(Glass glass)
        {
            Graph3d.glass = glass;
        }

        public static void SetAnaglyphMethod(AnaglyphMethod method)
        {
            Graph3dDraw.Method = method;
        }

        public static void setlefteye(int x, int y, int z)
        {
            _x1e = x;
            _ye = y;
            _ze = z;
        }
        public static void setrighteye(int x, int y, int z)
        {
            _x2e = x;
            _ye = y;
            _ze = z;
        }
        public static void putpixel(int x, int y, int z, Color c)
        {
            putpixel(x, y, z, c, Graph.bitmap.Pixels, Graph.width, Graph.height);
        }
        public static void putpixel(int x, int y, int z, Color c, byte[] pixels, int w, int h)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.putpixel(x, y, z, c, pixels, w, h);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void line(int x1m, int y1m, int z1m, int x2m, int y2m, int z2m)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.line(x1m, y1m, z1m, x2m, y2m, z2m, Graph.bitmap.Pixels);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }


        public static void rectangle(int x0, int y0, int z0, int Xscale, int Yscale, Surface s)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            switch (s)
            {
                case Surface.XY:
                    Graph3dDraw.rectangle(x0, y0, z0, Xscale, 90, 0, Yscale, 90, -90); break;
                case Surface.XZ:
                    Graph3dDraw.rectangle(x0, y0, z0, Xscale, 90, 0, Yscale, 0, -90); break;
                case Surface.YZ:
                    Graph3dDraw.rectangle(x0, y0, z0, Xscale, 0, -90, Yscale, 90, -90); break;
            }
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }
        public static void rectangle(int x0, int y0, int z0, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.rectangle(x0, y0, z0, r1, f1, t1, r2, f2, t2);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void fillrectangle(int x0, int y0, int z0, int Xscale, int Yscale, Surface s)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            switch (s)
            {
                case Surface.XY:
                    Graph3dDraw.fillrectangle(x0, y0, z0, Xscale, 90, 0, Yscale, 90, -90); break;
                case Surface.XZ:
                    Graph3dDraw.fillrectangle(x0, y0, z0, Xscale, 90, 0, Yscale, 0, -90); break;
                case Surface.YZ:
                    Graph3dDraw.fillrectangle(x0, y0, z0, Xscale, 0, -90, Yscale, 90, -90); break;
            }
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }
        public static void fillrectangle(int x0, int y0, int z0, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.fillrectangle(x0, y0, z0, r1, f1, t1, r2, f2, t2);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void image(int x0, int y0, int z0, int Xscale, int Yscale, Surface s, Bitmap image)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            switch (s)
            {
                case Surface.XY:
                    Graph3dImage.image(x0, y0, z0, Xscale, 90, 0, Yscale, 90, -90, image); break;
                case Surface.XZ:
                    Graph3dImage.image(x0, y0, z0, Xscale, 90, 0, Yscale, 0, -90, image); break;
                case Surface.YZ:
                    Graph3dImage.image(x0, y0, z0, Xscale, 0, -90, Yscale, 90, -90, image); break;
            }
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }
        public static void image(int x0, int y0, int z0, int Xscale, int Xfi, int Xteta, int Yscale, int Yfi, int Yteta, Bitmap image)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dImage.image(x0, y0, z0, Xscale, Xfi, Xteta, Yscale, Yfi, Yteta, image);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void stereoImage(int x0, int y0, int z0, int Xscale, int Yscale, Surface s, Bitmap image)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            switch (s)
            {
                case Surface.XY:
                    Graph3dStereoImage.image(x0, y0, z0, Xscale, 90, 0, Yscale, 90, -90, image); break;
                case Surface.XZ:
                    Graph3dStereoImage.image(x0, y0, z0, Xscale, 90, 0, Yscale, 0, -90, image); break;
                case Surface.YZ:
                    Graph3dStereoImage.image(x0, y0, z0, Xscale, 0, -90, Yscale, 90, -90, image); break;
            }
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }
        public static void stereoImage(int x0, int y0, int z0, int Xscale, int Xfi, int Xteta, int Yscale, int Yfi, int Yteta, Bitmap image)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dStereoImage.image(x0, y0, z0, Xscale, Xfi, Xteta, Yscale, Yfi, Yteta, image);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void ellipse(int x0, int y0, int z0, int startangle, int endangle, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.ellipse(x0, y0, z0, startangle, endangle, r1, f1, t1, r2, f2, t2, Graph.bitmap.Pixels);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void fillellipse(int x0, int y0, int z0, int startangle, int endangle, int r1, int f1, int t1, int r2, int f2, int t2)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.fillellipse(x0, y0, z0, startangle, endangle, r1, f1, t1, r2, f2, t2);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void circle(int x, int y, int z, int r)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.circle(x, y, z, r);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }

        public static void outtextxy(int x, int y, int z, string s)
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = false;
            Graph3dDraw.outtextxy(x, y, z, s);
            Graph.imediateDrawing = temp;
            if (Graph.imediateDrawing) Graph.delay(0);
        }
    }
}
