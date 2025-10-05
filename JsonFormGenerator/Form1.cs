using System.Diagnostics;

namespace JsonFormGenerator;
public partial class Form1 : Form {
    const int tabSize = 32;
    int tab;
    Point cursor;
    int nextLineY;

    public Form1() {
        InitializeComponent();
        Field[] fields = {
            new LabeledField<FieldObject>("user", new(new Field[]{
                new LabeledField<FieldText>("name", new()),
                new LabeledField<FieldText>("surname", new()),
                new LabeledField<FieldCheck>("is_over_18", new()),
                new LabeledField<FieldObject>("parent", new(new Field[]{
                    new LabeledField<FieldText>("name", new()),
                    new LabeledField<FieldText>("surname", new()),
                    new LabeledField<FieldCheck>("is_over_18", new())
                })),
                new LabeledField<FieldArray<FieldText>>("items-to-buy", new(() => new("empty"), 5))
            })),
            new LabeledField<FieldCheck>("did-read-rights", new())
        };
        Field.Create(this, fields);
        Field.CreateJson("./output.json", fields);
    }
    public void Tab() {
        tab++;
    }
    public void RemTab() {
        tab--;
    }
    public void Add(Control control) {
        Controls.Add(control);
        Debug.WriteLine("cursor: " + cursor);
        control.Location = cursor;
        cursor.X += control.Size.Width;
        if (control.Size.Height > nextLineY) {
            nextLineY = control.Size.Height;
            Debug.WriteLine("new Y: " + nextLineY);
        }
    }
    public void NextLine() {
        cursor.X = tab * tabSize;
        cursor.Y += nextLineY;
        nextLineY = 0;
    }
}
