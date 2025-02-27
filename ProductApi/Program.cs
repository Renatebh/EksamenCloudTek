using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Middleware;
using AWS.Logger;
using Amazon.CloudWatch;
using ProductApi.Services;

      
namespace ProductApi
    {
        public class Program
        {
            public static void Main(string[] args)
            {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IAmazonCloudWatch>(new AmazonCloudWatchClient(Amazon.RegionEndpoint.EUNorth1));

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Logging.AddAWSProvider(new AWSLoggerConfig
            {
                LogGroup = "productpi-log-goup", 
                Region = "eu-north-1"
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IProductRepo, ProductRepo>();

            builder.Services.AddSingleton<CloudWatchMetricsService>();

            builder.Services.AddDbContext<ProductContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


            var app = builder.Build();

            app.UseMiddleware<ApiCallCount>();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ProductContext>();
                context.Database.Migrate(); 
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
