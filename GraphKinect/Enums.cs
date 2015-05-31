using Microsoft.Kinect;

namespace GraphKinectDLL
{
    public enum ColorFormat
    {
        Rgb1280x960Fps12 = ColorImageFormat.RgbResolution1280x960Fps12,
        Rgb640x480Fps30 = ColorImageFormat.RgbResolution640x480Fps30,
        RawYuv640x480Fps15 = ColorImageFormat.RawYuvResolution640x480Fps15,
        Yuv640x480Fps15 = ColorImageFormat.YuvResolution640x480Fps15
    }

    public enum DepthFormat
    {
        R640x480Fps30 = DepthImageFormat.Resolution640x480Fps30,
        R320x240Fps30 = DepthImageFormat.Resolution320x240Fps30,
        R80x60Fps30 = DepthImageFormat.Resolution80x60Fps30,
    }

    public enum MapToCamera
    {
        Color,
        Depth,
        Metric
    }

    public enum BodyParts
    {
        HipCenter = JointType.HipCenter,
        Spine,
        ShoulderCenter,
        Head,
        ShoulderLeft,
        ElbowLeft,
        WristLeft,
        HandLeft,
        ShoulderRight,
        ElbowRight,
        WristRight,
        HandRight,
        HipLeft,
        KneeLeft,
        AnkleLeft,
        FootLeft,
        HipRight,
        KneeRight,
        AnkleRight,
        FootRight
    }
}
