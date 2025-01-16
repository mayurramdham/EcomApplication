using App.core.App.User.Command;
using Microsoft.Extensions.DependencyInjection;


namespace App.core
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>();
            });
            return services;
        }
    }
}
