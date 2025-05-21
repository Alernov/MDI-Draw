using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Paint
{
    public partial class ChildForm : Form
    {
        private MainForm fmParent = null;
        Bitmap bmp1, bmpR;
        int width, height;
        Graphics gr;
        Size s;
        Pen penline, penCopy;
        List<Point> mPoints;
        Color clr = Color.Black;
        bool copyR = false, insR = false;
        Color clrWh = Color.White;
        int x, y, w, h;
        Rectangle rect;
        int tool1 = 1;
        private bool isFillEnabled = false;

        public ChildForm()
        {
            InitializeComponent();
        }

        public ChildForm(MainForm fmParent)
        {
            this.fmParent = fmParent;
            InitializeComponent();
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            bmp1 = new Bitmap(width, height);
            pictureBox1.Image = bmp1;
            gr = Graphics.FromImage(bmp1);
            gr.Clear(Color.White);
            penCopy = new Pen(Color.PaleTurquoise, 1);
            penCopy.DashStyle = DashStyle.Dash;
            rect = new Rectangle();
            penline = new Pen(PenColor, 4); // Используем PenColor вместо clr
        }

        public bool IsFillEnabled
        {
            get { return isFillEnabled; }
            set { isFillEnabled = value; }
        }

        public Bitmap bmpOpen
        {
            get { return (Bitmap)pictureBox1.Image; }
            set
            {
                pictureBox1.Image = value;
                gr = Graphics.FromImage(value);
            }
        }

        public Bitmap GetCurrentImage()
        {
            return pictureBox1.Image != null ? new Bitmap(pictureBox1.Image) : null;
        }

        public int tool
        {
            get { return tool1; }
            set
            {
                // Если устанавливается обычный инструмент (не заливка)
                if (value != 0)
                {
                    // Сбрасываем режим заливки
                    isFillEnabled = false;
                    if (fmParent != null)
                    {
                        fmParent.FillMenuItemChecked = false;
                    }
                }

                tool1 = value;

                // Всегда используем цвет пера для инструментов рисования
                penline = new Pen(PenColor, penline.Width);

                if (tool1 == 5 || tool1 == 7) copyR = true;
                if (tool1 == 6) insR = true;

                if (tool1 == 8)
                {
                    if (mPoints != null && mPoints.Count != 0)
                        mPoints.Clear();
                    gr.Clear(clrWh);
                    bmp1 = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(bmp1))
                        g.Clear(clrWh);
                    pictureBox1.Image = bmp1;
                    gr = Graphics.FromImage(bmp1);
                    tool1 = 1; // Возвращаемся к инструменту по умолчанию
                }
            }
        }

        public int PenWidth
        {
            get { return (int)penline.Width; }
            set { penline = new Pen(PenColor, value); } // Используем PenColor вместо clr
        }

        private Color penColor = Color.Black;

        public Color PenColor
        {
            get { return penColor; }
            set
            {
                penColor = value;
                penline = new Pen(penColor, penline.Width);
            }
        }

        public Color FillColor
        {
            get { return clr; }
            set { clr = value; }
        }

        private void FillArea(int x, int y, Color fillColor)
        {
            Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
            Color targetColor = bmp.GetPixel(x, y);

            if (targetColor.ToArgb() == fillColor.ToArgb())
                return;

            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(new Point(x, y));

            while (pixels.Count > 0)
            {
                Point pt = pixels.Pop();
                if (pt.X < 0 || pt.X >= bmp.Width || pt.Y < 0 || pt.Y >= bmp.Height)
                    continue;

                if (bmp.GetPixel(pt.X, pt.Y) == targetColor)
                {
                    bmp.SetPixel(pt.X, pt.Y, fillColor);

                    pixels.Push(new Point(pt.X + 1, pt.Y));
                    pixels.Push(new Point(pt.X - 1, pt.Y));
                    pixels.Push(new Point(pt.X, pt.Y + 1));
                    pixels.Push(new Point(pt.X, pt.Y - 1));
                }
            }

            pictureBox1.Image = bmp;
            gr = Graphics.FromImage(bmp);
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isFillEnabled && e.Button == MouseButtons.Left)
            {
                FillArea(e.X, e.Y, clr);
                return;
            }

            x = e.X;
            y = e.Y;
            mPoints = new List<Point>();
            Point pStart = new Point(x, y);
            mPoints.Add(pStart);

            if (tool1 == 5 || tool1 == 7)
            {
                rect.X = e.X;
                rect.Y = e.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (tool1 == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    x = e.X;
                    y = e.Y;
                    mPoints.Add(e.Location);
                    penline = new Pen(clr, PenWidth);
                    gr.DrawLines(penline, mPoints.ToArray());
                    pictureBox1.Refresh();
                }
            }

            if (tool1 == 3 || tool1 == 4)
            {
                if (e.Button == MouseButtons.Left)
                {
                    w = e.X - x;
                    h = e.Y - y;
                    pictureBox1.Refresh();
                }
            }

            if (tool1 == 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    w = e.X;
                    h = e.Y;
                    pictureBox1.Refresh();
                }
            }

            if (tool1 == 5 || tool1 == 7)
            {
                if (e.Button == MouseButtons.Left)
                {
                    rect.Width = e.X - rect.X;
                    rect.Height = e.Y - rect.Y;
                    pictureBox1.Refresh();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (tool1 == 4)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);
                gr.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                gr.Save();
            }

            if (tool1 == 3)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);
                gr.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                gr.Save();
            }

            if (tool1 == 2)
            {
                gr.DrawLine(penline, x, y, w, h);
                gr.Save();
            }

            if (e.Button == MouseButtons.Left)
            {
                if (tool1 == 5 || tool1 == 7 && copyR == true)
                {
                    copyR = false;

                    Rectangle normalizedRect = rect;
                    if (rect.Width < 0)
                    {
                        normalizedRect.X += rect.Width;
                        normalizedRect.Width = -rect.Width;
                    }
                    if (rect.Height < 0)
                    {
                        normalizedRect.Y += rect.Height;
                        normalizedRect.Height = -rect.Height;
                    }

                    s = normalizedRect.Size;
                    bmpR = new Bitmap(s.Width, s.Height);
                    Rectangle dest_rect = new Rectangle(0, 0, bmpR.Width, bmpR.Height);

                    using (Graphics g = Graphics.FromImage(bmpR))
                    {
                        g.DrawImage(pictureBox1.Image, dest_rect, normalizedRect, GraphicsUnit.Pixel);
                    }

                    Clipboard.SetImage(bmpR);

                    if (tool1 == 7)
                    {
                        using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                        {
                            Brush fillBrush = new SolidBrush(clrWh);
                            g.FillRectangle(fillBrush, normalizedRect);
                        }
                    }
                    pictureBox1.Refresh();
                    rect = new Rectangle();
                }

                if (tool1 == 6 && insR == true)
                {
                    insR = false;
                    Image img = Clipboard.GetImage();
                    if (img != null)
                    {
                        Rectangle src_rect = new Rectangle(0, 0, img.Width, img.Height);
                        using (Graphics g1 = Graphics.FromImage(pictureBox1.Image))
                        {
                            g1.DrawImage(img, e.X, e.Y, src_rect, GraphicsUnit.Pixel);
                        }
                        pictureBox1.Refresh();
                    }
                }
            }
            x = y = w = h = 0;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (tool1 == 4)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);
                e.Graphics.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                e.Graphics.Save();
            }

            if (tool1 == 3)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);
                e.Graphics.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                e.Graphics.Save();
            }

            if (tool1 == 2)
            {
                e.Graphics.DrawLine(penline, x, y, w, h);
                e.Graphics.Save();
            }

            if (tool1 == 5 || tool1 == 7)
            {
                Rectangle drawRect = rect;
                if (rect.Width < 0)
                {
                    drawRect.X += rect.Width;
                    drawRect.Width = -rect.Width;
                }
                if (rect.Height < 0)
                {
                    drawRect.Y += rect.Height;
                    drawRect.Height = -rect.Height;
                }
                e.Graphics.DrawRectangle(penCopy, drawRect);
            }
        }
    }
}