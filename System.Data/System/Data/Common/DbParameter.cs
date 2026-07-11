using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbParameter : MarshalByRefObject, IDataParameter, IDbDataParameter
	{
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract DbType DbType { get; set; }

		[DefaultValue(ParameterDirection.Input)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract ParameterDirection Direction { get; set; }

		[Browsable(false)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract bool IsNullable { get; set; }

		[DefaultValue("")]
		public abstract string ParameterName { get; set; }

		public abstract int Size { get; set; }

		[DefaultValue("")]
		public abstract string SourceColumn { get; set; }

		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract bool SourceColumnNullMapping { get; set; }

		[DefaultValue(DataRowVersion.Current)]
		public abstract DataRowVersion SourceVersion { get; set; }

		byte IDbDataParameter.Precision
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		byte IDbDataParameter.Scale
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract object Value { get; set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public abstract void ResetDbType();
	}
}
