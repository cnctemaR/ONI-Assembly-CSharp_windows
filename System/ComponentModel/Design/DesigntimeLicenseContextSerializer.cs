using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class DesigntimeLicenseContextSerializer
	{
		private DesigntimeLicenseContextSerializer()
		{
		}

		public static void Serialize(Stream o, string cryptoKey, DesigntimeLicenseContext context)
		{
			((IFormatter)new BinaryFormatter()).Serialize(o, new object[] { cryptoKey, context.savedLicenseKeys });
		}

		internal static void Deserialize(Stream o, string cryptoKey, RuntimeLicenseContext context)
		{
			object obj = ((IFormatter)new BinaryFormatter()).Deserialize(o);
			if (obj is object[])
			{
				object[] array = (object[])obj;
				if (array[0] is string && (string)array[0] == cryptoKey)
				{
					context.savedLicenseKeys = (Hashtable)array[1];
				}
			}
		}
	}
}
