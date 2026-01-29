using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class FieldText : FieldData {
    Action<string>? update;
    public TextBox TextBox;

    public FieldText(Action<string>? update = null) {
        this.update = update;

        TextBox = new TextBox();
        var func = (object? s, EventArgs e) => { };
        TextBox.TextChanged += Resize;
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
    internal override Field ReadJson(ref Utf8JsonReader reader) {
        var result = new FieldText(update);
        result.TextBox.Text = reader.GetString();
        reader.Read();
        return result;
    }
    private void Resize(object? o, EventArgs e) {
        int width = TextRenderer.MeasureText(TextBox.Text, TextBox.Font).Width + 15;
        TextBox.Width = Math.Max(width, 100);
    }
}