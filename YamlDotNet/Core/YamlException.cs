using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace YamlDotNet.Core
{
	[Serializable]
	public class YamlException : Exception
	{
		public Mark Start { get; private set; }

		public Mark End { get; private set; }

		public YamlException()
		{
		}

		public YamlException(string message)
			: base(message)
		{
		}

		public YamlException(Mark start, Mark end, string message)
			: this(start, end, message, null)
		{
		}

		public YamlException(Mark start, Mark end, string message, Exception innerException)
			: base(string.Format("({0}) - ({1}): {2}", start, end, message), innerException)
		{
			this.Start = start;
			this.End = end;
		}

		public YamlException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected YamlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Start = (Mark)info.GetValue("Start", typeof(Mark));
			this.End = (Mark)info.GetValue("End", typeof(Mark));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Start", this.Start);
			info.AddValue("End", this.End);
		}
	}
}
