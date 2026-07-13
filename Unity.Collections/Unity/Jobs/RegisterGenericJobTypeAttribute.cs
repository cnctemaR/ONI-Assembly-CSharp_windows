using System;
using UnityEngine.Scripting.APIUpdating;

namespace Unity.Jobs
{
	[MovedFrom(true, "Unity.Entities", "Unity.Entities", null)]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterGenericJobTypeAttribute : Attribute
	{
		public RegisterGenericJobTypeAttribute(Type type)
		{
			this.ConcreteType = type;
		}

		public Type ConcreteType;
	}
}
