using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GraphDLL
{
    public sealed class GBitmap
    {
        private BitmapData bitmapData = null;
        private int cCount;
        private IntPtr Iptr = IntPtr.Zero;
        private System.Drawing.Rectangle rect;
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
                if (isLocked)
                    UnlockBits();
                return bitmap;
            }

            set
            {
                if (isLocked)
                    UnlockBits();
                bitmap = value;
            }
        }

        /*public byte[] Pixels
        {
            get
            {
                if (!isLocked)
                    LockBits();
                return pixels;
            }

            set
            {
                if (!isLocked)
                    LockBits();
                pixels = value;
            }
        }*/

        public GBitmap(string file)
        {
            bitmap = new System.Drawing.Bitmap(file);
            Initialize();
        }

        public GBitmap(System.Drawing.Bitmap original)
        {
            bitmap = original;
            Initialize();
        }

        public GBitmap(int Width, int Height)
        {
            bitmap = new System.Drawing.Bitmap(Width, Height);
            Initialize();
        }

        private void Initialize()
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Depth = System.Drawing.Image.GetPixelFormatSize(bitmap.PixelFormat);
            if (((Depth != 8) && (Depth != 0x18)) && (Depth != 0x20))
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }
            int num = Width * Height;
            rect = new System.Drawing.Rectangle(0, 0, Width, Height);
            int num2 = Depth / 8;
            pixels = new byte[num * num2];
            this.cCount = Depth / 8;
            isLocked = false;
        }

        public Color GetPixel(int x, int y)
        {
            int index = ((y * Width) + x) * cCount;
            if (index >= pixels.Length || index < 0)
                return Color.Black;
            if (!isLocked)
                LockBits();
            if (cCount == 4)
                return Color.FromARGB(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]);
            return Color.FromRGB(pixels[index + 2], pixels[index + 1], pixels[index]);
        }

        private void LockBits()
        {
            bitmapData = bitmap.LockBits(this.rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            Iptr = bitmapData.Scan0;
            Marshal.Copy(Iptr, pixels, 0, pixels.Length);
            isLocked = true;
        }

        public void SetPixel(int x, int y, Color color)
        {
            int index = ((y * Width) + x) * cCount;
            if (index >= pixels.Length || index < 0) return;

            if (!isLocked)
                LockBits();
            pixels[index] = color.b;
            pixels[index + 1] = color.g;
            pixels[index + 2] = color.r;
            pixels[index + 3] = color.a;
        }
        public void SetPixel(int x, int y)
        {
            int index = ((y * Width) + x) * cCount;
            if (index >= pixels.Length || index < 0) return;

            if (!isLocked)
                LockBits();
            pixels[index] = GFunc.color.b;
            pixels[index + 1] = GFunc.color.g;
            pixels[index + 2] = GFunc.color.r;
            pixels[index + 3] = GFunc.color.a;
        }

        private void UnlockBits()
        {
            Marshal.Copy(pixels, 0, Iptr, pixels.Length);
            bitmap.UnlockBits(bitmapData);
            isLocked = false;
        }

        public void Save(string file)
        {
            SysDraw.Save(file);
        }
    }
}


