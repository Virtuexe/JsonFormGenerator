using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class FieldUnionConstruct(string name, Func<Field>? factory) { public string Name = name; public Func<Field>? Factory = factory; }
public class FieldUnion : FieldData {
    public Func<Field>?[] Factories;
    public Field? SelectedField;
    public FieldSelection FieldSelection;

    private SurveyForm? form;
    public FieldUnion(FieldUnionConstruct[] fields, FieldSelectionUpdate? update = null) {
        Factories = new Func<Field>[fields.Length];
        string[] values = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++) {
            Factories[i] = fields[i].Factory;
            values[i] = fields[i].Name;
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
        writter.WriteStartObject();
        writter.WritePropertyName("type");
        FieldSelection.WriteJson(writter);
        writter.WritePropertyName("value");
        if (SelectedField != null)
            SelectedField.WriteJson(writter);
        else
            writter.WriteNullValue();
        writter.WriteEndObject();
    }
    private void Change(string? from, string? to) {
        var i = FieldSelection.ComboBox.SelectedIndex;
        if(form != null) SelectedField?.Destroy(form);
        if (to != null) SelectedField = Factories[i]?.Invoke();
        else SelectedField = null;
        form?.Survey.Create(form, new());
    }
}