using System;
using System.Linq;
using UnityEngine;

namespace TMPro
{
	public static class ShaderUtilities
	{
		public static void GetShaderPropertyIDs()
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.isInitialized = true;
				ShaderUtilities.ID_MainTex = Shader.PropertyToID("_MainTex");
				ShaderUtilities.ID_FaceTex = Shader.PropertyToID("_FaceTex");
				ShaderUtilities.ID_FaceColor = Shader.PropertyToID("_FaceColor");
				ShaderUtilities.ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
				ShaderUtilities.ID_Shininess = Shader.PropertyToID("_FaceShininess");
				ShaderUtilities.ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
				ShaderUtilities.ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
				ShaderUtilities.ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
				ShaderUtilities.ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
				ShaderUtilities.ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
				ShaderUtilities.ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
				ShaderUtilities.ID_WeightBold = Shader.PropertyToID("_WeightBold");
				ShaderUtilities.ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
				ShaderUtilities.ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
				ShaderUtilities.ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
				ShaderUtilities.ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
				ShaderUtilities.ID_GradientScale = Shader.PropertyToID("_GradientScale");
				ShaderUtilities.ID_ScaleX = Shader.PropertyToID("_ScaleX");
				ShaderUtilities.ID_ScaleY = Shader.PropertyToID("_ScaleY");
				ShaderUtilities.ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
				ShaderUtilities.ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
				ShaderUtilities.ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
				ShaderUtilities.ID_BevelAmount = Shader.PropertyToID("_Bevel");
				ShaderUtilities.ID_LightAngle = Shader.PropertyToID("_LightAngle");
				ShaderUtilities.ID_EnvMap = Shader.PropertyToID("_Cube");
				ShaderUtilities.ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
				ShaderUtilities.ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
				ShaderUtilities.ID_GlowColor = Shader.PropertyToID("_GlowColor");
				ShaderUtilities.ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
				ShaderUtilities.ID_GlowPower = Shader.PropertyToID("_GlowPower");
				ShaderUtilities.ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
				ShaderUtilities.ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
				ShaderUtilities.ID_ClipRect = Shader.PropertyToID("_ClipRect");
				ShaderUtilities.ID_UseClipRect = Shader.PropertyToID("_UseClipRect");
				ShaderUtilities.ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
				ShaderUtilities.ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
				ShaderUtilities.ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
				ShaderUtilities.ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
				ShaderUtilities.ID_StencilID = Shader.PropertyToID("_Stencil");
				ShaderUtilities.ID_StencilOp = Shader.PropertyToID("_StencilOp");
				ShaderUtilities.ID_StencilComp = Shader.PropertyToID("_StencilComp");
				ShaderUtilities.ID_StencilReadMask = Shader.PropertyToID("_StencilReadMask");
				ShaderUtilities.ID_StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");
				ShaderUtilities.ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
				ShaderUtilities.ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
				ShaderUtilities.ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
				ShaderUtilities.ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
			}
		}

		public static void UpdateShaderRatios(Material mat, bool isBold)
		{
			bool flag = !mat.shaderKeywords.Contains(ShaderUtilities.Keyword_Ratios);
			float @float = mat.GetFloat(ShaderUtilities.ID_GradientScale);
			float float2 = mat.GetFloat(ShaderUtilities.ID_FaceDilate);
			float float3 = mat.GetFloat(ShaderUtilities.ID_OutlineWidth);
			float float4 = mat.GetFloat(ShaderUtilities.ID_OutlineSoftness);
			float num = (isBold ? (mat.GetFloat(ShaderUtilities.ID_WeightBold) * 2f / @float) : (mat.GetFloat(ShaderUtilities.ID_WeightNormal) * 2f / @float));
			float num2 = Mathf.Max(1f, num + float2 + float3 + float4);
			float num3 = ((!flag) ? 1f : ((@float - ShaderUtilities.m_clamp) / (@float * num2)));
			mat.SetFloat(ShaderUtilities.ID_ScaleRatio_A, num3);
			if (mat.HasProperty(ShaderUtilities.ID_GlowOffset))
			{
				float float5 = mat.GetFloat(ShaderUtilities.ID_GlowOffset);
				float float6 = mat.GetFloat(ShaderUtilities.ID_GlowOuter);
				float num4 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, float5 + float6);
				float num5 = ((!flag) ? 1f : (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num4) / (@float * num2)));
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_B, num5);
			}
			if (mat.HasProperty(ShaderUtilities.ID_UnderlayOffsetX))
			{
				float float7 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetX);
				float float8 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetY);
				float float9 = mat.GetFloat(ShaderUtilities.ID_UnderlayDilate);
				float float10 = mat.GetFloat(ShaderUtilities.ID_UnderlaySoftness);
				float num6 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, Mathf.Max(Mathf.Abs(float7), Mathf.Abs(float8)) + float9 + float10);
				float num7 = ((!flag) ? 1f : (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num6) / (@float * num2)));
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_C, num7);
			}
		}

		public static Vector4 GetFontExtent(Material material)
		{
			return Vector4.zero;
		}

		public static bool IsMaskingEnabled(Material material)
		{
			return !(material == null) && material.HasProperty(ShaderUtilities.ID_ClipRect) && (material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_SOFT) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_HARD) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_TEX));
		}

		public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			float num;
			if (material == null)
			{
				num = 0f;
			}
			else
			{
				int num2 = ((!enableExtraPadding) ? 0 : 4);
				if (!material.HasProperty(ShaderUtilities.ID_GradientScale))
				{
					num = (float)num2;
				}
				else
				{
					Vector4 vector = Vector4.zero;
					Vector4 zero = Vector4.zero;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					float num6 = 0f;
					float num7 = 0f;
					float num8 = 0f;
					float num9 = 0f;
					float num10 = 0f;
					ShaderUtilities.UpdateShaderRatios(material, isBold);
					string[] shaderKeywords = material.shaderKeywords;
					if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_A))
					{
						num6 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
					}
					if (material.HasProperty(ShaderUtilities.ID_FaceDilate))
					{
						num3 = material.GetFloat(ShaderUtilities.ID_FaceDilate) * num6;
					}
					if (material.HasProperty(ShaderUtilities.ID_OutlineSoftness))
					{
						num4 = material.GetFloat(ShaderUtilities.ID_OutlineSoftness) * num6;
					}
					if (material.HasProperty(ShaderUtilities.ID_OutlineWidth))
					{
						num5 = material.GetFloat(ShaderUtilities.ID_OutlineWidth) * num6;
					}
					float num11 = num5 + num4 + num3;
					if (material.HasProperty(ShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(ShaderUtilities.Keyword_Glow))
					{
						if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_B))
						{
							num7 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_B);
						}
						num9 = material.GetFloat(ShaderUtilities.ID_GlowOffset) * num7;
						num10 = material.GetFloat(ShaderUtilities.ID_GlowOuter) * num7;
					}
					num11 = Mathf.Max(num11, num3 + num9 + num10);
					if (material.HasProperty(ShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(ShaderUtilities.Keyword_Underlay))
					{
						if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_C))
						{
							num8 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_C);
						}
						float num12 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num8;
						float num13 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num8;
						float num14 = material.GetFloat(ShaderUtilities.ID_UnderlayDilate) * num8;
						float num15 = material.GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num8;
						vector.x = Mathf.Max(vector.x, num3 + num14 + num15 - num12);
						vector.y = Mathf.Max(vector.y, num3 + num14 + num15 - num13);
						vector.z = Mathf.Max(vector.z, num3 + num14 + num15 + num12);
						vector.w = Mathf.Max(vector.w, num3 + num14 + num15 + num13);
					}
					vector.x = Mathf.Max(vector.x, num11);
					vector.y = Mathf.Max(vector.y, num11);
					vector.z = Mathf.Max(vector.z, num11);
					vector.w = Mathf.Max(vector.w, num11);
					vector.x += (float)num2;
					vector.y += (float)num2;
					vector.z += (float)num2;
					vector.w += (float)num2;
					vector.x = Mathf.Min(vector.x, 1f);
					vector.y = Mathf.Min(vector.y, 1f);
					vector.z = Mathf.Min(vector.z, 1f);
					vector.w = Mathf.Min(vector.w, 1f);
					zero.x = ((zero.x >= vector.x) ? zero.x : vector.x);
					zero.y = ((zero.y >= vector.y) ? zero.y : vector.y);
					zero.z = ((zero.z >= vector.z) ? zero.z : vector.z);
					zero.w = ((zero.w >= vector.w) ? zero.w : vector.w);
					float @float = material.GetFloat(ShaderUtilities.ID_GradientScale);
					vector *= @float;
					num11 = Mathf.Max(vector.x, vector.y);
					num11 = Mathf.Max(vector.z, num11);
					num11 = Mathf.Max(vector.w, num11);
					num = num11 + 0.5f;
				}
			}
			return num;
		}

		public static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			float num;
			if (materials == null)
			{
				num = 0f;
			}
			else
			{
				int num2 = ((!enableExtraPadding) ? 0 : 4);
				if (!materials[0].HasProperty(ShaderUtilities.ID_GradientScale))
				{
					num = (float)num2;
				}
				else
				{
					Vector4 vector = Vector4.zero;
					Vector4 zero = Vector4.zero;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					float num6 = 0f;
					float num7 = 0f;
					float num8 = 0f;
					float num9 = 0f;
					float num10 = 0f;
					float num11;
					for (int i = 0; i < materials.Length; i++)
					{
						ShaderUtilities.UpdateShaderRatios(materials[i], isBold);
						string[] shaderKeywords = materials[i].shaderKeywords;
						if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_A))
						{
							num6 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_A);
						}
						if (materials[i].HasProperty(ShaderUtilities.ID_FaceDilate))
						{
							num3 = materials[i].GetFloat(ShaderUtilities.ID_FaceDilate) * num6;
						}
						if (materials[i].HasProperty(ShaderUtilities.ID_OutlineSoftness))
						{
							num4 = materials[i].GetFloat(ShaderUtilities.ID_OutlineSoftness) * num6;
						}
						if (materials[i].HasProperty(ShaderUtilities.ID_OutlineWidth))
						{
							num5 = materials[i].GetFloat(ShaderUtilities.ID_OutlineWidth) * num6;
						}
						num11 = num5 + num4 + num3;
						if (materials[i].HasProperty(ShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(ShaderUtilities.Keyword_Glow))
						{
							if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_B))
							{
								num7 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_B);
							}
							num9 = materials[i].GetFloat(ShaderUtilities.ID_GlowOffset) * num7;
							num10 = materials[i].GetFloat(ShaderUtilities.ID_GlowOuter) * num7;
						}
						num11 = Mathf.Max(num11, num3 + num9 + num10);
						if (materials[i].HasProperty(ShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(ShaderUtilities.Keyword_Underlay))
						{
							if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_C))
							{
								num8 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_C);
							}
							float num12 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num8;
							float num13 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num8;
							float num14 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayDilate) * num8;
							float num15 = materials[i].GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num8;
							vector.x = Mathf.Max(vector.x, num3 + num14 + num15 - num12);
							vector.y = Mathf.Max(vector.y, num3 + num14 + num15 - num13);
							vector.z = Mathf.Max(vector.z, num3 + num14 + num15 + num12);
							vector.w = Mathf.Max(vector.w, num3 + num14 + num15 + num13);
						}
						vector.x = Mathf.Max(vector.x, num11);
						vector.y = Mathf.Max(vector.y, num11);
						vector.z = Mathf.Max(vector.z, num11);
						vector.w = Mathf.Max(vector.w, num11);
						vector.x += (float)num2;
						vector.y += (float)num2;
						vector.z += (float)num2;
						vector.w += (float)num2;
						vector.x = Mathf.Min(vector.x, 1f);
						vector.y = Mathf.Min(vector.y, 1f);
						vector.z = Mathf.Min(vector.z, 1f);
						vector.w = Mathf.Min(vector.w, 1f);
						zero.x = ((zero.x >= vector.x) ? zero.x : vector.x);
						zero.y = ((zero.y >= vector.y) ? zero.y : vector.y);
						zero.z = ((zero.z >= vector.z) ? zero.z : vector.z);
						zero.w = ((zero.w >= vector.w) ? zero.w : vector.w);
					}
					float @float = materials[0].GetFloat(ShaderUtilities.ID_GradientScale);
					vector *= @float;
					num11 = Mathf.Max(vector.x, vector.y);
					num11 = Mathf.Max(vector.z, num11);
					num11 = Mathf.Max(vector.w, num11);
					num = num11 + 0.25f;
				}
			}
			return num;
		}

		public static int ID_MainTex;

		public static int ID_FaceTex;

		public static int ID_FaceColor;

		public static int ID_FaceDilate;

		public static int ID_Shininess;

		public static int ID_UnderlayColor;

		public static int ID_UnderlayOffsetX;

		public static int ID_UnderlayOffsetY;

		public static int ID_UnderlayDilate;

		public static int ID_UnderlaySoftness;

		public static int ID_WeightNormal;

		public static int ID_WeightBold;

		public static int ID_OutlineTex;

		public static int ID_OutlineWidth;

		public static int ID_OutlineSoftness;

		public static int ID_OutlineColor;

		public static int ID_GradientScale;

		public static int ID_ScaleX;

		public static int ID_ScaleY;

		public static int ID_PerspectiveFilter;

		public static int ID_TextureWidth;

		public static int ID_TextureHeight;

		public static int ID_BevelAmount;

		public static int ID_GlowColor;

		public static int ID_GlowOffset;

		public static int ID_GlowPower;

		public static int ID_GlowOuter;

		public static int ID_LightAngle;

		public static int ID_EnvMap;

		public static int ID_EnvMatrix;

		public static int ID_EnvMatrixRotation;

		public static int ID_MaskCoord;

		public static int ID_ClipRect;

		public static int ID_MaskSoftnessX;

		public static int ID_MaskSoftnessY;

		public static int ID_VertexOffsetX;

		public static int ID_VertexOffsetY;

		public static int ID_UseClipRect;

		public static int ID_StencilID;

		public static int ID_StencilOp;

		public static int ID_StencilComp;

		public static int ID_StencilReadMask;

		public static int ID_StencilWriteMask;

		public static int ID_ShaderFlags;

		public static int ID_ScaleRatio_A;

		public static int ID_ScaleRatio_B;

		public static int ID_ScaleRatio_C;

		public static string Keyword_Bevel = "BEVEL_ON";

		public static string Keyword_Glow = "GLOW_ON";

		public static string Keyword_Underlay = "UNDERLAY_ON";

		public static string Keyword_Ratios = "RATIOS_OFF";

		public static string Keyword_MASK_SOFT = "MASK_SOFT";

		public static string Keyword_MASK_HARD = "MASK_HARD";

		public static string Keyword_MASK_TEX = "MASK_TEX";

		public static string Keyword_Outline = "OUTLINE_ON";

		public static string ShaderTag_ZTestMode = "unity_GUIZTestMode";

		public static string ShaderTag_CullMode = "_CullMode";

		private static float m_clamp = 1f;

		public static bool isInitialized = false;
	}
}
