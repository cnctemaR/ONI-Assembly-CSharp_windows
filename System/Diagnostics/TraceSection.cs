using System;
using System.Configuration;

namespace System.Diagnostics
{
	internal class TraceSection : ConfigurationElement
	{
		static TraceSection()
		{
			TraceSection._properties.Add(TraceSection._propListeners);
			TraceSection._properties.Add(TraceSection._propAutoFlush);
			TraceSection._properties.Add(TraceSection._propIndentSize);
			TraceSection._properties.Add(TraceSection._propUseGlobalLock);
		}

		[ConfigurationProperty("autoflush", DefaultValue = false)]
		public bool AutoFlush
		{
			get
			{
				return (bool)base[TraceSection._propAutoFlush];
			}
		}

		[ConfigurationProperty("indentsize", DefaultValue = 4)]
		public int IndentSize
		{
			get
			{
				return (int)base[TraceSection._propIndentSize];
			}
		}

		[ConfigurationProperty("listeners")]
		public ListenerElementsCollection Listeners
		{
			get
			{
				return (ListenerElementsCollection)base[TraceSection._propListeners];
			}
		}

		[ConfigurationProperty("useGlobalLock", DefaultValue = true)]
		public bool UseGlobalLock
		{
			get
			{
				return (bool)base[TraceSection._propUseGlobalLock];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return TraceSection._properties;
			}
		}

		private static readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propListeners = new ConfigurationProperty("listeners", typeof(ListenerElementsCollection), new ListenerElementsCollection(), ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propAutoFlush = new ConfigurationProperty("autoflush", typeof(bool), false, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propIndentSize = new ConfigurationProperty("indentsize", typeof(int), 4, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propUseGlobalLock = new ConfigurationProperty("useGlobalLock", typeof(bool), true, ConfigurationPropertyOptions.None);
	}
}
