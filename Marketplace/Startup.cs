using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using MarketPlace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Marketplace
{
    public class Startup
    {
        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var esConnection = EventStoreConnection.Create(
                Configuration["eventStore:connectionString"],
                ConnectionSettings.Create().KeepReconnecting(),
                Environment.ApplicationName);

            var store = new EsAggregateStore(esConnection);
            var purgomalumClient = new PurgomalumClient();

            services.AddSingleton(esConnection);
            services.AddSingleton<IAggregateStore>(store);

            services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
            
            services.AddSingleton(new ClassifiedAdsApplicationService(
                store, new FixedCurrencyLookup()));

            services.AddSingleton(new UserProfileApplicationService(
                store, t => purgomalumClient.CheckForProfanity(t)));

            var classifiedAdDetails = new List<ReadModels.ClassifiedAdDetails>();
            services.AddSingleton<IEnumerable<ReadModels.ClassifiedAdDetails>>(classifiedAdDetails);
            
            var userDetails = new List<ReadModels.UserDetails>();
            services.AddSingleton<IEnumerable<ReadModels.UserDetails>>(userDetails);

            var subscription = new ProjectionManager(
                esConnection, 
                new ClassifiedAdDetailsProjection(
                    classifiedAdDetails, 
                    userid => userDetails.FirstOrDefault(u => u.UserId == userid)?.DisplayName), 
                new UserDetailsProjection(userDetails),
                new ClassifiedAdUpcasters(esConnection, userId => userDetails.FirstOrDefault(u => u.UserId == userId)?.PhotoUrl));

            services.AddSingleton<IHostedService>(new EventStoreService(esConnection,subscription));
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
            
            // app.EnsureDatabase();
            
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
