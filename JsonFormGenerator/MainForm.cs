using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonFormGenerator {
    public partial class MainForm : Form {
        ItemForm itemForm;
        public MainForm() {
            InitializeComponent();
            itemForm = new();
            itemForm.TopLevel = false;
            itemForm.Dock = DockStyle.Fill;
            itemForm.FormBorderStyle = FormBorderStyle.None;
            itemForm.Show();
            panel.Controls.Add(itemForm, 0, 0);
        }

        private void ExportBtn(object sender, EventArgs e) {
            itemForm.Export();
        }
    }
}
