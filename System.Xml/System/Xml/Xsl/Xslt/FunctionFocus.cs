using System;
using System.Collections.Generic;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.XPath;

namespace System.Xml.Xsl.Xslt
{
	internal struct FunctionFocus : IFocus
	{
		public void StartFocus(IList<QilNode> args, XslFlags flags)
		{
			int num = 0;
			if ((flags & XslFlags.Current) != XslFlags.None)
			{
				this.current = (QilParameter)args[num++];
			}
			if ((flags & XslFlags.Position) != XslFlags.None)
			{
				this.position = (QilParameter)args[num++];
			}
			if ((flags & XslFlags.Last) != XslFlags.None)
			{
				this.last = (QilParameter)args[num++];
			}
			this.isSet = true;
		}

		public void StopFocus()
		{
			this.isSet = false;
			this.current = (this.position = (this.last = null));
		}

		public bool IsFocusSet
		{
			get
			{
				return this.isSet;
			}
		}

		public QilNode GetCurrent()
		{
			return this.current;
		}

		public QilNode GetPosition()
		{
			return this.position;
		}

		public QilNode GetLast()
		{
			return this.last;
		}

		private bool isSet;

		private QilParameter current;

		private QilParameter position;

		private QilParameter last;
	}
}
