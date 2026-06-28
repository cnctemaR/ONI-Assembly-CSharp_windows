using System;
using System.CodeDom.Compiler;

namespace System.Data
{
	internal class ClassICodeGeneratorOptions : ClassGeneratorOptions
	{
		public ClassICodeGeneratorOptions(ICodeGenerator codeGen)
		{
			this.gen = codeGen;
		}

		internal override string DataSetName(string source)
		{
			if (this.CreateDataSetName != null)
			{
				return this.CreateDataSetName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen);
		}

		internal override string TableTypeName(string source)
		{
			if (this.CreateTableTypeName != null)
			{
				return this.CreateTableTypeName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "DataTable";
		}

		internal override string TableMemberName(string source)
		{
			if (this.CreateTableMemberName != null)
			{
				return this.CreateTableMemberName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen);
		}

		internal override string TableColName(string source)
		{
			if (this.CreateTableColumnName != null)
			{
				return this.CreateTableColumnName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen);
		}

		internal override string TableDelegateName(string source)
		{
			if (this.CreateTableDelegateName != null)
			{
				return this.CreateTableDelegateName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "RowChangedEventHandler";
		}

		internal override string EventArgsName(string source)
		{
			if (this.CreateEventArgsName != null)
			{
				return this.CreateEventArgsName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "RowChangedEventArgs";
		}

		internal override string ColumnName(string source)
		{
			if (this.CreateColumnName != null)
			{
				return this.CreateColumnName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen);
		}

		internal override string RowName(string source)
		{
			if (this.CreateRowName != null)
			{
				return this.CreateRowName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "Row";
		}

		internal override string RelationName(string source)
		{
			if (this.CreateRelationName != null)
			{
				return this.CreateRelationName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "Relation";
		}

		internal override string TableAdapterNSName(string source)
		{
			if (this.CreateTableAdapterNSName != null)
			{
				return this.CreateTableAdapterNSName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen) + "TableAdapters";
		}

		internal override string TableAdapterName(string source)
		{
			if (this.CreateTableAdapterName != null)
			{
				return this.CreateTableAdapterName(source, this.gen);
			}
			return CustomDataClassGenerator.MakeSafeName(source, this.gen);
		}

		private ICodeGenerator gen;

		public CodeNamingMethod CreateDataSetName;

		public CodeNamingMethod CreateTableTypeName;

		public CodeNamingMethod CreateTableMemberName;

		public CodeNamingMethod CreateTableColumnName;

		public CodeNamingMethod CreateColumnName;

		public CodeNamingMethod CreateRowName;

		public CodeNamingMethod CreateRelationName;

		public CodeNamingMethod CreateTableDelegateName;

		public CodeNamingMethod CreateEventArgsName;

		public CodeNamingMethod CreateTableAdapterNSName;

		public CodeNamingMethod CreateTableAdapterName;
	}
}
