using System.Reflection;
using Hangfire;
using Lms.Application.Features.Circulations.Commands.RunDailyProcessing;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lms.Infrastructure
{
    public static class BackgroundJobInitializer
    {
        public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder app)
        {
            var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();
            var currentClass = typeof(BackgroundJobInitializer);
            var privateMethods = currentClass.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(method => method.ReturnType == typeof(void) && method.GetParameters().Length == 1);

            foreach (var method in privateMethods)
            {
                method.Invoke(null, [ recurringJobManager ]);
            }

            return app;
        }

#pragma warning disable RCS1213, IDE0051
        private static void DailyCirculation(IRecurringJobManager recurringJobManager)
        {
            recurringJobManager.AddOrUpdate<ISender>(
                "daily-circulation",
                sender => sender.Send(new RunDailyProcessingCommand(), default),
                Cron.Daily
            );
        }
#pragma warning restore
    }
}
