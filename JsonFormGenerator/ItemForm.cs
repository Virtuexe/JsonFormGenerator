using System.Diagnostics;

namespace JsonFormGenerator;
public partial class ItemForm : Form {
    Field[] _fields;

    const int tabSize = 32;
    int tab;
    Point cursor;
    int nextLineY;

    public ItemForm() {
        InitializeComponent();
        _fields = [
            new LabeledField<FieldObject>("user", new([
                new LabeledField<FieldText>("name", new()),
                new LabeledField<FieldText>("surname", new()),
                new LabeledField<FieldCheck>("is_over_18", new()),
                new LabeledField<FieldObject>("parent", new([
                    new LabeledField<FieldText>("name", new()),
                    new LabeledField<FieldText>("surname", new()),
                    new LabeledField<FieldCheck>("is_over_18", new())
                ])),
                new LabeledField<FieldArray<FieldSelection>>("items-to-buy", new(() => new(["banana", "apple", "strawberry"]), 5))
            ])),
            new LabeledField<FieldCheck>("did-read-rights", new()),
        ];
        Field.Create(this, _fields);
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
                    Field.CreateJson(dlg.FileName, _fields);
                }
                catch (Exception ex) {
                    MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public void Tab() {
        tab++;
    }
    public void RemTab() {
        tab--;
    }
    public void Add(Control control) {
        Controls.Add(control);
        control.Location = cursor;
        cursor.X += control.Size.Width;
        if (control.Size.Height > nextLineY) {
            nextLineY = control.Size.Height;
        }
    }
    public void NextLine() {
        cursor.X = tab * tabSize;
        cursor.Y += nextLineY;
        nextLineY = 0;
    }
}
