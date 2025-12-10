using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace JsonFormGenerator;

public struct FieldValue(object value) { public object Value = value; }
public abstract class Field {
    public abstract void Create(SurveyForm form, Cursor cursor);
    public abstract void Destroy(SurveyForm form);
    //public abstract void Remove(ItemForm form, Cursor cursor);
    internal abstract void WriteJson(Utf8JsonWriter writter);
    public abstract void ApplyTheme(Theme theme);
}
public abstract class FieldData : Field;

public struct LabeledFieldValue(string name, object value) { public string Name = name; public object Value = value; }
public class LabeledField : Field {
    public Label Label;
    public FieldData Field;

    public LabeledField(string name, FieldData field) {
        Label = new Label();
        Label.Text = name;
        Label.AutoSize = true;
        Field = field;
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(Label, form);
        if (Field is FieldBlock) {
            cursor.Tab();
            cursor.NextLine();
        }
        Field.Create(form, cursor);
        if (Field is FieldBlock) {
            cursor.UnTab();
        }
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(Label);
        Field.Destroy(form);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WritePropertyName(Label.Text);
        Field.WriteJson(writter);
    }
    public override void ApplyTheme(Theme theme) {
        Label.BackColor = theme.BackColor;
        Label.ForeColor = theme.TextColor;
        Field.ApplyTheme(theme);
    }
}

public struct FieldBlockValue(int index, object value) { public int Index = index; public object Value = value; }
public class FieldBlock : FieldData {
    public Field[] Fields;

    public FieldBlock(Field[] fields, Action<FieldBlockValue>? update = null) {
        this.Fields = fields;
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        foreach (var field in Fields) {
            field.Create(form, cursor);
            cursor.NextLine();
        }
    }
    public override void Destroy(SurveyForm form) {
        foreach (var field in Fields) {
            field.Destroy(form);
        }
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStartObject();
        foreach (var field in Fields) {
            field.WriteJson(writter);
        }
        writter.WriteEndObject();
    }
    public override void ApplyTheme(Theme theme) {
        foreach (var field in Fields) {
            field.ApplyTheme(theme);
        }
    }
}
public class FieldText : FieldData {
    public TextBox TextBox;

    public FieldText(Action<string>? update = null, string value = "")  {
        TextBox = new TextBox();
        var func = (object? s, EventArgs e) => { };
        TextBox.TextChanged += Resize;
        TextBox.Text = value;
        Resize(null, new());

        TextBox.TextChanged += (_, _) => update?.Invoke(TextBox.Text);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(TextBox, form);
    }
    public override void Destroy(SurveyForm form) {
        form.Controls.Remove(TextBox);
        TextBox.Dispose();
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStringValue(TextBox.Text);
    }
    public override void ApplyTheme(Theme theme) {
        TextBox.BackColor = theme.ItemColor;
        TextBox.ForeColor = theme.TextColor;

        TextBox.BorderStyle = BorderStyle.None;
    }
    private void Resize(object? o, EventArgs e) {
        int width = TextRenderer.MeasureText(TextBox.Text, TextBox.Font).Width + 15;
        TextBox.Width = Math.Max(width, 100);
    }
}
public class FieldCheck : FieldData {
    public CheckBox CheckBox;
    public FieldCheck(Action<bool>? update = null, bool value = false) {
        CheckBox = new CheckBox();
        CheckBox.Checked = value;
        CheckBox.AutoSize = true;
        CheckBox.Text = " ";

        CheckBox.CheckedChanged += (_, _) => update?.Invoke(CheckBox.Checked);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(CheckBox, form);
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(CheckBox);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteBooleanValue(CheckBox.Checked);
    }
    public override void ApplyTheme(Theme theme) {
        CheckBox.BackColor = theme.BackColor;
        CheckBox.ForeColor = theme.TextColor;
    }
}
public class FieldNumber : FieldData {
    public NumericUpDown NumericUpDown;
    public FieldNumber(Action<decimal>? update = null, int value = 0) {
        NumericUpDown = new NumericUpDown();
        NumericUpDown.Minimum = int.MinValue;
        NumericUpDown.Maximum = int.MaxValue;
        NumericUpDown.Value = value;

        NumericUpDown.ValueChanged += (_, _) => update?.Invoke(NumericUpDown.Value);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(NumericUpDown, form);
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(NumericUpDown);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteNumberValue(NumericUpDown.Value);
    }
    public override void ApplyTheme(Theme theme) {
        NumericUpDown.BackColor = theme.ItemColor;
        NumericUpDown.ForeColor = theme.TextColor;
    }
}
public class FieldSelection : FieldData {
    public ComboBox ComboBox;
    private int lastSelectedIndex = -1;
    public FieldSelection(string[] selections, Action<string>? update = null) {
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

        ComboBox.SelectedValueChanged += (_, _) => update?.Invoke((string)ComboBox.SelectedValue!);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(ComboBox, form);
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(ComboBox);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStringValue(ComboBox.Text);
    }
    public override void ApplyTheme(Theme theme) {
        ComboBox.BackColor = theme.ItemColor;
        ComboBox.ForeColor = theme.TextColor;
    }
    private void Resize(string[] selections) {
        int largestWidth = 0;
        foreach (var selection in selections) {
            int width = TextRenderer.MeasureText(selection, ComboBox.Font).Width;
            largestWidth = width > largestWidth ? width : largestWidth;
        }
        ComboBox.Width = largestWidth + 32;
    }
}
public struct FieldArrayValue(int index, object value) { public int Index = index; public object Value = value; }
public class FieldArray<T> : FieldData where T : FieldData {
    public List<T> Fields;

