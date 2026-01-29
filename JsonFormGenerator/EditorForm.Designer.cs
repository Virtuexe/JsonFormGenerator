namespace JsonFormGenerator {
    partial class EditorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            panel = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            create = new Button();
            exportBtn = new Button();
            import = new Button();
            panel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel
            // 
            panel.AutoScroll = true;
            panel.ColumnCount = 1;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            panel.Controls.Add(tableLayoutPanel2, 0, 1);
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Name = "panel";
            panel.RowCount = 2;
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 75F));
            panel.Size = new Size(800, 450);
            panel.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.BackColor = SystemColors.GradientActiveCaption;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            tableLayoutPanel2.Controls.Add(import, 1, 0);
            tableLayoutPanel2.Controls.Add(create, 1, 0);
            tableLayoutPanel2.Controls.Add(exportBtn, 3, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 378);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(794, 69);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // create
            // 
            create.Dock = DockStyle.Fill;
            create.Location = new Point(377, 3);
            create.Name = "create";
            create.Size = new Size(134, 63);
            create.TabIndex = 0;
            create.Text = "Create";
            create.UseVisualStyleBackColor = true;
            create.Click += CreateBtn;
            // 
            // exportBtn
            // 
            exportBtn.Dock = DockStyle.Fill;
            exportBtn.Location = new Point(657, 3);
            exportBtn.Name = "exportBtn";
            exportBtn.Size = new Size(134, 63);
            exportBtn.TabIndex = 1;
            exportBtn.Text = "Export";
            exportBtn.UseVisualStyleBackColor = true;
            exportBtn.Click += ExportBtn;
            // 
            // import
            // 
            import.Dock = DockStyle.Fill;
            import.Location = new Point(517, 3);
            import.Name = "import";
            import.Size = new Size(134, 63);
            import.TabIndex = 2;
            import.Text = "Import";
            import.UseVisualStyleBackColor = true;
            import.Click += ImportBtn;
            // 
            // EditorForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel);
            Name = "EditorForm";
            Text = "MainForm";
            panel.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel panel;
        private TableLayoutPanel tableLayoutPanel2;
        private Button create;
        private Button exportBtn;
        private Button import;
    }
}