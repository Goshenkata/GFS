using GFS;

namespace GFSGUI
{
    public partial class InputForm : Form
    {
        public string? output = null;
        private FileSystemManager _fileSystemManager;
        public InputForm(string formText, FileSystemManager fsManager)
        {
            InitializeComponent();
            _fileSystemManager = fsManager;
            label1.Text = formText;
            this.Text = formText;
        }

        private void writeBtn_Click(object sender, EventArgs e)
        {
            string dirName = textBox1.Text;
            var operationResult = _fileSystemManager.Mkdir(_fileSystemManager.CurrentPath,dirName);
            if (operationResult.Success)
            {
                Close();
            } else
            {
                label2.Text = operationResult.Message;
                label2.Visible = true;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
