using System;

namespace UnityEngine.Experimental
{
	public abstract class SubsystemDescriptor : ISubsystemDescriptor
	{
		public string id { get; set; }

		public Type subsystemImplementationType { get; set; }
	}
}
