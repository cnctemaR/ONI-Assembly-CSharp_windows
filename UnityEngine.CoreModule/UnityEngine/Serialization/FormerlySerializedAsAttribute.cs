using System;
using UnityEngine.Scripting;

namespace UnityEngine.Serialization
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	[RequiredByNativeCode]
	public class FormerlySerializedAsAttribute : Attribute
	{
		public FormerlySerializedAsAttribute(string oldName)
		{
			this.m_oldName = oldName;
		}

		public string oldName
		{
			get
			{
				return this.m_oldName;
			}
		}

		private string m_oldName;
	}
}
