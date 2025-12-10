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
        SurveyForm editorSurvey;
        SurveyForm? survey;
        public MainForm() {
            InitializeComponent();

            editorSurvey = new(CreateEditorSurvey());

            editorSurvey.TopLevel = false;
            editorSurvey.Dock = DockStyle.Fill;
            editorSurvey.FormBorderStyle = FormBorderStyle.None;
            editorSurvey.Show();
            panel.Controls.Add(editorSurvey, 0, 0);
        }
        private FieldBlock CreateEditorSurvey() {
            Func<FieldBlock> getField = null!;
            Action<string> text = (val) => {

            };
            getField = () => new FieldBlock([
                new LabeledField("Name", new FieldText()),
                new LabeledField("Type", new FieldUnion([
                    new ("Text", () => new([new LabeledField("default", new FieldText())])),
                    new ("Number", () => new([new LabeledField("default", new FieldNumber())])),
                    new ("Check", () => new([new LabeledField("default", new FieldCheck())])),
                    new ("Array", () => new([new LabeledField("Field", getField())])),
                    new ("Block", () => new([new LabeledField("fields", new FieldArray<FieldBlock>(new(getField)))])),
                ]))
            ]);
            var form = new FieldBlock([
                new LabeledField("Fields", new FieldArray<FieldBlock>(new(getField))),
            ]);
            return form;
        }
        private void ExportBtn(object sender, EventArgs e) {
            editorSurvey.Export();
        }

        private void CreateBtn(object sender, EventArgs e) {
            create.Enabled = false;

            survey = new(new([]));

            survey.Show();
            survey.FormClosed += (_, _) => create.Enabled = true;
        }
    }
}
