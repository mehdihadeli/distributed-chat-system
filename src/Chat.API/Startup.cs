using System.ComponentModel.DataAnnotations;
using Chat.Application;
using Chat.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Chat.API
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat.API", Version = "v1" });
            });

            services.AddCors(o => o.AddPolicy("ChatApiCorsPolicy",
                builder =>
                {
                    builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin();
                }));

            services.AddInfrastructure(Configuration);
            services.AddApplication();

             services.AddProblemDetails(x =>
            {
                // Control when an exception is included
                x.IncludeExceptionDetails = (ctx, _) =>
                {
                    // Fetch services from HttpContext.RequestServices
                    var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                    return env.IsDevelopment() || env.IsStaging();
                };
                x.Map<AppException>(ex => new ProblemDetails
                {
                    Title = "Application rule broken",
                    Status = StatusCodes.Status409Conflict,
                    Detail = ex.Message,
                    Type = "https://somedomain/application-rule-validation-error",
                });
                // Exception will produce and returns from our FluentValidation RequestValidationBehavior
                x.Map<ValidationException>(ex => new ProblemDetails
                {
                    Title = "input validation rules broken",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = JsonConvert.SerializeObject(ex.Value),
                    Type = "https://somedomain/input-validation-rules-error",
                });
                x.Map<BadRequestException>(ex => new ProblemDetails
                {
                    Title = "bad request exception",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message,
                    Type = "https://somedomain/bad-request-error",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat.API v1"));
            }

            app.UseProblemDetails();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("ChatApiCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", context => context.Response.WriteAsync("Chat API!"));
            });

            app.UseInfrastructure();
        }
    }
}