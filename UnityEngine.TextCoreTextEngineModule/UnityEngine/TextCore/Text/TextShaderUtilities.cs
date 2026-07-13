using System;
using System.Linq;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.TextCore.Text
{
	[ExcludeFromDocs]
	public static class TextShaderUtilities
	{
		internal static Shader ShaderRef_MobileSDF
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			get
			{
				bool flag = TextShaderUtilities.k_ShaderRef_MobileSDF == null;
				if (flag)
				{
					TextShaderUtilities.k_ShaderRef_MobileSDF = Shader.Find(TextShaderUtilities.k_SDFText);
				}
				return TextShaderUtilities.k_ShaderRef_MobileSDF;
			}
		}

		internal static Shader ShaderRef_MobileBitmap
		{
			get
			{
				bool flag = TextShaderUtilities.k_ShaderRef_MobileBitmap == null;
				if (flag)
				{
					TextShaderUtilities.k_ShaderRef_MobileBitmap = Shader.Find(TextShaderUtilities.k_BitmapText);
				}
				return TextShaderUtilities.k_ShaderRef_MobileBitmap;
			}
		}

		internal static Shader ShaderRef_Sprite
		{
			get
			{
				bool flag = TextShaderUtilities.k_ShaderRef_Sprite == null;
				if (flag)
				{
					TextShaderUtilities.k_ShaderRef_Sprite = Shader.Find("Text/Sprite");
					bool flag2 = TextShaderUtilities.k_ShaderRef_Sprite == null;
					if (flag2)
					{
						TextShaderUtilities.k_ShaderRef_Sprite = Shader.Find(TextShaderUtilities.k_SpriteText);
					}
				}
				return TextShaderUtilities.k_ShaderRef_Sprite;
			}
		}

		static TextShaderUtilities()
		{
			TextShaderUtilities.GetShaderPropertyIDs();
		}

		internal static void GetShaderPropertyIDs()
		{
			bool flag = !TextShaderUtilities.isInitialized;
			if (flag)
			{
				TextShaderUtilities.isInitialized = true;
				TextShaderUtilities.ID_MainTex = Shader.PropertyToID("_MainTex");
				TextShaderUtilities.ID_FaceTex = Shader.PropertyToID("_FaceTex");
				TextShaderUtilities.ID_FaceColor = Shader.PropertyToID("_FaceColor");
				TextShaderUtilities.ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
				TextShaderUtilities.ID_Shininess = Shader.PropertyToID("_FaceShininess");
				TextShaderUtilities.ID_OutlineOffset1 = Shader.PropertyToID("_OutlineOffset1");
				TextShaderUtilities.ID_OutlineOffset2 = Shader.PropertyToID("_OutlineOffset2");
				TextShaderUtilities.ID_OutlineOffset3 = Shader.PropertyToID("_OutlineOffset3");
				TextShaderUtilities.ID_OutlineMode = Shader.PropertyToID("_OutlineMode");
				TextShaderUtilities.ID_IsoPerimeter = Shader.PropertyToID("_IsoPerimeter");
				TextShaderUtilities.ID_Softness = Shader.PropertyToID("_Softness");
				TextShaderUtilities.ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
				TextShaderUtilities.ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
				TextShaderUtilities.ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
				TextShaderUtilities.ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
				TextShaderUtilities.ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
				TextShaderUtilities.ID_UnderlayOffset = Shader.PropertyToID("_UnderlayOffset");
				TextShaderUtilities.ID_UnderlayIsoPerimeter = Shader.PropertyToID("_UnderlayIsoPerimeter");
				TextShaderUtilities.ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
				TextShaderUtilities.ID_WeightBold = Shader.PropertyToID("_WeightBold");
				TextShaderUtilities.ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
				TextShaderUtilities.ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
				TextShaderUtilities.ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
				TextShaderUtilities.ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
				TextShaderUtilities.ID_Outline2Color = Shader.PropertyToID("_Outline2Color");
				TextShaderUtilities.ID_Outline2Width = Shader.PropertyToID("_Outline2Width");
				TextShaderUtilities.ID_Padding = Shader.PropertyToID("_Padding");
				TextShaderUtilities.ID_GradientScale = Shader.PropertyToID("_GradientScale");
				TextShaderUtilities.ID_ScaleX = Shader.PropertyToID("_ScaleX");
				TextShaderUtilities.ID_ScaleY = Shader.PropertyToID("_ScaleY");
				TextShaderUtilities.ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
				TextShaderUtilities.ID_Sharpness = Shader.PropertyToID("_Sharpness");
				TextShaderUtilities.ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
				TextShaderUtilities.ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
				TextShaderUtilities.ID_BevelAmount = Shader.PropertyToID("_Bevel");
				TextShaderUtilities.ID_LightAngle = Shader.PropertyToID("_LightAngle");
				TextShaderUtilities.ID_EnvMap = Shader.PropertyToID("_Cube");
				TextShaderUtilities.ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
				TextShaderUtilities.ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
				TextShaderUtilities.ID_GlowColor = Shader.PropertyToID("_GlowColor");
				TextShaderUtilities.ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
				TextShaderUtilities.ID_GlowPower = Shader.PropertyToID("_GlowPower");
				TextShaderUtilities.ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
				TextShaderUtilities.ID_GlowInner = Shader.PropertyToID("_GlowInner");
				TextShaderUtilities.ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
				TextShaderUtilities.ID_ClipRect = Shader.PropertyToID("_ClipRect");
				TextShaderUtilities.ID_UseClipRect = Shader.PropertyToID("_UseClipRect");
				TextShaderUtilities.ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
				TextShaderUtilities.ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
				TextShaderUtilities.ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
				TextShaderUtilities.ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
				TextShaderUtilities.ID_StencilID = Shader.PropertyToID("_Stencil");
				TextShaderUtilities.ID_StencilOp = Shader.PropertyToID("_StencilOp");
				TextShaderUtilities.ID_StencilComp = Shader.PropertyToID("_StencilComp");
				TextShaderUtilities.ID_StencilReadMask = Shader.PropertyToID("_StencilReadMask");
				TextShaderUtilities.ID_StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");
				TextShaderUtilities.ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
				TextShaderUtilities.ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
				TextShaderUtilities.ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
				TextShaderUtilities.ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
			}
		}

		private static void UpdateShaderRatios(Material mat)
		{
			bool flag = !mat.shaderKeywords.Contains(TextShaderUtilities.Keyword_Ratios);
			bool flag2 = !mat.HasProperty(TextShaderUtilities.ID_GradientScale) || !mat.HasProperty(TextShaderUtilities.ID_FaceDilate);
			if (!flag2)
			{
				float @float = mat.GetFloat(TextShaderUtilities.ID_GradientScale);
				float float2 = mat.GetFloat(TextShaderUtilities.ID_FaceDilate);
				float float3 = mat.GetFloat(TextShaderUtilities.ID_OutlineWidth);
				float float4 = mat.GetFloat(TextShaderUtilities.ID_OutlineSoftness);
				float num = Mathf.Max(mat.GetFloat(TextShaderUtilities.ID_WeightNormal), mat.GetFloat(TextShaderUtilities.ID_WeightBold)) / 4f;
				float num2 = Mathf.Max(1f, num + float2 + float3 + float4);
				float num3 = (flag ? ((@float - TextShaderUtilities.m_clamp) / (@float * num2)) : 1f);
				mat.SetFloat(TextShaderUtilities.ID_ScaleRatio_A, num3);
				bool flag3 = mat.HasProperty(TextShaderUtilities.ID_GlowOffset);
				if (flag3)
				{
					float float5 = mat.GetFloat(TextShaderUtilities.ID_GlowOffset);
					float float6 = mat.GetFloat(TextShaderUtilities.ID_GlowOuter);
					float num4 = (num + float2) * (@float - TextShaderUtilities.m_clamp);
					num2 = Mathf.Max(1f, float5 + float6);
					float num5 = (flag ? (Mathf.Max(0f, @float - TextShaderUtilities.m_clamp - num4) / (@float * num2)) : 1f);
					mat.SetFloat(TextShaderUtilities.ID_ScaleRatio_B, num5);
				}
				bool flag4 = mat.HasProperty(TextShaderUtilities.ID_UnderlayOffsetX);
				if (flag4)
				{
					float float7 = mat.GetFloat(TextShaderUtilities.ID_UnderlayOffsetX);
					float float8 = mat.GetFloat(TextShaderUtilities.ID_UnderlayOffsetY);
					float float9 = mat.GetFloat(TextShaderUtilities.ID_UnderlayDilate);
					float float10 = mat.GetFloat(TextShaderUtilities.ID_UnderlaySoftness);
					float num6 = (num + float2) * (@float - TextShaderUtilities.m_clamp);
					num2 = Mathf.Max(1f, Mathf.Max(Mathf.Abs(float7), Mathf.Abs(float8)) + float9 + float10);
					float num7 = (flag ? (Mathf.Max(0f, @float - TextShaderUtilities.m_clamp - num6) / (@float * num2)) : 1f);
					mat.SetFloat(TextShaderUtilities.ID_ScaleRatio_C, num7);
				}
			}
		}

		internal static Vector4 GetFontExtent(Material material)
		{
			return Vector4.zero;
		}

		internal static bool IsMaskingEnabled(Material material)
		{
			bool flag = material == null || !material.HasProperty(TextShaderUtilities.ID_ClipRect);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = material.shaderKeywords.Contains(TextShaderUtilities.Keyword_MASK_SOFT) || material.shaderKeywords.Contains(TextShaderUtilities.Keyword_MASK_HARD) || material.shaderKeywords.Contains(TextShaderUtilities.Keyword_MASK_TEX);
				flag2 = flag3;
			}
			return flag2;
		}

		internal static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
		{
			bool flag = !TextShaderUtilities.isInitialized;
			if (flag)
			{
				TextShaderUtilities.GetShaderPropertyIDs();
			}
			bool flag2 = material == null;
			float num;
			if (flag2)
			{
				num = 0f;
			}
			else
			{
				int num2 = (enableExtraPadding ? 4 : 0);
				bool flag3 = !material.HasProperty(TextShaderUtilities.ID_GradientScale);
				if (flag3)
				{
					bool flag4 = material.HasProperty(TextShaderUtilities.ID_Padding);
					if (flag4)
					{
						num2 += (int)material.GetFloat(TextShaderUtilities.ID_Padding);
					}
					num = (float)num2 + 1f;
				}
				else
				{
					bool flag5 = material.HasProperty(TextShaderUtilities.ID_IsoPerimeter);
					if (flag5)
					{
						num = TextShaderUtilities.ComputePaddingForProperties(material) + 0.25f + (float)num2;
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
						TextShaderUtilities.UpdateShaderRatios(material);
						string[] shaderKeywords = material.shaderKeywords;
						bool flag6 = material.HasProperty(TextShaderUtilities.ID_ScaleRatio_A);
						if (flag6)
						{
							num6 = material.GetFloat(TextShaderUtilities.ID_ScaleRatio_A);
						}
						bool flag7 = material.HasProperty(TextShaderUtilities.ID_FaceDilate);
						if (flag7)
						{
							num3 = material.GetFloat(TextShaderUtilities.ID_FaceDilate) * num6;
						}
						bool flag8 = material.HasProperty(TextShaderUtilities.ID_OutlineSoftness);
						if (flag8)
						{
							num4 = material.GetFloat(TextShaderUtilities.ID_OutlineSoftness) * num6;
						}
						bool flag9 = material.HasProperty(TextShaderUtilities.ID_OutlineWidth);
						if (flag9)
						{
							num5 = material.GetFloat(TextShaderUtilities.ID_OutlineWidth) * num6;
						}
						float num11 = num5 + num4 + num3;
						bool flag10 = material.HasProperty(TextShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(TextShaderUtilities.Keyword_Glow);
						if (flag10)
						{
							bool flag11 = material.HasProperty(TextShaderUtilities.ID_ScaleRatio_B);
							if (flag11)
							{
								num7 = material.GetFloat(TextShaderUtilities.ID_ScaleRatio_B);
							}
							num9 = material.GetFloat(TextShaderUtilities.ID_GlowOffset) * num7;
							num10 = material.GetFloat(TextShaderUtilities.ID_GlowOuter) * num7;
						}
						num11 = Mathf.Max(num11, num3 + num9 + num10);
						bool flag12 = material.HasProperty(TextShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(TextShaderUtilities.Keyword_Underlay);
						if (flag12)
						{
							bool flag13 = material.HasProperty(TextShaderUtilities.ID_ScaleRatio_C);
							if (flag13)
							{
								num8 = material.GetFloat(TextShaderUtilities.ID_ScaleRatio_C);
							}
							float num12 = 0f;
							float num13 = 0f;
							float num14 = 0f;
							float num15 = 0f;
							bool flag14 = material.HasProperty(TextShaderUtilities.ID_UnderlayOffset);
							if (flag14)
							{
								Vector2 vector2 = material.GetVector(TextShaderUtilities.ID_UnderlayOffset);
								num12 = vector2.x;
								num13 = vector2.y;
								num14 = material.GetFloat(TextShaderUtilities.ID_UnderlayDilate);
								num15 = material.GetFloat(TextShaderUtilities.ID_UnderlaySoftness);
							}
							else
							{
								bool flag15 = material.HasProperty(TextShaderUtilities.ID_UnderlayOffsetX);
								if (flag15)
								{
									num12 = material.GetFloat(TextShaderUtilities.ID_UnderlayOffsetX) * num8;
									num13 = material.GetFloat(TextShaderUtilities.ID_UnderlayOffsetY) * num8;
									num14 = material.GetFloat(TextShaderUtilities.ID_UnderlayDilate) * num8;
									num15 = material.GetFloat(TextShaderUtilities.ID_UnderlaySoftness) * num8;
								}
							}
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
						zero.x = ((zero.x < vector.x) ? vector.x : zero.x);
						zero.y = ((zero.y < vector.y) ? vector.y : zero.y);
						zero.z = ((zero.z < vector.z) ? vector.z : zero.z);
						zero.w = ((zero.w < vector.w) ? vector.w : zero.w);
						float @float = material.GetFloat(TextShaderUtilities.ID_GradientScale);
						vector *= @float;
						num11 = Mathf.Max(vector.x, vector.y);
						num11 = Mathf.Max(vector.z, num11);
						num11 = Mathf.Max(vector.w, num11);
						num = num11 + 1.25f;
					}
				}
			}
			return num;
		}

		private static float ComputePaddingForProperties(Material mat)
		{
			Vector4 vector = mat.GetVector(TextShaderUtilities.ID_IsoPerimeter);
			Vector2 vector2 = mat.GetVector(TextShaderUtilities.ID_OutlineOffset1);
			Vector2 vector3 = mat.GetVector(TextShaderUtilities.ID_OutlineOffset2);
			Vector2 vector4 = mat.GetVector(TextShaderUtilities.ID_OutlineOffset3);
			bool flag = mat.GetFloat(TextShaderUtilities.ID_OutlineMode) != 0f;
			Vector4 vector5 = mat.GetVector(TextShaderUtilities.ID_Softness);
			float @float = mat.GetFloat(TextShaderUtilities.ID_GradientScale);
			float num = Mathf.Max(0f, vector.x + vector5.x * 0.5f);
			bool flag2 = !flag;
			if (flag2)
			{
				num = Mathf.Max(num, vector.y + vector5.y * 0.5f + Mathf.Max(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y)));
				num = Mathf.Max(num, vector.z + vector5.z * 0.5f + Mathf.Max(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y)));
				num = Mathf.Max(num, vector.w + vector5.w * 0.5f + Mathf.Max(Mathf.Abs(vector4.x), Mathf.Abs(vector4.y)));
			}
			else
			{
				float num2 = Mathf.Max(Mathf.Abs(vector2.x), Mathf.Abs(vector2.y));
				float num3 = Mathf.Max(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y));
				num = Mathf.Max(num, vector.y + vector5.y * 0.5f + num2);
				num = Mathf.Max(num, vector.z + vector5.z * 0.5f + num3);
				float num4 = Mathf.Max(num2, num3);
				num += Mathf.Max(0f, vector.w + vector5.w * 0.5f - Mathf.Max(0f, num - num4));
			}
			Vector2 vector6 = mat.GetVector(TextShaderUtilities.ID_UnderlayOffset);
			float float2 = mat.GetFloat(TextShaderUtilities.ID_UnderlayDilate);
			float float3 = mat.GetFloat(TextShaderUtilities.ID_UnderlaySoftness);
			num = Mathf.Max(num, float2 + float3 * 0.5f + Mathf.Max(Mathf.Abs(vector6.x), Mathf.Abs(vector6.y)));
			return num * @float;
		}

		internal static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
		{
			bool flag = !TextShaderUtilities.isInitialized;
			if (flag)
			{
				TextShaderUtilities.GetShaderPropertyIDs();
			}
			bool flag2 = materials == null;
			float num;
			if (flag2)
			{
				num = 0f;
			}
			else
			{
				int num2 = (enableExtraPadding ? 4 : 0);
				bool flag3 = materials[0].HasProperty(TextShaderUtilities.ID_Padding);
				if (flag3)
				{
					num = (float)num2 + materials[0].GetFloat(TextShaderUtilities.ID_Padding);
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
						TextShaderUtilities.UpdateShaderRatios(materials[i]);
						string[] shaderKeywords = materials[i].shaderKeywords;
						bool flag4 = materials[i].HasProperty(TextShaderUtilities.ID_ScaleRatio_A);
						if (flag4)
						{
							num6 = materials[i].GetFloat(TextShaderUtilities.ID_ScaleRatio_A);
						}
						bool flag5 = materials[i].HasProperty(TextShaderUtilities.ID_FaceDilate);
						if (flag5)
						{
							num3 = materials[i].GetFloat(TextShaderUtilities.ID_FaceDilate) * num6;
						}
						bool flag6 = materials[i].HasProperty(TextShaderUtilities.ID_OutlineSoftness);
						if (flag6)
						{
							num4 = materials[i].GetFloat(TextShaderUtilities.ID_OutlineSoftness) * num6;
						}
						bool flag7 = materials[i].HasProperty(TextShaderUtilities.ID_OutlineWidth);
						if (flag7)
						{
							num5 = materials[i].GetFloat(TextShaderUtilities.ID_OutlineWidth) * num6;
						}
						num11 = num5 + num4 + num3;
						bool flag8 = materials[i].HasProperty(TextShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(TextShaderUtilities.Keyword_Glow);
						if (flag8)
						{
							bool flag9 = materials[i].HasProperty(TextShaderUtilities.ID_ScaleRatio_B);
							if (flag9)
							{
								num7 = materials[i].GetFloat(TextShaderUtilities.ID_ScaleRatio_B);
							}
							num9 = materials[i].GetFloat(TextShaderUtilities.ID_GlowOffset) * num7;
							num10 = materials[i].GetFloat(TextShaderUtilities.ID_GlowOuter) * num7;
						}
						num11 = Mathf.Max(num11, num3 + num9 + num10);
						bool flag10 = materials[i].HasProperty(TextShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(TextShaderUtilities.Keyword_Underlay);
						if (flag10)
						{
							bool flag11 = materials[i].HasProperty(TextShaderUtilities.ID_ScaleRatio_C);
							if (flag11)
							{
								num8 = materials[i].GetFloat(TextShaderUtilities.ID_ScaleRatio_C);
							}
							float num12 = materials[i].GetFloat(TextShaderUtilities.ID_UnderlayOffsetX) * num8;
							float num13 = materials[i].GetFloat(TextShaderUtilities.ID_UnderlayOffsetY) * num8;
							float num14 = materials[i].GetFloat(TextShaderUtilities.ID_UnderlayDilate) * num8;
							float num15 = materials[i].GetFloat(TextShaderUtilities.ID_UnderlaySoftness) * num8;
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
						zero.x = ((zero.x < vector.x) ? vector.x : zero.x);
						zero.y = ((zero.y < vector.y) ? vector.y : zero.y);
						zero.z = ((zero.z < vector.z) ? vector.z : zero.z);
						zero.w = ((zero.w < vector.w) ? vector.w : zero.w);
					}
					float @float = materials[0].GetFloat(TextShaderUtilities.ID_GradientScale);
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

		public static int ID_OutlineOffset1;

		public static int ID_OutlineOffset2;

		public static int ID_OutlineOffset3;

		public static int ID_OutlineMode;

		public static int ID_IsoPerimeter;

		public static int ID_Softness;

		public static int ID_UnderlayColor;

		public static int ID_UnderlayOffsetX;

		public static int ID_UnderlayOffsetY;

		public static int ID_UnderlayDilate;

		public static int ID_UnderlaySoftness;

		public static int ID_UnderlayOffset;

		public static int ID_UnderlayIsoPerimeter;

		public static int ID_WeightNormal;

		public static int ID_WeightBold;

		public static int ID_OutlineTex;

		public static int ID_OutlineWidth;

		public static int ID_OutlineSoftness;

		public static int ID_OutlineColor;

		public static int ID_Outline2Color;

		public static int ID_Outline2Width;

		public static int ID_Padding;

		public static int ID_GradientScale;

		public static int ID_ScaleX;

		public static int ID_ScaleY;

		public static int ID_PerspectiveFilter;

		public static int ID_Sharpness;

		public static int ID_TextureWidth;

		public static int ID_TextureHeight;

		public static int ID_BevelAmount;

		public static int ID_GlowColor;

		public static int ID_GlowOffset;

		public static int ID_GlowPower;

		public static int ID_GlowOuter;

		public static int ID_GlowInner;

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

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static readonly string k_SDFText = "Hidden/TextCore/Distance Field SSD";

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static readonly string k_BitmapText = "Hidden/Internal-GUITextureClipText";

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static readonly string k_SpriteText = "Hidden/TextCore/Sprite";

		private static Shader k_ShaderRef_MobileSDF;

		private static Shader k_ShaderRef_MobileBitmap;

		private static Shader k_ShaderRef_Sprite;
	}
}
