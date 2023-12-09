namespace GFSGUI
{
    public partial class ImageViewer : Form
    {
        public ImageViewer(byte[] data)
        {
            InitializeComponent();
            Image image = LoadImageFromBytes(data);
            pictureBox1.Image = image;
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

        private void ImageViewer_Load(object sender, EventArgs e)
        {

        }
    }
}
