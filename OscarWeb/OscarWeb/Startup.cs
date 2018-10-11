using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using ApplicationEnvironment = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment;

using OscarWeb.Models;

using Newtonsoft.Json.Serialization;

namespace OscarWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;

            //Add support for reference assemblies
            var mvcBuilder = serviceProvider.GetService<IMvcBuilder>();
            new MvcConfiguration().ConfigureMvc(mvcBuilder);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddAzureAdB2C(options => Configuration.Bind("AzureAdB2C", options))
                .AddCookie();

            //ensure the user must be authenticated before they can access any of the pages from the app
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/");
                })
                .AddJsonOptions(options =>
                options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //restrict AJAX client requests for added security
            services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                //uncomment this line to test the Session variables and timeout
                //options.IdleTimeout = TimeSpan.FromSeconds(30);

                //prevent session storage from being accessed from client script 
                //i.e. only server side code (added security) 
                options.Cookie.HttpOnly = true;
            });

            //bind our appsettings sections to our models
            services.Configure<WebServicesModel>(Configuration.GetSection("WebServices"));
            
            // Add Kendo UI services to the services container
            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            { 
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// Class added to provide support for reference assemblies such as Common.dll
    /// See https://github.com/dotnet/core-setup/issues/2981 for more details.
    /// </summary>
    public class MvcConfiguration : IDesignTimeMvcBuilderConfiguration
    {
        private class DirectReferenceAssemblyResolver : ICompilationAssemblyResolver
        {
            public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
            {
                if (!string.Equals(library.Type, "reference", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var paths = new List<string>();

                foreach (var assembly in library.Assemblies)
                {
                    var path = Path.Combine(ApplicationEnvironment.ApplicationBasePath, assembly);

                    if (!File.Exists(path))
                    {
                        return false;
                    }
                    paths.Add(path);
                }
                assemblies.AddRange(paths);
                return true;
            }
        }

        public void ConfigureMvc(IMvcBuilder builder)
        {
            // .NET Core SDK v1 does not pick up reference assemblies so
            // they have to be added for Razor manually. Resolved for
            // SDK v2 by https://github.com/dotnet/sdk/pull/876 OR SO WE THOUGHT
            /*builder.AddRazorOptions(razor =>
            {
                razor.AdditionalCompilationReferences.Add(
                    MetadataReference.CreateFromFile(
                        typeof(PdfHttpHandler).Assembly.Location));
            });*/

            // .NET Core SDK v2 does not resolve reference assemblies' paths
            // at all, so we have to hack around with reflection
            typeof(CompilationLibrary)
                .GetTypeInfo()
                .GetDeclaredField("<DefaultResolver>k__BackingField")
                .SetValue(null, new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
                {
                    new DirectReferenceAssemblyResolver(),
                    new AppBaseCompilationAssemblyResolver(),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver(),
                }));
        }
    }
}
