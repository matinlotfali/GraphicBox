using GraphDLL;

namespace GraphKinectDLL
{
    public static partial class GKinect
    {
        static int X0, X1, Y0, Y1, Z0, Z1;
        public static void initmouse()
        {
            if (IsRunning)
            {
                int x1, y1, z1;
                int Mouse = 0, xm, ym;
                Graph.cleardevice();
                Graph.setcolor(Color.White);
                while (Mouse == 0)
                {
                    Graph.putimage(0, 0, ColorCameraImage);
                    getjoint(0, MapToCamera.Color, BodyParts.HandRight, out x1, out y1, out z1, true);
                    Graph.setcolor(Color.Green);
                    Graph.fillcircle(x1, y1, 3);
                    Graph.outtextxy(100, 100, "Click when your Right hand points to Left-Top-Far corner of your space");
                    Graph.delay(0);
                    Graph.getmouse(out xm, out ym, out Mouse);
                }
                GKinect.getjoint(0, MapToCamera.Metric, BodyParts.HandRight, out X0, out Y0, out Z0, true);
                while (Mouse != 0)
                    Graph.getmouse(out xm, out ym, out Mouse);
                while (Mouse == 0)
                {
                    Graph.putimage(0, 0, ColorCameraImage);
                    GKinect.getjoint(0, MapToCamera.Color, BodyParts.HandRight, out x1, out y1, out z1, true);
                    Graph.setcolor(Color.Green);
                    Graph.fillcircle(x1, y1, 3);
                    Graph.outtextxy(100, 200, "Click when your Right hand points to Right-Down-Near corner of your space");
                    Graph.delay(0);
                    Graph.getmouse(out xm, out ym, out Mouse);
                }
                GKinect.getjoint(0, MapToCamera.Metric, BodyParts.HandRight, out X1, out Y1, out Z1, true);
                while (Mouse != 0)
                    Graph.getmouse(out xm, out ym, out Mouse);

                Graph3d.getmouse += getmouse;
            }
        }

        static int MaxZ = 300;
        public static bool getmouse(out int x, out int y, out int z)
        {
            int X, Y, Z;
            int xm, ym, zm;
            Graph.getmouse(out xm, out ym, out zm);
            GKinect.getjoint(0, MapToCamera.Metric, BodyParts.HandRight, out X, out Y, out Z,true);
            x = (X - X0) * Graph.width / (X1 - X0);
            y = (Y - Y0) * Graph.height / (Y1 - Y0);
            z = (Z - Z0) * 2 * MaxZ / (Z1 - Z0) + MaxZ;
            return (zm == 1);
        }
    }
}
