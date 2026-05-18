using System.ComponentModel.DataAnnotations;
using Application.Config.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Configs;

public sealed class CreateModel : PageModel
{
    private readonly IConfigRepository _configRepo;

    [BindProperty]
    public ConfigInput Config { get; set; } = new();

    public CreateModel(IConfigRepository configRepo) => _configRepo = configRepo;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        if (Config.WinCondition > Config.Rows && Config.WinCondition > Config.Columns)
            ModelState.AddModelError("Config.WinCondition",
                "Win condition must not exceed both board dimensions.");

        if (!Enum.TryParse<EBoardTopology>(Config.Topology, out _))
            ModelState.AddModelError("Config.Topology", "Invalid topology value.");

        if (_configRepo.Load(Config.Name) is not null)
            ModelState.AddModelError("Config.Name", "A configuration with this name already exists.");

        if (!ModelState.IsValid)
            return Page();

        var topology = Enum.Parse<EBoardTopology>(Config.Topology);

        var gameConfig = new GameConfig(
            Config.Name,
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

public sealed class ConfigInput
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";

    [Range(3, 20)]
    public int Rows { get; set; } = 6;

    [Range(3, 20)]
    public int Columns { get; set; } = 7;

    [Range(3, 10)]
    public int WinCondition { get; set; } = 4;

    [Required]
    public string Topology { get; set; } = "Rectangle";

    public bool Player1IsAI { get; set; }
    public string? Player1Difficulty { get; set; }
    public bool Player2IsAI { get; set; }
    public string? Player2Difficulty { get; set; }
}
