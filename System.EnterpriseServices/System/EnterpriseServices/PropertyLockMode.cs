using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[ComVisible(false)]
	[Serializable]
	public enum PropertyLockMode
	{
		Method = 1,
		SetGet = 0
	}
}
