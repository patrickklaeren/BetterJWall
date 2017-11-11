using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Atlassian.Jira
{
    /// <summary>
    /// Dictionary of Jira entities, used to store cached values.
    /// </summary>
    public class JiraEntityDictionary<T> : ConcurrentDictionary<string, T>
        where T : IJiraEntity
    {
        /// <summary>
        /// Create an empty dictionary.
        /// </summary>
        public JiraEntityDictionary()
        {
        }

        /// <summary>
        /// Create a dictionary and initialize it with the given entities.
        /// </summary>
        public JiraEntityDictionary(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                this.TryAdd(entity.Id, entity);
            }
        }

        /// <summary>
        /// Attempts to remove the entity that has the specified key.
        /// </summary>
        /// <param name="id">Identifier of the entity.</param>
        public bool TryRemove(string id)
        {
            T removedEntity;
            return TryRemove(id, out removedEntity);
        }

        /// <summary>
        /// Adds an entity to the dictionary if it missing, otherwise no-op.
        /// </summary>
        /// <returns>True if entity was added, false otherwise.</returns>
        public bool TryAdd(T entity)
        {
            return this.TryAdd(entity.Id, entity);
        }

        /// <summary>
        /// Adds a list of entities to the dictionary if their are missing.
        /// </summary>
        /// <returns>True if at least one entity was added, false otherwise.</returns>
        public bool TryAdd(IEnumerable<T> entities)
        {
            var result = false;
            foreach (var entity in entities)
            {
                result = this.TryAdd(entity.Id, entity) || result;
            }

            return result;
        }
    }
}
