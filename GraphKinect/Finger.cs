using GraphDLL;
using System.Windows.Forms;
using GraphKinectDLL;
using System;
using System.IO;

namespace GraphKinectDLL
{
    public static partial class GKinect
    {
        static bool Show = true;
        static int x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0, x3 = 0, y3 = 0, z3 = 0, x4 = 0, y4 = 0, z4 = 0;
        static double x12, y12, z12, x13, y13, z13;
        static double A, B, C, D, A2B2C2;
        static int xLeftEye = 0, yLeftEye = 0, zLeftEye = 0;
        static int SeenEyeX = 0, SeenEyeY = 0, SeenEyeZ = 0;
        static int xLeftEyeInMonitor = -1, yLeftEyeInMonitor = -1, zLeftEyeInMonitor = -1;
        static int xRightEyeInMonitor = -1, yRightEyeInMonitor = -1, zRightEyeInMonitor = -1;
        static int xNose = -1, yNose = -1, zNose = -1, dxLeftEye, dyLeftEye, dzLeftEye;
        static int xSkull = -1, ySkull = -1, zSkull = -1;
        static int fingerReadCounter = 0;
        static bool fingerPrediction = false;
        static int[,] fingerLocation;
        static int noseReadCounter = 0;
        static bool nosePrediction = false;
        static int[,] noseLocation;
        static int KinectAngle = 0;
        static int Smooth = 0;
        static double PixelLength;
        static int MonitorInch;
        static bool loaded;

        static void WriteMonitorIntoFile()
        {
            FileStream f = new FileStream("Monitor.txt",
                   FileMode.Create, FileAccess.Write, FileShare.None);
            StreamWriter w = new StreamWriter(f);
            w.WriteLine(MonitorInch);
            w.WriteLine(PixelLength);
            w.WriteLine(KinectAngle);
            w.WriteLine(x1);
            w.WriteLine(y1);
            w.WriteLine(z1);
            w.WriteLine(x2);
            w.WriteLine(y2);
            w.WriteLine(z2);
            w.WriteLine(x3);
            w.WriteLine(y3);
            w.WriteLine(z3);
            w.WriteLine(x4);
            w.WriteLine(y4);
            w.WriteLine(z4);
            w.Close();
            loaded = true;
        }

        static bool ReadMonitorFromFile()
        {
            StreamReader r = null;
            try
            {
                FileStream f = new FileStream("Monitor.txt",
                            FileMode.Open, FileAccess.Read, FileShare.None);
                r = new StreamReader(f);
                MonitorInch = Convert.ToInt32(r.ReadLine());
                PixelLength = Convert.ToDouble(r.ReadLine());
                KinectAngle = Convert.ToInt32(r.ReadLine());
                CameraVerticalAngle = KinectAngle;
                x1 = Convert.ToInt32(r.ReadLine());
                y1 = Convert.ToInt32(r.ReadLine());
                z1 = Convert.ToInt32(r.ReadLine());
                x2 = Convert.ToInt32(r.ReadLine());
                y2 = Convert.ToInt32(r.ReadLine());
                z2 = Convert.ToInt32(r.ReadLine());
                x3 = Convert.ToInt32(r.ReadLine());
                y3 = Convert.ToInt32(r.ReadLine());
                z3 = Convert.ToInt32(r.ReadLine());
                x4 = Convert.ToInt32(r.ReadLine());
                y4 = Convert.ToInt32(r.ReadLine());
                z4 = Convert.ToInt32(r.ReadLine());
                loaded = true;
            }
            catch
            {
                loaded = false;
            }
            finally
            {
                if (r != null)
                    r.Close();
            }

            return loaded;
        }

