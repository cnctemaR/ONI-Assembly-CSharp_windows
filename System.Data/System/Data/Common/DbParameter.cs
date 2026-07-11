using System;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbParameter : MarshalByRefObject, IDbDataParameter, IDataParameter
	{
		[Browsable(false)]
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public abstract DbType DbType { get; set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public abstract void ResetDbType();

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ParameterDirection.Input)]
		public abstract ParameterDirection Direction { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignOnly(true)]
		[Browsable(false)]
		public abstract bool IsNullable { get; set; }

		[DefaultValue("")]
		public abstract string ParameterName { get; set; }

		byte IDbDataParameter.Precision
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		byte IDbDataParameter.Scale
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public virtual byte Precision
		{
			get
			{
				return ((IDbDataParameter)this).Precision;
			}
			set
			{
				((IDbDataParameter)this).Precision = value;
			}
		}

		public virtual byte Scale
		{
			get
			{
				return ((IDbDataParameter)this).Scale;
			}
			set
			{
				((IDbDataParameter)this).Scale = value;
			}
		}

		public abstract int Size { get; set; }

		[DefaultValue("")]
		public abstract string SourceColumn { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DefaultValue(false)]
		public abstract bool SourceColumnNullMapping { get; set; }

		[DefaultValue(DataRowVersion.Current)]
		public virtual DataRowVersion SourceVersion
		{
			get
			{
				return DataRowVersion.Default;
			}
			set
			{
			}
		}

		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract object Value { get; set; }
	}
}
