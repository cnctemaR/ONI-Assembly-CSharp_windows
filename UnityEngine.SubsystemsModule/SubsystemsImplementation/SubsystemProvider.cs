using System;

namespace UnityEngine.SubsystemsImplementation
{
	public abstract class SubsystemProvider
	{
		public bool running
		{
			get
			{
				return this.m_Running;
			}
		}

		internal bool m_Running;
	}
}
