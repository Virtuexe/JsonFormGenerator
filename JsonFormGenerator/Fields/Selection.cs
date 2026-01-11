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
        Resize(selections);
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
    private void Resize(string[] selections) {
        int largestWidth = 0;
        foreach (var selection in selections) {
            int width = TextRenderer.MeasureText(selection, ComboBox.Font).Width;
            largestWidth = width > largestWidth ? width : largestWidth;
        }
        ComboBox.Width = largestWidth + 32;
    }
}