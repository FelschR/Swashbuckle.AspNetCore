using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    internal class ConfigureSchemaGeneratorOptions : IConfigureOptions<SchemaGeneratorOptions>
    {
        private readonly SwaggerGenOptions _swaggerGenOptions;
        private readonly IServiceProvider _serviceProvider;

        public ConfigureSchemaGeneratorOptions(
            IOptions<SwaggerGenOptions> swaggerGenOptionsAccessor,
            IServiceProvider serviceProvider)
        {
            _swaggerGenOptions = swaggerGenOptionsAccessor.Value;
            _serviceProvider = serviceProvider;
        }

        public void Configure(SchemaGeneratorOptions options)
        {
            DeepCopy(_swaggerGenOptions.SchemaGeneratorOptions, options);

            // Create and add any filters that were specified through the FilterDescriptor lists
            _swaggerGenOptions.SchemaFilterDescriptors.ForEach(
                filterDescriptor => options.SchemaFilters.Add(CreateFilter<ISchemaFilter>(filterDescriptor)));
        }

        private void DeepCopy(SchemaGeneratorOptions source, SchemaGeneratorOptions target)
        {
            target.CustomTypeMappings = new Dictionary<Type, Func<OpenApiSchema>>(source.CustomTypeMappings);
            target.SchemaIdSelector = source.SchemaIdSelector;
            target.IgnoreObsoleteProperties = source.IgnoreObsoleteProperties;
            target.UseOneOfForPolymorphism = source.UseOneOfForPolymorphism;
            target.UseAllOfForInheritance = source.UseAllOfForInheritance;
            target.SubTypesResolver = source.SubTypesResolver;
            target.DiscriminatorSelector = source.DiscriminatorSelector;
            target.UseAllOfToExtendReferenceSchemas = source.UseAllOfToExtendReferenceSchemas;
            target.UseInlineDefinitionsForEnums = source.UseInlineDefinitionsForEnums;
            target.SchemaFilters = new List<ISchemaFilter>(source.SchemaFilters);
            target.DescribeAllEnumsAsStrings = source.DescribeAllEnumsAsStrings;
            target.DescribeStringEnumsInCamelCase = source.DescribeStringEnumsInCamelCase;
        }

        private TFilter CreateFilter<TFilter>(FilterDescriptor filterDescriptor)
        {
            return (TFilter)ActivatorUtilities
                .CreateInstance(_serviceProvider, filterDescriptor.Type, filterDescriptor.Arguments);
        }
    }
}