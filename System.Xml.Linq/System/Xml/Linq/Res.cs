using System;
using System.Globalization;

namespace System.Xml.Linq
{
	internal static class Res
	{
		public static string GetString(string name)
		{
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(name);
			if (num <= 2345166790U)
			{
				if (num <= 626770679U)
				{
					if (num <= 272356109U)
					{
						if (num <= 85233838U)
						{
							if (num != 15713876U)
							{
								if (num == 85233838U)
								{
									if (name == "InvalidOperation_ExpectedInteractive")
									{
										return "The XmlReader state should be Interactive.";
									}
								}
							}
							else if (name == "InvalidOperation_DocumentStructure")
							{
								return "This operation would create an incorrectly structured document.";
							}
						}
						else if (num != 103965186U)
						{
							if (num == 272356109U)
							{
								if (name == "InvalidOperation_UnexpectedNodeType")
								{
									return "The XmlReader should not be on a node of type {0}.";
								}
							}
						}
						else if (name == "InvalidOperation_MissingRoot")
						{
							return "The root element is missing.";
						}
					}
					else if (num <= 569780718U)
					{
						if (num != 407913795U)
						{
							if (num == 569780718U)
							{
								if (name == "Argument_InvalidPrefix")
								{
									return "'{0}' is an invalid prefix.";
								}
							}
						}
						else if (name == "Argument_NamespaceDeclarationPrefixed")
						{
							return "The prefix '{0}' cannot be bound to the empty namespace name.";
						}
					}
					else if (num != 625070019U)
					{
						if (num == 626770679U)
						{
							if (name == "InvalidOperation_UnexpectedEvaluation")
							{
								return "The XPath expression evaluated to unexpected type {0}.";
							}
						}
					}
					else if (name == "InvalidOperation_UnresolvedEntityReference")
					{
						return "The XmlReader cannot resolve entity references.";
					}
				}
				else if (num <= 1639487891U)
				{
					if (num <= 1004902937U)
					{
						if (num != 922706940U)
						{
							if (num == 1004902937U)
							{
								if (name == "InvalidOperation_DuplicateAttribute")
								{
									return "Duplicate attribute.";
								}
							}
						}
						else if (name == "Argument_InvalidExpandedName")
						{
							return "'{0}' is an invalid expanded name.";
						}
					}
					else if (num != 1405029661U)
					{
						if (num == 1639487891U)
						{
							if (name == "Argument_NamespaceDeclarationXml")
							{
								return "The prefix 'xml' is bound to the namespace name 'http://www.w3.org/XML/1998/namespace'. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.";
							}
						}
					}
					else if (name == "NotSupported_CheckValidity")
					{
						return "This XPathNavigator does not support XSD validation.";
					}
				}
				else if (num <= 1755217186U)
				{
					if (num != 1718317927U)
					{
						if (num == 1755217186U)
						{
							if (name == "InvalidOperation_ExpectedNodeType")
							{
								return "The XmlReader must be on a node of type {0} instead of a node of type {1}.";
							}
						}
					}
					else if (name == "Argument_XObjectValue")
					{
						return "An XObject cannot be used as a value.";
					}
				}
				else if (num != 1837980878U)
				{
					if (num == 2345166790U)
					{
						if (name == "Argument_InvalidPIName")
						{
							return "'{0}' is an invalid name for a processing instruction.";
						}
					}
				}
				else if (name == "InvalidOperation_DeserializeInstance")
				{
					return "This instance cannot be deserialized.";
				}
			}
			else if (num <= 3510382528U)
			{
				if (num <= 3280571068U)
				{
					if (num <= 2491089664U)
					{
						if (num != 2437381507U)
						{
							if (num == 2491089664U)
							{
								if (name == "InvalidOperation_ExternalCode")
								{
									return "This operation was corrupted by external code.";
								}
							}
						}
						else if (name == "InvalidOperation_MissingAncestor")
						{
							return "A common ancestor is missing.";
						}
					}
					else if (num != 3172179164U)
					{
						if (num == 3280571068U)
						{
							if (name == "Argument_MustBeDerivedFrom")
							{
								return "The argument must be derived from {0}.";
							}
						}
					}
					else if (name == "Argument_NamespaceDeclarationXmlns")
					{
						return "The prefix 'xmlns' is bound to the namespace name 'http://www.w3.org/2000/xmlns/'. It must not be declared. Other prefixes must not be bound to this namespace name, and it must not be declared as the default namespace.";
					}
				}
				else if (num <= 3430209653U)
				{
					if (num != 3388508910U)
					{
						if (num == 3430209653U)
						{
							if (name == "InvalidOperation_BadNodeType")
							{
								return "This operation is not valid on a node of type {0}.";
							}
						}
					}
					else if (name == "Argument_ConvertToString")
					{
						return "The argument cannot be converted to a string.";
					}
				}
				else if (num != 3431575044U)
				{
					if (num == 3510382528U)
					{
						if (name == "InvalidOperation_ExpectedEndOfFile")
						{
							return "The XmlReader state should be EndOfFile after this operation.";
						}
					}
				}
				else if (name == "Argument_AddNonWhitespace")
				{
					return "Non white space characters cannot be added to content.";
				}
			}
			else if (num <= 3723879466U)
			{
				if (num <= 3594069654U)
				{
					if (num != 3539275280U)
					{
						if (num == 3594069654U)
						{
							if (name == "InvalidOperation_MissingParent")
							{
								return "The parent is missing.";
							}
						}
					}
					else if (name == "NotSupported_MoveToId")
					{
						return "This XPathNavigator does not support IDs.";
					}
				}
				else if (num != 3628001091U)
				{
					if (num == 3723879466U)
					{
						if (name == "Argument_AddNode")
						{
							return "A node of type {0} cannot be added to content.";
						}
					}
				}
				else if (name == "NotSupported_WriteBase64")
				{
					return "This XmlWriter does not support base64 encoded data.";
				}
			}
			else if (num <= 3797174551U)
			{
				if (num != 3734520932U)
				{
					if (num == 3797174551U)
					{
						if (name == "InvalidOperation_WriteAttribute")
						{
							return "An attribute cannot be written after content.";
						}
					}
				}
				else if (name == "Argument_CreateNavigator")
				{
					return "This XPathNavigator cannot be created on a node of type {0}.";
				}
			}
			else if (num != 3802163562U)
			{
				if (num == 3833352336U)
				{
					if (name == "NotSupported_WriteEntityRef")
					{
						return "This XmlWriter does not support entity references.";
					}
				}
			}
			else if (name == "Argument_AddAttribute")
			{
				return "An attribute cannot be added to content.";
			}
			return null;
		}

