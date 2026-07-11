using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class TypeInitializationException : SystemException
	{
		private TypeInitializationException()
			: base(Environment.GetResourceString("Type constructor threw an exception."))
		{
			base.SetErrorCode(-2146233036);
		}

		private TypeInitializationException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233036);
		}

		public TypeInitializationException(string fullTypeName, Exception innerException)
			: base(Environment.GetResourceString("The type initializer for '{0}' threw an exception.", new object[] { fullTypeName }), innerException)
		{
			this._typeName = fullTypeName;
			base.SetErrorCode(-2146233036);
		}

		internal TypeInitializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._typeName = info.GetString("TypeName");
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

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", this.TypeName, typeof(string));
		}

		private string _typeName;
	}
}
