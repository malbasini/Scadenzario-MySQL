using System.Configuration;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCourse.Customizations.Identity;
using Scadenzario.Customizations.Identity;
using Scadenzario.Areas.Identity.Data;
using Scadenzario.Models.Entity;
using Scadenzario.Models.Options;
using Scadenzario.Models.Services.Application;
using Scadenzario.Models.Services.Application.Beneficiari;
using Scadenzario.Models.Services.Application.Scadenze;
using Scadenzario.Models.Services.Applications.Beneficiari;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.Services.Infrastructure;
using Scadenze.Customizations.ModelBinders;
using Scdenzario.Models.Options;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
            string? connectionString = builder.Configuration.GetValue<String>("ConnectionStrings:Default");
            builder.Services.AddResponseCaching();
            builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));
            builder.Services.AddMvc(Options =>
            {
                Options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
                var homeProfile = new CacheProfile();
                homeProfile.Duration = builder.Configuration.GetValue<int>("ResponseCache:Home:Duration");
                homeProfile.Location = builder.Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                homeProfile.VaryByQueryKeys = new string[] { "Page" };
                Options.CacheProfiles.Add("Home", homeProfile);
                builder.Configuration.Bind("ResponseCache:Home", homeProfile);
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddTransient<IBeneficiariService, EfCoreBeneficiarioService>();
            builder.Services.AddTransient<ICachedBeneficiarioService, MemoryCacheBeneficiarioService>();
            IServiceCollection serviceCollection = builder.Services.AddDbContext<ScadenzarioIdentityDbContext>(
                optionsBuilder => optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddTransient<IScadenzeService,EfCoreScadenzeService>();
            builder.Services.AddTransient<IRicevuteService,EFCoreRicevutaService>();
            builder.Services.AddTransient<ICachedScadenzeService,MemoryCacheScadenzeService>();
            builder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredUniqueChars = 3;
                    //Conferma Account
                    options.SignIn.RequireConfirmedAccount = true;
                    //Blocco dell'account
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddEntityFrameworkStores<ScadenzarioIdentityDbContext>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                .AddPasswordValidator<CommonPasswordValidator<ApplicationUser>>();
            builder.Services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = builder.Configuration.GetValue<String>("LoginFacebook:AppId");
                facebookOptions.AppSecret = builder.Configuration.GetValue<String>("LoginFacebook:AppSecret");
            });
            builder.Services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = builder.Configuration.GetValue<String>("LoginGoogle:ClientId");
                googleOptions.ClientSecret = builder.Configuration.GetValue<String>("LoginGoogle:ClientSecret");
            });
            builder.Services.AddAuthentication().AddTwitter(t =>
            {
                t.ConsumerKey = builder.Configuration.GetValue<String>("LoginTwitter:key");
                t.ConsumerSecret = builder.Configuration.GetValue<String>("LoginTwitter:secret");
            });
            builder.Services.AddAuthentication().AddYahoo(YahooOptions =>
            {
                YahooOptions.ClientId = builder.Configuration.GetValue<String>("LoginYahoo:clientid");
                YahooOptions.ClientSecret = builder.Configuration.GetValue<String>("LoginYahoo:clientsecret");
            });
            builder.Services.AddAuthentication().AddMicrosoftAccount(microsoft =>
            {
                microsoft.ClientId = builder.Configuration.GetValue<String>("LoginMicrosoft:id");
                microsoft.ClientSecret = builder.Configuration.GetValue<String>("LoginMicrosoft:key");
            });
            builder.Services.AddAuthentication().AddGitHub(git =>
            {
                git.ClientId = builder.Configuration.GetValue<String>("Logingithub:clientid");
                git.ClientSecret = builder.Configuration.GetValue<String>("Logingithub:clientsecret");
                git.CallbackPath = "/signin-github";
                git.Scope.Add("read:user");
            });
            //Options
            builder.Services.Configure<AdminOption>(builder.Configuration.GetSection("AdminOption"));
            builder.Services.Configure<BeneficiariOptions>(builder.Configuration.GetSection("Beneficiari"));
            builder.Services.Configure<ScadenzeOptions>(builder.Configuration.GetSection("Scadenze"));
            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
            builder.Services.Configure<IMemoryCacheOptions>(builder.Configuration.GetSection("MemoryCache"));

var app = builder.Build();
ConfigureApp(app);
static void ConfigureApp(WebApplication app)
{
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseStaticFiles();
    //Endpoint routing Middleware
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    //EndpointMiddleware
    var supportedCultures = new string[] { "it-IT", "fr-FR" };
    app.UseRequestLocalization(options =>
        options.AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures)
            .SetDefaultCulture("it-IT")
    );
    /*--Una route viene identificata da un nome, in questo caso default
    e tre frammenti controller,action e id. Grazie a questa Route il
    meccanismo di routing è in grado di soddisfare le richieste. Facciamo un esempio
    Supponiamo che arrivi la seguente richiesta HTTP /Scadenze/Detail/5
    Grazie a questo template il meccanismo di routing sa che deve andare a chiamare
    un controller chiamato Scadenze, la cui action è Detail e a cui passa
    l'id 5.*/
    app.UseResponseCaching();
    app.UseEndpoints(routeBuilder =>
    {
        routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
        routeBuilder.MapRazorPages();
    });
    app.Run();
}