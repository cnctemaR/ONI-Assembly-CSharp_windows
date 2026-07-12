using System;

namespace UnityEngine.SubsystemsImplementation
{
	public abstract class SubsystemWithProvider : ISubsystem
	{
		public void Start()
		{
			bool running = this.running;
			if (!running)
			{
				this.OnStart();
				this.providerBase.m_Running = true;
				this.running = true;
			}
		}

		protected abstract void OnStart();

		public void Stop()
		{
			bool flag = !this.running;
			if (!flag)
			{
				this.OnStop();
				this.providerBase.m_Running = false;
				this.running = false;
			}
		}

		protected abstract void OnStop();

		public void Destroy()
		{
			this.Stop();
			bool flag = SubsystemManager.RemoveStandaloneSubsystem(this);
			if (flag)
			{
				this.OnDestroy();
			}
		}

		protected abstract void OnDestroy();

		public bool running { get; private set; }

		internal SubsystemProvider providerBase { get; set; }

		internal abstract void Initialize(SubsystemDescriptorWithProvider descriptor, SubsystemProvider subsystemProvider);

		internal abstract SubsystemDescriptorWithProvider descriptor { get; }
	}
}
