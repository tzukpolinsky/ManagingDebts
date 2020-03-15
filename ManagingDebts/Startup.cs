using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Services;
using System;

namespace ManagingDebts
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
            
            services.AddScoped((_) => new ManagingDebtsContext(Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
            services.AddScoped<ICustomersService, CustomersService>();
            services.AddScoped<IBankAccountsService, BankAccountsService>();
            services.AddScoped<IContractsService, ContractsService>();
            services.AddScoped<IBudgetsService, BudgetsService>();
            services.AddScoped<IPrivateSupplierFileReaderService, PrivateSupplierFileReaderService>();
            services.AddScoped<IBezekFileReaderService, BezekFileReaderService>();
            services.AddScoped<IElectricityFileReaderService, ElectricityFileReaderService>();
            services.AddScoped<IGeneralBillingSummaryService, GeneralBillingSummaryService>();
            services.AddScoped<ISuppliersService, SuppliersService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IBezekDataConfirmationService, BezekDataConfirmationService>();
            services.AddScoped<IElectricityDataConfirmationService, ElectricityDataConfirmtionService>();
            services.AddScoped<ISliService, SliService>();
            services.AddScoped<IBudgetContractService, BudgetContractService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPrivateSupplierDataConfirmationService, PrivateSupplierDataConfirmationService>();
            services.AddScoped<IMailService, MailService>();
            services.AddControllersWithViews();
            services.AddMemoryCache();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
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
            loggerFactory.AddFile("Log/log-{Date}.txt");
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();

                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
