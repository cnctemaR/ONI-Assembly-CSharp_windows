using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class TypeInitializationException : SystemException
	{
		public TypeInitializationException(string fullTypeName, Exception innerException)
			: base(Locale.GetText("An exception was thrown by the type initializer for ") + fullTypeName, innerException)
		{
			this.type_name = fullTypeName;
		}

		internal TypeInitializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.type_name = info.GetString("TypeName");
		}

		public string TypeName
		{
			get
			{
				return this.type_name;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("TypeName", this.type_name);
		}

		private string type_name;
	}
}
