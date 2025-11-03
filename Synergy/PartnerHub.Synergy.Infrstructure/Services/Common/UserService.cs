using Microsoft.AspNetCore.Http;
using PartnersHub.Synergy.Application.Interfaces.Common;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _currentUserId;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCurrentUserId(Guid userId)
    {
        _currentUserId = userId;
    }

    public Guid CurrentUserId
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }
            var listt = context.User.Claims.ToList();
            var userId = context?.User?.Claims?
                .Where(c => c.Type == "ContactID")
                .Select(c => Guid.Parse(c.Value))
                .SingleOrDefault();
            return userId ?? Guid.NewGuid();
        }
    }
    public string Name
    {
        get
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }
            var listt = context.User.Claims.ToList();
            var name = context?.User?.Claims?
                .Where(c => c.Type == "name")
                .Select(c => c.Value)
                .SingleOrDefault();
            return name ?? string.Empty;
        }
    }
}
