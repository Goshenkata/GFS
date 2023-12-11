using GFS;
using GFS.helper;
using System.Text;

namespace GFSGUI
{
    public partial class TextEditor : Form
    {
        private FileSystemManager _fsManager;
        private string _parentPath;
        private string _fileName;
        public TextEditor(FileSystemManager fsManager, string parentPath, string fileName)
        {
            InitializeComponent();
            _fsManager = fsManager;

            _parentPath = parentPath;
            if (parentPath[^1] != '/')
            {
                _parentPath = parentPath + '/';
            }
            _fileName = fileName;

            var fullPath = _parentPath + fileName;
            if (fileName == "")
                return;

            var text = fsManager.Cat(fullPath);
            if (text == "")
            {
                if (_fsManager.GetNode(fullPath)!.IsCorrupted)
                {
                    MessageBox.Show("The file you are trying to read seems to be corrupted", "File is corrupted",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            } 
            textBox1.Text = text;
            textBox2.Text = fileName;
        }

        private void writeBtn_Click(object sender, EventArgs e)
        {

            var fullPath = _parentPath + textBox2.Text;
            var oldPath = StringHelper.ConcatPath(_parentPath, _fileName);
            if (_fsManager.NodeExists(fullPath))
            {
                _fsManager.RmFile(oldPath);
                _fsManager.CreateFile(fullPath, Encoding.UTF8.GetBytes(textBox1.Text));
                Close();
            }
            else
            {
                _fsManager.CreateFile(fullPath, Encoding.UTF8.GetBytes(textBox1.Text));
                Close();
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
