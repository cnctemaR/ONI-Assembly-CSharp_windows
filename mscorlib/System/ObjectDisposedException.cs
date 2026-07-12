using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public class ObjectDisposedException : InvalidOperationException
	{
		private ObjectDisposedException()
			: this(null, "Cannot access a disposed object.")
		{
		}

		public ObjectDisposedException(string objectName)
			: this(objectName, "Cannot access a disposed object.")
		{
		}

		public ObjectDisposedException(string objectName, string message)
			: base(message)
		{
			base.HResult = -2146232798;
			this._objectName = objectName;
		}

		public ObjectDisposedException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146232798;
		}

		protected ObjectDisposedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._objectName = info.GetString("ObjectName");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ObjectName", this.ObjectName, typeof(string));
		}

		public override string Message
		{
			get
			{
				string objectName = this.ObjectName;
				if (objectName == null || objectName.Length == 0)
				{
					return base.Message;
				}
				string text = SR.Format("Object name: '{0}'.", objectName);
				return base.Message + Environment.NewLine + text;
			}
		}

		public string ObjectName
		{
			get
			{
				if (this._objectName == null)
				{
					return string.Empty;
				}
				return this._objectName;
			}
		}

		private string _objectName;
	}
}
