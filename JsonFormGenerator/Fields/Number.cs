using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
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
    internal override Field ReadJson(ref Utf8JsonReader reader) {
        var result = new FieldNumber();
        result.NumericUpDown.Value = reader.GetDecimal();
        reader.Read();
        return result;
    }
}