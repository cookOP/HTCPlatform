using HTCPlatform.Dapper.Filters.Query;
using HTCPlatform.Dapper.Parameters;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using System.Data.SqlClient;

namespace HTCPlatform.Dapper.Repositories
{
    public class DapperRepository : IDapperRepository
    {
      
        public virtual DbConnection Connection
        {
            get
            {
                return new SqlConnection(
                    @"Server=.;Database=HTCPlatform;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }
        
        #region 事务隔离级别设置
        /// <summary>
        /// 脏读
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="acquire"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        protected T ReadUncommitted<T>(Func<T> acquire, bool withNoLock)
        {
            if (withNoLock)
            {
                this.Execute("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            }
            try
            {
                return acquire();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (withNoLock)
                {
                    this.Execute("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
                }
            }
        }

        /// <summary>
        /// 异步脏读
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="acquire"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        protected async Task<T> ReadUncommittedAsync<T>(Func<Task<T>> acquire, bool withNoLock)
        {
            if (withNoLock)
            {
                this.Execute("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            }
            try
            {
                var result = await acquire();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (withNoLock)
                {
                    this.Execute("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
                }
            }
        }
        #endregion

        #region 新增、修改
        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity"></param>
        public int Insert<T>(string tableName, string fields, T entity)
        {
            string[] res = fields.Split(',');
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = "@" + res[i].Trim();
            }
            string sqlText = string.Format(" INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, fields, string.Join(",", res));

            return Connection.Execute(sqlText, entity);

        }

        /// <summary>
        /// 批量插入数据，返回成功的条数（未启用事物）
        /// </summary>
        /// <typeparam name="TEntity">数据库表对应的实体类型</typeparam>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fields">数据库表的所有字段，用【,】分隔（主键自增时应排除主键字段）</param>
        /// <param name="list">数据库表对应的实体集合</param>
        /// <returns>成功的条数</returns>
        public int InsertBulk<T>(string tableName, string fields, List<T> list)
        {
            string[] res = fields.Split(',');
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = "@" + res[i].Trim();
            }
            string sqlText = string.Format(" INSERT INTO {0} ({1}) VALUES ({2}); ", tableName, fields, string.Join(",", res));

            return Connection.Execute(sqlText, list);

        }


        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <param name="connection">sql链接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键字段值</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="dicUpdate">需要更新的字段名/字段值组成的键值对</param>
        /// <returns>更新成功的记录条数</returns>
        public int Update(string primaryKeyName, object primaryKeyValue, string tableName, Dictionary<string, object> dicUpdate)
        {
            if (dicUpdate == null || dicUpdate.Count == 0)
                return 0;
            StringBuilder sqlText = new StringBuilder();
            sqlText.AppendFormat(" UPDATE {0} SET ", tableName);
            DynamicParameters pars = new DynamicParameters();
            string[] fields = new string[dicUpdate.Count];
            int index = 0;
            foreach (string name in dicUpdate.Keys)
            {
                sqlText.AppendFormat(" {0}=@p_{1},", name, index);
                pars.Add("@p_" + index.ToString(), dicUpdate[name]);
                index++;
            }
            sqlText.Remove(sqlText.Length - 1, 1);
            sqlText.AppendFormat(" WHERE {0}=@p_{1} ", primaryKeyName, index.ToString());
            pars.Add("@p_" + index.ToString(), primaryKeyValue);

            return Connection.Execute(sqlText.ToString(), pars);

        }
        #endregion

        #region list

        public IList<TAny> GetList<TAny>(string tables, string fields, bool withNoLock = true, params Parameter[] parameters) where TAny : class
        {
            Func<IList<TAny>> acquire = (() =>
           {
               DynamicParameters dp;
               string sqlText = GetListSqlText(tables, fields, out dp, parameters);
               return Connection.Query<TAny>(sqlText, dp).ToList();
           });
            return ReadUncommitted(acquire, withNoLock);
        }

        public async Task<IList<TAny>> GetListAsync<TAny>(string tables, string fields, bool withNoLock = true, params Parameter[] parameters) where TAny : class
        {
            Func<Task<IList<TAny>>> acquire = (async () =>
            {
                DynamicParameters dp;
                string sqlText = GetListSqlText(tables, fields, out dp, parameters);
                var data = await Connection.QueryAsync<TAny>(sqlText, dp);
                return data.ToList();
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }


        public IList<TAny> Query<TAny>(string query, object parameters = null, bool withNoLock = true)
        {
            Func<IList<TAny>> acquire = (() =>
            {
                return Connection.Query<TAny>(query, parameters).ToList();
            });

            return ReadUncommitted(acquire, withNoLock);
        }

        public async Task<IList<TAny>> QueryAsync<TAny>(string query, object parameters = null, bool withNoLock = true)
        {
            Func<Task<IList<TAny>>> acquire = (async () =>
            {
                var data = await Connection.QueryAsync<TAny>(query, parameters);
                return data.ToList();
            });

            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="fields"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IList<TAny> Query<TAny>(string fields, string table, Parameter[] parameter = null, string orderBy = "", bool withNoLock = true) where TAny : class
        {
            Func<IList<TAny>> acquire = (() =>
            {
                string sql = $" select {fields} from {table} ";
                string wherestr = "";
                var dynamicParameters = GetDynamicParameter(ref wherestr, parameter);
                if (!string.IsNullOrWhiteSpace(wherestr))
                    sql += $" where 1=1 {wherestr} ";
                if (!string.IsNullOrWhiteSpace(orderBy))
                    sql += $" order by {orderBy} ";
                return Connection.Query<TAny>(sql, dynamicParameters).ToList();
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 异步执行SQL语句
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="fields"></param>
        /// <param name="table"></param>
        /// <param name="parameter"></param>
        /// <param name="orderBy"></param>
        /// <param name="withNoLock"></param>
        /// <returns></returns>
        public async Task<IList<TAny>> QueryAsync<TAny>(string fields, string table, Parameter[] parameter = null, string orderBy = "", bool withNoLock = true) where TAny : class
        {
            Func<Task<IList<TAny>>> acquire = (async () =>
            {
                string sql = $" select {fields} from {table} ";
                string wherestr = "";
                var dynamicParameters = GetDynamicParameter(ref wherestr, parameter);
                if (!string.IsNullOrWhiteSpace(wherestr))
                    sql += $" where 1=1 {wherestr} ";
                if (!string.IsNullOrWhiteSpace(orderBy))
                    sql += $" order by {orderBy} ";
                var data = await Connection.QueryAsync<TAny>(sql, dynamicParameters);
                return data.ToList();
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }
        #endregion

        #region 分页查询
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
        /// <returns>当前分页的数据</returns>
        public IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class
        {
            int totalCount;
            Func<IPagedList<TAny>> acquire = (() =>
            {
                DynamicParameters dp;
                string sqlText = GetPagedListSqlText(tables, fields, pageIndex, pageSize, orderBy, out dp, parameters);

                var list = Connection.Query<TAny>(sqlText, dp);
                totalCount = dp.Get<int?>("@RowCount") ?? 0;
                return new PagedList<TAny>()
                {
                    Items = list.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam> 
        /// <param name="tables">数据库表名（多表连接时为：【[uc_AdminUser] as a with(nolock) left join  Uc_AttendanceRecord as b with(nolock) on a.[AUID]=b.[AUID]】）</param>
        /// <param name="fields">要返回的数据库字段</param>
        /// <param name="pageIndex">要查询的页面数（pageIndex=0表示第一页）</param>
        /// <param name="pageSize">每页的数据大小</param> 
        /// <param name="orderBy">排序语句，必须包含升序/降序关键字，不可为空，不包含【ORDER BY】关键字</param>
        /// <param name="groupBy">groupBy</param>
        /// <param name="parameters">sqlWhere语句对应的参数</param>
        /// <param name="withNoLock">启用脏读</param>
        /// <returns>当前分页的数据</returns>
        public IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy,string groupBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class
        {
            int totalCount;
            Func<IPagedList<TAny>> acquire = (() =>
            {
                DynamicParameters dp;
                string sqlText = GetPagedListSqlText(tables, fields, pageIndex, pageSize, orderBy, groupBy, out dp, parameters);

                var list = Connection.Query<TAny>(sqlText, dp);
                totalCount = dp.Get<int?>("@RowCount") ?? 0;
                return new PagedList<TAny>()
                {
                    Items = list.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return ReadUncommitted(acquire, withNoLock);
        }
        public async Task<IPagedList<TAny>> GetPagedListQueryAsync<TAny>(string tables, string fields, int pageIndex, int pageSize, string orderBy, bool withNoLock = true, params Parameter[] parameters) where TAny : class
        {
            int totalCount;
            Func<Task<IPagedList<TAny>>> acquire = (async () =>
            {
                DynamicParameters dp;
                string sqlText = GetPagedListSqlText(tables, fields, pageIndex, pageSize, orderBy, out dp, parameters);

                var list = await Connection.QueryAsync<TAny>(sqlText, dp);
                totalCount = dp.Get<int?>("@RowCount") ?? 0;
                return new PagedList<TAny>()
                {
                    Items = list.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

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
        /// <param name="withNoLock"></param>
        /// <returns>当前分页的数据</returns>
        public IPagedList<TAny> GetPagedListQuery<TAny>(string tables, string fields, int pageIndex, int pageSize, string sqlWhere, string orderBy, DynamicParameters dynamicParameters, bool withNoLock = true)
        {

            int totalCount = 0;
            Func<IPagedList<TAny>> acquire = (() =>
            {
                string sqlText = string.Empty;
                if (dynamicParameters == null)
                    dynamicParameters = new DynamicParameters();

                dynamicParameters.Add("@RowCount", totalCount, DbType.Int32, ParameterDirection.Output);
                if (pageSize < 0)
                {
                    sqlText = string.Format(" SELECT {0} FROM {1} {2} ORDER BY {3} ; SELECT @RowCount=COUNT(*) FROM {1} {2};", fields, tables, sqlWhere, orderBy);
                }
                else
                {
                    if (pageIndex <= 0)
                    {
                        sqlText = string.Format(" SELECT TOP {0} {1} FROM {2} {3} ORDER BY {4} ; SELECT @RowCount=COUNT(*) FROM {2} {3} ", pageSize, fields, tables, sqlWhere, orderBy);
                    }
                    else
                    {
                        sqlText = string.Format(" SELECT * FROM(SELECT TOP {1} ROW_NUMBER() OVER(ORDER BY {2}) RowNumberIndex,{0} FROM {3} {4}) temTab1 WHERE RowNumberIndex > {5} ORDER BY RowNumberIndex ; SELECT @RowCount=COUNT(*) FROM {3} {4}; ", fields, (pageIndex+1) * pageSize, orderBy, tables, sqlWhere, pageIndex * pageSize);
                    }
                }
                var list = Connection.Query<TAny>(sqlText, dynamicParameters).ToList();
                totalCount = dynamicParameters.Get<int?>("@RowCount") ?? 0;
                return new PagedList<TAny>()
                {
                    Items = list.ToList(),
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return ReadUncommitted(acquire, withNoLock);
        }
        #endregion

        #region sql语句查询

       
        public int Execute(string query, object parameters = null)
        {
            return Connection.Execute(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            var data = await Connection.ExecuteAsync(query, parameters);
            return data;
        }

        public T ExecuteScalar<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<T> acquire = (() =>
           {
               return Connection.ExecuteScalar<T>(query, parameters);
           });
            return ReadUncommitted(acquire, withNoLock);
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object parameters = null, bool withNoLock = false)
        {
            Func<Task<T>> acquire = (async () =>
           {
               var data = await Connection.ExecuteScalarAsync<T>(query, parameters);
               return data;
           });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// 根据主键 ，查询一条记录
        /// </summary>
        /// <typeparam name="T">数据库表对应的实体类型</typeparam>
        /// <param name="connection">sql连接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <param name="tableName">数据库表名</param>
        /// <returns>数据库表对应的实体</returns>
        public T QueryFirstOrDefault<T>(string primaryKeyName, object primaryKeyValue, string tableName, bool withNoLock = true)
        {
            Func<T> acquire = ( () =>
            {
                string sqlText = string.Format(" SELECT TOP 1 * FROM {0} WHERE {1}=@p_1 ", tableName, primaryKeyName);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@p_1", primaryKeyValue);

                return Connection.QueryFirstOrDefault<T>(sqlText, parameters);
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 根据主键 ，查询一条记录
        /// </summary>
        /// <typeparam name="T">数据库表对应的实体类型</typeparam>
        /// <param name="connection">sql连接</param>
        /// <param name="primaryKeyName">主键字段名</param>
        /// <param name="primaryKeyValue">主键值</param>
        /// <param name="tableName">数据库表名</param>
        /// <returns>数据库表对应的实体</returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string primaryKeyName, object primaryKeyValue, string tableName, bool withNoLock = true)
        {
            Func<Task<T>> acquire = (async () =>
            {
                string sqlText = string.Format(" SELECT TOP 1 * FROM {0} WHERE {1}=@p_1 ", tableName, primaryKeyName);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@p_1", primaryKeyValue);

                return await Connection.QueryFirstOrDefaultAsync<T>(sqlText, parameters);
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <param name="tableName">参数</param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sqlText, DynamicParameters parameters)
        {
            return Connection.QueryFirstOrDefault<T>(sqlText, parameters);
        }

        /// <summary>
        /// sql查询返回第一条实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primaryKeyName"></param>
        /// <param name="primaryKeyValue"></param>
        /// <param name="tableName">参数</param>
        /// <returns></returns>
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sqlText, DynamicParameters parameters)
        {
            return await Connection.QueryFirstOrDefaultAsync<T>(sqlText, parameters);
        }
        /// <summary>
        /// 执行参数化SQL并返回IDataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
        }


        #endregion

        #region 存储过程


        public int ExecuteProc(string StoredProcedure, object parms = null)
        {
            return Connection.Execute(StoredProcedure, parms, null,null, CommandType.StoredProcedure);
        }

        public async Task<int> ExecuteProcAsync(string StoredProcedure, object parms = null)
        {
            var data = await Connection.ExecuteAsync(StoredProcedure, parms, null, null, CommandType.StoredProcedure);
            return data;
        }
        /// <summary>
        /// 执行存储过程，接收return值
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public int ExecuteProcWithReturn(string StoredProcedure, DynamicParameters parms = null)
        {
            if (parms == null)
            {
                parms = new DynamicParameters();
            }
            parms.Add("@return_value", 0, DbType.Int32, ParameterDirection.ReturnValue);

            Connection.Execute(StoredProcedure, parms, null, null, CommandType.StoredProcedure);

            return parms.Get<int>("@return_value");
        }

        public async Task<int> ExecuteProcWithReturnAsync(string StoredProcedure, DynamicParameters parms = null)
        {
            if (parms == null)
            {
                parms = new DynamicParameters();
            }
            parms.Add("@return_value", 0, DbType.Int32, ParameterDirection.ReturnValue);

            await Connection.ExecuteAsync(StoredProcedure, parms, null, null, CommandType.StoredProcedure);

            return parms.Get<int>("@return_value");
        }

        /// <summary>
        ///执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
       public T ExecuteScalarProc<T>(string StoredProcedure, object parms = null, bool withNoLock = false)
        {
            Func<T> acquire = (() =>
            {
                var data = Connection.ExecuteScalar<T>(StoredProcedure, parms, null, null, CommandType.StoredProcedure);
                return data;
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        /// <summary>
        /// 异步执行存储过程 查询单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarProcAsync<T>(string StoredProcedure, object parms = null, bool withNoLock = false)
        {
            Func<Task<T>> acquire = (async () =>
            {
                var data = await Connection.ExecuteScalarAsync<T>(StoredProcedure, parms, null, null, CommandType.StoredProcedure);
                return data;
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }


        /// <summary>
        /// 执行存储过程，返回list集合
        /// </summary>
        /// <typeparam name="TAny"></typeparam>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public IList<TAny> QueryProc<TAny>(string StoredProcedure, object parms = null, bool withNoLock = true) where TAny : class
        {
            Func<IList<TAny>> acquire = (() =>
           {
               return Connection.Query<TAny>(StoredProcedure, parms, null).ToList();
           });
            return ReadUncommitted(acquire, withNoLock);
        }

        public async Task<IList<TAny>> QueryProcAsync<TAny>(string StoredProcedure, object parms = null, bool withNoLock = true) where TAny : class
        {
            Func<Task<IList<TAny>>> acquire = (async () =>
            {
                var data = await Connection.QueryAsync<TAny>(StoredProcedure, parms, null);
                return data.ToList();
            });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }

        public IPagedList<TAny> GetPagedListProc<TAny>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = true) where TAny : class
        {
            Func<IPagedList<TAny>> acquire = (() =>
            {
                var data = QueryProc<TAny>(StoredProcedure, parms, withNoLock);
                int totalCount = 0;
                int pageIndex = 0;
                int pageSize = 0;
                if (parms != null )
                {
                    if (!string.IsNullOrWhiteSpace(totalCountName))
                    {
                        totalCount = parms.Get<int>($"@{totalCountName}");
                    }
                    if (!string.IsNullOrWhiteSpace(pageIndexName))
                    {
                        pageIndex = parms.Get<int>($"@{pageIndexName}");
                    }
                    if (!string.IsNullOrWhiteSpace(pageSizeName))
                    {
                        pageSize = parms.Get<int>($"@{pageSizeName}");
                    }
                }
                return new PagedList<TAny>()
                {
                    Items = data.ToList(),
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = GetTotalPages(totalCount, pageSize)
                };
            });
            return ReadUncommitted(acquire, withNoLock);
        }

        

        public async Task<IPagedList<TAny>> GetPagedListProcAsync<TAny>(string StoredProcedure, string totalCountName = "CountPage", string pageIndexName = "PageIndex", string pageSizeName = "PageSize", DynamicParameters parms = null, bool withNoLock = true) where TAny : class
        {
            Func<Task<IPagedList<TAny>>> acquire = (async () =>
           {
               var data = await QueryProcAsync<TAny>(StoredProcedure, parms, withNoLock: withNoLock);
               int totalCount = 0;
               int pageIndex = 0;
               int pageSize = 0;
               if (parms != null)
               {
                   if (!string.IsNullOrWhiteSpace(totalCountName))
                   {
                       totalCount = parms.Get<int>($"@{totalCountName}");
                   }
                   if (!string.IsNullOrWhiteSpace(pageIndexName))
                   {
                       pageIndex = parms.Get<int>($"@{pageIndexName}");
                   }
                   if (!string.IsNullOrWhiteSpace(pageSizeName))
                   {
                       pageSize = parms.Get<int>($"@{pageSizeName}");
                   }
               }
               return new PagedList<TAny>()
               {
                   Items = data,
                   TotalCount = totalCount,
                   PageIndex = pageIndex,
                   PageSize = pageSize,
                   TotalPages = pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
               };
           });
            return await ReadUncommittedAsync(acquire, withNoLock);
        }


        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public GridReader QueryProcMultiple(string StoredProcedure, object parms = null)
        {
            Func<GridReader> acquire = (() =>
            {
                return Connection.QueryMultiple(StoredProcedure, parms,null);
            });
            return ReadUncommitted(acquire, false);
        }

        /// <summary>
        /// 执行存储过程，返回多个结果集
        /// </summary>
        /// <param name="StoredProcedure"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<GridReader> QueryProcMultipleAsync(string StoredProcedure, object parms = null)
        {
            Func<Task<GridReader>> acquire = (async () =>
            {
                var data = await Connection.QueryMultipleAsync(StoredProcedure, parms, null);
                return data;
            });
            return await ReadUncommittedAsync(acquire, false);
        }
        #endregion

        #region 辅助方法 参数组成

        /// <summary>
        /// sql 拼接
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="fields"></param>
        /// <param name="dp"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GetListSqlText(string tables, string fields, out DynamicParameters dp, params Parameter[] parameters)
        {
            string sqlWhere = string.Empty;
            dp = GetDynamicParameter(ref sqlWhere, parameters);
            sqlWhere = " WHERE 1=1 " + sqlWhere;
            string sqlText = string.Empty;
            sqlText = string.Format(" SELECT {0} FROM {1} {2} ", fields, tables, sqlWhere);
            return sqlText;
        }

        private string GetPagedListSqlText(string tables, string fields, int pageIndex, int pageSize, string orderBy, out DynamicParameters dp, params Parameter[] parameters)
        {
            string sqlWhere = string.Empty;
            int totalCount = 0;
            dp = GetDynamicParameter(ref sqlWhere, parameters);
            sqlWhere = " WHERE 1=1 " + sqlWhere;

            string sqlText = string.Empty;

            if (dp == null)
                dp = new DynamicParameters();
            dp.Add("@RowCount", totalCount, DbType.Int32, ParameterDirection.Output);
            if (pageSize < 0)
            {
                sqlText = string.Format(" SELECT {0} FROM {1} {2} ORDER BY {3} ; SELECT @RowCount=COUNT(*) FROM {1} {2};", fields, tables, sqlWhere, orderBy);
            }
            else
            {
                if (pageIndex <= 0)
                {
                    sqlText = string.Format(" SELECT TOP {0} {1} FROM {2} {3} ORDER BY {4} ; SELECT @RowCount=COUNT(*) FROM {2} {3} ", pageSize, fields, tables, sqlWhere, orderBy);
                }
                else
                {
                    sqlText = string.Format(" SELECT * FROM(SELECT TOP {1} ROW_NUMBER() OVER(ORDER BY {2}) RowNumberIndex,{0} FROM {3} {4}) temTab1 WHERE RowNumberIndex > {5} ORDER BY RowNumberIndex ; SELECT @RowCount=COUNT(*) FROM {3} {4}; ", fields, (pageIndex+1) * pageSize, orderBy, tables, sqlWhere, pageIndex * pageSize);
                }
            }
            return sqlText;
        }

        private string GetPagedListSqlText(string tables, string fields, int pageIndex, int pageSize, string orderBy,string groupBy, out DynamicParameters dp, params Parameter[] parameters)
        {
            string sqlWhere = string.Empty;
            int totalCount = 0;
            dp = GetDynamicParameter(ref sqlWhere, parameters);
            sqlWhere = " WHERE 1=1 " + sqlWhere+ groupBy;

            string sqlText = string.Empty;

            if (dp == null)
                dp = new DynamicParameters();
            dp.Add("@RowCount", totalCount, DbType.Int32, ParameterDirection.Output);
            if (pageSize < 0)
            {
                sqlText = string.Format(" SELECT {0} FROM {1} {2} ORDER BY {3} ; SELECT @RowCount=COUNT(*) FROM {1} {2};", fields, tables, sqlWhere, orderBy);
            }
            else
            {
                if (pageIndex <= 0)
                {
                    sqlText = string.Format(" SELECT TOP {0} {1} FROM {2} {3} ORDER BY {4} ; SELECT @RowCount=COUNT(*) FROM {2} {3} ", pageSize, fields, tables, sqlWhere, orderBy);
                }
                else
                {
                    sqlText = string.Format(" SELECT * FROM(SELECT TOP {1} ROW_NUMBER() OVER(ORDER BY {2}) RowNumberIndex,{0} FROM {3} {4}) temTab1 WHERE RowNumberIndex > {5} ORDER BY RowNumberIndex ; SELECT @RowCount=COUNT(*) FROM {3} {4}; ", fields, (pageIndex+1) * pageSize, orderBy, tables, sqlWhere, pageIndex * pageSize);
                }
            }
            return sqlText;
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        private int GetTotalPages(int totalCount, int pageSize)
        {
            return pageSize == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// 把Parameter参数数组转换成DynamicParameters参数，并且输出sqlWhere语句（不包含【WHERE】关键字）
        /// </summary>
        /// <param name="sqlWhere">不包含【WHERE】和【WHERE 1=1】的sqlWhere语句</param>
        /// <param name="pars">Parameter参数数组</param>
        /// <returns>DynamicParameters参数</returns>
        protected static DynamicParameters GetDynamicParameter(ref string sqlWhere, Parameter[] pars)
        {
            if (pars == null || pars.Length == 0)
            {
                sqlWhere = string.Empty;
                return null;
            }
            DynamicParameters dp = new DynamicParameters();
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Parameter item in pars)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.Like:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.LeftLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value + "%");
                        break;
                    case OperateType.RightLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value);
                        break;
                    case OperateType.NotLike:
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), "%" + item.Value + "%");
                        break;
                    case OperateType.In:
                        Array arr = item.Value as Array;
                        if (arr != null)
                            sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr));
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                            sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr1));
                        break;
                    case OperateType.SqlFormat:
                        object[] arr2 = item.Value as object[];
                        if (arr2 != null)
                            sb.AppendFormat(item.Name, ArrayToString(arr2));
                        else
                            sb.AppendFormat(item.Name, item.Value);
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr3 = item.Value as object[];
                        if (arr3 != null)
                        {
                            string[] ps = new string[arr3.Length];
                            for (int i = 0; i < arr3.Length; i++)
                            {
                                ps[i] = "@p_" + index.ToString();
                                dp.Add("@p_" + index.ToString(), arr3[i]);
                                index++;
                            }
                            sb.AppendFormat(item.Name, ps);
                        }
                        else
                        {
                            sb.AppendFormat(item.Name, "@p_" + index.ToString());
                            dp.Add("@p_" + index.ToString(), item.Value);
                        }
                        break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@p_" + index.ToString());
                        dp.Add("@p_" + index.ToString(), item.Value);
                        break;
                    default:
                        break;
                }
                index++;
            }
            sqlWhere = sb.ToString();
            return dp;
        }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        protected string GetSqlParameter(params Parameter[] pars)
        {
            if (pars == null || pars.Length == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (Parameter item in pars)
            {
                if (item == null)
                    continue;
                switch (item.OperateType)
                {
                    case OperateType.Equal:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} = {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.NotEqual:
                        if (item.Value is DBNull)
                            sb.AppendFormat(" {0} {1} IS NOT NULL ", item.LogicType.ToString(), item.Name);
                        else
                        {
                            sb.AppendFormat(" {0} {1} != {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.Greater:
                        sb.AppendFormat(" {0} {1} > {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.GreaterEqual:
                        sb.AppendFormat(" {0} {1} >= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.Less:
                        sb.AppendFormat(" {0} {1} < {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.LessEqual:
                        sb.AppendFormat(" {0} {1} <= {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.Like:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.LeftLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.RightLike:
                        sb.AppendFormat(" {0} {1} LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.NotLike:
                        sb.AppendFormat(" {0} {1} NOT LIKE {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.In:
                        Array arr = item.Value as Array;
                        if (arr != null)
                            sb.AppendFormat(" {0} {1} IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr));
                        else
                            sb.AppendFormat(" {0} 1<>1 ", item.LogicType.ToString());
                        break;
                    case OperateType.NotIn:
                        Array arr1 = item.Value as Array;
                        if (arr1 != null)
                            sb.AppendFormat(" {0} {1} NOT IN ({2}) ", item.LogicType.ToString(), item.Name, ArrayToString(arr1));
                        break;
                    case OperateType.SqlFormat:
                        object[] arr2 = item.Value as object[];
                        if (arr2 != null)
                            sb.AppendFormat(item.Name, ArrayToString(arr2));
                        else
                            sb.AppendFormat(item.Name, item.Value);
                        break;
                    case OperateType.SqlFormatPar:
                        object[] arr3 = item.Value as object[];
                        if (arr3 != null)
                        {
                            string[] ps = new string[arr3.Length];
                            for (int i = 0; i < arr3.Length; i++)
                            {
                                ps[i] = "@p_" + index.ToString();

                                index++;
                            }
                            sb.AppendFormat(item.Name, ps);
                        }
                        else
                        {
                            sb.AppendFormat(item.Name, "@p_" + index.ToString());

                        }
                        break;
                    case OperateType.Between:
                        sb.AppendFormat(" {0} {1} BETWEEN {2} ", item.LogicType.ToString(), item.Name, "@p_" + index.ToString());

                        break;
                    case OperateType.End:
                        sb.AppendFormat(" {0} {1} ", item.LogicType.ToString(), "@p_" + index.ToString());

                        break;
                    default:
                        break;
                }
                index++;
            }
            return sb.ToString();
        }

        private static string ArrayToString(Array arr)
        {
            if (arr.GetLength(0) == 0)
                return string.Empty;

            object o = arr.GetValue(0);
            string[] str = new string[arr.GetLength(0)];
            if (o is int || o is decimal || o is double || o is float || o is long)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString();
                }
                return string.Join(",", str);
            }
            else
            {
                for (int i = 0; i < str.Length; i++)
                {
                    str[i] = arr.GetValue(i).ToString().Replace("'", "''");
                }
                return "'" + string.Join("','", str) + "'";
            }
        }
        #endregion
    }
}
