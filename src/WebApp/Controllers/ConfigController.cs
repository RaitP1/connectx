using Application.Config.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public sealed class ConfigController : Controller
{
    private readonly IConfigRepository _configRepository;

    public ConfigController(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public IActionResult Index()
    {
        var configs = _configRepository.List();
        return View(configs);
    }

    public IActionResult Create() => View(new ConfigFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ConfigFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var config = model.ToGameConfig();
        if (!config.IsValid())
        {
            ModelState.AddModelError("", "Invalid configuration. Check that win condition does not exceed board dimensions and symbols differ.");
            return View(model);
        }

        _configRepository.Save(config);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(string id)
    {
        var config = _configRepository.Load(id);
        if (config is null)
            return NotFound();

        return View(ConfigFormModel.FromGameConfig(config));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(string id, ConfigFormModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var config = model.ToGameConfig();
        if (!config.IsValid())
        {
            ModelState.AddModelError("", "Invalid configuration. Check that win condition does not exceed board dimensions and symbols differ.");
            return View(model);
        }

        if (id != model.Name)
            _configRepository.Delete(id);

        _configRepository.Save(config);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string id)
    {
        _configRepository.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
