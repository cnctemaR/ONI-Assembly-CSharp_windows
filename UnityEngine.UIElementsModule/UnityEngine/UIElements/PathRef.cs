using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class PathRef
	{
		public ref PropertyPath path
		{
			get
			{
				return ref this.m_Path;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.m_Path.IsEmpty;
			}
		}

		private PropertyPath m_Path;
	}
}
