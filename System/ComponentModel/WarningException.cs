using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[Serializable]
	public class WarningException : SystemException
	{
		public WarningException(string message)
			: base(message)
		{
		}

		public WarningException(string message, string helpUrl)
			: base(message)
		{
			this.helpUrl = helpUrl;
		}

		public WarningException(string message, string helpUrl, string helpTopic)
			: base(message)
		{
			this.helpUrl = helpUrl;
			this.helpTopic = helpTopic;
		}

		public WarningException()
			: base(global::Locale.GetText("Warning"))
		{
		}

		public WarningException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected WarningException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			try
			{
				this.helpTopic = info.GetString("helpTopic");
				this.helpUrl = info.GetString("helpUrl");
			}
			catch (SerializationException)
			{
				this.helpTopic = info.GetString("HelpTopic");
				this.helpUrl = info.GetString("HelpUrl");
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("helpTopic", this.helpTopic);
			info.AddValue("helpUrl", this.helpUrl);
		}

		public string HelpTopic
		{
			get
			{
				return this.helpTopic;
			}
		}

		public string HelpUrl
		{
			get
			{
				return this.helpUrl;
			}
		}

		private string helpUrl;

		private string helpTopic;
	}
}
