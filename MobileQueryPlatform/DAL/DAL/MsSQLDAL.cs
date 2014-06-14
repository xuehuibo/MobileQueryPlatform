﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using DAL.Interface;

namespace DAL
{
    internal class MsSQLDAL:IDisposable,IDAL
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// create by xuehuibo 2014-03-21
        /// </summary>
        /// <param name="connectionString">数据库连接参数</param>
        public MsSQLDAL(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            try
            {
                Connection.Open();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 私有变量
        /// <summary>
        /// 数据库连接
        /// </summary>
        private SqlConnection Connection
        {
            get;
            set;
        }

        /// <summary>
        /// 事务
        /// </summary>
        private SqlTransaction Tran
        {
            get;
            set;
        }

        #endregion

        #region Query

        /// <summary>
        /// 查询
        /// create by xuehuibo 2014-03-21
        /// </summary>
        /// <param name="sqlString">sql语句</param>
        /// <param name="paramCollection">参数集合</param>
        /// <returns></returns>
        public DataTable Select(string sqlString,out int i, params IDbDataParameter[] paramArray)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sqlString, Connection);
                cmd.Transaction = Tran;
                cmd.Parameters.AddRange(paramArray);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                i=adapter.Fill(dataTable);
                return dataTable;
            }
            catch
            {
                throw;
            }
        }

        public IDataReader Select(string sqlString, params IDbDataParameter[] paramArray)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sqlString, Connection);
                cmd.Transaction = Tran;
                cmd.Parameters.AddRange(paramArray);
                return cmd.ExecuteReader();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Procedure

        /// <summary>
        /// 执行存储过程
        /// create by xuehuibo 2014-03-21
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="paramArray">存储过程参数</param>
        /// <returns></returns>
        public DataSet RunProcedure(string procedureName, params IDbDataParameter[] paramArray)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlTransaction tran = Tran;
                SqlCommand cmd = new SqlCommand(procedureName, Connection);
                cmd.Parameters.AddRange(paramArray);
                cmd.Transaction = tran;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(ds);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region Insert
        public bool Save(DataTable dt, out int i)
        {
            try
            {
                SqlBulkCopy sbc = new SqlBulkCopy(Connection,SqlBulkCopyOptions.Default,Tran);
                sbc.DestinationTableName=dt.TableName;
                sbc.WriteToServer(dt);
                i = dt.Rows.Count;
                sbc.Close();
                return true;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Execute
        public bool Execute(string sqlString, out int i, params IDbDataParameter[] Params)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sqlString, Connection);
                cmd.Transaction = Tran;
                cmd.Parameters.AddRange(Params);
                i = cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            //关闭连接并释放
            Connection.Close();
            Connection = null;
        }

        #endregion

        #region Transcation

        public void BeginTran()
        {
            try
            {
                Tran = Connection.BeginTransaction();
            }
            catch 
            {
                throw;
            }
        }

        public void CommitTran()
        {
            try
            {
                Tran.Commit();
            }
            catch
            {
                try
                {
                    Tran.Rollback();
                }
                catch
                {

                }
                throw;
            }
        }

        public void RollBackTran()
        {
            try
            {
                Tran.Rollback();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region ParameterCreater

        public IDbDataParameter CreateParameter(string paramName, object value)
        {
            return new SqlParameter(paramName, value);
        }

        public IDbDataParameter CreateParameter(string paramName, DbType dbType)
        {
            return new SqlParameter()
            {
                ParameterName=paramName,
                DbType=dbType
            };
        }

        public IDbDataParameter CreateParameter(string paramName, DbType dbType, int size)
        {
            return new SqlParameter()
            {
                ParameterName=paramName,
                DbType=dbType,
                Size=size
            };
        }

        public IDbDataParameter CreateParameter(string paramName, DbType dbType, ParameterDirection direction)
        {
            return new SqlParameter()
            {
                ParameterName=paramName,
                DbType=dbType,
                Direction=direction
            };
        }

        public IDbDataParameter CreateParameter(string paramName, DbType dbType, int size, ParameterDirection direction)
        {
            return new SqlParameter()
            {
                ParameterName = paramName,
                DbType = dbType,
                Direction = direction,
                Size=size
            };
        }

        #endregion
    }
}