using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine._Scripting.APIUpdating
{
	internal class APIUpdaterRuntimeHelpers
	{
		[RequiredByNativeCode]
		internal static bool GetMovedFromAttributeDataForType(Type sourceType, out string assembly, out string nsp, out string klass)
		{
			klass = null;
			nsp = null;
			assembly = null;
			object[] customAttributes = sourceType.GetCustomAttributes(typeof(MovedFromAttribute), false);
			bool flag = customAttributes.Length != 1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				MovedFromAttribute movedFromAttribute = (MovedFromAttribute)customAttributes[0];
				klass = movedFromAttribute.data.className;
				nsp = movedFromAttribute.data.nameSpace;
				assembly = movedFromAttribute.data.assembly;
				flag2 = true;
			}
			return flag2;
		}

		[RequiredByNativeCode]
		internal static bool GetObsoleteTypeRedirection(Type sourceType, out string assemblyName, out string nsp, out string className)
		{
			object[] customAttributes = sourceType.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			assemblyName = null;
			nsp = null;
			className = null;
			bool flag = customAttributes.Length != 1;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ObsoleteAttribute obsoleteAttribute = (ObsoleteAttribute)customAttributes[0];
				string message = obsoleteAttribute.Message;
				bool flag3 = string.IsNullOrEmpty(message);
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					string text = "(UnityUpgradable) -> ";
					int num = message.IndexOf(text);
					bool flag4 = num >= 0;
					if (flag4)
					{
						string text2 = message.Substring(num + text.Length).Trim();
						bool flag5 = text2.Length == 0;
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							bool flag6 = text2[0] == '[';
							int num2;
							if (flag6)
							{
								num2 = text2.IndexOf(']');
								bool flag7 = num2 == -1;
								if (flag7)
								{
									return false;
								}
								assemblyName = text2.Substring(1, num2 - 1);
								text2 = text2.Substring(num2 + 1).Trim();
							}
							else
							{
								assemblyName = sourceType.Assembly.GetName().Name;
							}
							num2 = text2.LastIndexOf('.');
							bool flag8 = num2 > -1;
							if (flag8)
							{
								className = text2.Substring(num2 + 1);
								text2 = text2.Substring(0, num2);
							}
							else
							{
								className = text2;
								text2 = "";
							}
							bool flag9 = text2.Length > 0;
							if (flag9)
							{
								nsp = text2;
							}
							else
							{
								nsp = sourceType.Namespace;
							}
							flag2 = true;
						}
					}
					else
					{
						flag2 = false;
					}
				}
			}
			return flag2;
		}
	}
}
