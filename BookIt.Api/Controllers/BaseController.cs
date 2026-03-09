using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookIt.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)
                || int.TryParse(userIdString, out int id) == false)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }
            return id;
        }
    }
}
