using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class EntryProcessor
	{
		public RenderChainCommand firstHeadCommand { get; private set; }

		public RenderChainCommand lastHeadCommand { get; private set; }

		public RenderChainCommand firstTailCommand { get; private set; }

		public RenderChainCommand lastTailCommand { get; private set; }

		public void Init(Entry root, RenderTreeManager renderTreeManager, RenderData renderData)
		{
			UIRenderDevice device = renderTreeManager.device;
			this.m_RenderTreeManager = renderTreeManager;
			this.m_CurrentRenderData = renderData;
			this.m_PreProcessor.PreProcess(root);
			bool flag = this.m_PreProcessor.headAllocs.Count == 0 && renderData.headMesh != null;
			if (flag)
			{
				device.Free(renderData.headMesh);
				renderData.headMesh = null;
			}
			bool flag2 = this.m_PreProcessor.tailAllocs.Count == 0 && renderData.tailMesh != null;
			if (flag2)
			{
				device.Free(renderData.tailMesh);
				renderData.tailMesh = null;
			}
			bool hasExtraMeshes = renderData.hasExtraMeshes;
			if (hasExtraMeshes)
			{
				renderTreeManager.FreeExtraMeshes(renderData);
			}
			renderTreeManager.ResetGraphicEntries(renderData);
			RenderData parent = renderData.parent;
			bool isGroupTransform = renderData.isGroupTransform;
			bool flag3 = parent != null;
			if (flag3)
			{
				this.m_MaskDepthPopped = parent.childrenMaskDepth;
				this.m_StencilRefPopped = parent.childrenStencilRef;
				this.m_ClipRectIdPopped = (isGroupTransform ? UIRVEShaderInfoAllocator.infiniteClipRect : parent.clipRectID);
			}
			else
			{
				this.m_MaskDepthPopped = 0;
				this.m_StencilRefPopped = 0;
				this.m_ClipRectIdPopped = UIRVEShaderInfoAllocator.infiniteClipRect;
			}
			this.m_MaskDepthPushed = this.m_MaskDepthPopped + 1;
			this.m_StencilRefPushed = this.m_MaskDepthPopped;
			this.m_ClipRectIdPushed = renderData.clipRectID;
			this.m_MaskDepth = this.m_MaskDepthPopped;
			this.m_StencilRef = this.m_StencilRefPopped;
			this.m_ClipRectId = this.m_ClipRectIdPopped;
			this.m_VertexDataComputed = false;
			this.m_Transform = Matrix4x4.identity;
			this.m_TextCoreSettingsPage = new Color32(0, 0, 0, 0);
			this.m_MaskMeshes.Clear();
			this.m_IsDrawingMask = false;
		}

		public void ClearReferences()
		{
			this.m_PreProcessor.ClearReferences();
			this.m_RenderTreeManager = null;
			this.m_CurrentRenderData = null;
			this.m_Mesh = null;
			this.m_FirstCommand = null;
			this.m_LastCommand = null;
			this.firstHeadCommand = null;
			this.lastHeadCommand = null;
			this.firstTailCommand = null;
			this.lastTailCommand = null;
		}

		public void ProcessHead()
		{
			this.m_IsTail = false;
			this.ProcessFirstAlloc(this.m_PreProcessor.headAllocs, ref this.m_CurrentRenderData.headMesh);
			this.m_FirstCommand = null;
			this.m_LastCommand = null;
			this.ProcessRange(0, this.m_PreProcessor.childrenIndex - 1);
			this.firstHeadCommand = this.m_FirstCommand;
			this.lastHeadCommand = this.m_LastCommand;
		}

		public void ProcessTail()
		{
			this.m_IsTail = true;
			this.ProcessFirstAlloc(this.m_PreProcessor.tailAllocs, ref this.m_CurrentRenderData.tailMesh);
			this.m_FirstCommand = null;
			this.m_LastCommand = null;
			this.ProcessRange(this.m_PreProcessor.childrenIndex + 1, this.m_PreProcessor.flattenedEntries.Count - 1);
			this.firstTailCommand = this.m_FirstCommand;
			this.lastTailCommand = this.m_LastCommand;
			Debug.Assert(this.m_MaskDepth == this.m_MaskDepthPopped);
			Debug.Assert(this.m_MaskMeshes.Count == 0);
			Debug.Assert(!this.m_IsDrawingMask);
		}

		private void ProcessRange(int first, int last)
		{
			List<Entry> flattenedEntries = this.m_PreProcessor.flattenedEntries;
			for (int i = first; i <= last; i++)
			{
				Entry entry = flattenedEntries[i];
				switch (entry.type)
				{
				case EntryType.DrawSolidMesh:
					this.m_RenderType = VertexFlags.IsSolid;
					this.ProcessMeshEntry(entry, TextureId.invalid);
					break;
				case EntryType.DrawTexturedMesh:
				{
					Texture texture = entry.texture;
					TextureId textureId = TextureId.invalid;
					bool flag = texture != null;
					if (flag)
					{
						RectInt rectInt;
						bool flag2 = this.m_RenderTreeManager.atlas != null && this.m_RenderTreeManager.atlas.TryGetAtlas(this.m_CurrentRenderData.owner, texture as Texture2D, out textureId, out rectInt);
						if (flag2)
						{
							this.m_RenderType = VertexFlags.IsDynamic;
							this.m_AtlasRect = new Rect((float)rectInt.x, (float)rectInt.y, (float)rectInt.width, (float)rectInt.height);
							this.m_RemapUVs = true;
							this.m_RenderTreeManager.InsertTexture(this.m_CurrentRenderData, texture, textureId, true);
						}
						else
						{
							this.m_RenderType = VertexFlags.IsTextured;
							textureId = TextureRegistry.instance.Acquire(texture);
							this.m_RenderTreeManager.InsertTexture(this.m_CurrentRenderData, texture, textureId, false);
						}
					}
					else
					{
						this.m_RenderType = VertexFlags.IsSolid;
					}
					this.ProcessMeshEntry(entry, textureId);
					this.m_RemapUVs = false;
					break;
				}
				case EntryType.DrawTexturedMeshSkipAtlas:
				{
					this.m_RenderType = VertexFlags.IsTextured;
					TextureId textureId2 = TextureRegistry.instance.Acquire(entry.texture);
					this.m_RenderTreeManager.InsertTexture(this.m_CurrentRenderData, entry.texture, textureId2, false);
					this.ProcessMeshEntry(entry, textureId2);
					break;
				}
				case EntryType.DrawDynamicTexturedMesh:
					this.m_RenderType = VertexFlags.IsTextured;
					this.ProcessMeshEntry(entry, entry.textureId);
					break;
				case EntryType.DrawTextMesh:
				{
					this.m_RenderType = VertexFlags.IsText;
					TextureId textureId3 = TextureRegistry.instance.Acquire(entry.texture);
					this.m_RenderTreeManager.InsertTexture(this.m_CurrentRenderData, entry.texture, textureId3, false);
					this.ProcessMeshEntry(entry, textureId3);
					break;
				}
				case EntryType.DrawGradients:
				{
					this.m_RenderType = VertexFlags.IsSvgGradients;
					this.m_RenderTreeManager.InsertVectorImage(this.m_CurrentRenderData, entry.gradientsOwner);
					GradientRemap gradientRemap = this.m_RenderTreeManager.vectorImageManager.AddUser(entry.gradientsOwner, this.m_CurrentRenderData.owner);
					this.m_GradientSettingIndexOffset = gradientRemap.destIndex;
					bool flag3 = gradientRemap.atlas != TextureId.invalid;
					TextureId textureId4;
					if (flag3)
					{
						textureId4 = gradientRemap.atlas;
					}
					else
					{
						textureId4 = TextureRegistry.instance.Acquire(entry.gradientsOwner.atlas);
						this.m_RenderTreeManager.InsertTexture(this.m_CurrentRenderData, entry.gradientsOwner.atlas, textureId4, false);
					}
					this.ProcessMeshEntry(entry, textureId4);
					this.m_GradientSettingIndexOffset = -1;
					break;
				}
				case EntryType.DrawImmediate:
				{
					RenderChainCommand renderChainCommand = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand.type = CommandType.Immediate;
					renderChainCommand.owner = this.m_CurrentRenderData;
					renderChainCommand.callback = entry.immediateCallback;
					this.AppendCommand(renderChainCommand);
					break;
				}
				case EntryType.DrawImmediateCull:
				{
					RenderChainCommand renderChainCommand2 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand2.type = CommandType.ImmediateCull;
					renderChainCommand2.owner = this.m_CurrentRenderData;
					renderChainCommand2.callback = entry.immediateCallback;
					this.AppendCommand(renderChainCommand2);
					break;
				}
				case EntryType.DrawChildren:
				case EntryType.DedicatedPlaceholder:
					Debug.Assert(false);
					break;
				case EntryType.BeginStencilMask:
					Debug.Assert(this.m_MaskDepth == this.m_MaskDepthPopped);
					Debug.Assert(!this.m_IsDrawingMask);
					this.m_IsDrawingMask = true;
					this.m_StencilRef = this.m_StencilRefPushed;
					Debug.Assert(this.m_MaskDepth == this.m_StencilRef);
					break;
				case EntryType.EndStencilMask:
					Debug.Assert(this.m_IsDrawingMask);
					this.m_IsDrawingMask = false;
					this.m_MaskDepth = this.m_MaskDepthPushed;
					break;
				case EntryType.PopStencilMask:
					Debug.Assert(this.m_MaskDepth == this.m_StencilRef + 1);
					this.DrawReverseMask();
					this.m_MaskDepth = this.m_MaskDepthPopped;
					this.m_StencilRef = this.m_StencilRefPopped;
					break;
				case EntryType.PushClippingRect:
					this.m_ClipRectId = this.m_ClipRectIdPushed;
					break;
				case EntryType.PopClippingRect:
					this.m_ClipRectId = this.m_ClipRectIdPopped;
					break;
				case EntryType.PushScissors:
				{
					RenderChainCommand renderChainCommand3 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand3.type = CommandType.PushScissor;
					renderChainCommand3.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand3);
					break;
				}
				case EntryType.PopScissors:
				{
					RenderChainCommand renderChainCommand4 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand4.type = CommandType.PopScissor;
					renderChainCommand4.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand4);
					break;
				}
				case EntryType.PushGroupMatrix:
				{
					RenderChainCommand renderChainCommand5 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand5.type = CommandType.PushView;
					renderChainCommand5.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand5);
					break;
				}
				case EntryType.PopGroupMatrix:
				{
					RenderChainCommand renderChainCommand6 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand6.type = CommandType.PopView;
					renderChainCommand6.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand6);
					break;
				}
				case EntryType.PushDefaultMaterial:
				{
					RenderChainCommand renderChainCommand7 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand7.type = CommandType.PushDefaultMaterial;
					renderChainCommand7.owner = this.m_CurrentRenderData;
					renderChainCommand7.material = entry.material;
					renderChainCommand7.userProps = entry.userProps;
					this.AppendCommand(renderChainCommand7);
					break;
				}
				case EntryType.PopDefaultMaterial:
				{
					RenderChainCommand renderChainCommand8 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand8.type = CommandType.PopDefaultMaterial;
					renderChainCommand8.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand8);
					break;
				}
				case EntryType.CutRenderChain:
				{
					RenderChainCommand renderChainCommand9 = this.m_RenderTreeManager.AllocCommand();
					renderChainCommand9.type = CommandType.CutRenderChain;
					renderChainCommand9.owner = this.m_CurrentRenderData;
					this.AppendCommand(renderChainCommand9);
					break;
				}
				default:
					throw new NotImplementedException();
				}
			}
		}

		private void ProcessMeshEntry(Entry entry, TextureId textureId)
		{
			int length = entry.vertices.Length;
			int length2 = entry.indices.Length;
			Debug.Assert(length > 0 == length2 > 0);
			bool flag = length > 0 && length2 > 0;
			if (flag)
			{
				bool flag2 = this.m_VertsFilled + length > this.m_AllocVertexCount;
				if (flag2)
				{
					this.ProcessNextAlloc();
					Debug.Assert(this.m_VertsFilled + length <= this.m_AllocVertexCount);
				}
				bool flag3 = !this.m_VertexDataComputed;
				if (flag3)
				{
					UIRUtility.GetVerticesTransformInfo(this.m_CurrentRenderData, out this.m_Transform);
					this.m_CurrentRenderData.verticesSpace = this.m_Transform;
					this.m_TransformData = this.m_RenderTreeManager.shaderInfoAllocator.TransformAllocToVertexData(this.m_CurrentRenderData.transformID);
					this.m_OpacityData = this.m_RenderTreeManager.shaderInfoAllocator.OpacityAllocToVertexData(this.m_CurrentRenderData.opacityID);
					this.m_VertexDataComputed = true;
				}
				Color32 color = new Color32(this.m_OpacityData.r, this.m_OpacityData.g, 0, 0);
				Color32 color2 = this.m_RenderTreeManager.shaderInfoAllocator.ClipRectAllocToVertexData(this.m_ClipRectId);
				Color32 color3 = new Color32(this.m_TransformData.b, color2.b, this.m_OpacityData.b, 0);
				Color32 color4 = new Color32(this.m_TransformData.r, this.m_TransformData.g, color2.r, color2.g);
				Color32 color5 = new Color32((byte)this.m_RenderType, 0, 0, 0);
				bool flag4 = (entry.flags & EntryFlags.UsesTextCoreSettings) > (EntryFlags)0;
				if (flag4)
				{
					Color32 color6 = this.m_RenderTreeManager.shaderInfoAllocator.TextCoreSettingsToVertexData(this.m_CurrentRenderData.textCoreSettingsID);
					this.m_TextCoreSettingsPage.r = color6.r;
					this.m_TextCoreSettingsPage.g = color6.g;
					color3.a = color6.b;
				}
				NativeSlice<Vertex> nativeSlice = this.m_Verts.Slice<Vertex>(this.m_VertsFilled, length);
				int num = this.m_VertsFilled + (int)this.m_IndexOffset;
				NativeSlice<ushort> nativeSlice2 = this.m_Indices.Slice<ushort>(this.m_IndicesFilled, length2);
				bool flag5 = UIRUtility.ShapeWindingIsClockwise(this.m_MaskDepth, this.m_StencilRef);
				bool worldFlipsWinding = this.m_CurrentRenderData.worldFlipsWinding;
				bool flag6 = entry.material != null;
				Material material;
				if (flag6)
				{
					material = entry.material;
				}
				else
				{
					material = this.m_CurrentRenderData.owner.resolvedStyle.unityMaterial.material;
				}
				ConvertMeshJobData convertMeshJobData = new ConvertMeshJobData
				{
					vertSrc = (IntPtr)entry.vertices.GetUnsafePtr<Vertex>(),
					vertDst = (IntPtr)nativeSlice.GetUnsafePtr<Vertex>(),
					vertCount = length,
					transform = this.m_Transform,
					xformClipPages = color4,
					ids = color3,
					addFlags = color5,
					opacityPage = color,
					textCoreSettingsPage = this.m_TextCoreSettingsPage,
					usesTextCoreSettings = (((entry.flags & EntryFlags.UsesTextCoreSettings) != (EntryFlags)0) ? 1 : 0),
					textureId = textureId.ConvertToGpu(),
					gradientSettingsIndexOffset = this.m_GradientSettingIndexOffset,
					indexSrc = (IntPtr)entry.indices.GetUnsafePtr<ushort>(),
					indexDst = (IntPtr)nativeSlice2.GetUnsafePtr<ushort>(),
					indexCount = nativeSlice2.Length,
					indexOffset = num,
					flipIndices = ((flag5 == worldFlipsWinding) ? 1 : 0),
					forceZ = (this.m_RenderTreeManager.isFlat ? 1 : 0),
					positionZ = (this.m_IsDrawingMask ? 1f : 0f),
					remapUVs = (this.m_RemapUVs ? 1 : 0),
					atlasRect = this.m_AtlasRect,
					layoutSize = ((material != null) ? new Vector2(this.m_CurrentRenderData.owner.layout.width, this.m_CurrentRenderData.owner.layout.height) : new Vector2(0f, 0f))
				};
				this.m_RenderTreeManager.jobManager.Add(ref convertMeshJobData);
				bool isDrawingMask = this.m_IsDrawingMask;
				if (isDrawingMask)
				{
					this.m_MaskMeshes.Push(new EntryProcessor.MaskMesh
					{
						vertices = nativeSlice,
						indices = nativeSlice2,
						indexOffset = num
					});
				}
				RenderChainCommand renderChainCommand = this.CreateMeshDrawCommand(this.m_Mesh, length2, this.m_IndicesFilled, entry.material, textureId);
				this.AppendCommand(renderChainCommand);
				bool flag7 = entry.type == EntryType.DrawTextMesh;
				if (flag7)
				{
					renderChainCommand.sdfScale = entry.textScale;
					renderChainCommand.sharpness = entry.fontSharpness;
				}
				bool flag8 = (entry.flags & EntryFlags.IsPremultiplied) > (EntryFlags)0;
				if (flag8)
				{
					renderChainCommand.flags |= CommandFlags.IsPremultiplied;
				}
				this.m_VertsFilled += length;
				this.m_IndicesFilled += length2;
			}
		}

		private void DrawReverseMask()
		{
			for (;;)
			{
				EntryProcessor.MaskMesh maskMesh;
				bool flag = this.m_MaskMeshes.TryPop(out maskMesh);
				if (!flag)
				{
					break;
				}
				Debug.Assert(maskMesh.indices.Length > 0 == maskMesh.vertices.Length > 0);
				bool flag2 = maskMesh.indices.Length > 0 && maskMesh.vertices.Length > 0;
				if (flag2)
				{
					RenderChainCommand renderChainCommand = this.CreateMeshDrawCommand(this.m_Mesh, maskMesh.indices.Length, this.m_IndicesFilled, null, TextureId.invalid);
					this.AppendCommand(renderChainCommand);
					NativeSlice<Vertex> nativeSlice = this.m_Verts.Slice<Vertex>(this.m_VertsFilled, maskMesh.vertices.Length);
					NativeSlice<ushort> nativeSlice2 = this.m_Indices.Slice<ushort>(this.m_IndicesFilled, maskMesh.indices.Length);
					CopyMeshJobData copyMeshJobData = new CopyMeshJobData
					{
						vertSrc = (IntPtr)maskMesh.vertices.GetUnsafePtr<Vertex>(),
						vertDst = (IntPtr)nativeSlice.GetUnsafePtr<Vertex>(),
						vertCount = maskMesh.vertices.Length,
						indexSrc = (IntPtr)maskMesh.indices.GetUnsafePtr<ushort>(),
						indexDst = (IntPtr)nativeSlice2.GetUnsafePtr<ushort>(),
						indexCount = maskMesh.indices.Length,
						indexOffset = (int)this.m_IndexOffset + this.m_VertsFilled - maskMesh.indexOffset
					};
					this.m_RenderTreeManager.jobManager.Add(ref copyMeshJobData);
					this.m_IndicesFilled += maskMesh.indices.Length;
					this.m_VertsFilled += maskMesh.vertices.Length;
				}
			}
		}

		private RenderChainCommand CreateMeshDrawCommand(MeshHandle mesh, int indexCount, int indexOffset, Material material, TextureId texture)
		{
			RenderChainCommand renderChainCommand = this.m_RenderTreeManager.AllocCommand();
			renderChainCommand.type = CommandType.Draw;
			renderChainCommand.material = material;
			renderChainCommand.texture = texture;
			renderChainCommand.stencilRef = this.m_StencilRef;
			renderChainCommand.mesh = mesh;
			renderChainCommand.indexOffset = indexOffset;
			renderChainCommand.indexCount = indexCount;
			renderChainCommand.owner = this.m_CurrentRenderData;
			bool flag = (this.m_CurrentRenderData.owner.renderHints & RenderHints.LargePixelCoverage) > RenderHints.None;
			if (flag)
			{
				switch (this.m_RenderType)
				{
				case VertexFlags.IsSolid:
					renderChainCommand.flags |= CommandFlags.ForceRenderTypeSolid;
					break;
				case VertexFlags.IsText:
					renderChainCommand.flags |= CommandFlags.ForceRenderTypeText;
					break;
				case VertexFlags.IsTextured:
				case VertexFlags.IsDynamic:
					renderChainCommand.flags |= CommandFlags.ForceRenderTypeTextured;
					break;
				case VertexFlags.IsSvgGradients:
					renderChainCommand.flags |= CommandFlags.ForceRenderTypeSvgGradient;
					break;
				default:
					Debug.LogError(string.Format("Unknown Render Type '{0}'", this.m_RenderType));
					break;
				}
				renderChainCommand.flags |= CommandFlags.ForceSingleTextureSlot;
			}
			return renderChainCommand;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendCommand(RenderChainCommand next)
		{
			bool flag = this.m_FirstCommand == null;
			if (flag)
			{
				this.m_FirstCommand = next;
				this.m_LastCommand = next;
			}
			else
			{
				next.prev = this.m_LastCommand;
				this.m_LastCommand.next = next;
				this.m_LastCommand = next;
			}
		}

		private void ProcessFirstAlloc(List<EntryPreProcessor.AllocSize> allocList, ref MeshHandle mesh)
		{
			bool flag = allocList.Count > 0;
			if (flag)
			{
				EntryPreProcessor.AllocSize allocSize = allocList[0];
				EntryProcessor.UpdateOrAllocate(ref mesh, allocSize.vertexCount, allocSize.indexCount, this.m_RenderTreeManager.device, out this.m_Verts, out this.m_Indices, out this.m_IndexOffset, this.m_RenderTreeManager.statsByRef);
				this.m_AllocVertexCount = (int)mesh.allocVerts.size;
			}
			else
			{
				Debug.Assert(mesh == null);
				this.m_Verts = default(NativeSlice<Vertex>);
				this.m_Indices = default(NativeSlice<ushort>);
				this.m_IndexOffset = 0;
				this.m_AllocVertexCount = 0;
			}
			this.m_Mesh = mesh;
			this.m_VertsFilled = 0;
			this.m_IndicesFilled = 0;
			this.m_AllocIndex = 0;
		}

		private void ProcessNextAlloc()
		{
			List<EntryPreProcessor.AllocSize> list = (this.m_IsTail ? this.m_PreProcessor.tailAllocs : this.m_PreProcessor.headAllocs);
			Debug.Assert(this.m_AllocIndex < list.Count - 1);
			List<EntryPreProcessor.AllocSize> list2 = list;
			int num = this.m_AllocIndex + 1;
			this.m_AllocIndex = num;
			EntryPreProcessor.AllocSize allocSize = list2[num];
			this.m_Mesh = null;
			EntryProcessor.UpdateOrAllocate(ref this.m_Mesh, allocSize.vertexCount, allocSize.indexCount, this.m_RenderTreeManager.device, out this.m_Verts, out this.m_Indices, out this.m_IndexOffset, this.m_RenderTreeManager.statsByRef);
			this.m_AllocVertexCount = (int)this.m_Mesh.allocVerts.size;
			this.m_RenderTreeManager.InsertExtraMesh(this.m_CurrentRenderData, this.m_Mesh);
			this.m_VertsFilled = 0;
			this.m_IndicesFilled = 0;
		}

		private static void UpdateOrAllocate(ref MeshHandle data, int vertexCount, int indexCount, UIRenderDevice device, out NativeSlice<Vertex> verts, out NativeSlice<ushort> indices, out ushort indexOffset, ref ChainBuilderStats stats)
		{
			bool flag = data != null;
			if (flag)
			{
				bool flag2 = (ulong)data.allocVerts.size >= (ulong)((long)vertexCount) && (ulong)data.allocIndices.size >= (ulong)((long)indexCount);
				if (flag2)
				{
					device.Update(data, (uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
					stats.updatedMeshAllocations += 1U;
				}
				else
				{
					device.Free(data);
					data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
					stats.newMeshAllocations += 1U;
				}
			}
			else
			{
				data = device.Allocate((uint)vertexCount, (uint)indexCount, out verts, out indices, out indexOffset);
				stats.newMeshAllocations += 1U;
			}
		}

		private EntryPreProcessor m_PreProcessor = new EntryPreProcessor();

		private RenderTreeManager m_RenderTreeManager;

		private RenderData m_CurrentRenderData;

		private int m_MaskDepth;

		private int m_MaskDepthPopped;

		private int m_MaskDepthPushed;

		private int m_StencilRef;

		private int m_StencilRefPopped;

		private int m_StencilRefPushed;

		private BMPAlloc m_ClipRectId;

		private BMPAlloc m_ClipRectIdPopped;

		private BMPAlloc m_ClipRectIdPushed;

		private bool m_IsDrawingMask;

		private Stack<EntryProcessor.MaskMesh> m_MaskMeshes = new Stack<EntryProcessor.MaskMesh>(1);

		private bool m_VertexDataComputed;

		private Matrix4x4 m_Transform;

		private Color32 m_TransformData;

		private Color32 m_OpacityData;

		private Color32 m_TextCoreSettingsPage;

		private MeshHandle m_Mesh;

		private NativeSlice<Vertex> m_Verts;

		private NativeSlice<ushort> m_Indices;

		private ushort m_IndexOffset;

		private int m_AllocVertexCount;

		private int m_AllocIndex;

		private int m_VertsFilled;

		private int m_IndicesFilled;

		private VertexFlags m_RenderType;

		private bool m_RemapUVs;

		private Rect m_AtlasRect;

		private int m_GradientSettingIndexOffset;

		private bool m_IsTail;

		private RenderChainCommand m_FirstCommand;

		private RenderChainCommand m_LastCommand;

		private struct MaskMesh
		{
			public NativeSlice<Vertex> vertices;

			public NativeSlice<ushort> indices;

			public int indexOffset;
		}
	}
}
