using Films.Application.DTOs;
using Films.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Films;

public class IndexModel : PageModel
{
    private readonly IFilmService _filmService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IFilmService filmService, ILogger<IndexModel> logger)
    {
        _filmService = filmService;
        _logger = logger;
    }

    public IEnumerable<FilmDto> Films { get; set; } = new List<FilmDto>();
    
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Films = await _filmService.SearchFilmsAsync(SearchTerm);
                _logger.LogInformation("Searched films with term: {SearchTerm}", SearchTerm);
            }
            else
            {
                Films = await _filmService.GetAllFilmsAsync();
                _logger.LogInformation("Retrieved all films");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving films");
            Films = new List<FilmDto>();
        }
    }
}
