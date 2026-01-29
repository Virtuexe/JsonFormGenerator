using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFormGenerator;
public class Cursor {
    private Point pos;
    private List<int> nextLines = new List<int> { new() };
    public Cursor Clone() => (Cursor)this.MemberwiseClone();
    public void Add(Control c, SurveyForm form) {
        form.Controls.Add(c);
        c.Location = new(pos.X, pos.Y + form.AutoScrollPosition.Y);
        pos.X += c.Width;
        if (nextLines[^1] < c.Height)
            nextLines[^1] = c.Height;
    }
    private int tabCount;
    public void NextLine() {
        pos.X = tabCount * 32;
        pos.Y += nextLines[^1];
        nextLines.Add(0);
    }
    public void Tab() => tabCount++;
    public void UnTab() => tabCount--;
    //public void Set(Control c) {
    //    pos = c.Location;
    //}
}
