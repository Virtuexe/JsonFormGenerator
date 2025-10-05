using System.Diagnostics;

namespace JsonFormGenerator;

public abstract class Field {
    public void Create(ItemForm form) {
        CreateSub(form);
    }
    public static void Create(ItemForm form, Field[] fields) {
        foreach (Field field in fields) {
            field.Create(form);
        }
    }
    protected abstract void CreateSub(ItemForm form);
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
    protected override void CreateSub(ItemForm form) {
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
        var func = (object? s, EventArgs e) => { };
        TextBox.TextChanged += Resize;
        TextBox.Text = value;
        Resize(null, new());
    }
    protected override void CreateSub(ItemForm form) {
        form.Add(TextBox);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return "\"" + TextBox.Text + "\"";
    }
    private void Resize(object? o, EventArgs e) {
        int width = TextRenderer.MeasureText(TextBox.Text, TextBox.Font).Width + 15;
        TextBox.Width = Math.Max(width, 100);
    }
}
public class FieldCheck : Field {
    public CheckBox CheckBox;
    public FieldCheck(bool value = false)  {
        CheckBox = new CheckBox();
        CheckBox.Checked = value;
        CheckBox.AutoSize = true;
        CheckBox.Text = " ";
    }
    protected override void CreateSub(ItemForm form) {
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
    protected override void CreateSub(ItemForm form) {
        form.Add(NumericUpDown);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return NumericUpDown.Value.ToString();
    }
}
public class FieldSelection : Field {
    public ComboBox ComboBox;
    public FieldSelection(string[] selections) {
        ComboBox = new ComboBox();
        ComboBox.Items.AddRange(selections);
        Resize(selections);
    }
    protected override void CreateSub(ItemForm form) {
        form.Add(ComboBox);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return ComboBox.SelectedText;
    }
    private void Resize(string[] selections) {
        int largestWidth = 0;
        foreach (var selection in selections) {
            int width = TextRenderer.MeasureText(selection, ComboBox.Font).Width;
            largestWidth = width > largestWidth ? width : largestWidth;
            Debug.WriteLine(largestWidth);
        }
        ComboBox.Width = largestWidth + 32;
    }
}
public class FieldObject : Field {
    public Field[] Fields;
    public FieldObject(Field[] fields)  {
        Fields = fields;
    }
    protected override void CreateSub(ItemForm form) {
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
    public T[] Fields;
    public FieldArray(Func<T> factory, int count = 0) {
        Fields = new T[count];
        for (int i = 0; i < count; i++) {
            Fields[i] = factory();
        }
    }
    protected override void CreateSub(ItemForm form) {
        form.Tab();
        form.NextLine();
        foreach (var field in Fields) {
            field.Create(form);
        }
        form.RemTab();
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return WriteJsonBlock(Fields, tabCount, true);
    }
}
