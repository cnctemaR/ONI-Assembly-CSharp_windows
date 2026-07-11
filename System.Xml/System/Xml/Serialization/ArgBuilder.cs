using System;

namespace System.Xml.Serialization
{
	internal class ArgBuilder
	{
		internal ArgBuilder(string name, int index, Type argType)
		{
			this.Name = name;
			this.Index = index;
			this.ArgType = argType;
		}

		internal string Name;

		internal int Index;

		internal Type ArgType;
	}
}
