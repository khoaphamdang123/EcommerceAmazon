using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ecommerce_Product.Data;
using Serilog;
using StackExchange.Redis;
using Ecommerce_Product.Models;
using Ecommerce_Product.Repository;
using Ecommerce_Product.Service;
using Ecommerce_Product.Support_Serive;
using reCAPTCHA.AspNetCore;
using Quartz;
using Ecommerce_Product.Job;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);


var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var database = Environment.GetEnvironmentVariable("DB_NAME");
var username = Environment.GetEnvironmentVariable("DB_USER");
var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

Console.WriteLine("DB_HOST:"+host);

Console.WriteLine("DB_PORT:"+port);
Console.WriteLine("DB_NAME:"+database);
Console.WriteLine("DB_USER:"+username);
Console.WriteLine("DB_PASSWORD:"+password);


var paypal_client_id=Environment.GetEnvironmentVariable("PAYPAL_CLIENT_ID");

var paypal_client_secret=Environment.GetEnvironmentVariable("PAYPAL_CLIENT_SECRET");


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<CheckOrderJob>(opts => opts.WithIdentity("CheckOrderJob"));
    
    q.AddTrigger(opts => opts
        .ForJob("CheckOrderJob")
        .WithIdentity("CheckOrderJobTrigger")
        .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever()) // Run every 24 hours
    );
});


FirebaseApp.Create(new AppOptions()
{
  Credential=GoogleCredential.FromFile("ecommerce-product-92f11-firebase-adminsdk-fbsvc-b877dd5236.json")
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


builder.Services.AddSingleton<IConnectionMultiplexer>(sp=>{
    var configuration=builder.Configuration.GetSection("Redis:ConnectionString").Value;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Configuration["Paypal:ClientId"] = paypal_client_id;

builder.Configuration["Paypal:ClientSecret"] = paypal_client_secret;

builder.Configuration["ConnectionStrings:DefaultConnection"] = 
    $"Host={host};Port={port};Database={database};Username={username};Password={password}";


Console.WriteLine("ConnectionString:"+builder.Configuration["ConnectionStrings:DefaultConnection"]);

builder.Services.AddControllersWithViews();

var _logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("SourceContext", out var sourceContext) &&
        (sourceContext.ToString().Contains("Microsoft") || sourceContext.ToString().Contains("System") || sourceContext.ToString().Contains("Quartz"))
    ).Enrich.FromLogContext().CreateLogger();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddRecaptcha(options =>
{
    options.SiteKey =builder.Configuration.GetSection("Recapcha")["SiteKey"];
    options.SecretKey =builder.Configuration.GetSection("Recapcha")["SecretKey"];
});

builder.Services.AddDbContext<EcommerceshopContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddSession(options=>
{
  options.IdleTimeout=TimeSpan.FromHours(1);
  options.Cookie.HttpOnly = true;
  options.Cookie.IsEssential=true;  
});

builder.Services.AddSingleton<FirebaseService>();

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddAuthentication().AddCookie();

builder.Services.AddScoped<ILoginRepository,LoginService>();

builder.Services.AddScoped<IUserListRepository,UserListService>();

builder.Services.AddScoped<IAdminRepository,AdminListService>();

builder.Services.AddScoped<ICategoryListRepository,CategoryListService>();

builder.Services.AddScoped<IProductRepository,ProductService>();

builder.Services.AddScoped<IStaticFilesRepository,StaticFilesService>();

builder.Services.AddScoped<IOrderRepository,OrderListService>();

builder.Services.AddScoped<IPaymentRepository,PaymentListService>();

builder.Services.AddScoped<IDashboardRepository,DashboardService>();

builder.Services.AddScoped<IBannerListRepository,BannerListService>();

builder.Services.AddScoped<IManualRepository,ManualService>();

builder.Services.AddScoped<IVideoRepository,VideoService>();

builder.Services.AddScoped<ICartRepository,CartService>();

builder.Services.AddScoped<IBlogRepository,BlogListService>();

builder.Services.AddScoped<ISettingRepository,SettingService>();

builder.Services.AddScoped<ITrackDataRepository,TrackdataService>();

builder.Services.AddTransient<Service>();

builder.Services.AddTransient<SmtpService>();

builder.Services.AddSingleton<PaypalService>();

//builder.Services.AddSingleton(provider => new GoogleAnalyticsService());
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<SmtpModel>(builder.Configuration.GetSection("SmtpModel"));

builder.Services.Configure<RecaptchaResponse>(builder.Configuration.GetSection("Recapcha"));

// builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)  
//     .AddCookie(options =>  
//     {  
//         options.LoginPath = "/admin/login";  
//     });  

 builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options=>{
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1; 
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 1;
 })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
  options.Cookie.HttpOnly = true;  
  options.ExpireTimeSpan = TimeSpan.FromHours(1);  
  options.LoginPath = "/admin/login";  
  options.AccessDeniedPath = "/admin/login";  
  options.SlidingExpiration = true;  
  options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Redirect("/admin/login");
        return Task.CompletedTask;        
    };
});


// builder.Services.AddAuthentication(options =>
//                 {
//                     options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
//                     options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
//                 });


builder.Logging.ClearProviders();

builder.Logging.AddSerilog(_logger);

Console.WriteLine("Current Environement is:"+builder.Environment.EnvironmentName);

// builder.Services.AddAuthorization(options=>{
//     options.AddPolicy("Admin",policy=>policy.RequireRole("Admin"));
//     options.AddPolicy("User",policy=>policy.RequireRole("User"));
// });


var app = builder.Build(); 


app.UseSession();

// using (var scope = app.Services.CreateScope())
// {  
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

//     // Create roles if they don't exist
//     string[] roleNames = { "Admin", "User" };
//     foreach (var roleName in roleNames)
//     {
//         if (!await roleManager.RoleExistsAsync(roleName))
//         {
//             await roleManager.CreateAsync(new IdentityRole(roleName));
//         }
//     }

//     // Create a default admin user
//     var adminEmail = "admin@demo.com";
//     var adminPassword = "Admin@123";
//     string normalEmail = "user@demo.com";
//     string normalPassword="user123@demo.com";
 
//     if(normalEmail==null)
//     {
//         var newNormalUser = new IdentityUser{UserName = normalEmail,Email=normalEmail};
//         var createUser = await userManager.CreateAsync(newNormalUser,normalPassword);
//         if(createUser.Succeeded)
//         {
//             await userManager.AddToRoleAsync(newNormalUser,"User");
//         }
//     }
// }
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();

    Environment.SetEnvironmentVariable("DNS", "https://thanhquang-gnss.com");
}
else
{
    app.UseDeveloperExceptionPage();


    Environment.SetEnvironmentVariable("DNS", "http://localhost:5160");
}



// app.UseStatusCodePagesWithReExecute("/admin/Error/{0}");

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<NotFoundMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomePage}/{action=home}/{id?}"
);
app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=LoginAdmin}/{action=Index}/{id?}"
    );

app.Run();
