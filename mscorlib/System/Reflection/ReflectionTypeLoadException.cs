using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ReflectionTypeLoadException : SystemException
	{
		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions)
			: base(Locale.GetText("The classes in the module cannot be loaded."))
		{
			this.loaderExceptions = exceptions;
			this.types = classes;
		}

		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions, string message)
			: base(message)
		{
			this.loaderExceptions = exceptions;
			this.types = classes;
		}

		private ReflectionTypeLoadException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
			this.types = (Type[])info.GetValue("Types", typeof(Type[]));
			this.loaderExceptions = (Exception[])info.GetValue("Exceptions", typeof(Exception[]));
		}

		public Type[] Types
		{
			get
			{
				return this.types;
			}
		}

		public Exception[] LoaderExceptions
		{
			get
			{
				return this.loaderExceptions;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Types", this.types);
			info.AddValue("Exceptions", this.loaderExceptions);
		}

		private Exception[] loaderExceptions;

		private Type[] types;
	}
}
