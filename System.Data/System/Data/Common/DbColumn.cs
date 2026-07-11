using System;

namespace System.Data.Common
{
	public abstract class DbColumn
	{
		public bool? AllowDBNull { get; protected set; }

		public string BaseCatalogName { get; protected set; }

		public string BaseColumnName { get; protected set; }

		public string BaseSchemaName { get; protected set; }

		public string BaseServerName { get; protected set; }

		public string BaseTableName { get; protected set; }

		public string ColumnName { get; protected set; }

		public int? ColumnOrdinal { get; protected set; }

		public int? ColumnSize { get; protected set; }

		public bool? IsAliased { get; protected set; }

		public bool? IsAutoIncrement { get; protected set; }

		public bool? IsExpression { get; protected set; }

		public bool? IsHidden { get; protected set; }

		public bool? IsIdentity { get; protected set; }

		public bool? IsKey { get; protected set; }

		public bool? IsLong { get; protected set; }

		public bool? IsReadOnly { get; protected set; }

		public bool? IsUnique { get; protected set; }

		public int? NumericPrecision { get; protected set; }

		public int? NumericScale { get; protected set; }

		public string UdtAssemblyQualifiedName { get; protected set; }

		public Type DataType { get; protected set; }

		public string DataTypeName { get; protected set; }

		public virtual object this[string property]
		{
			get
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(property);
				if (num <= 2477638934U)
				{
					if (num <= 1067318116U)
					{
						if (num <= 687909556U)
						{
							if (num != 405521230U)
							{
								if (num == 687909556U)
								{
									if (property == "ColumnOrdinal")
									{
										return this.ColumnOrdinal;
									}
								}
							}
							else if (property == "DataTypeName")
							{
								return this.DataTypeName;
							}
						}
						else if (num != 720006947U)
						{
							if (num != 1005639113U)
							{
								if (num == 1067318116U)
								{
									if (property == "ColumnName")
									{
										return this.ColumnName;
									}
								}
							}
							else if (property == "IsHidden")
							{
								return this.IsHidden;
							}
						}
						else if (property == "IsLong")
						{
							return this.IsLong;
						}
					}
					else if (num <= 2215472237U)
					{
						if (num != 1154057342U)
						{
							if (num != 1309233724U)
							{
								if (num == 2215472237U)
								{
									if (property == "DataType")
									{
										return this.DataType;
									}
								}
							}
							else if (property == "IsKey")
							{
								return this.IsKey;
							}
						}
						else if (property == "ColumnSize")
						{
							return this.ColumnSize;
						}
					}
					else if (num != 2239129947U)
					{
						if (num != 2380251540U)
						{
							if (num == 2477638934U)
							{
								if (property == "IsUnique")
								{
									return this.IsUnique;
								}
							}
						}
						else if (property == "NumericPrecision")
						{
							return this.NumericPrecision;
						}
					}
					else if (property == "IsExpression")
					{
						return this.IsExpression;
					}
				}
				else if (num <= 3042527364U)
				{
					if (num <= 2711511624U)
					{
						if (num != 2504653387U)
						{
							if (num != 2586490225U)
							{
								if (num == 2711511624U)
								{
									if (property == "BaseServerName")
									{
										return this.BaseServerName;
									}
								}
							}
							else if (property == "UdtAssemblyQualifiedName")
							{
								return this.UdtAssemblyQualifiedName;
							}
						}
						else if (property == "IsIdentity")
						{
							return this.IsIdentity;
						}
					}
					else if (num != 2741140585U)
					{
						if (num != 2757192823U)
						{
							if (num == 3042527364U)
							{
								if (property == "BaseCatalogName")
								{
									return this.BaseCatalogName;
								}
							}
						}
						else if (property == "BaseTableName")
						{
							return this.BaseTableName;
						}
					}
					else if (property == "BaseColumnName")
					{
						return this.BaseColumnName;
					}
				}
				else if (num <= 3656290791U)
				{
					if (num != 3115085976U)
					{
						if (num != 3173893005U)
						{
							if (num == 3656290791U)
							{
								if (property == "IsReadOnly")
								{
									return this.IsReadOnly;
								}
							}
						}
						else if (property == "AllowDBNull")
						{
							return this.AllowDBNull;
						}
					}
					else if (property == "BaseSchemaName")
					{
						return this.BaseSchemaName;
					}
				}
				else if (num != 3912158903U)
				{
					if (num != 3938522122U)
					{
						if (num == 4233439846U)
						{
							if (property == "IsAliased")
							{
								return this.IsAliased;
							}
						}
					}
					else if (property == "NumericScale")
					{
						return this.NumericScale;
					}
				}
				else if (property == "IsAutoIncrement")
				{
					return this.IsAutoIncrement;
				}
				return null;
			}
		}
	}
}
