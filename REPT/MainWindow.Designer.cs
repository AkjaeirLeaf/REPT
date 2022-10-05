
namespace REPT
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.DisplayRenderMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayRenderMain)).BeginInit();
            this.SuspendLayout();
            // 
            // DisplayRenderMain
            // 
            this.DisplayRenderMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DisplayRenderMain.Location = new System.Drawing.Point(0, 0);
            this.DisplayRenderMain.Name = "DisplayRenderMain";
            this.DisplayRenderMain.Size = new System.Drawing.Size(800, 450);
            this.DisplayRenderMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DisplayRenderMain.TabIndex = 0;
            this.DisplayRenderMain.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DisplayRenderMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Rift Engine Planet Tools";
            ((System.ComponentModel.ISupportInitialize)(this.DisplayRenderMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox DisplayRenderMain;
    }
}

