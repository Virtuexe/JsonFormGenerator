using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsonFormGenerator {
    public partial class MainForm : Form {
        SurveyForm editorSurveyForm;

        List<LabeledField> fields { 
            get { return survey.Fields.Select((f) => (LabeledField)f).ToList();}
            set { survey.Fields = value.Select(f => (Field)f).ToArray(); }
        }
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

            survey = new([]);
        }
        private FieldBlock CreateEditorSurvey() {
            Func<int, FieldUnion> getType;
            Func<int, FieldBlock> getField = null!;
            getType = (i) => new FieldUnion(
                [
                    new ("Text", () => null /*new LabeledField("default", new FieldText())*/),
                    new ("Number", () => null /*new LabeledField("default", new FieldNumber())*/),
                    new ("Check", () => null /*new LabeledField("default", new FieldCheck())*/),
                    new ("Array", () => new LabeledField("Field", getField(-1))),
                    new ("Block", () => new LabeledField("fields", new FieldArray<FieldBlock>(new(getField))))
                ], (_, to) => {
                    if (i == -1) return;
                    switch (to) {
                        case "Text":
                            fields[i].Field = new FieldText();
                            break;
                        case "Number":
                            fields[i].Field = new FieldNumber();
                            break;
                        case "Check":
                            fields[i].Field = new FieldCheck();
                            break;
                        //case "Array":
                        //    fields[i].Field = new FieldArray();
                        //    break;
                    }
                    if (surveyForm != null) survey.Create(surveyForm, new());
                });
            getField = (i) => new FieldBlock(new[] {
                new LabeledField("Name", new FieldText((value) => {fields[i].Label.Text = value; if (surveyForm != null) survey.Create(surveyForm, new()); })),
                new LabeledField("Type", getType(i))
            });
            var form = new FieldBlock([
                new LabeledField("Fields", new FieldArray<FieldBlock>(new(getField), (i, add) => {
                    var fields = this.fields;
                    if (add) fields.Insert(i, new("", null));
                    else fields.RemoveAt(i);
                    this.fields = fields;
                    if (surveyForm != null) survey.Create(surveyForm, new());
                })),
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
