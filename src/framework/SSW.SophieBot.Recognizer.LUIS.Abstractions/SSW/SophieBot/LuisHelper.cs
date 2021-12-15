﻿using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Authoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SSW.SophieBot
{
    public static class LuisHelper
    {
        public static ModelCreateObject GetEntityCreateObject<T>()
            where T : class, IEntity
        {
            return GetEntityCreateObject(typeof(T));
        }

        public static ModelCreateObject GetEntityCreateObject(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));
            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new ArgumentException($"Given type should implement {typeof(IEntity).FullName}, but is {entityType.FullName}");
            }

            return new ModelCreateObject(ModelAttribute.GetName(entityType));
        }

        public static HierarchicalChildModelCreateObject GetChildEntityCreateObject(Type childEntityType)
        {
            return new HierarchicalChildModelCreateObject(ModelAttribute.GetName(childEntityType));
        }

        public static ClosedListModelCreateObject GetClEntityCreateObject<T>()
            where T : class, IClosedList
        {
            return GetClEntityCreateObject(typeof(T));
        }

        public static ClosedListModelCreateObject GetClEntityCreateObject(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));
            if (!typeof(IClosedList).IsAssignableFrom(entityType))
            {
                throw new ArgumentException($"Given type should implement {typeof(IClosedList).FullName}, but is {entityType.FullName}");
            }

            return new ClosedListModelCreateObject(name: ModelAttribute.GetName(entityType));
        }

        public static async Task<(Guid, string)> GetLuisAppIdAndActiveVersionAsync(
            ILUISAuthoringClient authoringClient,
            LuisOptions options,
            CancellationToken cancellationToken = default)
        {
            var appId = options.GetGuidAppId();
            if (appId == default)
            {
                throw new LuisException("Failed to get LUIS app ID from configuration, end execution");
            }

            var activeVersion = await authoringClient.GetActiveVersionAsync(appId, cancellationToken);

            return (appId, activeVersion);
        }

        public static List<EntityFeatureRelationModel> GetEntityFeatureRelationModels(Type entityType)
        {
            var featureAttributes = entityType.GetCustomAttributes(true).OfType<FeatureAttribute>();
            return featureAttributes.Select(attribute =>
            {
                if (!attribute.ModelName.IsNullOrEmpty())
                {
                    return new EntityFeatureRelationModel(attribute.ModelName, null, attribute.IsRequired);
                }

                if (!attribute.FeatureName.IsNullOrEmpty())
                {
                    return new EntityFeatureRelationModel(null, attribute.FeatureName, attribute.IsRequired);
                }

                return null;
            })
            .Where(model => model != null)
            .ToList();
        }

        public static void FailOperation(string message = null)
        {
            new OperationStatus(OperationStatusType.Failed, message).EnsureSuccessOperationStatus();
        }
    }
}
