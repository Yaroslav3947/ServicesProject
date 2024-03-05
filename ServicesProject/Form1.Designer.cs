namespace ServicesProject {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.connectBnt = new System.Windows.Forms.Button();
            this.successlbl = new System.Windows.Forms.Label();
            this.getClientBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectBnt
            // 
            this.connectBnt.Location = new System.Drawing.Point(281, 123);
            this.connectBnt.Name = "connectBnt";
            this.connectBnt.Size = new System.Drawing.Size(215, 68);
            this.connectBnt.TabIndex = 0;
            this.connectBnt.Text = "Connect";
            this.connectBnt.UseVisualStyleBackColor = true;
            this.connectBnt.Click += new System.EventHandler(this.connectBnt_Click);
            // 
            // successlbl
            // 
            this.successlbl.AutoSize = true;
            this.successlbl.Location = new System.Drawing.Point(370, 222);
            this.successlbl.Name = "successlbl";
            this.successlbl.Size = new System.Drawing.Size(0, 13);
            this.successlbl.TabIndex = 1;
            // 
            // getClientBtn
            // 
            this.getClientBtn.Location = new System.Drawing.Point(69, 123);
            this.getClientBtn.Name = "getClientBtn";
            this.getClientBtn.Size = new System.Drawing.Size(127, 33);
            this.getClientBtn.TabIndex = 2;
            this.getClientBtn.Text = "Get clients";
            this.getClientBtn.UseVisualStyleBackColor = true;
            this.getClientBtn.Click += new System.EventHandler(this.getClientBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.getClientBtn);
            this.Controls.Add(this.successlbl);
            this.Controls.Add(this.connectBnt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectBnt;
        private System.Windows.Forms.Label successlbl;
        private System.Windows.Forms.Button getClientBtn;
    }
}

