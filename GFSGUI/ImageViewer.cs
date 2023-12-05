using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GFSGUI
{
    public partial class ImageViewer : Form
    {
        public ImageViewer(byte[] data)
        {
            InitializeComponent();
            Image image = LoadImageFromBytes(data);
            PictureBox pictureBox = new PictureBox();
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox.Image = image;

            this.Size = pictureBox.Size;
            this.Controls.Add(pictureBox);
        }

        private Image LoadImageFromBytes(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    Image image = Image.FromStream(ms);
                    return image;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
