using System;
using System.Linq;

namespace FrostAura.Libraries.Data.Extensions.EntityFramework
{
    /// <summary>
    /// Validation extensions for DbSet.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Get an entity if already existing or add it and return it's context if it doesn't already exist.
        /// </summary>
        /// <typeparam name="TEntity">Type of the data entity.</typeparam>
        /// <param name="set">The databsae set to perform the query on.</param>
        /// <param name="entity">The entity to perform the query on.</param>
        /// <param name="identifierFunction">How the entity is unique.</param>
        /// <returns>The instance of the processed entity.</returns>
        public static TEntity GetOrAdd<TEntity>(this Microsoft.EntityFrameworkCore.DbSet<TEntity> set, TEntity entity, Func<TEntity, bool> identifierFunction) where TEntity : class
        {
            // Get the existing entity if any, else get null
            TEntity existingEntity = set
                .FirstOrDefault(identifierFunction);

            // If there is an existing entity, return it
            if (existingEntity != null) return existingEntity;

            // If no entity of this kind exists, create it and return it
            set.Add(entity);

            return entity;
        }
    }
}