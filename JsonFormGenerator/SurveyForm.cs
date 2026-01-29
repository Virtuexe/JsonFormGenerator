using System;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace JsonFormGenerator;
public partial class SurveyForm : Form {
    public Field Survey;

    public SurveyForm(Field survey) {
        InitializeComponent();
        Survey = survey;
        survey.Create(this, new());
    }
    public void Export() {
        using (var dlg = new SaveFileDialog()) {
            dlg.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            dlg.DefaultExt = "json";
            dlg.AddExtension = true;
            dlg.FileName = "output.json";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (dlg.ShowDialog(this) == DialogResult.OK) {
                try {
                    using (var fs = File.Create(dlg.FileName))
                    using (var writter = new Utf8JsonWriter(fs, new JsonWriterOptions { Indented = true })) {
                        Survey.WriteJson(writter);
                        writter.Flush();
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    public void Import() {
        using (var dlg = new OpenFileDialog()) {
            dlg.Filter = "JSON files (*.json)|*.json";
            if (dlg.ShowDialog(this) == DialogResult.OK) {

                byte[] jsonBytes = File.ReadAllBytes(dlg.FileName);

                var reader = new Utf8JsonReader(jsonBytes);

                reader.Read();

                try {
                    Survey.ReadJson(ref reader);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Import failed (incompatible file): {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Controls.Clear();
                Survey.Create(this, new());
            }
        }
    }
    public void Dispose(Control c) {
        Controls.Remove(c);
        c.Dispose();
    }
}