using System;
using System.Data;
namespace JsonFormGenerator;
public partial class MainForm : Form {
    SurveyForm editorSurveyForm;
    FieldBlock editorSurvey;
    FieldBlock survey;

    SurveyForm? surveyForm;
    public MainForm() {
        InitializeComponent();
        editorSurveyForm = new(CreateEditorSurvey());
        editorSurvey = (FieldBlock)editorSurveyForm.Survey;

        editorSurveyForm.TopLevel = false;
        editorSurveyForm.Dock = DockStyle.Fill;
        editorSurveyForm.FormBorderStyle = FormBorderStyle.None;
        editorSurveyForm.Show();
        panel.Controls.Add(editorSurveyForm, 0, 0);

        survey = new([]);
    }
    private Field CreateEditorSurvey() {
        FieldBlock survey = new([]);
        survey.Fields = [
            new LabeledField("fields", new FieldArray<FieldBlock>(
                (i) => CreateField(i), 
                (i, add) => BindArray(this.survey.Fields, i, add))
            )    
        ];
        return survey;
    }
    private void BindArray(List<Field?> fields, int i, bool add) {
        if (add) {
            fields.Insert(i, new LabeledField("", null));
        }
        else {
            if(surveyForm != null) fields[i]?.Destroy(surveyForm);
            fields.RemoveAt(i);
        }
        if(surveyForm != null) survey.Create(surveyForm, new());
    }
    private FieldBlock CreateField(int i) {
        FieldUnion union = new([
            new("text", null),
            new("number", null),
            new("check", null),
            new("selection", () => new FieldArray<FieldText>((i) => new())),
            new("array", null),
            new("block", null),
            new("union", null),
        ], (_, to) => BindField(i, to));
        FieldBlock field = new([
            new LabeledField("name", new FieldText((value) => ((LabeledField)survey.Fields[i]!).Label.Text = value)),
            new LabeledField("type", union)
        ]);
        return field;
    }
    private void BindField(int i, string? to) {
        ref var field = ref ((LabeledField)survey.Fields[i]!).Field;
        if (surveyForm != null) field?.Destroy(surveyForm);
        switch (to) {
            case "text":
                field = new FieldText();
                break;
            case "number":
                field = new FieldNumber();
                break;
            case "check":
                field = new FieldCheck();
                break;
            case "selection":
                field = new FieldSelection([]);
                break;
            case "array":
                break;
            case "block":
                field = new FieldBlock([]);
                break;
        }
        if (surveyForm != null) survey.Create(surveyForm, new());
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
