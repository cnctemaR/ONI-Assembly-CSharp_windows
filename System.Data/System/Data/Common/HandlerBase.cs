using System;
using System.Globalization;
using System.Xml;

namespace System.Data.Common
{
	internal static class HandlerBase
	{
		internal static void CheckForChildNodes(XmlNode node)
		{
			if (node.HasChildNodes)
			{
				throw ADP.ConfigBaseNoChildNodes(node.FirstChild);
			}
		}

		private static void CheckForNonElement(XmlNode node)
		{
			if (XmlNodeType.Element != node.NodeType)
			{
				throw ADP.ConfigBaseElementsOnly(node);
			}
		}

		internal static void CheckForUnrecognizedAttributes(XmlNode node)
		{
			if (node.Attributes.Count != 0)
			{
				throw ADP.ConfigUnrecognizedAttributes(node);
			}
		}

		internal static bool IsIgnorableAlsoCheckForNonElement(XmlNode node)
		{
			if (XmlNodeType.Comment == node.NodeType || XmlNodeType.Whitespace == node.NodeType)
			{
				return true;
			}
			HandlerBase.CheckForNonElement(node);
			return false;
		}

		internal static string RemoveAttribute(XmlNode node, string name, bool required, bool allowEmpty)
		{
			XmlNode xmlNode = node.Attributes.RemoveNamedItem(name);
			if (xmlNode == null)
			{
				if (required)
				{
					throw ADP.ConfigRequiredAttributeMissing(name, node);
				}
				return null;
			}
			else
			{
				string value = xmlNode.Value;
				if (!allowEmpty && value.Length == 0)
				{
					throw ADP.ConfigRequiredAttributeEmpty(name, node);
				}
				return value;
			}
		}

		internal static DataSet CloneParent(DataSet parentConfig, bool insenstive)
		{
			if (parentConfig == null)
			{
				parentConfig = new DataSet("system.data");
				parentConfig.CaseSensitive = !insenstive;
				parentConfig.Locale = CultureInfo.InvariantCulture;
			}
			else
			{
				parentConfig = parentConfig.Copy();
			}
			return parentConfig;
		}
	}
}
