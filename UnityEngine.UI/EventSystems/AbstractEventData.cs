using System;

namespace UnityEngine.EventSystems
{
	public abstract class AbstractEventData
	{
		public virtual void Reset()
		{
			this.m_Used = false;
		}

		public virtual void Use()
		{
			this.m_Used = true;
		}

		public virtual bool used
		{
			get
			{
				return this.m_Used;
			}
		}

		protected bool m_Used;
	}
}
