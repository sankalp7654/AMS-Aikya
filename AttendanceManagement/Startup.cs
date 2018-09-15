using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AttendanceManagement.Startup))]
namespace AttendanceManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
