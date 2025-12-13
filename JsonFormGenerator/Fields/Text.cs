using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class FieldText : FieldData {
    public TextBox TextBox;

    public FieldText(Action<string>? update = null, string value = "") {
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
    private void Resize(object? o, EventArgs e) {
        int width = TextRenderer.MeasureText(TextBox.Text, TextBox.Font).Width + 15;
        TextBox.Width = Math.Max(width, 100);
    }
}