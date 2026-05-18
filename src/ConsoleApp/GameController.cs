using Application.AI;
using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Application.Game.Mapping;
using ConsoleApp.UI;
using Domain;

namespace ConsoleApp;

public sealed class GameController
{
    private readonly IConfigRepository _configRepo;
    private readonly IGameRepository _gameRepo;
    private readonly GameUI _ui;
    private GameConfig _currentConfig;

    public GameController(IConfigRepository configRepo, IGameRepository gameRepo, GameUI ui)
    {
        _configRepo = configRepo;
        _gameRepo = gameRepo;
        _ui = ui;
        _currentConfig = new GameConfig(
            "Custom", 6, 7, 4, "Player 1", "X", "Player 2", "O",
            EBoardTopology.Rectangle, new PlayerType(false), new PlayerType(false));
    }

    public void Run()
    {
        var mainMenu = BuildMenuTree();
        while (true)
        {
            var result = RunMenu(mainMenu);
            if (result == EMenuAction.Exit)
                return;
        }
    }

    private Menu BuildMenuTree()
    {
        var playerConfigMenu = new Menu("Player Configuration", 3);
        playerConfigMenu.AddItem(new MenuItem(
            () => $"Player 1 Type [{FormatPlayerType(_currentConfig.Player1Type)}]",
            () => { ConfigurePlayerType(1); return ""; }));
        playerConfigMenu.AddItem(new MenuItem(
            () => $"Player 2 Type [{FormatPlayerType(_currentConfig.Player2Type)}]",
            () => { ConfigurePlayerType(2); return ""; }));

        var boardConfigMenu = new Menu("Board Configuration", 3);
        boardConfigMenu.AddItem(new MenuItem(
            () => $"Board Width [{_currentConfig.Columns}]",
            () => { ChangeIntSetting("Board Width", 3, 20, v => _currentConfig = _currentConfig with { Columns = v }); return ""; }));
        boardConfigMenu.AddItem(new MenuItem(
            () => $"Board Height [{_currentConfig.Rows}]",
            () => { ChangeIntSetting("Board Height", 3, 20, v => _currentConfig = _currentConfig with { Rows = v }); return ""; }));
        boardConfigMenu.AddItem(new MenuItem(
            () => $"Win Condition [{_currentConfig.WinCondition}]",
            () => { ChangeIntSetting("Win Condition", 3, 10, v => _currentConfig = _currentConfig with { WinCondition = v }); return ""; }));
        boardConfigMenu.AddItem(new MenuItem(
            () => $"Topology [{_currentConfig.Topology}]",
            () => { ToggleTopology(); return ""; }));

        var configManagementMenu = new Menu("Configuration Management", 3);
        configManagementMenu.AddItem(new MenuItem("Save Current Configuration", () => { SaveConfiguration(); return ""; }));
        configManagementMenu.AddItem(new MenuItem("Load Configuration", () => { LoadConfiguration(); return ""; }));
        configManagementMenu.AddItem(new MenuItem("Delete Configuration", () => { DeleteConfiguration(); return ""; }));

        var settingsMenu = new Menu("Settings", 2);
        settingsMenu.AddItem(new MenuItem("Board Settings", () => "", boardConfigMenu));
        settingsMenu.AddItem(new MenuItem("Player Settings", () => "", playerConfigMenu));
        settingsMenu.AddItem(new MenuItem("Manage Configurations", () => "", configManagementMenu));
        settingsMenu.AddItem(new MenuItem("View Current Settings", () => { ViewCurrentSettings(); return ""; }));

        var loadGameMenu = new Menu("Load Game", 2);
        loadGameMenu.AddItem(new MenuItem("Load Saved Game", () => { LoadAndContinueGame(); return ""; }));
        loadGameMenu.AddItem(new MenuItem("Delete Saved Game", () => { DeleteSavedGame(); return ""; }));

        var newGameMenu = new Menu("New Game", 2);
        newGameMenu.AddItem(new MenuItem("Start with Current Settings", () => { StartNewGame(_currentConfig); return ""; }));
        newGameMenu.AddItem(new MenuItem("Start from Saved Configuration", () => { StartFromSavedConfig(); return ""; }));

        var mainMenu = new Menu("Connect-X", 1);
        mainMenu.AddItem(new MenuItem("New Game", () => "", newGameMenu));
        mainMenu.AddItem(new MenuItem("Load Game", () => "", loadGameMenu));
        mainMenu.AddItem(new MenuItem("Settings", () => "", settingsMenu));
        mainMenu.AddItem(new MenuItem("How to Play", () => { ShowHowToPlay(); return ""; }));

        return mainMenu;
    }

