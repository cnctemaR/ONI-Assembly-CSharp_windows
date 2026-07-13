using System;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D
{
	[RequiredByNativeCode]
	[Serializable]
	public sealed class PhysicsLowLevelSettings2D : ScriptableObject
	{
		public PhysicsLowLevelSettings2D()
		{
			this.Reset();
		}

		private void Reset()
		{
			this.m_PhysicsWorldDefinition = new PhysicsWorldDefinition(false);
			this.m_PhysicsBodyDefinition = new PhysicsBodyDefinition(false);
			this.m_PhysicsShapeDefinition = new PhysicsShapeDefinition(false);
			this.m_PhysicsChainDefinition = new PhysicsChainDefinition(false);
			this.m_PhysicsDistanceJointDefinition = new PhysicsDistanceJointDefinition(false);
			this.m_PhysicsFixedJointDefinition = new PhysicsFixedJointDefinition(false);
			this.m_PhysicsHingeJointDefinition = new PhysicsHingeJointDefinition(false);
			this.m_PhysicsRelativeJointDefinition = new PhysicsRelativeJointDefinition(false);
			this.m_PhysicsSliderJointDefinition = new PhysicsSliderJointDefinition(false);
			this.m_PhysicsWheelJointDefinition = new PhysicsWheelJointDefinition(false);
			this.m_PhysicsLayerNames = PhysicsLayers.LayerNames.DefaultLayerNames;
			this.m_ConcurrentSimulations = 2;
			this.m_LengthUnitsPerMeter = 1f;
			this.m_DrawInBuild = false;
			this.m_BypassLowLevel = false;
		}

		public PhysicsLayers.LayerNames physicsLayerNames
		{
			get
			{
				return this.m_PhysicsLayerNames;
			}
			set
			{
				this.m_PhysicsLayerNames = value;
			}
		}

		public bool useFullLayers
		{
			get
			{
				return this.m_UseFullLayers;
			}
			set
			{
				this.m_UseFullLayers = value;
			}
		}

		public PhysicsWorldDefinition physicsWorldDefinition
		{
			get
			{
				return this.m_PhysicsWorldDefinition;
			}
			set
			{
				this.m_PhysicsWorldDefinition = value;
			}
		}

		public PhysicsBodyDefinition physicsBodyDefinition
		{
			get
			{
				return this.m_PhysicsBodyDefinition;
			}
			set
			{
				this.m_PhysicsBodyDefinition = value;
			}
		}

		public PhysicsShapeDefinition physicsShapeDefinition
		{
			get
			{
				return this.m_PhysicsShapeDefinition;
			}
			set
			{
				this.m_PhysicsShapeDefinition = value;
			}
		}

		public PhysicsChainDefinition physicsChainDefinition
		{
			get
			{
				return this.m_PhysicsChainDefinition;
			}
			set
			{
				this.m_PhysicsChainDefinition = value;
			}
		}

		[Range(1f, 64f)]
		public int concurrentSimulations
		{
			get
			{
				return this.m_ConcurrentSimulations;
			}
			set
			{
				this.m_ConcurrentSimulations = Mathf.Clamp(value, 1, 64);
			}
		}

		public float lengthUnitsPerMeter
		{
			get
			{
				return this.m_LengthUnitsPerMeter;
			}
			set
			{
				this.m_LengthUnitsPerMeter = Mathf.Max(1E-05f, value);
			}
		}

		public bool drawInBuild
		{
			get
			{
				return this.m_DrawInBuild;
			}
			set
			{
				this.m_DrawInBuild = value;
			}
		}

		public bool bypassLowLevel
		{
			get
			{
				return this.m_BypassLowLevel;
			}
			set
			{
				this.m_BypassLowLevel = value;
			}
		}

		[RequiredByNativeCode]
		private void GetPhysicsLayerNames(out PhysicsLayers.LayerNames layerNames)
		{
			layerNames = this.m_PhysicsLayerNames;
		}

		[RequiredByNativeCode]
		private void GetPhysicsWorldDefinition(out PhysicsWorldDefinition definition)
		{
			definition = this.m_PhysicsWorldDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsBodyDefinition(out PhysicsBodyDefinition definition)
		{
			definition = this.m_PhysicsBodyDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsShapeDefinition(out PhysicsShapeDefinition definition)
		{
			definition = this.m_PhysicsShapeDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsChainDefinition(out PhysicsChainDefinition definition)
		{
			definition = this.m_PhysicsChainDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsDistanceJointDefinition(out PhysicsDistanceJointDefinition definition)
		{
			definition = this.m_PhysicsDistanceJointDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsFixedJointDefinition(out PhysicsFixedJointDefinition definition)
		{
			definition = this.m_PhysicsFixedJointDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsHingeJointDefinition(out PhysicsHingeJointDefinition definition)
		{
			definition = this.m_PhysicsHingeJointDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsRelativeJointDefinition(out PhysicsRelativeJointDefinition definition)
		{
			definition = this.m_PhysicsRelativeJointDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsSliderJointDefinition(out PhysicsSliderJointDefinition definition)
		{
			definition = this.m_PhysicsSliderJointDefinition;
		}

		[RequiredByNativeCode]
		private void GetPhysicsWheelJointDefinition(out PhysicsWheelJointDefinition definition)
		{
			definition = this.m_PhysicsWheelJointDefinition;
		}

		[RequiredByNativeCode]
		private int GetConcurrentSimulations()
		{
			return this.m_ConcurrentSimulations;
		}

		[RequiredByNativeCode]
		private float GetLengthUnitsPerMeter()
		{
			return this.m_LengthUnitsPerMeter;
		}

		[RequiredByNativeCode]
		private bool GetDrawInBuild()
		{
			return this.m_DrawInBuild;
		}

		[RequiredByNativeCode]
		private bool GetBypassLowLevel()
		{
			return this.m_BypassLowLevel;
		}

		[RequiredByNativeCode]
		private bool GetUseFullLayers()
		{
			return this.m_UseFullLayers;
		}

		[SerializeField]
		[Header("Layers")]
		private PhysicsLayers.LayerNames m_PhysicsLayerNames;

		[SerializeField]
		private bool m_UseFullLayers;

		[SerializeField]
		[Header("Default Definitions")]
		private PhysicsWorldDefinition m_PhysicsWorldDefinition;

		[SerializeField]
		private PhysicsBodyDefinition m_PhysicsBodyDefinition;

		[SerializeField]
		private PhysicsShapeDefinition m_PhysicsShapeDefinition;

		[SerializeField]
		private PhysicsChainDefinition m_PhysicsChainDefinition;

		[SerializeField]
		private PhysicsDistanceJointDefinition m_PhysicsDistanceJointDefinition;

		[SerializeField]
		private PhysicsFixedJointDefinition m_PhysicsFixedJointDefinition;

		[SerializeField]
		private PhysicsHingeJointDefinition m_PhysicsHingeJointDefinition;

		[SerializeField]
		private PhysicsRelativeJointDefinition m_PhysicsRelativeJointDefinition;

		[SerializeField]
		private PhysicsSliderJointDefinition m_PhysicsSliderJointDefinition;

		[SerializeField]
		private PhysicsWheelJointDefinition m_PhysicsWheelJointDefinition;

		[SerializeField]
		[Range(1f, 64f)]
		[Header("Globals")]
		private int m_ConcurrentSimulations;

		[Min(1E-05f)]
		[SerializeField]
		private float m_LengthUnitsPerMeter;

		[SerializeField]
		private bool m_DrawInBuild;

		[SerializeField]
		private bool m_BypassLowLevel;
	}
}
