using GFS;
using GFS.DTO;
using GFS.helper;
using GFS.Structures;
namespace GFSGUI
{
    //no node name validation write in cli
    public partial class Form1 : Form
    {
        FileSystemManager _fsManager;
        MyStack<string> _prevStack = new MyStack<string>();
        MyStack<string> _forwardStack = new MyStack<string>();
        private string _selectedNodePath = null;

        private FileSystemNode resolveNode(bool dirOnly)
        {
            if (_selectedNodePath == null)
            {
                if (dirOnly)
                {
                    return _fsManager.GetNode(_fsManager.CurrentPath);
                }
                return null;
            }
            return _fsManager._fsData.GetNodeByPath(_selectedNodePath);
        }

        private void UpdateHistoryButtonsState()
        {
            if (_prevStack.isEmpty())
                goBackButton.Enabled = false;
            else goBackButton.Enabled = true;

            if (_forwardStack.isEmpty())
                forwardButton.Enabled = false;
            else forwardButton.Enabled = true;
        }
        public Form1()
        {
            _fsManager = new FileSystemManager();
            if (!_fsManager.IsInit())
            {

                CreateFs d = new CreateFs(_fsManager);
                DialogResult result = d.ShowDialog();
                if (result != DialogResult.OK)
                {

                    Close();
                    Environment.Exit(-1);
                    return;
                }
            }
            else
            {
                _fsManager.LoadFs();
            }
            InitializeComponent();
            flowLayoutPanel1.Controls.Clear();

            UpdateSelectedNode(_fsManager.GetNode("/"));
            UpdateListView();
            treeView1.Nodes[0].Nodes.Add(new TreeNode());
            LoadTreeView("/");
        }

        //returns bool if loading children is necessary
        private bool LoadTreeView(string fullPath)
        {
            var myFsNode = _fsManager.GetNode(fullPath);
            TreeNode treeNodeToLoad = FindTreeNode(fullPath);

            if (treeNodeToLoad == null)
                return false;

            if (treeNodeToLoad.ImageIndex == 0)
            {
                treeNodeToLoad.Nodes.Clear();
                var childrenIds = _fsManager._fsData.GetChildren(myFsNode);
                foreach (var childId in childrenIds)
                {
                    var child = _fsManager._fsData.LoadById(childId);
                    TreeNode newNode = new TreeNode(child.Name);
                    newNode.ImageIndex = ResolveImageId(child);
                    newNode.SelectedImageIndex = ResolveImageId(child);
                    if (_fsManager._fsData.GetChildren(child).Count > 0)
                    {
                        newNode.Nodes.Add(new TreeNode());
                    }
                    treeNodeToLoad.Nodes.Add(newNode);
                }
            }
            return true;
        }

        private TreeNode? FindTreeNode(string fullPath)
        {
            var current = treeView1.Nodes[0];
            if (fullPath == "/")
            {
                return current;
            }
            string[] path = StringHelper.Split(fullPath, '/');
            foreach (var item in path)
            {
                for (int i = 0; i < current.Nodes.Count; i++)
                {
                    var node = current.Nodes[i];
                    if (node.Text == item)
                    {
                        current = node;
                        break;
                    }
                }
            }
            return current;
        }
        private void BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            LoadTreeView(getTreeNodePath(e.Node));
        }

