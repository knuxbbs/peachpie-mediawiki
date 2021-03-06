using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;

namespace Website.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = System.IO.Directory.GetCurrentDirectory() + "/../mediawiki";

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5004/")
                .UseWebRoot(root)
                .UseContentRoot(root)
                .Build();

            host.Run();
        }
    }
	
	class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            var rewriteOptions = new RewriteOptions()
                .AddRewrite(@"^index.php/(.+)", "index.php?title=$1", skipRemainingRules: true);
            app.UseRewriter(rewriteOptions);

            app.UseSession();

            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "mediawiki"));

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}