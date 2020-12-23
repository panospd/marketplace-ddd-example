using Marketplace.ClassifiedAd;
using Marketplace.Domain;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;

namespace Marketplace
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
            // const string connectionString = "Host=localhost;Database=Marketplace_Chapter8;Username=ddd;Password=book";
            //
            // services
            //     .AddEntityFrameworkNpgsql()
            //     .AddDbContext<ClassifiedAdDbContext>(options => options.UseNpgsql(connectionString));

             var store = new DocumentStore
             {
                 Urls = new[] {"http://localhost:8080/"},
                 Database = "Marketplace_Chapter9",
                 Conventions =
                 {
                     FindIdentityProperty = m => m.Name == "DbId"
                 }
             };
            
            store.Initialize();
            services.AddScoped(c => store.OpenAsyncSession());
            
            var purgomalumClient = new PurgomalumClient();

            services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
            services.AddScoped<IUnitOfWork, RavenDbUnitOfWork>();
            
            services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            
            services.AddScoped<ClassifiedAdsApplicationService>();

            services.AddScoped(c => new UserProfileApplicationService(c.GetService<IUserProfileRepository>(),
                c.GetService<IUnitOfWork>(),
                text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));

            services.AddMvc();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title= "ClassifiedAds", Version="v1" } );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.EnsureDatabase();
            
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Marketplace api");
            });
        }
    }
}
