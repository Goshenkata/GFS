using GFS;
using GFS.helper;

namespace GFSGUI
{
    public partial class CreateFs : Form
    {
        FileSystemManager _fsManager;
        public CreateFs(FileSystemManager fsManager)
        {
            InitializeComponent();
            _fsManager = fsManager;
        }

        private void writeBtn_Click(object sender, EventArgs e)
        {
            long maxSize = (long)numericUpDown1.Value;
            int sectorSize = (int)numericUpDown2.Value;
            var dataSize = comboBox1.Text;
            if (dataSize == "")
            {
                label3.Visible = true;
                label3.Text = Messages.EnterData;
                return;
            }
            switch (dataSize)
            {
                case "MB":
                    maxSize *= 1024;
                    break;
                case "GB":
                    maxSize *= 1024 * 1024;
                    break;
            }
            sectorSize *= 1024;
            maxSize *= 1024;
            var result = _fsManager.CreateFilesystem(maxSize, sectorSize);
            if (!result.Success)
            {
                label3.Text = result.Message;
                label3.Visible = true;
                return;
            }
            Close();
            DialogResult = DialogResult.OK;
        }
    }
}
