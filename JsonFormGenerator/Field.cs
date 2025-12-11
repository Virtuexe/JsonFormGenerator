using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Windows.Markup;

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
    public FieldData? Field;

    public LabeledField(string name, FieldData? field) {
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
        Field?.Create(form, cursor);
        if (Field is FieldBlock) {
            cursor.UnTab();
        }
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(Label);
        Field?.Destroy(form);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WritePropertyName(Label.Text);
        if (Field != null) {
            Field.WriteJson(writter);
        } else {
            writter.WriteNullValue();
        }
    }
    public override void ApplyTheme(Theme theme) {
        Label.BackColor = theme.BackColor;
        Label.ForeColor = theme.TextColor;
        Field?.ApplyTheme(theme);
    }
}


public class FieldBlock : FieldData {
    public Field[] Fields;

    public FieldBlock(Field[] fields) {
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

public delegate void FieldSelectionUpdate(string? from, string? to);
public class FieldSelection : FieldData {
    public ComboBox ComboBox;
    private int lastSelectedIndex = -1;
    private string? lastSelected {
        get {
            var i = lastSelectedIndex;
            return i == -1 ? null : (string)ComboBox.Items[i]!;
        }
    }
    public FieldSelection(string[] selections, FieldSelectionUpdate? update = null) {
        ComboBox = new ComboBox();
        ComboBox.Items.AddRange(selections);
        ComboBox.SelectionChangeCommitted += (s, e) => {
            if (ComboBox.SelectedIndex == lastSelectedIndex) {
                ComboBox.SelectedIndex = -1;
                ComboBox.Text = string.Empty;
            }
            update?.Invoke(lastSelected, (string)ComboBox.SelectedItem!);
            lastSelectedIndex = ComboBox.SelectedIndex;
        };
        Resize(selections);
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
public class FieldArray<T> : FieldData where T : FieldData {
    public List<T> Fields;

    private Button addBtn;
    private Button removeBtn;
    private event Action<int>? OnAdd;
    private event Action<int>? OnRem;
    private Func<int,T> factory;
    private Theme? theme;

    private SurveyForm? form;
    public FieldArray(Func<int,T> factory, Action<int, bool>? update = null) {
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

        OnAdd += (i) => update?.Invoke(i, true);
        OnRem += (i) => update?.Invoke(i, false);
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
        Fields.Add(factory(Fields.Count));
        form.Survey.Create(form, new());
        
        OnAdd?.Invoke(Fields.Count-1);
    }
    private void Remove(SurveyForm form) {
        Fields[^1].Destroy(form);
        Fields.Remove(Fields[^1]);
        form.Survey.Create(form, new());

        OnRem?.Invoke(Fields.Count);
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
public class FieldUnion : FieldData {
    public Func<Field>[] Factories;
    public Field? SelectedField;
    public FieldSelection FieldSelection;
    private Dictionary<string, int> map;

    private SurveyForm? form;
    public FieldUnion((string name, Func<Field> factory)[] fields, FieldSelectionUpdate? update = null) {
        Factories = new Func<Field>[fields.Length];
        string[] values = new string[fields.Length];
        map = new();
        for (int i = 0; i < fields.Length; i++) {
            Factories[i] = fields[i].factory;
            values[i] = fields[i].name;
            map[fields[i].name] = i;
        }
        FieldSelection = new FieldSelection(values, (from, to) => { Change(from, to); update?.Invoke(from, to); });
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        this.form = form;
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
            writter.WriteString("type", FieldSelection.ComboBox.Text);
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

    private void Change(string? from, string? to) {
        if (form == null) return;
        SelectedField?.Destroy(form);
        if (to != null) SelectedField = Factories[map[to]]();
        else SelectedField = null;
        form.Survey.Create(form, new());
    }
}