        static Bitmap wall = new Bitmap(GraphKinect.Properties.Resources.Wall);
        static bool _ResetMonitorPoints()
        {
            int xLeft = (Graph.width - 640) / 2;
            int yTop = (Graph.height - 480) / 2;
            int xRight = Graph.width - xLeft;
            int yDown = Graph.height - yTop;
            int xm, ym, zm, click, turn = 1;
            string str = "Click on Top-Left point of screen!";
            Graph.getmouse(out xm, out ym, out click);
            Graph.setfont("Times New Roman", FontStyle.Bold, 28);
            while (!Graph.keydown(Keys.Enter) && (!loaded))
            {
                Graph.putimage(wall, 0, 0, Graph.width, Graph.height);
                for (int i = 0; i < 640; i++)
                    for (int j = 0; j < 480; j++)
                    {
                        Color col = GKinect.GetColorPixel(i, j);
                        Graph.putpixel(i + xLeft, j + yTop, col);
                    }
                Graph.setcolor(Color.Black);
                Graph.outtextxy(0, 0, "Correct Kinect Vertical camera angle by UP and DOWN and then press Enter.");
                Graph.outtextxy(Graph.width - 200, Graph.height - 30, "Kinect Camera");
                Graph.delay(0);
                if (Graph.keydown(Keys.Up))
                {
                    if (KinectAngle < 27) KinectAngle += 2;
                    CameraVerticalAngle = KinectAngle;
                }
                if (Graph.keydown(Keys.Down))
                {
                    if (KinectAngle > -27) KinectAngle -= 2;
                    CameraVerticalAngle = KinectAngle;
                }
                if (Graph.keydown(Keys.Escape))
                    return false;
            }
            if (loaded)
            {
                str = "Put your 3D glass and then click on Your eye!"; turn = 5;
            }
            while (turn < 6)
            {
                Graph.putimage(wall, 0, 0, Graph.width, Graph.height);
                for (int i = 0; i < 640; i++)
                    for (int j = 0; j < 480; j++)
                        Graph.putpixel(i + xLeft, j + yTop, GKinect.GetColorPixel(i, j));
                Graph.setcolor(Color.Black);
                Graph.setfont("Times New Roman", FontStyle.Bold, 28);
                Graph.outtextxy(Graph.width - 200, Graph.height - 30, "Kinect Camera");
                Graph.setfont("Times New Roman", FontStyle.Bold, 28);
                Graph.outtextxy(0, 0, str);
                Graph.getmouse(out xm, out ym, out click);
                zm = -1;
                if (xm >= xLeft && xm < xRight && ym >= yTop && ym < yDown)
                {
                    Graph.setcolor(Color.Yellow);
                    Graph.line(xm, yTop, xm, ym - 6);
                    Graph.line(xm - 1, yTop, xm - 1, ym - 26);
                    Graph.line(xm + 1, yTop, xm + 1, ym - 26);
                    Graph.line(xm, yDown, xm, ym + 6);
                    Graph.line(xm + 1, yDown, xm + 1, ym + 26);
                    Graph.line(xm - 1, yDown, xm - 1, ym + 26);
                    Graph.line(xLeft, ym, xm - 6, ym);
                    Graph.line(xLeft, ym + 1, xm - 26, ym + 1);
                    Graph.line(xLeft, ym - 1, xm - 26, ym - 1);
                    Graph.line(xm + 6, ym, xRight, ym);
                    Graph.line(xm + 26, ym + 1, xRight, ym + 1);
                    Graph.line(xm + 26, ym - 1, xRight, ym - 1);
                    Graph.setcolor(Color.White);
                    Graph.circle(xm, ym, 5);
                    Graph.circle(xm, ym, 6);
                    if (xm - 40 / 3 >= xLeft && xm + 40 / 3 < xRight && ym - 40 / 3 >= yTop && ym + 40 / 3 < yDown)
                    {
                        zm = GKinect.GetDepthPixel(xm - xLeft, ym - yTop, MapToCamera.Color);
                        Graph.setfont("Times New Roman", FontStyle.Bold, 16);
                        Graph.setcolor(Color.Blue);
                        Graph.outtextxy(xm - 160, ym - 30, "Distance = " + zm / 10.0 + " cm");
                        for (int i = -40; i < 40; i++)
                            for (int j = -40; j < 40; j++)
                                if (i * i + j * j < 1600)
                                    Graph.putpixel(xm - 50 + i, ym + 50 + j,
                                        GKinect.GetColorPixel(xm - xLeft + i / 3, ym - yTop + j / 3));
                        if (zm == -1) Graph.setcolor(Color.Red);
                        else Graph.setcolor(Color.White);
                        Graph.circle(xm - 50, ym + 50, 40);
                        Graph.circle(xm - 50, ym + 50, 41);
                        if (zm == -1) Graph.setcolor(Color.Red);
                        else Graph.setcolor(Color.Yellow);
                        Graph.line(xm - 90, ym + 50, xm - 10, ym + 50);
                        Graph.line(xm - 50, ym + 10, xm - 50, ym + 90);
                    }
                }
                switch (turn)
                {
                    case 1:
                        if (click == 1 && zm != -1)
                        {
                            x1 = xm - xLeft;
                            y1 = ym - yTop;
                            z1 = zm;
                            str = "Click on Down-Left point of screen!"; turn++;
                            while (click != 0) Graph.getmouse(out xm, out ym, out click);
                        }
                        break;
                    case 2:
                        Graph.setcolor(Color.Blue);
                        for (int r = 0; r <= 5; r++)
                            Graph.circle(x1 + xLeft, y1 + yTop, r);
                        if (click == 1 && zm != -1)
                        {
                            x3 = xm - xLeft;
                            y3 = ym - yTop;
                            z3 = zm;
                            str = "Click on Top-Right point of screen!"; turn++;
                            while (click != 0) Graph.getmouse(out xm, out ym, out click);
                        }
                        break;
                    case 3:
                        Graph.setcolor(Color.Blue);
                        for (int r = 0; r <= 5; r++)
                        {
                            Graph.circle(x1 + xLeft, y1 + yTop, r);
                            Graph.circle(x3 + xLeft, y3 + yTop, r);
                        }
                        if (click == 1 && zm != -1)
                        {
                            x2 = xm - xLeft;
                            y2 = ym - yTop;
                            z2 = zm;
                            str = "Click on Down-Right point of screen!"; turn++;
                            while (click != 0) Graph.getmouse(out xm, out ym, out click);
                        }
                        break;
                    case 4:
                        Graph.setcolor(Color.Blue);
                        for (int r = 0; r <= 5; r++)
                        {
                            Graph.circle(x1 + xLeft, y1 + yTop, r);
                            Graph.circle(x2 + xLeft, y2 + yTop, r);
                            Graph.circle(x3 + xLeft, y3 + yTop, r);
                        }
                        if (click == 1 && zm != -1)
                        {
                            x4 = xm - xLeft;
                            y4 = ym - yTop;
                            z4 = zm;
                            //dxLeftEye = xLeftEye - xHead; dyLeftEye = yLeftEye - yHead; dzLeftEye = zLeftEye - zHead;
                            str = "Put your 3D glass and then click on Your eye!"; turn++;
                            while (click != 0) Graph.getmouse(out xm, out ym, out click);
                        }
                        break;
                    case 5:
                        Graph.setcolor(Color.Blue);
                        for (int r = 0; r <= 5; r++)
                        {
                            Graph.circle(x1 + xLeft, y1 + yTop, r);
                            Graph.circle(x2 + xLeft, y2 + yTop, r);
                            Graph.circle(x3 + xLeft, y3 + yTop, r);
                            Graph.circle(x4 + xLeft, y4 + yTop, r);
                        }
                        if (click == 1 && zm != -1)
                        {
                            xLeftEye = xm - xLeft;
                            yLeftEye = ym - yTop;
                            zLeftEye = zm;
                            SeenEyeX = xLeftEye; SeenEyeY = yLeftEye; SeenEyeZ = zLeftEye;
                            //dxLeftEye = xLeftEye - xHead; dyLeftEye = yLeftEye - yHead; dzLeftEye = zLeftEye - zHead;
                            turn++;
                            while (click != 0) Graph.getmouse(out xm, out ym, out click);
                            WriteMonitorIntoFile();
                        }
                        break;
                }
                Graph.delay(0);
                if (Graph.keydown(Keys.Escape))
                {
                    while (click != 0) Graph.getmouse(out xm, out ym, out click);
                    return false;
                }
            }
            return true;
        }

