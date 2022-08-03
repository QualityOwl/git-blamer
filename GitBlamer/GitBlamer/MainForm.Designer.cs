namespace GitBlamer
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbException = new System.Windows.Forms.TextBox();
            this.lblException = new System.Windows.Forms.Label();
            this.lblLocalRepository = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblBranch = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStripMain.Size = new System.Drawing.Size(1225, 28);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(67, 24);
            this.configToolStripMenuItem.Text = "Config";
            this.configToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 28);
            this.splitContainerMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.btnClear);
            this.splitContainerMain.Panel1.Controls.Add(this.comboBox1);
            this.splitContainerMain.Panel1.Controls.Add(this.lblBranch);
            this.splitContainerMain.Panel1.Controls.Add(this.textBox1);
            this.splitContainerMain.Panel1.Controls.Add(this.lblLocalRepository);
            this.splitContainerMain.Panel1.Controls.Add(this.btnSubmit);
            this.splitContainerMain.Panel1.Controls.Add(this.tbException);
            this.splitContainerMain.Panel1.Controls.Add(this.lblException);
            this.splitContainerMain.Size = new System.Drawing.Size(1225, 807);
            this.splitContainerMain.SplitterDistance = 612;
            this.splitContainerMain.TabIndex = 1;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(245, 755);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(89, 32);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            // 
            // tbException
            // 
            this.tbException.Location = new System.Drawing.Point(14, 95);
            this.tbException.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbException.Multiline = true;
            this.tbException.Name = "tbException";
            this.tbException.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbException.Size = new System.Drawing.Size(585, 641);
            this.tbException.TabIndex = 2;
            // 
            // lblException
            // 
            this.lblException.AutoSize = true;
            this.lblException.Location = new System.Drawing.Point(15, 77);
            this.lblException.Name = "lblException";
            this.lblException.Size = new System.Drawing.Size(69, 16);
            this.lblException.TabIndex = 0;
            this.lblException.Text = "Exception:";
            // 
            // lblLocalRepository
            // 
            this.lblLocalRepository.AutoSize = true;
            this.lblLocalRepository.Location = new System.Drawing.Point(15, 15);
            this.lblLocalRepository.Name = "lblLocalRepository";
            this.lblLocalRepository.Size = new System.Drawing.Size(112, 16);
            this.lblLocalRepository.TabIndex = 10;
            this.lblLocalRepository.Text = "Local Repository:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(133, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(316, 22);
            this.textBox1.TabIndex = 1;
            // 
            // lblBranch
            // 
            this.lblBranch.AutoSize = true;
            this.lblBranch.Location = new System.Drawing.Point(15, 46);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(52, 16);
            this.lblBranch.TabIndex = 11;
            this.lblBranch.Text = "Branch:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(133, 43);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(316, 24);
            this.comboBox1.TabIndex = 12;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(366, 755);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(89, 32);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1225, 835);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "GitBlamer";
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbException;
        private System.Windows.Forms.Label lblException;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblLocalRepository;
        private System.Windows.Forms.Button btnClear;
    }
}