    private EMenuAction RunMenu(Menu menu)
    {
        while (true)
        {
            Console.Clear();
            DisplayMenu(menu);

            var key = Console.ReadKey(true);
            MenuItem? selectedItem = null;

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    menu.MoveCursor(-1);
                    continue;
                case ConsoleKey.DownArrow:
                    menu.MoveCursor(1);
                    continue;
                case ConsoleKey.Enter:
                    selectedItem = menu.GetCurrentItem();
                    break;
                default:
                    if (key.KeyChar >= '1' && key.KeyChar <= '9')
                    {
                        var index = key.KeyChar - '0' - 1;
                        var items = menu.AllItems;
                        if (index >= 0 && index < items.Count)
                            selectedItem = items[index];
                    }
                    break;
            }

            if (selectedItem is null) continue;

            var result = HandleItemSelection(selectedItem);
            if (result == EMenuAction.Exit)
                return EMenuAction.Exit;
            if (result == EMenuAction.ReturnToPrevious)
                return EMenuAction.ReturnToPrevious;
            if (result == EMenuAction.ReturnToMain)
                return EMenuAction.ReturnToMain;
        }
    }

    private EMenuAction HandleItemSelection(MenuItem item)
    {
        if (item.MenuAction is EMenuAction.Exit or EMenuAction.ReturnToPrevious or EMenuAction.ReturnToMain)
            return item.MenuAction;

        if (item.Submenu is not null)
        {
            var result = RunMenu(item.Submenu);
            if (result == EMenuAction.ReturnToMain)
                return EMenuAction.ReturnToMain;
            return EMenuAction.None;
        }

        item.Action();
        return EMenuAction.None;
    }

    private void DisplayMenu(Menu menu)
    {
        Console.WriteLine($"=== {menu.Title} ===");
        Console.WriteLine();

        var items = menu.AllItems;
        for (var i = 0; i < items.Count; i++)
        {
            var prefix = i == menu.CursorIndex ? " > " : "   ";
            Console.WriteLine($"{prefix}{i + 1}. {items[i].Label}");
        }

        Console.WriteLine();
        Console.WriteLine("[↑/↓] Navigate  [Enter] Select  [1-9] Quick select");
    }

    private void StartNewGame(GameConfig config)
    {
        if (!config.IsValid())
        {
            Console.WriteLine("Invalid configuration. Please check your settings.");
            WaitForKey();
            return;
        }

        var brain = new GameBrain(config);
        PlayGame(brain);
    }

    private void PlayGame(GameBrain brain)
    {
        while (!brain.IsGameOver)
        {
            var currentPlayerType = brain.CurrentPlayer == 0
                ? brain.Config.Player1Type
                : brain.Config.Player2Type;

            if (currentPlayerType.IsAI)
            {
                Console.Clear();
                _ui.DrawBoard(brain, brain.Config);
                _ui.ShowTurnIndicator(brain.Config, brain.CurrentPlayer);
                Console.WriteLine("AI is thinking...");

                var ai = new MinimaxAI(currentPlayerType.Difficulty ?? EAIDifficulty.Medium);
                var col = ai.GetMove(brain, brain.CurrentPlayer);
                brain.MakeMove(col);

                if (brain.Config.Player1Type.IsAI && brain.Config.Player2Type.IsAI)
                    Thread.Sleep(500);
            }
            else
            {
                var input = _ui.GetPlayerMove(brain);
                switch (input.Action)
                {
                    case EPlayerAction.Move:
                        brain.MakeMove(input.Column);
                        break;
                    case EPlayerAction.Save:
                        SaveCurrentGame(brain);
                        break;
                    case EPlayerAction.Quit:
                        return;
                }
            }
        }

        Console.Clear();
        _ui.DrawBoard(brain, brain.Config);
        _ui.ShowGameOver(brain.Winner, brain.IsDraw, brain.Config);
        WaitForKey();
    }

    private void SaveCurrentGame(GameBrain brain)
    {
        Console.Write("Enter save name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Save cancelled.");
            WaitForKey();
            return;
        }

        var dto = GameStateMapper.ToDto(brain, name);
        _gameRepo.Save(dto);
        Console.WriteLine($"Game saved as '{name}'.");
        WaitForKey();
    }

    private void LoadAndContinueGame()
    {
        var saves = _gameRepo.List();
        if (saves.Count == 0)
        {
            Console.WriteLine("No saved games found.");
            WaitForKey();
            return;
        }

        var name = SelectFromList("Select a saved game:", saves.Select(s => s.Name).ToList());
        if (name is null) return;

        var dto = _gameRepo.Load(name);
        if (dto is null)
        {
            Console.WriteLine("Save not found.");
            WaitForKey();
            return;
        }

        var brain = GameStateMapper.ToDomain(dto);
        PlayGame(brain);
    }

    private void DeleteSavedGame()
    {
        var saves = _gameRepo.List();
        if (saves.Count == 0)
        {
            Console.WriteLine("No saved games found.");
            WaitForKey();
            return;
        }

        var name = SelectFromList("Select a game to delete:", saves.Select(s => s.Name).ToList());
        if (name is null) return;

        _gameRepo.Delete(name);
        Console.WriteLine($"Game '{name}' deleted.");
        WaitForKey();
    }

    private void SaveConfiguration()
    {
        Console.Write("Enter configuration name: ");
        var name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Save cancelled.");
            WaitForKey();
            return;
        }

        var config = _currentConfig with { Name = name };
        _configRepo.Save(config);
        Console.WriteLine($"Configuration saved as '{name}'.");
        WaitForKey();
    }

    private void LoadConfiguration()
    {
        var configs = _configRepo.List();
        if (configs.Count == 0)
        {
            Console.WriteLine("No configurations found.");
            WaitForKey();
            return;
        }

        var name = SelectFromList("Select a configuration:", configs.Select(c => c.Name).ToList());
        if (name is null) return;

        var config = _configRepo.Load(name);
        if (config is null)
        {
            Console.WriteLine("Configuration not found.");
            WaitForKey();
            return;
        }

        _currentConfig = config;
        Console.WriteLine($"Configuration '{name}' loaded.");
        WaitForKey();
    }

    private void DeleteConfiguration()
    {
        var configs = _configRepo.List();
        if (configs.Count == 0)
        {
            Console.WriteLine("No configurations found.");
            WaitForKey();
            return;
        }

        var name = SelectFromList("Select a configuration to delete:", configs.Select(c => c.Name).ToList());
        if (name is null) return;

        _configRepo.Delete(name);
        Console.WriteLine($"Configuration '{name}' deleted.");
        WaitForKey();
    }

    private void StartFromSavedConfig()
    {
        var configs = _configRepo.List();
        if (configs.Count == 0)
        {
            Console.WriteLine("No configurations found.");
            WaitForKey();
            return;
        }

        var name = SelectFromList("Select a configuration:", configs.Select(c => c.Name).ToList());
        if (name is null) return;

        var config = _configRepo.Load(name);
        if (config is null) return;

        StartNewGame(config);
    }

    private void ConfigurePlayerType(int playerNumber)
    {
        Console.WriteLine($"Configure Player {playerNumber}:");
        Console.WriteLine("  1. Human");
        Console.WriteLine("  2. AI - Easy");
        Console.WriteLine("  3. AI - Medium");
        Console.WriteLine("  4. AI - Hard");
        Console.Write("Select: ");

        var key = Console.ReadKey(true);
        Console.WriteLine();

        var playerType = key.KeyChar switch
        {
            '1' => new PlayerType(false),
            '2' => new PlayerType(true, EAIDifficulty.Easy),
            '3' => new PlayerType(true, EAIDifficulty.Medium),
            '4' => new PlayerType(true, EAIDifficulty.Hard),
            _ => null as PlayerType
        };

        if (playerType is null)
        {
            Console.WriteLine("Invalid selection.");
            WaitForKey();
            return;
        }

        _currentConfig = playerNumber == 1
            ? _currentConfig with { Player1Type = playerType }
            : _currentConfig with { Player2Type = playerType };
    }

    private void ChangeIntSetting(string name, int min, int max, Action<int> apply)
    {
        Console.Write($"Enter {name} ({min}-{max}): ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var value) && value >= min && value <= max)
        {
            apply(value);
        }
        else
        {
            Console.WriteLine($"Invalid value. Must be between {min} and {max}.");
            WaitForKey();
        }
    }

    private void ToggleTopology()
    {
        _currentConfig = _currentConfig with
        {
            Topology = _currentConfig.Topology == EBoardTopology.Rectangle
                ? EBoardTopology.Cylinder
                : EBoardTopology.Rectangle
        };
    }

    private void ViewCurrentSettings()
    {
        Console.Clear();
        Console.WriteLine("=== Current Settings ===");
        Console.WriteLine();
        Console.WriteLine($"  Board Size:     {_currentConfig.Columns} x {_currentConfig.Rows}");
        Console.WriteLine($"  Win Condition:  {_currentConfig.WinCondition} in a row");
        Console.WriteLine($"  Topology:       {_currentConfig.Topology}");
        Console.WriteLine();
        Console.WriteLine($"  Player 1:       {_currentConfig.Player1Name} ({_currentConfig.Player1Symbol}) - {FormatPlayerType(_currentConfig.Player1Type)}");
        Console.WriteLine($"  Player 2:       {_currentConfig.Player2Name} ({_currentConfig.Player2Symbol}) - {FormatPlayerType(_currentConfig.Player2Type)}");
        Console.WriteLine();
        WaitForKey();
    }

    private void ShowHowToPlay()
    {
        Console.Clear();
        Console.WriteLine("=== How to Play ===");
        Console.WriteLine();
        Console.WriteLine("Connect-X is a two-player connection game.");
        Console.WriteLine("Players take turns dropping pieces into columns.");
        Console.WriteLine("Pieces fall to the lowest available row.");
        Console.WriteLine("The first player to connect the required number");
        Console.WriteLine("of pieces in a row (horizontally, vertically, or");
        Console.WriteLine("diagonally) wins the game.");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  ← / →    Move column selector");
        Console.WriteLine("  Enter    Drop piece in selected column");
        Console.WriteLine("  S        Save current game");
        Console.WriteLine("  Q        Quit current game");
        Console.WriteLine();
        Console.WriteLine("Menu Controls:");
        Console.WriteLine("  ↑ / ↓    Navigate menu");
        Console.WriteLine("  Enter    Select item");
        Console.WriteLine("  1-9      Quick select by number");
        Console.WriteLine();
        WaitForKey();
    }

    private static string FormatPlayerType(PlayerType type) =>
        type.IsAI ? $"AI ({type.Difficulty})" : "Human";

    private static string? SelectFromList(string prompt, IReadOnlyList<string> items)
    {
        Console.Clear();
        Console.WriteLine(prompt);
        Console.WriteLine();

        for (var i = 0; i < items.Count; i++)
            Console.WriteLine($"  {i + 1}. {items[i]}");

        Console.WriteLine($"  {items.Count + 1}. Cancel");
        Console.Write("Select: ");

        var input = Console.ReadLine();
        if (int.TryParse(input, out var choice) && choice >= 1 && choice <= items.Count)
            return items[choice - 1];

        return null;
    }

    private static void WaitForKey()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(true);
    }
}
