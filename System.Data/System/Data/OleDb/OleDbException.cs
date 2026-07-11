using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.Serialization;

namespace System.Data.OleDb
{
	[Serializable]
	public sealed class OleDbException : DbException
	{
		internal OleDbException()
		{
		}

		[TypeConverter("System.Data.OleDb.OleDbException.ErrorCodeConverter")]
		public override int ErrorCode
		{
			get
			{
				throw null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OleDbErrorCollection Errors
		{
			get
			{
				throw null;
			}
		}

		public new string Message
		{
			get
			{
				throw null;
			}
		}

		public new string Source
		{
			get
			{
				throw null;
			}
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
