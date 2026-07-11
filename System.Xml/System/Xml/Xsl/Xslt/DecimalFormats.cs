using System;
using System.Collections.ObjectModel;

namespace System.Xml.Xsl.Xslt
{
	internal class DecimalFormats : KeyedCollection<XmlQualifiedName, DecimalFormatDecl>
	{
		protected override XmlQualifiedName GetKeyForItem(DecimalFormatDecl format)
		{
			return format.Name;
		}
	}
}
