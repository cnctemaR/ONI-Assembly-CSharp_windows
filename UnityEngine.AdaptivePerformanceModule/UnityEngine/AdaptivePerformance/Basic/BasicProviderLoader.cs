using System;
using System.Collections.Generic;
using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.Bindings;

namespace UnityEngine.AdaptivePerformance.Basic
{
	[VisibleToOtherModules(new string[] { "UnityEditor.AdaptivePerformanceModule" })]
	internal class BasicProviderLoader : AdaptivePerformanceLoaderHelper
	{
		public override bool Initialized
		{
			get
			{
				return this.BasicSubsystem != null;
			}
		}

		public override bool Running
		{
			get
			{
				return this.BasicSubsystem != null && this.BasicSubsystem.running;
			}
		}

		public BasicAdaptivePerformanceSubsystem BasicSubsystem
		{
			get
			{
				return this.GetLoadedSubsystem<BasicAdaptivePerformanceSubsystem>();
			}
		}

		public override ISubsystem GetDefaultSubsystem()
		{
			return this.BasicSubsystem;
		}

		public override IAdaptivePerformanceSettings GetSettings()
		{
			return BasicProviderSettings.GetSettings();
		}

		public override bool Initialize()
		{
			base.CreateSubsystem<AdaptivePerformanceSubsystemDescriptor, BasicAdaptivePerformanceSubsystem>(BasicProviderLoader.s_BasicSubsystemDescriptors, "BasicAdaptivePerformanceSubsystem");
			bool flag = this.BasicSubsystem == null;
			if (flag)
			{
				Debug.LogError("Unable to start the Basic subsystem.");
			}
			return this.BasicSubsystem != null;
		}

		public override bool Start()
		{
			base.StartSubsystem<BasicAdaptivePerformanceSubsystem>();
			return true;
		}

		public override bool Stop()
		{
			base.StopSubsystem<BasicAdaptivePerformanceSubsystem>();
			return true;
		}

		public override bool Deinitialize()
		{
			base.DestroySubsystem<BasicAdaptivePerformanceSubsystem>();
			return base.Deinitialize();
		}

		private static List<AdaptivePerformanceSubsystemDescriptor> s_BasicSubsystemDescriptors = new List<AdaptivePerformanceSubsystemDescriptor>();
	}
}
