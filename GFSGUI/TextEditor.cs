using GFS;
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
    public partial class TextEditor : Form
    {
        FileSystemManager _fsManager;
        string _fullPath;
        public TextEditor(FileSystemManager fsManager, string fullPath)
        {
            InitializeComponent();
            _fsManager = fsManager;
            _fullPath = fullPath;
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
        }

        private void writeBtn_Click(object sender, EventArgs e)
        {

            _fsManager.CreateFile(_fullPath, Encoding.UTF8.GetBytes(textBox1.Text));
            Close();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
