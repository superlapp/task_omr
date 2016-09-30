using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(task_omr.Startup))]
namespace task_omr
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
