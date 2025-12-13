using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;

public abstract class FieldData : Field;
public abstract class Field {
    public abstract void Create(SurveyForm form, Cursor cursor);
    public abstract void Destroy(SurveyForm form);
    //public abstract void Remove(ItemForm form, Cursor cursor);
    internal abstract void WriteJson(Utf8JsonWriter writter);
}
