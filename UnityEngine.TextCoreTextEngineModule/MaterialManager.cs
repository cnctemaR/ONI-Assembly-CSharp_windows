using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	internal static class MaterialManager
	{
		public static Material GetFallbackMaterial(Material sourceMaterial, Material targetMaterial)
		{
			int instanceID = sourceMaterial.GetInstanceID();
			Texture texture = targetMaterial.GetTexture(TextShaderUtilities.ID_MainTex);
			int instanceID2 = texture.GetInstanceID();
			long num = ((long)instanceID << 32) | (long)((ulong)instanceID2);
			Material material;
			bool flag = MaterialManager.s_FallbackMaterials.TryGetValue(num, out material);
			Material material2;
			if (flag)
			{
				int num2 = sourceMaterial.ComputeCRC();
				int num3 = material.ComputeCRC();
				bool flag2 = num2 == num3;
				if (flag2)
				{
					material2 = material;
				}
				else
				{
					MaterialManager.CopyMaterialPresetProperties(sourceMaterial, material);
					material2 = material;
				}
			}
			else
			{
				bool flag3 = sourceMaterial.HasProperty(TextShaderUtilities.ID_GradientScale) && targetMaterial.HasProperty(TextShaderUtilities.ID_GradientScale);
				if (flag3)
				{
					material = new Material(sourceMaterial);
					material.hideFlags = HideFlags.HideAndDontSave;
					material.SetTexture(TextShaderUtilities.ID_MainTex, texture);
					material.SetFloat(TextShaderUtilities.ID_GradientScale, targetMaterial.GetFloat(TextShaderUtilities.ID_GradientScale));
					material.SetFloat(TextShaderUtilities.ID_TextureWidth, targetMaterial.GetFloat(TextShaderUtilities.ID_TextureWidth));
					material.SetFloat(TextShaderUtilities.ID_TextureHeight, targetMaterial.GetFloat(TextShaderUtilities.ID_TextureHeight));
					material.SetFloat(TextShaderUtilities.ID_WeightNormal, targetMaterial.GetFloat(TextShaderUtilities.ID_WeightNormal));
					material.SetFloat(TextShaderUtilities.ID_WeightBold, targetMaterial.GetFloat(TextShaderUtilities.ID_WeightBold));
				}
				else
				{
					material = new Material(targetMaterial);
				}
				MaterialManager.s_FallbackMaterials.Add(num, material);
				material2 = material;
			}
			return material2;
		}

		public static Material GetFallbackMaterial(FontAsset fontAsset, Material sourceMaterial, int atlasIndex)
		{
			int instanceID = sourceMaterial.GetInstanceID();
			Texture texture = fontAsset.atlasTextures[atlasIndex];
			int instanceID2 = texture.GetInstanceID();
			long num = ((long)instanceID << 32) | (long)((ulong)instanceID2);
			Material material;
			bool flag = MaterialManager.s_FallbackMaterials.TryGetValue(num, out material);
			Material material2;
			if (flag)
			{
				int num2 = sourceMaterial.ComputeCRC();
				int num3 = material.ComputeCRC();
				bool flag2 = num2 == num3;
				if (flag2)
				{
					material2 = material;
				}
				else
				{
					MaterialManager.CopyMaterialPresetProperties(sourceMaterial, material);
					material2 = material;
				}
			}
			else
			{
				material = new Material(sourceMaterial);
				material.SetTexture(TextShaderUtilities.ID_MainTex, texture);
				material.hideFlags = HideFlags.HideAndDontSave;
				MaterialManager.s_FallbackMaterials.Add(num, material);
				material2 = material;
			}
			return material2;
		}

		private static void CopyMaterialPresetProperties(Material source, Material destination)
		{
			bool flag = !source.HasProperty(TextShaderUtilities.ID_GradientScale) || !destination.HasProperty(TextShaderUtilities.ID_GradientScale);
			if (!flag)
			{
				Texture texture = destination.GetTexture(TextShaderUtilities.ID_MainTex);
				float @float = destination.GetFloat(TextShaderUtilities.ID_GradientScale);
				float float2 = destination.GetFloat(TextShaderUtilities.ID_TextureWidth);
				float float3 = destination.GetFloat(TextShaderUtilities.ID_TextureHeight);
				float float4 = destination.GetFloat(TextShaderUtilities.ID_WeightNormal);
				float float5 = destination.GetFloat(TextShaderUtilities.ID_WeightBold);
				destination.shader = source.shader;
				destination.CopyPropertiesFromMaterial(source);
				destination.shaderKeywords = source.shaderKeywords;
				destination.SetTexture(TextShaderUtilities.ID_MainTex, texture);
				destination.SetFloat(TextShaderUtilities.ID_GradientScale, @float);
				destination.SetFloat(TextShaderUtilities.ID_TextureWidth, float2);
				destination.SetFloat(TextShaderUtilities.ID_TextureHeight, float3);
				destination.SetFloat(TextShaderUtilities.ID_WeightNormal, float4);
				destination.SetFloat(TextShaderUtilities.ID_WeightBold, float5);
			}
		}

		private static Dictionary<long, Material> s_FallbackMaterials = new Dictionary<long, Material>();
	}
}
