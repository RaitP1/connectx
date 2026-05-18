namespace ConsoleApp.UI;

public sealed class MenuItem
{
    private readonly Func<string> _labelFunc;

    public string Label => _labelFunc();
    public Func<string> Action { get; }
    public Menu? Submenu { get; }
    public EMenuAction MenuAction { get; }

    public MenuItem(string label, Func<string> action, Menu? submenu = null)
        : this(() => label, action, submenu)
    {
    }

    public MenuItem(Func<string> labelFunc, Func<string> action, Menu? submenu = null)
    {
        _labelFunc = labelFunc;
        Action = action;
        Submenu = submenu;
        MenuAction = EMenuAction.None;
    }

    internal MenuItem(string label, EMenuAction menuAction)
    {
        _labelFunc = () => label;
        Action = () => "";
        MenuAction = menuAction;
    }
}
