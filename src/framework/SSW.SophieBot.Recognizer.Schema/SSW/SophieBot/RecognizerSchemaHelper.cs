﻿using SSW.SophieBot.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SSW.SophieBot
{
    public static class RecognizerSchemaHelper
    {
        public static ISet<ModelDescriptor> GetAllModelTypes<T>()
        {
            return GetAllModelTypes(typeof(T));
        }

        public static ISet<ModelDescriptor> GetAllModelTypes(Type schemaType)
        {
            Check.NotNull(schemaType, nameof(schemaType));

            var schemaTypes = GetInheritedSchemaTypes(schemaType).Append(schemaType);
            var schemaAssemblies = schemaTypes.Select(x => x.Assembly).Distinct();
            var modelDescriptors = schemaAssemblies
                .SelectMany(assembly => EnumerateModelDescriptorsFromAssembly(assembly))
                .Distinct();

            return modelDescriptors.SortByDependencies(m => m.Dependencies).ToHashSet();
        }

        public static ISet<Type> GetInheritedSchemaTypes<T>()
        {
            return GetInheritedSchemaTypes(typeof(T));
        }

        public static ISet<Type> GetInheritedSchemaTypes(Type type)
        {
            var typeList = new List<Type>();

            var baseSchemaTypes = SchemaAttribute.GetBaseSchemaTypes(type);
            if (!baseSchemaTypes.Any())
            {
                return typeList.ToHashSet();
            }

            typeList.AddRange(baseSchemaTypes);

            foreach (var baseSchemaType in baseSchemaTypes)
            {
                var recursiveBaseSchemaTypes = GetInheritedSchemaTypes(baseSchemaType);
                typeList.AddRange(recursiveBaseSchemaTypes);
            }

            return typeList.ToHashSet();
        }

        private static IEnumerable<ModelDescriptor> EnumerateModelDescriptorsFromAssembly(Assembly assembly)
        {
            var modelDescriptors = AssemblyHelper.GetAllTypes(assembly)
                .Where(
                    type => type != null &&
                        type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericType &&
                        typeof(IRecognizerModel).IsAssignableFrom(type)
                )
                .Select(type => new ModelDescriptor(type))
                .ToList();

            foreach(var modelDescriptor in modelDescriptors)
            {
                SetDependencies(modelDescriptor, modelDescriptors);
            }

            return modelDescriptors;
        }

        private static void SetDependencies(ModelDescriptor modelDescriptor, List<ModelDescriptor> modelDescriptors)
        {
            foreach (var dependedModelType in FindDependedModelTypesRecursively(modelDescriptor.ModelType))
            {
                var dependedModel = modelDescriptors.FirstOrDefault(m => m.ModelType == dependedModelType);
                if (dependedModel == null)
                {
                    throw new RecognizerSchemaException($"Failed to find depended model type {dependedModelType.FullName} during schema load");
                }

                modelDescriptor.AddDependency(dependedModel);
            }
        }

        private static IEnumerable<Type> FindDependedModelTypesRecursively(Type modelType)
        {
            var dependencies = modelType.GetCustomAttributes(true)
                .OfType<IModelDependency>()
                .Select(attribute => attribute.Dependency);

            foreach(var dependency in dependencies)
            {
                dependencies = dependencies.Union(FindDependedModelTypesRecursively(dependency));
            }

            return dependencies;
        }
    }
}
