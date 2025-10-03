using System.Diagnostics;

namespace JsonFormGenerator;

public abstract class Field {
    public Label Label;
    public Field(string name) {
        Label = new Label();
        Label.Text = name;
        Label.AutoSize = true;
        Label.TextAlign = ContentAlignment.MiddleLeft;
    }
    public void Create(Form1 form) {
        form.Add(Label);
        CreateSub(form);
    }
    protected abstract void CreateSub(Form1 form);
}
public class FieldText : Field {
    public TextBox TextBox;
    public FieldText(string name, string value = "") : base(name) {
        TextBox = new TextBox();
        TextBox.Text = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(TextBox);
        form.NextLine();
    }
}
public class FieldCheck : Field {
    public CheckBox CheckBox;
    public FieldCheck(string name, bool value = false) : base(name) {
        CheckBox = new CheckBox();
        CheckBox.Checked = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(CheckBox);
        form.NextLine();
    }
}
public class FieldNumber : Field {
    public NumericUpDown NumericUpDown;
    public FieldNumber(string name, int value = 0) : base(name) {
        NumericUpDown = new NumericUpDown();
        NumericUpDown.Value = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(NumericUpDown);
        form.NextLine();
    }
}
public class FieldObject : Field {
    public Field[] Fields;
    public FieldObject(string name, Field[] fields) : base(name) {
        Fields = fields;
    }
    protected override void CreateSub(Form1 form) {
        form.Tab();
        form.NextLine();
        foreach (var field in Fields) {
            field.Create(form);
        }
        form.RemTab();
        form.NextLine();
    }
}
