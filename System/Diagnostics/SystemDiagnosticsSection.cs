using System;
using System.Configuration;

namespace System.Diagnostics
{
	internal class SystemDiagnosticsSection : ConfigurationSection
	{
		static SystemDiagnosticsSection()
		{
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propAssert);
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propPerfCounters);
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propSources);
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propSharedListeners);
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propSwitches);
			SystemDiagnosticsSection._properties.Add(SystemDiagnosticsSection._propTrace);
		}

		[ConfigurationProperty("assert")]
		public AssertSection Assert
		{
			get
			{
				return (AssertSection)base[SystemDiagnosticsSection._propAssert];
			}
		}

		[ConfigurationProperty("performanceCounters")]
		public PerfCounterSection PerfCounters
		{
			get
			{
				return (PerfCounterSection)base[SystemDiagnosticsSection._propPerfCounters];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SystemDiagnosticsSection._properties;
			}
		}

		[ConfigurationProperty("sources")]
		public SourceElementsCollection Sources
		{
			get
			{
				return (SourceElementsCollection)base[SystemDiagnosticsSection._propSources];
			}
		}

		[ConfigurationProperty("sharedListeners")]
		public ListenerElementsCollection SharedListeners
		{
			get
			{
				return (ListenerElementsCollection)base[SystemDiagnosticsSection._propSharedListeners];
			}
		}

		[ConfigurationProperty("switches")]
		public SwitchElementsCollection Switches
		{
			get
			{
				return (SwitchElementsCollection)base[SystemDiagnosticsSection._propSwitches];
			}
		}

		[ConfigurationProperty("trace")]
		public TraceSection Trace
		{
			get
			{
				return (TraceSection)base[SystemDiagnosticsSection._propTrace];
			}
		}

		protected override void InitializeDefault()
		{
			this.Trace.Listeners.InitializeDefaultInternal();
		}

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propAssert = new ConfigurationProperty("assert", typeof(AssertSection), new AssertSection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propPerfCounters = new ConfigurationProperty("performanceCounters", typeof(PerfCounterSection), new PerfCounterSection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propSources = new ConfigurationProperty("sources", typeof(SourceElementsCollection), new SourceElementsCollection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propSharedListeners = new ConfigurationProperty("sharedListeners", typeof(SharedListenerElementsCollection), new SharedListenerElementsCollection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propSwitches = new ConfigurationProperty("switches", typeof(SwitchElementsCollection), new SwitchElementsCollection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propTrace = new ConfigurationProperty("trace", typeof(TraceSection), new TraceSection(), ConfigurationPropertyOptions.None);
	}
}
