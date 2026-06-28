using System;

namespace System.EnterpriseServices
{
	internal interface ISharedPropertyGroup
	{
		ISharedProperty CreateProperty(string name, out bool fExists);

		ISharedProperty CreatePropertyByPosition(int position, out bool fExists);

		ISharedProperty Property(string name);

		ISharedProperty PropertyByPosition(int position);
	}
}
