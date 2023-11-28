namespace GFSGUI
{
    partial class InputForm
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
            textBox1 = new TextBox();
            writeBtn = new Button();
            label1 = new Label();
            closeBtn = new Button();
            label2 = new Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Location = new Point(100, 50);
            textBox1.MaxLength = 100;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(301, 27);
            textBox1.TabIndex = 0;
            // 
            // writeBtn
            // 
            writeBtn.BackColor = SystemColors.ActiveCaption;
            writeBtn.Font = new Font("Segoe UI", 12F);
            writeBtn.Location = new Point(100, 116);
            writeBtn.Name = "writeBtn";
            writeBtn.Size = new Size(110, 42);
            writeBtn.TabIndex = 1;
            writeBtn.Text = "Create";
            writeBtn.UseVisualStyleBackColor = false;
            writeBtn.Click += writeBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(100, 9);
            label1.Name = "label1";
            label1.Size = new Size(78, 28);
            label1.TabIndex = 2;
            label1.Text = "My text";
            // 
            // closeBtn
            // 
            closeBtn.BackColor = Color.IndianRed;
            closeBtn.Font = new Font("Segoe UI", 12F);
            closeBtn.Location = new Point(291, 116);
            closeBtn.Name = "closeBtn";
            closeBtn.Size = new Size(110, 42);
            closeBtn.TabIndex = 3;
            closeBtn.Text = "Close";
            closeBtn.UseVisualStyleBackColor = false;
            closeBtn.Click += closeBtn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F);
            label2.ForeColor = Color.DarkRed;
            label2.Location = new Point(100, 87);
            label2.Name = "label2";
            label2.Size = new Size(58, 20);
            label2.TabIndex = 4;
            label2.Text = "My text";
            label2.Visible = false;
            // 
            // InputForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(510, 170);
            Controls.Add(label2);
            Controls.Add(closeBtn);
            Controls.Add(label1);
            Controls.Add(writeBtn);
            Controls.Add(textBox1);
            Name = "InputForm";
            Text = "InputForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button writeBtn;
        private Label label1;
        private Button closeBtn;
        private Label label2;
    }
}