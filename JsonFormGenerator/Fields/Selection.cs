using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public delegate void FieldSelectionUpdate(string? from, string? to);
public class FieldSelection : FieldData {
    public ComboBox ComboBox;
    private List<string> selections;
    private int lastSelectedIndex = -1;
    private string? lastSelected {
        get {
            var i = lastSelectedIndex;
            return i == -1 ? null : (string)ComboBox.Items[i]!;
        }
    } 
    public FieldSelection(string[] selections, FieldSelectionUpdate? update = null) {
        ComboBox = new ComboBox();
        ComboBox.Items.AddRange(selections);
        ComboBox.SelectionChangeCommitted += (s, e) => {
            if (ComboBox.SelectedIndex == lastSelectedIndex) {
                ComboBox.SelectedIndex = -1;
                ComboBox.Text = string.Empty;
            }
            update?.Invoke(lastSelected, (string)ComboBox.SelectedItem!);
            lastSelectedIndex = ComboBox.SelectedIndex;
        };
        this.selections = selections.ToList();
        Resize();
    }
    public override void Create(SurveyForm form, Cursor cursor) {
        cursor.Add(ComboBox, form);
    }
    public override void Destroy(SurveyForm form) {
        form.Dispose(ComboBox);
    }
    internal override void WriteJson(Utf8JsonWriter writter) {
        writter.WriteStringValue((string)ComboBox.SelectedItem!);
    }
    internal override Field ReadJson(ref Utf8JsonReader reader) {
        var items = new string[ComboBox.Items.Count];
        for (int i = 0; i < ComboBox.Items.Count; i++) items[i] = (string)ComboBox.Items[i];

        var result = new FieldSelection(items);
        string? value = reader.GetString();
        if (value != null) result.ComboBox.SelectedItem = value;
        reader.Read();
        return result;
    }
    public void Insert(int i, string value) {
        selections.Insert(i, value);
        ComboBox.Items.Insert(i, value);
        Resize();
    }
    public void Remove(int i) {
        selections.RemoveAt(i);
        ComboBox.Items.RemoveAt(i);
        Resize();
    }
    public void Rename(int i, string value) {
        selections[i] = value;
        ComboBox.Items[i] = value;
        Resize();
    }
    private void Resize() {
        int largestWidth = 0;
        foreach (var selection in selections) {
            int width = TextRenderer.MeasureText(selection, ComboBox.Font).Width;
            largestWidth = width > largestWidth ? width : largestWidth;
        }
        ComboBox.Width = largestWidth + 32;
    }
}