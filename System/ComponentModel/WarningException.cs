using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel
{
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
			this.HelpUrl = helpUrl;
			this.HelpTopic = helpTopic;
		}

		protected WarningException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.HelpUrl = (string)info.GetValue("helpUrl", typeof(string));
			this.HelpTopic = (string)info.GetValue("helpTopic", typeof(string));
		}

		public string HelpUrl { get; }

		public string HelpTopic { get; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("helpUrl", this.HelpUrl);
			info.AddValue("helpTopic", this.HelpTopic);
		}
	}
}
