using System;

namespace Unity.Properties
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DontCreatePropertyAttribute : Attribute
	{
	}
}
