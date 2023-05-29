using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using softaware.Authentication.Hmac;
using softaware.Authentication.Hmac.AspNetCore;
using softaware.Authentication.Hmac.AuthorizationProvider;
using System.Linq;

namespace YourWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {



            services.AddControllers();
            services.AddSwaggerGen();

            var hmacAuthenticatedApps = this.Configuration.GetSection("Authentication").GetSection("HmacAuthenticatedApps").Get<HmacAuthenticationClientConfiguration[]>().ToDictionary(e => e.AppId, e => e.ApiKey);

            services
                    .AddAuthentication(o =>
                    {
                        o.DefaultScheme = HmacAuthenticationDefaults.AuthenticationScheme;
                    })
                     .AddHmacAuthentication(HmacAuthenticationDefaults.AuthenticationScheme, "HMAC Authentication", options => { });

            // adding the memory cache
            services.AddMemoryCache();

            // Add HMAC authentication handler as a transient service
            services.AddTransient<IHmacAuthorizationProvider>(_ => new MemoryHmacAuthenticationProvider(hmacAuthenticatedApps));
            //   services.AddTransient<HmacAuthenticationHandler>();

            //services.AddControllers();
            //services.AddSwaggerGen();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add HMAC authentication handler as a global message handler
            //    app.UseMiddleware<HmacAuthenticationHandler>();

            // Use the HmacAuthenticationHandler as a global message handler

            /*

            app.Use(async (context, next) =>
            {
                // Create a scope to resolve dependencies
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    // Retrieve an instance of the HmacAuthenticationHandler from the dependency injection container
                    var handler = scope.ServiceProvider.GetService<HmacAuthenticationHandler>();

                    // Call the SendAsync method of the handler to perform HMAC authentication
                    await handler.SendAsync(context.Request.PathBase, context.RequestAborted);

                    // Call the next middleware in the pipeline
                    await next();
                }
            });
            */
            app.UseSwagger();
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }







}
