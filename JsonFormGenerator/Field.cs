using System.Diagnostics;

namespace JsonFormGenerator;

public abstract class Field {
    //CREATE
    public void Create(ItemForm form) {
        CreateSub(form);
    }
    public static void Create(ItemForm form, Field[] fields) {
        foreach (Field field in fields) {
            field.Create(form);
        }
    }
    protected abstract void CreateSub(ItemForm form);
    //JSON
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
    //THEME
    public void ApplyTheme(Theme theme) {
        ApplyThemeSub(theme);
    }
    public static void ApplyTheme(Field[] fields, Theme theme) {
        foreach (Field field in fields) {
            field.ApplyTheme(theme);
        }
    }
    protected abstract void ApplyThemeSub(Theme theme);
}
public class LabeledField : Field {
    public Label Label { get; set; }
    public Field Field;
    public LabeledField(string name, Field field) {
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
    protected override void ApplyThemeSub(Theme theme) {
        Label.BackColor = theme.BackColor;
        Label.ForeColor = theme.TextColor;
        Field.ApplyTheme(theme);
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
    protected override void ApplyThemeSub(Theme theme) {
        TextBox.BackColor = theme.ItemColor;
        TextBox.ForeColor = theme.TextColor;

        TextBox.BorderStyle = BorderStyle.None;
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
    protected override void ApplyThemeSub(Theme theme) {
        CheckBox.BackColor = theme.BackColor;
        CheckBox.ForeColor = theme.TextColor;
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
    protected override void ApplyThemeSub(Theme theme) {
        NumericUpDown.BackColor = theme.ItemColor;
        NumericUpDown.ForeColor = theme.TextColor;
    }
}
public class FieldSelection : Field {
    public ComboBox ComboBox;
    private int lastSelectedIndex = -1;
    public FieldSelection(string[] selections) {
        ComboBox = new ComboBox();
        ComboBox.Items.AddRange(selections);
        ComboBox.SelectionChangeCommitted += (s, e) => {
            if (ComboBox.SelectedIndex == lastSelectedIndex) {
                ComboBox.SelectedIndex = -1;
                ComboBox.Text = string.Empty;
            }
            lastSelectedIndex = ComboBox.SelectedIndex;
        };
        Resize(selections);
    }
    protected override void CreateSub(ItemForm form) {
        form.Add(ComboBox);
        form.NextLine();
    }
    protected override string WriteJsonSub(int tabCount) {
        return ComboBox.SelectedText;
    }
    protected override void ApplyThemeSub(Theme theme) {
        ComboBox.BackColor = theme.ItemColor;
        ComboBox.ForeColor = theme.TextColor;
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
    protected override void ApplyThemeSub(Theme theme) {
        ApplyTheme(Fields, theme);
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
    protected override void ApplyThemeSub(Theme theme) {
        ApplyTheme(Fields, theme);
    }
}
public class FieldUnion : Field {
    public LabeledField[] Fields;
    public FieldSelection FieldSelection;
    public FieldUnion(LabeledField[] fields, ItemForm form) {
        Fields = fields;
        string[] values = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++) {
            values[i] = fields[i].Label.Text;
        }
        FieldSelection = new FieldSelection(values);
        FieldSelection.ComboBox.SelectedValueChanged += (s, e) => form.Recreate();
    }
    protected override void CreateSub(ItemForm form) {
        form.Tab();
        FieldSelection.Create(form);
        int i = FieldSelection.ComboBox.SelectedIndex;
        if (i != -1) {
            Fields[FieldSelection.ComboBox.SelectedIndex].Create(form);
        }
        form.RemTab();
    }
    protected override string WriteJsonSub(int tabCount) {
        throw new NotImplementedException();
    }
    protected override void ApplyThemeSub(Theme theme) {
        throw new NotImplementedException();
    }
}
