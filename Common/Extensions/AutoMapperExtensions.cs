using AutoMapper;
using System.Reflection;

namespace Common.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IMappingExpression<TSource, TDestination> IgnoreUnmapped<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> expression)
        {
            // Get all destination writable properties
            var destinationProperties = typeof(TDestination)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            // Get all source readable properties
            var sourceProperties = typeof(TSource)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .Select(p => p.Name)
                .ToHashSet();

            // Find properties on destination that are not present on source
            var unmappedProperties = destinationProperties
                .Where(dest => !sourceProperties.Contains(dest.Name))
                .Select(dest => dest.Name);

            // Ignore each unmapped property explicitly
            foreach (var propertyName in unmappedProperties)
            {
                expression.ForMember(propertyName, opt => opt.Ignore());
            }

            return expression;
        }
    }
}