		public static string GetString(string name, params object[] args)
		{
			string @string = Res.GetString(name);
			if (args == null || args.Length == 0)
			{
				return @string;
			}
			return string.Format(CultureInfo.CurrentCulture, @string, args);
		}

		internal const string Argument_AddAttribute = "Argument_AddAttribute";

		internal const string Argument_AddNode = "Argument_AddNode";

		internal const string Argument_AddNonWhitespace = "Argument_AddNonWhitespace";

		internal const string Argument_ConvertToString = "Argument_ConvertToString";

		internal const string Argument_CreateNavigator = "Argument_CreateNavigator";

		internal const string Argument_InvalidExpandedName = "Argument_InvalidExpandedName";

		internal const string Argument_InvalidPIName = "Argument_InvalidPIName";

		internal const string Argument_InvalidPrefix = "Argument_InvalidPrefix";

		internal const string Argument_MustBeDerivedFrom = "Argument_MustBeDerivedFrom";

		internal const string Argument_NamespaceDeclarationPrefixed = "Argument_NamespaceDeclarationPrefixed";

		internal const string Argument_NamespaceDeclarationXml = "Argument_NamespaceDeclarationXml";

		internal const string Argument_NamespaceDeclarationXmlns = "Argument_NamespaceDeclarationXmlns";

		internal const string Argument_XObjectValue = "Argument_XObjectValue";

		internal const string InvalidOperation_BadNodeType = "InvalidOperation_BadNodeType";

		internal const string InvalidOperation_DocumentStructure = "InvalidOperation_DocumentStructure";

		internal const string InvalidOperation_DuplicateAttribute = "InvalidOperation_DuplicateAttribute";

		internal const string InvalidOperation_ExpectedEndOfFile = "InvalidOperation_ExpectedEndOfFile";

		internal const string InvalidOperation_ExpectedInteractive = "InvalidOperation_ExpectedInteractive";

		internal const string InvalidOperation_ExpectedNodeType = "InvalidOperation_ExpectedNodeType";

		internal const string InvalidOperation_ExternalCode = "InvalidOperation_ExternalCode";

		internal const string InvalidOperation_DeserializeInstance = "InvalidOperation_DeserializeInstance";

		internal const string InvalidOperation_MissingAncestor = "InvalidOperation_MissingAncestor";

		internal const string InvalidOperation_MissingParent = "InvalidOperation_MissingParent";

		internal const string InvalidOperation_MissingRoot = "InvalidOperation_MissingRoot";

		internal const string InvalidOperation_UnexpectedEvaluation = "InvalidOperation_UnexpectedEvaluation";

		internal const string InvalidOperation_UnexpectedNodeType = "InvalidOperation_UnexpectedNodeType";

		internal const string InvalidOperation_UnresolvedEntityReference = "InvalidOperation_UnresolvedEntityReference";

		internal const string InvalidOperation_WriteAttribute = "InvalidOperation_WriteAttribute";

		internal const string NotSupported_CheckValidity = "NotSupported_CheckValidity";

		internal const string NotSupported_MoveToId = "NotSupported_MoveToId";

		internal const string NotSupported_WriteBase64 = "NotSupported_WriteBase64";

		internal const string NotSupported_WriteEntityRef = "NotSupported_WriteEntityRef";
	}
}
