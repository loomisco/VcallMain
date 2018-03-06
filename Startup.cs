using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vcall.Startup))]
namespace Vcall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
