using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements.UIR.Implementation
{
	internal class UIRStylePainter : IStylePainter
	{
		private MeshWriteData GetPooledMeshWriteData()
		{
			bool flag = this.m_NextMeshWriteDataPoolItem == this.m_MeshWriteDataPool.Count;
			if (flag)
			{
				this.m_MeshWriteDataPool.Add(new MeshWriteData());
			}
			List<MeshWriteData> meshWriteDataPool = this.m_MeshWriteDataPool;
			int nextMeshWriteDataPoolItem = this.m_NextMeshWriteDataPoolItem;
			this.m_NextMeshWriteDataPoolItem = nextMeshWriteDataPoolItem + 1;
			return meshWriteDataPool[nextMeshWriteDataPoolItem];
		}

		private MeshWriteData AllocRawVertsIndices(uint vertexCount, uint indexCount, ref MeshBuilder.AllocMeshData allocatorData)
		{
			this.m_CurrentEntry.vertices = this.m_VertsPool.Alloc((int)vertexCount);
			this.m_CurrentEntry.indices = this.m_IndicesPool.Alloc((int)indexCount);
			MeshWriteData pooledMeshWriteData = this.GetPooledMeshWriteData();
			pooledMeshWriteData.Reset(this.m_CurrentEntry.vertices, this.m_CurrentEntry.indices);
			return pooledMeshWriteData;
		}

		private MeshWriteData AllocThroughDrawMesh(uint vertexCount, uint indexCount, ref MeshBuilder.AllocMeshData allocatorData)
		{
			return this.DrawMesh((int)vertexCount, (int)indexCount, allocatorData.texture, allocatorData.material, allocatorData.flags);
		}

		public UIRStylePainter(RenderChain renderChain)
		{
			this.m_Owner = renderChain;
			this.meshGenerationContext = new MeshGenerationContext(this);
			this.m_Atlas = renderChain.atlas;
			this.m_VectorImageManager = renderChain.vectorImageManager;
			this.m_AllocRawVertsIndicesDelegate = new MeshBuilder.AllocMeshData.Allocator(this.AllocRawVertsIndices);
			this.m_AllocThroughDrawMeshDelegate = new MeshBuilder.AllocMeshData.Allocator(this.AllocThroughDrawMesh);
			int num = 32;
			this.m_MeshWriteDataPool = new List<MeshWriteData>(num);
			for (int i = 0; i < num; i++)
			{
				this.m_MeshWriteDataPool.Add(new MeshWriteData());
			}
			this.m_VertsPool = renderChain.vertsPool;
			this.m_IndicesPool = renderChain.indicesPool;
		}

		public MeshGenerationContext meshGenerationContext { get; }

		public VisualElement currentElement { get; private set; }

		public List<UIRStylePainter.Entry> entries
		{
			get
			{
				return this.m_Entries;
			}
		}

		public UIRStylePainter.ClosingInfo closingInfo
		{
			get
			{
				return this.m_ClosingInfo;
			}
		}

		public int totalVertices { get; private set; }

		public int totalIndices { get; private set; }

		public void Begin(VisualElement ve)
		{
			this.currentElement = ve;
			this.m_NextMeshWriteDataPoolItem = 0;
			this.m_SVGBackgroundEntryIndex = -1;
			this.currentElement.renderChainData.displacementUVStart = (this.currentElement.renderChainData.displacementUVEnd = 0);
			this.m_MaskDepth = 0;
			this.m_StencilRef = 0;
			VisualElement parent = this.currentElement.hierarchy.parent;
			bool flag = parent != null;
			if (flag)
			{
				this.m_MaskDepth = parent.renderChainData.childrenMaskDepth;
				this.m_StencilRef = parent.renderChainData.childrenStencilRef;
			}
			bool flag2 = (this.currentElement.renderHints & RenderHints.GroupTransform) > RenderHints.None;
			bool flag3 = flag2;
			if (flag3)
			{
				RenderChainCommand renderChainCommand = this.m_Owner.AllocCommand();
				renderChainCommand.owner = this.currentElement;
				renderChainCommand.type = CommandType.PushView;
				this.m_Entries.Add(new UIRStylePainter.Entry
				{
					customCommand = renderChainCommand
				});
				this.m_ClosingInfo.needsClosing = (this.m_ClosingInfo.popViewMatrix = true);
			}
			bool flag4 = parent != null;
			if (flag4)
			{
				this.m_ClipRectID = (flag2 ? UIRVEShaderInfoAllocator.infiniteClipRect : parent.renderChainData.clipRectID);
			}
			else
			{
				this.m_ClipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
			}
			bool flag5 = ve.subRenderTargetMode > VisualElement.RenderTargetMode.None;
			if (flag5)
			{
				RenderChainCommand renderChainCommand2 = this.m_Owner.AllocCommand();
				renderChainCommand2.owner = this.currentElement;
				renderChainCommand2.type = CommandType.PushRenderTexture;
				this.m_Entries.Add(new UIRStylePainter.Entry
				{
					customCommand = renderChainCommand2
				});
				this.m_ClosingInfo.needsClosing = (this.m_ClosingInfo.blitAndPopRenderTexture = true);
				bool flag6 = this.m_MaskDepth > 0 || this.m_StencilRef > 0;
				if (flag6)
				{
					Debug.LogError("The RenderTargetMode feature must not be used within a stencil mask.");
				}
			}
			bool flag7 = ve.defaultMaterial != null;
			if (flag7)
			{
				RenderChainCommand renderChainCommand3 = this.m_Owner.AllocCommand();
				renderChainCommand3.owner = this.currentElement;
				renderChainCommand3.type = CommandType.PushDefaultMaterial;
				renderChainCommand3.state.material = ve.defaultMaterial;
				this.m_Entries.Add(new UIRStylePainter.Entry
				{
					customCommand = renderChainCommand3
				});
				this.m_ClosingInfo.needsClosing = (this.m_ClosingInfo.PopDefaultMaterial = true);
			}
			bool hasPainter2D = this.meshGenerationContext.hasPainter2D;
			if (hasPainter2D)
			{
				this.meshGenerationContext.painter2D.Reset();
			}
		}

		public void LandClipUnregisterMeshDrawCommand(RenderChainCommand cmd)
		{
			Debug.Assert(this.m_ClosingInfo.needsClosing);
			this.m_ClosingInfo.clipUnregisterDrawCommand = cmd;
		}

		public void LandClipRegisterMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, int indexOffset)
		{
			Debug.Assert(this.m_ClosingInfo.needsClosing);
			this.m_ClosingInfo.clipperRegisterVertices = vertices;
			this.m_ClosingInfo.clipperRegisterIndices = indices;
			this.m_ClosingInfo.clipperRegisterIndexOffset = indexOffset;
		}

		public MeshWriteData AddGradientsEntry(int vertexCount, int indexCount, TextureId texture, Material material, MeshGenerationContext.MeshFlags flags)
		{
			MeshWriteData pooledMeshWriteData = this.GetPooledMeshWriteData();
			bool flag = vertexCount == 0 || indexCount == 0;
			MeshWriteData meshWriteData;
			if (flag)
			{
				pooledMeshWriteData.Reset(default(NativeSlice<Vertex>), default(NativeSlice<ushort>));
				meshWriteData = pooledMeshWriteData;
			}
			else
			{
				this.m_CurrentEntry = new UIRStylePainter.Entry
				{
					vertices = this.m_VertsPool.Alloc(vertexCount),
					indices = this.m_IndicesPool.Alloc(indexCount),
					material = material,
					texture = texture,
					clipRectID = this.m_ClipRectID,
					stencilRef = this.m_StencilRef,
					maskDepth = this.m_MaskDepth,
					addFlags = VertexFlags.IsSvgGradients
				};
				Debug.Assert(this.m_CurrentEntry.vertices.Length == vertexCount);
				Debug.Assert(this.m_CurrentEntry.indices.Length == indexCount);
				pooledMeshWriteData.Reset(this.m_CurrentEntry.vertices, this.m_CurrentEntry.indices, new Rect(0f, 0f, 1f, 1f));
				this.m_Entries.Add(this.m_CurrentEntry);
				this.totalVertices += this.m_CurrentEntry.vertices.Length;
				this.totalIndices += this.m_CurrentEntry.indices.Length;
				this.m_CurrentEntry = default(UIRStylePainter.Entry);
				meshWriteData = pooledMeshWriteData;
			}
			return meshWriteData;
		}

		public MeshWriteData DrawMesh(int vertexCount, int indexCount, Texture texture, Material material, MeshGenerationContext.MeshFlags flags)
		{
			MeshWriteData pooledMeshWriteData = this.GetPooledMeshWriteData();
			bool flag = vertexCount == 0 || indexCount == 0;
			MeshWriteData meshWriteData;
			if (flag)
			{
				pooledMeshWriteData.Reset(default(NativeSlice<Vertex>), default(NativeSlice<ushort>));
				meshWriteData = pooledMeshWriteData;
			}
			else
			{
				this.m_CurrentEntry = new UIRStylePainter.Entry
				{
					vertices = this.m_VertsPool.Alloc(vertexCount),
					indices = this.m_IndicesPool.Alloc(indexCount),
					material = material,
					uvIsDisplacement = ((flags & MeshGenerationContext.MeshFlags.UVisDisplacement) == MeshGenerationContext.MeshFlags.UVisDisplacement),
					clipRectID = this.m_ClipRectID,
					stencilRef = this.m_StencilRef,
					maskDepth = this.m_MaskDepth,
					addFlags = VertexFlags.IsSolid
				};
				Debug.Assert(this.m_CurrentEntry.vertices.Length == vertexCount);
				Debug.Assert(this.m_CurrentEntry.indices.Length == indexCount);
				Rect rect = new Rect(0f, 0f, 1f, 1f);
				bool flag2 = texture != null;
				if (flag2)
				{
					TextureId textureId;
					RectInt rectInt;
					bool flag3 = (flags & MeshGenerationContext.MeshFlags.SkipDynamicAtlas) != MeshGenerationContext.MeshFlags.SkipDynamicAtlas && this.m_Atlas != null && this.m_Atlas.TryGetAtlas(this.currentElement, texture as Texture2D, out textureId, out rectInt);
					if (flag3)
					{
						this.m_CurrentEntry.addFlags = VertexFlags.IsDynamic;
						rect = new Rect((float)rectInt.x, (float)rectInt.y, (float)rectInt.width, (float)rectInt.height);
						this.m_CurrentEntry.texture = textureId;
						this.m_Owner.InsertTexture(this.currentElement, texture, textureId, true);
					}
					else
					{
						TextureId textureId2 = TextureRegistry.instance.Acquire(texture);
						this.m_CurrentEntry.addFlags = VertexFlags.IsTextured;
						this.m_CurrentEntry.texture = textureId2;
						this.m_Owner.InsertTexture(this.currentElement, texture, textureId2, false);
					}
				}
				pooledMeshWriteData.Reset(this.m_CurrentEntry.vertices, this.m_CurrentEntry.indices, rect);
				this.m_Entries.Add(this.m_CurrentEntry);
				this.totalVertices += this.m_CurrentEntry.vertices.Length;
				this.totalIndices += this.m_CurrentEntry.indices.Length;
				this.m_CurrentEntry = default(UIRStylePainter.Entry);
				meshWriteData = pooledMeshWriteData;
			}
			return meshWriteData;
		}

		internal void TryAtlasTexture(Texture texture, MeshGenerationContext.MeshFlags flags, out Rect outUVRegion, out bool outIsAtlas, out TextureId outTextureId, out VertexFlags outAddFlags)
		{
			outUVRegion = new Rect(0f, 0f, 1f, 1f);
			outIsAtlas = false;
			outTextureId = default(TextureId);
			outAddFlags = VertexFlags.IsSolid;
			bool flag = texture == null;
			if (!flag)
			{
				TextureId textureId;
				RectInt rectInt;
				bool flag2 = (flags & MeshGenerationContext.MeshFlags.SkipDynamicAtlas) != MeshGenerationContext.MeshFlags.SkipDynamicAtlas && this.m_Atlas != null && this.m_Atlas.TryGetAtlas(this.currentElement, texture as Texture2D, out textureId, out rectInt);
				if (flag2)
				{
					outAddFlags = VertexFlags.IsDynamic;
					outUVRegion = new Rect((float)rectInt.x, (float)rectInt.y, (float)rectInt.width, (float)rectInt.height);
					outIsAtlas = true;
					outTextureId = textureId;
				}
				else
				{
					outAddFlags = VertexFlags.IsTextured;
					outTextureId = TextureRegistry.instance.Acquire(texture);
				}
			}
		}

		internal unsafe void BuildEntryFromNativeMesh(MeshWriteDataInterface meshData, Texture texture, TextureId textureId, bool isAtlas, Material material, MeshGenerationContext.MeshFlags flags, Rect uvRegion, VertexFlags addFlags)
		{
			bool flag = meshData.vertexCount == 0 || meshData.indexCount == 0;
			if (!flag)
			{
				NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshData.vertices, meshData.vertexCount);
				NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshData.indices, meshData.indexCount);
				bool flag2 = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
				if (!flag2)
				{
					this.m_CurrentEntry = new UIRStylePainter.Entry
					{
						vertices = this.m_VertsPool.Alloc(nativeSlice.Length),
						indices = this.m_IndicesPool.Alloc(nativeSlice2.Length),
						material = material,
						uvIsDisplacement = ((flags & MeshGenerationContext.MeshFlags.UVisDisplacement) == MeshGenerationContext.MeshFlags.UVisDisplacement),
						clipRectID = this.m_ClipRectID,
						stencilRef = this.m_StencilRef,
						maskDepth = this.m_MaskDepth,
						addFlags = VertexFlags.IsSolid
					};
					bool flag3 = textureId.index >= 0;
					if (flag3)
					{
						this.m_CurrentEntry.addFlags = addFlags;
						this.m_CurrentEntry.texture = textureId;
						this.m_Owner.InsertTexture(this.currentElement, texture, textureId, isAtlas);
					}
					Debug.Assert(this.m_CurrentEntry.vertices.Length == nativeSlice.Length);
					Debug.Assert(this.m_CurrentEntry.indices.Length == nativeSlice2.Length);
					this.m_CurrentEntry.vertices.CopyFrom(nativeSlice);
					this.m_CurrentEntry.indices.CopyFrom(nativeSlice2);
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_CurrentEntry = default(UIRStylePainter.Entry);
				}
			}
		}

		internal unsafe void BuildGradientEntryFromNativeMesh(MeshWriteDataInterface meshData, TextureId svgTextureId)
		{
			bool flag = meshData.vertexCount == 0 || meshData.indexCount == 0;
			if (!flag)
			{
				NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshData.vertices, meshData.vertexCount);
				NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshData.indices, meshData.indexCount);
				bool flag2 = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
				if (!flag2)
				{
					this.m_CurrentEntry = new UIRStylePainter.Entry
					{
						vertices = this.m_VertsPool.Alloc(nativeSlice.Length),
						indices = this.m_IndicesPool.Alloc(nativeSlice2.Length),
						texture = svgTextureId,
						clipRectID = this.m_ClipRectID,
						stencilRef = this.m_StencilRef,
						maskDepth = this.m_MaskDepth,
						addFlags = VertexFlags.IsSvgGradients
					};
					Debug.Assert(this.m_CurrentEntry.vertices.Length == nativeSlice.Length);
					Debug.Assert(this.m_CurrentEntry.indices.Length == nativeSlice2.Length);
					this.m_CurrentEntry.vertices.CopyFrom(nativeSlice);
					this.m_CurrentEntry.indices.CopyFrom(nativeSlice2);
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_CurrentEntry = default(UIRStylePainter.Entry);
				}
			}
		}

		public unsafe void BuildRawEntryFromNativeMesh(MeshWriteDataInterface meshData)
		{
			bool flag = meshData.vertexCount == 0 || meshData.indexCount == 0;
			if (!flag)
			{
				NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshData.vertices, meshData.vertexCount);
				NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshData.indices, meshData.indexCount);
				bool flag2 = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
				if (!flag2)
				{
					this.m_CurrentEntry.vertices = this.m_VertsPool.Alloc(meshData.vertexCount);
					this.m_CurrentEntry.indices = this.m_IndicesPool.Alloc(meshData.indexCount);
					this.m_CurrentEntry.vertices.CopyFrom(nativeSlice);
					this.m_CurrentEntry.indices.CopyFrom(nativeSlice2);
				}
			}
		}

		public void DrawText(TextElement te)
		{
			bool flag = !TextUtilities.IsFontAssigned(te);
			if (!flag)
			{
				TextInfo textInfo = te.uitkTextHandle.Update();
				bool hasMultipleColors = textInfo.hasMultipleColors;
				bool flag2 = hasMultipleColors;
				if (flag2)
				{
					te.renderChainData.flags = te.renderChainData.flags | RenderDataFlags.IsIgnoringDynamicColorHint;
				}
				else
				{
					te.renderChainData.flags = te.renderChainData.flags & ~RenderDataFlags.IsIgnoringDynamicColorHint;
				}
				this.DrawTextInfo(textInfo, te.contentRect.min, !hasMultipleColors);
			}
		}

		public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font)
		{
			PanelTextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(this.currentElement);
			this.m_TextInfo.Clear();
			TextGenerationSettings textGenerationSettings = new TextGenerationSettings
			{
				text = text,
				screenRect = Rect.zero,
				fontAsset = font,
				textSettings = textSettingsFrom,
				fontSize = fontSize,
				color = color,
				material = font.material,
				inverseYAxis = true
			};
			TextGenerator.GenerateText(textGenerationSettings, this.m_TextInfo);
			this.DrawTextInfo(this.m_TextInfo, pos, false);
		}

		private void DrawTextInfo(TextInfo textInfo, Vector2 offset, bool useHints)
		{
			for (int i = 0; i < textInfo.materialCount; i++)
			{
				bool flag = textInfo.meshInfo[i].vertexCount == 0;
				if (!flag)
				{
					this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
					this.m_CurrentEntry.stencilRef = this.m_StencilRef;
					this.m_CurrentEntry.maskDepth = this.m_MaskDepth;
					bool flag2 = ((Texture2D)textInfo.meshInfo[i].material.mainTexture).format != TextureFormat.Alpha8;
					if (flag2)
					{
						Texture mainTexture = textInfo.meshInfo[i].material.mainTexture;
						TextureId textureId = TextureRegistry.instance.Acquire(mainTexture);
						this.m_CurrentEntry.texture = textureId;
						this.m_Owner.InsertTexture(this.currentElement, mainTexture, textureId, false);
						MeshBuilder.MakeText(textInfo.meshInfo[i], offset, new MeshBuilder.AllocMeshData
						{
							alloc = this.m_AllocRawVertsIndicesDelegate
						}, VertexFlags.IsTextured, false);
					}
					else
					{
						Texture mainTexture2 = textInfo.meshInfo[i].material.mainTexture;
						float num = 0f;
						bool flag3 = !TextGeneratorUtilities.IsBitmapRendering(textInfo.meshInfo[i].glyphRenderMode);
						if (flag3)
						{
							num = textInfo.meshInfo[i].material.GetFloat(TextShaderUtilities.ID_GradientScale);
						}
						this.m_CurrentEntry.isTextEntry = true;
						this.m_CurrentEntry.fontTexSDFScale = num;
						this.m_CurrentEntry.texture = TextureRegistry.instance.Acquire(mainTexture2);
						this.m_Owner.InsertTexture(this.currentElement, mainTexture2, this.m_CurrentEntry.texture, false);
						bool flag4 = useHints && RenderEvents.NeedsColorID(this.currentElement);
						MeshBuilder.MakeText(textInfo.meshInfo[i], offset, new MeshBuilder.AllocMeshData
						{
							alloc = this.m_AllocRawVertsIndicesDelegate
						}, VertexFlags.IsText, flag4);
					}
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_CurrentEntry = default(UIRStylePainter.Entry);
				}
			}
		}

		public void DrawRectangle(MeshGenerationContextUtils.RectangleParams rectParams)
		{
			bool flag = rectParams.rect.width < 1E-30f || rectParams.rect.height < 1E-30f;
			if (!flag)
			{
				bool flag2 = rectParams.vectorImage != null;
				if (flag2)
				{
					this.DrawVectorImage(rectParams);
				}
				else
				{
					bool flag3 = rectParams.sprite != null;
					if (flag3)
					{
						this.DrawSprite(rectParams);
					}
					else
					{
						Rect rect;
						bool flag4;
						TextureId textureId;
						VertexFlags vertexFlags;
						this.TryAtlasTexture(rectParams.texture, rectParams.meshFlags, out rect, out flag4, out textureId, out vertexFlags);
						MeshBuilderNative.NativeRectParams nativeRectParams = rectParams.ToNativeParams(rect);
						bool flag5 = rectParams.texture != null;
						MeshWriteDataInterface meshWriteDataInterface;
						if (flag5)
						{
							meshWriteDataInterface = MeshBuilderNative.MakeTexturedRect(nativeRectParams, 0f);
						}
						else
						{
							meshWriteDataInterface = MeshBuilderNative.MakeSolidRect(nativeRectParams, 0f);
						}
						this.BuildEntryFromNativeMesh(meshWriteDataInterface, rectParams.texture, textureId, flag4, rectParams.material, rectParams.meshFlags, rect, vertexFlags);
					}
				}
			}
		}

		public void DrawBorder(MeshGenerationContextUtils.BorderParams borderParams)
		{
			MeshWriteDataInterface meshWriteDataInterface = MeshBuilderNative.MakeBorder(borderParams.ToNativeParams(), 0f);
			this.BuildEntryFromNativeMesh(meshWriteDataInterface, null, default(TextureId), false, null, MeshGenerationContext.MeshFlags.None, new Rect(0f, 0f, 1f, 1f), VertexFlags.IsSolid);
		}

		public void DrawImmediate(Action callback, bool cullingEnabled)
		{
			RenderChainCommand renderChainCommand = this.m_Owner.AllocCommand();
			renderChainCommand.type = (cullingEnabled ? CommandType.ImmediateCull : CommandType.Immediate);
			renderChainCommand.owner = this.currentElement;
			renderChainCommand.callback = callback;
			this.m_Entries.Add(new UIRStylePainter.Entry
			{
				customCommand = renderChainCommand
			});
		}

		public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
		{
			bool flag = vectorImage == null;
			if (!flag)
			{
				int num = 0;
				TextureId textureId = default(TextureId);
				bool flag2 = vectorImage.atlas != null;
				bool flag3 = flag2;
				MeshWriteData meshWriteData;
				if (flag3)
				{
					this.RegisterVectorImageGradient(vectorImage, out num, out textureId);
					meshWriteData = this.AddGradientsEntry(vectorImage.vertices.Length, vectorImage.indices.Length, textureId, null, MeshGenerationContext.MeshFlags.None);
				}
				else
				{
					meshWriteData = this.DrawMesh(vectorImage.vertices.Length, vectorImage.indices.Length, null, null, MeshGenerationContext.MeshFlags.None);
				}
				Matrix4x4 matrix4x = Matrix4x4.TRS(offset, Quaternion.AngleAxis(rotationAngle.ToDegrees(), Vector3.forward), new Vector3(scale.x, scale.y, 1f));
				bool flag4 = (scale.x < 0f) ^ (scale.y < 0f);
				int num2 = vectorImage.vertices.Length;
				for (int i = 0; i < num2; i++)
				{
					VectorImageVertex vectorImageVertex = vectorImage.vertices[i];
					Vector3 vector = matrix4x.MultiplyPoint3x4(vectorImageVertex.position);
					vector.z = Vertex.nearZ;
					uint num3 = (uint)((ulong)vectorImageVertex.settingIndex + (ulong)((long)num));
					Color32 color = new Color32((byte)(num3 >> 8), (byte)num3, 0, 0);
					meshWriteData.SetNextVertex(new Vertex
					{
						position = vector,
						tint = vectorImageVertex.tint,
						uv = vectorImageVertex.uv,
						settingIndex = color,
						flags = vectorImageVertex.flags,
						circle = vectorImageVertex.circle
					});
				}
				bool flag5 = !flag4;
				if (flag5)
				{
					meshWriteData.SetAllIndices(vectorImage.indices);
				}
				else
				{
					ushort[] indices = vectorImage.indices;
					for (int j = 0; j < indices.Length; j += 3)
					{
						meshWriteData.SetNextIndex(indices[j]);
						meshWriteData.SetNextIndex(indices[j + 2]);
						meshWriteData.SetNextIndex(indices[j + 1]);
					}
				}
			}
		}

		public VisualElement visualElement
		{
			get
			{
				return this.currentElement;
			}
		}

		public unsafe void DrawVisualElementBackground()
		{
			bool flag = this.currentElement.layout.width <= 1E-30f || this.currentElement.layout.height <= 1E-30f;
			if (!flag)
			{
				ComputedStyle computedStyle = *this.currentElement.computedStyle;
				bool flag2 = computedStyle.backgroundColor.a > 1E-30f;
				if (flag2)
				{
					MeshGenerationContextUtils.RectangleParams rectangleParams = new MeshGenerationContextUtils.RectangleParams
					{
						rect = this.currentElement.rect,
						color = computedStyle.backgroundColor,
						colorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.backgroundColorID),
						playmodeTintColor = ((this.currentElement.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white)
					};
					MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out rectangleParams.topLeftRadius, out rectangleParams.bottomLeftRadius, out rectangleParams.topRightRadius, out rectangleParams.bottomRightRadius);
					MeshGenerationContextUtils.AdjustBackgroundSizeForBorders(this.currentElement, ref rectangleParams);
					this.DrawRectangle(rectangleParams);
				}
				Vector4 vector = new Vector4((float)computedStyle.unitySliceLeft, (float)computedStyle.unitySliceTop, (float)computedStyle.unitySliceRight, (float)computedStyle.unitySliceBottom);
				MeshGenerationContextUtils.RectangleParams rectangleParams2 = default(MeshGenerationContextUtils.RectangleParams);
				MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out rectangleParams2.topLeftRadius, out rectangleParams2.bottomLeftRadius, out rectangleParams2.topRightRadius, out rectangleParams2.bottomRightRadius);
				Background backgroundImage = computedStyle.backgroundImage;
				bool flag3 = backgroundImage.texture != null || backgroundImage.sprite != null || backgroundImage.vectorImage != null || backgroundImage.renderTexture != null;
				if (flag3)
				{
					MeshGenerationContextUtils.RectangleParams rectangleParams3 = default(MeshGenerationContextUtils.RectangleParams);
					float num = this.visualElement.resolvedStyle.unitySliceScale;
					bool flag4;
					ScaleMode scaleMode = BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(computedStyle.backgroundPositionX, computedStyle.backgroundPositionY, computedStyle.backgroundRepeat, computedStyle.backgroundSize, out flag4);
					bool flag5 = backgroundImage.texture != null;
					if (flag5)
					{
						bool flag6 = Mathf.RoundToInt(vector.x) != 0 || Mathf.RoundToInt(vector.y) != 0 || Mathf.RoundToInt(vector.z) != 0 || Mathf.RoundToInt(vector.w) != 0;
						rectangleParams3 = MeshGenerationContextUtils.RectangleParams.MakeTextured(this.currentElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.texture, flag6 ? (flag4 ? scaleMode : ScaleMode.StretchToFill) : ScaleMode.ScaleToFit, this.currentElement.panel.contextType);
						rectangleParams3.rect = new Rect(0f, 0f, (float)rectangleParams3.texture.width, (float)rectangleParams3.texture.height);
					}
					else
					{
						bool flag7 = backgroundImage.sprite != null;
						if (flag7)
						{
							bool flag8 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
							rectangleParams3 = MeshGenerationContextUtils.RectangleParams.MakeSprite(this.currentElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.sprite, flag8 ? ScaleMode.StretchToFill : scaleMode, this.currentElement.panel.contextType, rectangleParams2.HasRadius(0.001f), ref vector, flag8);
							bool flag9 = rectangleParams3.texture != null;
							if (flag9)
							{
								rectangleParams3.rect = new Rect(0f, 0f, backgroundImage.sprite.rect.width, backgroundImage.sprite.rect.height);
							}
							num *= UIElementsUtility.PixelsPerUnitScaleForElement(this.visualElement, backgroundImage.sprite);
						}
						else
						{
							bool flag10 = backgroundImage.renderTexture != null;
							if (flag10)
							{
								rectangleParams3 = MeshGenerationContextUtils.RectangleParams.MakeTextured(this.currentElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.renderTexture, ScaleMode.ScaleToFit, this.currentElement.panel.contextType);
								rectangleParams3.rect = new Rect(0f, 0f, (float)rectangleParams3.texture.width, (float)rectangleParams3.texture.height);
							}
							else
							{
								bool flag11 = backgroundImage.vectorImage != null;
								if (flag11)
								{
									bool flag12 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
									rectangleParams3 = MeshGenerationContextUtils.RectangleParams.MakeVectorTextured(this.currentElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.vectorImage, flag12 ? ScaleMode.StretchToFill : scaleMode, this.currentElement.panel.contextType);
									rectangleParams3.rect = new Rect(0f, 0f, rectangleParams3.vectorImage.size.x, rectangleParams3.vectorImage.size.y);
								}
							}
						}
					}
					rectangleParams3.topLeftRadius = rectangleParams2.topLeftRadius;
					rectangleParams3.topRightRadius = rectangleParams2.topRightRadius;
					rectangleParams3.bottomRightRadius = rectangleParams2.bottomRightRadius;
					rectangleParams3.bottomLeftRadius = rectangleParams2.bottomLeftRadius;
					bool flag13 = vector != Vector4.zero;
					if (flag13)
					{
						rectangleParams3.leftSlice = Mathf.RoundToInt(vector.x);
						rectangleParams3.topSlice = Mathf.RoundToInt(vector.y);
						rectangleParams3.rightSlice = Mathf.RoundToInt(vector.z);
						rectangleParams3.bottomSlice = Mathf.RoundToInt(vector.w);
						rectangleParams3.sliceScale = num;
						bool flag14 = !flag4;
						if (flag14)
						{
							rectangleParams3.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams3.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams3.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill);
							rectangleParams3.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.StretchToFill);
						}
						else
						{
							rectangleParams3.backgroundPositionX = computedStyle.backgroundPositionX;
							rectangleParams3.backgroundPositionY = computedStyle.backgroundPositionY;
							rectangleParams3.backgroundRepeat = computedStyle.backgroundRepeat;
							rectangleParams3.backgroundSize = computedStyle.backgroundSize;
						}
					}
					else
					{
						rectangleParams3.backgroundPositionX = computedStyle.backgroundPositionX;
						rectangleParams3.backgroundPositionY = computedStyle.backgroundPositionY;
						rectangleParams3.backgroundRepeat = computedStyle.backgroundRepeat;
						rectangleParams3.backgroundSize = computedStyle.backgroundSize;
					}
					rectangleParams3.color = computedStyle.unityBackgroundImageTintColor;
					rectangleParams3.colorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.tintColorID);
					MeshGenerationContextUtils.AdjustBackgroundSizeForBorders(this.currentElement, ref rectangleParams3);
					bool flag15 = rectangleParams3.texture != null || rectangleParams3.vectorImage != null;
					if (flag15)
					{
						this.DrawRectangleRepeat(rectangleParams3, this.currentElement.rect, this.currentElement.scaledPixelsPerPoint);
					}
					else
					{
						this.DrawRectangle(rectangleParams3);
					}
				}
			}
		}

		private void DrawRectangleRepeat(MeshGenerationContextUtils.RectangleParams rectParams, Rect totalRect, float scaledPixelsPerPoint)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			bool flag = this.m_RepeatRectUVList == null;
			if (flag)
			{
				this.m_RepeatRectUVList = new List<UIRStylePainter.RepeatRectUV>[2];
				this.m_RepeatRectUVList[0] = new List<UIRStylePainter.RepeatRectUV>();
				this.m_RepeatRectUVList[1] = new List<UIRStylePainter.RepeatRectUV>();
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
							UIRStylePainter.RepeatRectUV repeatRectUV;
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
									UIRStylePainter.RepeatRectUV repeatRectUV2;
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
										UIRStylePainter.RepeatRectUV repeatRectUV3;
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
										UIRStylePainter.RepeatRectUV repeatRectUV4;
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
											UIRStylePainter.RepeatRectUV repeatRectUV5;
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
											UIRStylePainter.RepeatRectUV repeatRectUV6;
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
							UIRStylePainter.RepeatRectUV repeatRectUV7 = this.m_RepeatRectUVList[i][m];
							Vector2 position5 = repeatRectUV7.rect.position;
							ref Vector2 ptr = ref position5;
							int num20 = i;
							ptr[num20] += num15;
							repeatRectUV7.rect.position = position5;
							this.m_RepeatRectUVList[i][m] = repeatRectUV7;
						}
					}
					Rect rect13 = new Rect(rect);
					foreach (UIRStylePainter.RepeatRectUV repeatRectUV8 in this.m_RepeatRectUVList[1])
					{
						Rect rect14 = repeatRectUV8.rect;
						rect2.y = rect14.y;
						rect14 = repeatRectUV8.rect;
						rect2.height = rect14.height;
						rect14 = repeatRectUV8.uv;
						rect.y = rect14.y;
						rect14 = repeatRectUV8.uv;
						rect.height = rect14.height;
						bool flag46 = rect2.y < totalRect.y;
						if (flag46)
						{
							float num21 = totalRect.y - rect2.y;
							float num22 = rect2.height - num21;
							float num23 = num21 + num22;
							float num24 = rect13.height * num22 / num23;
							float num25 = rect13.height * num21 / num23;
							rect.y = num25 + rect13.y;
							rect.height = num24;
							rect2.y = totalRect.y;
							rect2.height = num22;
						}
						bool flag47 = rect2.yMax > totalRect.yMax;
						if (flag47)
						{
							float num26 = rect2.yMax - totalRect.yMax;
							float num27 = rect2.height - num26;
							float num28 = num27 + num26;
							float num29 = rect.height * num27 / num28;
							rect.height = num29;
							rect.y = rect.yMax - num29;
							rect2.height = num27;
						}
						bool flag48 = rectParams.vectorImage == null;
						if (flag48)
						{
							float num30 = rect.y - rect13.y;
							float num31 = rect13.yMax - rect.yMax;
							rect.y += num31 - num30;
						}
						foreach (UIRStylePainter.RepeatRectUV repeatRectUV9 in this.m_RepeatRectUVList[0])
						{
							rect14 = repeatRectUV9.rect;
							rect2.x = rect14.x;
							rect14 = repeatRectUV9.rect;
							rect2.width = rect14.width;
							rect14 = repeatRectUV9.uv;
							rect.x = rect14.x;
							rect14 = repeatRectUV9.uv;
							rect.width = rect14.width;
							bool flag49 = rect2.x < totalRect.x;
							if (flag49)
							{
								float num32 = totalRect.x - rect2.x;
								float num33 = rect2.width - num32;
								float num34 = num32 + num33;
								float num35 = rect.width * num33 / num34;
								float num36 = rect13.x + rect13.width * num32 / num34;
								rect.x = num36;
								rect.width = num35;
								rect2.x = totalRect.x;
								rect2.width = num33;
							}
							bool flag50 = rect2.xMax > totalRect.xMax;
							if (flag50)
							{
								float num37 = rect2.xMax - totalRect.xMax;
								float num38 = rect2.width - num37;
								float num39 = num38 + num37;
								float num40 = rect.width * num38 / num39;
								rect.width = num40;
								rect2.width = num38;
							}
							this.StampRectangleWithSubRect(rectParams, rect2, totalRect, rect);
						}
					}
				}
			}
		}

		private void StampRectangleWithSubRect(MeshGenerationContextUtils.RectangleParams rectParams, Rect targetRect, Rect totalRect, Rect targetUV)
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
					Rect rect2 = MeshGenerationContextUtils.RectangleParams.RectIntersection(subRect, targetRect);
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
				this.DrawRectangle(rectParams);
			}
		}

		public void DrawVisualElementBorder()
		{
			bool flag = this.currentElement.layout.width >= 1E-30f && this.currentElement.layout.height >= 1E-30f;
			if (flag)
			{
				IResolvedStyle resolvedStyle = this.currentElement.resolvedStyle;
				bool flag2 = (resolvedStyle.borderLeftColor != Color.clear && resolvedStyle.borderLeftWidth > 0f) || (resolvedStyle.borderTopColor != Color.clear && resolvedStyle.borderTopWidth > 0f) || (resolvedStyle.borderRightColor != Color.clear && resolvedStyle.borderRightWidth > 0f) || (resolvedStyle.borderBottomColor != Color.clear && resolvedStyle.borderBottomWidth > 0f);
				if (flag2)
				{
					MeshGenerationContextUtils.BorderParams borderParams = new MeshGenerationContextUtils.BorderParams
					{
						rect = this.currentElement.rect,
						leftColor = resolvedStyle.borderLeftColor,
						topColor = resolvedStyle.borderTopColor,
						rightColor = resolvedStyle.borderRightColor,
						bottomColor = resolvedStyle.borderBottomColor,
						leftWidth = resolvedStyle.borderLeftWidth,
						topWidth = resolvedStyle.borderTopWidth,
						rightWidth = resolvedStyle.borderRightWidth,
						bottomWidth = resolvedStyle.borderBottomWidth,
						leftColorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.borderLeftColorID),
						topColorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.borderTopColorID),
						rightColorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.borderRightColorID),
						bottomColorPage = ColorPage.Init(this.m_Owner, this.currentElement.renderChainData.borderBottomColorID),
						playmodeTintColor = ((this.currentElement.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white)
					};
					MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out borderParams.topLeftRadius, out borderParams.bottomLeftRadius, out borderParams.topRightRadius, out borderParams.bottomRightRadius);
					this.DrawBorder(borderParams);
				}
			}
		}

		public void ApplyVisualElementClipping()
		{
			bool flag = this.currentElement.renderChainData.clipMethod == ClipMethod.Scissor;
			if (flag)
			{
				RenderChainCommand renderChainCommand = this.m_Owner.AllocCommand();
				renderChainCommand.type = CommandType.PushScissor;
				renderChainCommand.owner = this.currentElement;
				this.m_Entries.Add(new UIRStylePainter.Entry
				{
					customCommand = renderChainCommand
				});
				this.m_ClosingInfo.needsClosing = (this.m_ClosingInfo.popScissorClip = true);
			}
			else
			{
				bool flag2 = this.currentElement.renderChainData.clipMethod == ClipMethod.Stencil;
				if (flag2)
				{
					bool flag3 = this.m_MaskDepth > this.m_StencilRef;
					if (flag3)
					{
						this.m_StencilRef++;
						Debug.Assert(this.m_MaskDepth == this.m_StencilRef);
					}
					this.m_ClosingInfo.maskStencilRef = this.m_StencilRef;
					bool flag4 = UIRUtility.IsVectorImageBackground(this.currentElement);
					if (flag4)
					{
						this.GenerateStencilClipEntryForSVGBackground();
					}
					else
					{
						this.GenerateStencilClipEntryForRoundedRectBackground();
					}
					this.m_MaskDepth++;
				}
			}
			this.m_ClipRectID = this.currentElement.renderChainData.clipRectID;
		}

		private ushort[] AdjustSpriteWinding(Vector2[] vertices, ushort[] indices)
		{
			ushort[] array = new ushort[indices.Length];
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
					array[i] = indices[i + 1];
					array[i + 1] = indices[i];
					array[i + 2] = indices[i + 2];
				}
				else
				{
					array[i] = indices[i];
					array[i + 1] = indices[i + 1];
					array[i + 2] = indices[i + 2];
				}
			}
			return array;
		}

		public void DrawSprite(MeshGenerationContextUtils.RectangleParams rectParams)
		{
			Sprite sprite = rectParams.sprite;
			bool flag = sprite.texture == null || sprite.triangles.Length == 0;
			if (!flag)
			{
				MeshBuilder.AllocMeshData allocMeshData = new MeshBuilder.AllocMeshData
				{
					alloc = this.m_AllocThroughDrawMeshDelegate,
					texture = sprite.texture,
					flags = rectParams.meshFlags
				};
				Vector2[] vertices = sprite.vertices;
				ushort[] triangles = sprite.triangles;
				Vector2[] uv = sprite.uv;
				int num = sprite.vertices.Length;
				Vertex[] array = new Vertex[num];
				ushort[] array2 = this.AdjustSpriteWinding(vertices, triangles);
				MeshWriteData meshWriteData = allocMeshData.Allocate((uint)array.Length, (uint)array2.Length);
				Rect uvRegion = meshWriteData.uvRegion;
				ColorPage colorPage = rectParams.colorPage;
				Color32 pageAndID = colorPage.pageAndID;
				Color32 color = new Color32(0, 0, 0, colorPage.isValid ? 1 : 0);
				Color32 color2 = new Color32(0, 0, colorPage.pageAndID.r, colorPage.pageAndID.g);
				Color32 color3 = new Color32(0, 0, 0, colorPage.pageAndID.b);
				for (int i = 0; i < num; i++)
				{
					Vector2 vector = vertices[i];
					vector -= rectParams.spriteGeomRect.position;
					vector /= rectParams.spriteGeomRect.size;
					vector.y = 1f - vector.y;
					vector *= rectParams.rect.size;
					vector += rectParams.rect.position;
					Vector2 vector2 = uv[i];
					vector2 *= uvRegion.size;
					vector2 += uvRegion.position;
					array[i] = new Vertex
					{
						position = new Vector3(vector.x, vector.y, Vertex.nearZ),
						tint = rectParams.color,
						uv = vector2,
						flags = color,
						opacityColorPages = color2,
						ids = color3
					};
				}
				meshWriteData.SetAllVertices(array);
				meshWriteData.SetAllIndices(array2);
			}
		}

		public void RegisterVectorImageGradient(VectorImage vi, out int settingIndexOffset, out TextureId texture)
		{
			texture = default(TextureId);
			GradientRemap gradientRemap = this.m_VectorImageManager.AddUser(vi, this.currentElement);
			settingIndexOffset = gradientRemap.destIndex;
			bool flag = gradientRemap.atlas != TextureId.invalid;
			if (flag)
			{
				texture = gradientRemap.atlas;
			}
			else
			{
				texture = TextureRegistry.instance.Acquire(vi.atlas);
				this.m_Owner.InsertTexture(this.currentElement, vi.atlas, texture, false);
			}
		}

		public void DrawVectorImage(MeshGenerationContextUtils.RectangleParams rectParams)
		{
			VectorImage vectorImage = rectParams.vectorImage;
			Debug.Assert(vectorImage != null);
			int num = 0;
			TextureId textureId = default(TextureId);
			bool flag = vectorImage.atlas != null && this.m_VectorImageManager != null;
			bool flag2 = flag;
			if (flag2)
			{
				GradientRemap gradientRemap = this.m_VectorImageManager.AddUser(vectorImage, this.currentElement);
				num = gradientRemap.destIndex;
				bool flag3 = gradientRemap.atlas != TextureId.invalid;
				if (flag3)
				{
					textureId = gradientRemap.atlas;
				}
				else
				{
					textureId = TextureRegistry.instance.Acquire(vectorImage.atlas);
					this.m_Owner.InsertTexture(this.currentElement, vectorImage.atlas, textureId, false);
				}
			}
			int count = this.m_Entries.Count;
			int num2;
			int num3;
			this.MakeVectorGraphics(rectParams, flag, textureId, num, out num2, out num3);
			Debug.Assert(count <= this.m_Entries.Count + 1);
			bool flag4 = count != this.m_Entries.Count;
			if (flag4)
			{
				this.m_SVGBackgroundEntryIndex = this.m_Entries.Count - 1;
				bool flag5 = num2 != 0 && num3 != 0;
				if (flag5)
				{
					UIRStylePainter.Entry entry = this.m_Entries[this.m_SVGBackgroundEntryIndex];
					entry.vertices = entry.vertices.Slice<Vertex>(0, num2);
					entry.indices = entry.indices.Slice<ushort>(0, num3);
					this.m_Entries[this.m_SVGBackgroundEntryIndex] = entry;
				}
			}
		}

		private void MakeVectorGraphics(MeshGenerationContextUtils.RectangleParams rectParams, bool isUsingGradients, TextureId svgTexture, int settingIndexOffset, out int finalVertexCount, out int finalIndexCount)
		{
			VectorImage vectorImage = rectParams.vectorImage;
			Debug.Assert(vectorImage != null);
			finalVertexCount = 0;
			finalIndexCount = 0;
			int num = vectorImage.vertices.Length;
			Vertex[] array = new Vertex[num];
			for (int i = 0; i < num; i++)
			{
				VectorImageVertex vectorImageVertex = vectorImage.vertices[i];
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
			bool flag = (float)rectParams.leftSlice <= 1E-30f && (float)rectParams.topSlice <= 1E-30f && (float)rectParams.rightSlice <= 1E-30f && (float)rectParams.bottomSlice <= 1E-30f;
			MeshWriteDataInterface meshWriteDataInterface;
			if (flag)
			{
				meshWriteDataInterface = MeshBuilderNative.MakeVectorGraphicsStretchBackground(array, vectorImage.indices, vectorImage.size.x, vectorImage.size.y, rectParams.rect, rectParams.uv, rectParams.scaleMode, rectParams.color, rectParams.colorPage.ToNativeColorPage(), settingIndexOffset, ref finalVertexCount, ref finalIndexCount);
			}
			else
			{
				Vector4 vector = new Vector4((float)rectParams.leftSlice, (float)rectParams.topSlice, (float)rectParams.rightSlice, (float)rectParams.bottomSlice);
				meshWriteDataInterface = MeshBuilderNative.MakeVectorGraphics9SliceBackground(array, vectorImage.indices, vectorImage.size.x, vectorImage.size.y, rectParams.rect, vector, rectParams.color, rectParams.colorPage.ToNativeColorPage(), settingIndexOffset);
			}
			if (isUsingGradients)
			{
				this.BuildGradientEntryFromNativeMesh(meshWriteDataInterface, svgTexture);
			}
			else
			{
				this.BuildEntryFromNativeMesh(meshWriteDataInterface, null, default(TextureId), false, null, MeshGenerationContext.MeshFlags.None, new Rect(0f, 0f, 1f, 1f), VertexFlags.IsSolid);
			}
		}

		internal void Reset()
		{
			this.ValidateMeshWriteData();
			this.m_Entries.Clear();
			this.m_ClosingInfo = default(UIRStylePainter.ClosingInfo);
			this.m_NextMeshWriteDataPoolItem = 0;
			this.currentElement = null;
			this.totalVertices = (this.totalIndices = 0);
		}

		private void ValidateMeshWriteData()
		{
			for (int i = 0; i < this.m_NextMeshWriteDataPoolItem; i++)
			{
				MeshWriteData meshWriteData = this.m_MeshWriteDataPool[i];
				bool flag = meshWriteData.vertexCount > 0 && meshWriteData.currentVertex < meshWriteData.vertexCount;
				if (flag)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Not enough vertices written in generateVisualContent callback (asked for ",
						meshWriteData.vertexCount.ToString(),
						" but only wrote ",
						meshWriteData.currentVertex.ToString(),
						")"
					}));
					Vertex vertex = meshWriteData.m_Vertices[0];
					while (meshWriteData.currentVertex < meshWriteData.vertexCount)
					{
						meshWriteData.SetNextVertex(vertex);
					}
				}
				bool flag2 = meshWriteData.indexCount > 0 && meshWriteData.currentIndex < meshWriteData.indexCount;
				if (flag2)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Not enough indices written in generateVisualContent callback (asked for ",
						meshWriteData.indexCount.ToString(),
						" but only wrote ",
						meshWriteData.currentIndex.ToString(),
						")"
					}));
					while (meshWriteData.currentIndex < meshWriteData.indexCount)
					{
						meshWriteData.SetNextIndex(0);
					}
				}
			}
		}

		private void GenerateStencilClipEntryForRoundedRectBackground()
		{
			bool flag = this.currentElement.layout.width <= 1E-30f || this.currentElement.layout.height <= 1E-30f;
			if (!flag)
			{
				IResolvedStyle resolvedStyle = this.currentElement.resolvedStyle;
				Vector2 vector;
				Vector2 vector2;
				Vector2 vector3;
				Vector2 vector4;
				MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out vector, out vector2, out vector3, out vector4);
				float borderTopWidth = resolvedStyle.borderTopWidth;
				float borderLeftWidth = resolvedStyle.borderLeftWidth;
				float borderBottomWidth = resolvedStyle.borderBottomWidth;
				float borderRightWidth = resolvedStyle.borderRightWidth;
				MeshGenerationContextUtils.RectangleParams rectangleParams = new MeshGenerationContextUtils.RectangleParams
				{
					rect = this.currentElement.rect,
					color = Color.white,
					topLeftRadius = Vector2.Max(Vector2.zero, vector - new Vector2(borderLeftWidth, borderTopWidth)),
					topRightRadius = Vector2.Max(Vector2.zero, vector3 - new Vector2(borderRightWidth, borderTopWidth)),
					bottomLeftRadius = Vector2.Max(Vector2.zero, vector2 - new Vector2(borderLeftWidth, borderBottomWidth)),
					bottomRightRadius = Vector2.Max(Vector2.zero, vector4 - new Vector2(borderRightWidth, borderBottomWidth)),
					playmodeTintColor = ((this.currentElement.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white)
				};
				rectangleParams.rect.x = rectangleParams.rect.x + borderLeftWidth;
				rectangleParams.rect.y = rectangleParams.rect.y + borderTopWidth;
				rectangleParams.rect.width = rectangleParams.rect.width - (borderLeftWidth + borderRightWidth);
				rectangleParams.rect.height = rectangleParams.rect.height - (borderTopWidth + borderBottomWidth);
				bool flag2 = this.currentElement.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox;
				if (flag2)
				{
					rectangleParams.rect.x = rectangleParams.rect.x + resolvedStyle.paddingLeft;
					rectangleParams.rect.y = rectangleParams.rect.y + resolvedStyle.paddingTop;
					rectangleParams.rect.width = rectangleParams.rect.width - (resolvedStyle.paddingLeft + resolvedStyle.paddingRight);
					rectangleParams.rect.height = rectangleParams.rect.height - (resolvedStyle.paddingTop + resolvedStyle.paddingBottom);
				}
				this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
				this.m_CurrentEntry.stencilRef = this.m_StencilRef;
				this.m_CurrentEntry.maskDepth = this.m_MaskDepth;
				this.m_CurrentEntry.isClipRegisterEntry = true;
				MeshBuilderNative.NativeRectParams nativeRectParams = rectangleParams.ToNativeParams(new Rect(0f, 0f, 1f, 1f));
				MeshWriteDataInterface meshWriteDataInterface = MeshBuilderNative.MakeSolidRect(nativeRectParams, 1f);
				bool flag3 = meshWriteDataInterface.vertexCount > 0 && meshWriteDataInterface.indexCount > 0;
				if (flag3)
				{
					this.BuildRawEntryFromNativeMesh(meshWriteDataInterface);
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_ClosingInfo.needsClosing = true;
				}
				this.m_CurrentEntry = default(UIRStylePainter.Entry);
			}
		}

		private void GenerateStencilClipEntryForSVGBackground()
		{
			bool flag = this.m_SVGBackgroundEntryIndex == -1;
			if (!flag)
			{
				UIRStylePainter.Entry entry = this.m_Entries[this.m_SVGBackgroundEntryIndex];
				Debug.Assert(entry.vertices.Length > 0);
				Debug.Assert(entry.indices.Length > 0);
				this.m_CurrentEntry.vertices = entry.vertices;
				this.m_CurrentEntry.indices = entry.indices;
				this.m_CurrentEntry.uvIsDisplacement = entry.uvIsDisplacement;
				this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
				this.m_CurrentEntry.stencilRef = this.m_StencilRef;
				this.m_CurrentEntry.maskDepth = this.m_MaskDepth;
				this.m_CurrentEntry.isClipRegisterEntry = true;
				this.m_ClosingInfo.needsClosing = true;
				int length = this.m_CurrentEntry.vertices.Length;
				NativeSlice<Vertex> nativeSlice = this.m_VertsPool.Alloc(length);
				for (int i = 0; i < length; i++)
				{
					Vertex vertex = this.m_CurrentEntry.vertices[i];
					vertex.position.z = 1f;
					nativeSlice[i] = vertex;
				}
				this.m_CurrentEntry.vertices = nativeSlice;
				this.totalVertices += this.m_CurrentEntry.vertices.Length;
				this.totalIndices += this.m_CurrentEntry.indices.Length;
				this.m_Entries.Add(this.m_CurrentEntry);
				this.m_CurrentEntry = default(UIRStylePainter.Entry);
			}
		}

		private RenderChain m_Owner;

		private List<UIRStylePainter.Entry> m_Entries = new List<UIRStylePainter.Entry>();

		private AtlasBase m_Atlas;

		private VectorImageManager m_VectorImageManager;

		private UIRStylePainter.Entry m_CurrentEntry;

		private UIRStylePainter.ClosingInfo m_ClosingInfo;

		private int m_MaskDepth;

		private int m_StencilRef;

		private BMPAlloc m_ClipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;

		private int m_SVGBackgroundEntryIndex = -1;

		private TempAllocator<Vertex> m_VertsPool;

		private TempAllocator<ushort> m_IndicesPool;

		private List<MeshWriteData> m_MeshWriteDataPool;

		private int m_NextMeshWriteDataPoolItem;

		private List<UIRStylePainter.RepeatRectUV>[] m_RepeatRectUVList = null;

		private MeshBuilder.AllocMeshData.Allocator m_AllocRawVertsIndicesDelegate;

		private MeshBuilder.AllocMeshData.Allocator m_AllocThroughDrawMeshDelegate;

		private TextInfo m_TextInfo = new TextInfo();

		internal struct Entry
		{
			public NativeSlice<Vertex> vertices;

			public NativeSlice<ushort> indices;

			public Material material;

			public float fontTexSDFScale;

			public TextureId texture;

			public RenderChainCommand customCommand;

			public BMPAlloc clipRectID;

			public VertexFlags addFlags;

			public bool uvIsDisplacement;

			public bool isTextEntry;

			public bool isClipRegisterEntry;

			public int stencilRef;

			public int maskDepth;
		}

		internal struct ClosingInfo
		{
			public bool needsClosing;

			public bool popViewMatrix;

			public bool popScissorClip;

			public bool blitAndPopRenderTexture;

			public bool PopDefaultMaterial;

			public RenderChainCommand clipUnregisterDrawCommand;

			public NativeSlice<Vertex> clipperRegisterVertices;

			public NativeSlice<ushort> clipperRegisterIndices;

			public int clipperRegisterIndexOffset;

			public int maskStencilRef;
		}

		private struct RepeatRectUV
		{
			public Rect rect;

			public Rect uv;
		}
	}
}
