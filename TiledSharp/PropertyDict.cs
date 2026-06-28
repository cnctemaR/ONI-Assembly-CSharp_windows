using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
	public class PropertyDict : Dictionary<string, string>
	{
		public PropertyDict(XElement xmlProp)
		{
			if (xmlProp != null)
			{
				foreach (XElement xelement in xmlProp.Elements("property"))
				{
					string value = xelement.Attribute("name").Value;
					string value2 = xelement.Attribute("value").Value;
					base.Add(value, value2);
				}
			}
		}
	}
}
