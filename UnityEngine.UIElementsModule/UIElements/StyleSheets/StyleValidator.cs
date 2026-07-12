using System;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements.StyleSheets
{
	internal class StyleValidator
	{
		public StyleValidator()
		{
			this.m_SyntaxParser = new StyleSyntaxParser();
			this.m_StyleMatcher = new StyleMatcher();
		}

		public StyleValidationResult ValidateProperty(string name, string value)
		{
			StyleValidationResult styleValidationResult = new StyleValidationResult
			{
				status = StyleValidationStatus.Ok
			};
			bool flag = name.StartsWith("--");
			StyleValidationResult styleValidationResult2;
			if (flag)
			{
				styleValidationResult2 = styleValidationResult;
			}
			else
			{
				string text;
				bool flag2 = !StylePropertyCache.TryGetSyntax(name, out text);
				if (flag2)
				{
					string text2 = StylePropertyCache.FindClosestPropertyName(name);
					styleValidationResult.status = StyleValidationStatus.Error;
					styleValidationResult.message = "Unknown property '" + name + "'";
					bool flag3 = !string.IsNullOrEmpty(text2);
					if (flag3)
					{
						styleValidationResult.message = styleValidationResult.message + " (did you mean '" + text2 + "'?)";
					}
					styleValidationResult2 = styleValidationResult;
				}
				else
				{
					Expression expression = this.m_SyntaxParser.Parse(text);
					bool flag4 = expression == null;
					if (flag4)
					{
						styleValidationResult.status = StyleValidationStatus.Error;
						styleValidationResult.message = string.Concat(new string[] { "Invalid '", name, "' property syntax '", text, "'" });
						styleValidationResult2 = styleValidationResult;
					}
					else
					{
						MatchResult matchResult = this.m_StyleMatcher.Match(expression, value);
						bool flag5 = !matchResult.success;
						if (flag5)
						{
							styleValidationResult.errorValue = matchResult.errorValue;
							switch (matchResult.errorCode)
							{
							case MatchResultErrorCode.Syntax:
							{
								styleValidationResult.status = StyleValidationStatus.Error;
								string text3;
								bool flag6 = this.IsUnitMissing(text, value, out text3);
								if (flag6)
								{
									styleValidationResult.hint = "Property expects a unit. Did you forget to add " + text3 + "?";
								}
								else
								{
									bool flag7 = this.IsUnsupportedColor(text);
									if (flag7)
									{
										styleValidationResult.hint = "Unsupported color '" + value + "'.";
									}
								}
								styleValidationResult.message = string.Concat(new string[] { "Expected (", text, ") but found '", matchResult.errorValue, "'" });
								break;
							}
							case MatchResultErrorCode.EmptyValue:
								styleValidationResult.status = StyleValidationStatus.Error;
								styleValidationResult.message = "Expected (" + text + ") but found empty value";
								break;
							case MatchResultErrorCode.ExpectedEndOfValue:
								styleValidationResult.status = StyleValidationStatus.Warning;
								styleValidationResult.message = "Expected end of value but found '" + matchResult.errorValue + "'";
								break;
							default:
								Debug.LogAssertion(string.Format("Unexpected error code '{0}'", matchResult.errorCode));
								break;
							}
						}
						styleValidationResult2 = styleValidationResult;
					}
				}
			}
			return styleValidationResult2;
		}

		private bool IsUnitMissing(string propertySyntax, string propertyValue, out string unitHint)
		{
			unitHint = null;
			float num;
			bool flag = !float.TryParse(propertyValue, out num);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = propertySyntax.Contains("<length>") || propertySyntax.Contains("<length-percentage>");
				if (flag3)
				{
					unitHint = "px or %";
				}
				else
				{
					bool flag4 = propertySyntax.Contains("<time>");
					if (flag4)
					{
						unitHint = "s or ms";
					}
				}
				flag2 = !string.IsNullOrEmpty(unitHint);
			}
			return flag2;
		}

		private bool IsUnsupportedColor(string propertySyntax)
		{
			return propertySyntax.StartsWith("<color>");
		}

		private StyleSyntaxParser m_SyntaxParser;

		private StyleMatcher m_StyleMatcher;
	}
}
