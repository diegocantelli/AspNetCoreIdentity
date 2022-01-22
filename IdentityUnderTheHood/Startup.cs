using IdentityUnderTheHood.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityUnderTheHood
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
            //Necess�io especificar o nome da autentica��o que ser� usada, pois podem existir v�rios handlers de autentica��o
            //configurados
            services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", opt =>
            {
                //Atrav�s dessa propriedade que o asp net ir� saber de qual cookie pegar as informa��es de autentica��o
                opt.Cookie.Name = "MyCookieAuth";

                //Definindo o local da p�gina de login para que possa ocorrer o redirecionamento em caso de n�o estar
                //autenticado
                opt.LoginPath = "/Account/Login";

                opt.AccessDeniedPath = "/Account/AccessDenied";

                //Definindo o tempo de expira��o do cookie
                opt.Cookie.Expiration = TimeSpan.FromMinutes(30);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy => policy.RequireClaim("Admin"));

                //Policys com mais de uma claim
                options.AddPolicy("HrManagerOnly",
                    policy => policy
                        .RequireClaim("Department", "HR")
                        .RequireClaim("Manager")
                        .Requirements.Add(new HRManagementsProbationRequirement(3)));

                //Cria uma policy chamada MustBelongToHrDepartment que deve conter uma claim Chamada Department, que deve ter o valor HR
                //No caso deste exemplo as claims est�o sendo setadas manualmento no m�todo OnPostAsync em login.cshtml.cs
                options.AddPolicy("MustBelongToHrDepartment",
                    policy => policy.RequireClaim("Department", "HR"));
            });

            services.AddSingleton<IAuthorizationHandler, HRManagementsProbationRequirementHandler>();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //Acionando o middleware de autentica��o no pipeline, para que seja tratado o cookie de autentica��o
            //e autenticar o usu�rio atrav�s do cookie armazenado
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
