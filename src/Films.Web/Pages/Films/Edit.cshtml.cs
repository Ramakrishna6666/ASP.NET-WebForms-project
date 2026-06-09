using Films.Application.DTOs;
using Films.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Films;

public class EditModel : PageModel
{
    private readonly IFilmService _filmService;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IFilmService filmService, ILogger<EditModel> logger)
    {
        _filmService = filmService;
        _logger = logger;
    }

    [BindProperty]
    public UpdateFilmDto Film { get; set; } = new UpdateFilmDto();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var film = await _filmService.GetFilmByIdAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            Film = new UpdateFilmDto
            {
                Id = film.Id,
                Title = film.Title,
                Description = film.Description,
                Year = film.Year,
                Genre = film.Genre
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving film for edit: {FilmId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _filmService.UpdateFilmAsync(Film);
            _logger.LogInformation("Film updated: {FilmId}", Film.Id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating film: {FilmId}", Film.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the film.");
            return Page();
        }
    }
}
