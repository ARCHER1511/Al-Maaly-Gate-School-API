using Common.Constants;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomCORS(this IApplicationBuilder app) 
        {
            app.UseCors(CORSConstants.AllowAllPolicy);
            return app;
        }
    }
}
