using System;
using System.Text;

namespace System.Data.SqlClient
{
	internal class SqlMetaDataPriv
	{
		internal SqlMetaDataPriv()
		{
		}

		internal virtual void CopyFrom(SqlMetaDataPriv original)
		{
			this.type = original.type;
			this.tdsType = original.tdsType;
			this.precision = original.precision;
			this.scale = original.scale;
			this.length = original.length;
			this.collation = original.collation;
			this.codePage = original.codePage;
			this.encoding = original.encoding;
			this.isNullable = original.isNullable;
			this.udtDatabaseName = original.udtDatabaseName;
			this.udtSchemaName = original.udtSchemaName;
			this.udtTypeName = original.udtTypeName;
			this.udtAssemblyQualifiedName = original.udtAssemblyQualifiedName;
			this.xmlSchemaCollectionDatabase = original.xmlSchemaCollectionDatabase;
			this.xmlSchemaCollectionOwningSchema = original.xmlSchemaCollectionOwningSchema;
			this.xmlSchemaCollectionName = original.xmlSchemaCollectionName;
			this.metaType = original.metaType;
		}

		internal SqlDbType type;

		internal byte tdsType;

		internal byte precision = byte.MaxValue;

		internal byte scale = byte.MaxValue;

		internal int length;

		internal SqlCollation collation;

		internal int codePage;

		internal Encoding encoding;

		internal bool isNullable;

		internal string udtDatabaseName;

		internal string udtSchemaName;

		internal string udtTypeName;

		internal string udtAssemblyQualifiedName;

		internal string xmlSchemaCollectionDatabase;

		internal string xmlSchemaCollectionOwningSchema;

		internal string xmlSchemaCollectionName;

		internal MetaType metaType;
	}
}
