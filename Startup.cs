using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace middleware
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //custum middleware using extension method 
            app.CustomRun(async (context) =>
            {
                await context.Response.WriteAsync("middleware response runcustom says  hello world!!");
            });
            app.Run(async (context)=>
            {  
                await context.Response.WriteAsync("middleware response run says hello helloword!!"); 
            });
            //custom middleware implemenation using class based implementation
            app.UseSampleMiddleware();

            //conditional pipeline using Map()
            app.UseSampleMiddlewareMap();

            //conditional pipeline using Mapwhen()
            app.UseSampleMiddlewareMapWhen();
        }
    }

    //the app.run method encapsulaate the app.use
    public static class RunExtensions
    { 
        public static void CustomRun(this IApplicationBuilder app,
        RequestDelegate handler)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            app.Use(_ => handler);
        }
    }



    //class based middleware for creating custom middleware
    public class SampleMiddleWare
    {
        private readonly RequestDelegate _next;
        public SampleMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //do logic 
            await _next(context);
        }
    }
    
    // encapsulate the samplemidleware   
    public static class SampleBuilderExtension
    {
        public static  IApplicationBuilder UseSampleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SampleMiddleWare>();
        }
    }



    /// <summary>
    /// conditional pipeline with middleware 
    /// </summary>
    
     //map(): we only be added if the uri matches this path
    public static class SampleMiddlewareCondtionalOperatorMapExtensions
    {
        public static IApplicationBuilder UseSampleMiddlewareMap(
            this IApplicationBuilder builder)
        {
            return builder.Map("/test/1", _ =>
            _.UseMiddleware<SampleMiddleWare>());
        }
    }

    //mapwhen()
    public static class SampleMiddlewareConditionalOperatorMapWhenExtensions
    {
        public static IApplicationBuilder UseSampleMiddlewareMapWhen(
            this IApplicationBuilder builder)
        {
            return builder.MapWhen(context => context.Request.IsHttps,
            _ => _.UseMiddleware<SampleMiddleWare>());
        }
    }
}
