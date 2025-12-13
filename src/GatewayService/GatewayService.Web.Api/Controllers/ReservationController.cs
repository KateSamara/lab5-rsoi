using System.IdentityModel.Tokens.Jwt;
using GatewayService.Domain.Interfaces.Services;
using GatewayService.Web.Dto.Converters;
using GatewayService.Web.Dto.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Web.Api.Controllers;

[ApiController]
[Route("/api/v1/reservations")]
[ServiceFilter(typeof(ValidationFilterAttribute))]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService ?? throw new ArgumentNullException(nameof(reservationService));
    }
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReservationsByUsernameAsync()
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtToken);
        var username = jwt.Claims.First(c => c.Type == "sub").Value;
        
        var reservations = await _reservationService.GetReservationsByUsernameAsync(username, jwtToken);
        
        return Ok(reservations.ConvertAll(r => r.ToDto()));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationCreateDto reservationCreate)
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtToken);
        var username = jwt.Claims.First(c => c.Type == "sub").Value;
        
        var newReservation = await _reservationService.CreateReservationAsync(username, reservationCreate.ToDomain(), jwtToken);
        
        if (newReservation == null)
            return StatusCode(StatusCodes.Status403Forbidden);
        
        return Ok(newReservation.ToDto());
    }

    [HttpPost("{reservationId}/return")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteReservationAsync([FromRoute] Guid reservationId,
        [FromBody] ReservationDeleteDto reservationDelete)
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtToken);
        var username = jwt.Claims.First(c => c.Type == "sub").Value;
        
        var isDeleted = await _reservationService.DeleteReservationAsync(username, reservationId, reservationDelete.ToDomain(), jwtToken);
        if (!isDeleted)
            return StatusCode(StatusCodes.Status404NotFound, new { message = "Reservation not found." });
        
        return NoContent();
    }
}