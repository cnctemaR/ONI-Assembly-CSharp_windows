using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[Serializable]
	public class WarningException : SystemException
	{
		public WarningException()
			: this(null, null, null)
		{
		}

		public WarningException(string message)
			: this(message, null, null)
		{
		}

		public WarningException(string message, string helpUrl)
			: this(message, helpUrl, null)
		{
		}

		public WarningException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public WarningException(string message, string helpUrl, string helpTopic)
			: base(message)
		{
			this.helpUrl = helpUrl;
			this.helpTopic = helpTopic;
		}

		protected WarningException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.helpUrl = (string)info.GetValue("helpUrl", typeof(string));
			this.helpTopic = (string)info.GetValue("helpTopic", typeof(string));
		}

		public string HelpUrl
		{
			get
			{
				return this.helpUrl;
			}
		}

		public string HelpTopic
		{
			get
			{
				return this.helpTopic;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("helpUrl", this.helpUrl);
			info.AddValue("helpTopic", this.helpTopic);
			base.GetObjectData(info, context);
		}

		private readonly string helpUrl;

		private readonly string helpTopic;
	}
}