        private string getTreeNodePath(TreeNode node)
        {
            MyList<string> pathParts = new MyList<string>();
            var current = node;
            while (current != treeView1.Nodes[0])
            {
                pathParts.AddFirst(current.Text);
                current = current.Parent;
            }
            var path = StringHelper.Join(pathParts.GetArray(), "/");
            if (path == "")
            {
                path = "/";
            }
            if (path[0] != '/')
            {
                path = '/' + path;
            }
            return path;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void UpdateListView()
        {
            var data = _fsManager.Ls(_fsManager.CurrentPath);
            var sorted = new MyList<FileLs>();
            //add directories first and then files;
            foreach (var item in data)
                if (item.IsDirectory)
                    sorted.AddLast(item);
            foreach (var item in data)
                if (!item.IsDirectory)
                    sorted.AddLast(item);
            listView1.Items.Clear();
            foreach (var item in sorted)
            {
                ListViewItem listViewItem = new ListViewItem(item.Name);
                listViewItem.ImageIndex = ResolveImageId(item);
                Font font = new Font(FontFamily.GenericSansSerif, 14);
                listViewItem.Font = font;
                listView1.Items.Add(listViewItem);
            }
            errText.Visible = false; ;
        }

        private int ResolveImageId(FileLs item)
        {
            if (item.IsDirectory)
            {
                return !item.IsCorrupted ? 0 : 1;
            }
            if (StringHelper.isImage(item.Name))
            {
                return !item.IsCorrupted ? 4 : 5;
            }
            return !item.IsCorrupted ? 2 : 3;
        }

        private int ResolveImageId(FileSystemNode item)
        {
            if (item.IsDirectory)
            {
                return !item.IsCorrupted ? 0 : 1;
            }
            if (StringHelper.isImage(item.Name))
            {
                return !item.IsCorrupted ? 4 : 5;
            }
            return !item.IsCorrupted ? 2 : 3;
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedItemName = listView1.SelectedItems[0].Text;
                string fullPath = StringHelper.ConcatPath(_fsManager.CurrentPath, selectedItemName);
                var node = _fsManager.GetNode(fullPath);
                _selectedNodePath = fullPath;
                if (node.IsDirectory)
                {
                    _prevStack.Push(_fsManager.CurrentPath);
                    _forwardStack.Clear();
                    _fsManager.CurrentPath = fullPath;
                    UpdateHistoryButtonsState();
                }
                else
                {
                    if (StringHelper.isImage(fullPath))
                    {
                        var bytes = _fsManager.GetBytes(fullPath);
                        ImageViewer image = new ImageViewer(bytes);
                        image.Show();
                    }
                    else
                    {
                        TextEditor textEditor = new TextEditor(_fsManager, _fsManager.CurrentPath, selectedItemName);
                        textEditor.ShowDialog();
                    }
                }
                UpdateListView();
                LoadTreeView(fullPath);
            }
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            _forwardStack.Push(_fsManager.CurrentPath);
            _fsManager.CurrentPath = _prevStack.Pop();
            UpdateSelectedNode(null);
            UpdateListView();
            UpdateHistoryButtonsState();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            _prevStack.Push(_fsManager.CurrentPath);
            _fsManager.CurrentPath = _forwardStack.Pop();
            UpdateSelectedNode(null);
            UpdateListView();
            UpdateHistoryButtonsState();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var node = resolveNode(true);
            InputForm inputForm = new InputForm(Messages.CreateDir, _fsManager, InputForm.InputFormOperationEnum.Mkdir, _fsManager.ResolvePath(node));
            inputForm.ShowDialog();
            UpdateListView();
            LoadTreeView(_fsManager.CurrentPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    var fullPath = openFileDialog1.FileNames[i];
                    var fileName = openFileDialog1.SafeFileNames[i];
                    var node = resolveNode(true);

                    var myParentPath = node == _fsManager.GetNode("/") ?
                        "/" :
                        _fsManager.ResolvePath(node);

                    var myPath = StringHelper.ConcatPath(myParentPath, fileName);
                    //todo add validation
                    _fsManager.ImportFile(fullPath, myPath, false);
                }
                UpdateListView();
            }
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.Nodes.Clear();
            e.Node.Nodes.Add(new TreeNode(""));
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var delimiter = _fsManager.CurrentPath[^1] == '/' ? "" : "/";
            var newSelectedItemPath = _fsManager.CurrentPath + delimiter + e.Item.Text;
            UpdateSelectedNode(_fsManager.GetNode(newSelectedItemPath));
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateSelectedNode(_fsManager.GetNode(getTreeNodePath(e.Node)));
        }
        private void UpdateSelectedNode(FileSystemNode node)
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Controls.Add(goBackButton);
            flowLayoutPanel1.Controls.Add(forwardButton);
            _selectedNodePath = _fsManager.ResolvePath(node);
            if (node != null)
            {
                var size = new Size(63, 35);
                MyList<Button> buttons = new MyList<Button>();

                Button renameBtn = new Button();
                renameBtn.Text = "rename";
                renameBtn.Size = new Size(size.Width + 20, size.Height);
                renameBtn.Click += renameBtnClick;
                buttons.AddLast(renameBtn);

                Button openBtn = new Button();
                openBtn.Size = size;
                openBtn.Text = "open";
                openBtn.Click += openBtnClick;
                buttons.AddLast(openBtn);


                if (node.IsDirectory)
                {
                    Button rmdirBtn = new Button();
                    rmdirBtn.Size = size;
                    rmdirBtn.Text = "rmdir";
                    rmdirBtn.Click += rmDirBtnClick;
                    buttons.AddLast(rmdirBtn);

                    flowLayoutPanel1.Controls.Add(button1);
                    flowLayoutPanel1.Controls.Add(button3);
                    flowLayoutPanel1.Controls.Add(button2);
                }
                else
                {

                    Button rmBtn = new Button();
                    rmBtn.Size = size;
                    rmBtn.Text = "rm";
                    rmBtn.Click += rmBtnClick;
                    buttons.AddLast(rmBtn);

                    Button exportBtn = new Button();
                    exportBtn.Size = size;
                    exportBtn.Text = "export";
                    exportBtn.Click += exportBtnClick;
                    buttons.AddLast(exportBtn);
                }

                flowLayoutPanel1.Controls.AddRange(buttons.ToArray());
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.ImageIndex == 0)
            {

                _prevStack.Push(_fsManager.CurrentPath);
                _fsManager.CurrentPath = getTreeNodePath(e.Node);
                UpdateHistoryButtonsState();
            }
            else if (e.Node.ImageIndex == 2)
            {
                TextEditor textEditor = new TextEditor(_fsManager, getTreeNodePath(e.Node.Parent), e.Node.Text);
                textEditor.Show();
            }
            else if (e.Node.ImageIndex == 4)
            {
                var bytes = _fsManager.GetBytes(getTreeNodePath(e.Node));
                ImageViewer image = new ImageViewer(bytes);
                image.Show();
            }
            UpdateListView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var node = resolveNode(true);

            if (!node.IsDirectory || listView1.SelectedItems.Count > 0)
            {
                node = _fsManager.GetNode(_fsManager.CurrentPath);
            }

            var fp = _fsManager.ResolvePath(node);
            if (node == _fsManager.GetNode("/"))
            {
                fp = "/";
            }

            TextEditor textEditor = new TextEditor(_fsManager, fp, "");
            textEditor.ShowDialog();
            UpdateListView();
            LoadTreeView(fp);
        }

        private void renameBtnClick(object sender, EventArgs e)
        {
            var node = resolveNode(false);
            if (node != null && node != _fsManager.GetNode("/"))
            {
                InputForm inputForm = new InputForm("Rename node", _fsManager, InputForm.InputFormOperationEnum.Rename, _selectedNodePath);
                inputForm.ShowDialog();
                UpdateListView();
                LoadTreeView(StringHelper.GetParentPath(_selectedNodePath));
                errText.Visible = false; ;
            }
            else
            {
                errText.Visible = true;
                errText.Text = Messages.NothingSelected;
            }
        }
        private void rmDirBtnClick(object sender, EventArgs e)
        {
            if (_selectedNodePath != null)
            {
                var parentPath = StringHelper.GetParentPath(_selectedNodePath);
                var opResult = _fsManager.Rmdir(parentPath, StringHelper.GetName(_selectedNodePath));
                if (_fsManager.CurrentPath == _selectedNodePath)
                {
                    _fsManager.CurrentPath = parentPath;
                    UpdateSelectedNode(_fsManager.GetNode(_fsManager.CurrentPath));
                }

                UpdateListView();
                LoadTreeView(StringHelper.GetParentPath(_selectedNodePath));
                if (!opResult.Success)
                {
                    errText.Visible = true;
                    errText.Text = opResult.Message;
                }
            }
        }
        private void rmBtnClick(object sender, EventArgs e)
        {
            var node = resolveNode(false);
            var fullPath = _fsManager.ResolvePath(node);
            _fsManager.RmFile(fullPath);
            UpdateListView();
            LoadTreeView(StringHelper.GetParentPath(_selectedNodePath));
            _selectedNodePath = null;
            UpdateSelectedNode(null);
        }
        private void openBtnClick(object sender, EventArgs e)
        {
            listView1_ItemActivate(sender, e);
        }

        private void exportBtnClick(object sender, EventArgs e)
        {
            var node = resolveNode(false);
            if (node != null && !node.IsDirectory)
            {
                var source = _fsManager.ResolvePath(node);
                var result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK && folderBrowserDialog1.SelectedPath != null)
                {
                    var destination = StringHelper.ConcatPath(folderBrowserDialog1.SelectedPath, StringHelper.GetName(_selectedNodePath));
                    _fsManager.Export(source, destination);
                }
            }
            else
            {
                errText.Visible = true;
                errText.Text = Messages.NothingSelected;
            }
        }
    }
}
