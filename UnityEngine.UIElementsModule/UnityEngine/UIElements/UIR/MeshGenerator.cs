using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements.UIR
{
	internal class MeshGenerator : IMeshGenerator, IDisposable
	{
		public MeshGenerator(MeshGenerationContext mgc)
		{
			this.m_MeshGenerationContext = mgc;
			this.m_OnMeshGenerationDelegate = new MeshGenerationCallback(this.OnMeshGeneration);
			this.textJobSystem = new TextJobSystem();
		}

		public VisualElement currentElement { get; set; }

		public TextJobSystem textJobSystem { get; set; }

		private static Vector2 ConvertBorderRadiusPercentToPoints(Vector2 borderRectSize, Length length)
		{
			float num = length.value;
			float num2 = length.value;
			bool flag = length.unit == LengthUnit.Percent;
			if (flag)
			{
				num = borderRectSize.x * length.value / 100f;
				num2 = borderRectSize.y * length.value / 100f;
			}
			num = Mathf.Max(num, 0f);
			num2 = Mathf.Max(num2, 0f);
			return new Vector2(num, num2);
		}

		public unsafe static void GetVisualElementRadii(VisualElement ve, out Vector2 topLeft, out Vector2 bottomLeft, out Vector2 topRight, out Vector2 bottomRight)
		{
			IResolvedStyle resolvedStyle = ve.resolvedStyle;
			Vector2 vector = new Vector2(resolvedStyle.width, resolvedStyle.height);
			ComputedStyle computedStyle = *ve.computedStyle;
			topLeft = MeshGenerator.ConvertBorderRadiusPercentToPoints(vector, computedStyle.borderTopLeftRadius);
			bottomLeft = MeshGenerator.ConvertBorderRadiusPercentToPoints(vector, computedStyle.borderBottomLeftRadius);
			topRight = MeshGenerator.ConvertBorderRadiusPercentToPoints(vector, computedStyle.borderTopRightRadius);
			bottomRight = MeshGenerator.ConvertBorderRadiusPercentToPoints(vector, computedStyle.borderBottomRightRadius);
		}

		public static void AdjustBackgroundSizeForBorders(VisualElement visualElement, ref MeshGenerator.RectangleParams rectParams)
		{
			IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
			Vector4 zero = Vector4.zero;
			bool flag = resolvedStyle.borderLeftWidth >= 1f && resolvedStyle.borderLeftColor.a >= 1f;
			if (flag)
			{
				zero.x = 0.5f;
			}
			bool flag2 = resolvedStyle.borderTopWidth >= 1f && resolvedStyle.borderTopColor.a >= 1f;
			if (flag2)
			{
				zero.y = 0.5f;
			}
			bool flag3 = resolvedStyle.borderRightWidth >= 1f && resolvedStyle.borderRightColor.a >= 1f;
			if (flag3)
			{
				zero.z = 0.5f;
			}
			bool flag4 = resolvedStyle.borderBottomWidth >= 1f && resolvedStyle.borderBottomColor.a >= 1f;
			if (flag4)
			{
				zero.w = 0.5f;
			}
			rectParams.rectInset = zero;
		}

		public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font)
		{
			TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(this.currentElement);
			this.m_TextInfo.Clear();
			this.m_Settings.text = text;
			this.m_Settings.fontAsset = font;
			this.m_Settings.textSettings = textSettingsFrom;
			this.m_Settings.fontSize = (int)Mathf.Round(fontSize);
			this.m_Settings.color = color;
			this.m_Settings.textWrappingMode = TextWrappingMode.NoWrap;
			TextGenerator.GetTextGenerator().GenerateText(this.m_Settings, this.m_TextInfo);
			this.DrawTextBase(this.m_TextInfo, default(NativeTextInfo), pos, false);
		}

		private unsafe void DrawTextBase(TextInfo textInfo, NativeTextInfo nativeTextInfo, Vector2 pos, bool isNative)
		{
			int i = 0;
			int num = (isNative ? nativeTextInfo.meshInfoCount : textInfo.meshInfo.Length);
			while (i < num)
			{
				MeshInfo meshInfo = default(MeshInfo);
				FontAsset fontAsset = null;
				SpriteAsset spriteAsset = null;
				bool flag = false;
				Span<ATGMeshInfo> span = default(Span<ATGMeshInfo>);
				ATGMeshInfo atgmeshInfo = default(ATGMeshInfo);
				bool flag2 = !isNative;
				int j;
				if (flag2)
				{
					meshInfo = textInfo.meshInfo[i];
					Debug.Assert((meshInfo.vertexCount & 3) == 0);
					j = meshInfo.vertexCount;
					goto IL_00F0;
				}
				atgmeshInfo = *nativeTextInfo.meshInfos[i];
				int length = atgmeshInfo.textElementInfos.Length;
				j = length * 4;
				TextAsset textAssetByID = TextAsset.GetTextAssetByID(atgmeshInfo.textAssetId);
				bool flag3 = textAssetByID == null;
				if (!flag3)
				{
					bool flag4 = textAssetByID is FontAsset;
					if (flag4)
					{
						fontAsset = textAssetByID as FontAsset;
					}
					else
					{
						flag = true;
						spriteAsset = textAssetByID as SpriteAsset;
					}
					goto IL_00F0;
				}
				IL_047E:
				i++;
				continue;
				IL_00F0:
				int num2 = (int)((ulong)UIRenderDevice.maxVerticesPerPage & 18446744073709551612UL);
				float num3 = 1f / this.currentElement.scaledPixelsPerPoint;
				while (j > 0)
				{
					int num4 = Mathf.Min(j, num2);
					int num5 = num4 >> 2;
					int num6 = num5 * 6;
					Texture2D texture2D;
					GlyphRenderMode glyphRenderMode;
					if (isNative)
					{
						bool flag5 = flag;
						if (flag5)
						{
							texture2D = (Texture2D)spriteAsset.material.mainTexture;
							glyphRenderMode = GlyphRenderMode.COLOR;
						}
						else
						{
							texture2D = (Texture2D)fontAsset.material.mainTexture;
							glyphRenderMode = fontAsset.atlasRenderMode;
						}
					}
					else
					{
						texture2D = (Texture2D)meshInfo.material.mainTexture;
						glyphRenderMode = meshInfo.glyphRenderMode;
					}
					this.m_Atlases.Add(texture2D);
					this.m_RenderModes.Add(glyphRenderMode);
					float num7 = 0f;
					List<GlyphRenderMode> renderModes = this.m_RenderModes;
					bool flag6;
					if (!TextGeneratorUtilities.IsBitmapRendering(renderModes[renderModes.Count - 1]))
					{
						List<Texture2D> atlases = this.m_Atlases;
						flag6 = atlases[atlases.Count - 1].format == TextureFormat.Alpha8;
					}
					else
					{
						flag6 = false;
					}
					bool flag7 = flag6;
					if (flag7)
					{
						if (isNative)
						{
							num7 = (float)(flag ? 0 : (fontAsset.atlasPadding + 1));
						}
						else
						{
							num7 = meshInfo.material.GetFloat(TextShaderUtilities.ID_GradientScale);
						}
					}
					this.m_SdfScales.Add(num7);
					NativeSlice<Vertex> nativeSlice;
					NativeSlice<ushort> nativeSlice2;
					this.m_MeshGenerationContext.AllocateTempMesh(num4, num6, out nativeSlice, out nativeSlice2);
					int k = 0;
					int num8 = 0;
					int num9 = 0;
					while (k < num4)
					{
						if (isNative)
						{
							Span<NativeTextElementInfo> textElementInfos = atgmeshInfo.textElementInfos;
							bool flag8 = !flag && (fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED);
							nativeSlice[k] = MeshGenerator.ConvertTextVertexToUIRVertex(ref textElementInfos[num8].bottomLeft, pos, num3, false, flag8);
							nativeSlice[k + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref textElementInfos[num8].topLeft, pos, num3, false, flag8);
							nativeSlice[k + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref textElementInfos[num8].topRight, pos, num3, false, flag8);
							nativeSlice[k + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref textElementInfos[num8].bottomRight, pos, num3, false, flag8);
						}
						else
						{
							nativeSlice[k] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[k], pos, num3, false, false);
							nativeSlice[k + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[k + 1], pos, num3, false, false);
							nativeSlice[k + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[k + 2], pos, num3, false, false);
							nativeSlice[k + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[k + 3], pos, num3, false, false);
						}
						nativeSlice2[num9] = (ushort)k;
						nativeSlice2[num9 + 1] = (ushort)(k + 1);
						nativeSlice2[num9 + 2] = (ushort)(k + 2);
						nativeSlice2[num9 + 3] = (ushort)(k + 2);
						nativeSlice2[num9 + 4] = (ushort)(k + 3);
						nativeSlice2[num9 + 5] = (ushort)k;
						k += 4;
						num8++;
						num9 += 6;
					}
					this.m_VerticesArray.Add(nativeSlice);
					this.m_IndicesArray.Add(nativeSlice2);
					j -= num4;
				}
				Debug.Assert(j == 0);
				goto IL_047E;
			}
			this.DrawText(this.m_VerticesArray, this.m_IndicesArray, this.m_Atlases, this.m_RenderModes, this.m_SdfScales);
			this.m_VerticesArray.Clear();
			this.m_IndicesArray.Clear();
			this.m_Atlases.Clear();
			this.m_SdfScales.Clear();
			this.m_RenderModes.Clear();
		}

		public void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Material> materials, List<GlyphRenderMode> renderModes)
		{
			for (int i = 0; i < materials.Count; i++)
			{
				Material material = materials[i];
				this.m_Atlases.Add(material.mainTexture as Texture2D);
				float num = 0f;
				bool flag;
				if (!TextGeneratorUtilities.IsBitmapRendering(renderModes[i]))
				{
					List<Texture2D> atlases = this.m_Atlases;
					flag = atlases[atlases.Count - 1].format == TextureFormat.Alpha8;
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					num = material.GetFloat(TextShaderUtilities.ID_GradientScale);
				}
				this.m_SdfScales.Add(num);
			}
			this.DrawText(vertices, indices, this.m_Atlases, renderModes, this.m_SdfScales);
			this.m_Atlases.Clear();
			this.m_SdfScales.Clear();
		}

		public void DrawText(List<NativeSlice<Vertex>> vertices, List<NativeSlice<ushort>> indices, List<Texture2D> atlases, List<GlyphRenderMode> renderModes, List<float> sdfScales)
		{
			bool flag = vertices == null;
			if (!flag)
			{
				int i = 0;
				int count = vertices.Count;
				while (i < count)
				{
					bool flag2 = vertices[i].Length == 0;
					if (!flag2)
					{
						bool flag3 = atlases[i].format != TextureFormat.Alpha8;
						if (flag3)
						{
							this.MakeText(atlases[i], vertices[i], indices[i], false, 0f, 0f, true);
						}
						else
						{
							float num = 0f;
							this.MakeText(atlases[i], vertices[i], indices[i], true, sdfScales[i], num, false);
						}
					}
					i++;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Vertex ConvertTextVertexToUIRVertex(ref TextCoreVertex vertex, Vector2 posOffset, float inverseScale, bool isDynamicColor = false, bool isColorGlyph = false)
		{
			float num = 0f;
			bool flag = vertex.uv2.y < 0f;
			if (flag)
			{
				num = 1f;
			}
			return new Vertex
			{
				position = new Vector3(vertex.position.x * inverseScale + posOffset.x, vertex.position.y * inverseScale + posOffset.y),
				uv = new Vector2(vertex.uv0.x, vertex.uv0.y),
				tint = (isColorGlyph ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertex.color.a) : vertex.color),
				flags = new Color32(0, (byte)(num * 255f), 0, isDynamicColor ? 2 : 0)
			};
		}

		private void MakeText(Texture texture, NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, bool isSdf, float sdfScale, float sharpness, bool multiChannel)
		{
			if (isSdf)
			{
				this.m_MeshGenerationContext.entryRecorder.DrawSdfText(this.m_MeshGenerationContext.parentEntry, vertices, indices, texture, sdfScale, sharpness);
			}
			else
			{
				this.m_MeshGenerationContext.entryRecorder.DrawRasterText(this.m_MeshGenerationContext.parentEntry, vertices, indices, texture, multiChannel);
			}
		}

		public void DrawRectangle(MeshGenerator.RectangleParams rectParams)
		{
			bool flag = rectParams.rect.width < 1E-30f || rectParams.rect.height < 1E-30f;
			if (!flag)
			{
				MeshGenerator.TessellationJobParameters tessellationJobParameters = new MeshGenerator.TessellationJobParameters
				{
					isBorderJob = false
				};
				rectParams.ToNativeParams(out tessellationJobParameters.rectParams);
				tessellationJobParameters.rectParams.texture = this.m_GCHandlePool.GetIntPtr(rectParams.texture);
				tessellationJobParameters.rectParams.sprite = this.m_GCHandlePool.GetIntPtr(rectParams.sprite);
				bool flag2 = rectParams.sprite != null && rectParams.sprite.texture != null;
				if (flag2)
				{
					tessellationJobParameters.rectParams.spriteTexture = this.m_GCHandlePool.GetIntPtr(rectParams.sprite.texture);
					tessellationJobParameters.rectParams.spriteVertices = this.m_GCHandlePool.GetIntPtr(rectParams.sprite.vertices);
					tessellationJobParameters.rectParams.spriteUVs = this.m_GCHandlePool.GetIntPtr(rectParams.sprite.uv);
					tessellationJobParameters.rectParams.spriteTriangles = this.m_GCHandlePool.GetIntPtr(rectParams.sprite.triangles);
				}
				bool flag3 = rectParams.backgroundRepeatInstanceList != null;
				if (flag3)
				{
					tessellationJobParameters.rectParams.backgroundRepeatInstanceListStartIndex = rectParams.backgroundRepeatInstanceListStartIndex;
					tessellationJobParameters.rectParams.backgroundRepeatInstanceListEndIndex = rectParams.backgroundRepeatInstanceListEndIndex;
					tessellationJobParameters.rectParams.backgroundRepeatInstanceList = this.m_GCHandlePool.GetIntPtr(rectParams.backgroundRepeatInstanceList);
				}
				tessellationJobParameters.rectParams.vectorImage = this.m_GCHandlePool.GetIntPtr(rectParams.vectorImage);
				VectorImage vectorImage = rectParams.vectorImage;
				bool flag4 = ((vectorImage != null) ? vectorImage.atlas : null) != null;
				tessellationJobParameters.rectParams.meshFlags = tessellationJobParameters.rectParams.meshFlags | (flag4 ? 4 : 0);
				UnsafeMeshGenerationNode unsafeMeshGenerationNode;
				this.m_MeshGenerationContext.InsertUnsafeMeshGenerationNode(out unsafeMeshGenerationNode);
				tessellationJobParameters.node = unsafeMeshGenerationNode;
				this.m_TesselationJobParameters.Add(tessellationJobParameters);
			}
		}

		public void DrawBorder(MeshGenerator.BorderParams borderParams)
		{
			MeshGenerator.TessellationJobParameters tessellationJobParameters = new MeshGenerator.TessellationJobParameters
			{
				isBorderJob = true,
				borderParams = borderParams
			};
			UnsafeMeshGenerationNode unsafeMeshGenerationNode;
			this.m_MeshGenerationContext.InsertUnsafeMeshGenerationNode(out unsafeMeshGenerationNode);
			tessellationJobParameters.node = unsafeMeshGenerationNode;
			this.m_TesselationJobParameters.Add(tessellationJobParameters);
		}

		public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
		{
			bool flag = vectorImage == null || vectorImage.vertices.Length == 0 || vectorImage.indices.Length == 0;
			if (!flag)
			{
				NativeSlice<Vertex> nativeSlice;
				NativeSlice<ushort> nativeSlice2;
				this.m_MeshGenerationContext.AllocateTempMesh(vectorImage.vertices.Length, vectorImage.indices.Length, out nativeSlice, out nativeSlice2);
				bool flag2 = vectorImage.atlas != null;
				bool flag3 = flag2;
				if (flag3)
				{
					this.m_MeshGenerationContext.entryRecorder.DrawGradients(this.m_MeshGenerationContext.parentEntry, nativeSlice, nativeSlice2, vectorImage);
				}
				else
				{
					this.m_MeshGenerationContext.entryRecorder.DrawMesh(this.m_MeshGenerationContext.parentEntry, nativeSlice, nativeSlice2);
				}
				Matrix4x4 matrix4x = Matrix4x4.TRS(offset, Quaternion.AngleAxis(rotationAngle.ToDegrees(), Vector3.forward), new Vector3(scale.x, scale.y, 1f));
				bool flag4 = (scale.x < 0f) ^ (scale.y < 0f);
				int num = vectorImage.vertices.Length;
				for (int i = 0; i < num; i++)
				{
					VectorImageVertex vectorImageVertex = vectorImage.vertices[i];
					Vector3 vector = matrix4x.MultiplyPoint3x4(vectorImageVertex.position);
					vector.z = Vertex.nearZ;
					Color32 color = new Color32((byte)(vectorImageVertex.settingIndex >> 8), (byte)vectorImageVertex.settingIndex, 0, 0);
					nativeSlice[i] = new Vertex
					{
						position = vector,
						tint = vectorImageVertex.tint,
						uv = vectorImageVertex.uv,
						settingIndex = color,
						flags = vectorImageVertex.flags,
						circle = vectorImageVertex.circle
					};
				}
				bool flag5 = !flag4;
				if (flag5)
				{
					nativeSlice2.CopyFrom(vectorImage.indices);
				}
				else
				{
					ushort[] indices = vectorImage.indices;
					for (int j = 0; j < indices.Length; j += 3)
					{
						nativeSlice2[j] = indices[j];
						nativeSlice2[j + 1] = indices[j + 2];
						nativeSlice2[j + 2] = indices[j + 1];
					}
				}
			}
		}

		public void DrawRectangleRepeat(MeshGenerator.RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint)
		{
			this.DoDrawRectangleRepeat(ref rectParams, totalRect, scaledPixelsPerPoint);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DoDrawRectangleRepeat(ref MeshGenerator.RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			bool flag = this.m_RepeatRectUVList == null;
			if (flag)
			{
				this.m_RepeatRectUVList = new List<MeshGenerator.RepeatRectUV>[2];
				this.m_RepeatRectUVList[0] = new List<MeshGenerator.RepeatRectUV>();
				this.m_RepeatRectUVList[1] = new List<MeshGenerator.RepeatRectUV>();
			}
			else
			{
				this.m_RepeatRectUVList[0].Clear();
				this.m_RepeatRectUVList[1].Clear();
			}
			Rect rect2 = rectParams.rect;
			bool flag2 = rectParams.backgroundSize.sizeType > BackgroundSizeType.Length;
			if (flag2)
			{
				bool flag3 = rectParams.backgroundSize.sizeType == BackgroundSizeType.Contain;
				if (flag3)
				{
					float num = totalRect.width / rect2.width;
					float num2 = totalRect.height / rect2.height;
					Rect rect3 = rect2;
					bool flag4 = num < num2;
					if (flag4)
					{
						rect3.width = totalRect.width;
						rect3.height = rect2.height * totalRect.width / rect2.width;
					}
					else
					{
						rect3.width = rect2.width * totalRect.height / rect2.height;
						rect3.height = totalRect.height;
					}
					rect2 = rect3;
				}
				else
				{
					bool flag5 = rectParams.backgroundSize.sizeType == BackgroundSizeType.Cover;
					if (flag5)
					{
						float num3 = totalRect.width / rect2.width;
						float num4 = totalRect.height / rect2.height;
						Rect rect4 = rect2;
						bool flag6 = num3 > num4;
						if (flag6)
						{
							rect4.width = totalRect.width;
							rect4.height = rect2.height * totalRect.width / rect2.width;
						}
						else
						{
							rect4.width = rect2.width * totalRect.height / rect2.height;
							rect4.height = totalRect.height;
						}
						rect2 = rect4;
					}
				}
			}
			else
			{
				bool flag7 = !rectParams.backgroundSize.x.IsNone() || !rectParams.backgroundSize.y.IsNone();
				if (flag7)
				{
					bool flag8 = !rectParams.backgroundSize.x.IsNone() && rectParams.backgroundSize.y.IsAuto();
					if (flag8)
					{
						Rect rect5 = rect2;
						bool flag9 = rectParams.backgroundSize.x.unit == LengthUnit.Percent;
						if (flag9)
						{
							rect5.width = totalRect.width * rectParams.backgroundSize.x.value / 100f;
							rect5.height = rect5.width * rect2.height / rect2.width;
						}
						else
						{
							bool flag10 = rectParams.backgroundSize.x.unit == LengthUnit.Pixel;
							if (flag10)
							{
								rect5.width = rectParams.backgroundSize.x.value;
								rect5.height = rect5.width * rect2.height / rect2.width;
							}
						}
						rect2 = rect5;
					}
					else
					{
						bool flag11 = !rectParams.backgroundSize.x.IsNone() && !rectParams.backgroundSize.y.IsNone();
						if (flag11)
						{
							Rect rect6 = rect2;
							bool flag12 = !rectParams.backgroundSize.x.IsAuto();
							if (flag12)
							{
								bool flag13 = rectParams.backgroundSize.x.unit == LengthUnit.Percent;
								if (flag13)
								{
									rect6.width = totalRect.width * rectParams.backgroundSize.x.value / 100f;
								}
								else
								{
									bool flag14 = rectParams.backgroundSize.x.unit == LengthUnit.Pixel;
									if (flag14)
									{
										rect6.width = rectParams.backgroundSize.x.value;
									}
								}
							}
							bool flag15 = !rectParams.backgroundSize.y.IsAuto();
							if (flag15)
							{
								bool flag16 = rectParams.backgroundSize.y.unit == LengthUnit.Percent;
								if (flag16)
								{
									rect6.height = totalRect.height * rectParams.backgroundSize.y.value / 100f;
								}
								else
								{
									bool flag17 = rectParams.backgroundSize.y.unit == LengthUnit.Pixel;
									if (flag17)
									{
										rect6.height = rectParams.backgroundSize.y.value;
									}
								}
								bool flag18 = rectParams.backgroundSize.x.IsAuto();
								if (flag18)
								{
									rect6.width = rect6.height * rect2.width / rect2.height;
								}
							}
							rect2 = rect6;
						}
					}
				}
			}
			bool flag19 = rect2.size.x <= 1E-30f || rect2.size.y <= 1E-30f;
			if (!flag19)
			{
				bool flag20 = totalRect.size.x <= 1E-30f || totalRect.size.y <= 1E-30f;
				if (!flag20)
				{
					bool flag21 = rectParams.backgroundSize.x.IsAuto() && rectParams.backgroundRepeat.y == Repeat.Round;
					if (flag21)
					{
						float num5 = 1f / rect2.height;
						int num6 = (int)(totalRect.height * num5 + 0.5f);
						num6 = Math.Max(num6, 1);
						Rect rect7 = default(Rect);
						rect7.height = totalRect.height / (float)num6;
						rect7.width = rect7.height * rect2.width * num5;
						rect2 = rect7;
					}
					else
					{
						bool flag22 = rectParams.backgroundSize.y.IsAuto() && rectParams.backgroundRepeat.x == Repeat.Round;
						if (flag22)
						{
							float num7 = 1f / rect2.width;
							int num8 = (int)(totalRect.width * num7 + 0.5f);
							num8 = Math.Max(num8, 1);
							Rect rect8 = default(Rect);
							rect8.width = totalRect.width / (float)num8;
							rect8.height = rect8.width * rect2.height * num7;
							rect2 = rect8;
						}
					}
					for (int i = 0; i < 2; i++)
					{
						Repeat repeat = ((i == 0) ? rectParams.backgroundRepeat.x : rectParams.backgroundRepeat.y);
						BackgroundPosition backgroundPosition = ((i == 0) ? rectParams.backgroundPositionX : rectParams.backgroundPositionY);
						float num9 = 0f;
						bool flag23 = repeat == Repeat.NoRepeat;
						if (flag23)
						{
							Rect rect9 = rect2;
							MeshGenerator.RepeatRectUV repeatRectUV;
							repeatRectUV.uv = rect;
							repeatRectUV.rect = rect9;
							num9 = rect9.size[i];
							this.m_RepeatRectUVList[i].Add(repeatRectUV);
						}
						else
						{
							bool flag24 = repeat == Repeat.Repeat;
							if (flag24)
							{
								Rect rect10 = rect2;
								int num10 = (int)((totalRect.size[i] + 1f / scaledPixelsPerPoint) / rect2.size[i]);
								bool flag25 = backgroundPosition.keyword == BackgroundPositionKeyword.Center;
								if (flag25)
								{
									bool flag26 = (num10 & 1) == 1;
									if (flag26)
									{
										num10 += 2;
									}
									else
									{
										num10++;
									}
								}
								else
								{
									num10 += 2;
								}
								for (int j = 0; j < num10; j++)
								{
									Vector2 position = rect10.position;
									position[i] = (float)j * rect2.size[i];
									rect10.position = position;
									MeshGenerator.RepeatRectUV repeatRectUV2;
									repeatRectUV2.rect = rect10;
									repeatRectUV2.uv = rect;
									num9 += repeatRectUV2.rect.size[i];
									this.m_RepeatRectUVList[i].Add(repeatRectUV2);
								}
							}
							else
							{
								bool flag27 = repeat == Repeat.Space;
								if (flag27)
								{
									Rect rect11 = rect2;
									int num11 = (int)(totalRect.size[i] / rect2.size[i]);
									bool flag28 = num11 >= 0;
									if (flag28)
									{
										MeshGenerator.RepeatRectUV repeatRectUV3;
										repeatRectUV3.rect = rect11;
										repeatRectUV3.uv = rect;
										this.m_RepeatRectUVList[i].Add(repeatRectUV3);
										num9 = rect2.size[i];
									}
									bool flag29 = num11 >= 2;
									if (flag29)
									{
										Vector2 position2 = rect11.position;
										position2[i] = totalRect.size[i] - rect2.size[i];
										rect11.position = position2;
										MeshGenerator.RepeatRectUV repeatRectUV4;
										repeatRectUV4.rect = rect11;
										repeatRectUV4.uv = rect;
										this.m_RepeatRectUVList[i].Add(repeatRectUV4);
										num9 = totalRect.size[i];
									}
									bool flag30 = num11 > 2;
									if (flag30)
									{
										float num12 = (totalRect.size[i] - rect2.size[i] * (float)num11) / (float)(num11 - 1);
										for (int k = 0; k < num11 - 2; k++)
										{
											Vector2 position3 = rect11.position;
											position3[i] = (rect2.size[i] + num12) * (float)(1 + k);
											rect11.position = position3;
											MeshGenerator.RepeatRectUV repeatRectUV5;
											repeatRectUV5.rect = rect11;
											repeatRectUV5.uv = rect;
											this.m_RepeatRectUVList[i].Add(repeatRectUV5);
										}
									}
								}
								else
								{
									bool flag31 = repeat == Repeat.Round;
									if (flag31)
									{
										int num13 = (int)((totalRect.size[i] + rect2.size[i] * 0.5f) / rect2.size[i]);
										num13 = Math.Max(num13, 1);
										float num14 = totalRect.size[i] / (float)num13;
										bool flag32 = backgroundPosition.keyword == BackgroundPositionKeyword.Center;
										if (flag32)
										{
											bool flag33 = (num13 & 1) == 1;
											if (flag33)
											{
												num13 += 2;
											}
											else
											{
												num13++;
											}
										}
										else
										{
											num13++;
										}
										Rect rect12 = rect2;
										Vector2 size = rect12.size;
										size[i] = num14;
										rect12.size = size;
										rect2 = rect12;
										for (int l = 0; l < num13; l++)
										{
											Vector2 position4 = rect12.position;
											position4[i] = num14 * (float)l;
											rect12.position = position4;
											MeshGenerator.RepeatRectUV repeatRectUV6;
											repeatRectUV6.rect = rect12;
											repeatRectUV6.uv = rect;
											this.m_RepeatRectUVList[i].Add(repeatRectUV6);
											num9 += repeatRectUV6.rect.size[i];
										}
									}
								}
							}
						}
						float num15 = 0f;
						bool flag34 = false;
						bool flag35 = backgroundPosition.keyword == BackgroundPositionKeyword.Center;
						if (flag35)
						{
							num15 = (totalRect.size[i] - num9) * 0.5f;
							flag34 = true;
						}
						else
						{
							bool flag36 = repeat != Repeat.Space;
							if (flag36)
							{
								bool flag37 = backgroundPosition.offset.unit == LengthUnit.Percent;
								if (flag37)
								{
									num15 = (totalRect.size[i] - rect2.size[i]) * backgroundPosition.offset.value / 100f;
									flag34 = true;
								}
								else
								{
									bool flag38 = backgroundPosition.offset.unit == LengthUnit.Pixel;
									if (flag38)
									{
										num15 = backgroundPosition.offset.value;
									}
								}
								bool flag39 = backgroundPosition.keyword == BackgroundPositionKeyword.Right || backgroundPosition.keyword == BackgroundPositionKeyword.Bottom;
								if (flag39)
								{
									num15 = totalRect.size[i] - num9 - num15;
								}
							}
						}
						bool flag40 = flag34 && rectParams.sprite == null && rectParams.vectorImage == null;
						if (flag40)
						{
							float num16 = rect2.size[i] * scaledPixelsPerPoint;
							bool flag41 = Mathf.Abs(Mathf.Round(num16) - num16) < 0.001f;
							if (flag41)
							{
								num15 = AlignmentUtils.CeilToPixelGrid(num15, scaledPixelsPerPoint, -0.02f);
							}
						}
						bool flag42 = repeat == Repeat.Repeat || repeat == Repeat.Round;
						if (flag42)
						{
							float num17 = rect2.size[i];
							bool flag43 = num17 > 1E-30f;
							if (flag43)
							{
								bool flag44 = num15 < -num17;
								if (flag44)
								{
									int num18 = (int)(-num15 / num17);
									num15 += (float)num18 * num17;
								}
								bool flag45 = num15 > 0f;
								if (flag45)
								{
									int num19 = (int)(num15 / num17);
									num15 -= (float)(1 + num19) * num17;
								}
							}
						}
						for (int m = 0; m < this.m_RepeatRectUVList[i].Count; m++)
						{
							MeshGenerator.RepeatRectUV repeatRectUV7 = this.m_RepeatRectUVList[i][m];
							Vector2 position5 = repeatRectUV7.rect.position;
							ref Vector2 ptr = ref position5;
							int num20 = i;
							ptr[num20] += num15;
							repeatRectUV7.rect.position = position5;
							this.m_RepeatRectUVList[i][m] = repeatRectUV7;
						}
					}
					Rect rect13 = new Rect(rect);
					int num21 = this.m_RepeatRectUVList[1].Count * this.m_RepeatRectUVList[0].Count;
					bool flag46 = num21 > 1;
					if (flag46)
					{
						bool flag47 = rectParams.vectorImage == null;
						if (flag47)
						{
							bool flag48 = this.m_BackgroundRepeatInstanceList == null;
							if (flag48)
							{
								this.m_BackgroundRepeatInstanceList = new NativePagedList<MeshGenerator.BackgroundRepeatInstance>(8, "Renderer.MeshGenerator", Allocator.Persistent, Allocator.TempJob);
							}
							rectParams.backgroundRepeatInstanceList = this.m_BackgroundRepeatInstanceList;
							rectParams.backgroundRepeatInstanceListStartIndex = this.m_BackgroundRepeatInstanceList.GetCount();
						}
					}
					int num22 = 0;
					foreach (MeshGenerator.RepeatRectUV repeatRectUV8 in this.m_RepeatRectUVList[1])
					{
						rect2.y = repeatRectUV8.rect.y;
						rect2.height = repeatRectUV8.rect.height;
						rect.y = repeatRectUV8.uv.y;
						rect.height = repeatRectUV8.uv.height;
						bool flag49 = rect2.y < totalRect.y;
						if (flag49)
						{
							float num23 = totalRect.y - rect2.y;
							float num24 = rect2.height - num23;
							float num25 = num23 + num24;
							float num26 = rect13.height * num24 / num25;
							float num27 = rect13.height * num23 / num25;
							rect.y = num27 + rect13.y;
							rect.height = num26;
							rect2.y = totalRect.y;
							rect2.height = num24;
						}
						bool flag50 = rect2.yMax > totalRect.yMax;
						if (flag50)
						{
							float num28 = rect2.yMax - totalRect.yMax;
							float num29 = rect2.height - num28;
							float num30 = num29 + num28;
							float num31 = rect.height * num29 / num30;
							rect.height = num31;
							rect.y = rect.yMax - num31;
							rect2.height = num29;
						}
						bool flag51 = rectParams.vectorImage == null;
						if (flag51)
						{
							float num32 = rect.y - rect13.y;
							float num33 = rect13.yMax - rect.yMax;
							rect.y += num33 - num32;
						}
						foreach (MeshGenerator.RepeatRectUV repeatRectUV9 in this.m_RepeatRectUVList[0])
						{
							rect2.x = repeatRectUV9.rect.x;
							rect2.width = repeatRectUV9.rect.width;
							rect.x = repeatRectUV9.uv.x;
							rect.width = repeatRectUV9.uv.width;
							bool flag52 = rect2.x < totalRect.x;
							if (flag52)
							{
								float num34 = totalRect.x - rect2.x;
								float num35 = rect2.width - num34;
								float num36 = num34 + num35;
								float num37 = rect.width * num35 / num36;
								float num38 = rect13.x + rect13.width * num34 / num36;
								rect.x = num38;
								rect.width = num37;
								rect2.x = totalRect.x;
								rect2.width = num35;
							}
							bool flag53 = rect2.xMax > totalRect.xMax;
							if (flag53)
							{
								float num39 = rect2.xMax - totalRect.xMax;
								float num40 = rect2.width - num39;
								float num41 = num40 + num39;
								float num42 = rect.width * num40 / num41;
								rect.width = num42;
								rect2.width = num40;
							}
							this.StampRectangleWithSubRect(rectParams, rect2, totalRect, rect, ref rectParams.backgroundRepeatInstanceList);
							num22++;
							bool flag54 = rectParams.backgroundRepeatInstanceList != null;
							if (flag54)
							{
								bool flag55 = num22 > 60;
								if (flag55)
								{
									num22 = 0;
									rectParams.backgroundRepeatInstanceListEndIndex = this.m_BackgroundRepeatInstanceList.GetCount();
									this.DrawRectangle(rectParams);
									rectParams.backgroundRepeatInstanceListStartIndex = rectParams.backgroundRepeatInstanceListEndIndex;
								}
							}
						}
					}
					bool flag56 = rectParams.backgroundRepeatInstanceList != null && num22 > 0;
					if (flag56)
					{
						rectParams.backgroundRepeatInstanceListEndIndex = this.m_BackgroundRepeatInstanceList.GetCount();
						this.DrawRectangle(rectParams);
					}
				}
			}
		}

		private void StampRectangleWithSubRect(MeshGenerator.RectangleParams rectParams, Rect targetRect, Rect totalRect, Rect targetUV, ref NativePagedList<MeshGenerator.BackgroundRepeatInstance> backgroundRepeatInstanceList)
		{
			bool flag = targetRect.width < 0.001f || targetRect.height < 0.001f;
			if (!flag)
			{
				Rect rect = targetRect;
				rect.size /= targetUV.size;
				rect.position -= new Vector2(targetUV.position.x, 1f - targetUV.position.y - targetUV.size.y) * rect.size;
				Rect subRect = rectParams.subRect;
				subRect.position *= rect.size;
				subRect.position += rect.position;
				subRect.size *= rect.size;
				bool flag2 = rectParams.HasSlices(0.001f);
				if (flag2)
				{
					rectParams.backgroundRepeatRect = Rect.zero;
					rectParams.rect = targetRect;
				}
				else
				{
					Rect rect2 = MeshGenerator.RectangleParams.RectIntersection(subRect, targetRect);
					bool flag3 = rect2.size.x < 0.001f || rect2.size.y < 0.001f;
					if (flag3)
					{
						return;
					}
					bool flag4 = rect2.size != subRect.size;
					if (flag4)
					{
						Vector2 vector = rect2.size / subRect.size;
						Vector2 vector2 = rectParams.uv.size * vector;
						Vector2 vector3 = rectParams.uv.size - vector2;
						bool flag5 = rect2.x > subRect.x;
						if (flag5)
						{
							float num = (subRect.xMax - rect2.xMax) / subRect.width * rectParams.uv.size.x;
							rectParams.uv.x = rectParams.uv.x + (vector3.x - num);
						}
						bool flag6 = rect2.yMax < subRect.yMax;
						if (flag6)
						{
							float num2 = (rect2.y - subRect.y) / subRect.height * rectParams.uv.size.y;
							rectParams.uv.y = rectParams.uv.y + (vector3.y - num2);
						}
						rectParams.uv.size = vector2;
					}
					bool flag7 = rectParams.vectorImage != null;
					if (flag7)
					{
						rectParams.backgroundRepeatRect = Rect.zero;
						rectParams.rect = rect2;
					}
					else
					{
						bool flag8 = totalRect == rect2;
						if (flag8)
						{
							rectParams.backgroundRepeatRect = Rect.zero;
						}
						else
						{
							rectParams.backgroundRepeatRect = rect2;
						}
						rectParams.rect = totalRect;
					}
				}
				bool flag9 = rectParams.vectorImage == null && backgroundRepeatInstanceList != null;
				if (flag9)
				{
					MeshGenerator.BackgroundRepeatInstance backgroundRepeatInstance;
					backgroundRepeatInstance.rect = rectParams.rect;
					backgroundRepeatInstance.backgroundRepeatRect = rectParams.backgroundRepeatRect;
					backgroundRepeatInstance.uv = rectParams.uv;
					backgroundRepeatInstanceList.Add(backgroundRepeatInstance);
				}
				else
				{
					this.DrawRectangle(rectParams);
				}
			}
		}

		private static void AdjustSpriteWinding(Vector2[] vertices, ushort[] indices, NativeSlice<ushort> newIndices)
		{
			for (int i = 0; i < indices.Length; i += 3)
			{
				Vector3 vector = vertices[(int)indices[i]];
				Vector3 vector2 = vertices[(int)indices[i + 1]];
				Vector3 vector3 = vertices[(int)indices[i + 2]];
				Vector3 normalized = (vector2 - vector).normalized;
				Vector3 normalized2 = (vector3 - vector).normalized;
				Vector3 vector4 = Vector3.Cross(normalized, normalized2);
				bool flag = vector4.z >= 0f;
				if (flag)
				{
					newIndices[i] = indices[i + 1];
					newIndices[i + 1] = indices[i];
					newIndices[i + 2] = indices[i + 2];
				}
				else
				{
					newIndices[i] = indices[i];
					newIndices[i + 1] = indices[i + 1];
					newIndices[i + 2] = indices[i + 2];
				}
			}
		}

		public void ScheduleJobs(MeshGenerationContext mgc)
		{
			int count = this.m_TesselationJobParameters.Count;
			bool flag = count == 0;
			if (!flag)
			{
				bool flag2 = this.m_JobParameters.Length < count;
				if (flag2)
				{
					this.m_JobParameters.Dispose();
					this.m_JobParameters = new NativeArray<MeshGenerator.TessellationJobParameters>(count, MeshGenerator.k_MemoryLabel, NativeArrayOptions.UninitializedMemory);
				}
				for (int i = 0; i < count; i++)
				{
					this.m_JobParameters[i] = this.m_TesselationJobParameters[i];
				}
				this.m_TesselationJobParameters.Clear();
				MeshGenerator.TessellationJob tessellationJob = new MeshGenerator.TessellationJob
				{
					jobParameters = this.m_JobParameters.Slice<MeshGenerator.TessellationJobParameters>(0, count)
				};
				mgc.GetTempMeshAllocator(out tessellationJob.allocator);
				JobHandle jobHandle = tessellationJob.Schedule(count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_OnMeshGenerationDelegate, null, MeshGenerationCallbackType.Work, true);
			}
		}

		private void OnMeshGeneration(MeshGenerationContext ctx, object data)
		{
			bool flag = this.m_BackgroundRepeatInstanceList != null;
			if (flag)
			{
				this.m_BackgroundRepeatInstanceList.Reset();
			}
			this.m_GCHandlePool.ReturnAll();
		}

		internal bool disposed { get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					bool flag = this.m_BackgroundRepeatInstanceList != null;
					if (flag)
					{
						this.m_BackgroundRepeatInstanceList.Dispose();
					}
					this.m_GCHandlePool.Dispose();
					this.m_JobParameters.Dispose();
				}
				this.disposed = true;
			}
		}

		private const string k_MemoryLabelName = "Renderer.MeshGenerator";

		private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.MeshGenerator", Allocator.Persistent);

		private static readonly ProfilerMarker k_MarkerDrawRectangle = new ProfilerMarker("MeshGenerator.DrawRectangle");

		private static readonly ProfilerMarker k_MarkerDrawBorder = new ProfilerMarker("MeshGenerator.DrawBorder");

		private static readonly ProfilerMarker k_MarkerDrawVectorImage = new ProfilerMarker("MeshGenerator.DrawVectorImage");

		private static readonly ProfilerMarker k_MarkerDrawRectangleRepeat = new ProfilerMarker("MeshGenerator.DrawRectangleRepeat");

		private MeshGenerationContext m_MeshGenerationContext;

		private List<MeshGenerator.RepeatRectUV>[] m_RepeatRectUVList = null;

		private NativePagedList<MeshGenerator.BackgroundRepeatInstance> m_BackgroundRepeatInstanceList = null;

		private GCHandlePool m_GCHandlePool = new GCHandlePool(256, 64);

		private NativeArray<MeshGenerator.TessellationJobParameters> m_JobParameters;

		private TextInfo m_TextInfo = new TextInfo();

		private TextGenerationSettings m_Settings = new TextGenerationSettings
		{
			screenRect = Rect.zero,
			richText = true
		};

		private List<NativeSlice<Vertex>> m_VerticesArray = new List<NativeSlice<Vertex>>();

		private List<NativeSlice<ushort>> m_IndicesArray = new List<NativeSlice<ushort>>();

		private List<Texture2D> m_Atlases = new List<Texture2D>();

		private List<float> m_SdfScales = new List<float>();

		private List<GlyphRenderMode> m_RenderModes = new List<GlyphRenderMode>();

		private MeshGenerationCallback m_OnMeshGenerationDelegate;

		private List<MeshGenerator.TessellationJobParameters> m_TesselationJobParameters = new List<MeshGenerator.TessellationJobParameters>(256);

		private struct RepeatRectUV
		{
			public Rect rect;

			public Rect uv;
		}

		public struct BackgroundRepeatInstance
		{
			public Rect rect;

			public Rect backgroundRepeatRect;

			public Rect uv;
		}

		public struct BorderParams
		{
			internal void ToNativeParams(out MeshBuilderNative.NativeBorderParams nativeBorderParams)
			{
				nativeBorderParams = new MeshBuilderNative.NativeBorderParams
				{
					rect = this.rect,
					leftColor = this.leftColor,
					topColor = this.topColor,
					rightColor = this.rightColor,
					bottomColor = this.bottomColor,
					leftWidth = this.leftWidth,
					topWidth = this.topWidth,
					rightWidth = this.rightWidth,
					bottomWidth = this.bottomWidth,
					topLeftRadius = this.topLeftRadius,
					topRightRadius = this.topRightRadius,
					bottomRightRadius = this.bottomRightRadius,
					bottomLeftRadius = this.bottomLeftRadius,
					leftColorPage = this.leftColorPage.ToNativeColorPage(),
					topColorPage = this.topColorPage.ToNativeColorPage(),
					rightColorPage = this.rightColorPage.ToNativeColorPage(),
					bottomColorPage = this.bottomColorPage.ToNativeColorPage()
				};
			}

			public Rect rect;

			public Color playmodeTintColor;

			public Color leftColor;

			public Color topColor;

			public Color rightColor;

			public Color bottomColor;

			public float leftWidth;

			public float topWidth;

			public float rightWidth;

			public float bottomWidth;

			public Vector2 topLeftRadius;

			public Vector2 topRightRadius;

			public Vector2 bottomRightRadius;

			public Vector2 bottomLeftRadius;

			internal ColorPage leftColorPage;

			internal ColorPage topColorPage;

			internal ColorPage rightColorPage;

			internal ColorPage bottomColorPage;
		}

		public struct RectangleParams
		{
			public static MeshGenerator.RectangleParams MakeSolid(Rect rect, Color color, Color playModeTintColor)
			{
				return new MeshGenerator.RectangleParams
				{
					rect = rect,
					color = color,
					uv = new Rect(0f, 0f, 1f, 1f),
					playmodeTintColor = playModeTintColor
				};
			}

			private static void AdjustUVsForScaleMode(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
			{
				float num = Mathf.Abs((float)texture.width * uv.width / ((float)texture.height * uv.height));
				float num2 = rect.width / rect.height;
				switch (scaleMode)
				{
				case ScaleMode.StretchToFill:
					break;
				case ScaleMode.ScaleAndCrop:
				{
					bool flag = num2 > num;
					if (flag)
					{
						float num3 = uv.height * (num / num2);
						float num4 = (uv.height - num3) * 0.5f;
						uv = new Rect(uv.x, uv.y + num4, uv.width, num3);
					}
					else
					{
						float num5 = uv.width * (num2 / num);
						float num6 = (uv.width - num5) * 0.5f;
						uv = new Rect(uv.x + num6, uv.y, num5, uv.height);
					}
					break;
				}
				case ScaleMode.ScaleToFit:
				{
					bool flag2 = num2 > num;
					if (flag2)
					{
						float num7 = num / num2;
						rect = new Rect(rect.xMin + rect.width * (1f - num7) * 0.5f, rect.yMin, num7 * rect.width, rect.height);
					}
					else
					{
						float num8 = num2 / num;
						rect = new Rect(rect.xMin, rect.yMin + rect.height * (1f - num8) * 0.5f, rect.width, num8 * rect.height);
					}
					break;
				}
				default:
					throw new NotImplementedException();
				}
				rectOut = rect;
				uvOut = uv;
			}

			private static void AdjustSpriteUVsForScaleMode(Rect containerRect, Rect srcRect, Rect spriteGeomRect, Sprite sprite, ScaleMode scaleMode, out Rect rectOut, out Rect uvOut)
			{
				float num = sprite.rect.width / sprite.rect.height;
				float num2 = containerRect.width / containerRect.height;
				Rect rect = spriteGeomRect;
				rect.position -= sprite.bounds.min;
				rect.position /= sprite.bounds.size;
				rect.size /= sprite.bounds.size;
				Vector2 position = rect.position;
				position.y = 1f - rect.size.y - position.y;
				rect.position = position;
				switch (scaleMode)
				{
				case ScaleMode.StretchToFill:
				{
					Vector2 size = containerRect.size;
					containerRect.position = rect.position * size;
					containerRect.size = rect.size * size;
					break;
				}
				case ScaleMode.ScaleAndCrop:
				{
					Rect rect2 = containerRect;
					bool flag = num2 > num;
					if (flag)
					{
						rect2.height = rect2.width / num;
						rect2.position = new Vector2(rect2.position.x, -(rect2.height - containerRect.height) / 2f);
					}
					else
					{
						rect2.width = rect2.height * num;
						rect2.position = new Vector2(-(rect2.width - containerRect.width) / 2f, rect2.position.y);
					}
					Vector2 size2 = rect2.size;
					rect2.position += rect.position * size2;
					rect2.size = rect.size * size2;
					Rect rect3 = MeshGenerator.RectangleParams.RectIntersection(containerRect, rect2);
					bool flag2 = rect3.width < 1E-30f || rect3.height < 1E-30f;
					if (flag2)
					{
						rect3 = Rect.zero;
					}
					else
					{
						Rect rect4 = rect3;
						rect4.position -= rect2.position;
						rect4.position /= rect2.size;
						rect4.size /= rect2.size;
						Vector2 position2 = rect4.position;
						position2.y = 1f - rect4.size.y - position2.y;
						rect4.position = position2;
						srcRect.position += rect4.position * srcRect.size;
						srcRect.size *= rect4.size;
					}
					containerRect = rect3;
					break;
				}
				case ScaleMode.ScaleToFit:
				{
					bool flag3 = num2 > num;
					if (flag3)
					{
						float num3 = num / num2;
						containerRect = new Rect(containerRect.xMin + containerRect.width * (1f - num3) * 0.5f, containerRect.yMin, num3 * containerRect.width, containerRect.height);
					}
					else
					{
						float num4 = num2 / num;
						containerRect = new Rect(containerRect.xMin, containerRect.yMin + containerRect.height * (1f - num4) * 0.5f, containerRect.width, num4 * containerRect.height);
					}
					containerRect.position += rect.position * containerRect.size;
					containerRect.size *= rect.size;
					break;
				}
				default:
					throw new NotImplementedException();
				}
				rectOut = containerRect;
				uvOut = srcRect;
			}

			internal static Rect RectIntersection(Rect a, Rect b)
			{
				Rect zero = Rect.zero;
				zero.min = Vector2.Max(a.min, b.min);
				zero.max = Vector2.Min(a.max, b.max);
				zero.size = Vector2.Max(zero.size, Vector2.zero);
				return zero;
			}

			private static Rect ComputeGeomRect(Sprite sprite)
			{
				Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
				foreach (Vector2 vector3 in sprite.vertices)
				{
					vector = Vector2.Min(vector, vector3);
					vector2 = Vector2.Max(vector2, vector3);
				}
				return new Rect(vector, vector2 - vector);
			}

			private static Rect ComputeUVRect(Sprite sprite)
			{
				Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
				foreach (Vector2 vector3 in sprite.uv)
				{
					vector = Vector2.Min(vector, vector3);
					vector2 = Vector2.Max(vector2, vector3);
				}
				return new Rect(vector, vector2 - vector);
			}

			private static Rect ApplyPackingRotation(Rect uv, SpritePackingRotation rotation)
			{
				switch (rotation)
				{
				case SpritePackingRotation.FlipHorizontal:
				{
					uv.position += new Vector2(uv.size.x, 0f);
					Vector2 size = uv.size;
					size.x = -size.x;
					uv.size = size;
					break;
				}
				case SpritePackingRotation.FlipVertical:
				{
					uv.position += new Vector2(0f, uv.size.y);
					Vector2 size2 = uv.size;
					size2.y = -size2.y;
					uv.size = size2;
					break;
				}
				case SpritePackingRotation.Rotate180:
					uv.position += uv.size;
					uv.size = -uv.size;
					break;
				}
				return uv;
			}

			public static MeshGenerator.RectangleParams MakeTextured(Rect rect, Rect uv, Texture texture, ScaleMode scaleMode, Color playModeTintColor)
			{
				MeshGenerator.RectangleParams.AdjustUVsForScaleMode(rect, uv, texture, scaleMode, out rect, out uv);
				Vector2 vector = new Vector2((float)texture.width, (float)texture.height);
				return new MeshGenerator.RectangleParams
				{
					rect = rect,
					subRect = new Rect(0f, 0f, 1f, 1f),
					uv = uv,
					color = Color.white,
					texture = texture,
					contentSize = vector,
					textureSize = vector,
					scaleMode = scaleMode,
					playmodeTintColor = playModeTintColor
				};
			}

			public static MeshGenerator.RectangleParams MakeSprite(Rect containerRect, Rect subRect, Sprite sprite, ScaleMode scaleMode, Color playModeTintColor, bool hasRadius, ref Vector4 slices, bool useForRepeat = false)
			{
				bool flag = sprite == null || sprite.bounds.size.x < 1E-30f || sprite.bounds.size.y < 1E-30f;
				MeshGenerator.RectangleParams rectangleParams2;
				if (flag)
				{
					MeshGenerator.RectangleParams rectangleParams = default(MeshGenerator.RectangleParams);
					rectangleParams2 = rectangleParams;
				}
				else
				{
					bool flag2 = sprite.texture == null;
					if (flag2)
					{
						Debug.LogWarning("Ignoring textureless sprite named \"" + sprite.name + "\", please import as a VectorImage instead");
						MeshGenerator.RectangleParams rectangleParams = default(MeshGenerator.RectangleParams);
						rectangleParams2 = rectangleParams;
					}
					else
					{
						Rect rect = MeshGenerator.RectangleParams.ComputeGeomRect(sprite);
						Rect rect2 = MeshGenerator.RectangleParams.ComputeUVRect(sprite);
						Vector4 border = sprite.border;
						bool flag3 = border != Vector4.zero || slices != Vector4.zero;
						bool flag4 = subRect != new Rect(0f, 0f, 1f, 1f);
						bool flag5 = scaleMode == ScaleMode.ScaleAndCrop || flag3 || hasRadius || useForRepeat || flag4;
						bool flag6 = flag5 && sprite.packed && sprite.packingRotation > SpritePackingRotation.None;
						if (flag6)
						{
							rect2 = MeshGenerator.RectangleParams.ApplyPackingRotation(rect2, sprite.packingRotation);
						}
						bool flag7 = flag4;
						Rect rect3;
						if (flag7)
						{
							rect3 = subRect;
							rect3.position *= rect2.size;
							rect3.position += rect2.position;
							rect3.size *= rect2.size;
						}
						else
						{
							rect3 = rect2;
						}
						Rect rect4;
						Rect rect5;
						MeshGenerator.RectangleParams.AdjustSpriteUVsForScaleMode(containerRect, rect3, rect, sprite, scaleMode, out rect4, out rect5);
						Rect rect6 = rect;
						rect6.size /= sprite.bounds.size;
						rect6.position -= sprite.bounds.min;
						rect6.position /= sprite.bounds.size;
						rect6.position = new Vector2(rect6.position.x, 1f - (rect6.position.y + rect6.height));
						MeshGenerator.RectangleParams rectangleParams = new MeshGenerator.RectangleParams
						{
							rect = rect4,
							uv = rect5,
							subRect = rect6,
							color = Color.white,
							texture = (flag5 ? sprite.texture : null),
							sprite = (flag5 ? null : sprite),
							contentSize = sprite.rect.size,
							textureSize = new Vector2((float)sprite.texture.width, (float)sprite.texture.height),
							spriteGeomRect = rect,
							scaleMode = scaleMode,
							playmodeTintColor = playModeTintColor,
							meshFlags = (sprite.packed ? MeshGenerationContext.MeshFlags.SkipDynamicAtlas : MeshGenerationContext.MeshFlags.None)
						};
						MeshGenerator.RectangleParams rectangleParams3 = rectangleParams;
						Vector4 vector = new Vector4(border.x, border.w, border.z, border.y);
						bool flag8 = slices != Vector4.zero && vector != Vector4.zero && vector != slices;
						if (flag8)
						{
							Debug.LogWarning(string.Format("Sprite \"{0}\" borders {1} are overridden by style slices {2}", sprite.name, vector, slices));
						}
						else
						{
							bool flag9 = slices == Vector4.zero;
							if (flag9)
							{
								slices = vector;
							}
						}
						rectangleParams2 = rectangleParams3;
					}
				}
				return rectangleParams2;
			}

			public static MeshGenerator.RectangleParams MakeVectorTextured(Rect rect, Rect uv, VectorImage vectorImage, ScaleMode scaleMode, Color playModeTintColor)
			{
				return new MeshGenerator.RectangleParams
				{
					rect = rect,
					subRect = new Rect(0f, 0f, 1f, 1f),
					uv = uv,
					color = Color.white,
					vectorImage = vectorImage,
					contentSize = new Vector2(vectorImage.width, vectorImage.height),
					scaleMode = scaleMode,
					playmodeTintColor = playModeTintColor
				};
			}

			internal bool HasRadius(float epsilon)
			{
				return (this.topLeftRadius.x > epsilon && this.topLeftRadius.y > epsilon) || (this.topRightRadius.x > epsilon && this.topRightRadius.y > epsilon) || (this.bottomRightRadius.x > epsilon && this.bottomRightRadius.y > epsilon) || (this.bottomLeftRadius.x > epsilon && this.bottomLeftRadius.y > epsilon);
			}

			internal bool HasSlices(float epsilon)
			{
				return (float)this.leftSlice > epsilon || (float)this.topSlice > epsilon || (float)this.rightSlice > epsilon || (float)this.bottomSlice > epsilon;
			}

			internal void ToNativeParams(out MeshBuilderNative.NativeRectParams nativeRectParams)
			{
				nativeRectParams = new MeshBuilderNative.NativeRectParams
				{
					rect = this.rect,
					subRect = this.subRect,
					backgroundRepeatRect = this.backgroundRepeatRect,
					uv = this.uv,
					color = this.color,
					scaleMode = this.scaleMode,
					topLeftRadius = this.topLeftRadius,
					topRightRadius = this.topRightRadius,
					bottomRightRadius = this.bottomRightRadius,
					bottomLeftRadius = this.bottomLeftRadius,
					spriteGeomRect = this.spriteGeomRect,
					contentSize = this.contentSize,
					textureSize = this.textureSize,
					texturePixelsPerPoint = 1f,
					leftSlice = this.leftSlice,
					topSlice = this.topSlice,
					rightSlice = this.rightSlice,
					bottomSlice = this.bottomSlice,
					sliceScale = this.sliceScale,
					rectInset = this.rectInset,
					colorPage = this.colorPage.ToNativeColorPage(),
					meshFlags = (int)this.meshFlags
				};
			}

			public Rect rect;

			public Rect uv;

			public Color color;

			public Rect subRect;

			public Rect backgroundRepeatRect;

			public NativePagedList<MeshGenerator.BackgroundRepeatInstance> backgroundRepeatInstanceList;

			public int backgroundRepeatInstanceListStartIndex;

			public int backgroundRepeatInstanceListEndIndex;

			public BackgroundPosition backgroundPositionX;

			public BackgroundPosition backgroundPositionY;

			public BackgroundRepeat backgroundRepeat;

			public BackgroundSize backgroundSize;

			public Texture texture;

			public Sprite sprite;

			public VectorImage vectorImage;

			public ScaleMode scaleMode;

			public Color playmodeTintColor;

			public Vector2 topLeftRadius;

			public Vector2 topRightRadius;

			public Vector2 bottomRightRadius;

			public Vector2 bottomLeftRadius;

			public Vector2 contentSize;

			public Vector2 textureSize;

			public int leftSlice;

			public int topSlice;

			public int rightSlice;

			public int bottomSlice;

			public float sliceScale;

			internal Rect spriteGeomRect;

			public Vector4 rectInset;

			internal ColorPage colorPage;

			internal MeshGenerationContext.MeshFlags meshFlags;
		}

		private struct TessellationJobParameters
		{
			public bool isBorderJob;

			public MeshBuilderNative.NativeRectParams rectParams;

			public MeshGenerator.BorderParams borderParams;

			public UnsafeMeshGenerationNode node;
		}

		private struct TessellationJob : IJobParallelFor
		{
			public void Execute(int i)
			{
				MeshGenerator.TessellationJobParameters tessellationJobParameters = this.jobParameters[i];
				bool isBorderJob = tessellationJobParameters.isBorderJob;
				if (isBorderJob)
				{
					this.DrawBorder(tessellationJobParameters.node, ref tessellationJobParameters.borderParams);
				}
				else
				{
					ref MeshBuilderNative.NativeRectParams ptr = ref tessellationJobParameters.rectParams;
					bool flag = ptr.vectorImage != IntPtr.Zero;
					if (flag)
					{
						this.DrawVectorImage(tessellationJobParameters.node, ref ptr, this.ExtractHandle<VectorImage>(ptr.vectorImage));
					}
					else
					{
						bool flag2 = ptr.sprite != IntPtr.Zero;
						if (flag2)
						{
							this.DrawSprite(tessellationJobParameters.node, ref ptr, this.ExtractHandle<Sprite>(ptr.sprite));
						}
						else
						{
							this.DrawRectangle(tessellationJobParameters.node, ref ptr, this.ExtractHandle<Texture>(ptr.texture));
						}
					}
				}
			}

			private T ExtractHandle<T>(IntPtr handlePtr) where T : class
			{
				GCHandle gchandle = ((handlePtr != IntPtr.Zero) ? GCHandle.FromIntPtr(handlePtr) : default(GCHandle));
				return gchandle.IsAllocated ? (gchandle.Target as T) : default(T);
			}

			private unsafe void DrawBorder(UnsafeMeshGenerationNode node, ref MeshGenerator.BorderParams borderParams)
			{
				MeshBuilderNative.NativeBorderParams nativeBorderParams;
				borderParams.ToNativeParams(out nativeBorderParams);
				MeshWriteDataInterface meshWriteDataInterface = MeshBuilderNative.MakeBorder(ref nativeBorderParams);
				bool flag = meshWriteDataInterface.vertexCount == 0 || meshWriteDataInterface.indexCount == 0;
				if (!flag)
				{
					NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
					NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
					bool flag2 = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
					if (!flag2)
					{
						NativeSlice<Vertex> nativeSlice3;
						NativeSlice<ushort> nativeSlice4;
						this.allocator.AllocateTempMesh(nativeSlice.Length, nativeSlice2.Length, out nativeSlice3, out nativeSlice4);
						Debug.Assert(nativeSlice3.Length == nativeSlice.Length);
						Debug.Assert(nativeSlice4.Length == nativeSlice2.Length);
						nativeSlice3.CopyFrom(nativeSlice);
						nativeSlice4.CopyFrom(nativeSlice2);
						node.DrawMesh(nativeSlice3, nativeSlice4, null);
					}
				}
			}

			private unsafe void DrawRectangle(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, Texture tex)
			{
				bool flag = rectParams.backgroundRepeatInstanceList != IntPtr.Zero;
				if (flag)
				{
					NativePagedList<MeshGenerator.BackgroundRepeatInstance> nativePagedList = ((GCHandle)rectParams.backgroundRepeatInstanceList).Target as NativePagedList<MeshGenerator.BackgroundRepeatInstance>;
					int num = rectParams.backgroundRepeatInstanceListEndIndex - rectParams.backgroundRepeatInstanceListStartIndex;
					int num2 = Math.Min(4 * num, (int)UIRenderDevice.maxVerticesPerPage);
					int num3 = Math.Min(6 * num, (int)(UIRenderDevice.maxVerticesPerPage * 3U));
					int num4 = num2;
					int num5 = num3;
					NativeSlice<Vertex> nativeSlice;
					NativeSlice<ushort> nativeSlice2;
					this.allocator.AllocateTempMesh(num4, num5, out nativeSlice, out nativeSlice2);
					NativePagedList<MeshGenerator.BackgroundRepeatInstance>.Enumerator enumerator = new NativePagedList<MeshGenerator.BackgroundRepeatInstance>.Enumerator(nativePagedList, rectParams.backgroundRepeatInstanceListStartIndex);
					for (int i = 0; i < num; i++)
					{
						Debug.Assert(enumerator.HasNext());
						MeshGenerator.BackgroundRepeatInstance next = enumerator.GetNext();
						rectParams.rect = next.rect;
						rectParams.backgroundRepeatRect = next.backgroundRepeatRect;
						rectParams.uv = next.uv;
						bool flag2 = rectParams.texture != IntPtr.Zero;
						MeshWriteDataInterface meshWriteDataInterface;
						if (flag2)
						{
							meshWriteDataInterface = MeshBuilderNative.MakeTexturedRect(ref rectParams);
						}
						else
						{
							meshWriteDataInterface = MeshBuilderNative.MakeSolidRect(ref rectParams);
						}
						bool flag3 = meshWriteDataInterface.vertexCount == 0 || meshWriteDataInterface.indexCount == 0;
						if (!flag3)
						{
							NativeSlice<Vertex> nativeSlice3 = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
							NativeSlice<ushort> nativeSlice4 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
							bool flag4 = num2 < meshWriteDataInterface.vertexCount || num3 < meshWriteDataInterface.indexCount;
							if (flag4)
							{
								bool flag5 = nativeSlice.Length - num2 > 0 && nativeSlice2.Length - num3 > 0;
								if (flag5)
								{
									node.DrawMesh(nativeSlice.Slice<Vertex>(0, nativeSlice.Length - num2), nativeSlice2.Slice<ushort>(0, nativeSlice2.Length - num3), tex);
								}
								num4 = Math.Min(Math.Max(meshWriteDataInterface.vertexCount, num4) * 2, (int)UIRenderDevice.maxVerticesPerPage);
								num5 = Math.Min(Math.Max(meshWriteDataInterface.indexCount, num5) * 2, (int)(UIRenderDevice.maxVerticesPerPage * 3U));
								this.allocator.AllocateTempMesh(num4, num5, out nativeSlice, out nativeSlice2);
								num2 = num4;
								num3 = num5;
							}
							int num6 = nativeSlice.Length - num2;
							void* ptr = (void*)((byte*)nativeSlice.GetUnsafePtr<Vertex>() + (IntPtr)num6 * (IntPtr)sizeof(Vertex));
							int num7 = meshWriteDataInterface.vertexCount * sizeof(Vertex);
							UnsafeUtility.MemCpy(ptr, nativeSlice3.GetUnsafePtr<Vertex>(), (long)num7);
							ushort num8 = (ushort)num6;
							num6 = nativeSlice2.Length - num3;
							for (int j = 0; j < meshWriteDataInterface.indexCount; j++)
							{
								nativeSlice2[num6 + j] = nativeSlice4[j] + num8;
							}
							num2 -= meshWriteDataInterface.vertexCount;
							num3 -= meshWriteDataInterface.indexCount;
						}
					}
					bool flag6 = nativeSlice.Length - num2 > 0 && nativeSlice2.Length - num3 > 0;
					if (flag6)
					{
						node.DrawMesh(nativeSlice.Slice<Vertex>(0, nativeSlice.Length - num2), nativeSlice2.Slice<ushort>(0, nativeSlice2.Length - num3), tex);
					}
				}
				else
				{
					bool flag7 = rectParams.texture != IntPtr.Zero;
					MeshWriteDataInterface meshWriteDataInterface2;
					if (flag7)
					{
						meshWriteDataInterface2 = MeshBuilderNative.MakeTexturedRect(ref rectParams);
					}
					else
					{
						meshWriteDataInterface2 = MeshBuilderNative.MakeSolidRect(ref rectParams);
					}
					bool flag8 = meshWriteDataInterface2.vertexCount == 0 || meshWriteDataInterface2.indexCount == 0;
					if (!flag8)
					{
						NativeSlice<Vertex> nativeSlice5 = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface2.vertices, meshWriteDataInterface2.vertexCount);
						NativeSlice<ushort> nativeSlice6 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface2.indices, meshWriteDataInterface2.indexCount);
						bool flag9 = nativeSlice5.Length == 0 || nativeSlice6.Length == 0;
						if (!flag9)
						{
							NativeSlice<Vertex> nativeSlice7;
							NativeSlice<ushort> nativeSlice8;
							this.allocator.AllocateTempMesh(nativeSlice5.Length, nativeSlice6.Length, out nativeSlice7, out nativeSlice8);
							Debug.Assert(nativeSlice7.Length == nativeSlice5.Length);
							Debug.Assert(nativeSlice8.Length == nativeSlice6.Length);
							nativeSlice7.CopyFrom(nativeSlice5);
							nativeSlice8.CopyFrom(nativeSlice6);
							node.DrawMesh(nativeSlice7, nativeSlice8, tex);
						}
					}
				}
			}

			private void DrawSprite(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, Sprite sprite)
			{
				bool flag = rectParams.spriteTexture == IntPtr.Zero;
				if (!flag)
				{
					Texture2D texture2D = this.ExtractHandle<Texture2D>(rectParams.spriteTexture);
					Vector2[] array = this.ExtractHandle<Vector2[]>(rectParams.spriteVertices);
					Vector2[] array2 = this.ExtractHandle<Vector2[]>(rectParams.spriteUVs);
					ushort[] array3 = this.ExtractHandle<ushort[]>(rectParams.spriteTriangles);
					bool flag2 = array3 != null && array3.Length == 0;
					if (!flag2)
					{
						int num = array.Length;
						NativeSlice<Vertex> nativeSlice;
						NativeSlice<ushort> nativeSlice2;
						this.allocator.AllocateTempMesh(num, array3.Length, out nativeSlice, out nativeSlice2);
						MeshGenerator.AdjustSpriteWinding(array, array3, nativeSlice2);
						MeshBuilderNative.NativeColorPage colorPage = rectParams.colorPage;
						Color32 pageAndID = colorPage.pageAndID;
						Color32 color = new Color32(0, 0, 0, (colorPage.isValid != 0) ? 1 : 0);
						Color32 color2 = new Color32(0, 0, colorPage.pageAndID.r, colorPage.pageAndID.g);
						Color32 color3 = new Color32(0, 0, 0, colorPage.pageAndID.b);
						for (int i = 0; i < num; i++)
						{
							Vector2 vector = array[i];
							vector -= rectParams.spriteGeomRect.position;
							vector /= rectParams.spriteGeomRect.size;
							vector.y = 1f - vector.y;
							vector *= rectParams.rect.size;
							vector += rectParams.rect.position;
							nativeSlice[i] = new Vertex
							{
								position = new Vector3(vector.x, vector.y, Vertex.nearZ),
								tint = rectParams.color,
								uv = array2[i],
								flags = color,
								opacityColorPages = color2,
								ids = color3
							};
						}
						MeshGenerationContext.MeshFlags meshFlags = (MeshGenerationContext.MeshFlags)rectParams.meshFlags;
						bool flag3 = meshFlags == MeshGenerationContext.MeshFlags.SkipDynamicAtlas;
						node.DrawMeshInternal(nativeSlice, nativeSlice2, texture2D, flag3 ? TextureOptions.SkipDynamicAtlas : TextureOptions.None);
					}
				}
			}

			private unsafe void DrawVectorImage(UnsafeMeshGenerationNode node, ref MeshBuilderNative.NativeRectParams rectParams, VectorImage vi)
			{
				bool flag = (rectParams.meshFlags & 4) != 0;
				int num = vi.vertices.Length;
				Vertex[] array = new Vertex[num];
				for (int i = 0; i < num; i++)
				{
					VectorImageVertex vectorImageVertex = vi.vertices[i];
					array[i] = new Vertex
					{
						position = vectorImageVertex.position,
						tint = vectorImageVertex.tint,
						uv = vectorImageVertex.uv,
						settingIndex = new Color32((byte)(vectorImageVertex.settingIndex >> 8), (byte)vectorImageVertex.settingIndex, 0, 0),
						flags = vectorImageVertex.flags,
						circle = vectorImageVertex.circle
					};
				}
				bool flag2 = (float)rectParams.leftSlice <= 1E-30f && (float)rectParams.topSlice <= 1E-30f && (float)rectParams.rightSlice <= 1E-30f && (float)rectParams.bottomSlice <= 1E-30f;
				MeshWriteDataInterface meshWriteDataInterface;
				if (flag2)
				{
					meshWriteDataInterface = MeshBuilderNative.MakeVectorGraphicsStretchBackground(array, vi.indices, vi.size.x, vi.size.y, rectParams.rect, rectParams.uv, rectParams.scaleMode, rectParams.color, rectParams.colorPage);
				}
				else
				{
					Vector4 vector = new Vector4((float)rectParams.leftSlice, (float)rectParams.topSlice, (float)rectParams.rightSlice, (float)rectParams.bottomSlice);
					meshWriteDataInterface = MeshBuilderNative.MakeVectorGraphics9SliceBackground(array, vi.indices, vi.size.x, vi.size.y, rectParams.rect, vector, rectParams.color, rectParams.colorPage);
				}
				NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
				NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
				bool flag3 = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
				if (!flag3)
				{
					NativeSlice<Vertex> nativeSlice3;
					NativeSlice<ushort> nativeSlice4;
					this.allocator.AllocateTempMesh(nativeSlice.Length, nativeSlice2.Length, out nativeSlice3, out nativeSlice4);
					Debug.Assert(nativeSlice3.Length == nativeSlice.Length);
					Debug.Assert(nativeSlice4.Length == nativeSlice2.Length);
					nativeSlice3.CopyFrom(nativeSlice);
					nativeSlice4.CopyFrom(nativeSlice2);
					bool flag4 = flag;
					if (flag4)
					{
						node.DrawGradientsInternal(nativeSlice3, nativeSlice4, vi);
					}
					else
					{
						node.DrawMesh(nativeSlice3, nativeSlice4, null);
					}
				}
			}

			[ReadOnly]
			public TempMeshAllocator allocator;

			[ReadOnly]
			public NativeSlice<MeshGenerator.TessellationJobParameters> jobParameters;
		}
	}
}
