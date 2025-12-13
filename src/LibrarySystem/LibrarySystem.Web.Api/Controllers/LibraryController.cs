using LibrarySystem.Domain.Interfaces.Services;
using LibrarySystem.Domain.Models;
using LibrarySystem.Domain.Models.Books;
using LibrarySystem.Domain.Models.Libraries;
using LibrarySystem.Web.Dto;
using LibrarySystem.Web.Dto.Converters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Web.Api.Controllers;

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
        var libraryRequest = new LibraryRequest
        {
            Page = page,
            Size = size,
            City = city
        };
        
        var librariesPaged = await _libraryService.GetLibrariesPagedAsync(libraryRequest);
        
        return Ok(librariesPaged.ToDto());
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
        var bookRequest = new BookRequest
        {
            Page = page,
            Size = size,
            LibraryUuid = libraryUid,
            ShowAll = showAll
        };
        
        var booksPaged = await _libraryService.GetBookPagedByLibraryUuidAsync(bookRequest);
        
        return Ok(booksPaged.ToDto());
    }
    
    [HttpGet("ids")]
    [Authorize]
    [ProducesResponseType(typeof(List<LibraryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLibrariesByIdsAsync([FromQuery] List<Guid> ids)
    {
        var libraries = await _libraryService.GetLibrariesByIdsAsync(ids);
        
        return Ok(libraries.ConvertAll(l => l.ToDto()));
    }

    [HttpPatch("{libraryUid}/books/{bookUid}")]
    [Authorize]
    [ProducesResponseType(typeof(LibraryBookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBookCountAsync([FromRoute] Guid libraryUid, [FromRoute] Guid bookUid,
        [FromQuery] bool isIncrease)
    {
        var libraryBook = await _libraryService.ChangeBookCountAsync(libraryUid, bookUid, isIncrease);
        
        return Ok(libraryBook.ToDto());
    }
}