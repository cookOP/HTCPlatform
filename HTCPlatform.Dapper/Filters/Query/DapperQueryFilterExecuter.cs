using HTCPlatform.Dapper.Expressions;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HTCPlatform.Dapper.Filters.Query
{
    public class DapperQueryFilterExecuter: IDapperQueryFilterExecuter
    {
        public static readonly DapperQueryFilterExecuter Instance = new DapperQueryFilterExecuter();
        //private readonly IList<IDapperQueryFilter> _queryFilters;

        //public DapperQueryFilterExecuter(IIocResolver iocResolver)
        //{
        //    _queryFilters = iocResolver.ResolveAll<IDapperQueryFilter>();
        //}
        public DapperQueryFilterExecuter()
        {
            //利用反射获取所有实现类的示例集合
            //var type = typeof(IDapperQueryFilter);
            //var types = AppDomain.CurrentDomain.GetAssemblies()
            //    .SelectMany(s => s.GetTypes())
            //    .Where(p => type.IsAssignableFrom(p.GetGenericTypeDefinition()));
            //foreach (var v in types)
            //{
            //    if (v.IsClass)
            //    {
            //        _queryFilters.Add((Activator.CreateInstance(v) as IDapperQueryFilter));
            //    }
            //}
        }

        public IPredicate ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            //ICollection<IDapperQueryFilter> filters = _queryFilters.ToList();

            //foreach (IDapperQueryFilter filter in filters)
            //{
            //    predicate = filter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            //}

            IPredicate pg = predicate.ToPredicateGroup<TEntity, TPrimaryKey>();
            return pg;
        }

        public PredicateGroup ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class
        {
            //ICollection<IDapperQueryFilter> filters = _queryFilters.ToList();
            var groups = new PredicateGroup
            {
                Operator = GroupOperator.And,
                Predicates = new List<IPredicate>()
            };

            //foreach (IDapperQueryFilter filter in filters)
            //{
            //    IFieldPredicate predicate = filter.ExecuteFilter<TEntity, TPrimaryKey>();
            //    if (predicate != null)
            //    {
            //        groups.Predicates.Add(predicate);
            //    }
            //}

            return groups;
        }
    }
}
