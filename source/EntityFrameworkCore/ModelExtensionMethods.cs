using FFCEI.Microservices.EntityFrameworkCore.Generic;
using FFCEI.Microservices.Models;
using System.Linq.Expressions;

namespace FFCEI.Microservices.EntityFrameworkCore;

public static class ModelExtensionMethods
{
    public static async Task<TModel?> NavigationCollectionTryToUndelete<TModel>(
        this List<TModel> navigationProperty,
        IModelRepository<TModel> modelRepository,
        Func<TModel, bool> navigationPropertySearchPredicate,
        Expression<Func<TModel, bool>> modelRepositorySearchPredicate)
        where TModel : IModel
    {
        if (navigationProperty is null)
        {
            throw new ArgumentNullException(nameof(navigationProperty));
        }

        if (modelRepository is null)
        {
            throw new ArgumentNullException(nameof(modelRepository));
        }

        if (navigationPropertySearchPredicate is null)
        {
            throw new ArgumentNullException(nameof(navigationPropertySearchPredicate));
        }

        if (modelRepositorySearchPredicate is null)
        {
            throw new ArgumentNullException(nameof(modelRepositorySearchPredicate));
        }

        var result = navigationProperty.FirstOrDefault(navigationPropertySearchPredicate);

        if (result is null)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            result = await modelRepository.FirstOrDefaultByPredicateAdvancedAsync(true, modelRepositorySearchPredicate);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            if ((result is not null) && (result is ILogicallyDeletableModel deletableResult))
            {
                deletableResult.LogicallyUndelete();

                navigationProperty.Add(result);
            }
        }
        else if (result is ILogicallyDeletableModel deletableResult)
        {
            deletableResult.LogicallyUndelete();
        }

        return result;
    }
}
