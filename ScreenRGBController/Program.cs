using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenRGBController
{
    class Program
    {
        private static Thread thread;

        static void Main(string[] args)
        {

            thread = new Thread(ArduinoStuff);
            thread.Start();

        }

        static Graphics bufferGraphics;
        static Bitmap buffer;

        static Color GetAverageScreenColor()
        {

            bufferGraphics.CopyFromScreen(0, 0, 0, 0, new Size(buffer.Width, buffer.Height));

            BitmapData srcData = buffer.LockBits(new Rectangle(0, 0, buffer.Width, buffer.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;
            IntPtr scan0 = srcData.Scan0;

            long red = 0;
            long green = 0;
            long blue = 0;
            long samples = 0;

            int width = buffer.Width;
            int height = buffer.Height;

            unsafe
            {
                byte* p = (byte*)(void*)scan0;

                for (int y = 0; y < height; y++)
                {
                    for(int x = 0; x < width; x++)
                    {
                        int idx = y * stride + x * 4;

                        blue += p[idx];
                        green += p[idx + 1];
                        red += p[idx + 2];

                        samples++;
                    }
                }
            }

            buffer.UnlockBits(srcData);

            return Color.FromArgb((byte)(red / samples), (byte)(green / samples), (byte)(blue / samples));
        }

        static Color GetHueRGB(Color other)
        {
            float hue = other.GetHue();

            int type = (int)(hue / 60);
            float deg = (float)(hue % 60) / 60f;
            float invDeg = 1f - deg;

            int from = (int)(255 * invDeg);
            int to = (int)(255 * deg);

            switch (type)
            {
                case 0:
                    return Color.FromArgb(255, to, 0);
                case 1:
                    return Color.FromArgb(from, 255, 0);
                case 2:
                    return Color.FromArgb(0, 255, to);
                case 3:
                    return Color.FromArgb(0, from, 255);
                case 4:
                    return Color.FromArgb(to, 0, 255);
                case 5:
                    return Color.FromArgb(255, 0, from);
                case 6:
                    return Color.FromArgb(255, 0, 0);

            }

            return Color.White;
        }

        static void ArduinoStuff()
        {
            buffer = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            bufferGraphics = Graphics.FromImage(buffer);

            SerialPort serial = new SerialPort();
            serial.PortName = "COM5";
            serial.BaudRate = 115200;
            serial.Open();

            while (!Environment.HasShutdownStarted)
            {

                Color avg = GetAverageScreenColor();

                /*
                Color hue = GetHueRGB(avg);

                float sat = avg.GetSaturation();

                float scale = 1f - sat;

                byte r = hue.R; //(byte)(hue.R * (1f - scale) + avg.R * scale);
                byte g = hue.G; //(byte)(hue.G * (1f - scale) + avg.G * scale);
                byte b = hue.B; // (byte)(hue.B * (1f - scale) + avg.B * scale);

                float highest = Math.Max(r, Math.Max(g, b));
                float norm = 255f / highest;

                r = (byte)(r * norm);
                g = (byte)(g * norm);
                b = (byte)(b * norm);
                */

                byte r = avg.R;
                byte g = avg.G;
                byte b = avg.B;


                //Console.WriteLine($"<{r.ToString("D3")} {g.ToString("D3")} {b.ToString("D3")}>");
                serial.Write($"<{r.ToString("D3")} {g.ToString("D3")} {b.ToString("D3")}>");

                if (serial.BytesToRead >= 11)
                {
                    //Console.WriteLine(serial.ReadLine());
                }

            }

            bufferGraphics.Dispose();
            buffer.Dispose();

            serial.Close();

        }
    }
}
