using DocumentManaging.API.Middleware;
using DocumentManaging.DataAccess.Interfaces.Repositories;
using DocumentManaging.DataAccess.Interfaces.Storages;
using DocumentManaging.DataAccess.Repositories;
using DocumentManaging.DataAccess.Storages;
using DocumentManaging.Services.Interfaces;
using DocumentManaging.Services.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DocumentManaging.API
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
            var cosmosDbConnection = Configuration.GetSection("cosmosDbConnection");
            if (cosmosDbConnection == null)
            {
                throw new ArgumentNullException("CosmosDbConnection is not pressented or empty");
            }

            var blobStorageConnection = Configuration.GetSection("blobStorageConnection");
            if (blobStorageConnection == null)
            {
                throw new ArgumentException("BlobStorageConnection is not pressented or empty");
            }

            services.AddSingleton<CosmosClient, CosmosClient>(provider => new CosmosClient(
                cosmosDbConnection.Value,
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                }));

            services.AddSingleton<IBlobStorage, BlobStorage>(provider => new BlobStorage(blobStorageConnection.Value));
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

      
    }
}
