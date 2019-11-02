namespace SIAMTest
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
            this.GetFTPBtn = new System.Windows.Forms.Button();
            this.FtpURLText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FilesLoadedLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // GetFTPBtn
            // 
            this.GetFTPBtn.Location = new System.Drawing.Point(769, 30);
            this.GetFTPBtn.Name = "GetFTPBtn";
            this.GetFTPBtn.Size = new System.Drawing.Size(154, 23);
            this.GetFTPBtn.TabIndex = 0;
            this.GetFTPBtn.Text = "Загрузить";
            this.GetFTPBtn.UseVisualStyleBackColor = true;
            this.GetFTPBtn.Click += new System.EventHandler(this.LoadFileBtn_Click);
            // 
            // FtpURLText
            // 
            this.FtpURLText.Location = new System.Drawing.Point(12, 31);
            this.FtpURLText.Name = "FtpURLText";
            this.FtpURLText.Size = new System.Drawing.Size(751, 22);
            this.FtpURLText.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Файл:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Загружено файлов:";
            // 
            // FilesLoadedLabel
            // 
            this.FilesLoadedLabel.AutoSize = true;
            this.FilesLoadedLabel.Location = new System.Drawing.Point(159, 132);
            this.FilesLoadedLabel.Name = "FilesLoadedLabel";
            this.FilesLoadedLabel.Size = new System.Drawing.Size(0, 17);
            this.FilesLoadedLabel.TabIndex = 7;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 171);
            this.Controls.Add(this.FilesLoadedLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FtpURLText);
            this.Controls.Add(this.GetFTPBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Тест СИАМ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetFTPBtn;
        private System.Windows.Forms.TextBox FtpURLText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label FilesLoadedLabel;
    }
}

