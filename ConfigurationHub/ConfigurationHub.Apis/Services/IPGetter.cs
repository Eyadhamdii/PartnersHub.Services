namespace PartnersHub.ConfigurationHub.Apis.Middlewares {
    public class IPGetter {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IPGetter(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIp() {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            return ipAddress.MapToIPv4().ToString();
        }
    }
}