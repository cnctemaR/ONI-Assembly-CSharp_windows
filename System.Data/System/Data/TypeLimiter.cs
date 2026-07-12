using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Data
{
	internal sealed class TypeLimiter
	{
		private TypeLimiter(TypeLimiter.Scope scope)
		{
			this.m_instanceScope = scope;
		}

		private static bool IsTypeLimitingDisabled
		{
			get
			{
				return global::System.LocalAppContextSwitches.AllowArbitraryTypeInstantiation;
			}
		}

		[NullableContext(2)]
		public static TypeLimiter Capture()
		{
			TypeLimiter.Scope scope = TypeLimiter.s_activeScope;
			if (scope == null)
			{
				return null;
			}
			return new TypeLimiter(scope);
		}

		[NullableContext(2)]
		public static void EnsureTypeIsAllowed(Type type, TypeLimiter capturedLimiter = null)
		{
			if (type == null)
			{
				return;
			}
			TypeLimiter.Scope scope = ((capturedLimiter != null) ? capturedLimiter.m_instanceScope : null) ?? TypeLimiter.s_activeScope;
			if (scope == null)
			{
				return;
			}
			if (scope.IsAllowedType(type))
			{
				return;
			}
			throw ExceptionBuilder.TypeNotAllowed(type);
		}

		[return: Nullable(2)]
		public static IDisposable EnterRestrictedScope(DataSet dataSet)
		{
			if (TypeLimiter.IsTypeLimitingDisabled)
			{
				return null;
			}
			return TypeLimiter.s_activeScope = new TypeLimiter.Scope(TypeLimiter.s_activeScope, TypeLimiter.GetPreviouslyDeclaredDataTypes(dataSet));
		}

		[return: Nullable(2)]
		public static IDisposable EnterRestrictedScope(DataTable dataTable)
		{
			if (TypeLimiter.IsTypeLimitingDisabled)
			{
				return null;
			}
			return TypeLimiter.s_activeScope = new TypeLimiter.Scope(TypeLimiter.s_activeScope, TypeLimiter.GetPreviouslyDeclaredDataTypes(dataTable));
		}

		private static IEnumerable<Type> GetPreviouslyDeclaredDataTypes(DataTable dataTable)
		{
			if (dataTable == null)
			{
				return Enumerable.Empty<Type>();
			}
			return from DataColumn column in dataTable.Columns
				select column.DataType;
		}

		private static IEnumerable<Type> GetPreviouslyDeclaredDataTypes(DataSet dataSet)
		{
			if (dataSet == null)
			{
				return Enumerable.Empty<Type>();
			}
			return dataSet.Tables.Cast<DataTable>().SelectMany<DataTable, Type>((DataTable table) => TypeLimiter.GetPreviouslyDeclaredDataTypes(table));
		}

		[Nullable(2)]
		[ThreadStatic]
		private static TypeLimiter.Scope s_activeScope;

		private TypeLimiter.Scope m_instanceScope;

		private const string AppDomainDataSetDefaultAllowedTypesKey = "System.Data.DataSetDefaultAllowedTypes";

		private sealed class Scope : IDisposable
		{
			internal Scope([Nullable(2)] TypeLimiter.Scope previousScope, IEnumerable<Type> allowedTypes)
			{
				this.m_previousScope = previousScope;
				this.m_allowedTypes = new HashSet<Type>(allowedTypes.Where<Type>((Type type) => type != null));
			}

			public void Dispose()
			{
				if (this != TypeLimiter.s_activeScope)
				{
					throw new ObjectDisposedException(base.GetType().FullName);
				}
				TypeLimiter.s_activeScope = this.m_previousScope;
			}

			public bool IsAllowedType(Type type)
			{
				if (TypeLimiter.Scope.IsTypeUnconditionallyAllowed(type))
				{
					return true;
				}
				for (TypeLimiter.Scope scope = this; scope != null; scope = scope.m_previousScope)
				{
					if (scope.m_allowedTypes.Contains(type))
					{
						return true;
					}
				}
				Type[] array = (Type[])AppDomain.CurrentDomain.GetData("System.Data.DataSetDefaultAllowedTypes");
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (type == array[i])
						{
							return true;
						}
					}
				}
				return false;
			}

			private static bool IsTypeUnconditionallyAllowed(Type type)
			{
				while (!TypeLimiter.Scope.s_allowedTypes.Contains(type))
				{
					if (type.IsEnum)
					{
						return true;
					}
					if (type.IsSZArray)
					{
						type = type.GetElementType();
					}
					else
					{
						if (!type.IsGenericType || type.IsGenericTypeDefinition || !(type.GetGenericTypeDefinition() == typeof(List<>)))
						{
							return false;
						}
						type = type.GetGenericArguments()[0];
					}
				}
				return true;
			}

			private static readonly HashSet<Type> s_allowedTypes = new HashSet<Type>
			{
				typeof(bool),
				typeof(char),
				typeof(sbyte),
				typeof(byte),
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(float),
				typeof(double),
				typeof(decimal),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(string),
				typeof(Guid),
				typeof(SqlBinary),
				typeof(SqlBoolean),
				typeof(SqlByte),
				typeof(SqlBytes),
				typeof(SqlChars),
				typeof(SqlDateTime),
				typeof(SqlDecimal),
				typeof(SqlDouble),
				typeof(SqlGuid),
				typeof(SqlInt16),
				typeof(SqlInt32),
				typeof(SqlInt64),
				typeof(SqlMoney),
				typeof(SqlSingle),
				typeof(SqlString),
				typeof(object),
				typeof(Type),
				typeof(BigInteger),
				typeof(Uri),
				typeof(Color),
				typeof(Point),
				typeof(PointF),
				typeof(Rectangle),
				typeof(RectangleF),
				typeof(Size),
				typeof(SizeF)
			};

			private HashSet<Type> m_allowedTypes;

			[Nullable(2)]
			private readonly TypeLimiter.Scope m_previousScope;
		}
	}
}
