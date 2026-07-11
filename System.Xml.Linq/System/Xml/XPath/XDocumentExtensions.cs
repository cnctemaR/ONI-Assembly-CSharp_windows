using System;
using System.ComponentModel;
using System.Xml.Linq;
using Unity;

namespace System.Xml.XPath
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class XDocumentExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IXPathNavigable ToXPathNavigable(this XNode node)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
