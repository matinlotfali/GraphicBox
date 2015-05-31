using System;
using Microsoft.Kinect;

namespace GraphKinectDLL
{
    public static partial class GKinect
    {
        static double ClickSensabliy = 0.36;
        static bool IsLeftFinger = false, IsRightFinger = false;

        static SkeletonPoint GetLeftFinger(int SkeletonID, out bool click)
        {
            SkeletonPoint Hand = _skeletons[SkeletonID].Joints[JointType.HandLeft].Position;
            SkeletonPoint Wrist = _skeletons[SkeletonID].Joints[JointType.WristLeft].Position;
            SkeletonPoint Elbow = _skeletons[SkeletonID].Joints[JointType.ElbowLeft].Position;
            SkeletonPoint Finger = GetFinger(Hand, Wrist, Elbow, SkeletonID);
            click = (Distance(Finger, Elbow) <= ClickSensabliy);
            return Finger;
        }

        static SkeletonPoint GetRightFinger(int SkeletonID, out bool click)
        {
            SkeletonPoint Hand = _skeletons[SkeletonID].Joints[JointType.HandRight].Position;
            SkeletonPoint Wrist = _skeletons[SkeletonID].Joints[JointType.WristRight].Position;
            SkeletonPoint Elbow = _skeletons[SkeletonID].Joints[JointType.ElbowRight].Position;
            SkeletonPoint Finger = GetFinger(Hand, Wrist, Elbow, SkeletonID);
            click = (Distance(Finger, Elbow) <= ClickSensabliy);
            return Finger;
        }

        private static SkeletonPoint GetFinger(SkeletonPoint Hand, SkeletonPoint Wrist, SkeletonPoint Elbow, int SkeletonID)
        {
            DepthImagePoint HandD = myKin.MapSkeletonPointToDepth(Hand, myKin.DepthStream.Format);
            DepthImagePoint WristD = myKin.MapSkeletonPointToDepth(Wrist, myKin.DepthStream.Format);
            //DepthImagePoint ElbowD = myKin.MapSkeletonPointToDepth(ElbowS, myKin.DepthStream.Format);

            //double HandToWrist = Distance(HandS, WristS);
            double WristToElbow = Distance(Wrist, Elbow);
            double maxDistance = WristToElbow;

            SkeletonPoint max = Hand;

            XYQueue queue = new XYQueue();
            queue.insert(WristD.X, WristD.Y);
            queue.insert(HandD.X, HandD.Y);
            do
            {
                int x, y;
                queue.delete(out x, out y);
                if (x >= 0 && x < myKin.DepthStream.FrameWidth && y >= 0 && y < myKin.DepthStream.FrameHeight)     //safheye asli
                {
                    if (GetPersonID(x, y, MapToCamera.Depth) == SkeletonID)                       //rooye adam
                    {
                        SkeletonPoint point = myKin.MapDepthToSkeletonPoint(myKin.DepthStream.Format, x, y, DepthMap[y * myKin.DepthStream.FrameWidth + x]);
                        if (Distance(Hand, point) < 0.1)          //tooye mohite dast kafe dast
                        {
                            double b = Distance(Elbow, point);
                            if (b >= WristToElbow)                //samte angosht
                            {
                                if (b > maxDistance)                //doortarin angosht
                                {
                                    maxDistance = b;
                                    max = point;
                                }
                                queue.insert(x + 1, y);
                                queue.insert(x - 1, y);
                                queue.insert(x, y + 1);
                                queue.insert(x, y - 1);
                            }
                        }
                    }
                }
            } while (queue.Count != 0);
            return max;
        }


        static double Distance(SkeletonPoint a, SkeletonPoint b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
        }

        static double Distance(SkeletonPoint a, int x, int y, short z)
        {
            SkeletonPoint s = myKin.MapDepthToSkeletonPoint(myKin.DepthStream.Format, x, y, z);
            return Distance(a, s);
        }

    }
}
