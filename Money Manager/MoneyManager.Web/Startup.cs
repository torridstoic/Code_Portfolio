using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MoneyManager.Web.Startup))]
namespace MoneyManager.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
