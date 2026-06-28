using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Data.OleDb
{
	[Serializable]
	public sealed class OleDbException : DbException
	{
		internal OleDbException(OleDbConnection cnc)
		{
			this.connection = cnc;
		}

		[TypeConverter(typeof(OleDbException.ErrorCodeConverter))]
		public override int ErrorCode
		{
			get
			{
				IntPtr intPtr = libgda.gda_connection_get_errors(this.connection.GdaConnection);
				if (intPtr != IntPtr.Zero)
				{
					GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList));
					return (int)libgda.gda_error_get_number(gdaList.data);
				}
				return -1;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OleDbErrorCollection Errors
		{
			get
			{
				OleDbErrorCollection oleDbErrorCollection = new OleDbErrorCollection();
				IntPtr intPtr = libgda.gda_connection_get_errors(this.connection.GdaConnection);
				if (intPtr != IntPtr.Zero)
				{
					for (GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList)); gdaList != null; gdaList = (GdaList)Marshal.PtrToStructure(gdaList.next, typeof(GdaList)))
					{
						oleDbErrorCollection.Add(new OleDbError(libgda.gda_error_get_description(gdaList.data), (int)libgda.gda_error_get_number(gdaList.data), libgda.gda_error_get_source(gdaList.data), libgda.gda_error_get_sqlstate(gdaList.data)));
					}
				}
				return oleDbErrorCollection;
			}
		}

		public new string Message
		{
			get
			{
				string text = string.Empty;
				IntPtr intPtr = libgda.gda_connection_get_errors(this.connection.GdaConnection);
				if (intPtr != IntPtr.Zero)
				{
					for (GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList)); gdaList != null; gdaList = (GdaList)Marshal.PtrToStructure(gdaList.next, typeof(GdaList)))
					{
						text = text + ";" + libgda.gda_error_get_description(gdaList.data);
					}
					return text;
				}
				return null;
			}
		}

		public new string Source
		{
			get
			{
				IntPtr intPtr = libgda.gda_connection_get_errors(this.connection.GdaConnection);
				if (intPtr != IntPtr.Zero)
				{
					GdaList gdaList = (GdaList)Marshal.PtrToStructure(intPtr, typeof(GdaList));
					return libgda.gda_error_get_source(gdaList.data);
				}
				return null;
			}
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			if (si == null)
			{
				throw new ArgumentNullException("si");
			}
			si.AddValue("connection", this.connection);
			base.GetObjectData(si, context);
			throw new NotImplementedException();
		}

		private OleDbConnection connection;

		internal sealed class ErrorCodeConverter : Int32Converter
		{
			[MonoTODO]
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
