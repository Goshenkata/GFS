using GFS;
using GFS.DTO;
using GFS.helper;
using GFS.Structures;
namespace GFSGUI
{
    public partial class Form1 : Form
    {
        FileSystemManager _fsManager;
        MyStack<string> prevStack = new MyStack<string>();
        MyStack<string> forwardStack = new MyStack<string>();

        private void UpdateHistoryButtonsState()
        {
            if (prevStack.isEmpty())
                goBackButton.Enabled = false;
            else goBackButton.Enabled = true;

            if (forwardStack.isEmpty())
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
                foreach (var child in myFsNode.Children)
                {
                    TreeNode newNode = new TreeNode(child.Name);
                    newNode.ImageIndex = ResolveImageId(child);
                    newNode.SelectedImageIndex = ResolveImageId(child);
                    if (child.Children.Count > 0)
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
        }

        private int ResolveImageId(FileLs item)
        {
            if (item.IsDirectory)
            {
                return !item.IsCorrupted ? 0 : 1;
            }
            return !item.IsCorrupted ? 2 : 3;
        }

        private int ResolveImageId(FileSystemNode item)
        {
            if (item.IsDirectory)
            {
                return !item.IsCorrupted ? 0 : 1;
            }
            return !item.IsCorrupted ? 2 : 3;
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            string selectedItemName = listView1.SelectedItems[0].Text;
            var delimiter = _fsManager.CurrentPath[^1] != '/' ? "/" : "";
            string fullPath = _fsManager.CurrentPath + delimiter + selectedItemName;
            if (_fsManager.GetNode(fullPath)!.IsDirectory)
            {
                prevStack.Push(_fsManager.CurrentPath);
                forwardStack.Clear();
                _fsManager.CurrentPath = fullPath;
                UpdateListView();
                UpdateHistoryButtonsState();
            }
            else
            {
                TextEditor textEditor = new TextEditor(_fsManager, _fsManager.CurrentPath, selectedItemName);
                textEditor.Show();
            }
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            forwardStack.Push(_fsManager.CurrentPath);
            _fsManager.CurrentPath = prevStack.Pop();
            UpdateListView();
            UpdateHistoryButtonsState();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            prevStack.Push(_fsManager.CurrentPath);
            _fsManager.CurrentPath = forwardStack.Pop();
            UpdateListView();
            UpdateHistoryButtonsState();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputForm inputForm = new InputForm(Messages.CreateDir, _fsManager);
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
                    var delimiter = _fsManager.CurrentPath[^1] == '/' ? "" : "/";
                    var myPath = _fsManager.CurrentPath + delimiter + fileName;
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
            if (node != null)
            {
                label1.Text = node.ToString();
            }
            else
            {
                label1.Text = "NULL";
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.ImageIndex == 0)
            {
                _fsManager.CurrentPath = getTreeNodePath(e.Node);
            }
            else if (e.Node.ImageIndex == 2)
            {
                TextEditor textEditor = new TextEditor(_fsManager, getTreeNodePath(e.Node.Parent), e.Node.Text);
                textEditor.Show();
            }
            UpdateListView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TextEditor textEditor = new TextEditor(_fsManager, _fsManager.CurrentPath, "");
            textEditor.ShowDialog();
            UpdateListView();
            LoadTreeView(_fsManager.CurrentPath);
        }
    }
}
