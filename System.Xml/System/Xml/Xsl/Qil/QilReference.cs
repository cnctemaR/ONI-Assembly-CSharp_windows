using System;

namespace System.Xml.Xsl.Qil
{
	internal class QilReference : QilNode
	{
		public QilReference(QilNodeType nodeType)
			: base(nodeType)
		{
		}

		public string DebugName
		{
			get
			{
				return this._debugName;
			}
			set
			{
				if (value.Length > 1000)
				{
					value = value.Substring(0, 1000);
				}
				this._debugName = value;
			}
		}

		private const int MaxDebugNameLength = 1000;

		private string _debugName;
	}
}
