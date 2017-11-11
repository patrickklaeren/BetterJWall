using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Atlassian.Jira.Linq
{
    public class JiraQueryable<T>: IOrderedQueryable<T>, IQueryable<T>
    {
        private readonly JiraQueryProvider _provider;
        private readonly Expression _expression;

        public JiraQueryable(JiraQueryProvider provider)
        {
            this._provider = provider;
            this._expression = Expression.Constant(this);
        }

        public JiraQueryable(JiraQueryProvider provider, Expression expression)
        {
            _provider = provider;
            _expression = expression;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_provider.Execute(this.Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_provider.Execute(this.Expression)).GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return _provider;
            }
        }
    }
}
