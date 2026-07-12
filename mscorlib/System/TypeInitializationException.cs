using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public sealed class TypeInitializationException : SystemException
	{
		private TypeInitializationException()
			: base("Type constructor threw an exception.")
		{
			base.HResult = -2146233036;
		}

		public TypeInitializationException(string fullTypeName, Exception innerException)
			: this(fullTypeName, SR.Format("The type initializer for '{0}' threw an exception.", fullTypeName), innerException)
		{
		}

		internal TypeInitializationException(string message)
			: base(message)
		{
			base.HResult = -2146233036;
		}

		internal TypeInitializationException(string fullTypeName, string message, Exception innerException)
			: base(message, innerException)
		{
			this._typeName = fullTypeName;
			base.HResult = -2146233036;
		}

		internal TypeInitializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._typeName = info.GetString("TypeName");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", this.TypeName, typeof(string));
		}

		public string TypeName
		{
			get
			{
				if (this._typeName == null)
				{
					return string.Empty;
				}
				return this._typeName;
			}
		}

		private string _typeName;
	}
}
