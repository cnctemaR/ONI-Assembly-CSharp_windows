using System;

namespace UnityEngine
{
	public abstract class Subsystem : ISubsystem
	{
		public abstract void Start();

		public abstract void Stop();

		public void Destroy()
		{
			bool flag = Internal_SubsystemInstances.s_StandaloneSubsystemInstances.Remove(this);
			if (flag)
			{
				this.OnDestroy();
			}
		}

		public abstract bool running { get; }

		protected abstract void OnDestroy();

		internal ISubsystemDescriptor m_subsystemDescriptor;
	}
}
