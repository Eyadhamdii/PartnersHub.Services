using PartnersHub.InnovationHub.Application.Common.Interfaces;

namespace PartnersHub.InnovationHub.Apis.Common
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string UserId => _contextAccessor?.HttpContext?.User.FindFirst("ContactId")?.Value ?? Guid.NewGuid().ToString();
    }
}
