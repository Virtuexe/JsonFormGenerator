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
    public void Dispose(Control c) {
        Controls.Remove(c);
        c.Dispose();
    }
}
public class Cursor {
    private Point pos;
    private List<int> nextLines = new List<int> { new() };
    public Cursor Clone() => (Cursor)this.MemberwiseClone();
    public void Add(Control c, SurveyForm form) {
        form.Controls.Add(c);
        c.Location = new(pos.X, pos.Y + form.AutoScrollPosition.Y);
        pos.X += c.Width;
        if (nextLines[^1] < c.Height)
            nextLines[^1] = c.Height;
    }
    private int tabCount;
    public void NextLine() {
        pos.X = tabCount * 32;
        pos.Y += nextLines[^1];
        nextLines.Add(0);
    }
    public void Tab() => tabCount++;
    public void UnTab() => tabCount--;
    //public void Set(Control c) {
    //    pos = c.Location;
    //}
}