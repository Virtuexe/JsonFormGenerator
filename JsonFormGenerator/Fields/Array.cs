using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;

public abstract class FieldArray : FieldData;
public delegate void FieldArrayUpdate(int index, bool add);
public class FieldArray<T> : FieldArray where T : FieldData {
    public List<T> Fields;

    private Button addBtn;
    private Button removeBtn;
    private event Action<int>? OnAdd;
    private event Action<int>? OnRem;
    public Func<int, T>? Factory;
    private Theme? theme;

    private SurveyForm? form;
    public FieldArray(Func<int, T>? factory, FieldArrayUpdate? update = null) {
        Fields = new List<T>();

        addBtn = new Button();
        removeBtn = new Button();
        addBtn.Text = "ADD";
        removeBtn.Text = "REMOVE";
        addBtn.AutoSize = true;
        removeBtn.AutoSize = true;
        addBtn.Click += (_, _) => Add(form!);
        removeBtn.Click += (_, _) => Remove(form!);

        this.Factory = factory;

        OnAdd += (i) => update?.Invoke(i, true);
        OnRem += (i) => update?.Invoke(i, false);
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Tab();
        foreach (var field in Fields) {
            cursor.NextLine();
            field.Create(form, cursor);
        }
        cursor.UnTab();

        cursor.NextLine();
        cursor.Add(addBtn, form);
        cursor.Add(removeBtn, form);

        this.form = form;
        UpdateBtns();
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(addBtn);
        form.Dispose(removeBtn);
        foreach (var field in Fields) {
            field.Destroy(form);
        }
    }
    private void Add(SurveyForm form) {
        Fields.Add(Factory!(Fields.Count));
        form.Survey.Create(form, new());

        OnAdd?.Invoke(Fields.Count - 1);
    }
    private void Remove(SurveyForm form) {
        Fields[^1].Destroy(form);
        Fields.Remove(Fields[^1]);
        form.Survey.Create(form, new());

        OnRem?.Invoke(Fields.Count);
    }
    private void UpdateBtns() {
        addBtn.Enabled = Factory != null;
        removeBtn.Enabled = Fields.Count != 0;
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStartArray();
        foreach (var field in Fields) {
            field.WriteJson(writter);
        }
        writter.WriteEndArray();
    }
}