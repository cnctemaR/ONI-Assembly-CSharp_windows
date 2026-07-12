using System;

namespace System.Security.Cryptography.Asn1
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	internal sealed class ChoiceAttribute : Attribute
	{
		public bool AllowNull { get; set; }
	}
}
