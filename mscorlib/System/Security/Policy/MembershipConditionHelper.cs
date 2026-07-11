using System;

namespace System.Security.Policy
{
	internal sealed class MembershipConditionHelper
	{
		internal static int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Tag != MembershipConditionHelper.XmlTag)
			{
				string text = string.Format(Locale.GetText("Invalid tag {0}, expected {1}."), se.Tag, MembershipConditionHelper.XmlTag);
				throw new ArgumentException(text, parameterName);
			}
			int num = minimumVersion;
			string text2 = se.Attribute("version");
			if (text2 != null)
			{
				try
				{
					num = int.Parse(text2);
				}
				catch (Exception ex)
				{
					string text3 = Locale.GetText("Couldn't parse version from '{0}'.");
					text3 = string.Format(text3, text2);
					throw new ArgumentException(text3, parameterName, ex);
				}
			}
			return num;
		}

		internal static SecurityElement Element(Type type, int version)
		{
			SecurityElement securityElement = new SecurityElement(MembershipConditionHelper.XmlTag);
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", version.ToString());
			return securityElement;
		}

		private static readonly string XmlTag = "IMembershipCondition";
	}
}
