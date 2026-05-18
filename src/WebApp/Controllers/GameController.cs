using Application.AI;
using Application.Config.Interfaces;
using Application.Game.Dto;
using Application.Game.Interfaces;
using Application.Game.Mapping;
using Domain;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public sealed class GameController : Controller
{
    private const string SessionKey = "CurrentGame";

    private readonly IConfigRepository _configRepository;
    private readonly IGameRepository _gameRepository;

    public GameController(IConfigRepository configRepository, IGameRepository gameRepository)
    {
        _configRepository = configRepository;
        _gameRepository = gameRepository;
    }

    public IActionResult New()
    {
        var configs = _configRepository.List();
        return View(configs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Start(string configName)
    {
        var config = _configRepository.Load(configName);
        if (config is null)
            return RedirectToAction(nameof(New));

        var brain = new GameBrain(config);
        SaveToSession(brain);

        if (config.Player1Type.IsAI && config.Player2Type.IsAI)
            return RedirectToAction(nameof(RunAiGame));

        if (config.Player1Type.IsAI)
        {
            RunAiMove(brain);
            SaveToSession(brain);
        }

        return RedirectToAction(nameof(Play));
    }

    public IActionResult Play()
    {
        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        if (brain.IsGameOver)
            return RedirectToAction(nameof(GameOver));

        return View(BuildViewModel(brain));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Move(int column)
    {
        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        if (brain.IsGameOver)
            return RedirectToAction(nameof(GameOver));

        if (!brain.MakeMove(column))
        {
            return View(nameof(Play), BuildViewModel(brain, "That column is full. Choose another."));
        }

        if (!brain.IsGameOver)
        {
            var nextPlayerType = brain.CurrentPlayer == 0
                ? brain.Config.Player1Type
                : brain.Config.Player2Type;

            if (nextPlayerType.IsAI)
            {
                RunAiMove(brain);
            }
        }

        SaveToSession(brain);

        if (brain.IsGameOver)
            return RedirectToAction(nameof(GameOver));

        return RedirectToAction(nameof(Play));
    }

    public IActionResult RunAiGame()
    {
        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        var maxMoves = brain.Rows * brain.Columns;
        var moves = 0;
        while (!brain.IsGameOver && moves++ < maxMoves)
        {
            RunAiMove(brain);
        }

        SaveToSession(brain);
        return RedirectToAction(nameof(GameOver));
    }

    public IActionResult GameOver()
    {
        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        return View(BuildViewModel(brain));
    }

    public IActionResult Save()
    {
        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName) || saveName.Length > 100)
            return BadRequest();

        var brain = LoadFromSession();
        if (brain is null)
            return RedirectToAction(nameof(New));

        var dto = GameStateMapper.ToDto(brain, saveName.Trim());
        _gameRepository.Save(dto);

        TempData["Message"] = $"Game saved as \"{saveName.Trim()}\".";
        return RedirectToAction(nameof(Play));
    }

    public IActionResult Load()
    {
        var saves = _gameRepository.List();
        return View(saves);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Resume(string saveName)
    {
        var dto = _gameRepository.Load(saveName);
        if (dto is null)
            return RedirectToAction(nameof(Load));

        var brain = GameStateMapper.ToDomain(dto);
        SaveToSession(brain);

        return RedirectToAction(nameof(Play));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteSave(string saveName)
    {
        _gameRepository.Delete(saveName);
        return RedirectToAction(nameof(Load));
    }

    private void SaveToSession(GameBrain brain)
    {
        var dto = GameStateMapper.ToDto(brain, "_session");
        HttpContext.Session.Set(SessionKey, dto);
    }

    private GameBrain? LoadFromSession()
    {
        var dto = HttpContext.Session.Get<GameStateDto>(SessionKey);
        return dto is null ? null : GameStateMapper.ToDomain(dto);
    }

    private static void RunAiMove(GameBrain brain)
    {
        var playerType = brain.CurrentPlayer == 0
            ? brain.Config.Player1Type
            : brain.Config.Player2Type;

        if (!playerType.IsAI || playerType.Difficulty is null)
            return;

        var ai = new MinimaxAI(playerType.Difficulty.Value);
        var column = ai.GetMove(brain, brain.CurrentPlayer);
        brain.MakeMove(column);
    }

    private static PlayViewModel BuildViewModel(GameBrain brain, string? message = null)
    {
        var board = new int?[brain.Rows][];
        for (var r = 0; r < brain.Rows; r++)
        {
            board[r] = new int?[brain.Columns];
            for (var c = 0; c < brain.Columns; c++)
                board[r][c] = brain.GetCell(r, c);
        }

        return new PlayViewModel
        {
            Board = board,
            Rows = brain.Rows,
            Columns = brain.Columns,
            CurrentPlayer = brain.CurrentPlayer,
            Config = brain.Config,
            IsGameOver = brain.IsGameOver,
            Winner = brain.Winner,
            IsDraw = brain.IsDraw,
            Message = message,
        };
    }
}
