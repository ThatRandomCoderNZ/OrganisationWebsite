using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OrganisationWebsite.Startup))]
namespace OrganisationWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
