using System;

namespace UnityEngine.Experimental
{
	public abstract class Subsystem : ISubsystem
	{
		public abstract void Start();

		public abstract void Stop();

		public abstract void Destroy();

		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
}
