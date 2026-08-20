using Films.Application.DTOs;
using Films.Application.Interfaces;
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

    public async Task OnGetAsync()
    {
        try
        {
            Films = await _filmService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading films");
            Films = new List<FilmDto>();
        }
    }
}
