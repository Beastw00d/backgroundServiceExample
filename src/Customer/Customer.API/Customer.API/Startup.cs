using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Platibus.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;

namespace Customer.API
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
                options.SwaggerDoc("v1", new Info { Title = "Customer API", Version = "v1" });
            });

            var database = OpenDatabaseConnection();
            services.AddSingleton(database);


            var serviceProvider = services.BuildServiceProvider();
            services.AddPlatibusServices(configure: configuration =>
            {
                var sink = serviceProvider.GetService<AspNetCoreLoggingSink>();
                configuration.DiagnosticService.AddSink(sink);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UsePlatibusMiddleware();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer API");
            });

        }

        private IMongoDatabase OpenDatabaseConnection()
        {
            var connectionString = Configuration.GetConnectionString("customersDB");
            var client = new MongoClient(connectionString);
            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            return client.GetDatabase(databaseName);
        }
    }
}
