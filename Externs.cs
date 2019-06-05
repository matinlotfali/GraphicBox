using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

[StructLayout(LayoutKind.Sequential)]
internal struct DEVMODE1
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;

    public short dmOrientation;
    public short dmPaperSize;
    public short dmPaperLength;
    public short dmPaperWidth;

    public short dmScale;
    public short dmCopies;
    public short dmDefaultSource;
    public short dmPrintQuality;
    public short dmColor;
    public short dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public short dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmFormName;
    public short dmLogPixels;
    public short dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;

    public int dmDisplayFlags;
    public int dmDisplayFrequency;

    public int dmICMMethod;
    public int dmICMIntent;
    public int dmMediaType;
    public int dmDitherType;
    public int dmReserved1;
    public int dmReserved2;

    public int dmPanningWidth;
    public int dmPanningHeight;
};

internal class User32
{
    [DllImport("user32.dll")]
    static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
    [DllImport("user32.dll")]
    static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SetCursorPos(int x, int y);

    const int ENUM_CURRENT_SETTINGS = -1;
    const int CDS_UPDATEREGISTRY = 0x01;
    const int CDS_TEST = 0x02;
    const int DISP_CHANGE_SUCCESSFUL = 0;
    const int DISP_CHANGE_RESTART = 1;
    const int DISP_CHANGE_FAILED = -1;

    public static bool CResolution(int iWidth, int iHeight, out DEVMODE1 devmode)
    {
        DEVMODE1 dm = new DEVMODE1();
        dm.dmDeviceName = new String(new char[32]);
        dm.dmFormName = new String(new char[32]);
        dm.dmSize = (short)Marshal.SizeOf(dm);
        devmode = dm;
        if (0 != EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
        {
            devmode = dm;
            dm.dmPelsWidth = iWidth;
            dm.dmPelsHeight = iHeight;
            return CResolution(ref dm);
        }
        return false;
    }

    public static bool CResolution(ref DEVMODE1 dm)
    {
        int iRet = ChangeDisplaySettings(ref dm, CDS_TEST);

        if (iRet == DISP_CHANGE_FAILED)
        {
            MessageBox.Show("Unable to process your request");
            MessageBox.Show("Description: Unable To Process Your Request. Sorry For This Inconvenience.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }
        else
        {
            iRet = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);

            switch (iRet)
            {
                case DISP_CHANGE_SUCCESSFUL:
                    {
                        return true;
                        //successfull change
                    }
                case DISP_CHANGE_RESTART:
                    {

                        MessageBox.Show("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //windows 9x series you have to restart
                        return false;
                    }
                default:
                    {

                        MessageBox.Show("Description: Failed To Change The Resolution.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                        //failed to change
                    }
            }
        }
    }
}
