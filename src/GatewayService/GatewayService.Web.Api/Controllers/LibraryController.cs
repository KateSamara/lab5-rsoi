using GatewayService.Domain.Interfaces.Services;
using GatewayService.Web.Dto.Books;
using GatewayService.Web.Dto.Converters;
using GatewayService.Web.Dto.Libraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Web.Api.Controllers;

[ApiController]
[Route("/api/v1/libraries")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
       _libraryService = libraryService ?? throw new ArgumentNullException(nameof(libraryService));
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(LibraryPagedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLibrariesPagedAsync([FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string city)
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        var libraries = await _libraryService.GetLibrariesByCityPagedAsync(page, size, city, jwtToken);
        
        return Ok(libraries.ToDto());
    }
    
    [HttpGet("{libraryUid}/books")]
    [Authorize]
    [ProducesResponseType(typeof(BookPagedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBooksPagedByLibraryUuid([FromRoute] Guid libraryUid,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] bool showAll)
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        
        var books = await _libraryService.GetBooksPagedByLibraryUuid(libraryUid, page, size, showAll, jwtToken);
        
        return Ok(books.ToDto());
    }
}