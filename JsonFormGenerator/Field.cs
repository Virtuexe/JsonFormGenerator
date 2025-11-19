using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace JsonFormGenerator;

public abstract class Field {
    public abstract void Create(ItemForm form, Cursor cursor);
    //public abstract void Remove(ItemForm form, Cursor cursor);
    internal abstract void WriteJson(Utf8JsonWriter writter);
    public abstract void ApplyTheme(Theme theme);
}
public abstract class FieldData : Field;
public class LabeledField<T> : Field where T : FieldData {
    public Label Label;
    public T Field;
    public LabeledField(string name, T field) {
        Label = new Label();
        Label.Text = name;
        Label.AutoSize = true;
        Field = field;
    }
    public override void Create(ItemForm form, Cursor cursor) {
        form.Add(Label, cursor);
        if (Field is FieldBlock || Field is FieldFactory<FieldBlock>) {
            form.Tab(cursor);
            form.NextLine(cursor);
        }
        Field.Create(form, cursor);
        if (Field is FieldBlock || Field is FieldFactory<FieldBlock>) {
            form.UnTab(cursor);
        }
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

public class FieldBlock : FieldData {
    private Field[] fields;
    public FieldBlock(Field[] fields) {
        this.fields = fields;
    }
    public override void Create(ItemForm form, Cursor cursor) {
        foreach (var field in fields) {
            field.Create(form, cursor);
            form.NextLine(cursor);
        }
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStartObject();
        foreach (var field in fields) {
            field.WriteJson(writter);
        }
        writter.WriteEndObject();
    }
    public override void ApplyTheme(Theme theme) {
        foreach (var field in fields) {
            field.ApplyTheme(theme);
        }
    }
}
public class FieldText : FieldData {
    public TextBox TextBox;
    public FieldText(string value = "")  {
        TextBox = new TextBox();
        var func = (object? s, EventArgs e) => { };
        TextBox.TextChanged += Resize;
        TextBox.Text = value;
        Resize(null, new());
    }
    public override void Create(ItemForm form, Cursor cursor) {
        form.Add(TextBox, cursor);
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
    public FieldCheck(bool value = false)  {
        CheckBox = new CheckBox();
        CheckBox.Checked = value;
        CheckBox.AutoSize = true;
        CheckBox.Text = " ";
    }
    public override void Create(ItemForm form, Cursor cursor) {
        form.Add(CheckBox, cursor);
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
    public FieldNumber(int value = 0)  {
        NumericUpDown = new NumericUpDown();
        NumericUpDown.Minimum = int.MinValue;
        NumericUpDown.Maximum = int.MaxValue;
        NumericUpDown.Value = value;
    }
    public override void Create(ItemForm form, Cursor cursor) {
        form.Add(NumericUpDown, cursor);
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
    public override void Create(ItemForm form, Cursor cursor) {
        form.Add(ComboBox, cursor);
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
    Button addBtn;
    Button removeBtn;
    Func<T> factory;
    Theme? theme;

    ItemForm? form;

    Cursor? arrayCursor;

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
        UpdateRmvBtn();
    }
    public override void Create(ItemForm form, Cursor cursor) {
        form.Tab(cursor);
        foreach (var field in Fields) {
            field.Create(form, cursor);
        }
        arrayCursor = cursor.Clone();
        form.UnTab(cursor);

        form.NextLine(cursor);
        form.Add(addBtn, cursor);
        form.Add(removeBtn, cursor);

        this.form = form;
    }
    private void Add(ItemForm form) {
        Fields.Add(factory());
        form.NextLine(arrayCursor!);
        Fields[^1].Create(form, arrayCursor!);
        form.Recalculate();
    }
    private void Remove(ItemForm form) {
        throw new NotImplementedException();
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
    public LabeledField<FieldBlock>[] Fields;
    private int selectedFieldIndex => FieldSelection.ComboBox.SelectedIndex;
    public LabeledField<FieldBlock>? SelectedField {
        get => selectedFieldIndex != -1 ? Fields[selectedFieldIndex] : null;
        set => Fields[selectedFieldIndex] = value!;
    }
    public FieldSelection FieldSelection;
    public FieldUnion(LabeledField<FieldBlock>[] fields, ItemForm form) {
        Fields = fields;
        string[] values = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++) {
            values[i] = fields[i].Label.Text;
        }
        FieldSelection = new FieldSelection(values);
        FieldSelection.ComboBox.SelectedValueChanged += (s, e) => form.Recreate();
    }
    public override void Create(ItemForm form, Cursor cursor) {
        FieldSelection.Create(form, cursor);
        if (SelectedField != null) {
            form.Tab(cursor);
            form.NextLine(cursor);
            SelectedField.Field.Create(form, cursor);
            form.UnTab(cursor);
        }
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        if (SelectedField != null) {
            writter.WriteStartObject();
            writter.WriteString("type", SelectedField.Label.Text);
            writter.WritePropertyName("data");
            SelectedField.Field.WriteJson(writter);
            writter.WriteEndObject();
        }
        else {
            writter.WriteNullValue();
        }
    }
    public override void ApplyTheme(Theme theme) {
        foreach (var field in Fields) {
            field.ApplyTheme(theme);
        }
    }
}
public class FieldFactory<T> : FieldData where T : FieldData {
    Func<T> factory;
    public T? Field;
    public FieldFactory(Func<T> factory) {
        this.factory = factory;
    }
    public override void Create(ItemForm form, Cursor cursor) {
        Field = factory();
        Field.Create(form, cursor);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        if (Field != null) {
            Field.WriteJson(writter);
        }
    }
    public override void ApplyTheme(Theme theme) {
        throw new NotImplementedException();
    }
}
