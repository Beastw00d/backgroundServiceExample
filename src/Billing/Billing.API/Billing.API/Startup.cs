using Billing.API.BackgroundTasks;
using Billing.API.IntegrationEvents.Events;
using Billing.API.Repositories;
using Billing.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using MongoDB.Driver;
using Platibus;
using Platibus.Journaling;
using Platibus.MongoDB;
using Platibus.Serialization;
using Swashbuckle.AspNetCore.Swagger;


namespace Billing.API
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
            services.AddMvc();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info {Title = "Billing API", Version = "v1"});
            });

            var billingConnectionString = Configuration.GetConnectionString("billingDB");
            var journalingConnectionString = Configuration.GetConnectionString("journalingDB");
            if (Configuration.GetValue<bool>("UseMongo2Go"))
            {
                
                var runner = MongoDbRunner.StartForDebugging();
                services.AddSingleton(runner);
                billingConnectionString = runner.ConnectionString + "billing";
                journalingConnectionString = runner.ConnectionString + "journaling";
            }

            var billingDb = OpenDatabaseConnection(billingConnectionString);

            var journalDb = OpenDatabaseConnection(journalingConnectionString);
            var messageJournal =
                new MongoDBMessageJournal(
                    new MongoDBMessageJournalOptions(journalDb));
            services.AddSingleton(billingDb);
            services.AddSingleton<IInvoiceRepository, MongoInvoiceRepository>();
            services.AddSingleton<IJournalingUpdateService, JournalingUpdateService>();
            services.AddSingleton<IJournalConsumerProgressTracker, MongoJournalConsumerProgressTracker>();
            services.AddSingleton<IMessageNamingService, DefaultMessageNamingService>();
            services.AddSingleton<ISerializationService, DefaultSerializationService>();
            services.AddSingleton<IHostedService, JournalingManagerService>();
            services.AddSingleton<IMessageJournal>(sp => messageJournal);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing API");
            });
        }

        private IMongoDatabase OpenDatabaseConnection(string connectionString)
        {            
            var client = new MongoClient(connectionString);
            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            return client.GetDatabase(databaseName);
        }
    }
}
