using Identity.Models;
using Identity.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Reflection;

namespace Identity;

public class DatabaseService : DbContext
{
    public DatabaseService(DbContextOptions<DatabaseService> options) : base(options)
    {
    }

    public DbSet<Request> Requests { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Step> Steps { get; set; }
    public DbSet<RequestFlowStpe> RequestFlowStpes { get; set; }

    public void Migrate()
    {
        Policy
            .Handle<Exception>()
            .WaitAndRetry(3, r => TimeSpan.FromSeconds(5))
            .Execute(() => Database.Migrate());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Step>().HasData(SeedSteps());

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    private static List<Step> SeedSteps()
    {
        return
        [
            new Step
            {
                Id = Guid.Parse("89c150e7-516e-4307-bc7f-8bc0c5975701"),
                Name = "Phone Number Confrmation",
                Type = StepType.PhoneNumberConfrmation,
                FlowStepType = FlowStepType.Main
            },
            new Step
            {
                Id = Guid.Parse("2dd8b03e-edc0-4db2-8bb1-b5b327e98b8b"),
                Name = "Email Confrmation",
                Type = StepType.EmailConfrmation,
                FlowStepType = FlowStepType.Main
            },
            new Step
            {
                Id = Guid.Parse("5e8569c9-6b12-4c48-ab9f-36cf196a6e6e"),
                Name = "Policy Confrmation",
                Type = StepType.PolicyConfirmation,
                FlowStepType = FlowStepType.Main
            },
            new Step
            {
                Id = Guid.Parse("790d83b4-4599-43e1-899b-067310f57376"),
                Name = "PIN Confrmation",
                Type = StepType.PINConfirmation,
                FlowStepType = FlowStepType.Main
            },
            new Step
            {
                Id = Guid.Parse("e0321176-68f6-4bc7-8504-fd1e3584445e"),
                Name = "Biomtric Confirmation",
                Type = StepType.BiometricConfirmation,
                FlowStepType = FlowStepType.Supportive
            },
        ];
    }
}