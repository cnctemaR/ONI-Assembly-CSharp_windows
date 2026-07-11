using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	[ComVisible(true)]
	[Serializable]
	public sealed class IndexerNameAttribute : Attribute
	{
		public IndexerNameAttribute(string indexerName)
		{
		}
	}
}
