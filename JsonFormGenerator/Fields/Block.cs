using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;

public class FieldBlock : FieldData {
    public List<Field?> Fields;

    public FieldBlock(List<Field?> fields) {
        this.Fields = fields;
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        foreach (var field in Fields) {
            if(field == null) continue; 
            field.Create(form, cursor);
            cursor.NextLine();
        }
    }
    public override void Destroy(SurveyForm form) {
        foreach (var field in Fields) {
            field?.Destroy(form);
        }
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStartObject();
        foreach (var field in Fields) {
            field?.WriteJson(writter);
        }
        writter.WriteEndObject();
    }
}
