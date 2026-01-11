using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class LabeledField : Field {
    public Label Label;
    public FieldData? Field;
    public LabeledField(string name, FieldData? field = null) {
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
        if (Field is not FieldUnion) {
            writter.WritePropertyName(Label.Text);
        }
        if (Field != null) {
            Field.WriteJson(writter);
        } else {
            writter.WriteNullValue();
        }
    }
}
