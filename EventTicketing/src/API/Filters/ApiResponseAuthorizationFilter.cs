using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EventTicketing.Application.DTOs;
using ApplicationDto = EventTicketing.Application.DTOs;

namespace EventTicketing.API.Filters;

public class ApiResponseAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var result = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
            if (result != null)
            {
                var requiredRoles = result.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToList();
                
                if (requiredRoles != null && requiredRoles.Any())
                {
                    var hasRequiredRole = requiredRoles.Any(role => 
                        context.HttpContext.User.IsInRole(role));
                    
                    if (!hasRequiredRole)
                    {
                        context.Result = new ObjectResult(new ApplicationDto.ApiResponse<object>
                        {
                            Success = false,
                            Message = "Access forbidden - Insufficient permissions",
                            Status = ResponseStatus.Forbidden,
                            Errors = new List<ApiError>(),
                            Timestamp = DateTime.UtcNow,
                            Path = context.HttpContext.Request.Path
                        })
                        {
                            StatusCode = StatusCodes.Status403Forbidden
                        };
                    }
                }
            }
        }
    }
}