        static bool _ResetMonitorEquation()
        {
            x12 = (x2 - x1) * 1.0 / Graph.width; y12 = (y2 - y1) * 1.0 / Graph.width; z12 = (z2 - z1) * 1.0 / Graph.width;
            x13 = (x3 - x1) * 1.0 / Graph.height; y13 = (y3 - y1) * 1.0 / Graph.height; z13 = (z3 - z1) * 1.0 / Graph.height;
            A = y13 * z12 - y12 * z13;
            B = x12 * z13 - x13 * z12;
            C = x13 * y12 - x12 * y13;
            if (y13 == 0)
            {
                Graph.outtextxy(0, 0, "Error: Do not lay down monitor screen!");
                return false;
            }
            if (A == 0)
            {
                Graph.outtextxy(0, 0, "Error: Do not Place the Kinect right in front of monitor!");
                return false;
            }
            // |Z| = |X| * |Y| => |Z / zResizeScale| = |X|
            double zResizeScale = Math.Sqrt(Math.Sqrt(A * A + B * B + C * C));
            A /= zResizeScale;
            B /= zResizeScale;
            C /= zResizeScale;
            A2B2C2 = A * A + B * B + C * C;
            D = A * x1 + B * y1 + C * z1;
            return true;
        }

        static void _KinectToMonitor(int xKinect, int yKinect, int zKinect, out int xMonitor, out int yMonitor, out int zMonitor)
        {
            double t = (D - (A * xKinect + B * yKinect + C * zKinect))
                / A2B2C2;
            double xpk = A * t + xKinect;
            double ypk = B * t + yKinect;
            double zpk = C * t + zKinect;
            zMonitor = 0 - (int)(Math.Sqrt((Math.Pow(xpk - xKinect, 2)
                + Math.Pow(ypk - yKinect, 2)
                + Math.Pow(zpk - zKinect, 2)) / A2B2C2));
            xMonitor = (int)((y13 * (zpk - z1) - z13 * (ypk - y1))
                / (y13 * z12 - y12 * z13));
            yMonitor = (int)((ypk - y1 - y12 * xMonitor) / y13);
            yMonitor = (int)(yMonitor * (1 +
                1.0 * xMonitor * (y3 - y1 + y2 - y4) / (Graph.width * (y4 - y2))));
        }

