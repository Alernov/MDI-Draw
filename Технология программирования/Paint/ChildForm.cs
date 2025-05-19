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
        private MainForm fmParent = null;  // Родительская форма
        Bitmap bmp1, bmpR;                // Основное изображение и временное для операций
        int width, height;                 // Размеры изображения
        Graphics gr;                       // Графический контекст для рисования
        Size s;                            // Размер выделенной области
        Pen penline, penCopy;             // Перо для рисования и перо для выделения
        List<Point> mPoints;               // Точки для рисования линий
        Color clr = Color.Black;           // Цвет по умолчанию
        bool fillF = false, copyR = false, insR = false; // Флаги заливки, копирования и вставки
        List<Brush> fillBrush = new List<Brush>(); // Кисти для заливки
        Color clrBr = Color.White;         // Цвет заливки
        Color clrWh = Color.White;         // Цвет фона
        int x, y, w, h;                    // Координаты и размеры фигур
        int t = 0;                         // Счетчик для кистей
        Rectangle rect;                    // Прямоугольник выделения

        public ChildForm()
        {
            InitializeComponent();
        }

        // Конструктор с передачей родительской формы
        public ChildForm(MainForm fmParent)
        {
            this.fmParent = fmParent;
            InitializeComponent();
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            bmp1 = new Bitmap(width, height);  // Создание нового изображения
            pictureBox1.Image = bmp1;          // Установка изображения в PictureBox
            gr = Graphics.FromImage(bmp1);     // Получение графического контекста
            gr.Clear(Color.White);             // Очистка белым цветом
            penCopy = new Pen(Color.PaleTurquoise, 1);  // Перо для выделения (пунктир)
            penCopy.DashStyle = DashStyle.Dash;
            rect = new Rectangle();            // Инициализация прямоугольника выделения
            penline = new Pen(clr, 4);        // Основное перо для рисования
        }

        // Свойство для открытия изображения
        public Bitmap bmpOpen
        {
            get { return (Bitmap)pictureBox1.Image; }
            set
            {
                pictureBox1.Image = value;
                gr = Graphics.FromImage(value);  // Обновление графического контекста
            }
        }

        // Свойство для выбора инструмента
        public int tool
        {
            get { return tool1; }
            set
            {
                int to = tool1;
                tool1 = value;
                penline = new Pen(clr, 4);  // Сброс пера

                // Установка флагов для инструментов копирования/вставки
                if (tool1 == 5 || tool1 == 7) copyR = true;
                if (tool1 == 6) insR = true;

                // Очистка холста (инструмент 8)
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
                    tool1 = to;  // Возврат предыдущего инструмента
                }
            }
        }
        int tool1 = 1;  // Текущий инструмент (по умолчанию - карандаш)

        // Свойство для цвета заливки
        public Color clrFil
        {
            get { return clrBr; }
            set { clrBr = value; fillF = true; fillBrush.Add(new SolidBrush(clrBr)); }
        }

        // Свойство для основного цвета
        public Color clr1
        {
            get { return clr; }
            set { clr = value; }
        }

        // Свойство для толщины пера (без заливки)
        public int penNonFil
        {
            get { return (int)penline.Width; }
            set { penline = new Pen(clr, value); fillF = false; }
        }

        // Обработчик нажатия кнопки мыши
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            mPoints = new List<Point>();
            Point pStart = new Point(x, y);
            mPoints.Add(pStart);

            // Для инструментов выделения (5 и 7) запоминаем начальную точку
            if (tool1 == 5 || tool1 == 7)
            {
                rect.X = e.X;
                rect.Y = e.Y;
            }
        }

        // Обработчик движения мыши
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Инструмент 1 - Карандаш
            if (tool1 == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    x = e.X;
                    y = e.Y;
                    mPoints.Add(e.Location);
                    penline = new Pen(clr, 4);
                    gr.DrawLines(penline, mPoints.ToArray());  // Рисование линии
                    pictureBox1.Refresh();
                }
            }

            // Инструменты 3 и 4 - Прямоугольник и Эллипс
            if (tool1 == 3 || tool1 == 4)
            {
                if (e.Button == MouseButtons.Left)
                {
                    w = e.X - x;  // Вычисление ширины
                    h = e.Y - y;  // Вычисление высоты
                    pictureBox1.Refresh();
                }
            }

            // Инструмент 2 - Линия
            if (tool1 == 2)
            {
                if (e.Button == MouseButtons.Left)
                {
                    w = e.X;
                    h = e.Y;
                    pictureBox1.Refresh();
                }
            }

            // Инструменты 5 и 7 - Выделение и Вырезание
            if (tool1 == 5 || tool1 == 7)
            {
                if (e.Button == MouseButtons.Left)
                {
                    rect.Width = e.X - rect.X;  // Вычисление ширины выделения
                    rect.Height = e.Y - rect.Y;  // Вычисление высоты выделения
                    pictureBox1.Refresh();
                }
            }
        }

        // Обработчик отпускания кнопки мыши
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Инструмент 4 - Эллипс
            if (tool1 == 4)
            {
                // Нормализация координат (для рисования в любом направлении)
                int drawX = w < 0 ? x + w : x;
                int drawY = h < 0 ? y + h : y;
                int drawWidth = Math.Abs(w);
                int drawHeight = Math.Abs(h);

                if (fillF)  // Если включена заливка
                {
                    gr.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                    gr.FillEllipse(fillBrush[t], drawX, drawY, drawWidth, drawHeight);
                }
                else
                    gr.DrawEllipse(penline, drawX, drawY, drawWidth, drawHeight);
                gr.Save();
                fillBrush.Add(new SolidBrush(clrBr));
            }

            // Инструмент 3 - Прямоугольник
            if (tool1 == 3)
            {
                // Нормализация координат
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

            // Инструмент 2 - Линия
            if (tool1 == 2)
            {
                gr.DrawLine(penline, x, y, w, h);
                gr.Save();
            }

            if (e.Button == MouseButtons.Left)
            {
                // Инструменты 5 и 7 - Копирование и Вырезание
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

                    // Копирование выделенной области во временное изображение
                    using (Graphics g = Graphics.FromImage(bmpR))
                    {
                        g.DrawImage(pictureBox1.Image, dest_rect, normalizedRect, GraphicsUnit.Pixel);
                    }

                    Clipboard.SetImage(bmpR);  // Помещение в буфер обмена

                    // Для инструмента 7 (Вырезание) - очистка выделенной области
                    if (tool1 == 7)
                    {
                        using (Graphics g = Graphics.FromImage(pictureBox1.Image))
                        {
                            Brush fillBrush = new SolidBrush(clrWh);
                            g.FillRectangle(fillBrush, normalizedRect);
                        }
                    }
                    pictureBox1.Refresh();
                    rect = new Rectangle();  // Сброс прямоугольника выделения
                }

                // Инструмент 6 - Вставка
                if (tool1 == 6 && insR == true)
                {
                    insR = false;
                    Image img = Clipboard.GetImage();  // Получение изображения из буфера
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

        // Обработчик события отрисовки PictureBox
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Инструмент 4 - Эллипс (предварительный просмотр)
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

            // Инструмент 3 - Прямоугольник (предварительный просмотр)
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

            // Инструмент 2 - Линия (предварительный просмотр)
            if (tool1 == 2)
            {
                penline = new Pen(clr, 4);
                e.Graphics.DrawLine(penline, x, y, w, h);
                e.Graphics.Save();
            }

            // Инструменты 5 и 7 - Отображение прямоугольника выделения
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