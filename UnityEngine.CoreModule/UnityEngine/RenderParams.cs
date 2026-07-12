using System;
using UnityEngine.Rendering;

namespace UnityEngine
{
	public struct RenderParams
	{
		public RenderParams(Material mat)
		{
			this.layer = 0;
			this.renderingLayerMask = GraphicsSettings.defaultRenderingLayerMask;
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
		}

		public int layer { readonly get; set; }

		public uint renderingLayerMask { readonly get; set; }

		public int rendererPriority { readonly get; set; }

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
	}
}
