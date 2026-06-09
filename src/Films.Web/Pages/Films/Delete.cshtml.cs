using Films.Application.DTOs;
using Films.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Films;

public class DeleteModel : PageModel
{
    private readonly IFilmService _filmService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IFilmService filmService, ILogger<DeleteModel> logger)
    {
        _filmService = filmService;
        _logger = logger;
    }

    public FilmDto? Film { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Film = await _filmService.GetFilmByIdAsync(id);
            if (Film == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving film for delete: {FilmId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _filmService.DeleteFilmAsync(id);
            _logger.LogInformation("Film deleted: {FilmId}", id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting film: {FilmId}", id);
            return RedirectToPage("./Index");
        }
    }
}
