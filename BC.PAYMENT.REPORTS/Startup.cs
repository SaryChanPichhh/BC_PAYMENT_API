using BC.ACCOUNTING.REPORT.Services;
using BC.PAYMENT.INFRASTRUCTURE;
using BC.PAYMENT.REPORTS.Models;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.Data.Entity;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using CustomReportStorageWebExtension = BC.PAYMENT.REPORT.Services.CustomReportStorageWebExtension;
using CustomWebDocumentViewerReportResolver = BC.PAYMENT.REPORT.Services.CustomWebDocumentViewerReportResolver;

namespace BC.PAYMENT.REPORTS
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("SharedConfig/appsettings.json", optional: true, reloadOnChange: true)  // Shared Config
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)             // API-Specific Config
                .AddEnvironmentVariables();
            builder.Build();
          
            services.AddDevExpressControls();
            services.RegisterServices();
            services.Configure<ReportSettings>(Configuration.GetSection("ReportSettings"));
            services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
            services.AddScoped<IConnectionStringsProvider, CustomSqlDataSourceProvider>();
            services.AddTransient<IWebDocumentViewerReportResolver, CustomWebDocumentViewerReportResolver>();



            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
            });

            //reports template


            //var jwtKey = Configuration["Jwt:Key"];
            //var jwtIssuer = Configuration["Jwt:Issuer"];
            //var jwtAudience = Configuration["Jwt:Audience"];

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = jwtIssuer,
            //            ValidAudience = jwtAudience,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            //        };
            //    });

            services
                .AddControllersWithViews();
            services.AddDistributedMemoryCache(); // ? Required for Session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // ? Keep session for 30 mins
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.ConfigureReportingServices(configurator =>
            {
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();

                    //designerConfigurator.RegisterDataSourceWizardJsonConnectionStorage<T>();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
                {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
           

            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;
            app.UseDevExpressControls();
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
