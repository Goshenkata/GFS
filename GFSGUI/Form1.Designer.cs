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
            TreeNode treeNode1 = new TreeNode("Node0");
            TreeNode treeNode2 = new TreeNode("Node3");
            TreeNode treeNode3 = new TreeNode("Node2", new TreeNode[] { treeNode2 });
            TreeNode treeNode4 = new TreeNode("Node4");
            TreeNode treeNode5 = new TreeNode("Node1", new TreeNode[] { treeNode3, treeNode4 });
            tableLayoutPanel1 = new TableLayoutPanel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            goBackButton = new Button();
            imageList2 = new ImageList(components);
            forwardButton = new Button();
            listView1 = new ListView();
            imageList1 = new ImageList(components);
            treeView1 = new TreeView();
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
            treeView1.Location = new Point(3, 56);
            treeView1.Name = "treeView1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Node0";
            treeNode2.Name = "Node3";
            treeNode2.Text = "Node3";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Node2";
            treeNode4.Name = "Node4";
            treeNode4.Text = "Node4";
            treeNode5.Name = "Node1";
            treeNode5.Text = "Node1";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode5 });
            treeView1.Size = new Size(190, 473);
            treeView1.TabIndex = 2;
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
    }
}
