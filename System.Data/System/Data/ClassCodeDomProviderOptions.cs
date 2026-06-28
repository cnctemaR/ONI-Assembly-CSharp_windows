using System;
using System.CodeDom.Compiler;

namespace System.Data
{
	internal class ClassCodeDomProviderOptions : ClassGeneratorOptions
	{
		public ClassCodeDomProviderOptions(CodeDomProvider codeProvider)
		{
			this.provider = codeProvider;
		}

		internal override string DataSetName(string source)
		{
			if (this.CreateDataSetName != null)
			{
				return this.CreateDataSetName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider);
		}

		internal override string TableTypeName(string source)
		{
			if (this.CreateTableTypeName != null)
			{
				return this.CreateTableTypeName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "DataTable";
		}

		internal override string TableMemberName(string source)
		{
			if (this.CreateTableMemberName != null)
			{
				return this.CreateTableMemberName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider);
		}

		internal override string TableColName(string source)
		{
			if (this.CreateTableColumnName != null)
			{
				return this.CreateTableColumnName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider);
		}

		internal override string TableDelegateName(string source)
		{
			if (this.CreateTableDelegateName != null)
			{
				return this.CreateTableDelegateName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "RowChangedEventHandler";
		}

		internal override string EventArgsName(string source)
		{
			if (this.CreateEventArgsName != null)
			{
				return this.CreateEventArgsName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "RowChangedEventArgs";
		}

		internal override string ColumnName(string source)
		{
			if (this.CreateColumnName != null)
			{
				return this.CreateColumnName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider);
		}

		internal override string RowName(string source)
		{
			if (this.CreateRowName != null)
			{
				return this.CreateRowName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "Row";
		}

		internal override string RelationName(string source)
		{
			if (this.CreateRelationName != null)
			{
				return this.CreateRelationName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "Relation";
		}

		internal override string TableAdapterNSName(string source)
		{
			if (this.CreateTableAdapterNSName != null)
			{
				return this.CreateTableAdapterNSName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider) + "TableAdapters";
		}

		internal override string TableAdapterName(string source)
		{
			if (this.CreateTableAdapterName != null)
			{
				return this.CreateTableAdapterName(source, this.provider);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.provider);
		}

		private CodeDomProvider provider;

		public CodeDomNamingMethod CreateDataSetName;

		public CodeDomNamingMethod CreateTableTypeName;

		public CodeDomNamingMethod CreateTableMemberName;

		public CodeDomNamingMethod CreateTableColumnName;

		public CodeDomNamingMethod CreateColumnName;

		public CodeDomNamingMethod CreateRowName;

		public CodeDomNamingMethod CreateRelationName;

		public CodeDomNamingMethod CreateTableDelegateName;

		public CodeDomNamingMethod CreateEventArgsName;

		public CodeDomNamingMethod CreateTableAdapterNSName;

		public CodeDomNamingMethod CreateTableAdapterName;
	}
}
