using System;

namespace UnityEngine
{
	[Obsolete("Use SubsystemWithProvider instead.", false)]
	public abstract class Subsystem : ISubsystem
	{
		public abstract bool running { get; }

		public abstract void Start();

		public abstract void Stop();

		public void Destroy()
		{
			bool flag = SubsystemManager.RemoveDeprecatedSubsystem(this);
			if (flag)
			{
				this.OnDestroy();
			}
		}

		protected abstract void OnDestroy();

		internal ISubsystemDescriptor m_SubsystemDescriptor;
	}
}
