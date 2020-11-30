using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Moviez_.Startup))]
namespace Moviez_
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
