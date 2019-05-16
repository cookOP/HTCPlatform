using HTCPlatform.Dapper.Parameters;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace HTCPlatform.Dapper.Repositories
{
    public interface IDapperRepository
    {

        int Insert<T>(string tableName, string fields, T entity);

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <typeparam name="TEntity">数据库表对应的实体类型</typeparam>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        int InsertBulk<T>(string tableName, string fields, List<T> list);

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <param name="connection">sql链接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键字段值</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="dicUpdate">需要更新的字段名/字段值组成的键值对</param>
        /// <returns>更新成功的记录条数</returns>
        int Update(string primaryKeyName, object primaryKeyValue, string tableName, Dictionary<string, object> dicUpdate);


        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="tables">查询的表，可以join关联多表</param>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns></returns>
        IList<TAny> GetList<TAny>(string tables, string fields, bool withNoLock = true, params Parameter[] parameters) where TAny : class;

        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="tables">查询的表，可以join关联多表</param>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns></returns>
        Task<IList<TAny>> GetListAsync<TAny>(string tables, string fields, bool withNoLock = true, params Parameter[] parameters) where TAny : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="tables">查询的表，可以join关联多表</param>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="pageIndex">当前页，从0起始</param>
        /// <param name="pageSize">行数，-1代表查询全部</param>
        /// <param name="orderBy">排序的字段，如 ID asc,Name desc</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns></returns>
        IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="tables">查询的表，可以join关联多表</param>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="pageIndex">当前页，从0起始</param>
        /// <param name="pageSize">行数，-1代表查询全部</param>
        /// <param name="orderBy">排序的字段，如 ID asc,Name desc</param>
        /// <param name="groupBy"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns></returns>
        IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy, string groupBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class;

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="fields"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <param name="orderBy"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        IList<TAny> Query<TAny>(string fields, string table, Parameter[] parameter = null, string orderBy = "", bool withNoLock = true) where TAny : class;

        /// <summary>
        /// 异步执行sql语句
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="fields"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <param name="orderBy"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        Task<IList<TAny>> QueryAsync<TAny>(string fields, string table, Parameter[] parameter = null, string orderBy = "", bool withNoLock = true) where TAny : class;

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="connection">sql链接</param>
        /// <param name="tables">数据库表名（多表连接时为：【[uc_AdminUser] as a with(nolock) left join  Uc_AttendanceRecord as b with(nolock) on a.[AUID]=b.[AUID]】）</param>
        /// <param name="fields">要返回的数据库字段</param>
        /// <param name="pageIndex">要查询的页面数（pageIndex=0表示第一页）</param>
        /// <param name="pageSize">每页的数据大小</param>
        /// <param name="sqlWhere">sqlWhere语句，必须包含【WHERE】关键字</param>
        /// <param name="orderBy">排序语句，必须包含升序/降序关键字，不可为空，不包含【ORDER BY】关键字</param>
        /// <param name="dynamicParameters">sqlWhere语句对应的参数</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns>当前分页的数据</returns>
        IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string sqlWhere, string orderBy, DynamicParameters dynamicParameters, bool withNoLock = true);


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="tables">查询的表，可以join关联多表</param>
        /// <param name="fields">需要查询的字段</param>
        /// <param name="pageIndex">当前页，从0起始</param>
        /// <param name="pageSize">行数，-1代表查询全部</param>
        /// <param name="orderBy">排序的字段，如 ID asc,Name desc</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <param name="parameters">自定义参数</param>
        /// <returns></returns>
        Task<IPagedList<TAny>> GetPagedListQueryAsync<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class;

        /// <summary>
        /// sql语句查询
        /// </summary>
        /// <param name="query">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        IList<TAny> Query<TAny>(string query, object parameters = null, bool withNoLock = true);

        /// <summary>
        /// sql语句查询
        /// </summary>
        /// <param name="query">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        Task<IList<TAny>> QueryAsync<TAny>(string query, object parameters = null, bool withNoLock = true);

        /// <summary>
        /// 执行sql，返回影响的条数
        /// </summary>
        /// <param name="query">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        int Execute(string query, object parameters = null);

        /// <summary>
        /// 执行sql，返回影响的条数
        /// </summary>
        /// <param name="query">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(string query, object parameters = null);
        /// <summary>
        /// 执行sql 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string query, object parameters = null, bool withNoLock = false);
        /// <summary>
        /// 异步执行sql 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, bool withNoLock = false);

        /// <summary>
        /// 根据主键 ，查询一条记录
        /// </summary>
        /// <typeparam name="T">数据库表对应的实体类型</typeparam>
        /// <param name="connection">sql连接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="withNoLock"></param>
        /// <returns>数据库表对应的实体</returns>
        T QueryFirstOrDefault<T>(string primaryKeyName, object primaryKeyValue, string tableName, bool withNoLock = true);

        /// <summary>
        ///异步 根据主键 ，查询一条记录
        /// </summary>
        /// <typeparam name="T">数据库表对应的实体类型</typeparam>
        /// <param name="connection">sql连接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="withNoLock"></param>
        /// <returns>数据库表对应的实体</returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string primaryKeyName, object primaryKeyValue, string tableName, bool withNoLock = true);
        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <param name="tableName">参数</param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sqlText, DynamicParameters parameters);
        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <param name="tableName">参数</param>
        /// <returns></returns>
        T QueryFirstOrDefault<T>(string sqlText, DynamicParameters parameters);

        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);


        #region 存储过程

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        int ExecuteProc(string StoredProcedure, object parms = null);

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        Task<int> ExecuteProcAsync(string StoredProcedure, object parms = null);

        /// <summary>
        /// 执行存储过程，接收return值
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        int ExecuteProcWithReturn(string StoredProcedure, DynamicParameters parms = null);


        /// <summary>
        /// 执行存储过程，接收return值
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        Task<int> ExecuteProcWithReturnAsync(string StoredProcedure, DynamicParameters parms = null);

        /// <summary>
        ///执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        T ExecuteScalarProc<T>(string StoredProcedure, object parms = null, bool withNoLock = false);

        /// <summary>
        /// 异步执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = false);
        /// <summary>
        /// 执行存储过程，返回list集合
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        IList<TAny> QueryProc<TAny>(string StoredProcedure, object parms = null, bool withNoLock = true) where TAny : class;

        /// <summary>
        /// 执行存储过程，返回list集合
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        Task<IList<TAny>> QueryProcAsync<TAny>(string StoredProcedure, object parms = null, bool withNoLock = true) where TAny : class;


        /// <summary>
        /// 执行存储过程,返回ipagedlist对象
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="totalCountName">总行数字段名称，不需要@.例如:@TotalCount 传入TotalCount即可，区分大小写</param>
        /// <param name="pageIndexName">页码字段参数名称 区分大小写</param>
        /// <param name="pageSizeName">每页数量字段参数名称，区分大小写</param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        IPagedList<TAny> GetPagedListProc<TAny>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = true) where TAny : class;


        /// <summary>
        /// 执行存储过程,返回ipagedlist对象
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="totalCountName">总行数字段名称，不需要@.例如:@TotalCount 传入TotalCount即可，区分大小写</param>
        /// <param name="pageIndexName">页码字段参数名称 区分大小写</param>
        /// <param name="pageSizeName">每页数量字段参数名称，区分大小写</param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        Task<IPagedList<TAny>> GetPagedListProcAsync<TAny>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = true) where TAny : class;


        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        GridReader QueryProcMultiple(string StoredProcedure, object parms = null);

        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <param name="withNoLock">是否启用脏读</param>
        /// <returns></returns>
        Task<GridReader> QueryProcMultipleAsync(string StoredProcedure, object parms = null); 
        #endregion
    }
}
