using Films.Application.DTOs;
using Films.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Films;

public class CreateModel : PageModel
{
    private readonly IFilmService _filmService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IFilmService filmService, ILogger<CreateModel> logger)
    {
        _filmService = filmService;
        _logger = logger;
    }

    [BindProperty]
    public CreateFilmDto Film { get; set; } = new CreateFilmDto();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _filmService.CreateFilmAsync(Film);
            _logger.LogInformation("Film created: {Title}", Film.Title);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating film: {Title}", Film.Title);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the film.");
            return Page();
        }
    }
}
