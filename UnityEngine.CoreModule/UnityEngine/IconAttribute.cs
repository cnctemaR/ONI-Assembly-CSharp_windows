using System;
using System.Diagnostics;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	[Conditional("UNITY_EDITOR")]
	internal class IconAttribute : Attribute
	{
		public string path
		{
			get
			{
				return this.m_IconPath;
			}
		}

		private IconAttribute()
		{
		}

		public IconAttribute(string path)
		{
			this.m_IconPath = path;
		}

		private string m_IconPath;
	}
}
