using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{idd}",
                defaults: new { controller = "Redirect", action = "Index", id = UrlParameter.Optional , idd= UrlParameter.Optional}
            );
        }
    }
}