namespace GFSGUI
{
    partial class TextEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            closeBtn = new Button();
            writeBtn = new Button();
            textBox1 = new TextBox();
            panel2 = new Panel();
            label1 = new Label();
            textBox2 = new TextBox();
            errText = new Label();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 2);
            tableLayoutPanel1.Controls.Add(textBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(panel2, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(800, 488);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(errText);
            panel1.Controls.Add(closeBtn);
            panel1.Controls.Add(writeBtn);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 417);
            panel1.Name = "panel1";
            panel1.Size = new Size(794, 68);
            panel1.TabIndex = 1;
            // 
            // closeBtn
            // 
            closeBtn.BackColor = Color.IndianRed;
            closeBtn.Font = new Font("Segoe UI", 12F);
            closeBtn.Location = new Point(454, 24);
            closeBtn.Name = "closeBtn";
            closeBtn.Size = new Size(110, 42);
            closeBtn.TabIndex = 1;
            closeBtn.Text = "Close";
            closeBtn.UseVisualStyleBackColor = false;
            closeBtn.Click += closeBtn_Click;
            // 
            // writeBtn
            // 
            writeBtn.BackColor = SystemColors.ActiveCaption;
            writeBtn.Font = new Font("Segoe UI", 12F);
            writeBtn.Location = new Point(152, 23);
            writeBtn.Name = "writeBtn";
            writeBtn.Size = new Size(101, 42);
            writeBtn.TabIndex = 0;
            writeBtn.Text = "Write";
            writeBtn.UseVisualStyleBackColor = false;
            writeBtn.Click += writeBtn_Click;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(3, 51);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(794, 360);
            textBox1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(label1);
            panel2.Controls.Add(textBox2);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(794, 42);
            panel2.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 9);
            label1.Name = "label1";
            label1.Size = new Size(67, 20);
            label1.TabIndex = 1;
            label1.Text = "filename";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(152, 6);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(596, 27);
            textBox2.TabIndex = 0;
            // 
            // errText
            // 
            errText.AutoSize = true;
            errText.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            errText.ForeColor = Color.DarkRed;
            errText.Location = new Point(9, 5);
            errText.Name = "errText";
            errText.Size = new Size(18, 20);
            errText.TabIndex = 6;
            errText.Text = "C";
            errText.Visible = false;
            // 
            // TextEditor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 488);
            Controls.Add(tableLayoutPanel1);
            Name = "TextEditor";
            Text = "TextEditor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textBox1;
        private Panel panel1;
        private Button closeBtn;
        private Button writeBtn;
        private Panel panel2;
        private Label label1;
        private TextBox textBox2;
        private Label errText;
    }
}