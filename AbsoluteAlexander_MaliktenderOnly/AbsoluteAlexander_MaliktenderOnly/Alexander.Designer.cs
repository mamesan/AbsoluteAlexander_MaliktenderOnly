namespace AbsoluteAlexander_MaliktenderOnly
{
    partial class Alexander
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.管理 = new System.Windows.Forms.TabPage();
            this.textBox_list = new System.Windows.Forms.TextBox();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.checkedListBox_init = new System.Windows.Forms.CheckedListBox();
            this.textBoxlocalPath_init = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.管理.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.管理);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(962, 671);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(954, 645);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // 管理
            // 
            this.管理.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.管理.Controls.Add(this.textBox_list);
            this.管理.Controls.Add(this.button_delete);
            this.管理.Controls.Add(this.button_add);
            this.管理.Controls.Add(this.checkedListBox_init);
            this.管理.Controls.Add(this.textBoxlocalPath_init);
            this.管理.Controls.Add(this.label2);
            this.管理.Location = new System.Drawing.Point(4, 22);
            this.管理.Name = "管理";
            this.管理.Padding = new System.Windows.Forms.Padding(3);
            this.管理.Size = new System.Drawing.Size(954, 645);
            this.管理.TabIndex = 1;
            this.管理.Text = "管理";
            // 
            // textBox_list
            // 
            this.textBox_list.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_list.Location = new System.Drawing.Point(33, 90);
            this.textBox_list.Name = "textBox_list";
            this.textBox_list.Size = new System.Drawing.Size(227, 28);
            this.textBox_list.TabIndex = 13;
            // 
            // button_delete
            // 
            this.button_delete.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold);
            this.button_delete.Location = new System.Drawing.Point(357, 90);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(75, 28);
            this.button_delete.TabIndex = 15;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            // 
            // button_add
            // 
            this.button_add.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold);
            this.button_add.Location = new System.Drawing.Point(276, 90);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(75, 28);
            this.button_add.TabIndex = 14;
            this.button_add.Text = "add";
            this.button_add.UseVisualStyleBackColor = true;
            // 
            // checkedListBox_init
            // 
            this.checkedListBox_init.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkedListBox_init.FormattingEnabled = true;
            this.checkedListBox_init.Location = new System.Drawing.Point(33, 124);
            this.checkedListBox_init.Name = "checkedListBox_init";
            this.checkedListBox_init.Size = new System.Drawing.Size(399, 256);
            this.checkedListBox_init.TabIndex = 16;
            // 
            // textBoxlocalPath_init
            // 
            this.textBoxlocalPath_init.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxlocalPath_init.Location = new System.Drawing.Point(87, 39);
            this.textBoxlocalPath_init.Name = "textBoxlocalPath_init";
            this.textBoxlocalPath_init.Size = new System.Drawing.Size(345, 28);
            this.textBoxlocalPath_init.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(30, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Path:";
            // 
            // Alexander
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "Alexander";
            this.Size = new System.Drawing.Size(962, 671);
            this.tabControl1.ResumeLayout(false);
            this.管理.ResumeLayout(false);
            this.管理.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage 管理;
        private System.Windows.Forms.TextBox textBox_list;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.CheckedListBox checkedListBox_init;
        private System.Windows.Forms.TextBox textBoxlocalPath_init;
        private System.Windows.Forms.Label label2;
    }
}
