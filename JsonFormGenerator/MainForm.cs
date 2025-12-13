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
        Func<int[], int, FieldUnionConstruct[]> typeRaw = null!;
        Func<int[], int, LabeledField> type = null!;
        Func<int[], int, FieldBlockLF> field = null!;
        Func<int[], FieldArray<FieldBlockLF>> fields = null!;
        typeRaw = (indexes, i) => [
            new("text", null),
            new("number", null),
            new("check", null),
            new("array", () => new LabeledField("type", new FieldUnion(typeRaw(indexes, i), (from, to) => BindFieldArray(indexes, i, to)))),
            new("block", () => fields([..indexes, i])),
        ];
        type = (indexes, i) => new LabeledField("type", new FieldUnion(typeRaw(indexes, i), (from, to) => BindField(indexes, i, to)));
        field = (indexes, i) => new([
           new LabeledField("name", new FieldText((value) => BindLabel(indexes, i, value))),
           type(indexes, i),
        ]);
        fields = (indexes) => new FieldArray<FieldBlockLF>((i) => field(indexes, i), (i, add) => BindArray(indexes, i, add));
        FieldBlock survey = new([
            new LabeledField("fields", fields([]))
        ]);
        return survey;
    }
    private List<LabeledField> TraverseSurvey(int[] indexes) {
        var fields = survey.Fields;
        foreach (var i in indexes) {
            fields = ((FieldBlockLF)fields[i].Field!).Fields;
        }
        return fields;
    }
    private void BindArray(int[] indexes, int i, bool add) {
        var fields = TraverseSurvey(indexes);
        if (add) {
            fields.Insert(i, new("", new FieldBlockLF([])));
        }
        else {
            if (surveyForm != null) fields[i].Destroy(surveyForm);
            fields.RemoveAt(i);
        }
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindLabel(int[] indexes, int i, string value) {
        var fields = TraverseSurvey(indexes);
        fields[i].Label.Text = value;
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindField(int[] indexes, int i, string? to) {
        var fields = TraverseSurvey(indexes);
        if (surveyForm != null) fields[i].Field?.Destroy(surveyForm);
        switch (to) {
            case "text":
                fields[i].Field = new FieldText();
                break;
            case "number":
                fields[i].Field = new FieldNumber();
                break;
            case "check":
                fields[i].Field = new FieldCheck();
                break;
            case "array":
                break;
            case "block":
                fields[i].Field = new FieldBlockLF([]);
                break;
        }
        if (surveyForm != null) survey.Create(surveyForm, new());
    }
    private void BindFieldArray(int[] indexes, int i, string? to) {
        var fields = TraverseSurvey(indexes);
        if(surveyForm != null) fields[i].Field?.Destroy(surveyForm);
        switch(to) {
            case "text":
                fields[i].Field = new FieldArray<FieldText>((i) => new());
                break;
            case "number":
                fields[i].Field = new FieldArray<FieldNumber>((i) => new());
                break;
            case "check":
                fields[i].Field = new FieldArray<FieldCheck>((i) => new());
                break;
            case "array":
                break;
            case "block":
                break;
        }
        if(surveyForm != null) survey.Create(surveyForm, new());
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
