using System.IdentityModel.Tokens.Jwt;
using GatewayService.Domain.Interfaces.Services;
using GatewayService.Web.Dto.Converters;
using GatewayService.Web.Dto.Ratings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Web.Api.Controllers;

[ApiController]
[Route("/api/v1/rating")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;
    
    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<RatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRatingsByUsernameAsync()
    {
        var jwtToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(jwtToken);
        var username = jwt.Claims.First(c => c.Type == "sub").Value;
        
        var rating = await _ratingService.GetRatingsByUsernameAsync(username, jwtToken);
        
        return Ok(rating.ToDto());
    }
}