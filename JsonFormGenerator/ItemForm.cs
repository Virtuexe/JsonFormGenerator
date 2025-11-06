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
        //Theme.MainTheme.ApplyTheme(this);
        _fields = [
            new LabeledField("Object", new FieldUnion([
                new LabeledField("User", new FieldObject([
                    new LabeledField("Name", new FieldText()),
                    new LabeledField("Surname", new FieldText()),
                    new LabeledField("Age", new FieldNumber()),
                ])),
                new LabeledField("User2", new FieldObject([
                    new LabeledField("Name", new FieldText()),
                    new LabeledField("Surname", new FieldText()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                ]))
            ], this)),
            new LabeledField("over-18", new FieldCheck()),
            new LabeledField("Object", new FieldUnion([
                new LabeledField("User", new FieldObject([
                    new LabeledField("Name", new FieldText()),
                    new LabeledField("Surname", new FieldText()),
                    new LabeledField("Age", new FieldNumber()),
                ])),
                new LabeledField("User2", new FieldObject([
                    new LabeledField("Name", new FieldText()),
                    new LabeledField("Surname", new FieldText()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                    new LabeledField("Age", new FieldNumber()),
                ]))
            ], this)),
            new LabeledField("over-18", new FieldCheck()),
        ];
        //Field.ApplyTheme(_fields, Theme.MainTheme);
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
    public void Recreate() {
        var scrollValue = VerticalScroll.Value;
        Controls.Clear();
        tab = 0;
        cursor = new();
        nextLineY = 0;
        Field.Create(this, _fields);
        VerticalScroll.Value = scrollValue;
        VerticalScroll.Value += 1; //Scrollbar buged needs reload
    }
    public void NextLine() {
        cursor.X = tab * tabSize;
        cursor.Y += nextLineY;
        nextLineY = 0;
    }
}
