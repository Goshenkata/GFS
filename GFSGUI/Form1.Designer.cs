namespace GFSGUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            TreeNode treeNode1 = new TreeNode("/");
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            goBackButton = new Button();
            imageList2 = new ImageList(components);
            forwardButton = new Button();
            button1 = new Button();
            button2 = new Button();
            label1 = new Label();
            listView1 = new ListView();
            imageList1 = new ImageList(components);
            treeView1 = new TreeView();
            imageList3 = new ImageList(components);
            openFileDialog1 = new OpenFileDialog();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
            tableLayoutPanel1.Controls.Add(listView1, 1, 1);
            tableLayoutPanel1.Controls.Add(treeView1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(982, 553);
            tableLayoutPanel1.TabIndex = 0;
            tableLayoutPanel1.Paint += tableLayoutPanel1_Paint;
            // 
            // flowLayoutPanel1
            // 
            tableLayoutPanel1.SetColumnSpan(flowLayoutPanel1, 2);
            flowLayoutPanel1.Controls.Add(goBackButton);
            flowLayoutPanel1.Controls.Add(forwardButton);
            flowLayoutPanel1.Controls.Add(button1);
            flowLayoutPanel1.Controls.Add(button2);
            flowLayoutPanel1.Controls.Add(label1);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(976, 47);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // goBackButton
            // 
            goBackButton.Enabled = false;
            goBackButton.ImageIndex = 1;
            goBackButton.ImageList = imageList2;
            goBackButton.Location = new Point(3, 3);
            goBackButton.Name = "goBackButton";
            goBackButton.Size = new Size(35, 35);
            goBackButton.TabIndex = 0;
            goBackButton.UseVisualStyleBackColor = true;
            goBackButton.Click += goBackButton_Click;
            // 
            // imageList2
            // 
            imageList2.ColorDepth = ColorDepth.Depth32Bit;
            imageList2.ImageStream = (ImageListStreamer)resources.GetObject("imageList2.ImageStream");
            imageList2.TransparentColor = Color.Transparent;
            imageList2.Images.SetKeyName(0, "right-arrow.png");
            imageList2.Images.SetKeyName(1, "arrow.png");
            // 
            // forwardButton
            // 
            forwardButton.Enabled = false;
            forwardButton.ImageIndex = 0;
            forwardButton.ImageList = imageList2;
            forwardButton.Location = new Point(44, 3);
            forwardButton.Name = "forwardButton";
            forwardButton.Size = new Size(35, 35);
            forwardButton.TabIndex = 1;
            forwardButton.TextImageRelation = TextImageRelation.ImageAboveText;
            forwardButton.UseVisualStyleBackColor = true;
            forwardButton.Click += forwardButton_Click;
            // 
            // button1
            // 
            button1.Location = new Point(85, 3);
            button1.Name = "button1";
            button1.Size = new Size(63, 35);
            button1.TabIndex = 2;
            button1.Text = "mkdir";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(154, 3);
            button2.Name = "button2";
            button2.Size = new Size(63, 35);
            button2.TabIndex = 3;
            button2.Text = "import";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(223, 0);
            label1.Name = "label1";
            label1.Size = new Size(50, 20);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // listView1
            // 
            listView1.Dock = DockStyle.Fill;
            listView1.GroupImageList = imageList1;
            listView1.LargeImageList = imageList1;
            listView1.Location = new Point(199, 56);
            listView1.Name = "listView1";
            listView1.Size = new Size(780, 473);
            listView1.SmallImageList = imageList1;
            listView1.StateImageList = imageList1;
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Tile;
            listView1.ItemActivate += listView1_ItemActivate;
            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "folder.png");
            imageList1.Images.SetKeyName(1, "folder(1).png");
            imageList1.Images.SetKeyName(2, "document.png");
            imageList1.Images.SetKeyName(3, "document(1).png");
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList3;
            treeView1.Location = new Point(3, 56);
            treeView1.Name = "treeView1";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node0";
            treeNode1.Text = "/";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode1 });
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(190, 473);
            treeView1.TabIndex = 2;
            treeView1.AfterCollapse += treeView1_AfterCollapse;
            treeView1.BeforeExpand += BeforeExpand;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // imageList3
            // 
            imageList3.ColorDepth = ColorDepth.Depth32Bit;
            imageList3.ImageStream = (ImageListStreamer)resources.GetObject("imageList3.ImageStream");
            imageList3.TransparentColor = Color.Transparent;
            imageList3.Images.SetKeyName(0, "folder.png");
            imageList3.Images.SetKeyName(1, "folder(1).png");
            imageList3.Images.SetKeyName(2, "document.png");
            imageList3.Images.SetKeyName(3, "document(1).png");
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "GFS";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
        private ListView listView1;
        private TreeView treeView1;
        private ImageList imageList1;
        private Button goBackButton;
        private Button forwardButton;
        private ImageList imageList2;
        private Button button1;
        private Button button2;
        private OpenFileDialog openFileDialog1;
        private ImageList imageList3;
        private Label label1;
    }
}
