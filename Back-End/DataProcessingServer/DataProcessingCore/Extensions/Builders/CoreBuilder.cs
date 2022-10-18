using Microsoft.Extensions.DependencyInjection;

namespace DataProcessingCore
{
    /// <summary>
    /// A builder for configuring Core Serviecs.
    /// </summary>
    public class CoreBuilder
    {

        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where core services are configured.
        /// </summary>
        public IServiceCollection Services { get; }


        public CoreBuilder(IServiceCollection services)
        {
            Services = services;
        }

    }
}
