using Application.Game.Dto;
using Application.Game.Interfaces;
using Application.Game.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Games;

public sealed class ListModel : PageModel
{
    private readonly IGameRepository _gameRepo;

    public IReadOnlyList<GameListItem> Games { get; private set; } = [];

    public ListModel(IGameRepository gameRepo) => _gameRepo = gameRepo;

    public void OnGet()
    {
        Games = _gameRepo.List().Select(dto =>
        {
            var isOver = false;
            try { isOver = GameStateMapper.ToDomain(dto).IsGameOver; }
            catch { /* corrupted state — treat as not over */ }
            return new GameListItem(dto, isOver);
        }).ToList();
    }

    public IActionResult OnPostJoin(string name)
    {
        var game = _gameRepo.Load(name);
        if (game is null)
            return RedirectToPage();

        var brain = GameStateMapper.ToDomain(game);
        if (brain.IsGameOver)
            return RedirectToPage();

        HttpContext.Session.SetString($"game_{name}_slot", "1");
        return RedirectToPage("Play", new { gameId = name });
    }

    public IActionResult OnPostResume(string name)
    {
        var game = _gameRepo.Load(name);
        if (game is null)
            return RedirectToPage();

        var existingSlot = HttpContext.Session.GetString($"game_{name}_slot");
        if (existingSlot is null)
            HttpContext.Session.SetString($"game_{name}_slot", "0");

        return RedirectToPage("Play", new { gameId = name });
    }

    public IActionResult OnPostDelete(string name)
    {
        _gameRepo.Delete(name);
        return RedirectToPage();
    }
}

public sealed record GameListItem(GameStateDto State, bool IsGameOver);