        static void _ResetEyes()
        {
            _KinectToMonitor(xLeftEye, yLeftEye, zLeftEye,
                out xLeftEyeInMonitor, out yLeftEyeInMonitor, out zLeftEyeInMonitor);
            Graph3d.setlefteye(xLeftEyeInMonitor, yLeftEyeInMonitor, zLeftEyeInMonitor);
            double EyesDistance = 65 / PixelLength;
            _KinectToMonitor(
                (int)(xLeftEye + x12 * EyesDistance),
                (int)(yLeftEye + y12 * EyesDistance),
                (int)(zLeftEye + z12 * EyesDistance)
                , out xRightEyeInMonitor, out yRightEyeInMonitor, out zRightEyeInMonitor);
            Graph3d.setrighteye(xRightEyeInMonitor, yRightEyeInMonitor, zRightEyeInMonitor);
        }

        static bool KinectCalibration()
        {
            if (!_ResetMonitorPoints()) return false;
            bool flag = _ResetMonitorEquation();
            if (flag)
            {
                _ResetEyes();
                xNose = -1; yNose = -1; zNose = -1;
            }
            return flag;
        }

        static bool FingerTracking(out int xFinger, out int yFinger,
            out int zFinger, out int NoseX, out int NoseY, out int NoseZ
            , out int SkullX, out int SkullY, out int SkullZ)
        {
            int SkullMin, SkullMax = -3;
            xFinger = -1; yFinger = -1; zFinger = -1;
            NoseX = -1; NoseY = -1; NoseZ = -1;
            SkullX = -1; SkullY = -1; SkullZ = -1;
            int xf = -1, yf = -1, zf = -1;
            int x, y, z = 10000;
            int xp1, xp3, yp1, yp3, l, dl;
            int NoseSearchArea = 150; // (int)(z12 * Graph.width / 3);
            int zBase;
            if (zNose > 0) zBase = zNose;
            else zBase = zLeftEye;
            bool FingerOrNose = true, Exit = false;
            if (Show)
            {
                for (int i = xLeftEye > 20 ? xLeftEye - 20 : 0; i < x1; i++)
                    for (int j = yLeftEye > 50 ? yLeftEye - 50 : 0; j < y3; j++)
                        Graph.putpixel(Graph.width - x1 + i, Graph.height - y3 + j, GKinect.GetColorPixel(i, j));
                Graph.setcolor(Color.White);
                Graph.line(Graph.width - x1 + x1, Graph.height - y3 + y1, Graph.width - x1 + x3, Graph.height - y3 + y3);
                Graph.line(Graph.width - x1 + x1, Graph.height - y3 + y1, Graph.width - x1 + x2, Graph.height - y3 + y2);
                Graph.line(Graph.width - x1 + x3, Graph.height - y3 + y3, Graph.width - x1 + x4, Graph.height - y3 + y4);
                Graph.line(Graph.width - x1 + x2, Graph.height - y3 + y2, Graph.width - x1 + x4, Graph.height - y3 + y4);
                Graph.setcolor(Color.Yellow);
                Graph.line(Graph.width - x1 + x1, Graph.height - y3 + y1, Graph.width - x1 + xLeftEye, Graph.height - y3 + yLeftEye);
                Graph.line(Graph.width - x1 + x2, Graph.height - y3 + y2, Graph.width - x1 + xLeftEye, Graph.height - y3 + yLeftEye);
                Graph.line(Graph.width - x1 + x3, Graph.height - y3 + y3, Graph.width - x1 + xLeftEye, Graph.height - y3 + yLeftEye);
                Graph.line(Graph.width - x1 + x4, Graph.height - y3 + y4, Graph.width - x1 + xLeftEye, Graph.height - y3 + yLeftEye);
                Graph.setcolor(Color.Blue);
                //Graph.setfont("Times New Roman", FontStyle.Bold, 20);
                Graph.outtextxy(Graph.width - 200, Graph.height - 30, "Kinect Camera");
                //Graph.outtextxy(xLeftEye, yLeftEye - 20, "EYE: X=" + xLeftEye + " Y=" + yLeftEye + " Z=" + zLeftEye);
            }
            l = (int)Math.Sqrt((x3 - x1) * (x3 - x1) + (y3 - y1) * (y3 - y1));
            int xLast = (int)(xLeftEye - 10 / PixelLength);
            if (xLast < 0)
                xLast = 0;
            for (xp1 = x1; (xp1 > xLast) && (!Exit); xp1--)
            {
                double d = 1.0 * (xp1 - xLeftEye) / (x1 - xLeftEye);
                //if (d < 0.2) break; // End of Refresh Area near to eye
                if (FingerOrNose)
                    if (d < 0.25) { FingerOrNose = false; SkullMax = -3; continue; } else { } // End of Refresh Area for fingers (near to eyes)
                else
                    if (d >= 0.25) continue;
                yp1 = (int)(yLeftEye + d * (y1 - yLeftEye));
                if (FingerOrNose)
                {
                    xp3 = (int)(xLeftEye + d * (x3 - xLeftEye));
                    yp3 = (int)(yLeftEye + d * (y3 - yLeftEye));
                }
                else
                {
                    xp3 = (int)(xp1 + x13 * 100 / PixelLength);
                    yp3 = (int)(yp1 + y13 * 100 / PixelLength);
                }
                int TempX = (xp1 - xp3);
                TempX *= TempX;
                int TempY = (yp1 - yp3);
                TempY *= TempY;
                l = (int)Math.Sqrt(TempX + TempY);
                SkullMin = FingerOrNose ? 0 : -l;
                if (FingerOrNose) SkullMax = l;
                else if (SkullMax == -3) SkullMax = l;
                for (dl = SkullMin; dl <= SkullMax; dl++)
                {
                    double dll = 1.0 * dl / l;
                    x = (int)(xp1 + dll * (xp3 - xp1));
                    if (x > 640) x = 639;
                    y = (int)(yp1 + dll * (yp3 - yp1));
                    if (y < 0) y = 0;
                    z = GKinect.GetDepthPixel(x, y, MapToCamera.Color);
                    if (FingerOrNose)
                    {
                        if (z != -1 && z >= z1 && z <= z2)
                        {
                            if (xf == -1 && yf == -1 && zf == -1)
                            {
                                if (Show)
                                {
                                    Graph.setcolor(Color.Yellow);
                                    Graph.circle(Graph.width - x1 + x, Graph.height - y3 + y, 5);
                                }
                                xf = x; yf = y; zf = z;
                                _KinectToMonitor(xf, yf, zf, out xFinger, out yFinger, out zFinger);
                                FingerOrNose = false;
                                SkullMax = -3;
                                break;
                            }
                        }
                    }
                    else
                        if (z != -1 && z >= zBase - NoseSearchArea * 2 && z <= zBase + NoseSearchArea)
                        {
                            if (NoseX == -1 && NoseY == -1 && NoseZ == -1)
                            {
                                NoseX = x; NoseY = y; NoseZ = z; //Exit = true; break;
                            }
                            SkullX = x; SkullY = y; SkullZ = z;
                            SkullMax = dl - 1; break;
                        }
                }
            }
            if (NoseX == -1 && NoseY == -1 && NoseZ == -1)
                if (!KinectCalibration())
                    Graph.closegraph();
            if (Show)
            {
                Graph.setcolor(Color.Yellow);
                Graph.circle(Graph.width - x1 + SkullX, Graph.height - y3 + SkullY, 5);
            }
            if (xFinger >= 0 && xFinger < Graph.width && yFinger >= 0 && yFinger < Graph.height)
                return true;
            return false;
        }

