using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[Serializable]
	public abstract class SerializationBinder
	{
		public abstract Type BindToType(string assemblyName, string typeName);
	}
}
