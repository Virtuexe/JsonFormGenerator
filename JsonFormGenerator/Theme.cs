namespace JsonFormGenerator;

public class Theme {
    public static Theme MainTheme = new Theme {
        BackColor = Color.FromArgb(1, 4, 9),
        ItemColor = Color.FromArgb(61, 68, 77),
        TextColor = Color.White
    };
    public Color BackColor;
    public Color ItemColor;
    public Color TextColor;
    public void ApplyTheme(Form form) {
        form.BackColor = BackColor;
        //form.ForeColor = itemColor;
    }
}
