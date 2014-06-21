using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace GraphDLL
{
    public sealed class Bitmap
    {
        private BitmapData bitmapData = null;
        private IntPtr Iptr = IntPtr.Zero;
        private Rectangle rect;
        private System.Drawing.Bitmap bitmap;
        private byte[] pixels;

        public int Depth { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public bool isLocked { get; private set; }

        public System.Drawing.Bitmap SysDraw
        {
            get
            {
                UnlockBits();
                return bitmap;
            }

            set
            {
                UnlockBits();
                bitmap = value;
            }
        }

        public byte[] Pixels
        {
            get
            {
                LockBits();
                return pixels;
            }

            set
            {
                LockBits();
                pixels = value;
            }
        }

        public Bitmap(string file)
        {
            bitmap = new System.Drawing.Bitmap(file);
            Initialize();
        }

        public Bitmap(Bitmap original, int width, int height)
        {
            bitmap = new System.Drawing.Bitmap(original.SysDraw, new Size(width, height));
            Initialize();
        }

        public Bitmap(System.Drawing.Bitmap original)
        {
            bitmap = (System.Drawing.Bitmap)original.Clone();
            Initialize();
        }

        public Bitmap(int Width, int Height)
        {
            bitmap = new System.Drawing.Bitmap(Width, Height);
            Initialize();
        }

        public Bitmap(int Width, int Height, PixelFormat format)
        {
            bitmap = new System.Drawing.Bitmap(Width, Height, format);
            Initialize();
        }

        private void Initialize()
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            if (System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat) != 32)
            {
                System.Drawing.Bitmap temp = new System.Drawing.Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                Graphics.FromImage(temp).DrawImage(SysDraw, 0, 0, Width, Height);
                bitmap = temp;
            }
            pixels = new byte[Width * Height * 4];
            rect = new System.Drawing.Rectangle(0, 0, Width, Height);
            isLocked = false;
        }

        private void LockBits()
        {
            if (!isLocked)
            {
                bitmapData = bitmap.LockBits(this.rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
                Iptr = bitmapData.Scan0;
                Marshal.Copy(Iptr, pixels, 0, pixels.Length);
                isLocked = true;
            }
        }


        public void SetPixel(int x, int y) { SetPixel(x, y, Graph.color); }
        public void SetPixel(int x, int y, Color color)
        {
            LockBits();
            SetPixel(x, y, color, pixels, Width, Height);
        }

        public static void SetPixel(int x, int y, Color color, byte[] pixels, int Width, int Height)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int index = ((y * Width) + x) * 4;
                SetPixel(index, color, pixels);
            }
        }
        private static void SetPixel(int index, Color color, byte[] pixels)
        {
            pixels[index] = color.b;
            pixels[index + 1] = color.g;
            pixels[index + 2] = color.r;
            pixels[index + 3] = color.a;
        }

        public Color GetPixel(int x, int y)
        {
            LockBits();
            return GetPixel(x, y, pixels, Width, Height);
        }

        public static Color GetPixel(int x, int y, byte[] pixels, int Width, int Height)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                int index = ((y * Width) + x) * 4;
                return GetPixel(index, pixels);
            }
            return Color.Black;
        }
        public static Color GetPixel(int index, byte[] pixels)
        {
            return Color.FromARGB(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]);
        }

        private void UnlockBits()
        {
            if (isLocked)
            {
                Marshal.Copy(pixels, 0, Iptr, pixels.Length);
                bitmap.UnlockBits(bitmapData);
                isLocked = false;
            }
        }

        public void Save(string file)
        {
            SysDraw.Save(file);
        }

        public void Transparent(Color c)
        {
            for (int i = 0; i < pixels.Length; i++)
                if (GetPixel(i, pixels) == c)
                    SetPixel(i, Color.FromARGB(0, 0, 0, 0), pixels);
        }

        public void Transparent(Color c, byte telorance)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                Color back = GetPixel(i, pixels);
                if (Math.Pow(back.a - c.a, 2) + Math.Pow(back.r - c.r, 2) + Math.Pow(back.g - c.g, 2) + Math.Pow(back.b - c.b, 2) <= telorance * telorance)
                    SetPixel(i, Color.FromARGB(0, 0, 0, 0), pixels);
            }
        }

        public Bitmap Opacity(double opacity)
        {
            Bitmap pic = new Bitmap(bitmap);
            for (int i = 3; i < Pixels.Length; i += 4)
                if (Pixels[i] != 0)
                    pic.Pixels[i] = (byte)(opacity * pic.Pixels[i] / 100);
            return pic;
        }

        public Bitmap Convert3DToAnaglyph(bool LeftEyeIsLeft)
        {
            Bitmap result = new Bitmap(Width / 2, Height);

            if (LeftEyeIsLeft)
            {
                for (int i = 0; i < Width / 2; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        Color c = GetPixel(i, j);
                        c = Graph3dDraw.AnaglyphLeft(c, j, result.pixels, i);
                        result.SetPixel(i, j, c);

                        c = GetPixel(i + Width / 2, j);
                        c = Graph3dDraw.AnaglyphRight(c, j, result.pixels, i);
                        result.SetPixel(i, j, c);
                    }
            }
            else
            {
                for (int i = 0; i < Width / 2; i++)
                    for (int j = 0; j < Height; j++)
                    {
                        Color c = GetPixel(i, j);
                        c = Graph3dDraw.AnaglyphRight(c, j, result.pixels, i);
                        result.SetPixel(i, j, c);

                        c = GetPixel(i + Width / 2, j);
                        c = Graph3dDraw.AnaglyphLeft(c, j, result.pixels, i);
                        result.SetPixel(i, j, c);
                    }
            }

            return result;
        }
    }
}


