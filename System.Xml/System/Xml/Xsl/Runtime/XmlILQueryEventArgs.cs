using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlILQueryEventArgs : XsltMessageEncounteredEventArgs
	{
		public XmlILQueryEventArgs(string message)
		{
			this.message = message;
		}

		public override string Message
		{
			get
			{
				return this.message;
			}
		}

		private string message;
	}
}