    private Button addBtn;
    private Button removeBtn;
    private Func<T> factory;
    private Theme? theme;

    private SurveyForm? form;
    public FieldArray(Func<T> factory) {
        Fields = new List<T>();

        addBtn = new Button();
        removeBtn = new Button();
        addBtn.Text = "ADD";
        removeBtn.Text = "REMOVE";
        addBtn.AutoSize = true;
        removeBtn.AutoSize = true;
        addBtn.Click += (_, _) => Add(form!);
        removeBtn.Click += (_, _) => Remove(form!);

        this.factory = factory;
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Tab();
        cursor.NextLine();
        foreach (var field in Fields) {
            field.Create(form, cursor);
        }
        cursor.UnTab();

        cursor.NextLine();
        cursor.Add(addBtn, form);
        cursor.Add(removeBtn, form);

        this.form = form;
        UpdateRmvBtn();
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(addBtn);
        form.Dispose(removeBtn);
        foreach (var field in Fields) {
            field.Destroy(form);
        }
    }
    private void Add(SurveyForm form) {
        Fields.Add(factory());
        form.Survey.Create(form, new());
    }
    private void Remove(SurveyForm form) {
        Fields[^1].Destroy(form);
        Fields.Remove(Fields[^1]);
        form.Survey.Create(form, new());
    }
    private void UpdateRmvBtn() {
        removeBtn.Enabled = Fields.Count != 0;
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStartArray();
        foreach (var field in Fields) {
            field.WriteJson(writter);
        }
        writter.WriteEndArray();
    }
    public override void ApplyTheme(Theme theme) {
        this.theme = theme;
        foreach (var field in Fields) {
            field.ApplyTheme(theme);
        }
    }
}
struct FieldUnionValue(string name, object value) { public string Name = name; public object Value = value; }
public class FieldUnion : FieldData {
    public Func<FieldBlock>[] Factories;
    public FieldBlock? SelectedField;
    private int selectedIndex => FieldSelection.ComboBox.SelectedIndex;
    private string selectedName => FieldSelection.ComboBox.Text;
    public FieldSelection FieldSelection;
    public FieldUnion((string name, Func<FieldBlock> factories)[] fields) {
        Factories = fields.Select((f) => f.factories).ToArray();
        string[] values = fields.Select(f => f.name).ToArray();
        FieldSelection = new FieldSelection(values);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        FieldSelection.Create(form, cursor);
        if (SelectedField != null) {
            cursor.Tab();
            cursor.NextLine();
            SelectedField.Create(form, cursor);
            cursor.UnTab();
        }
    }
    public override void Destroy(SurveyForm form) {
        SelectedField?.Destroy(form);
        FieldSelection.Destroy(form);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        if (SelectedField != null) {
            writter.WriteStartObject();
            writter.WriteString("type", selectedName);
            writter.WritePropertyName("data");
            SelectedField.WriteJson(writter);
            writter.WriteEndObject();
        }
        else {
            writter.WriteNullValue();
        }
    }
    public override void ApplyTheme(Theme theme) {
        //this.theme = theme;
    }
}
