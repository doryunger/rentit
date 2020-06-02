using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RentIt.Startup))]
namespace RentIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
