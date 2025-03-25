using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

[ApiController]
[Route("api/user/wishlist")]
[Authorize(Policy = "ShopperOnly")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        return !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId);
    }

    [HttpPost]
    public async Task<IActionResult> GetWishlist()
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = ValidationMessages.MESAGGE_ID_NOT_FOUND });
        }

        var response = await _wishlistService.GetWishlist(userId);
        return Ok(response);
    }

    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = ValidationMessages.MESAGGE_ID_NOT_FOUND });
        }

        var response = await _wishlistService.AddToWishlist(userId, productId);
        return Ok(response);
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized(new { message = ValidationMessages.MESAGGE_ID_NOT_FOUND });
        }

        var response = await _wishlistService.RemoveFromWishlist(userId, productId);
        return Ok(response);
    }
}
