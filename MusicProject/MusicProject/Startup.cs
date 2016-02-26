using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MusicProject.Startup))]
namespace MusicProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
