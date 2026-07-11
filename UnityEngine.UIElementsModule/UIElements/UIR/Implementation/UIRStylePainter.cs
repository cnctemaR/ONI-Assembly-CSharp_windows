using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.TextCore;

namespace UnityEngine.UIElements.UIR.Implementation
{
	internal class UIRStylePainter : IStylePainter, IDisposable
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
			this.m_CurrentEntry.vertices = this.m_VertsPool.Alloc(vertexCount);
			this.m_CurrentEntry.indices = this.m_IndicesPool.Alloc(indexCount);
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
			this.device = renderChain.device;
			this.m_AtlasManager = renderChain.atlasManager;
			this.m_VectorImageManager = renderChain.vectorImageManager;
			this.m_AllocRawVertsIndicesDelegate = new MeshBuilder.AllocMeshData.Allocator(this.AllocRawVertsIndices);
			this.m_AllocThroughDrawMeshDelegate = new MeshBuilder.AllocMeshData.Allocator(this.AllocThroughDrawMesh);
			int num = 32;
			this.m_MeshWriteDataPool = new List<MeshWriteData>(num);
			for (int i = 0; i < num; i++)
			{
				this.m_MeshWriteDataPool.Add(new MeshWriteData());
			}
		}

		public MeshGenerationContext meshGenerationContext { get; }

		public VisualElement currentElement { get; private set; }

		public UIRenderDevice device { get; }

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

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_IndicesPool.Dispose();
					this.m_VertsPool.Dispose();
				}
				this.disposed = true;
			}
		}

		public void Begin(VisualElement ve)
		{
			this.currentElement = ve;
			this.m_NextMeshWriteDataPoolItem = 0;
			this.m_SVGBackgroundEntryIndex = -1;
			this.currentElement.renderChainData.usesLegacyText = (this.currentElement.renderChainData.usesAtlas = (this.currentElement.renderChainData.disableNudging = false));
			this.currentElement.renderChainData.displacementUVStart = (this.currentElement.renderChainData.displacementUVEnd = 0);
			bool flag = (this.currentElement.renderHints & RenderHints.GroupTransform) > RenderHints.None;
			bool flag2 = flag;
			if (flag2)
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
			bool flag3 = this.currentElement.hierarchy.parent != null;
			if (flag3)
			{
				this.m_StencilClip = this.currentElement.hierarchy.parent.renderChainData.isStencilClipped;
				this.m_ClipRectID = (flag ? UIRVEShaderInfoAllocator.infiniteClipRect : this.currentElement.hierarchy.parent.renderChainData.clipRectID);
			}
			else
			{
				this.m_StencilClip = false;
				this.m_ClipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
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
					vertices = this.m_VertsPool.Alloc((uint)vertexCount),
					indices = this.m_IndicesPool.Alloc((uint)indexCount),
					material = material,
					uvIsDisplacement = (flags == MeshGenerationContext.MeshFlags.UVisDisplacement),
					clipRectID = this.m_ClipRectID,
					isStencilClipped = this.m_StencilClip,
					addFlags = VertexFlags.IsSolid
				};
				Debug.Assert(this.m_CurrentEntry.vertices.Length == vertexCount);
				Debug.Assert(this.m_CurrentEntry.indices.Length == indexCount);
				Rect rect = new Rect(0f, 0f, 1f, 1f);
				bool flag2 = flags == MeshGenerationContext.MeshFlags.IsSVGGradients;
				bool flag3 = flags == MeshGenerationContext.MeshFlags.IsCustomSVGGradients;
				bool flag4 = flag2 || flag3;
				if (flag4)
				{
					this.m_CurrentEntry.addFlags = (flag2 ? VertexFlags.IsSVGGradients : VertexFlags.IsCustomSVGGradients);
					bool flag5 = flag3;
					if (flag5)
					{
						this.m_CurrentEntry.custom = texture;
					}
					this.currentElement.renderChainData.usesAtlas = true;
				}
				else
				{
					bool flag6 = texture != null;
					if (flag6)
					{
						RectInt rectInt;
						bool flag7 = this.m_AtlasManager != null && this.m_AtlasManager.TryGetLocation(texture as Texture2D, out rectInt);
						if (flag7)
						{
							this.m_CurrentEntry.addFlags = ((texture.filterMode == FilterMode.Point) ? VertexFlags.IsAtlasTexturedPoint : VertexFlags.IsAtlasTexturedBilinear);
							this.currentElement.renderChainData.usesAtlas = true;
							rect = new Rect((float)rectInt.x, (float)rectInt.y, (float)rectInt.width, (float)rectInt.height);
						}
						else
						{
							this.m_CurrentEntry.addFlags = VertexFlags.IsCustomTextured;
							this.m_CurrentEntry.custom = texture;
						}
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

		public void DrawText(MeshGenerationContextUtils.TextParams textParams, TextHandle handle, float pixelsPerPoint)
		{
			bool flag = textParams.font == null;
			if (!flag)
			{
				bool useLegacy = handle.useLegacy;
				if (useLegacy)
				{
					this.DrawTextNative(textParams, handle, pixelsPerPoint);
				}
				else
				{
					this.DrawTextCore(textParams, handle, pixelsPerPoint);
				}
			}
		}

		private void DrawTextNative(MeshGenerationContextUtils.TextParams textParams, TextHandle handle, float pixelsPerPoint)
		{
			float num = TextHandle.ComputeTextScaling(this.currentElement.worldTransform, pixelsPerPoint);
			TextNativeSettings textNativeSettings = MeshGenerationContextUtils.TextParams.GetTextNativeSettings(textParams, num);
			using (NativeArray<TextVertex> vertices = TextNative.GetVertices(textNativeSettings))
			{
				bool flag = vertices.Length == 0;
				if (!flag)
				{
					Vector2 offset = TextNative.GetOffset(textNativeSettings, textParams.rect);
					this.m_CurrentEntry.isTextEntry = true;
					this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
					this.m_CurrentEntry.isStencilClipped = this.m_StencilClip;
					MeshBuilder.MakeText(vertices, offset, new MeshBuilder.AllocMeshData
					{
						alloc = this.m_AllocRawVertsIndicesDelegate
					});
					this.m_CurrentEntry.font = textParams.font.material.mainTexture;
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_CurrentEntry = default(UIRStylePainter.Entry);
					this.currentElement.renderChainData.usesLegacyText = true;
				}
			}
		}

		private void DrawTextCore(MeshGenerationContextUtils.TextParams textParams, TextHandle handle, float pixelsPerPoint)
		{
			TextInfo textInfo = handle.Update(textParams, pixelsPerPoint);
			for (int i = 0; i < textInfo.materialCount; i++)
			{
				bool flag = textInfo.meshInfo[i].vertexCount == 0;
				if (flag)
				{
					break;
				}
				this.m_CurrentEntry.isTextEntry = true;
				this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
				this.m_CurrentEntry.isStencilClipped = this.m_StencilClip;
				MeshBuilder.MakeText(textInfo.meshInfo[i], textParams.rect.min, new MeshBuilder.AllocMeshData
				{
					alloc = this.m_AllocRawVertsIndicesDelegate
				});
				this.m_CurrentEntry.font = textInfo.meshInfo[i].material.mainTexture;
				this.m_Entries.Add(this.m_CurrentEntry);
				this.totalVertices += this.m_CurrentEntry.vertices.Length;
				this.totalIndices += this.m_CurrentEntry.indices.Length;
				this.m_CurrentEntry = default(UIRStylePainter.Entry);
			}
		}

		public void DrawRectangle(MeshGenerationContextUtils.RectangleParams rectParams)
		{
			MeshBuilder.AllocMeshData allocMeshData = new MeshBuilder.AllocMeshData
			{
				alloc = this.m_AllocThroughDrawMeshDelegate,
				texture = rectParams.texture,
				material = rectParams.material
			};
			bool flag = rectParams.vectorImage != null;
			if (flag)
			{
				this.DrawVectorImage(rectParams);
			}
			else
			{
				bool flag2 = rectParams.texture != null;
				if (flag2)
				{
					MeshBuilder.MakeTexturedRect(rectParams, 0.995f, allocMeshData);
				}
				else
				{
					MeshBuilder.MakeSolidRect(rectParams, 0.995f, allocMeshData);
				}
			}
		}

		public void DrawBorder(MeshGenerationContextUtils.BorderParams borderParams)
		{
			MeshBuilder.MakeBorder(borderParams, 0.995f, new MeshBuilder.AllocMeshData
			{
				alloc = this.m_AllocThroughDrawMeshDelegate,
				material = borderParams.material,
				texture = null,
				flags = MeshGenerationContext.MeshFlags.UVisDisplacement
			});
		}

		public void DrawImmediate(Action callback)
		{
			RenderChainCommand renderChainCommand = this.m_Owner.AllocCommand();
			renderChainCommand.type = CommandType.Immediate;
			renderChainCommand.owner = this.currentElement;
			renderChainCommand.callback = callback;
			this.m_Entries.Add(new UIRStylePainter.Entry
			{
				customCommand = renderChainCommand
			});
		}

		public VisualElement visualElement
		{
			get
			{
				return this.currentElement;
			}
		}

		public void DrawVisualElementBackground()
		{
			bool flag = this.currentElement.layout.width <= Mathf.Epsilon || this.currentElement.layout.height <= Mathf.Epsilon;
			if (!flag)
			{
				ComputedStyle computedStyle = this.currentElement.computedStyle;
				bool flag2 = computedStyle.backgroundColor != Color.clear;
				if (flag2)
				{
					MeshGenerationContextUtils.RectangleParams rectangleParams = new MeshGenerationContextUtils.RectangleParams
					{
						rect = GUIUtility.AlignRectToDevice(this.currentElement.rect),
						color = computedStyle.backgroundColor.value,
						playmodeTintColor = ((this.currentElement.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white)
					};
					MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out rectangleParams.topLeftRadius, out rectangleParams.bottomLeftRadius, out rectangleParams.topRightRadius, out rectangleParams.bottomRightRadius);
					this.DrawRectangle(rectangleParams);
				}
				Background value = computedStyle.backgroundImage.value;
				bool flag3 = value.texture != null || value.vectorImage != null;
				if (flag3)
				{
					MeshGenerationContextUtils.RectangleParams rectangleParams2 = default(MeshGenerationContextUtils.RectangleParams);
					bool flag4 = value.texture != null;
					if (flag4)
					{
						rectangleParams2 = MeshGenerationContextUtils.RectangleParams.MakeTextured(GUIUtility.AlignRectToDevice(this.currentElement.rect), new Rect(0f, 0f, 1f, 1f), value.texture, computedStyle.unityBackgroundScaleMode.value, this.currentElement.panel.contextType);
					}
					else
					{
						bool flag5 = value.vectorImage != null;
						if (flag5)
						{
							rectangleParams2 = MeshGenerationContextUtils.RectangleParams.MakeVectorTextured(GUIUtility.AlignRectToDevice(this.currentElement.rect), new Rect(0f, 0f, 1f, 1f), value.vectorImage, computedStyle.unityBackgroundScaleMode.value, this.currentElement.panel.contextType);
						}
					}
					MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out rectangleParams2.topLeftRadius, out rectangleParams2.bottomLeftRadius, out rectangleParams2.topRightRadius, out rectangleParams2.bottomRightRadius);
					rectangleParams2.leftSlice = computedStyle.unitySliceLeft.value;
					rectangleParams2.topSlice = computedStyle.unitySliceTop.value;
					rectangleParams2.rightSlice = computedStyle.unitySliceRight.value;
					rectangleParams2.bottomSlice = computedStyle.unitySliceBottom.value;
					bool flag6 = computedStyle.unityBackgroundImageTintColor != Color.clear;
					if (flag6)
					{
						rectangleParams2.color = computedStyle.unityBackgroundImageTintColor.value;
					}
					this.DrawRectangle(rectangleParams2);
				}
			}
		}

		public void DrawVisualElementBorder()
		{
			bool flag = this.currentElement.layout.width >= Mathf.Epsilon && this.currentElement.layout.height >= Mathf.Epsilon;
			if (flag)
			{
				ComputedStyle computedStyle = this.currentElement.computedStyle;
				bool flag2 = (computedStyle.borderLeftColor != Color.clear && computedStyle.borderLeftWidth.value > 0f) || (computedStyle.borderTopColor != Color.clear && computedStyle.borderTopWidth.value > 0f) || (computedStyle.borderRightColor != Color.clear && computedStyle.borderRightWidth.value > 0f) || (computedStyle.borderBottomColor != Color.clear && computedStyle.borderBottomWidth.value > 0f);
				if (flag2)
				{
					MeshGenerationContextUtils.BorderParams borderParams = new MeshGenerationContextUtils.BorderParams
					{
						rect = GUIUtility.AlignRectToDevice(this.currentElement.rect),
						leftColor = computedStyle.borderLeftColor.value,
						topColor = computedStyle.borderTopColor.value,
						rightColor = computedStyle.borderRightColor.value,
						bottomColor = computedStyle.borderBottomColor.value,
						leftWidth = computedStyle.borderLeftWidth.value,
						topWidth = computedStyle.borderTopWidth.value,
						rightWidth = computedStyle.borderRightWidth.value,
						bottomWidth = computedStyle.borderBottomWidth.value,
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
					bool flag3 = UIRUtility.IsVectorImageBackground(this.currentElement);
					if (flag3)
					{
						this.GenerateStencilClipEntryForSVGBackground();
					}
					else
					{
						this.GenerateStencilClipEntryForRoundedRectBackground();
					}
				}
			}
			this.m_ClipRectID = this.currentElement.renderChainData.clipRectID;
		}

		public void DrawVectorImage(MeshGenerationContextUtils.RectangleParams rectParams)
		{
			VectorImage vectorImage = rectParams.vectorImage;
			Debug.Assert(vectorImage != null);
			VertexFlags vertexFlags = ((vectorImage.atlas != null) ? VertexFlags.IsSVGGradients : VertexFlags.IsSolid);
			int num = 0;
			bool flag = vectorImage.atlas != null && this.m_VectorImageManager != null;
			if (flag)
			{
				GradientRemap gradientRemap = this.m_VectorImageManager.AddUser(vectorImage);
				vertexFlags = (gradientRemap.isAtlassed ? VertexFlags.IsSVGGradients : VertexFlags.IsCustomSVGGradients);
				num = gradientRemap.destIndex;
			}
			int count = this.m_Entries.Count;
			MeshGenerationContext.MeshFlags meshFlags = MeshGenerationContext.MeshFlags.None;
			bool flag2 = vertexFlags == VertexFlags.IsSVGGradients;
			if (flag2)
			{
				meshFlags = MeshGenerationContext.MeshFlags.IsSVGGradients;
			}
			else
			{
				bool flag3 = vertexFlags == VertexFlags.IsCustomSVGGradients;
				if (flag3)
				{
					meshFlags = MeshGenerationContext.MeshFlags.IsCustomSVGGradients;
				}
			}
			MeshBuilder.AllocMeshData allocMeshData = new MeshBuilder.AllocMeshData
			{
				alloc = this.m_AllocThroughDrawMeshDelegate,
				texture = ((vertexFlags == VertexFlags.IsCustomSVGGradients) ? vectorImage.atlas : null),
				flags = meshFlags
			};
			int num2;
			int num3;
			MeshBuilder.MakeVectorGraphics(rectParams, num, allocMeshData, out num2, out num3);
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

		internal void Reset()
		{
			bool disposed = this.disposed;
			if (disposed)
			{
				DisposeHelper.NotifyDisposedUsed(this);
			}
			else
			{
				this.m_Entries.Clear();
				this.m_VertsPool.SessionDone();
				this.m_IndicesPool.SessionDone();
				this.m_ClosingInfo = default(UIRStylePainter.ClosingInfo);
				this.m_NextMeshWriteDataPoolItem = 0;
				this.currentElement = null;
				this.totalVertices = (this.totalIndices = 0);
			}
		}

		private void GenerateStencilClipEntryForRoundedRectBackground()
		{
			bool flag = this.currentElement.layout.width <= Mathf.Epsilon || this.currentElement.layout.height <= Mathf.Epsilon;
			if (!flag)
			{
				ComputedStyle computedStyle = this.currentElement.computedStyle;
				Vector2 vector;
				Vector2 vector2;
				Vector2 vector3;
				Vector2 vector4;
				MeshGenerationContextUtils.GetVisualElementRadii(this.currentElement, out vector, out vector2, out vector3, out vector4);
				float value = computedStyle.borderTopWidth.value;
				float value2 = computedStyle.borderLeftWidth.value;
				float value3 = computedStyle.borderBottomWidth.value;
				float value4 = computedStyle.borderRightWidth.value;
				MeshGenerationContextUtils.RectangleParams rectangleParams = new MeshGenerationContextUtils.RectangleParams
				{
					rect = GUIUtility.AlignRectToDevice(this.currentElement.rect),
					color = Color.white,
					topLeftRadius = Vector2.Max(Vector2.zero, vector - new Vector2(value2, value)),
					topRightRadius = Vector2.Max(Vector2.zero, vector3 - new Vector2(value4, value)),
					bottomLeftRadius = Vector2.Max(Vector2.zero, vector2 - new Vector2(value2, value3)),
					bottomRightRadius = Vector2.Max(Vector2.zero, vector4 - new Vector2(value4, value3)),
					playmodeTintColor = ((this.currentElement.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white)
				};
				rectangleParams.rect.x = rectangleParams.rect.x + value2;
				rectangleParams.rect.y = rectangleParams.rect.y + value;
				rectangleParams.rect.width = rectangleParams.rect.width - (value2 + value4);
				rectangleParams.rect.height = rectangleParams.rect.height - (value + value3);
				bool flag2 = computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox;
				if (flag2)
				{
					rectangleParams.rect.x = rectangleParams.rect.x + computedStyle.paddingLeft.value.value;
					rectangleParams.rect.y = rectangleParams.rect.y + computedStyle.paddingTop.value.value;
					rectangleParams.rect.width = rectangleParams.rect.width - (computedStyle.paddingLeft.value.value + computedStyle.paddingRight.value.value);
					rectangleParams.rect.height = rectangleParams.rect.height - (computedStyle.paddingTop.value.value + computedStyle.paddingBottom.value.value);
				}
				this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
				this.m_CurrentEntry.isStencilClipped = this.m_StencilClip;
				this.m_CurrentEntry.isClipRegisterEntry = true;
				MeshBuilder.MakeSolidRect(rectangleParams, -0.995f, new MeshBuilder.AllocMeshData
				{
					alloc = this.m_AllocRawVertsIndicesDelegate
				});
				bool flag3 = this.m_CurrentEntry.vertices.Length > 0 && this.m_CurrentEntry.indices.Length > 0;
				if (flag3)
				{
					this.m_Entries.Add(this.m_CurrentEntry);
					this.totalVertices += this.m_CurrentEntry.vertices.Length;
					this.totalIndices += this.m_CurrentEntry.indices.Length;
					this.m_StencilClip = true;
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
				this.m_StencilClip = true;
				this.m_CurrentEntry.vertices = entry.vertices;
				this.m_CurrentEntry.indices = entry.indices;
				this.m_CurrentEntry.uvIsDisplacement = entry.uvIsDisplacement;
				this.m_CurrentEntry.clipRectID = this.m_ClipRectID;
				this.m_CurrentEntry.isStencilClipped = this.m_StencilClip;
				this.m_CurrentEntry.isClipRegisterEntry = true;
				this.m_ClosingInfo.needsClosing = true;
				int length = this.m_CurrentEntry.vertices.Length;
				NativeSlice<Vertex> nativeSlice = this.m_VertsPool.Alloc((uint)length);
				for (int i = 0; i < length; i++)
				{
					Vertex vertex = this.m_CurrentEntry.vertices[i];
					vertex.position.z = -0.995f;
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

		private UIRAtlasManager m_AtlasManager;

		private VectorImageManager m_VectorImageManager;

		private UIRStylePainter.Entry m_CurrentEntry;

		private UIRStylePainter.ClosingInfo m_ClosingInfo;

		private bool m_StencilClip = false;

		private BMPAlloc m_ClipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;

		private int m_SVGBackgroundEntryIndex = -1;

		private UIRStylePainter.TempDataAlloc<Vertex> m_VertsPool = new UIRStylePainter.TempDataAlloc<Vertex>(8192);

		private UIRStylePainter.TempDataAlloc<ushort> m_IndicesPool = new UIRStylePainter.TempDataAlloc<ushort>(16384);

		private List<MeshWriteData> m_MeshWriteDataPool;

		private int m_NextMeshWriteDataPoolItem;

		private MeshBuilder.AllocMeshData.Allocator m_AllocRawVertsIndicesDelegate;

		private MeshBuilder.AllocMeshData.Allocator m_AllocThroughDrawMeshDelegate;

		internal struct Entry
		{
			public NativeSlice<Vertex> vertices;

			public NativeSlice<ushort> indices;

			public Material material;

			public Texture custom;

			public Texture font;

			public RenderChainCommand customCommand;

			public BMPAlloc clipRectID;

			public VertexFlags addFlags;

			public bool uvIsDisplacement;

			public bool isTextEntry;

			public bool isClipRegisterEntry;

			public bool isStencilClipped;
		}

		internal struct ClosingInfo
		{
			public bool needsClosing;

			public bool popViewMatrix;

			public bool popScissorClip;

			public RenderChainCommand clipUnregisterDrawCommand;

			public NativeSlice<Vertex> clipperRegisterVertices;

			public NativeSlice<ushort> clipperRegisterIndices;

			public int clipperRegisterIndexOffset;
		}

		internal struct TempDataAlloc<T> : IDisposable where T : struct
		{
			public TempDataAlloc(int maxPoolElems)
			{
				this.maxPoolElemCount = maxPoolElems;
				this.pool = default(NativeArray<T>);
				this.excess = new List<NativeArray<T>>();
				this.takenFromPool = 0U;
			}

			public void Dispose()
			{
				foreach (NativeArray<T> nativeArray in this.excess)
				{
					nativeArray.Dispose();
				}
				this.excess.Clear();
				bool isCreated = this.pool.IsCreated;
				if (isCreated)
				{
					this.pool.Dispose();
				}
			}

			internal NativeSlice<T> Alloc(uint count)
			{
				bool flag = (ulong)(this.takenFromPool + count) <= (ulong)((long)this.pool.Length);
				NativeSlice<T> nativeSlice2;
				if (flag)
				{
					NativeSlice<T> nativeSlice = this.pool.Slice<T>((int)this.takenFromPool, (int)count);
					this.takenFromPool += count;
					nativeSlice2 = nativeSlice;
				}
				else
				{
					NativeArray<T> nativeArray = new NativeArray<T>((int)count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
					this.excess.Add(nativeArray);
					nativeSlice2 = nativeArray;
				}
				return nativeSlice2;
			}

			internal void SessionDone()
			{
				int num = this.pool.Length;
				foreach (NativeArray<T> nativeArray in this.excess)
				{
					bool flag = nativeArray.Length < this.maxPoolElemCount;
					if (flag)
					{
						num += nativeArray.Length;
					}
					nativeArray.Dispose();
				}
				this.excess.Clear();
				bool flag2 = num > this.pool.Length;
				if (flag2)
				{
					bool isCreated = this.pool.IsCreated;
					if (isCreated)
					{
						this.pool.Dispose();
					}
					this.pool = new NativeArray<T>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				}
				this.takenFromPool = 0U;
			}

			private int maxPoolElemCount;

			private NativeArray<T> pool;

			private List<NativeArray<T>> excess;

			private uint takenFromPool;
		}
	}
}
