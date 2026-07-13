using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsChainDefinition
	{
		public PhysicsChainDefinition()
		{
			this = PhysicsChainDefinition.defaultDefinition;
		}

		public PhysicsChainDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.PhysicsChain_GetDefaultDefinition(useSettings);
		}

		public static PhysicsChainDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsChain_GetDefaultDefinition(true);
			}
		}

		public PhysicsShape.SurfaceMaterial surfaceMaterial
		{
			readonly get
			{
				return this.m_SurfaceMaterial;
			}
			set
			{
				this.m_SurfaceMaterial = value;
			}
		}

		public PhysicsShape.ContactFilter contactFilter
		{
			readonly get
			{
				return this.m_ContactFilter;
			}
			set
			{
				this.m_ContactFilter = value;
			}
		}

		public bool isLoop
		{
			readonly get
			{
				return this.m_IsLoop;
			}
			set
			{
				this.m_IsLoop = value;
			}
		}

		public bool triggerEvents
		{
			readonly get
			{
				return this.m_TriggerEvents;
			}
			set
			{
				this.m_TriggerEvents = value;
			}
		}

		[SerializeField]
		private PhysicsShape.SurfaceMaterial m_SurfaceMaterial;

		[SerializeField]
		private PhysicsShape.ContactFilter m_ContactFilter;

		[SerializeField]
		private bool m_IsLoop;

		[SerializeField]
		private bool m_TriggerEvents;
	}
}
