using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphDLL
{
    public static partial class Graph
    {
        internal static bool mouseInsideForm = false;
        public static bool getmouse(out int x, out int y, out int click)
        {
            x = GForm.MouseX;
            y = GForm.MouseY;
            click = GForm.LeftClick ? 1 : GForm.RightClick ? 2 : GForm.MiddleClick ? 3 : 0;
            return mouseInsideForm;
        }

        public static void showMouse()
        {
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Cursor.Show();
                Graph.showmouse = true;
            });
        }

        public static void hidemouse()
        {
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Cursor.Hide();
                Graph.showmouse = false;
            });
        }
    }

    public delegate bool GetMouse3D(out int x, out int y, out int z);
    public static partial class Graph3d
    {

        internal static int MouseX, MouseY, MouseZ;
        static int centerX, centerY;
        static int mouseSize = 10;
        static bool initCalled = false;

        public static Bitmap mouseCursor;
        public static GetMouse3D getmouse = new GetMouse3D(_getmouse);

        static bool _getmouse(out int x, out int y, out int z)
        {
            if (!initCalled)
            {
                x = y = z = -1;
                return false;
            }

            int tempX = MouseX, tempY = MouseY, tempZ = MouseZ;

            if (GForm.RightClick)
                tempZ -= Cursor.Position.Y - centerY;
            else
                tempY += Cursor.Position.Y - centerY;
            tempX += Cursor.Position.X - centerX;

            double x1, x2, ym;
            Graph3dDraw._to2Da(tempX, tempY, tempZ, out x1, out x2, out ym);
            if (x1 >= 0 && x2 < Graph.width && ym >= 0 && ym < Graph.height)
            {
                MouseX = tempX;
                MouseY = tempY;
                MouseZ = tempZ;

                while (Cursor.Position.X != Graph.form.Left + Graph.form.Width / 2 || Cursor.Position.Y != Graph.form.Top + Graph.form.Height / 2)
                    User32.SetCursorPos(Graph.form.Left + Graph.form.Width / 2, Graph.form.Top + Graph.form.Height / 2);

                centerX = Cursor.Position.X;
                centerY = Cursor.Position.Y;
            }
            z = MouseZ;
            x = MouseX;
            y = MouseY;
            return GForm.LeftClick;
        }

        public static void initmouse()
        {
            Graph.hidemouse();

            while (Cursor.Position.X != Graph.form.Left + Graph.form.Width / 2 || Cursor.Position.Y != Graph.form.Top + Graph.form.Height / 2)
                User32.SetCursorPos(Graph.form.Left + Graph.form.Width / 2, Graph.form.Top + Graph.form.Height / 2);
            centerX = Cursor.Position.X;
            centerY = Cursor.Position.Y;

            MouseX = Graph.width / 2;
            MouseY = Graph.height / 2;
            MouseZ = 0;
            if (mouseCursor == null) mouseCursor = drawMousePic();
            showMouse();
            initCalled = true;
        }

        public static void showMouse()
        {
            Graph3d.mouse3da = true;
        }

        public static void hidemouse()
        {
            Graph3d.mouse3da = false;
        }

        internal static Bitmap drawMousePic()
        {
            Bitmap pic = new Bitmap(mouseSize, mouseSize * 2);
            pic.Transparent(Color.Black);

            for (int i = 0; i < mouseSize; i++)
                for (int j = 0; j <= i; j++)
                    pic.SetPixel(j, i);
            for (int i = 0; i < (mouseSize) / 3; i++)
                for (int j = 0; j <= (mouseSize) / 3 - i; j++)
                    pic.SetPixel(j, i + mouseSize);
            for (int i = 0; i < (mouseSize) * 0.75; i++)
            {
                pic.SetPixel(i, 2 * i);
                pic.SetPixel(i, 2 * i + 1);
                pic.SetPixel(i, 2 * i + 2);
            }
            return pic;
        }

        internal static void DrawMouse()
        {
            Graph3dImage.image(MouseX, MouseY, MouseZ, mouseCursor.Width, 90, 0, mouseCursor.Height, 90, -90, mouseCursor);
        }
    }
}
