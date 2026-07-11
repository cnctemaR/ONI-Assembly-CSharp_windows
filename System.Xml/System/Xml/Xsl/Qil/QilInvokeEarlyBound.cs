using System;
using System.Reflection;

namespace System.Xml.Xsl.Qil
{
	internal class QilInvokeEarlyBound : QilTernary
	{
		public QilInvokeEarlyBound(QilNodeType nodeType, QilNode name, QilNode method, QilNode arguments, XmlQueryType resultType)
			: base(nodeType, name, method, arguments)
		{
			this.xmlType = resultType;
		}

		public QilName Name
		{
			get
			{
				return (QilName)base.Left;
			}
			set
			{
				base.Left = value;
			}
		}

		public MethodInfo ClrMethod
		{
			get
			{
				return (MethodInfo)((QilLiteral)base.Center).Value;
			}
			set
			{
				((QilLiteral)base.Center).Value = value;
			}
		}

		public QilList Arguments
		{
			get
			{
				return (QilList)base.Right;
			}
			set
			{
				base.Right = value;
			}
		}
	}
}
