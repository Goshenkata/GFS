using GFS;
using GFS.DTO;
using GFS.Structures;
namespace GFSGUI
{
    public partial class Form1 : Form
    {
        FileSystemManager _fsManager;
        
        public Form1()
        {
            InitializeComponent();
            //todo handle create
            _fsManager = new FileSystemManager();
            _fsManager.LoadFs();
            UpdateListView();
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
            foreach (var item in sorted)
            {
                ListViewItem listViewItem = new ListViewItem(item.Name);
                listView1.Items.Add(listViewItem);
            }
        }
    }
}
