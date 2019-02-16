using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace JwtAuthentication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var key = System.Text.Encoding.ASCII.GetBytes("TestKey1234567845454545454545454545454");

            services.AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
           })
           .AddJwtBearer(options =>
           {
               options.SaveToken = false;  
               options.RequireHttpsMetadata = false;   // Default true // Use false ONLY for DEVELOPMENT
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidAudience = "yourdomain.com",
                   ValidIssuer = "yourdomain.com",
                   ClockSkew = TimeSpan.Zero,
                   ValidateAudience = true,
                   ValidateIssuer = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key)
               };

               options.Events = new JwtBearerEvents()
               {
                   // called when token validation failed due to validation parameters
                   OnAuthenticationFailed = async (context) =>
                   {
                       int x = context.Response.StatusCode;
                   },

                   //called when request arrives
                   OnMessageReceived = async (context) =>
                   {
                       int x = context.Response.StatusCode;
                   },

                   // called when response is 401 Unauthorized
                   OnChallenge = async (context) =>
                   {
                       int x = context.Response.StatusCode;
                   },

                   // Called after token is validated
                   OnTokenValidated = async (context) =>
                   {
                       int x = context.Response.StatusCode;
                   },
               };
           });

            // Createing Policy for POLICY based Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ManagerPolicy", policyBuilder =>
                {
                    policyBuilder.RequireRole("Manager");
                });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Unhandled error catch middleware
            app.Use(async (context, next) =>
          {
              try
              {
                  await next();
              }
              catch (Exception ex)
              {
                  await context.Response.WriteAsync(ex.Message);
              }
          });

            // Middleware gets called when response status code is between 400 - 599
            app.UseStatusCodePages(async statusCodeContext =>
           {
               if (statusCodeContext.HttpContext.Response.StatusCode == 401)
               {
                   await statusCodeContext.HttpContext.Response.WriteAsync("UnAuthorized !!!");
               }

               if (statusCodeContext.HttpContext.Response.StatusCode == 403)
               {
                   await statusCodeContext.HttpContext.Response.WriteAsync("Forbidden !!!");
               }
           });

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
