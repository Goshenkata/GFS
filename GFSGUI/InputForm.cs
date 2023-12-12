﻿using GFS;
using GFS.DTO;
using GFS.helper;

namespace GFSGUI
{
    public partial class InputForm : Form
    {
        private FileSystemManager _fileSystemManager;
        private InputFormOperationEnum _operation;
        private FileSystemNode? _fileSystemNode;
        public InputForm(string formText, FileSystemManager fsManager, InputFormOperationEnum op, FileSystemNode node = null)
        {
            InitializeComponent();
            _fileSystemManager = fsManager;
            _operation = op;
            _fileSystemNode = node;
            label1.Text = formText;
            this.Text = formText;
            if (_operation == InputFormOperationEnum.Rename)
            {
                textBox1.Text = node.Name;
                writeBtn.Text = "Rename";
            }
            else
            {
                writeBtn.Text = "Create";
            }
        }


        private void writeBtn_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            OperationResult operationResult = new OperationResult() { Success = false, Message = "" };
            switch (_operation)
            {
                case InputFormOperationEnum.Mkdir:
                    string pat = "/";
                    if (_fileSystemNode != _fileSystemManager.GetNode("/"))
                    {
                        pat = StringHelper.ConcatPath(_fileSystemNode.Path, _fileSystemNode.Name);
                    }
                    operationResult = _fileSystemManager.Mkdir(pat, text);
                    break;
                case InputFormOperationEnum.Rename:
                    if (_fileSystemNode != null)
                    {
                        operationResult = _fileSystemManager.RenameNode(ref _fileSystemNode, text);
                    }
                    break;
            }
            if (operationResult.Success)
            {
                Close();
            }
            else
            {
                label2.Text = operationResult.Message;
                label2.Visible = true;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        public enum InputFormOperationEnum
        {
            Mkdir, Rename
        }
    }
}
