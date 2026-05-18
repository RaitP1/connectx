using Application.AI;
using Application.Config.Interfaces;
using Application.Game.Interfaces;
using Application.Game.Mapping;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Games;

public sealed class NewModel : PageModel
{
    private readonly IConfigRepository _configRepo;
    private readonly IGameRepository _gameRepo;

    public IReadOnlyList<GameConfig> Configs { get; private set; } = [];

    public NewModel(IConfigRepository configRepo, IGameRepository gameRepo)
    {
        _configRepo = configRepo;
        _gameRepo = gameRepo;
    }

    public void OnGet()
    {
        Configs = _configRepo.List();
    }

    public IActionResult OnPost(string configName)
    {
        var config = _configRepo.Load(configName);
        if (config is null)
            return RedirectToPage();

        var brain = new GameBrain(config);
        var gameId = Guid.NewGuid().ToString("N")[..8];

        if (config.Player1Type.IsAI && config.Player2Type.IsAI)
        {
            RunAiVsAi(brain);
        }

        var dto = GameStateMapper.ToDto(brain, gameId);
        _gameRepo.Save(dto);

        HttpContext.Session.SetString($"game_{gameId}_slot", "0");

        return RedirectToPage("Play", new { gameId });
    }

    private static void RunAiVsAi(GameBrain brain)
    {
        var maxMoves = brain.Config.Rows * brain.Config.Columns;
        var moves = 0;
        while (!brain.IsGameOver && moves++ < maxMoves)
        {
            var playerType = brain.CurrentPlayer == 0
                ? brain.Config.Player1Type
                : brain.Config.Player2Type;

            var ai = new MinimaxAI(playerType.Difficulty ?? EAIDifficulty.Medium);
            var col = ai.GetMove(brain, brain.CurrentPlayer);
            brain.MakeMove(col);
        }
    }
}
