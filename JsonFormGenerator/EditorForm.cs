using System;
using System.Windows.Forms;

namespace JsonFormGenerator;

public partial class EditorForm : Form {
    private readonly SurveyBuilder _builder;
    private readonly SurveyForm _mainSurveyForm;
    private EditorForm? _userForm;
    private bool _isEditor;

    public EditorForm(Field? mainSurvey = null) {
        InitializeComponent();

        _builder = new SurveyBuilder();

        // Initialize Editor UI
        if (mainSurvey == null) {
            _isEditor = true;
            _mainSurveyForm = new SurveyForm(_builder.CreateEditorSchema());
        }
        else {
            _isEditor = false;
            _mainSurveyForm = new SurveyForm(mainSurvey);
        }
        SetEditor(_isEditor);


        // UI Layout Configuration
        _mainSurveyForm.TopLevel = false;
        _mainSurveyForm.Dock = DockStyle.Fill;
        _mainSurveyForm.FormBorderStyle = FormBorderStyle.None;
        _mainSurveyForm.Show();
        panel.Controls.Add(_mainSurveyForm, 0, 0);
    }

    public void SetEditor(bool value) {
        _isEditor = value;
        if (_isEditor) {
            panel.SetColumn(import, 1);
            create.Visible = true;

            import.Visible = false;
            panel.SetColumn(create, 2);
        }
        else {
            panel.SetColumn(import, 2);
            create.Visible = false;

            import.Visible = true;
            panel.SetColumn(create, 1);
        }
    }

    private void ExportBtn(object sender, EventArgs e) {
        _mainSurveyForm.Export();
    }
    private void ImportBtn(object sender, EventArgs e) {
        _mainSurveyForm.Import();
    }

    private void CreateBtn(object sender, EventArgs e) {
        if (!_isEditor) return;

        create.Enabled = false;
        _userForm = new EditorForm(_builder.Survey);
        _builder.PreviewForm = _userForm._mainSurveyForm;

        _userForm.Show();
        _userForm.FormClosed += (_, _) => {
            create.Enabled = true;
            _builder.PreviewForm = null;
        };
    }
}