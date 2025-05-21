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
using System.IO;

namespace Paint
{
    public partial class MainForm : Form
    {
        private int childFormNumber = 0;
        string childFormActive = "";
        int childActive;
        Bitmap bmp1, bmp2;
        int width, height, width2, height2;
        List<string> filenameActiveChildForm = new List<string>();
        Color clr = Color.Black;
        Color clrWh = Color.White;
        Color next;
        int r = 0, g = 0, b = 0;
        string line = "";

        public MainForm()
        {
            InitializeComponent();
            fillToolStripMenuItem.CheckOnClick = true;
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            ChildForm childForm = new ChildForm(this);
            childForm.MdiParent = this;
            childForm.Text = "Окно " + childFormNumber++;
            childForm.Show();
            width = childForm.pictureBox1.Width;
            height = childForm.pictureBox1.Height;
            filenameActiveChildForm.Add("");
            childActive = childFormNumber - 1;
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Все файлы (*.*)|*.*|Изображения (*.jpg;*.bmp;*.gif;*.png)|*.jpg;*.bmp;*.gif;*.png";

            if (this.ActiveMdiChild == null)
                ShowNewForm(this, null);

            childFormActive = ((ChildForm)this.ActiveMdiChild).Text;
            childActive = Convert.ToInt32(childFormActive.Substring(5));

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    filenameActiveChildForm[childActive] = openFileDialog1.FileName;
                    bmp1 = new Bitmap(filenameActiveChildForm[childActive]);
                    bmp2 = new Bitmap(width, height);
                    width2 = bmp1.Width;
                    height2 = bmp1.Height;

                    if (((ChildForm)this.ActiveMdiChild).pictureBox1.Image != null)
                    {
                        ((ChildForm)this.ActiveMdiChild).pictureBox1.Image.Dispose();
                    }

                    using (Graphics g = Graphics.FromImage(bmp2))
                    {
                        g.Clear(Color.White);
                        g.DrawImage(bmp1, new Rectangle(0, 0, Math.Min(width, width2), Math.Min(height, height2)));
                    }

                    ((ChildForm)this.ActiveMdiChild).bmpOpen = bmp2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(false);
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile(true);
        }

        private void SaveFile(bool saveAs)
        {
            var activeChild = this.ActiveMdiChild as ChildForm;
            if (activeChild?.pictureBox1.Image == null) return;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "BMP (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png";
                saveDialog.FilterIndex = 4;
                saveDialog.DefaultExt = "png";

                if (string.IsNullOrEmpty(filenameActiveChildForm[childActive]) || saveAs)
                {
                    if (saveDialog.ShowDialog() != DialogResult.OK)
                        return;

                    filenameActiveChildForm[childActive] = saveDialog.FileName;
                }

                try
                {
                    ImageFormat format = ImageFormat.Png;
                    switch (Path.GetExtension(filenameActiveChildForm[childActive]).ToLower())
                    {
                        case ".bmp": format = ImageFormat.Bmp; break;
                        case ".jpg": case ".jpeg": format = ImageFormat.Jpeg; break;
                        case ".gif": format = ImageFormat.Gif; break;
                        case ".png": default: format = ImageFormat.Png; break;
                    }

                    activeChild.GetCurrentImage().Save(filenameActiveChildForm[childActive], format);
                    MessageBox.Show("Файл сохранен успешно!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 5;
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 6;
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 7;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 8;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 2;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 3;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).tool = 4;
        }
        public bool FillMenuItemChecked
        {
            get { return fillToolStripMenuItem.Checked; }
            set { fillToolStripMenuItem.Checked = value; }
        }
        private void fillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var activeChild = this.ActiveMdiChild as ChildForm;
            if (activeChild != null)
            {
                // Сбрасываем текущий инструмент при включении заливки
                if (fillToolStripMenuItem.Checked)
                {
                    activeChild.tool = 0; // 0 - специальное значение для заливки
                }
                activeChild.IsFillEnabled = fillToolStripMenuItem.Checked;
            }
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                clr = colorDialog1.Color;
                if (this.ActiveMdiChild != null)
                {
                    ((ChildForm)this.ActiveMdiChild).FillColor = clr;
                }
            }
        }

        private void penWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                ((ChildForm)this.ActiveMdiChild).PenWidth = 4;
            }
        }
    }
}