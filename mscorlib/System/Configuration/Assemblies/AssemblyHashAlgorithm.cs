using System;
using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[ComVisible(true)]
	[Serializable]
	public enum AssemblyHashAlgorithm
	{
		None,
		MD5 = 32771,
		SHA1
	}
}
