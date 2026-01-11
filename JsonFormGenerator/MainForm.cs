using System;
using System.Data;
namespace JsonFormGenerator;
using FieldBlockLF = FieldBlockTemplate<List<LabeledField>, LabeledField>;
public partial class MainForm : Form {
    SurveyForm editorSurveyForm;
    FieldBlockLF survey;

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
    private Field CreateEditorSurvey() {
        FieldBlock survey = new([
            new LabeledField("fields", new FieldArray<FieldUnion>((i) => CreateField()))    
        ]);
        return survey;
    }
    private FieldUnion CreateField() {
        FieldUnion union = new([
            new("text", null),
            new("number", null),
            new("check", null),
            new("selection", () => new FieldArray<FieldText>((i) => new())),
            new("array", null),
            new("block", null),
            new("union", null),
        ]);
        return union;
    }
    private void BindArray(List<LabeledField> fields, int i, bool add) {
        if (add) {
            fields.Insert(i, new("", new FieldBlockLF([])));
        }
        else {
            if (surveyForm != null) fields[i].Destroy(surveyForm);
            fields.RemoveAt(i);
        }
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindLabel(LabeledField field, string value) {
        field.Label.Text = value;
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindField(LabeledField field, string? to) {
        if (surveyForm != null) field.Field?.Destroy(surveyForm);
        switch (to) {
            case "text":
                field.Field = new FieldText();
                break;
            case "number":
                field.Field = new FieldNumber();
                break;
            case "check":
                field.Field = new FieldCheck();
                break;
            case "array":
                break;
            case "block":
                field.Field = new FieldBlockLF([]);
                break;
        }
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindArrayField(int[] index, int i, string? to) {

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
