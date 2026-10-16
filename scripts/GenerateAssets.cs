using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AssetGen
{
    public class IconBuilder
    {
        public static void GenerateAll(string baseDir)
        {
            string assetsDir = Path.Combine(baseDir, "assets");
            string storeDir = Path.Combine(assetsDir, "store");
            Directory.CreateDirectory(storeDir);

            DrawIcon(512, 512, Path.Combine(assetsDir, "icon.png"));
            DrawIcon(44, 44, Path.Combine(storeDir, "Square44x44Logo.png"));
            DrawIcon(150, 150, Path.Combine(storeDir, "Square150x150Logo.png"));
            DrawIcon(310, 310, Path.Combine(storeDir, "Square310x310Logo.png"));
            DrawIcon(310, 150, Path.Combine(storeDir, "Wide310x150Logo.png"));
            DrawIcon(50, 50, Path.Combine(storeDir, "StoreLogo.png"));

            // Generate multi-size ICO
            int[] sizes = new int[] { 16, 32, 48, 64, 128, 256 };
            byte[][] pngBuffers = new byte[sizes.Length][];
            for (int i = 0; i < sizes.Length; i++)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap bmp = RenderBitmap(sizes[i], sizes[i]))
                    {
                        bmp.Save(ms, ImageFormat.Png);
                    }
                    pngBuffers[i] = ms.ToArray();
                }
            }

            string icoPath = Path.Combine(assetsDir, "icon.ico");
            using (FileStream fs = File.Create(icoPath))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write((ushort)0); // Reserved
                bw.Write((ushort)1); // Type 1=Icon
                bw.Write((ushort)sizes.Length);

                int offset = 6 + sizes.Length * 16;
                for (int i = 0; i < sizes.Length; i++)
                {
                    int sz = sizes[i];
                    bw.Write((byte)(sz >= 256 ? 0 : sz));
                    bw.Write((byte)(sz >= 256 ? 0 : sz));
                    bw.Write((byte)0); // Colors
                    bw.Write((byte)0); // Reserved
                    bw.Write((ushort)1); // Color planes
                    bw.Write((ushort)32); // BPP
                    bw.Write((uint)pngBuffers[i].Length);
                    bw.Write((uint)offset);
                    offset += pngBuffers[i].Length;
                }

                for (int i = 0; i < sizes.Length; i++)
                {
                    bw.Write(pngBuffers[i]);
                }
            }

            Console.WriteLine("All assets & icon.ico successfully generated!");
        }

        public static void DrawIcon(int w, int h, string outPath)
        {
            using (Bitmap bmp = RenderBitmap(w, h))
            {
                bmp.Save(outPath, ImageFormat.Png);
            }
            Console.WriteLine("Saved: " + outPath);
        }

        public static Bitmap RenderBitmap(int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.Transparent);

                float scale = Math.Min(w, h) / 512.0f;
                float pad = 24.0f * scale;
                float rectW = w - 2 * pad;
                float rectH = h - 2 * pad;
                float radius = 104.0f * scale;

                // Rounded Rect Squircle Path
                using (GraphicsPath path = new GraphicsPath())
                {
                    float d = radius * 2;
                    path.AddArc(pad, pad, d, d, 180, 90);
                    path.AddArc(pad + rectW - d, pad, d, d, 270, 90);
                    path.AddArc(pad + rectW - d, pad + rectH - d, d, d, 0, 90);
                    path.AddArc(pad, pad + rectH - d, d, d, 90, 90);
                    path.CloseFigure();

                    // Background Fluent Gradient
                    using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                        new PointF(pad, pad),
                        new PointF(w - pad, h - pad),
                        Color.FromArgb(255, 0, 134, 240),
                        Color.FromArgb(255, 0, 45, 90)))
                    {
                        g.FillPath(bgBrush, path);
                    }

                    // Border highlight
                    using (Pen pBorder = new Pen(Color.FromArgb(60, 255, 255, 255), Math.Max(1.0f, 2.0f * scale)))
                    {
                        g.DrawPath(pBorder, path);
                    }
                }

                float cx = w / 2.0f;
                float cy = h / 2.0f;

                // Subtle Geometry Circle Guide
                float circR = 140.0f * scale;
                using (Pen pCirc = new Pen(Color.FromArgb(80, 96, 205, 255), Math.Max(1.0f, 2.0f * scale)))
                {
                    pCirc.DashStyle = DashStyle.Dash;
                    g.DrawEllipse(pCirc, cx - circR, cy - circR, circR * 2, circR * 2);
                }

                // 3D Isometric Geometric Prism
                float u = 52.0f * scale;

                PointF[] topPts = new PointF[] {
                    new PointF(cx, cy - 2 * u),
                    new PointF(cx + 1.732f * u, cy - u),
                    new PointF(cx, cy),
                    new PointF(cx - 1.732f * u, cy - u)
                };

                PointF[] leftPts = new PointF[] {
                    new PointF(cx - 1.732f * u, cy - u),
                    new PointF(cx, cy),
                    new PointF(cx, cy + 2 * u),
                    new PointF(cx - 1.732f * u, cy + u)
                };

                PointF[] rightPts = new PointF[] {
                    new PointF(cx, cy),
                    new PointF(cx + 1.732f * u, cy - u),
                    new PointF(cx + 1.732f * u, cy + u),
                    new PointF(cx, cy + 2 * u)
                };

                // Fill Isometric Faces
                using (SolidBrush bTop = new SolidBrush(Color.FromArgb(240, 96, 205, 255)))
                {
                    g.FillPolygon(bTop, topPts);
                }
                using (SolidBrush bLeft = new SolidBrush(Color.FromArgb(240, 0, 90, 158)))
                {
                    g.FillPolygon(bLeft, leftPts);
                }
                using (SolidBrush bRight = new SolidBrush(Color.FromArgb(240, 0, 120, 212)))
                {
                    g.FillPolygon(bRight, rightPts);
                }

                // Edge outlines
                using (Pen pEdge = new Pen(Color.FromArgb(255, 255, 255, 255), Math.Max(1.0f, 2.5f * scale)))
                {
                    g.DrawPolygon(pEdge, topPts);
                    g.DrawPolygon(pEdge, leftPts);
                    g.DrawPolygon(pEdge, rightPts);
                }

                // Compass / Triangle Overlay
                using (Pen pComp = new Pen(Color.FromArgb(255, 255, 220, 100), Math.Max(1.0f, 3.0f * scale)))
                {
                    PointF[] compPts = new PointF[] {
                        new PointF(cx - 1.732f * u, cy + u),
                        new PointF(cx, cy - 2 * u),
                        new PointF(cx + 1.732f * u, cy + u)
                    };
                    g.DrawLines(pComp, compPts);
                }

                // Center Point Marker
                using (SolidBrush bCenter = new SolidBrush(Color.White))
                {
                    float cr = Math.Max(3.0f, 7.0f * scale);
                    g.FillEllipse(bCenter, cx - cr, cy - cr, cr * 2, cr * 2);
                }
            }
            return bmp;
        }
    }
}
