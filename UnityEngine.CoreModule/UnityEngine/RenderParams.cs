using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderParams
	{
		public RenderParams(Material mat)
		{
			this.layer = 0;
			this.renderingLayerMask = RenderingLayerMask.defaultRenderingLayerMask;
			this.rendererPriority = 0;
			this.worldBounds = new Bounds(Vector3.zero, Vector3.zero);
			this.camera = null;
			this.motionVectorMode = MotionVectorGenerationMode.Camera;
			this.reflectionProbeUsage = ReflectionProbeUsage.Off;
			this.material = mat;
			this.matProps = null;
			this.shadowCastingMode = ShadowCastingMode.Off;
			this.receiveShadows = false;
			this.lightProbeUsage = LightProbeUsage.Off;
			this.lightProbeProxyVolume = null;
			this.overrideSceneCullingMask = false;
			this.sceneCullingMask = 0UL;
			this.entityId = EntityId.None;
			this.forceMeshLod = -1;
			this.meshLodSelectionBias = 0f;
		}

		public int layer { readonly get; set; }

		public uint renderingLayerMask { readonly get; set; }

		public int rendererPriority { readonly get; set; }

		[Obsolete("Please use entityId instead.", false)]
		public int instanceID
		{
			get
			{
				return this.entityId;
			}
			set
			{
				this.entityId = value;
			}
		}

		public EntityId entityId { readonly get; set; }

		public Bounds worldBounds { readonly get; set; }

		public Camera camera { readonly get; set; }

		public MotionVectorGenerationMode motionVectorMode { readonly get; set; }

		public ReflectionProbeUsage reflectionProbeUsage { readonly get; set; }

		public Material material { readonly get; set; }

		public MaterialPropertyBlock matProps { readonly get; set; }

		public ShadowCastingMode shadowCastingMode { readonly get; set; }

		public bool receiveShadows { readonly get; set; }

		public LightProbeUsage lightProbeUsage { readonly get; set; }

		public LightProbeProxyVolume lightProbeProxyVolume { readonly get; set; }

		public bool overrideSceneCullingMask { readonly get; set; }

		public ulong sceneCullingMask { readonly get; set; }

		public int forceMeshLod { readonly get; set; }

		public float meshLodSelectionBias { readonly get; set; }
	}
}
