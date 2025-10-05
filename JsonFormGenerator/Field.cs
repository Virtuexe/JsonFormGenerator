using System.Diagnostics;

namespace JsonFormGenerator;

public abstract class Field {
    public void Create(Form1 form) {
        CreateSub(form);
    }
    public static void Create(Form1 form, Field[] fields) {
        foreach (Field field in fields) {
            field.Create(form);
        }
    }
    protected abstract void CreateSub(Form1 form);
    public static void CreateJson(string path, Field[] fields) {
        File.WriteAllText(path, WriteJsonBlock(fields));
    }
    public string WriteJson(int tabCount = 0, bool startOfLine = true) {
        string res = "";
        if (startOfLine) {
            for (int i = 0; i < tabCount; i++) {
                res += "    ";
            }
        }
        res += WriteJsonSub(tabCount);
        return res;
    }
    public static string WriteJsonBlock(Field[] fields, int tabCount = 0, bool arrayType = false) {
        string l = arrayType ? "[" : "{";
        string r = arrayType ? "]" : "}";

        string result = $"{l}\n";
        for (int i = 0; i < fields.Length; i++) {
            Field f = fields[i];
            result += f.WriteJson(tabCount + 1);
            if (i < fields.Length - 1)
                result += ",\n";
        }

        result += "\n";
        for (int i = 0; i < tabCount; i++) {
            result += "    ";
        }
        result += r;
        return result;
    }
    protected abstract string WriteJsonSub(int tabCount);
}
public class LabeledField<T> : Field where T : Field {
    public Label Label;
    public T Field;
    public LabeledField(string name, T field) {
        Label = new Label();
        Label.Text = name;
        Label.AutoSize = true;
        Field = field;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(Label);
        Field.Create(form);
    }
    protected override string WriteJsonSub(int tabCount) {
        return "\"" + Label.Text + "\"" + ": " + Field.WriteJson(tabCount, false);
    }
}
public class FieldText : Field {
    public TextBox TextBox;
    public FieldText(string value = "")  {
        TextBox = new TextBox();
        TextBox.TextChanged += (s, e) => {
            int width = TextRenderer.MeasureText(TextBox.Text, TextBox.Font).Width + 15;
            TextBox.Width = Math.Max(width, 50);
        };
        TextBox.Text = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(TextBox);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return "\"" + TextBox.Text + "\"";
    }
}
public class FieldCheck : Field {
    public CheckBox CheckBox;
    public FieldCheck(bool value = false)  {
        CheckBox = new CheckBox();
        CheckBox.Checked = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(CheckBox);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return CheckBox.Checked ? "true" : "false";
    }
}
public class FieldNumber : Field {
    public NumericUpDown NumericUpDown;
    public FieldNumber(int value = 0)  {
        NumericUpDown = new NumericUpDown();
        NumericUpDown.Value = value;
    }
    protected override void CreateSub(Form1 form) {
        form.Add(NumericUpDown);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return NumericUpDown.Value.ToString();
    }
}
public class FieldObject : Field {
    public Field[] Fields;
    public FieldObject(Field[] fields)  {
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
    protected override string WriteJsonSub(int tabCount) {
        return WriteJsonBlock(Fields, tabCount);
    }
}
public class FieldArray<T> : Field where T : Field {
    public T[] _fields;
    public FieldArray(Func<T> factory, int count)  {
        _fields = new T[count];
        for (int i = 0; i < count; i++) {
            _fields[i] = factory();
        }
    }
    protected override void CreateSub(Form1 form) {
        form.Tab();
        form.NextLine();
        foreach (var field in _fields) {
            field.Create(form);
        }
        form.RemTab();
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return WriteJsonBlock(_fields, tabCount, true);
    }
}
