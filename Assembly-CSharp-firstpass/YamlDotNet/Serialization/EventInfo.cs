using System;

namespace YamlDotNet.Serialization
{
	public abstract class EventInfo
	{
		protected EventInfo(IObjectDescriptor source)
		{
			this.Source = source;
		}

		public IObjectDescriptor Source { get; private set; }
	}
}
