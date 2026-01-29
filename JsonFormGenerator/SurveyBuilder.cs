using System;
using System.Collections.Generic;

namespace JsonFormGenerator;
public class SurveyBuilder {
    public FieldBlock Survey { get; private set; }
    public SurveyForm? PreviewForm { get; set; }

    public SurveyBuilder() {
        Survey = new FieldBlock([]);
    }

    public Field CreateEditorSchema() {
        FieldBlock editorSchema = new([]);
        editorSchema.Fields = [
            new LabeledField("fields", new FieldArray<FieldBlock>(
                (i) => CreateField(i),
                (i, add) => BindArray(this.Survey.Fields, i, add))
            )
        ];
        return editorSchema;
    }

    private FieldBlock CreateField(int i) {
        FieldUnion union = new([
            new("text", null),
            new("number", null),
            new("check", null),
            new("selection", () => new FieldArray<FieldText>(
                (i2) => new((value) => BindFieldSelectionValue(i, i2, value)),
                (i2, add) => BindFieldSelection(i, i2, add))),
        ], (_, to) => BindField(i, to));

        FieldBlock field = new([
            new LabeledField("name", new FieldText((value) => ((LabeledField)Survey.Fields[i]!).Label.Text = value)),
            new LabeledField("type", union)
        ]);
        return field;
    }

    private void BindArray(List<Field?> fields, int i, bool add) {
        if (add) {
            fields.Insert(i, new LabeledField("", null));
        }
        else {
            if (PreviewForm != null) fields[i]?.Destroy(PreviewForm);
            fields.RemoveAt(i);
        }
        if (PreviewForm != null) Survey.Create(PreviewForm, new());
    }

    private void BindField(int i, string? to) {
        ref var field = ref ((LabeledField)Survey.Fields[i]!).Field;
        if (PreviewForm != null) field?.Destroy(PreviewForm);

        switch (to) {
            case "text":
                field = new FieldText();
                break;
            case "number":
                field = new FieldNumber();
                break;
            case "check":
                field = new FieldCheck();
                break;
            case "selection":
                field = new FieldSelection([]);
                break;
            case "array":
                break;
            case "block":
                field = new FieldBlock([]);
                break;
        }
        if (PreviewForm != null) Survey.Create(PreviewForm, new());
    }

    private void BindFieldSelection(int fieldIndex, int valueIndex, bool add) {
        var selection = (FieldSelection)((LabeledField)Survey.Fields[fieldIndex]!).Field!;
        if (add)
            selection.Insert(valueIndex, "");
        else
            selection.Remove(valueIndex);
    }

    private void BindFieldSelectionValue(int fieldIndex, int valueIndex, string value) {
        var selection = (FieldSelection)((LabeledField)Survey.Fields[fieldIndex]!).Field!;
        selection.Rename(valueIndex, value);
        if (PreviewForm != null) Survey.Create(PreviewForm, new());
    }
}
