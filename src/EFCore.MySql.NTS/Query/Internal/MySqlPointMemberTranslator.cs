// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using NetTopologySuite.Geometries;
using Pomelo.EntityFrameworkCore.MySql.Utilities;

namespace Pomelo.EntityFrameworkCore.MySql.Query.Internal
{
    internal class MySqlPointMemberTranslator : IMemberTranslator
    {
        private static readonly IDictionary<MemberInfo, string> _geometryMemberToFunctionName = new Dictionary<MemberInfo, string>
        {
            { typeof(Point).GetRuntimeProperty(nameof(Point.X)), "ST_X" },
            { typeof(Point).GetRuntimeProperty(nameof(Point.Y)), "ST_Y" },
        };

        private readonly ISqlExpressionFactory _sqlExpressionFactory;

        public MySqlPointMemberTranslator(ISqlExpressionFactory sqlExpressionFactory)
        {
            _sqlExpressionFactory = sqlExpressionFactory;
        }

        public SqlExpression Translate(SqlExpression instance, MemberInfo member, Type returnType, IDiagnosticsLogger<DbLoggerCategory.Query> logger)
        {
            if (typeof(Point).IsAssignableFrom(member.DeclaringType))
            {
                if (_geometryMemberToFunctionName.TryGetValue(member, out var functionName))
                {
                    return _sqlExpressionFactory.Function(
                        functionName,
                        new[] { instance },
                        nullable: true,
                        argumentsPropagateNullability: Statics.TrueArrays[1],
                        returnType);
                }
            }

            return null;
        }
    }
}
