using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class LabeledField : Field {
    public Label Label;
    public Field? Field;
    public LabeledField(string name, Field? field = null) {
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
    internal override Field ReadJson(ref Utf8JsonReader reader) {
        var result = new LabeledField(Label.Text);
        reader.Read();
        if (reader.TokenType != JsonTokenType.Null) {
            result.Field = Field?.ReadJson(ref reader);
        }
        return result;
    }
}
