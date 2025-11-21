using ContractClaimSystem.Services;
using ContractMonthlyClaimSystem.Data;
using Microsoft.AspNetCore.Identity;

public void ConfigureServices(IServiceCollection services)
{
    // Database connection
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

    // Identity
    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    // Register services
    services.AddScoped<IClaimService, ClaimService>();
    services.AddScoped<IHRService, HRService>();

    services.AddControllersWithViews();
}