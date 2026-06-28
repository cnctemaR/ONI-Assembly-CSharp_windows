using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ObjectDisposedException : InvalidOperationException
	{
		public ObjectDisposedException(string objectName)
			: base(Locale.GetText("The object was used after being disposed."))
		{
			this.obj_name = objectName;
			this.msg = Locale.GetText("The object was used after being disposed.");
		}

		public ObjectDisposedException(string objectName, string message)
			: base(message)
		{
			this.obj_name = objectName;
			this.msg = message;
		}

		public ObjectDisposedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ObjectDisposedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.obj_name = info.GetString("ObjectName");
		}

		public override string Message
		{
			get
			{
				return this.msg;
			}
		}

		public string ObjectName
		{
			get
			{
				return this.obj_name;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ObjectName", this.obj_name);
		}

		private string obj_name;

		private string msg;
	}
}
