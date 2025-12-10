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
        SurveyForm editorSurveyForm;

        LabeledField field;
        FieldBlock survey;

        SurveyForm? surveyForm;
        public MainForm() {
            InitializeComponent();

            editorSurveyForm = new(CreateEditorSurvey());

            editorSurveyForm.TopLevel = false;
            editorSurveyForm.Dock = DockStyle.Fill;
            editorSurveyForm.FormBorderStyle = FormBorderStyle.None;
            editorSurveyForm.Show();
            panel.Controls.Add(editorSurveyForm, 0, 0);

            field = new("", new FieldText());
            survey = new([field]);
        }
        private FieldBlock CreateEditorSurvey() {
            Func<FieldBlock> getField = null!;
            Action<string> text = (val) => {

            };
            getField = () => new FieldBlock([
                new LabeledField("Name", new FieldText((value) => field.Label.Text = value)),
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
            editorSurveyForm.Export();
        }

        private void CreateBtn(object sender, EventArgs e) {
            create.Enabled = false;

            surveyForm = new(survey);

            surveyForm.Show();
            surveyForm.FormClosed += (_, _) => create.Enabled = true;
        }
    }
}
