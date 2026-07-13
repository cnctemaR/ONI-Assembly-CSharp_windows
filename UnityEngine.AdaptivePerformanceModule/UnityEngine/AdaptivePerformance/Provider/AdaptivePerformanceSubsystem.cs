using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.AdaptivePerformance.Provider
{
	public class AdaptivePerformanceSubsystem : AdaptivePerformanceSubsystemBase<AdaptivePerformanceSubsystem, AdaptivePerformanceSubsystemDescriptor, AdaptivePerformanceSubsystem.APProvider>
	{
		public override IApplicationLifecycle ApplicationLifecycle
		{
			get
			{
				return base.provider.ApplicationLifecycle;
			}
		}

		public override IDevicePerformanceLevelControl PerformanceLevelControl
		{
			get
			{
				return base.provider.PerformanceLevelControl;
			}
		}

		public override Version Version
		{
			get
			{
				return base.provider.Version;
			}
		}

		public override Feature Capabilities
		{
			get
			{
				return base.provider.Capabilities;
			}
			protected set
			{
				base.provider.Capabilities = value;
			}
		}

		public override string Stats
		{
			get
			{
				return base.provider.Stats;
			}
		}

		public override bool Initialized
		{
			get
			{
				return base.provider.Initialized;
			}
			protected set
			{
				base.provider.Initialized = value;
			}
		}

		public override PerformanceDataRecord Update()
		{
			return base.provider.Update();
		}

		public abstract class APProvider : SubsystemProvider<AdaptivePerformanceSubsystem>
		{
			public abstract Feature Capabilities { get; set; }

			public abstract PerformanceDataRecord Update();

			public abstract IApplicationLifecycle ApplicationLifecycle { get; }

			public abstract IDevicePerformanceLevelControl PerformanceLevelControl { get; }

			public abstract Version Version { get; }

			public virtual string Stats
			{
				get
				{
					return "";
				}
			}

			public abstract bool Initialized { get; set; }

			public new bool running
			{
				get
				{
					return this.m_Running;
				}
			}

			protected new bool m_Running;
		}
	}
}
