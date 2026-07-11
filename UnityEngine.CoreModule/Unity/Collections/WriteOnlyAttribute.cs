using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	/// <summary>
	///   <para>The WriteOnly attribute lets you mark a member of a struct used in a job as write-only.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public sealed class WriteOnlyAttribute : Attribute
	{
	}
}
