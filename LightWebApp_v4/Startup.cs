using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LightWebApp_v4.Startup))]
namespace LightWebApp_v4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
