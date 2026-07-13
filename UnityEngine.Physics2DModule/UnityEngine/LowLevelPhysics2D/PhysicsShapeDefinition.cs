using System;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsShapeDefinition
	{
		public PhysicsShapeDefinition()
		{
			this = PhysicsShapeDefinition.defaultDefinition;
		}

		public PhysicsShapeDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultDefinition(useSettings);
		}

		public static PhysicsShapeDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsShape_GetDefaultDefinition(true);
			}
		}

		public float density
		{
			readonly get
			{
				return this.m_Density;
			}
			set
			{
				this.m_Density = Mathf.Max(0f, value);
			}
		}

		public bool isTrigger
		{
			readonly get
			{
				return this.m_IsTrigger;
			}
			set
			{
				this.m_IsTrigger = value;
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

		public bool contactEvents
		{
			readonly get
			{
				return this.m_ContactEvents;
			}
			set
			{
				this.m_ContactEvents = value;
			}
		}

		public bool hitEvents
		{
			readonly get
			{
				return this.m_HitEvents;
			}
			set
			{
				this.m_HitEvents = value;
			}
		}

		public bool contactFilterCallbacks
		{
			readonly get
			{
				return this.m_ContactFilterCallbacks;
			}
			set
			{
				this.m_ContactFilterCallbacks = value;
			}
		}

		public bool preSolveCallbacks
		{
			readonly get
			{
				return this.m_PreSolveCallbacks;
			}
			set
			{
				this.m_PreSolveCallbacks = value;
			}
		}

		public bool startStaticContacts
		{
			readonly get
			{
				return this.m_StartStaticContacts;
			}
			set
			{
				this.m_StartStaticContacts = value;
			}
		}

		public bool startMassUpdate
		{
			readonly get
			{
				return this.m_StartMassUpdate;
			}
			set
			{
				this.m_StartMassUpdate = value;
			}
		}

		public PhysicsShape.SurfaceMaterial surfaceMaterial;

		public PhysicsShape.ContactFilter contactFilter;

		public PhysicsShape.MoverData moverData;

		[Min(0f)]
		[SerializeField]
		private float m_Density;

		[SerializeField]
		private bool m_IsTrigger;

		[SerializeField]
		private bool m_TriggerEvents;

		[SerializeField]
		private bool m_ContactEvents;

		[SerializeField]
		private bool m_HitEvents;

		[SerializeField]
		private bool m_ContactFilterCallbacks;

		[SerializeField]
		private bool m_PreSolveCallbacks;

		[SerializeField]
		private bool m_StartStaticContacts;

		[SerializeField]
		private bool m_StartMassUpdate;
	}
}
