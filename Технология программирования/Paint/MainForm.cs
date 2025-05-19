using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Paint

{
    public partial class MainForm : Form
    {
        private int childFormNumber = 0;
        String childFormActive = "";
        int childActive;
        Bitmap bmp1, bmp2;
        int width, height, width2, height2;
        List<String> filenameActiveChildForm = new List<String>();
        Color clr = Color.Black;
        List<Brush> fillBrush = new List<Brush>();
        Color clrBr = Color.White;
        Color clrWh = Color.White;
        Color next;
        int r = 0, g = 0, b = 0;
        String line="";
        

        public MainForm()
        {
            InitializeComponent();
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
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Все файлы (*.*)|*.*| Изображение jpg (*.jpg)|*.jpg| Изображение bmp (*.bmp)|*.bmp| Изображение gif (*.gif)|*.gif| Изображение png (*.png)|*.png";
            if (((ChildForm)this.ActiveMdiChild) == null)
                ShowNewForm(this, null);
            childFormActive = ((ChildForm)this.ActiveMdiChild).Text;
            childActive = Convert.ToInt32(childFormActive.Substring(5));
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
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
                if (width2 < width)
                {
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            if (i < width2 && j < height2)
                            {
                                Color cur = bmp1.GetPixel(i, j);
                                r = cur.R;
                                g = cur.G;
                                b = cur.B;
                                next = Color.FromArgb((byte)r, (byte)g, (byte)b);
                                bmp2.SetPixel(i, j, next);
                            }
                            else
                                bmp2.SetPixel(i, j, clrWh);
                        }
                }
                else if (height2 < height)
                { 
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            if (j < height2)
                            {
                                Color cur = bmp1.GetPixel(i, j);
                                r = cur.R;
                                g = cur.G;
                                b = cur.B;
                                next = Color.FromArgb((byte)r, (byte)g, (byte)b);
                                bmp2.SetPixel(i, j, next);
                            }
                            else
                                bmp2.SetPixel(i, j, clrWh);
                        }
                }
                else 
                {
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                        {
                            Color cur = bmp1.GetPixel(i, j);
                            r = cur.R;
                            g = cur.G;
                            b = cur.B;
                            next = Color.FromArgb((byte)r, (byte)g, (byte)b);
                            bmp2.SetPixel(i, j, next);
                        }
                }
                ((ChildForm)this.ActiveMdiChild).bmpOpen = bmp2;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
{
    var activeChild = (ChildForm)this.ActiveMdiChild;
    if (activeChild?.pictureBox1.Image != null)
    {
        using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
        {
            saveFileDialog1.Filter = "Изображение bmp (*.bmp)|*.bmp|Изображение jpg (*.jpg)|*.jpg|Изображение gif (*.gif)|*.gif|Изображение png (*.png)|*.png";
            int childActive = Convert.ToInt32(activeChild.Text.Substring(5));

            if (string.IsNullOrEmpty(filenameActiveChildForm[childActive]) && saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                filenameActiveChildForm[childActive] = saveFileDialog1.FileName;
            }

            Image bmpSave = new Bitmap(activeChild.pictureBox1.Image);
            bmpSave.Save(filenameActiveChildForm[childActive]);
        }
    }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Изображение bmp (*.bmp)|*.bmp| Изображение jpg (*.jpg)|*.jpg| Изображение gif (*.gif)|*.gif| Изображение png (*.png)|*.png|Все файлы (*.*)|*.*";
            childFormActive = ((ChildForm)this.ActiveMdiChild).Text;
            childActive = Convert.ToInt32(childFormActive.Substring(5));
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                filenameActiveChildForm[childActive] = saveFileDialog1.FileName;
                Image bmpSave = ((ChildForm)this.ActiveMdiChild).pictureBox1.Image; 
                bmpSave = new Bitmap(bmpSave);
                bmpSave.Save(filenameActiveChildForm[childActive]);
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


        private void nonFillingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ChildForm)this.ActiveMdiChild).penNonFil = 4;
        }

       
        }
    }
