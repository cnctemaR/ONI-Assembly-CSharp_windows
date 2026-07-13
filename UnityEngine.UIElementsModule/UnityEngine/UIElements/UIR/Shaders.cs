using System;

namespace UnityEngine.UIElements.UIR
{
	internal static class Shaders
	{
		public static Material defaultMaterial
		{
			get
			{
				return Shaders.GetOrCreateMaterial(ref Shaders.s_DefaultMaterial, Shaders.k_Default);
			}
		}

		private static Material GetOrCreateMaterial(ref Material material, string shaderName)
		{
			bool flag = material == null;
			if (flag)
			{
				Shader shader = Shader.Find(shaderName);
				bool flag2 = shader == null;
				if (flag2)
				{
					Debug.LogError("Could not find shader '" + shaderName + "'");
					return null;
				}
				material = new Material(shader);
				material.hideFlags = HideFlags.DontSave;
			}
			return material;
		}

		public static void Acquire()
		{
			Shaders.s_RefCount++;
		}

		public static void Release()
		{
			Shaders.s_RefCount--;
			Debug.Assert(Shaders.s_RefCount >= 0, "UIR materials acquire/release don't match.");
			bool flag = Shaders.s_RefCount < 1;
			if (flag)
			{
				Shaders.s_RefCount = 0;
				UIRUtility.Destroy(Shaders.s_DefaultMaterial);
				Shaders.s_DefaultMaterial = null;
			}
		}

		public static readonly string k_AtlasBlit = "Hidden/Internal-UIRAtlasBlitCopy";

		public static readonly string k_Default = "Hidden/Internal-UIRDefault";

		public static readonly string k_RuntimeGaussianBlur = "Hidden/UIR/GaussianBlur";

		public static readonly string k_RuntimeColorEffect = "Hidden/UIR/ColorEffect";

		public static readonly string k_ColorConversionBlit = "Hidden/Internal-UIE-ColorConversionBlit";

		public static readonly string k_ForceGammaKeyword = "_UIE_FORCE_GAMMA";

		public static readonly string k_TextureSlotCount1 = "_UIE_TEXTURE_SLOT_COUNT_1";

		public static readonly string k_TextureSlotCount2 = "_UIE_TEXTURE_SLOT_COUNT_2";

		public static readonly string k_TextureSlotCount4 = "_UIE_TEXTURE_SLOT_COUNT_4";

		public static readonly string k_ForceRenderTypeSolid = "_UIE_RENDER_TYPE_SOLID";

		public static readonly string k_ForceRenderTypeTextured = "_UIE_RENDER_TYPE_TEXTURE";

		public static readonly string k_ForceRenderTypeText = "_UIE_RENDER_TYPE_TEXT";

		public static readonly string k_ForceRenderTypeSvgGradient = "_UIE_RENDER_TYPE_GRADIENT";

		private static Material s_DefaultMaterial;

		private static int s_RefCount;
	}
}
