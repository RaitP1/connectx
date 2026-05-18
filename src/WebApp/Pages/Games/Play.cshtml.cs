using Application.AI;
using Application.Game.Interfaces;
using Application.Game.Mapping;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Games;

public sealed class PlayModel : PageModel
{
    private readonly IGameRepository _gameRepo;

    public GameBrain? Brain { get; private set; }
    public string GameId { get; private set; } = "";
    public int MySlot { get; private set; }
    public string? Message { get; private set; }
    public int? LastMoveRow { get; private set; }
    public int? LastMoveCol { get; private set; }

    public PlayModel(IGameRepository gameRepo) => _gameRepo = gameRepo;

    public IActionResult OnGet(string gameId)
    {
        if (!LoadGame(gameId))
            return RedirectToPage("List");

        if (TempData["LastMoveRow"] is int row && TempData["LastMoveCol"] is int col)
        {
            LastMoveRow = row;
            LastMoveCol = col;
        }

        return Page();
    }

    public IActionResult OnPost(string gameId, int column)
    {
        if (!LoadGame(gameId))
            return RedirectToPage("List");

        if (Brain!.IsGameOver)
        {
            Message = "Game is already over.";
            return Page();
        }

        if (Brain.CurrentPlayer != MySlot)
        {
            Message = "It is not your turn.";
            return Page();
        }

        if (!Brain.MakeMove(column))
        {
            Message = "Invalid move.";
            return Page();
        }

        FindLastMove(column);

        if (!Brain.IsGameOver)
        {
            var nextPlayerType = Brain.CurrentPlayer == 0
                ? Brain.Config.Player1Type
                : Brain.Config.Player2Type;

            if (nextPlayerType.IsAI)
            {
                var ai = new MinimaxAI(nextPlayerType.Difficulty ?? EAIDifficulty.Medium);
                var aiCol = ai.GetMove(Brain, Brain.CurrentPlayer);
                Brain.MakeMove(aiCol);
                FindLastMove(aiCol);
            }
        }

        var dto = GameStateMapper.ToDto(Brain, gameId);
        _gameRepo.Save(dto);

        TempData["LastMoveRow"] = LastMoveRow;
        TempData["LastMoveCol"] = LastMoveCol;

        return RedirectToPage(new { gameId });
    }

    public IActionResult OnPostSave(string gameId, string saveName)
    {
        if (!LoadGame(gameId))
            return RedirectToPage("List");

        if (string.IsNullOrWhiteSpace(saveName))
        {
            Message = "Save name cannot be empty.";
            return Page();
        }

        var dto = GameStateMapper.ToDto(Brain!, saveName);
        _gameRepo.Save(dto);
        Message = $"Game saved as '{saveName}'.";

        return Page();
    }

    public IActionResult OnGetPoll(string gameId)
    {
        var state = _gameRepo.Load(gameId);
        if (state is null)
            return new JsonResult(new { error = "Game not found" });

        var brain = GameStateMapper.ToDomain(state);

        return new JsonResult(new
        {
            currentPlayer = brain.CurrentPlayer,
            gameOver = brain.IsGameOver,
            winner = brain.Winner,
            isDraw = brain.IsDraw,
        });
    }

    private bool LoadGame(string gameId)
    {
        GameId = gameId;
        var state = _gameRepo.Load(gameId);
        if (state is null)
            return false;

        Brain = GameStateMapper.ToDomain(state);
        var slotStr = HttpContext.Session.GetString($"game_{gameId}_slot");
        MySlot = int.TryParse(slotStr, out var parsed) ? parsed : -1;
        return true;
    }

    private void FindLastMove(int column)
    {
        LastMoveCol = column;
        for (var row = 0; row < Brain!.Rows; row++)
        {
            if (Brain.GetCell(row, column) is not null)
            {
                LastMoveRow = row;
                break;
            }
        }
    }
}
