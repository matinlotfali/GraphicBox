using Microsoft.Kinect;
using GraphDLL;
using System;
using System.Collections.Generic;
using System.IO;

namespace GraphKinectDLL
{
    public static partial class GKinect
    {
        private static KinectSensor myKin;

        #region Initialize
        static ColorFormat _colorformat;
        static DepthFormat _depthformat;

        public static void initkinect(ColorFormat colorformat, DepthFormat depthformat)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                myKin = KinectSensor.KinectSensors[0];
                _colorformat = colorformat;
                _depthformat = depthformat;

                myKin.ColorFrameReady += myKin_ColorFrameReady;
                myKin.DepthFrameReady += myKin_DepthFrameReady;
                myKin.SkeletonFrameReady += myKin_SkeletonFrameReady;

                myKin.Start();
                myKin.ElevationAngle = _angle;
            }
            else
                Console.WriteLine("Kinect sensor not detected!");
        }

        public static void EnableSmoothSetting(int Correction, int Prediction, int Smoothing, int JitterRadius, int MaxDeviationRadius)
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                if ((Correction >= 0 && Correction <= 100) && (Prediction >= 0 && Prediction <= 100) && (Smoothing >= 0 && Smoothing <= 100)
                    && (JitterRadius >= 0 && JitterRadius <= 100) && (MaxDeviationRadius >= 0 && MaxDeviationRadius <= 100))
                {
                    _transfersmoothingparameter.Correction = Correction / 101F;
                    _transfersmoothingparameter.Prediction = Prediction / 101F;
                    _transfersmoothingparameter.Smoothing = Smoothing / 101F;
                    _transfersmoothingparameter.JitterRadius = JitterRadius / 101F;
                    _transfersmoothingparameter.MaxDeviationRadius = MaxDeviationRadius / 101F;
                }
                else
                {
                    Console.WriteLine("All smooth setting values are between 0 to 100!");
                }
            }
            else
                Console.WriteLine("Kinect sensor not detected!");
        }

        #endregion

        #region Camera Angle

        private static int _angle = 0;
        public static int CameraVerticalAngle
        {
            get
            {
                return _angle;
            }
            set
            {

                _angle = value;
                if (IsRunning)
                    if (value >= myKin.MinElevationAngle && value <= myKin.MaxElevationAngle)
                    {
                        while (true)
                            try
                            {
                                myKin.ElevationAngle = value;
                                break;
                            }
                            catch
                            { }
                    }
                    else
                        Console.WriteLine("Kinect's vertical angle changes between " + myKin.MinElevationAngle + " to " + myKin.MaxElevationAngle + "!");
            }
        }

        #endregion

        #region Color

        public static byte[] ColorMap { get; private set; }
        static byte[] _bytes;
        static void myKin_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (var colorframe = e.OpenColorImageFrame())
            {
                if (colorframe != null)
                {
                    colorframe.CopyPixelDataTo(_bytes);
                    ColorMap = _bytes;
                }
            }
        }

        public static Color GetColorPixel(int x, int y)
        {
            return GetColorPixel(x, y, ColorMap);
        }

        public static Color GetColorPixel(int x, int y, byte[] map)
        {
            if (myKin != null)
            {
                EnableColorStream();

                if (ColorMap != null && map != null)
                {
                    int place = (y * myKin.ColorStream.FrameWidth + x) * myKin.ColorStream.FrameBytesPerPixel;
                    if (myKin.ColorStream.Format == ColorImageFormat.RawYuvResolution640x480Fps15)
                        return Color.FromRGB(map[place + 1], map[place + 1], map[place + 1]);
                    else
                        return Color.FromRGB(map[place + 2], map[place + 1], map[place]);
                }
            }
            return Color.Black;
        }

        private static void EnableColorStream()
        {
            if (myKin.ColorStream.IsEnabled == false)
            {
                _bytes = new byte[myKin.ColorStream.FramePixelDataLength];
                myKin.ColorStream.Enable((ColorImageFormat)_colorformat);
            }
        }

        #endregion

        #region Depth

        public static DepthImagePixel[,] DepthMappedToColor { get; private set; }
        public static DepthImagePixel[,] DepthMappedToMetric { get; private set; }
        static bool colorNeeded = false, metricNeeded = false;

        static DepthImagePixel[] _shorts;
        public static DepthImagePixel[] DepthMap { get; private set; }
        private static void myKin_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (var depthframe = e.OpenDepthImageFrame())
            {
                if (depthframe != null)
                {
                    depthframe.CopyDepthImagePixelDataTo(_shorts);
                    DepthMap = _shorts;

                    if (colorNeeded)
                        DepthMappedToColor = MakeDepthOnColor(DepthMap);
                    if (metricNeeded)
                        DepthMappedToMetric = MakeDepthOnMetric(DepthMap);
                }
            }
        }

        static ColorImagePoint[] _cpoints;
        private static DepthImagePixel[,] MakeDepthOnColor(DepthImagePixel[] shorts)
        {
            if (_cpoints == null)
                _cpoints = new ColorImagePoint[shorts.Length];

            DepthImagePixel[,] matrix = new DepthImagePixel[myKin.DepthStream.FrameWidth, myKin.DepthStream.FrameHeight];
            myKin.CoordinateMapper.MapDepthFrameToColorFrame(myKin.DepthStream.Format, shorts, myKin.ColorStream.Format, _cpoints);
            for (int i = 0; i < _cpoints.Length; i++)
            {
                int x = _cpoints[i].X;
                int y = _cpoints[i].Y;
                if (x >= 0 && x < myKin.DepthStream.FrameWidth && y >= 0 && y < myKin.ColorStream.FrameHeight && matrix[x, y].IsKnownDepth == false)
                    matrix[x, y] = shorts[i];
            }
            return matrix;
        }

        static SkeletonPoint[] _spoints;
        private static DepthImagePixel[,] MakeDepthOnMetric(DepthImagePixel[] shorts)
        {
            if (_spoints == null)
                _spoints = new SkeletonPoint[shorts.Length];

            DepthImagePixel[,] matrix = new DepthImagePixel[myKin.DepthStream.FrameWidth, myKin.DepthStream.FrameHeight];
            myKin.CoordinateMapper.MapDepthFrameToSkeletonFrame(myKin.DepthStream.Format, shorts, _spoints);
            for (int i = 0; i < _cpoints.Length; i++)
            {
                int x = (int)_spoints[i].X + myKin.DepthStream.FrameWidth / 2;
                int y = (int)_spoints[i].Y + myKin.ColorStream.FrameHeight / 2;
                if (x >= 0 && x < myKin.DepthStream.FrameWidth && y >= 0 && y < myKin.ColorStream.FrameHeight && matrix[x, y].IsKnownDepth == false)
                    matrix[x, y] = shorts[i];
            }
            return matrix;
        }

        public static int GetDepthPixel(int x, int y, MapToCamera camera)
        {
            if (myKin != null && x >= 0 && y >= 0)
            {
                EnableDepthStream();

                switch (camera)
                {
                    case MapToCamera.Depth:
                        if (DepthMap != null)
                            return DepthMap[y * myKin.DepthStream.FrameWidth + x].Depth;
                        break;
                    case MapToCamera.Color:
                        if (!colorNeeded) colorNeeded = true;
                        if (DepthMappedToColor != null)
                            return DepthMappedToColor[x, y].Depth;
                        break;
                    case MapToCamera.Metric:
                        if (!metricNeeded) metricNeeded = true;
                        if (DepthMappedToMetric != null)
                            return DepthMappedToMetric[x, y].Depth;
                        break;
                }
            }
            return -1;
        }

        public static int GetPersonID(int x, int y, MapToCamera camera)
        {
            if (myKin != null && x >= 0 && y >= 0)
            {
                EnableDepthStream();
                EnableSkeletonStream();
                switch (camera)
                {
                    case MapToCamera.Depth:
                        if (DepthMap != null)
                            return (DepthMap[y * myKin.DepthStream.FrameWidth + x].PlayerIndex) - 1;
                        break;
                    case MapToCamera.Color:
                        if (!colorNeeded) colorNeeded = true;
                        if (DepthMappedToColor != null)
                            return (DepthMappedToColor[x, y].PlayerIndex) - 1;
                        break;
                }
            }
            return -1;
        }

        public static void GetPixelInMetric(int i, int j, out int x, out int y, out int z)
        {
            x = y = z = -1;
            if (myKin != null && i >= 0 && j >= 0)
            {
                EnableDepthStream();

                if (DepthMap != null)
                {
                    DepthImagePoint dpoint = new DepthImagePoint();
                    dpoint.X = i;
                    dpoint.Y = j;
                    dpoint.Depth = DepthMap[j * myKin.DepthStream.FrameWidth + i].Depth;
                    SkeletonPoint point = myKin.CoordinateMapper.MapDepthPointToSkeletonPoint((DepthImageFormat)_depthformat, dpoint);
                    x = (int)(point.X * 1000f);
                    y = (int)(point.Y * 1000f);
                    z = (int)(point.Z * 1000f);
                }
            }
        }

        private static void EnableDepthStream()
        {
            if (!myKin.DepthStream.IsEnabled)
            {
                _shorts = new DepthImagePixel[myKin.DepthStream.FramePixelDataLength];
                myKin.DepthStream.Enable((DepthImageFormat)_depthformat);
            }
        }

        #endregion

        #region Skeleton

        private static TransformSmoothParameters _transfersmoothingparameter;
        private static Skeleton[] _skeletons = null;

        static void myKin_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (var skeletonframe = e.OpenSkeletonFrame())
            {
                if (skeletonframe != null)
                {
                    skeletonframe.CopySkeletonDataTo(_skeletons);
                }
            }
        }

        public static void getjoint(int SkeletonID, MapToCamera camera, BodyParts joint, out int x, out int y, out int z)
        {
            getjoint(SkeletonID, camera, joint, out x, out y, out z, true);
        }

        public static void getjoint(int SkeletonID, MapToCamera camera, BodyParts joint, out int x, out int y, out int z, bool sortID)
        {
            x = y = z = -1;
            if (myKin != null)
            {
                EnableSkeletonStream();

                if (_skeletons != null)
                {
                    if (sortID)
                        SkeletonID = _SelectTrackingSkeleton(SkeletonID);

                    if (SkeletonID != -1)
                        if (_skeletons[SkeletonID] != null)
                        {
                            SkeletonPoint point = _skeletons[SkeletonID].Joints[(JointType)joint].Position;
                            if (point.X != 0 && point.Y != 0 && point.Z != 0)
                            {
                                switch (camera)
                                {
                                    case MapToCamera.Metric:
                                        x = Convert.ToInt32(point.X * 1000f);
                                        y = Convert.ToInt32(point.Y * 1000f);
                                        z = Convert.ToInt32(point.Z * 1000f);
                                        break;
                                    case MapToCamera.Depth:
                                        DepthImagePoint dPoint0 = myKin.CoordinateMapper.MapSkeletonPointToDepthPoint(point, myKin.DepthStream.Format);
                                        x = dPoint0.X;
                                        y = dPoint0.Y;
                                        z = dPoint0.Depth;
                                        break;
                                    case MapToCamera.Color:
                                        ColorImagePoint cPoint = myKin.CoordinateMapper.MapSkeletonPointToColorPoint(point, myKin.ColorStream.Format);
                                        DepthImagePoint dPoint1 = myKin.CoordinateMapper.MapSkeletonPointToDepthPoint(point, myKin.DepthStream.Format);
                                        x = cPoint.X;
                                        y = cPoint.Y;
                                        z = dPoint1.Depth;
                                        break;
                                }
                            }
                        }
                }
            }
        }

        private static void EnableSkeletonStream()
        {
            if (!myKin.SkeletonStream.IsEnabled)
            {
                _skeletons = new Skeleton[6];
                myKin.SkeletonStream.Enable(_transfersmoothingparameter);
            }
        }


        private static int _SelectTrackingSkeleton(int Counter)
        {
            if (_skeletons[0] != null)
                for (int i = 0; i < 6; i++)
                    if (_skeletons[i].TrackingState != SkeletonTrackingState.NotTracked)
                    {
                        if (Counter == 0) return i;
                        Counter--;
                    }
            return -1;
        }

        #endregion

        #region Assistant Methods

        public static int FirstSkeleton
        {
            get
            {
                return _SelectTrackingSkeleton(0);
            }
        }

        public static bool IsRunning
        {
            get
            {
                return (myKin != null && myKin.IsRunning);
            }
        }

        public static Bitmap CameraStereoImage(int scaleDepth)
        {
            Bitmap result = null;
            if (myKin != null)
            {
                Console.WriteLine("Camera Stereo Image:");
                EnableColorStream();
                EnableDepthStream();
                if (!colorNeeded) colorNeeded = true;
                while (ColorMap == null) ;
                while (DepthMappedToColor == null) ;

                byte[] colorMap = (byte[])ColorMap.Clone();
                DepthImagePixel[,] depthMap = (DepthImagePixel[,])DepthMappedToColor.Clone();
                int width = myKin.ColorStream.FrameWidth;
                int height = myKin.ColorStream.FrameHeight;

                Console.WriteLine("Measuring maximum and minimum depth for drawing...");
                short maxDepth = 0;
                short minDepth = 4096;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if (depthMap[i, j].Depth > maxDepth)
                            maxDepth = depthMap[i, j].Depth;
                        if (depthMap[i, j].Depth != 0 && depthMap[i, j].Depth < minDepth)
                            minDepth = depthMap[i, j].Depth;
                    }

                Graph.cleardevice();
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                    {
                        if (depthMap[i, j].Depth != 0)
                        {
                            byte c = (byte)(255 - (depthMap[i, j].Depth - minDepth) * 255 / (maxDepth - minDepth));
                            Graph.putpixel(i, j, Color.FromRGB(c, c, c));
                        }
                    }
                Graph.delay(0);

                Console.WriteLine("Please click on the most buttom of object wanted:");
                int buttom, click, x, y;
                do
                {
                    Graph.getmouse(out x, out buttom, out click);
                    Graph.cleardevice();
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < buttom; j++)
                        {
                            if (depthMap[i, j].Depth != 0)
                            {
                                Graph.putpixel(i, j, GetColorPixel(i, j, colorMap));
                            }
                        }
                    Graph.delay(0);
                } while (click != 1);

                while (click == 1)
                    Graph.getmouse(out x, out buttom, out click);

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < buttom; j++)
                    {
                        if (depthMap[i, j].Depth != 0)
                        {
                            byte c = (byte)(255 - (depthMap[i, j].Depth - minDepth) * 255 / (maxDepth - minDepth));
                            Graph.putpixel(i, j, Color.FromRGB(c, c, c));
                        }
                    }
                Graph.delay(0);

                Console.WriteLine("Please select the objects and then press Enter:");

                Bitmap depthPic = Graph.getimage(0, 0, width, height);
                int[,] selected = new int[width, buttom];
                int count = 0;
                while (true)
                {
                    do
                    {
                        Graph.putimage(depthPic, 0, 0);
                        Graph.getmouse(out x, out y, out click);

                        if (y < buttom)
                        {
                            if (depthMap[x, y].Depth != 0 && selected[x, y] == 0)
                            {
                                count++;
                                XYQueue queue = new XYQueue(width * buttom);
                                int def = 20;
                                queue.insert(x, y);
                                do
                                {
                                    int x0, y0;
                                    queue.delete(out x0, out y0);

                                    if (selected[x0, y0] != count)
                                    {
                                        selected[x0, y0] = count;

                                        if (x0 > 0 && selected[x0 - 1, y0] == 0 && depthMap[x0 - 1, y0].Depth != 0 && Math.Abs(depthMap[x0 - 1, y0].Depth - depthMap[x0, y0].Depth) <= def)
                                            queue.insert(x0 - 1, y0);
                                        if (x0 < width - 1 && selected[x0 + 1, y0] == 0 && depthMap[x0 + 1, y0].Depth != 0 && Math.Abs(depthMap[x0 + 1, y0].Depth - depthMap[x0, y0].Depth) <= def)
                                            queue.insert(x0 + 1, y0);
                                        if (y0 > 0 && selected[x0, y0 - 1] == 0 && depthMap[x0, y0 - 1].Depth != 0 && Math.Abs(depthMap[x0, y0 - 1].Depth - depthMap[x0, y0].Depth) <= def)
                                            queue.insert(x0, y0 - 1);
                                        if (y0 < buttom - 1 && selected[x0, y0 + 1] == 0 && depthMap[x0, y0 + 1].Depth != 0 && Math.Abs(depthMap[x0, y0 + 1].Depth - depthMap[x0, y0].Depth) <= def)
                                            queue.insert(x0, y0 + 1);
                                    }
                                } while (queue.Count != 0);
                            }

                            for (int i = 0; i < width; i++)
                                for (int j = 0; j < buttom; j++)
                                    if (selected[i, j] != 0 && selected[i, j] == selected[x, y])
                                        Graph.putpixel(i, j, GetColorPixel(i, j, colorMap));
                        }

                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < buttom; j++)
                                if (selected[i, j] < 0)
                                    Graph.putpixel(i, j, GetColorPixel(i, j, colorMap));

                        Graph.delay(0);
                    } while (click != 1 && !Graph.keydown(System.Windows.Forms.Keys.Enter));
                    if (Graph.keydown(System.Windows.Forms.Keys.Enter))
                        break;

                    while (click == 1)
                        Graph.getmouse(out x, out y, out click);

                    int change = selected[x, y];
                    if (change > 0)
                    {
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < buttom; j++)
                                if (selected[i, j] == change)
                                    selected[i, j] *= -selected[i, j];
                        Console.WriteLine("Object Selected.");
                    }
                }


                Console.WriteLine("Converting to stereo image...");
                int lbx = width, lby = buttom, ubx = 0, uby = 0;
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < buttom; j++)
                        if (selected[i, j] < 0)
                        {
                            if (lbx > i)
                                lbx = i;

                            if (lby > j)
                                lby = j;

                            if (ubx < i)
                                ubx = i;

                            if (uby < j)
                                uby = j;
                        }

                int mx = (ubx + lbx) / 2;
                int my = (uby + lby) / 2;

                int movex = width / 2 - mx;
                int movey = height / 2 - my;

                Bitmap pic = new Bitmap(width * 2, height);

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < buttom; j++)
                        if (selected[i, j] < 0)
                        {
                            int z = (depthMap[i, j].Depth - minDepth) * scaleDepth / (4096 - minDepth);
                            {
                                double x1, x2, y0;
                                Graph3dDraw._to2Da(i + movex, j + movey, z, out x1, out x2, out y0);
                                Color c = GetColorPixel(i, j, colorMap);
                                pic.SetPixel((int)x1, (int)y0, c);
                                pic.SetPixel((int)x2 + width, (int)y0, c);
                            }
                        }

                Console.WriteLine("Croping to smaller stereo image...");
                int lb1 = -1, ub1 = -1, lb2 = -1, ub2 = -1;
                for (int i = 0; i < width; i++)
                {
                    int j;
                    for (j = 0; j < buttom; j++)
                        if (pic.GetPixel(i, j).a > 0)
                        {
                            lb1 = i;
                            break;
                        }
                    if (j < buttom)
                        break;
                }

                for (int i = width - 1; i > 0; i--)
                {
                    int j;
                    for (j = 0; j < buttom; j++)
                        if (pic.GetPixel(i, j).a > 0)
                        {
                            ub1 = i;
                            break;
                        }
                    if (j < buttom)
                        break;
                }

                for (int i = 0; i < width; i++)
                {
                    int j;
                    for (j = 0; j < buttom; j++)
                        if (pic.GetPixel(i + width, j).a > 0)
                        {
                            lb2 = i;
                            break;
                        }
                    if (j < buttom)
                        break;
                }

                for (int i = width - 1; i > 0; i--)
                {
                    int j;
                    for (j = 0; j < buttom; j++)
                        if (pic.GetPixel(i + width, j).a > 0)
                        {
                            ub2 = i;
                            break;
                        }
                    if (j < buttom)
                        break;
                }

                for (int j = 0; j < height; j++)
                {
                    int i;
                    for (i = 0; i < width * 2; i++)
                        if (pic.GetPixel(i, j).a > 0)
                        {
                            lby = j;
                            break;
                        }
                    if (i < width)
                        break;
                }

                for (int j = height - 1; j >= 0; j--)
                {
                    int i;
                    for (i = 0; i < width * 2; i++)
                        if (pic.GetPixel(i, j).a > 0)
                        {
                            uby = j;
                            break;
                        }
                    if (i < width)
                        break;
                }

                int finLB = (lb1 < lb2) ? lb1 : lb2;
                int finUB = (ub1 > ub2) ? ub1 : ub2;
                result = new Bitmap((finUB - finLB + 1) * 2, uby - lby + 1);

                int dx = finLB - lb1;
                for (int i = lb1; i < ub1; i++)
                    for (int j = lby; j < uby; j++)
                    {
                        Color c = pic.GetPixel(i, j);
                        result.SetPixel(dx + i - lb1, j - lby, c);
                    }

                dx = finUB + 1 - lb2;
                for (int i = lb2; i < ub2; i++)
                    for (int j = lby; j < uby; j++)
                    {
                        Color c = pic.GetPixel(i + width, j);
                        result.SetPixel(dx + i - lb2, j - lby, c);
                    }

                Console.WriteLine("Done");
            }
            return result;
        }

        public static byte[] CameraStereoPerson(int scaleDepth)
        {
            if (myKin != null)
            {
                EnableColorStream();
                EnableDepthStream();
                if (!colorNeeded) colorNeeded = true;
                while (ColorMap == null) ;
                while (DepthMappedToColor == null) ;

                short minZ = FindMinDepth();
                short[,] dmatrix = Capture(minZ);
                return ShortMatrixToByte(scaleDepth, dmatrix);
            }
            return null;
        }

        private static byte[] ShortMatrixToByte(int scaleDepth, short[,] dmatrix)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            int countAlphas = 0;
            for (int j = 0; j < myKin.ColorStream.FrameHeight; j++)
                for (int i = 0; i < myKin.ColorStream.FrameWidth; i++)
                {
                    if (dmatrix[i, j] > 0)
                    {
                        if (countAlphas > 0)
                        {
                            bw.Write(true);
                            bw.Write(countAlphas);
                            countAlphas = 0;
                        }
                        Color c = GetColorPixel(i, j, ColorMap);
                        bw.Write(false);
                        bw.Write(dmatrix[i, j]);
                        bw.Write(c.r);
                        bw.Write(c.g);
                        bw.Write(c.b);
                    }
                    else
                        countAlphas++;
                }

            if (countAlphas > 0)
            {
                bw.Write(true);
                bw.Write(countAlphas);
            }
            bw.Close();
            return stream.ToArray();
        }

        private static short[,] Capture(short minZ)
        {
            int lastX = 0;
            int lastY = 0;
            short lastZ = 0;
            short[,] dmatrix = new short[myKin.ColorStream.FrameWidth, myKin.ColorStream.FrameHeight];
            for (int i = 0; i < myKin.ColorStream.FrameWidth; i++)
                for (int j = 0; j < myKin.ColorStream.FrameHeight; j++)
                {
                    DepthImagePixel d = DepthMappedToColor[i, j];
                    if (d.IsKnownDepth && (d.Depth - minZ) < 200)
                    {
                        if (lastX == i && j - lastY < 10)
                            for (int k = lastY + 1; k < j; k++)
                                dmatrix[i, k] = lastZ;
                        dmatrix[i, j] = d.Depth;
                        lastX = i;
                        lastY = j;
                        lastZ = d.Depth;
                    }
                }
            return dmatrix;
        }

        private static short FindMinDepth()
        {
            short minZ = short.MaxValue;
            for (int i = 0; i < myKin.ColorStream.FrameWidth; i++)
                for (int j = 0; j < myKin.ColorStream.FrameHeight / 2; j++)
                {
                    DepthImagePixel d = DepthMappedToColor[i, j];
                    if (d.IsKnownDepth && d.Depth < minZ)
                        minZ = d.Depth;
                }
            return minZ;
        }

        public static Bitmap CameraColorImage
        {
            get
            {
                Bitmap pic = null;
                if (myKin != null)
                {
                    EnableColorStream();
                    while (ColorMap == null) ;
                    pic = new Bitmap(myKin.ColorStream.FrameWidth, myKin.ColorStream.FrameHeight);
                    pic.Pixels = (byte[])ColorMap.Clone();
                    for (int i = 3; i < pic.Pixels.Length; i += 4)
                        pic.Pixels[i] = 255;
                }
                return pic;
            }
        }

        #endregion
    }
}

