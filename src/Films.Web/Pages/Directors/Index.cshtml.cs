using Films.Application.DTOs;
using Films.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Directors;

public class IndexModel : PageModel
{
    private readonly IDirectedByService _directorService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IDirectedByService directorService, ILogger<IndexModel> logger)
    {
        _directorService = directorService;
        _logger = logger;
    }

    public IEnumerable<DirectedByDto> Directors { get; set; } = new List<DirectedByDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Directors = await _directorService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading directors");
            Directors = new List<DirectedByDto>();
        }
    }
}
