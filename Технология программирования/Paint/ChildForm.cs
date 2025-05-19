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
        bool fillF = false, copyR = false, insR = false;
        List<Brush> fillBrush = new List<Brush>();
        Color clrBr = Color.White;
        Color clrWh = Color.White;
        int x, y, w, h;
        int t = 0;
        Rectangle rect;

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
            penline = new Pen(clr, 4);
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

        public int tool
        {
            get { return tool1; }
            set
            {
                int to = tool1;
                tool1 = value; penline = new Pen(clr, 4);
                if (tool1 == 5 || tool1 == 7) copyR = true;
                if (tool1 == 6) insR = true;
                if (tool1 == 8)
                {
                    if (mPoints.Count != 0)
                        mPoints.Clear();
                    gr.Clear(clrWh);
                    bmp1 = new Bitmap(width, height);
                    using (Graphics g = Graphics.FromImage(bmp1))
                        g.Clear(clrWh);
                    pictureBox1.Image = bmp1;
                    gr = Graphics.FromImage(bmp1);
                    tool1 = to;
                }
            }
        }
        int tool1 = 1;

        public Color clrFil
        {
            get { return clrBr; }
            set { clrBr = value; fillF = true; fillBrush.Add(new SolidBrush(clrBr)); }
        }
        public Color clr1
        {
            get { return clr; }
            set { clr = value; }
        }
        public int penNonFil
        {
            get { return (int)penline.Width; }
            set { penline = new Pen(clr, value); fillF = false; }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
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
                    penline = new Pen(clr, 4);
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

                if (fillF)
                {
                    gr.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                    gr.FillEllipse(fillBrush[t], drawX, drawY, drawWidth, drawHeight);
                }
                else
                    gr.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                gr.Save();
                fillBrush.Add(new SolidBrush(clrBr));
            }
            if (tool1 == 3)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);

                if (fillF)
                {
                    gr.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                    gr.FillRectangle(fillBrush[t], drawX, drawY, drawWidth, drawHeight);
                }
                else
                    gr.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                gr.Save();
                fillBrush.Add(new SolidBrush(clrBr));
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

                    // Нормализация прямоугольника выделения
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
            // Сброс координат
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

                if (fillF)
                {
                    penline = new Pen(clr, 7);
                    t++;
                    fillBrush.Add(new SolidBrush(clrBr));
                    e.Graphics.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                    e.Graphics.FillEllipse(fillBrush[t], drawX, drawY, drawWidth, drawHeight);
                }
                else
                {
                    penline = new Pen(clr, 4);
                    e.Graphics.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                }
                e.Graphics.Save();
            }
            if (tool1 == 3)
            {
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);

                if (fillF)
                {
                    penline = new Pen(clr, 7);
                    t++;
                    fillBrush.Add(new SolidBrush(clrBr));
                    e.Graphics.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                    e.Graphics.FillRectangle(fillBrush[t], drawX, drawY, drawWidth, drawHeight);
                }
                else
                {
                    penline = new Pen(clr, 4);
                    e.Graphics.DrawRectangle(penline, drawX, drawY, drawWidth, drawHeight);
                }
                e.Graphics.Save();
            }
            if (tool1 == 2)
            {
                penline = new Pen(clr, 4);
                e.Graphics.DrawLine(penline, x, y, w, h);
                e.Graphics.Save();
            }
            if (tool1 == 5 || tool1 == 7)
            {
                // Нормализация прямоугольника выделения для отрисовки
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
