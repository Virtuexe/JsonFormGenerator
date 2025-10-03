using System.Diagnostics;

namespace JsonFormGenerator;
public partial class Form1 : Form {
    const int tabSize = 16;
    int tab;
    Point cursor;
    int nextLineY;

    public Form1() {
        InitializeComponent();
        Field[] fields = {
            new FieldCheck("idk", true),
            new FieldObject("obj", new Field[] {
                new FieldCheck("idk", true),
                new FieldNumber("idk", 69),
                new FieldText("name", "Hello"),
            }),
            new FieldText("name", "Hello"),
        };
        foreach (Field field in fields) {
            field.Create(this);
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
