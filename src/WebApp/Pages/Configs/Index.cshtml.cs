using Application.Config.Interfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Configs;

public sealed class IndexModel : PageModel
{
    private readonly IConfigRepository _configRepo;

    public IReadOnlyList<GameConfig> Configs { get; private set; } = [];

    public IndexModel(IConfigRepository configRepo) => _configRepo = configRepo;

    public void OnGet()
    {
        Configs = _configRepo.List();
    }

    public IActionResult OnPostDelete(string name)
    {
        _configRepo.Delete(name);
        return RedirectToPage();
    }
}
