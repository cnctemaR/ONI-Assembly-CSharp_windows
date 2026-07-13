using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.VectorGraphics
{
	[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
	internal static class VectorImageUtils
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
		internal static void MakeVectorImageAsset(IEnumerable<VectorUtils.Geometry> geoms, Rect rect, uint rasterSize, out VectorImage outAsset, out Texture2D outTexAtlas)
		{
			VectorUtils.TextureAtlas textureAtlas = VectorUtils.GenerateAtlas(geoms, rasterSize, false, false, false);
			bool flag = textureAtlas != null;
			if (flag)
			{
				VectorUtils.FillUVs(geoms, textureAtlas);
			}
			bool flag2 = textureAtlas != null && textureAtlas.Texture != null;
			outTexAtlas = (flag2 ? textureAtlas.Texture : null);
			List<VectorImageVertex> list = new List<VectorImageVertex>(100);
			List<ushort> list2 = new List<ushort>(300);
			List<GradientSettings> list3 = new List<GradientSettings>();
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				bool flag3 = geometry.Vertices.Length == 0;
				if (!flag3)
				{
					Vector2[] array = new Vector2[geometry.Vertices.Length];
					for (int i = 0; i < geometry.Vertices.Length; i++)
					{
						Vector2 vector3 = geometry.WorldTransform.MultiplyPoint(geometry.Vertices[i]);
						array[i] = vector3;
					}
					Rect rect2 = VectorUtils.Bounds(array);
					vector = Vector2.Min(vector, rect2.min);
					vector2 = Vector2.Max(vector2, rect2.max);
				}
			}
			Rect zero = Rect.zero;
			bool flag4 = vector.x != float.MaxValue;
			if (flag4)
			{
				zero = new Rect(vector, vector2 - vector);
			}
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(0);
			Dictionary<IFill, VectorUtils.PackRectItem> dictionary = new Dictionary<IFill, VectorUtils.PackRectItem>();
			bool flag5 = textureAtlas != null && textureAtlas.Entries != null;
			if (flag5)
			{
				foreach (VectorUtils.PackRectItem packRectItem in textureAtlas.Entries)
				{
					bool flag6 = packRectItem.Fill != null;
					if (flag6)
					{
						dictionary[packRectItem.Fill] = packRectItem;
					}
				}
			}
			bool flag7 = flag2 && textureAtlas != null && textureAtlas.Entries != null && textureAtlas.Entries.Count > 0;
			if (flag7)
			{
				VectorUtils.PackRectItem packRectItem2 = textureAtlas.Entries[textureAtlas.Entries.Count - 1];
				list3.Add(new GradientSettings
				{
					gradientType = GradientType.Linear,
					addressMode = AddressMode.Wrap,
					radialFocus = Vector2.zero,
					location = new RectInt((int)packRectItem2.Position.x, (int)packRectItem2.Position.y, (int)packRectItem2.Size.x, (int)packRectItem2.Size.y)
				});
			}
			foreach (VectorUtils.Geometry geometry2 in geoms)
			{
				for (int j = 0; j < geometry2.Vertices.Length; j++)
				{
					Vector2 vector4 = geometry2.WorldTransform.MultiplyPoint(geometry2.Vertices[j]);
					vector4 -= zero.position;
					geometry2.Vertices[j] = vector4;
				}
				VectorUtils.AdjustWinding(geometry2.Vertices, geometry2.Indices, VectorUtils.WindingDir.CCW);
				int count = list.Count;
				for (int k = 0; k < geometry2.Vertices.Length; k++)
				{
					Vector3 vector5 = geometry2.Vertices[k];
					vector5.z = Vertex.nearZ;
					list.Add(new VectorImageVertex
					{
						position = vector5,
						uv = (flag2 ? geometry2.UVs[k] : Vector2.zero),
						tint = geometry2.Color,
						settingIndex = (uint)geometry2.SettingIndex
					});
				}
				ushort[] array2 = new ushort[geometry2.Indices.Length];
				for (int l = 0; l < geometry2.Indices.Length; l++)
				{
					array2[l] = (ushort)((int)geometry2.Indices[l] + count);
				}
				list2.AddRange(array2);
				bool flag8 = textureAtlas != null && textureAtlas.Entries != null && textureAtlas.Entries.Count > 0;
				if (flag8)
				{
					VectorUtils.PackRectItem packRectItem3;
					bool flag9 = geometry2.Fill == null || !dictionary.TryGetValue(geometry2.Fill, out packRectItem3) || hashSet.Contains(packRectItem3.SettingIndex);
					if (!flag9)
					{
						hashSet.Add(packRectItem3.SettingIndex);
						GradientFillType gradientFillType = GradientFillType.Linear;
						Vector2 vector6 = Vector2.zero;
						AddressMode addressMode = AddressMode.Wrap;
						GradientFill gradientFill = geometry2.Fill as GradientFill;
						bool flag10 = gradientFill != null;
						if (flag10)
						{
							gradientFillType = gradientFill.Type;
							vector6 = gradientFill.RadialFocus;
							addressMode = gradientFill.Addressing;
						}
						TextureFill textureFill = geometry2.Fill as TextureFill;
						bool flag11 = textureFill != null;
						if (flag11)
						{
							addressMode = textureFill.Addressing;
						}
						list3.Add(new GradientSettings
						{
							gradientType = (GradientType)gradientFillType,
							addressMode = (AddressMode)addressMode,
							radialFocus = vector6,
							location = new RectInt((int)packRectItem3.Position.x, (int)packRectItem3.Position.y, (int)packRectItem3.Size.x, (int)packRectItem3.Size.y)
						});
					}
				}
			}
			bool flag12 = rect == Rect.zero;
			if (flag12)
			{
				rect = zero;
			}
			else
			{
				Vector2 vector7 = zero.position - rect.position;
				for (int m = 0; m < list.Count; m++)
				{
					VectorImageVertex vectorImageVertex = list[m];
					Vector2 vector8 = vectorImageVertex.position;
					vector8 += vector7;
					vector8 = Vector2.Max(rect.min, Vector2.Min(rect.max, vector8));
					vectorImageVertex.position = new Vector3(vector8.x, vector8.y, vectorImageVertex.position.z);
					list[m] = vectorImageVertex;
				}
			}
			outAsset = VectorImageUtils.MakeVectorImageAsset(list, list2, outTexAtlas, list3, rect);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
		internal static Texture2D RenderVectorImageToTexture2D(VectorImage vi, int width, int height, Material mat, int antiAliasing = 1)
		{
			bool flag = vi == null;
			Texture2D texture2D;
			if (flag)
			{
				texture2D = null;
			}
			else
			{
				bool flag2 = width <= 0 || height <= 0;
				if (flag2)
				{
					texture2D = null;
				}
				else
				{
					RenderTexture active = RenderTexture.active;
					RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0)
					{
						msaaSamples = antiAliasing,
						sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear)
					};
					RenderTexture temporary = RenderTexture.GetTemporary(renderTextureDescriptor);
					RenderTexture.active = temporary;
					PanelSettings panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
					panelSettings.clearColor = true;
					panelSettings.clearDepthStencil = true;
					panelSettings.targetTexture = temporary;
					GL.PushMatrix();
					BaseRuntimePanel panel = panelSettings.panel;
					VisualElement visualTree = panel.visualTree;
					visualTree.StretchToParentSize();
					visualTree.style.backgroundImage = new StyleBackground(vi);
					panel.Repaint(Event.current);
					panel.Render();
					GL.PopMatrix();
					Object.DestroyImmediate(panelSettings);
					Texture2D texture2D2 = new Texture2D(width, height, TextureFormat.RGBA32, false);
					texture2D2.hideFlags = HideFlags.HideAndDontSave;
					texture2D2.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
					texture2D2.Apply();
					RenderTexture.active = active;
					RenderTexture.ReleaseTemporary(temporary);
					texture2D = texture2D2;
				}
			}
			return texture2D;
		}

		private static Texture2D BuildAtlasWithEncodedSettings(GradientSettings[] settings, Texture2D atlas)
		{
			RenderTexture active = RenderTexture.active;
			int num = atlas.width + 3;
			int num2 = Math.Max(settings.Length, atlas.height);
			RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(num, num2, RenderTextureFormat.ARGB32, 0)
			{
				sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear)
			};
			RenderTexture temporary = RenderTexture.GetTemporary(renderTextureDescriptor);
			GL.Clear(false, true, Color.black, 1f);
			Graphics.Blit(atlas, temporary, Vector2.one, new Vector2(-3f / (float)num, 0f));
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
			VectorUtils.RawTexture rawTexture = new VectorUtils.RawTexture
			{
				Width = 3,
				Height = settings.Length,
				Rgba = new Color32[3 * settings.Length]
			};
			for (int i = 0; i < settings.Length; i++)
			{
				GradientSettings gradientSettings = settings[i];
				int num3 = 0;
				int num4 = i;
				bool flag = gradientSettings.gradientType == GradientType.Radial;
				if (flag)
				{
					Vector2 vector = gradientSettings.radialFocus;
					vector += Vector2.one;
					vector /= 2f;
					vector.y = 1f - vector.y;
					VectorUtils.WriteRawFloat4Packed(rawTexture, (float)gradientSettings.gradientType / 255f, (float)gradientSettings.addressMode / 255f, vector.x, vector.y, num3++, num4);
				}
				else
				{
					VectorUtils.WriteRawFloat4Packed(rawTexture, 0f, (float)gradientSettings.addressMode / 255f, 0f, 0f, num3++, num4);
				}
				Vector2Int position = gradientSettings.location.position;
				Vector2Int size = gradientSettings.location.size;
				size.x--;
				size.y--;
				VectorUtils.WriteRawInt2Packed(rawTexture, position.x + 3, position.y, num3++, num4);
				VectorUtils.WriteRawInt2Packed(rawTexture, size.x, size.y, num3++, num4);
			}
			texture2D.SetPixels32(0, 0, 3, settings.Length, rawTexture.Rgba, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
		internal static VectorImage MakeVectorImageAsset(List<VectorImageVertex> vertices, List<ushort> indices, Texture2D atlas, List<GradientSettings> settings, Rect rect)
		{
			VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
			vectorImage.vertices = vertices.ToArray();
			vectorImage.indices = indices.ToArray();
			vectorImage.atlas = atlas;
			vectorImage.settings = settings.ToArray();
			vectorImage.size = rect.size;
			return vectorImage;
		}
	}
}
