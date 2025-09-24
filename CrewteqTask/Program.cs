using CrewteqTask.VerticalSlicing.Data.Context;
using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Data.Repository.Repository;
using CrewteqTask.VerticalSlicing.Features.Employee.Mappings;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace CrewteqTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(builder.Configuration["ConnectionStrings:SqlServer"]));

            // Register repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Register mapping services
            builder.Services.AddScoped<IEmployeeMappingService, EmployeeMappingService>();

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //      app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            ;
            app.UseAuthorization();

            app.MapControllers();
            app.MapScalarApiReference();
            app.MapOpenApi();
            app.Run();
        }
    }
}
