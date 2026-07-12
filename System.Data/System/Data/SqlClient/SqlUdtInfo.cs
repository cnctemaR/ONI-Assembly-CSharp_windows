using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;

namespace System.Data.SqlClient
{
	internal class SqlUdtInfo
	{
		private SqlUdtInfo(SqlUserDefinedTypeAttribute attr)
		{
			this.SerializationFormat = attr.Format;
			this.IsByteOrdered = attr.IsByteOrdered;
			this.IsFixedLength = attr.IsFixedLength;
			this.MaxByteSize = attr.MaxByteSize;
			this.Name = attr.Name;
			this.ValidationMethodName = attr.ValidationMethodName;
		}

		internal static SqlUdtInfo GetFromType(Type target)
		{
			SqlUdtInfo sqlUdtInfo = SqlUdtInfo.TryGetFromType(target);
			if (sqlUdtInfo == null)
			{
				throw InvalidUdtException.Create(target, "no UDT attribute");
			}
			return sqlUdtInfo;
		}

		internal static SqlUdtInfo TryGetFromType(Type target)
		{
			if (SqlUdtInfo.s_types2UdtInfo == null)
			{
				SqlUdtInfo.s_types2UdtInfo = new Dictionary<Type, SqlUdtInfo>();
			}
			SqlUdtInfo sqlUdtInfo = null;
			if (!SqlUdtInfo.s_types2UdtInfo.TryGetValue(target, out sqlUdtInfo))
			{
				object[] customAttributes = target.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
				if (customAttributes != null && customAttributes.Length == 1)
				{
					sqlUdtInfo = new SqlUdtInfo((SqlUserDefinedTypeAttribute)customAttributes[0]);
				}
				SqlUdtInfo.s_types2UdtInfo.Add(target, sqlUdtInfo);
			}
			return sqlUdtInfo;
		}

		internal readonly Format SerializationFormat;

		internal readonly bool IsByteOrdered;

		internal readonly bool IsFixedLength;

		internal readonly int MaxByteSize;

		internal readonly string Name;

		internal readonly string ValidationMethodName;

		[ThreadStatic]
		private static Dictionary<Type, SqlUdtInfo> s_types2UdtInfo;
	}
}
