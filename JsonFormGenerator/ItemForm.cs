using System;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;


namespace JsonFormGenerator;
public partial class ItemForm : Form {
    FieldBlock form;
    List<Row> controlsOrder = new List<Row> { new() };
    class Row {
        public int tabCount;
        public List<Control> Controls = new();
    }

    public ItemForm() {
        InitializeComponent();
        form = new FieldBlock([
            new LabeledField<FieldArray<FieldBlock>>("Fields", new (new(GetField))),
            new LabeledField<FieldArray<FieldBlock>>("Objects", new (new(GetField))),
        ]);
        form.Create(this, new());
    }
    private FieldBlock GetField() {
        return new FieldBlock([
            new LabeledField<FieldText>("Name", new()),
            new LabeledField<FieldUnion>("Type", new([
                new ("Text", new([new LabeledField<FieldText>("default", new())])),
                new ("Number", new([new LabeledField<FieldNumber>("default", new())])),
                new ("Check", new([new LabeledField<FieldCheck>("default", new())])),
                new ("Array", new([new LabeledField<FieldFactory<FieldBlock>>("Field", new(GetField))])),
                new ("Block", new([new LabeledField<FieldArray<FieldBlock>>("fields", new(new(GetField)))])),
            ], this))
        ]);
    }
    public void Export() { //chatgeepeetee 🛌
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
                        form.WriteJson(writter);
                        writter.Flush();
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    public void Recreate() {
        Controls.Clear();
        controlsOrder.Clear();
        controlsOrder = new List<Row>{ new() }; 
        form.Create(this, new());
    }
    public void Recalculate() {
        Cursor c = new();
        List<Row> controlsOrder2 = new();
        controlsOrder2.AddRange(controlsOrder);
        controlsOrder = new List<Row> { new() };
        foreach (var row in controlsOrder2) {
            if (row.Controls.Count == 0) continue;
            if (row.Tab) Tab(c);
            else if (row.UnTab) UnTab(c);
            foreach (var control in row.Controls) {
                Add(control, c);
            }
            NextLine(c);
        }
    }
    public void Add(Control control, Cursor cursor) {
        control.Location = cursor.Location;
        Controls.Add(control);
        controlsOrder[cursor.Index.Y].Controls.Insert(cursor.Index.X, control);

        int move = cursor.Move(control);
        PushX(cursor.Index, move);
    }
    public void NextLine(Cursor cursor) {
        int move = cursor.NextLine();
        controlsOrder.Insert(cursor.Index.Y, new());
        PushY(cursor.Index, move);
    }
    public void Tab(Cursor c) {
        controlsOrder[c.Index.Y].tabCount += 1;
        c.Tab();
    }
    public void UnTab(Cursor c) {
        controlsOrder[c.Index.Y].tabCount -= 1;
        c.UnTab();
    }
    void PushX(Point index, int move) {
        for (int x = index.X; x < controlsOrder[index.Y].Controls.Count; x++) {
            Move(controlsOrder[index.Y].Controls[x], new(move, 0));
        }
    }
    void PushY(Point index, int move) {
        for (int y = index.Y+1; y < controlsOrder.Count; y++) {
            for (int x = 0; x < controlsOrder[y].Controls.Count; x++) {
                Move(controlsOrder[y].Controls[x], new(0, move));
            }
        }
    }
}
public class Cursor {
    public Point Index;
    public Point Location;
    const int tabSize = 32;
    internal int tab;
    internal int nextLineY;
    public Cursor Clone() {
        return (Cursor)this.MemberwiseClone();
    }
    public int Move(Control control) {
        Location.X += control.Size.Width;
        Index.X += 1;
        if (control.Size.Height > nextLineY) {
            nextLineY = control.Size.Height;
        }
        return control.Size.Width;
    }
    public int NextLine() {
        var nxt = nextLineY; 
        Location.X = tab * tabSize;
        Location.Y += nextLineY;
        nextLineY = 0;

        Index.X = 0;
        Index.Y += 1;
        return nxt;
    }
    public void Tab() => tab++;
    public void UnTab() => tab--;
}