using System;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	[Serializable]
	internal class EnvoyInfo : IEnvoyInfo
	{
		public EnvoyInfo(IMessageSink sinks)
		{
			this.envoySinks = sinks;
		}

		public IMessageSink EnvoySinks
		{
			get
			{
				return this.envoySinks;
			}
			set
			{
				this.envoySinks = value;
			}
		}

		private IMessageSink envoySinks;
	}
}
