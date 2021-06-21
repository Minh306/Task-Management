using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;
using Autofac.Extensions.DependencyInjection;

[assembly: OwinStartupAttribute(typeof(TaskManagement.Startup))]
namespace TaskManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
            //var builder = new ContainerBuilder();

            //// STANDARD MVC SETUP:

            //// Register your MVC controllers.
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //// Run other optional steps, like registering model binders,
            //// web abstractions, etc., then set the dependency resolver
            //// to be Autofac.
            //var container = builder.Build();
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //// OWIN MVC SETUP:

            //// Register the Autofac middleware FIRST, then the Autofac MVC middleware.
            //app.UseAutofacMiddleware(container);
            //app.UseAutofacMvc();
        }
    }
}
