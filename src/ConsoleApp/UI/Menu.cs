namespace ConsoleApp.UI;

public sealed class Menu
{
    private readonly List<MenuItem> _items = [];
    private readonly Dictionary<ConsoleKey, int> _shortcutKeys = [];
    private readonly int _level;
    private int _cursorIndex;
    private IReadOnlyList<MenuItem>? _allItemsCache;

    public string Title { get; }
    public int CursorIndex => _cursorIndex;

    public IReadOnlyList<MenuItem> NavigationItems { get; }

    public IReadOnlyList<MenuItem> AllItems => _allItemsCache ??= BuildAllItems();

    public Menu(string title, int level)
    {
        Title = title;
        _level = level;
        NavigationItems = BuildNavigationItems(level);
    }

    public void AddItem(MenuItem item, ConsoleKey? shortcutKey = null)
    {
        if (shortcutKey is not null)
        {
            if (_shortcutKeys.ContainsKey(shortcutKey.Value))
                throw new ArgumentException($"Shortcut key '{shortcutKey}' is already registered in menu '{Title}'.");

            _shortcutKeys[shortcutKey.Value] = _items.Count;
        }

        _items.Add(item);
        _allItemsCache = null;
    }

    public void MoveCursor(int delta)
    {
        var count = AllItems.Count;
        if (count == 0) return;

        _cursorIndex = ((_cursorIndex + delta) % count + count) % count;
    }

    public string SelectCurrent()
    {
        var items = AllItems;
        if (_cursorIndex < 0 || _cursorIndex >= items.Count) return "";

        var item = items[_cursorIndex];
        return item.Action();
    }

    public bool SelectByNumber(int number)
    {
        var items = AllItems;
        var index = number - 1;
        if (index < 0 || index >= items.Count) return false;

        _cursorIndex = index;
        items[index].Action();
        return true;
    }

    public MenuItem GetCurrentItem()
    {
        return AllItems[_cursorIndex];
    }

    public bool SelectByShortcutKey(ConsoleKey key)
    {
        if (!_shortcutKeys.TryGetValue(key, out var index)) return false;

        _cursorIndex = index;
        _items[index].Action();
        return true;
    }

    private List<MenuItem> BuildAllItems()
    {
        var all = new List<MenuItem>(_items);
        all.AddRange(NavigationItems);
        return all;
    }

    private static List<MenuItem> BuildNavigationItems(int level)
    {
        var items = new List<MenuItem>();

        if (level >= 3)
            items.Add(new MenuItem("Return to Main", EMenuAction.ReturnToMain));

        if (level >= 2)
            items.Add(new MenuItem("Return to Previous", EMenuAction.ReturnToPrevious));

        items.Add(new MenuItem("Exit", EMenuAction.Exit));

        return items;
    }
}