        public static bool getfinger(out int xf, out int yf, out int zf)
        {
            int x, y, z;
            int NewNoseX = -1, NewNoseY = -1, NewNoseZ = -1;
            int NewSkullX = -1, NewSkullY = -1, NewSkullZ = -1;
            int NoseX, NoseY, NoseZ, SkullX, SkullY, SkullZ;
            bool flag = FingerTracking(out xf, out yf, out zf,
                out NoseX, out NoseY, out NoseZ,
                out SkullX, out SkullY, out SkullZ);
            if (Smooth == 0)
            {
                Graph.getmouse(out x, out y, out z);
                return z == 1;
            }
            if (xf == -1 && yf == -1 && zf == -1)
            {
                fingerReadCounter = 0;
                fingerPrediction = false;
            }
            else
            {
                fingerLocation[fingerReadCounter, 0] = xf;
                fingerLocation[fingerReadCounter, 1] = yf;
                fingerLocation[fingerReadCounter, 2] = zf;
                fingerReadCounter =
                    (fingerReadCounter < fingerLocation.GetLength(0) - 1)
                    ? fingerReadCounter + 1 : 0;
                if (fingerPrediction)
                {
                    int sx = 0, sy = 0, sz = 0;
                    for (int i = 0; i < fingerLocation.GetLength(0); i++)
                    {
                        sx += fingerLocation[i, 0];
                        sy += fingerLocation[i, 1];
                        sz += fingerLocation[i, 2];
                    }
                    xf = sx / fingerLocation.GetLength(0);
                    yf = sy / fingerLocation.GetLength(0);
                    zf = sz / fingerLocation.GetLength(0);
                }
                else
                    if (fingerReadCounter > fingerLocation.GetLength(0) - 2)
                        fingerPrediction = true;
            }
            if (NoseX == -1 && NoseY == -1 && NoseZ == -1)
            {
                noseReadCounter = 0;
                nosePrediction = false;
            }
            else
            {
                noseLocation[noseReadCounter, 0] = NoseX;
                noseLocation[noseReadCounter, 1] = NoseY;
                noseLocation[noseReadCounter, 2] = NoseZ;
                noseLocation[noseReadCounter, 3] = SkullX;
                noseLocation[noseReadCounter, 4] = SkullY;
                noseLocation[noseReadCounter, 5] = SkullZ;
                noseReadCounter =
                    (noseReadCounter < noseLocation.GetLength(0) - 1)
                    ? noseReadCounter + 1 : 0;
                if (nosePrediction)
                {
                    int nx = 0, ny = 0, nz = 0;
                    int sx = 0, sy = 0, sz = 0;
                    for (int i = 0; i < noseLocation.GetLength(0); i++)
                    {
                        nx += noseLocation[i, 0];
                        ny += noseLocation[i, 1];
                        nz += noseLocation[i, 2];
                        sx += noseLocation[i, 3];
                        sy += noseLocation[i, 4];
                        sz += noseLocation[i, 5];
                    }
                    NewNoseX = nx / noseLocation.GetLength(0);
                    NewNoseY = ny / noseLocation.GetLength(0);
                    NewNoseZ = nz / noseLocation.GetLength(0);
                    NewSkullX = sx / noseLocation.GetLength(0);
                    NewSkullY = sy / noseLocation.GetLength(0);
                    NewSkullZ = sz / noseLocation.GetLength(0);
                    if (xNose == -1 && yNose == -1 && zNose == -1)
                    {
                        xNose = NewNoseX; yNose = NewNoseY; zNose = NewNoseZ;
                        xSkull = NewSkullX; ySkull = NewSkullY; zSkull = NewSkullZ;
                        dxLeftEye = xSkull - xLeftEye;
                        dyLeftEye = ySkull - yLeftEye;
                        dzLeftEye = zSkull - zLeftEye;
                    }
                    else
                    {
                        xNose = NewNoseX; yNose = NewNoseY; zNose = NewNoseZ;
                        xSkull = NewSkullX; ySkull = NewSkullY; zSkull = NewSkullZ;
                        xLeftEye = xSkull - dxLeftEye;
                        yLeftEye = ySkull - dyLeftEye;
                        zLeftEye = zSkull - dzLeftEye;
                        if (((yLeftEye - SeenEyeY > y3 - y1) && (xLeftEye - SeenEyeX < x1 - x3)) || xLeftEye < 0 || yLeftEye < 0)
                            if (!KinectCalibration())
                                Graph.closegraph();
                            else { }
                        else
                            _ResetEyes();
                    }
                }
                else
                    if (noseReadCounter > noseLocation.GetLength(0) - 2)
                        nosePrediction = true;
            }

            Graph.getmouse(out x, out y, out z);
            return z == 1;
        }

