﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE
#if EFCORE

using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        /// <summary>A DbContext extension method that creates a new store command.</summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>The new store command from the DbContext.</returns>
        public static DbCommand CreateStoreCommand(this DbContext context)
        {
            DbCommand command = null;
#if NETSTANDARD2_0
            if (context.Database.ProviderName == "Devart.Data.Oracle.Entity.EFCore")
            {
                var oracleRelationalDatabaseFacadeExtensions = context.Database.GetDbConnection().GetType().Assembly
                    .GetType("Microsoft.EntityFrameworkCore.OracleRelationalDatabaseFacadeExtensions");
                var getOracleConnectionMethod = oracleRelationalDatabaseFacadeExtensions.GetMethod("GetOracleConnection", BindingFlags.Static | BindingFlags.Public);
                var entityConnection = getOracleConnectionMethod.Invoke(oracleRelationalDatabaseFacadeExtensions, new []{ context.Database });
                command = ((dynamic) entityConnection).CreateCommand();
            }
            else
            {  
#endif 
            var entityConnection = context.Database.GetDbConnection();
            command = entityConnection.CreateCommand();

#if NETSTANDARD2_0
            }
#endif

            var entityTransaction = context.Database.GetService<IRelationalConnection>().CurrentTransaction;
            if (entityTransaction != null)
            {
                command.Transaction = entityTransaction.GetDbTransaction();
            }

            var commandTimeout = context.Database.GetCommandTimeout();
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }

            return command;
        }
    }
}

#endif
#endif