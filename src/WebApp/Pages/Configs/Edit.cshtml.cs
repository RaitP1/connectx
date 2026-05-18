using Application.Config.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Configs;

public sealed class EditModel : PageModel
{
    private readonly IConfigRepository _configRepo;

    [BindProperty]
    public ConfigInput Config { get; set; } = new();

    public EditModel(IConfigRepository configRepo) => _configRepo = configRepo;

    public IActionResult OnGet(string name)
    {
        var existing = _configRepo.Load(name);
        if (existing is null)
            return RedirectToPage("Index");

        Config = new ConfigInput
        {
            Name = existing.Name,
            Rows = existing.Rows,
            Columns = existing.Columns,
            WinCondition = existing.WinCondition,
            Topology = existing.Topology.ToString(),
            Player1IsAI = existing.Player1Type.IsAI,
            Player1Difficulty = existing.Player1Type.Difficulty?.ToString(),
            Player2IsAI = existing.Player2Type.IsAI,
            Player2Difficulty = existing.Player2Type.Difficulty?.ToString(),
        };

        return Page();
    }

    public IActionResult OnPost(string name)
    {
        if (Config.WinCondition > Config.Rows && Config.WinCondition > Config.Columns)
            ModelState.AddModelError("Config.WinCondition",
                "Win condition must not exceed both board dimensions.");

        if (!Enum.TryParse<EBoardTopology>(Config.Topology, out _))
            ModelState.AddModelError("Config.Topology", "Invalid topology value.");

        if (!ModelState.IsValid)
            return Page();

        var topology = Enum.Parse<EBoardTopology>(Config.Topology);

        _configRepo.Delete(name);

        var gameConfig = new GameConfig(
            name,
            Config.Rows,
            Config.Columns,
            Config.WinCondition,
            "Player 1", "X", "Player 2", "O",
            topology,
            ParsePlayerType(Config.Player1IsAI, Config.Player1Difficulty),
            ParsePlayerType(Config.Player2IsAI, Config.Player2Difficulty));

        _configRepo.Save(gameConfig);
        return RedirectToPage("Index");
    }

    private static PlayerType ParsePlayerType(bool isAI, string? difficulty)
    {
        if (!isAI) return new PlayerType(false);
        var diff = Enum.TryParse<EAIDifficulty>(difficulty, out var d) ? d : EAIDifficulty.Medium;
        return new PlayerType(true, diff);
    }
}
