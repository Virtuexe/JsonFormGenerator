using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
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
}