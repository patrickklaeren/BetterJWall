using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a named entity within JIRA.
    /// </summary>
    public class JiraNamedEntity : IJiraEntity
    {
        private string _id;
        private string _name;

        /// <summary>
        /// Creates an instance of a JiraNamedEntity base on a remote entity.
        /// </summary>
        public JiraNamedEntity(AbstractNamedRemoteEntity remoteEntity)
            : this(remoteEntity.id, remoteEntity.name)
        {
        }

        /// <summary>
        /// Creates an instance of a JiraNamedEntity.
        /// </summary>
        /// <param name="id">Identifier of the entity.</param>
        /// <param name="name">Name of the entity.</param>
        public JiraNamedEntity(string id, string name = null)
        {
            _id = id;
            _name = name;
        }

        /// <summary>
        /// Id of the entity
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Name of the entity
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        protected virtual Task<IEnumerable<JiraNamedEntity>> GetEntitiesAsync(Jira jira, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(_name))
            {
                return _name;
            }
            else
            {
                return _id;
            }
        }

        internal async Task<JiraNamedEntity> LoadIdAndNameAsync(Jira jira, CancellationToken token)
        {
            if (String.IsNullOrEmpty(_id) || String.IsNullOrEmpty(_name))
            {
                var entities = await this.GetEntitiesAsync(jira, token).ConfigureAwait(false);
                var entity = entities.FirstOrDefault(e =>
                    String.Equals(e.Name, this._name, StringComparison.OrdinalIgnoreCase)
                    || String.Equals(e.Id, this._id, StringComparison.OrdinalIgnoreCase));

                if (entity == null)
                {
                    throw new InvalidOperationException(String.Format("Entity with id '{0}' and name '{1}' was not found for type '{2}'. Available: [{3}]",
                        this._id,
                        this._name,
                        this.GetType(),
                        String.Join(",", entities.Select(s => s.Id + ":" + s.Name).ToArray())));
                }

                _id = entity.Id;
                _name = entity.Name;
            }

            return this;
        }
    }
}
