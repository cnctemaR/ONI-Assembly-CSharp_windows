using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class TypeLoadException : SystemException
	{
		public TypeLoadException()
			: base(Locale.GetText("A type load exception has occurred."))
		{
			base.HResult = -2146233054;
		}

		public TypeLoadException(string message)
			: base(message)
		{
			base.HResult = -2146233054;
		}

		public TypeLoadException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233054;
		}

		internal TypeLoadException(string className, string assemblyName)
			: this()
		{
			this.className = className;
			this.assemblyName = assemblyName;
		}

		protected TypeLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.className = info.GetString("TypeLoadClassName");
			this.assemblyName = info.GetString("TypeLoadAssemblyName");
		}

		public override string Message
		{
			get
			{
				if (this.className == null)
				{
					return base.Message;
				}
				if (this.assemblyName != null && this.assemblyName != string.Empty)
				{
					return string.Format("Could not load type '{0}' from assembly '{1}'.", this.className, this.assemblyName);
				}
				return string.Format("Could not load type '{0}'.", this.className);
			}
		}

		public string TypeName
		{
			get
			{
				if (this.className == null)
				{
					return string.Empty;
				}
				return this.className;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("TypeLoadClassName", this.className, typeof(string));
			info.AddValue("TypeLoadAssemblyName", this.assemblyName, typeof(string));
			info.AddValue("TypeLoadMessageArg", string.Empty, typeof(string));
			info.AddValue("TypeLoadResourceID", 0, typeof(int));
		}

		private const int Result = -2146233054;

		private string className;

		private string assemblyName;
	}
}