        static public bool initfinger(int MaxX, int MaxY, bool FullScreen, int smoothFactor = 2)
        {
            int Xres = 1, Yres = 0;
            Smooth = smoothFactor;
            fingerLocation = new int[smoothFactor + 1, 3];
            noseLocation = new int[20, 6];
            Console.WriteLine("Put the Xbox Kinect in your left side...");
            Console.WriteLine("If you entered Monitor's screen coordinates already press ENTER to load them!");
            Console.Write("But if not, enter your monitor size in inch : ");
            bool loaded = false;
            try
            {
                MonitorInch = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                if (ReadMonitorFromFile())
                    loaded = true;
                else
                {
                    Console.WriteLine("Monitor coordinates have not been saved!");
                    bool get = true;
                    do
                    {
                        Console.Write("Enter your monitor size in inch : ");
                        try
                        {
                            MonitorInch = Convert.ToInt32(Console.ReadLine());
                            get = false;
                        }
                        catch
                        {
                            Console.WriteLine("You should enter a number in inch!");
                        }
                    } while (get);
                }
            }
            if (!loaded)
            {
                try
                {
                    Console.Write("Enter current X resolution of your monitor : ");
                    Xres = Convert.ToInt32(Console.ReadLine());
                    Console.Write("Enter current Y resolution of your monitor : ");
                    Yres = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Wrong number!");
                    Console.ReadKey();
                    Graph.closegraph();
                }
            }
            if (FullScreen)
                Graph.fullscreen();
            Graph.initgraph(MaxX, MaxY);
            if (!loaded)
            {
                double d = Math.Sqrt(Xres * 1.0 * Xres + Yres * 1.0 * Yres);
                PixelLength = (MonitorInch * 25.4) / d;
                if (FullScreen)
                {
                    double xPixel, yPixel;
                    if (MaxX * Yres < Xres * MaxY)
                    {
                        xPixel = MaxX * Yres / MaxY; yPixel = Yres;
                    }
                    else
                    {
                        xPixel = Xres; yPixel = Xres * MaxY / MaxX;
                    }
                    PixelLength *= Math.Sqrt(
                        (xPixel * 1.0 * xPixel + yPixel * 1.0 * yPixel) /
                        (MaxX * 1.0 * MaxX + MaxY * 1.0 * MaxY));
                }
            }
            EnableSmoothSetting(100, 0, 100, 0, 0);
            initkinect(ColorFormat.Rgb640x480Fps30, DepthFormat.R640x480Fps30);
            Graph3d.mouseCursor = new Bitmap(GraphKinect.Properties.Resources.hand1);
            Graph3d.initmouse();
            Graph3d.getmouse = new GetMouse3D(getfinger);
            return KinectCalibration();
        }
    }
}

