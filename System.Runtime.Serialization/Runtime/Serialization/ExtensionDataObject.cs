using System;

namespace System.Runtime.Serialization
{
	public sealed class ExtensionDataObject
	{
		internal ExtensionDataObject(object target)
		{
			this.target = target;
		}

		private object target;
	}
}
