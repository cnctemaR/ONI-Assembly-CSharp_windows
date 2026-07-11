using System;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.XPath
{
	[Serializable]
	public class XPathException : SystemException
	{
		protected XPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.res = (string)info.GetValue("res", typeof(string));
			this.args = (string[])info.GetValue("args", typeof(string[]));
			string text = null;
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "version")
				{
					text = (string)serializationEntry.Value;
				}
			}
			if (text == null)
			{
				this.message = XPathException.CreateMessage(this.res, this.args);
				return;
			}
			this.message = null;
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("res", this.res);
			info.AddValue("args", this.args);
			info.AddValue("version", "2.0");
		}

		public XPathException()
			: this(string.Empty, null)
		{
		}

		public XPathException(string message)
			: this(message, null)
		{
		}

		public XPathException(string message, Exception innerException)
			: this("{0}", new string[] { message }, innerException)
		{
		}

		internal static XPathException Create(string res)
		{
			return new XPathException(res, null);
		}

		internal static XPathException Create(string res, string arg)
		{
			return new XPathException(res, new string[] { arg });
		}

		internal static XPathException Create(string res, string arg, string arg2)
		{
			return new XPathException(res, new string[] { arg, arg2 });
		}

		internal static XPathException Create(string res, string arg, Exception innerException)
		{
			return new XPathException(res, new string[] { arg }, innerException);
		}

		private XPathException(string res, string[] args)
			: this(res, args, null)
		{
		}

		private XPathException(string res, string[] args, Exception inner)
			: base(XPathException.CreateMessage(res, args), inner)
		{
			base.HResult = -2146231997;
			this.res = res;
			this.args = args;
		}

		private static string CreateMessage(string res, string[] args)
		{
			string text2;
			try
			{
				string text = Res.GetString(res, args);
				if (text == null)
				{
					text = "UNKNOWN(" + res + ")";
				}
				text2 = text;
			}
			catch (MissingManifestResourceException)
			{
				text2 = "UNKNOWN(" + res + ")";
			}
			return text2;
		}

		public override string Message
		{
			get
			{
				if (this.message != null)
				{
					return this.message;
				}
				return base.Message;
			}
		}

		private string res;

		private string[] args;

		private string message;
	}
}
