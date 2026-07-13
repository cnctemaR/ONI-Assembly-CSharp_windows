using System;
using System.ComponentModel;
using UnityEngine.Serialization;

namespace UnityEngine.LowLevelPhysics2D
{
	[Serializable]
	public struct PhysicsWorldDefinition
	{
		[Obsolete("PhysicsWorldDefinition.simulationMode has been deprecated. Please use PhysicsWorldDefinition.simulateType instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SimulationMode2D simulationMode
		{
			readonly get
			{
				return (SimulationMode2D)this.simulateType;
			}
			set
			{
				this.simulateType = (PhysicsWorld.SimulationType)value;
			}
		}

		public PhysicsWorldDefinition()
		{
			this = PhysicsWorldDefinition.defaultDefinition;
		}

		public PhysicsWorldDefinition(bool useSettings)
		{
			this = PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultDefinition(useSettings);
		}

		public static PhysicsWorldDefinition defaultDefinition
		{
			get
			{
				return PhysicsLowLevelScripting2D.PhysicsWorld_GetDefaultDefinition(true);
			}
		}

		public Vector2 gravity
		{
			readonly get
			{
				return this.m_Gravity;
			}
			set
			{
				this.m_Gravity = value;
			}
		}

		public PhysicsWorld.SimulationType simulateType
		{
			readonly get
			{
				return this.m_SimulationType;
			}
			set
			{
				this.m_SimulationType = value;
			}
		}

		public int simulationSubSteps
		{
			readonly get
			{
				return this.m_SimulationSubSteps;
			}
			set
			{
				this.m_SimulationSubSteps = Mathf.Max(1, value);
			}
		}

		public int simulationWorkers
		{
			readonly get
			{
				return this.m_SimulationWorkers;
			}
			set
			{
				this.m_SimulationWorkers = Mathf.Clamp(value, 0, 64);
			}
		}

		public PhysicsWorld.TransformWriteMode transformWriteMode
		{
			readonly get
			{
				return this.m_TransformWriteMode;
			}
			set
			{
				this.m_TransformWriteMode = value;
			}
		}

		public PhysicsWorld.TransformPlane transformPlane
		{
			readonly get
			{
				return this.m_TransformPlane;
			}
			set
			{
				this.m_TransformPlane = value;
			}
		}

		public bool transformTweening
		{
			readonly get
			{
				return this.m_TransformTweening;
			}
			set
			{
				this.m_TransformTweening = value;
			}
		}

		public bool sleepingAllowed
		{
			readonly get
			{
				return this.m_SleepingAllowed;
			}
			set
			{
				this.m_SleepingAllowed = value;
			}
		}

		public bool continuousAllowed
		{
			readonly get
			{
				return this.m_ContinuousAllowed;
			}
			set
			{
				this.m_ContinuousAllowed = value;
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

		public bool autoBodyUpdateCallbacks
		{
			readonly get
			{
				return this.m_AutoBodyUpdateCallbacks;
			}
			set
			{
				this.m_AutoBodyUpdateCallbacks = value;
			}
		}

		public bool autoContactCallbacks
		{
			readonly get
			{
				return this.m_AutoContactCallbacks;
			}
			set
			{
				this.m_AutoContactCallbacks = value;
			}
		}

		public bool autoTriggerCallbacks
		{
			readonly get
			{
				return this.m_AutoTriggerCallbacks;
			}
			set
			{
				this.m_AutoTriggerCallbacks = value;
			}
		}

		public bool autoJointThresholdCallbacks
		{
			readonly get
			{
				return this.m_AutoJointThresholdCallbacks;
			}
			set
			{
				this.m_AutoJointThresholdCallbacks = value;
			}
		}

		public float bounceThreshold
		{
			readonly get
			{
				return this.m_BounceThreshold;
			}
			set
			{
				this.m_BounceThreshold = Mathf.Max(0f, value);
			}
		}

		public float contactHitEventThreshold
		{
			readonly get
			{
				return this.m_ContactHitEventThreshold;
			}
			set
			{
				this.m_ContactHitEventThreshold = Mathf.Max(0f, value);
			}
		}

		public float contactFrequency
		{
			readonly get
			{
				return this.m_ContactFrequency;
			}
			set
			{
				this.m_ContactFrequency = Mathf.Max(0f, value);
			}
		}

		public float contactDamping
		{
			readonly get
			{
				return this.m_ContactDamping;
			}
			set
			{
				this.m_ContactDamping = Mathf.Max(0f, value);
			}
		}

		public float contactSpeed
		{
			readonly get
			{
				return this.m_ContactSpeed;
			}
			set
			{
				this.m_ContactSpeed = Mathf.Max(0f, value);
			}
		}

		public float maximumLinearSpeed
		{
			readonly get
			{
				return this.m_MaximumLinearSpeed;
			}
			set
			{
				this.m_MaximumLinearSpeed = Mathf.Max(0f, value);
			}
		}

		public PhysicsWorld.DrawOptions drawOptions
		{
			readonly get
			{
				return this.m_DrawOptions;
			}
			set
			{
				this.m_DrawOptions = value;
			}
		}

		public PhysicsWorld.DrawFillOptions drawFillOptions
		{
			readonly get
			{
				return this.m_DrawFillOptions;
			}
			set
			{
				this.m_DrawFillOptions = value;
			}
		}

		public float drawThickness
		{
			readonly get
			{
				return this.m_DrawThickness;
			}
			set
			{
				this.m_DrawThickness = Mathf.Clamp(value, 1f, 10f);
			}
		}

		public float drawFillAlpha
		{
			readonly get
			{
				return this.m_DrawFillAlpha;
			}
			set
			{
				this.m_DrawFillAlpha = Mathf.Clamp01(value);
			}
		}

		public float drawPointScale
		{
			readonly get
			{
				return this.m_DrawPointScale;
			}
			set
			{
				this.m_DrawPointScale = Mathf.Clamp(value, 0.001f, 10f);
			}
		}

		public float drawNormalScale
		{
			readonly get
			{
				return this.m_DrawNormalScale;
			}
			set
			{
				this.m_DrawNormalScale = Mathf.Clamp(value, 0.001f, 10f);
			}
		}

		public float drawImpulseScale
		{
			readonly get
			{
				return this.m_DrawImpulseScale;
			}
			set
			{
				this.m_DrawImpulseScale = Mathf.Clamp(value, 0.001f, 10f);
			}
		}

		public int drawCapacity
		{
			readonly get
			{
				return this.m_DrawCapacity;
			}
			set
			{
				this.m_DrawCapacity = Mathf.Max(0, value);
			}
		}

		public PhysicsWorld.DrawColors drawColors
		{
			readonly get
			{
				return this.m_DrawColors;
			}
			set
			{
				this.m_DrawColors = value;
			}
		}

		[SerializeField]
		private Vector2 m_Gravity;

		[FormerlySerializedAs("m_SimulationMode")]
		[SerializeField]
		private PhysicsWorld.SimulationType m_SimulationType;

		[Min(1f)]
		[SerializeField]
		private int m_SimulationSubSteps;

		[Range(0f, 64f)]
		[SerializeField]
		private int m_SimulationWorkers;

		[SerializeField]
		private PhysicsWorld.TransformWriteMode m_TransformWriteMode;

		[SerializeField]
		private PhysicsWorld.TransformPlane m_TransformPlane;

		[SerializeField]
		private bool m_TransformTweening;

		[SerializeField]
		private bool m_SleepingAllowed;

		[SerializeField]
		private bool m_ContinuousAllowed;

		[SerializeField]
		private bool m_ContactFilterCallbacks;

		[SerializeField]
		private bool m_PreSolveCallbacks;

		[SerializeField]
		private bool m_AutoBodyUpdateCallbacks;

		[SerializeField]
		private bool m_AutoContactCallbacks;

		[SerializeField]
		private bool m_AutoTriggerCallbacks;

		[SerializeField]
		private bool m_AutoJointThresholdCallbacks;

		[SerializeField]
		[Min(0f)]
		private float m_BounceThreshold;

		[SerializeField]
		[Min(0f)]
		private float m_ContactHitEventThreshold;

		[Min(0f)]
		[SerializeField]
		private float m_ContactFrequency;

		[Min(0f)]
		[SerializeField]
		private float m_ContactDamping;

		[Min(0f)]
		[SerializeField]
		private float m_ContactSpeed;

		[Min(0f)]
		[SerializeField]
		private float m_MaximumLinearSpeed;

		[SerializeField]
		private PhysicsWorld.DrawOptions m_DrawOptions;

		[SerializeField]
		private PhysicsWorld.DrawFillOptions m_DrawFillOptions;

		[SerializeField]
		[Range(1f, 10f)]
		private float m_DrawThickness;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_DrawFillAlpha;

		[Range(0.001f, 10f)]
		[SerializeField]
		private float m_DrawPointScale;

		[Range(0.001f, 10f)]
		[SerializeField]
		private float m_DrawNormalScale;

		[SerializeField]
		[Range(0.001f, 10f)]
		private float m_DrawImpulseScale;

		[SerializeField]
		[Min(0f)]
		private int m_DrawCapacity;

		[SerializeField]
		private PhysicsWorld.DrawColors m_DrawColors;
	}
}
