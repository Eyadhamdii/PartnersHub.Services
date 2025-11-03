namespace PartnersHub.Communities.Apis.Controllers
{
    public interface IModule
    {
        static abstract void AddRoutes(IEndpointRouteBuilder app);
    }
}
