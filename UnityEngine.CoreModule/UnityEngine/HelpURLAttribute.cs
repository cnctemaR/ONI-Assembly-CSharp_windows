using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HelpURLAttribute : Attribute
	{
		public HelpURLAttribute(string url)
		{
			this.m_Url = url;
		}

		public string URL
		{
			get
			{
				return this.m_Url;
			}
		}

		internal readonly string m_Url;
	}
}
