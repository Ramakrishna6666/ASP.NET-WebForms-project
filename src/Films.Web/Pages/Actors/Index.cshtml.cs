using Films.Application.DTOs;
using Films.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Films.Web.Pages.Actors;

public class IndexModel : PageModel
{
    private readonly IActorService _actorService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IActorService actorService, ILogger<IndexModel> logger)
    {
        _actorService = actorService;
        _logger = logger;
    }

    public IEnumerable<ActorDto> Actors { get; set; } = new List<ActorDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Actors = await _actorService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading actors");
            Actors = new List<ActorDto>();
        }
    }
}
