using ConsoleApp.UI;

namespace ConnectX.UnitTests.ConsoleApp.UI;

public class MenuItemTests
{
    [Fact]
    public void StaticLabel_ReturnsFixedText()
    {
        var item = new MenuItem("Start Game", () => "");

        Assert.Equal("Start Game", item.Label);
    }

    [Fact]
    public void DynamicLabel_ReflectsCurrentValue()
    {
        var width = 7;
        var item = new MenuItem(() => $"Board Width [{width}]", () => "");

        Assert.Equal("Board Width [7]", item.Label);

        width = 10;
        Assert.Equal("Board Width [10]", item.Label);
    }

    [Fact]
    public void Action_ExecutesAndReturnsResult()
    {
        var executed = false;
        var item = new MenuItem("Do Thing", () => { executed = true; return "done"; });

        var result = item.Action();

        Assert.True(executed);
        Assert.Equal("done", result);
    }

    [Fact]
    public void HasSubmenu_WhenChildMenuProvided()
    {
        var child = new Menu("Sub", 2);
        var item = new MenuItem("Settings", () => "", child);

        Assert.NotNull(item.Submenu);
        Assert.Same(child, item.Submenu);
    }

    [Fact]
    public void HasNoSubmenu_ByDefault()
    {
        var item = new MenuItem("Simple", () => "");

        Assert.Null(item.Submenu);
    }
}

public class MenuTests
{
    [Fact]
    public void CursorWraps_DownFromLast_GoesToFirst()
    {
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => ""));
        menu.AddItem(new MenuItem("B", () => ""));
        menu.AddItem(new MenuItem("C", () => ""));

        var totalItems = menu.AllItems.Count;
        menu.MoveCursor(totalItems); // move past the end

        Assert.Equal(0, menu.CursorIndex % totalItems);
    }

    [Fact]
    public void CursorWraps_UpFromFirst_GoesToLast()
    {
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => ""));
        menu.AddItem(new MenuItem("B", () => ""));
        menu.AddItem(new MenuItem("C", () => ""));

        menu.MoveCursor(-1);

        Assert.Equal(menu.AllItems.Count - 1, menu.CursorIndex);
    }

    [Fact]
    public void Level1_HasExitOnly()
    {
        var menu = new Menu("Main", 1);
        menu.AddItem(new MenuItem("Play", () => ""));

        var navItems = menu.NavigationItems;

        Assert.Single(navItems);
        Assert.Equal(EMenuAction.Exit, navItems[0].MenuAction);
    }

    [Fact]
    public void Level2_HasExitAndReturnToPrevious()
    {
        var menu = new Menu("Settings", 2);
        menu.AddItem(new MenuItem("Option", () => ""));

        var navItems = menu.NavigationItems;

        Assert.Equal(2, navItems.Count);
        Assert.Contains(navItems, n => n.MenuAction == EMenuAction.Exit);
        Assert.Contains(navItems, n => n.MenuAction == EMenuAction.ReturnToPrevious);
    }

    [Fact]
    public void Level3_HasExitReturnToPreviousAndReturnToMain()
    {
        var menu = new Menu("Player Config", 3);
        menu.AddItem(new MenuItem("Name", () => ""));

        var navItems = menu.NavigationItems;

        Assert.Equal(3, navItems.Count);
        Assert.Contains(navItems, n => n.MenuAction == EMenuAction.Exit);
        Assert.Contains(navItems, n => n.MenuAction == EMenuAction.ReturnToPrevious);
        Assert.Contains(navItems, n => n.MenuAction == EMenuAction.ReturnToMain);
    }

    [Fact]
    public void NumberKeyDispatch_SelectsCorrectItem()
    {
        var selected = "";
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => { selected = "A"; return ""; }));
        menu.AddItem(new MenuItem("B", () => { selected = "B"; return ""; }));
        menu.AddItem(new MenuItem("C", () => { selected = "C"; return ""; }));

        menu.SelectByNumber(2);

        Assert.Equal("B", selected);
    }

    [Fact]
    public void NumberKeyDispatch_InvalidNumber_DoesNothing()
    {
        var executed = false;
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => { executed = true; return ""; }));

        var result = menu.SelectByNumber(5);

        Assert.False(executed);
        Assert.False(result);
    }

    [Fact]
    public void SelectCurrent_ExecutesHighlightedItemAction()
    {
        var selected = "";
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => { selected = "A"; return ""; }));
        menu.AddItem(new MenuItem("B", () => { selected = "B"; return ""; }));

        menu.MoveCursor(1);
        menu.SelectCurrent();

        Assert.Equal("B", selected);
    }

    [Fact]
    public void AllItems_IncludesUserItemsAndNavigationItems()
    {
        var menu = new Menu("Test", 2);
        menu.AddItem(new MenuItem("Play", () => ""));
        menu.AddItem(new MenuItem("Settings", () => ""));

        var all = menu.AllItems;

        Assert.True(all.Count > 2);
        Assert.Equal("Play", all[0].Label);
        Assert.Equal("Settings", all[1].Label);
    }

    [Fact]
    public void DuplicateShortcutKey_ThrowsOnRegistration()
    {
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => ""), ConsoleKey.D1);

        Assert.Throws<ArgumentException>(() =>
            menu.AddItem(new MenuItem("B", () => ""), ConsoleKey.D1));
    }

    [Fact]
    public void UniqueShortcutKeys_Allowed()
    {
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => ""), ConsoleKey.D1);
        menu.AddItem(new MenuItem("B", () => ""), ConsoleKey.D2);

        Assert.Equal(2, menu.AllItems.Count - menu.NavigationItems.Count);
    }

    [Fact]
    public void SelectByShortcutKey_ExecutesCorrectItem()
    {
        var selected = "";
        var menu = new Menu("Test", 1);
        menu.AddItem(new MenuItem("A", () => { selected = "A"; return ""; }), ConsoleKey.A);
        menu.AddItem(new MenuItem("B", () => { selected = "B"; return ""; }), ConsoleKey.B);

        menu.SelectByShortcutKey(ConsoleKey.B);

        Assert.Equal("B", selected);
    }
}
