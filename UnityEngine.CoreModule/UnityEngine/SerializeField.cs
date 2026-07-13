using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeField : Attribute
	{
	}
}
