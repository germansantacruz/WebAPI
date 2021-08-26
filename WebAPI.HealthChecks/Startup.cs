using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebAPI.HealthChecks.DependencyInjection;

namespace WebAPI.HealthChecks
{
    public class Startup
    {
        private readonly string PolicyName = "MyCORSPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPI.HealthChecks", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: PolicyName,
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44361")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddHealthChecks()
                .AddCheck<MyRandomHealthCheck>("My random health check")
                .AddSqlServer(
                 connectionString: Configuration["ConnectionStrings:CQRSDemo"],
                 failureStatus: HealthStatus.Degraded);

            services.AddHealthChecksUI()
                .AddInMemoryStorage();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI.HealthChecks v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health")
                    .RequireHost(new string[] { "localhost:5001", "localhost:44300" })
                    .RequireCors(PolicyName);
                    //.RequireAuthorization();
                endpoints.MapHealthChecks("/health-ui",
                    new HealthCheckOptions()
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                endpoints.MapHealthChecksUI();
            });
        }
    }
}
