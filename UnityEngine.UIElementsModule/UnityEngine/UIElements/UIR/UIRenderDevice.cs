using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Rendering;

namespace UnityEngine.UIElements.UIR
{
	internal class UIRenderDevice : IDisposable
	{
		internal static uint maxVerticesPerPage
		{
			get
			{
				return 65535U;
			}
		}

		internal bool breakBatches { get; set; }

		internal bool isFlat { get; }

		internal bool forceGammaRendering { get; }

		internal uint frameIndex
		{
			get
			{
				return this.m_FrameIndex;
			}
		}

		static UIRenderDevice()
		{
			Utility.EngineUpdate += UIRenderDevice.OnEngineUpdateGlobal;
			Utility.FlushPendingResources += UIRenderDevice.OnFlushPendingResources;
		}

		public UIRenderDevice(uint initialVertexCapacity = 0U, uint initialIndexCapacity = 0U, bool isFlat = true, bool forceGammaRendering = false)
		{
			Debug.Assert(!UIRenderDevice.m_SynchronousFree);
			Debug.Assert(true);
			bool flag = UIRenderDevice.m_ActiveDeviceCount++ == 0;
			if (flag)
			{
				bool flag2 = !UIRenderDevice.m_SubscribedToNotifications;
				if (flag2)
				{
					Utility.NotifyOfUIREvents(true);
					UIRenderDevice.m_SubscribedToNotifications = true;
				}
			}
			this.isFlat = isFlat;
			this.forceGammaRendering = forceGammaRendering;
			this.m_NextPageVertexCount = Math.Max(initialVertexCapacity / 2U, 2048U);
			this.m_LargeMeshVertexCount = this.m_NextPageVertexCount;
			this.m_IndexToVertexCountRatio = initialIndexCapacity / initialVertexCapacity;
			this.m_IndexToVertexCountRatio = Mathf.Max(this.m_IndexToVertexCountRatio, 2f);
			this.m_DeferredFrees = new List<List<UIRenderDevice.AllocToFree>>(4);
			this.m_Updates = new List<List<UIRenderDevice.AllocToUpdate>>(4);
			int num = 0;
			while ((long)num < 4L)
			{
				this.m_DeferredFrees.Add(new List<UIRenderDevice.AllocToFree>());
				this.m_Updates.Add(new List<UIRenderDevice.AllocToUpdate>());
				num++;
			}
			this.InitVertexDeclaration();
			this.m_Fences = new uint[4];
			this.m_ConstantProps = new MaterialPropertyBlock();
			this.m_BatchProps = new MaterialPropertyBlock();
			this.m_DefaultStencilState = Utility.CreateStencilState(new StencilState
			{
				enabled = isFlat,
				readMask = byte.MaxValue,
				writeMask = byte.MaxValue,
				compareFunctionFront = CompareFunction.Equal,
				passOperationFront = StencilOp.Keep,
				failOperationFront = StencilOp.Keep,
				zFailOperationFront = StencilOp.IncrementSaturate,
				compareFunctionBack = CompareFunction.Less,
				passOperationBack = StencilOp.Keep,
				failOperationBack = StencilOp.Keep,
				zFailOperationBack = StencilOp.DecrementSaturate
			});
			this.m_CommandListManager = new CommandListManager(this.m_VertexDecl, this.m_DefaultStencilState);
		}

		private void InitVertexDeclaration()
		{
			VertexAttributeDescriptor[] array = new VertexAttributeDescriptor[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
				new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord3, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord4, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord5, VertexAttributeFormat.UNorm8, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord6, VertexAttributeFormat.Float32, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord7, VertexAttributeFormat.Float32, 1, 0)
			};
			this.m_VertexDecl = Utility.GetVertexDeclaration(array);
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void DisposeImmediate()
		{
			Debug.Assert(!UIRenderDevice.m_SynchronousFree);
			UIRenderDevice.m_SynchronousFree = true;
			this.Dispose();
			UIRenderDevice.m_SynchronousFree = false;
		}

		protected virtual void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				UIRenderDevice.m_ActiveDeviceCount--;
				if (disposing)
				{
					this.m_CommandListManager.ResetUIRendererDrawCallData();
					UIRenderDevice.DeviceToFree deviceToFree = new UIRenderDevice.DeviceToFree
					{
						handle = Utility.InsertCPUFence(),
						page = this.m_FirstPage,
						commandListManager = this.m_CommandListManager
					};
					bool flag = deviceToFree.handle == 0U;
					if (flag)
					{
						deviceToFree.Dispose();
					}
					else
					{
						UIRenderDevice.m_DeviceFreeQueue.AddLast(deviceToFree);
						bool synchronousFree = UIRenderDevice.m_SynchronousFree;
						if (synchronousFree)
						{
							UIRenderDevice.ProcessDeviceFreeQueue();
						}
					}
				}
				this.disposed = true;
			}
		}

		public MeshHandle Allocate(uint vertexCount, uint indexCount, out NativeSlice<Vertex> vertexData, out NativeSlice<ushort> indexData, out ushort indexOffset)
		{
			MeshHandle meshHandle = this.m_MeshHandles.Get();
			meshHandle.triangleCount = indexCount / 3U;
			this.Allocate(meshHandle, vertexCount, indexCount, out vertexData, out indexData, false);
			indexOffset = (ushort)meshHandle.allocVerts.start;
			return meshHandle;
		}

		public void Update(MeshHandle mesh, uint vertexCount, out NativeSlice<Vertex> vertexData)
		{
			Debug.Assert(mesh.allocVerts.size >= vertexCount);
			bool flag = mesh.allocTime == this.m_FrameIndex;
			if (flag)
			{
				vertexData = mesh.allocPage.vertices.cpuData.Slice<Vertex>((int)mesh.allocVerts.start, (int)vertexCount);
			}
			else
			{
				uint start = mesh.allocVerts.start;
				NativeSlice<ushort> nativeSlice = new NativeSlice<ushort>(mesh.allocPage.indices.cpuData, (int)mesh.allocIndices.start, (int)mesh.allocIndices.size);
				NativeSlice<ushort> nativeSlice2;
				ushort num;
				UIRenderDevice.AllocToUpdate allocToUpdate;
				this.UpdateAfterGPUUsedData(mesh, vertexCount, mesh.allocIndices.size, out vertexData, out nativeSlice2, out num, out allocToUpdate, false);
				int size = (int)mesh.allocIndices.size;
				int num2 = (int)((uint)num - start);
				for (int i = 0; i < size; i++)
				{
					nativeSlice2[i] = (ushort)((int)nativeSlice[i] + num2);
				}
			}
		}

		public void Update(MeshHandle mesh, uint vertexCount, uint indexCount, out NativeSlice<Vertex> vertexData, out NativeSlice<ushort> indexData, out ushort indexOffset)
		{
			Debug.Assert(mesh.allocVerts.size >= vertexCount);
			Debug.Assert(mesh.allocIndices.size >= indexCount);
			bool flag = mesh.allocTime == this.m_FrameIndex;
			if (flag)
			{
				vertexData = mesh.allocPage.vertices.cpuData.Slice<Vertex>((int)mesh.allocVerts.start, (int)vertexCount);
				indexData = mesh.allocPage.indices.cpuData.Slice<ushort>((int)mesh.allocIndices.start, (int)indexCount);
				indexOffset = (ushort)mesh.allocVerts.start;
				this.UpdateCopyBackIndices(mesh, true);
			}
			else
			{
				UIRenderDevice.AllocToUpdate allocToUpdate;
				this.UpdateAfterGPUUsedData(mesh, vertexCount, indexCount, out vertexData, out indexData, out indexOffset, out allocToUpdate, true);
			}
		}

		private void UpdateCopyBackIndices(MeshHandle mesh, bool copyBackIndices)
		{
			bool flag = mesh.updateAllocID == 0U;
			if (!flag)
			{
				int num = (int)(mesh.updateAllocID - 1U);
				List<UIRenderDevice.AllocToUpdate> list = this.ActiveUpdatesForMeshHandle(mesh);
				UIRenderDevice.AllocToUpdate allocToUpdate = list[num];
				allocToUpdate.copyBackIndices = true;
				list[num] = allocToUpdate;
			}
		}

		internal List<UIRenderDevice.AllocToUpdate> ActiveUpdatesForMeshHandle(MeshHandle mesh)
		{
			return this.m_Updates[(int)(mesh.allocTime % (uint)this.m_Updates.Count)];
		}

		private bool TryAllocFromPage(Page page, uint vertexCount, uint indexCount, ref Alloc va, ref Alloc ia, bool shortLived)
		{
			va = page.vertices.allocator.Allocate(vertexCount, shortLived);
			bool flag = va.size > 0U;
			if (flag)
			{
				ia = page.indices.allocator.Allocate(indexCount, shortLived);
				bool flag2 = ia.size > 0U;
				if (flag2)
				{
					return true;
				}
				page.vertices.allocator.Free(va);
				va.size = 0U;
			}
			return false;
		}

		private void Allocate(MeshHandle meshHandle, uint vertexCount, uint indexCount, out NativeSlice<Vertex> vertexData, out NativeSlice<ushort> indexData, bool shortLived)
		{
			Page page = null;
			Alloc alloc = default(Alloc);
			Alloc alloc2 = default(Alloc);
			bool flag = vertexCount <= this.m_LargeMeshVertexCount;
			if (flag)
			{
				bool flag2 = this.m_FirstPage != null;
				if (flag2)
				{
					page = this.m_FirstPage;
					for (;;)
					{
						bool flag3 = this.TryAllocFromPage(page, vertexCount, indexCount, ref alloc, ref alloc2, shortLived) || page.next == null;
						if (flag3)
						{
							break;
						}
						page = page.next;
					}
				}
				bool flag4 = alloc2.size == 0U;
				if (flag4)
				{
					this.m_NextPageVertexCount <<= 1;
					this.m_NextPageVertexCount = Math.Max(this.m_NextPageVertexCount, vertexCount * 2U);
					this.m_NextPageVertexCount = Math.Min(this.m_NextPageVertexCount, UIRenderDevice.maxVerticesPerPage);
					uint num = (uint)(this.m_NextPageVertexCount * this.m_IndexToVertexCountRatio + 0.5f);
					num = Math.Max(num, indexCount * 2U);
					Debug.Assert(((page != null) ? page.next : null) == null);
					page = new Page(this.m_NextPageVertexCount, num, 4U);
					page.next = this.m_FirstPage;
					this.m_FirstPage = page;
					alloc = page.vertices.allocator.Allocate(vertexCount, shortLived);
					alloc2 = page.indices.allocator.Allocate(indexCount, shortLived);
					Debug.Assert(alloc.size > 0U);
					Debug.Assert(alloc2.size > 0U);
				}
			}
			else
			{
				Page page2 = this.m_FirstPage;
				Page page3 = this.m_FirstPage;
				int num2 = int.MaxValue;
				while (page2 != null)
				{
					int num3 = page2.vertices.cpuData.Length - (int)vertexCount;
					int num4 = page2.indices.cpuData.Length - (int)indexCount;
					bool flag5 = page2.isEmpty && num3 >= 0 && num4 >= 0 && num3 < num2;
					if (flag5)
					{
						page = page2;
						num2 = num3;
					}
					page3 = page2;
					page2 = page2.next;
				}
				bool flag6 = page == null;
				if (flag6)
				{
					uint num5 = ((vertexCount > UIRenderDevice.maxVerticesPerPage) ? 2U : vertexCount);
					Debug.Assert(vertexCount <= UIRenderDevice.maxVerticesPerPage, "Requested Vertex count is above the limit. Alloc will fail.");
					page = new Page(num5, indexCount, 4U);
					bool flag7 = page3 != null;
					if (flag7)
					{
						page3.next = page;
					}
					else
					{
						this.m_FirstPage = page;
					}
				}
				alloc = page.vertices.allocator.Allocate(vertexCount, shortLived);
				alloc2 = page.indices.allocator.Allocate(indexCount, shortLived);
			}
			Debug.Assert(alloc.size == vertexCount, "Vertices allocated != Vertices requested");
			Debug.Assert(alloc2.size == indexCount, "Indices allocated != Indices requested");
			bool flag8 = alloc.size != vertexCount || alloc2.size != indexCount;
			if (flag8)
			{
				bool flag9 = alloc.handle != null;
				if (flag9)
				{
					page.vertices.allocator.Free(alloc);
				}
				bool flag10 = alloc2.handle != null;
				if (flag10)
				{
					page.vertices.allocator.Free(alloc2);
				}
				alloc2 = default(Alloc);
				alloc = default(Alloc);
			}
			page.vertices.RegisterUpdate(alloc.start, alloc.size);
			page.indices.RegisterUpdate(alloc2.start, alloc2.size);
			vertexData = new NativeSlice<Vertex>(page.vertices.cpuData, (int)alloc.start, (int)alloc.size);
			indexData = new NativeSlice<ushort>(page.indices.cpuData, (int)alloc2.start, (int)alloc2.size);
			meshHandle.allocPage = page;
			meshHandle.allocVerts = alloc;
			meshHandle.allocIndices = alloc2;
			meshHandle.allocTime = this.m_FrameIndex;
		}

		private void UpdateAfterGPUUsedData(MeshHandle mesh, uint vertexCount, uint indexCount, out NativeSlice<Vertex> vertexData, out NativeSlice<ushort> indexData, out ushort indexOffset, out UIRenderDevice.AllocToUpdate allocToUpdate, bool copyBackIndices)
		{
			UIRenderDevice.AllocToUpdate allocToUpdate2 = default(UIRenderDevice.AllocToUpdate);
			uint nextUpdateID = this.m_NextUpdateID;
			this.m_NextUpdateID = nextUpdateID + 1U;
			allocToUpdate2.id = nextUpdateID;
			allocToUpdate2.allocTime = this.m_FrameIndex;
			allocToUpdate2.meshHandle = mesh;
			allocToUpdate2.copyBackIndices = copyBackIndices;
			allocToUpdate = allocToUpdate2;
			Debug.Assert(this.m_NextUpdateID > 0U);
			bool flag = mesh.updateAllocID == 0U;
			if (flag)
			{
				allocToUpdate.permAllocVerts = mesh.allocVerts;
				allocToUpdate.permAllocIndices = mesh.allocIndices;
				allocToUpdate.permPage = mesh.allocPage;
			}
			else
			{
				int num = (int)(mesh.updateAllocID - 1U);
				List<UIRenderDevice.AllocToUpdate> list = this.m_Updates[(int)(mesh.allocTime % (uint)this.m_Updates.Count)];
				UIRenderDevice.AllocToUpdate allocToUpdate3 = list[num];
				Debug.Assert(allocToUpdate3.id == mesh.updateAllocID);
				allocToUpdate.copyBackIndices |= allocToUpdate3.copyBackIndices;
				allocToUpdate.permAllocVerts = allocToUpdate3.permAllocVerts;
				allocToUpdate.permAllocIndices = allocToUpdate3.permAllocIndices;
				allocToUpdate.permPage = allocToUpdate3.permPage;
				allocToUpdate3.allocTime = uint.MaxValue;
				list[num] = allocToUpdate3;
				List<UIRenderDevice.AllocToFree> list2 = this.m_DeferredFrees[(int)(this.m_FrameIndex % (uint)this.m_DeferredFrees.Count)];
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = mesh.allocVerts,
					page = mesh.allocPage,
					vertices = true
				});
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = mesh.allocIndices,
					page = mesh.allocPage,
					vertices = false
				});
			}
			bool flag2 = this.TryAllocFromPage(mesh.allocPage, vertexCount, indexCount, ref mesh.allocVerts, ref mesh.allocIndices, true);
			if (flag2)
			{
				mesh.allocPage.vertices.RegisterUpdate(mesh.allocVerts.start, mesh.allocVerts.size);
				mesh.allocPage.indices.RegisterUpdate(mesh.allocIndices.start, mesh.allocIndices.size);
			}
			else
			{
				this.Allocate(mesh, vertexCount, indexCount, out vertexData, out indexData, true);
			}
			mesh.triangleCount = indexCount / 3U;
			mesh.updateAllocID = allocToUpdate.id;
			mesh.allocTime = allocToUpdate.allocTime;
			this.m_Updates[(int)((ulong)this.m_FrameIndex % (ulong)((long)this.m_Updates.Count))].Add(allocToUpdate);
			vertexData = new NativeSlice<Vertex>(mesh.allocPage.vertices.cpuData, (int)mesh.allocVerts.start, (int)vertexCount);
			indexData = new NativeSlice<ushort>(mesh.allocPage.indices.cpuData, (int)mesh.allocIndices.start, (int)indexCount);
			indexOffset = (ushort)mesh.allocVerts.start;
		}

		public void Free(MeshHandle mesh)
		{
			bool flag = mesh.updateAllocID > 0U;
			if (flag)
			{
				int num = (int)(mesh.updateAllocID - 1U);
				List<UIRenderDevice.AllocToUpdate> list = this.m_Updates[(int)(mesh.allocTime % (uint)this.m_Updates.Count)];
				UIRenderDevice.AllocToUpdate allocToUpdate = list[num];
				Debug.Assert(allocToUpdate.id == mesh.updateAllocID);
				List<UIRenderDevice.AllocToFree> list2 = this.m_DeferredFrees[(int)(this.m_FrameIndex % (uint)this.m_DeferredFrees.Count)];
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = allocToUpdate.permAllocVerts,
					page = allocToUpdate.permPage,
					vertices = true
				});
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = allocToUpdate.permAllocIndices,
					page = allocToUpdate.permPage,
					vertices = false
				});
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = mesh.allocVerts,
					page = mesh.allocPage,
					vertices = true
				});
				list2.Add(new UIRenderDevice.AllocToFree
				{
					alloc = mesh.allocIndices,
					page = mesh.allocPage,
					vertices = false
				});
				allocToUpdate.allocTime = uint.MaxValue;
				list[num] = allocToUpdate;
			}
			else
			{
				bool flag2 = mesh.allocTime != this.m_FrameIndex;
				if (flag2)
				{
					int num2 = (int)(this.m_FrameIndex % (uint)this.m_DeferredFrees.Count);
					this.m_DeferredFrees[num2].Add(new UIRenderDevice.AllocToFree
					{
						alloc = mesh.allocVerts,
						page = mesh.allocPage,
						vertices = true
					});
					this.m_DeferredFrees[num2].Add(new UIRenderDevice.AllocToFree
					{
						alloc = mesh.allocIndices,
						page = mesh.allocPage,
						vertices = false
					});
				}
				else
				{
					mesh.allocPage.vertices.allocator.Free(mesh.allocVerts);
					mesh.allocPage.indices.allocator.Free(mesh.allocIndices);
				}
			}
			mesh.allocVerts = default(Alloc);
			mesh.allocIndices = default(Alloc);
			mesh.allocPage = null;
			mesh.updateAllocID = 0U;
			this.m_MeshHandles.Return(mesh);
		}

		public void OnFrameRenderingBegin()
		{
			this.m_DrawStats = default(UIRenderDevice.DrawStatistics);
			this.m_DrawStats.currentFrameIndex = (int)this.m_FrameIndex;
			for (Page page = this.m_FirstPage; page != null; page = page.next)
			{
				page.vertices.SendUpdates();
				page.indices.SendUpdates();
			}
			this.UpdateFenceValue();
		}

		internal unsafe static NativeSlice<T> PtrToSlice<T>(void* p, int count) where T : struct
		{
			return NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<T>(p, UnsafeUtility.SizeOf<T>(), count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int FlagsToTextureSlotCount(UIRenderDevice.EvaluationFlags flags)
		{
			int num = (int)((flags & UIRenderDevice.EvaluationFlags.TextureSlotCountBits) >> 6);
			return UIRenderDevice.s_EvaluationFlagsToTextureSlotCount[num];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UIRenderDevice.EvaluationFlags TextureSlotCountToFlags(TextureSlotCount count)
		{
			return (UIRenderDevice.EvaluationFlags)(UIRenderDevice.s_TextureSlotCountToEvaluationFlags[(int)count] << 6);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ApplyDrawCommandState(RenderChainCommand cmd, int textureSlot, Material newMat, bool newMatDiffers, MaterialPropertyBlock userProps, UIRenderDevice.EvaluationFlags defaultTextureSlotCountFlags, bool kickRanges, Texture gradientSettings, Texture shaderInfo, ref UIRenderDevice.EvaluationState st)
		{
			if (newMatDiffers)
			{
				st.material = newMat;
				st.userProps = userProps;
				st.flags |= UIRenderDevice.EvaluationFlags.MustApplyMaterial;
				st.flags &= ~(UIRenderDevice.EvaluationFlags.TextureSlotCount1 | UIRenderDevice.EvaluationFlags.TextureSlotCount2 | UIRenderDevice.EvaluationFlags.TextureSlotCount8);
				bool flag = (cmd.flags & CommandFlags.ForceSingleTextureSlot) > CommandFlags.None;
				if (flag)
				{
					st.flags |= UIRenderDevice.EvaluationFlags.TextureSlotCount1;
				}
				else
				{
					st.flags |= defaultTextureSlotCountFlags;
				}
				st.flags &= ~(UIRenderDevice.EvaluationFlags.ForceRenderTypeSolid | UIRenderDevice.EvaluationFlags.ForceRenderTypeTextured | UIRenderDevice.EvaluationFlags.ForceRenderTypeSvgGradient);
				bool flag2 = (cmd.flags & CommandFlags.ForceRenderTypeBits) > CommandFlags.None;
				if (flag2)
				{
					uint num = (uint)((cmd.flags & CommandFlags.ForceRenderTypeBits) >> 1);
					st.flags |= (UIRenderDevice.EvaluationFlags)(num << 3);
				}
				bool flag3 = (st.flags & UIRenderDevice.EvaluationFlags.IsSerializing) > UIRenderDevice.EvaluationFlags.None;
				if (flag3)
				{
					this.SetupCommandList(ref st, gradientSettings, shaderInfo, cmd.flags);
				}
			}
			if (kickRanges)
			{
				this.m_TextureSlotManager.StartNewBatch(UIRenderDevice.FlagsToTextureSlotCount(st.flags));
			}
			st.curPage = cmd.mesh.allocPage;
			bool flag4 = cmd.texture != TextureId.invalid;
			if (flag4)
			{
				bool flag5 = textureSlot < 0;
				if (flag5)
				{
					textureSlot = this.m_TextureSlotManager.FindOldestSlot();
					this.m_TextureSlotManager.Bind(cmd.texture, cmd.sdfScale, cmd.sharpness, (cmd.flags & CommandFlags.IsPremultiplied) > CommandFlags.None, textureSlot, st.batchProps, st.activeCommandList);
					st.flags |= UIRenderDevice.EvaluationFlags.MustApplyBatchProps;
				}
				else
				{
					this.m_TextureSlotManager.MarkUsed(textureSlot);
				}
			}
			bool flag6 = cmd.stencilRef != st.stencilRef;
			if (flag6)
			{
				st.stencilRef = cmd.stencilRef;
				st.flags |= UIRenderDevice.EvaluationFlags.MustApplyStencil;
			}
		}

		private void ApplyBatchState(ref UIRenderDevice.EvaluationState st)
		{
			bool flag = (st.flags & UIRenderDevice.EvaluationFlags.MustApplyMaterial) > UIRenderDevice.EvaluationFlags.None;
			if (flag)
			{
				this.m_DrawStats.materialSetCount = this.m_DrawStats.materialSetCount + 1U;
				bool flag2 = st.activeCommandList == null;
				if (flag2)
				{
					Debug.Assert(this.isFlat || (st.flags & UIRenderDevice.EvaluationFlags.IsRenderingNestedTreeRT) == UIRenderDevice.EvaluationFlags.IsRenderingNestedTreeRT);
					bool flag3 = false;
					bool forceGammaRendering = this.forceGammaRendering;
					if (forceGammaRendering)
					{
						st.material.EnableKeyword(Shaders.k_ForceGammaKeyword);
						flag3 = true;
					}
					else
					{
						st.material.DisableKeyword(Shaders.k_ForceGammaKeyword);
					}
					UIRenderDevice.EvaluationFlags evaluationFlags = st.flags & UIRenderDevice.EvaluationFlags.TextureSlotCountBits;
					UIRenderDevice.EvaluationFlags evaluationFlags2 = evaluationFlags;
					if (evaluationFlags2 <= UIRenderDevice.EvaluationFlags.TextureSlotCount2)
					{
						if (evaluationFlags2 == UIRenderDevice.EvaluationFlags.TextureSlotCount1)
						{
							st.material.EnableKeyword(Shaders.k_TextureSlotCount1);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount2);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount4);
							flag3 = true;
							goto IL_01C5;
						}
						if (evaluationFlags2 == UIRenderDevice.EvaluationFlags.TextureSlotCount2)
						{
							st.material.DisableKeyword(Shaders.k_TextureSlotCount1);
							st.material.EnableKeyword(Shaders.k_TextureSlotCount2);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount4);
							flag3 = true;
							goto IL_01C5;
						}
					}
					else
					{
						if (evaluationFlags2 == UIRenderDevice.EvaluationFlags.TextureSlotCount4)
						{
							st.material.DisableKeyword(Shaders.k_TextureSlotCount1);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount2);
							st.material.EnableKeyword(Shaders.k_TextureSlotCount4);
							flag3 = true;
							goto IL_01C5;
						}
						if (evaluationFlags2 == UIRenderDevice.EvaluationFlags.TextureSlotCount8)
						{
							st.material.DisableKeyword(Shaders.k_TextureSlotCount1);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount2);
							st.material.DisableKeyword(Shaders.k_TextureSlotCount4);
							goto IL_01C5;
						}
					}
					throw new ArgumentException("Unsupported texture slot count.");
					IL_01C5:
					UIRenderDevice.EvaluationFlags evaluationFlags3 = st.flags & UIRenderDevice.EvaluationFlags.ForceRenderTypeBits;
					UIRenderDevice.EvaluationFlags evaluationFlags4 = evaluationFlags3;
					if (evaluationFlags4 <= UIRenderDevice.EvaluationFlags.ForceRenderTypeTextured)
					{
						if (evaluationFlags4 == UIRenderDevice.EvaluationFlags.ForceRenderTypeSolid)
						{
							st.material.EnableKeyword(Shaders.k_ForceRenderTypeSolid);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeTextured);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeText);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
							flag3 = true;
							goto IL_0374;
						}
						if (evaluationFlags4 == UIRenderDevice.EvaluationFlags.ForceRenderTypeTextured)
						{
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSolid);
							st.material.EnableKeyword(Shaders.k_ForceRenderTypeTextured);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeText);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
							flag3 = true;
							goto IL_0374;
						}
					}
					else
					{
						if (evaluationFlags4 == UIRenderDevice.EvaluationFlags.ForceRenderTypeText)
						{
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSolid);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeTextured);
							st.material.EnableKeyword(Shaders.k_ForceRenderTypeText);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
							flag3 = true;
							goto IL_0374;
						}
						if (evaluationFlags4 == UIRenderDevice.EvaluationFlags.ForceRenderTypeSvgGradient)
						{
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeSolid);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeTextured);
							st.material.DisableKeyword(Shaders.k_ForceRenderTypeText);
							st.material.EnableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
							flag3 = true;
							goto IL_0374;
						}
					}
					st.material.DisableKeyword(Shaders.k_ForceRenderTypeSolid);
					st.material.DisableKeyword(Shaders.k_ForceRenderTypeTextured);
					st.material.DisableKeyword(Shaders.k_ForceRenderTypeText);
					st.material.DisableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
					IL_0374:
					bool flag4 = flag3;
					if (flag4)
					{
						this.m_ScreenSpaceAlteredMaterials.Add(st.material);
					}
					st.material.SetPass(0);
					Utility.SetPropertyBlock(st.constantProps);
					bool flag5 = st.userProps != null;
					if (flag5)
					{
						Utility.SetPropertyBlock(st.userProps);
					}
					st.flags |= UIRenderDevice.EvaluationFlags.TextureSlotCountBitOffset;
				}
				else
				{
					bool flag6 = st.userProps != null;
					if (flag6)
					{
						st.activeCommandList.ApplyUserProps(st.userProps);
					}
				}
			}
			bool flag7 = (st.flags & UIRenderDevice.EvaluationFlags.MustApplyBatchProps) > UIRenderDevice.EvaluationFlags.None;
			if (flag7)
			{
				bool flag8 = st.activeCommandList == null;
				if (flag8)
				{
					Utility.SetPropertyBlock(st.batchProps);
				}
				else
				{
					st.activeCommandList.ApplyBatchProps();
				}
			}
			bool flag9 = (st.flags & UIRenderDevice.EvaluationFlags.MustApplyStencil) > UIRenderDevice.EvaluationFlags.None;
			if (flag9)
			{
				this.m_DrawStats.stencilRefChanges = this.m_DrawStats.stencilRefChanges + 1U;
				bool flag10 = st.activeCommandList == null;
				if (flag10)
				{
					Utility.SetStencilState(this.m_DefaultStencilState, st.stencilRef);
				}
			}
			st.flags &= ~(UIRenderDevice.EvaluationFlags.MustApplyMaterial | UIRenderDevice.EvaluationFlags.MustApplyBatchProps | UIRenderDevice.EvaluationFlags.MustApplyStencil);
		}

		public unsafe void EvaluateChain(RenderChainCommand head, Material defaultMat, Texture gradientSettings, Texture shaderInfo, Rect? scissor, float pixelsPerPoint, bool isSerializing, TextureSlotCount defaultTextureSlotCount, bool isRenderingNestedTreeRT, ref Exception immediateException)
		{
			Utility.ProfileDrawChainBegin();
			bool breakBatches = this.breakBatches;
			int num = 1024;
			DrawBufferRange* ptr;
			checked
			{
				ptr = stackalloc DrawBufferRange[unchecked((UIntPtr)num) * (UIntPtr)sizeof(DrawBufferRange)];
			}
			int num2 = num - 1;
			int num3 = 0;
			int num4 = 0;
			DrawBufferRange drawBufferRange = default(DrawBufferRange);
			int num5 = -1;
			int num6 = 0;
			UIRenderDevice.EvaluationFlags evaluationFlags = UIRenderDevice.TextureSlotCountToFlags(defaultTextureSlotCount);
			UIRenderDevice.EvaluationFlags evaluationFlags2 = (isRenderingNestedTreeRT ? UIRenderDevice.EvaluationFlags.IsRenderingNestedTreeRT : UIRenderDevice.EvaluationFlags.None);
			UIRenderDevice.EvaluationState evaluationState = new UIRenderDevice.EvaluationState
			{
				flags = (UIRenderDevice.EvaluationFlags.TextureSlotCountBitOffset | evaluationFlags | evaluationFlags2)
			};
			if (isSerializing)
			{
				this.m_CommandListManager.BeginSerialize(defaultTextureSlotCount);
				evaluationState.activeCommandList = this.m_CommandListManager.defaultCommandList;
				evaluationState.flags |= UIRenderDevice.EvaluationFlags.IsSerializing;
			}
			else
			{
				evaluationState.constantProps = this.m_ConstantProps;
				this.InitializeConstantProperties(evaluationState.constantProps, gradientSettings, shaderInfo);
				evaluationState.batchProps = this.m_BatchProps;
				evaluationState.batchProps.Clear();
			}
			DrawParams drawParams = this.m_DrawParams;
			drawParams.Reset();
			RenderChainCommand.PushScissor(drawParams, scissor ?? DrawParams.k_UnlimitedRect, pixelsPerPoint);
			this.m_TextureSlotManager.Reset();
			this.m_TextureSlotManager.StartNewBatch((int)defaultTextureSlotCount);
			MaterialPropertyBlock materialPropertyBlock = null;
			while (head != null)
			{
				bool flag = head.type == CommandType.BeginDisable;
				if (flag)
				{
					this.m_DrawStats.commandCount = this.m_DrawStats.commandCount + 1U;
					num6++;
					head = head.next;
				}
				else
				{
					bool flag2 = head.type == CommandType.EndDisable;
					if (flag2)
					{
						this.m_DrawStats.commandCount = this.m_DrawStats.commandCount + 1U;
						num6--;
						head = head.next;
					}
					else
					{
						bool flag3 = num6 > 0;
						if (flag3)
						{
							this.m_DrawStats.skippedCommandCount = this.m_DrawStats.skippedCommandCount + 1U;
							head = head.next;
						}
						else
						{
							this.m_DrawStats.drawCommandCount = this.m_DrawStats.drawCommandCount + ((head.type == CommandType.Draw) ? 1U : 0U);
							bool flag4 = drawBufferRange.indexCount > 0 && num4 == num - 1;
							bool flag5 = false;
							bool flag6 = false;
							bool flag7 = false;
							int num7 = -1;
							Material material = null;
							bool flag8 = false;
							bool flag9 = head.type == CommandType.Draw;
							if (!flag9)
							{
								if (head.type != CommandType.PopDefaultMaterial)
								{
									goto IL_03E2;
								}
								RenderChainCommand next = head.next;
								if (next == null || next.type != CommandType.PushDefaultMaterial)
								{
									goto IL_03E2;
								}
								Object @object = defaultMat;
								RenderChainCommand next2 = head.next;
								if (!(@object == ((next2 != null) ? next2.material : null)))
								{
									goto IL_03E2;
								}
								MaterialPropertyBlock materialPropertyBlock2 = materialPropertyBlock;
								RenderChainCommand next3 = head.next;
								bool flag10 = materialPropertyBlock2 == ((next3 != null) ? next3.userProps : null);
								IL_03E3:
								bool flag11 = flag10;
								if (flag11)
								{
									head = head.next.next;
									continue;
								}
								flag5 = true;
								flag6 = true;
								goto IL_0403;
								IL_03E2:
								flag10 = false;
								goto IL_03E3;
							}
							uint num8 = (uint)((evaluationState.flags & UIRenderDevice.EvaluationFlags.ForceRenderTypeBits) >> 3);
							uint num9 = (uint)((head.flags & CommandFlags.ForceRenderTypeBits) >> 1);
							bool flag12 = (head.flags & CommandFlags.ForceSingleTextureSlot) > CommandFlags.None;
							UIRenderDevice.EvaluationFlags evaluationFlags3 = evaluationState.flags & UIRenderDevice.EvaluationFlags.TextureSlotCountBits;
							UIRenderDevice.EvaluationFlags evaluationFlags4 = (flag12 ? UIRenderDevice.EvaluationFlags.TextureSlotCount1 : evaluationFlags);
							material = ((head.material != null) ? head.material : defaultMat);
							bool flag13 = num8 != num9 || evaluationFlags3 != evaluationFlags4 || material != evaluationState.material;
							if (flag13)
							{
								flag7 = true;
								flag8 = true;
								flag5 = true;
								flag6 = true;
							}
							else
							{
								bool flag14 = head.mesh.allocPage != evaluationState.curPage;
								if (flag14)
								{
									flag7 = true;
									flag5 = true;
									flag6 = true;
								}
								else
								{
									bool flag15 = (long)num5 != (long)((ulong)head.mesh.allocIndices.start + (ulong)((long)head.indexOffset));
									if (flag15)
									{
										flag5 = true;
									}
								}
								bool flag16 = head.texture != TextureId.invalid;
								if (flag16)
								{
									flag7 = true;
									num7 = this.m_TextureSlotManager.IndexOf(head.texture);
									bool flag17 = num7 < 0 && this.m_TextureSlotManager.FreeSlots < 1;
									if (flag17)
									{
										flag5 = true;
										flag6 = true;
									}
								}
								bool flag18 = head.stencilRef != evaluationState.stencilRef;
								if (flag18)
								{
									flag7 = true;
									flag5 = true;
									flag6 = true;
								}
								bool flag19 = flag5 && flag4;
								if (flag19)
								{
									flag6 = true;
								}
							}
							IL_0403:
							bool flag20 = breakBatches;
							if (flag20)
							{
								flag5 = true;
								flag6 = true;
							}
							bool flag21 = flag5;
							if (flag21)
							{
								bool flag22 = drawBufferRange.indexCount > 0;
								if (flag22)
								{
									int num10 = (num3 + num4++) & num2;
									ptr[num10] = drawBufferRange;
									Debug.Assert(num4 < num || flag6);
									drawBufferRange = default(DrawBufferRange);
									this.m_DrawStats.drawRangeCount = this.m_DrawStats.drawRangeCount + 1U;
								}
								bool flag23 = head.type == CommandType.Draw;
								if (flag23)
								{
									drawBufferRange.firstIndex = (int)(head.mesh.allocIndices.start + (uint)head.indexOffset);
									drawBufferRange.indexCount = head.indexCount;
									drawBufferRange.vertsReferenced = (int)head.mesh.allocVerts.size;
									drawBufferRange.minIndexVal = (int)head.mesh.allocVerts.start;
									num5 = drawBufferRange.firstIndex + head.indexCount;
									this.m_DrawStats.totalIndices = this.m_DrawStats.totalIndices + (uint)head.indexCount;
								}
								bool flag24 = flag6;
								if (flag24)
								{
									bool flag25 = num4 > 0;
									if (flag25)
									{
										this.ApplyBatchState(ref evaluationState);
										this.KickRanges(ptr, ref num4, ref num3, num, evaluationState.curPage, evaluationState.activeCommandList);
									}
									bool flag26 = head.type > CommandType.Draw;
									if (flag26)
									{
										bool flag27 = head.type == CommandType.CutRenderChain;
										if (flag27)
										{
											evaluationState.material = null;
											evaluationState.commandListOwner = head.owner.owner;
										}
										bool flag28 = head.type == CommandType.Immediate || head.type == CommandType.ImmediateCull;
										if (flag28)
										{
											this.ResetScreenSpaceMaterials();
										}
										head.ExecuteNonDrawMesh(drawParams, pixelsPerPoint, ref immediateException);
										bool flag29 = head.type == CommandType.Immediate || head.type == CommandType.ImmediateCull || head.type == CommandType.PopDefaultMaterial || head.type == CommandType.PushDefaultMaterial;
										if (flag29)
										{
											evaluationState.material = null;
											evaluationState.flags &= ~UIRenderDevice.EvaluationFlags.MustApplyMaterial;
											this.m_DrawStats.immediateDraws = this.m_DrawStats.immediateDraws + 1U;
											bool flag30 = head.type == CommandType.PopDefaultMaterial;
											if (flag30)
											{
												int num11 = drawParams.defaultMaterial.Count - 1;
												defaultMat = drawParams.defaultMaterial[num11];
												materialPropertyBlock = drawParams.props[num11];
												drawParams.defaultMaterial.RemoveAt(num11);
											}
											bool flag31 = head.type == CommandType.PushDefaultMaterial;
											if (flag31)
											{
												drawParams.defaultMaterial.Add(defaultMat);
												drawParams.props.Add(head.userProps);
												defaultMat = head.material;
												materialPropertyBlock = head.userProps;
											}
										}
									}
								}
								bool flag32 = head.type == CommandType.Draw && flag7;
								if (flag32)
								{
									this.ApplyDrawCommandState(head, num7, material, flag8, materialPropertyBlock, evaluationFlags, flag6, gradientSettings, shaderInfo, ref evaluationState);
								}
								head = head.next;
							}
							else
							{
								bool flag33 = drawBufferRange.indexCount == 0;
								if (flag33)
								{
									num5 = (drawBufferRange.firstIndex = (int)(head.mesh.allocIndices.start + (uint)head.indexOffset));
								}
								drawBufferRange.indexCount += head.indexCount;
								int minIndexVal = drawBufferRange.minIndexVal;
								int start = (int)head.mesh.allocVerts.start;
								int num12 = drawBufferRange.minIndexVal + drawBufferRange.vertsReferenced;
								int num13 = (int)(head.mesh.allocVerts.start + head.mesh.allocVerts.size);
								drawBufferRange.minIndexVal = Mathf.Min(minIndexVal, start);
								drawBufferRange.vertsReferenced = Mathf.Max(num12, num13) - drawBufferRange.minIndexVal;
								num5 += head.indexCount;
								this.m_DrawStats.totalIndices = this.m_DrawStats.totalIndices + (uint)head.indexCount;
								bool flag34 = flag7;
								if (flag34)
								{
									this.ApplyDrawCommandState(head, num7, material, flag8, materialPropertyBlock, evaluationFlags, flag6, gradientSettings, shaderInfo, ref evaluationState);
								}
								head = head.next;
							}
						}
					}
				}
			}
			bool flag35 = drawBufferRange.indexCount > 0;
			if (flag35)
			{
				int num14 = (num3 + num4++) & num2;
				ptr[num14] = drawBufferRange;
			}
			bool flag36 = num4 > 0;
			if (flag36)
			{
				this.ApplyBatchState(ref evaluationState);
				this.KickRanges(ptr, ref num4, ref num3, num, evaluationState.curPage, evaluationState.activeCommandList);
			}
			Debug.Assert(num6 == 0, "Rendering disabled counter is not 0, indicating a mismatch of commands");
			RenderChainCommand.PopScissor(drawParams, pixelsPerPoint);
			this.UpdateFenceValue();
			Utility.ProfileDrawChainEnd();
			bool flag37 = (evaluationState.flags & UIRenderDevice.EvaluationFlags.IsSerializing) > UIRenderDevice.EvaluationFlags.None;
			if (flag37)
			{
				this.m_CommandListManager.EndSerialize();
			}
			this.ResetScreenSpaceMaterials();
		}

		private void ResetScreenSpaceMaterials()
		{
			foreach (Material material in this.m_ScreenSpaceAlteredMaterials)
			{
				bool flag = material == null;
				if (!flag)
				{
					material.DisableKeyword(Shaders.k_ForceGammaKeyword);
					material.DisableKeyword(Shaders.k_TextureSlotCount1);
					material.DisableKeyword(Shaders.k_TextureSlotCount2);
					material.DisableKeyword(Shaders.k_TextureSlotCount4);
					material.DisableKeyword(Shaders.k_ForceRenderTypeSolid);
					material.DisableKeyword(Shaders.k_ForceRenderTypeTextured);
					material.DisableKeyword(Shaders.k_ForceRenderTypeText);
					material.DisableKeyword(Shaders.k_ForceRenderTypeSvgGradient);
				}
			}
			this.m_ScreenSpaceAlteredMaterials.Clear();
		}

		private void InitializeConstantProperties(MaterialPropertyBlock constantProps, Texture gradientSettings, Texture shaderInfo)
		{
			bool flag = gradientSettings != null;
			if (flag)
			{
				constantProps.SetTexture(UIRenderDevice.s_GradientSettingsTexID, gradientSettings);
			}
			bool flag2 = shaderInfo != null;
			if (flag2)
			{
				constantProps.SetTexture(UIRenderDevice.s_ShaderInfoTexID, shaderInfo);
			}
		}

		private void SetupCommandList(ref UIRenderDevice.EvaluationState st, Texture gradientSettings, Texture shaderInfo, CommandFlags commandFlags)
		{
			bool flag = st.commandListOwner == null;
			if (!flag)
			{
				CommandList orCreateCommandList = this.m_CommandListManager.GetOrCreateCommandList(st.commandListOwner, st.material, commandFlags);
				this.InitializeConstantProperties(orCreateCommandList.constantProps, gradientSettings, shaderInfo);
				st.activeCommandList = orCreateCommandList;
				st.constantProps = null;
				st.batchProps = null;
				st.flags |= UIRenderDevice.EvaluationFlags.TextureSlotCountBitOffset;
				this.m_TextureSlotManager.Reset();
			}
		}

		private unsafe void UpdateFenceValue()
		{
			uint num = Utility.InsertCPUFence();
			fixed (uint* ptr = &this.m_Fences[(int)((ulong)this.m_FrameIndex % (ulong)((long)this.m_Fences.Length))])
			{
				uint* ptr2 = ptr;
				bool flag2;
				do
				{
					uint num2 = *ptr2;
					bool flag = num - num2 <= 0U;
					if (flag)
					{
						break;
					}
					int num3 = Interlocked.CompareExchange(ref *(int*)ptr2, (int)num, (int)num2);
					flag2 = (long)num3 == (long)((ulong)num2);
				}
				while (!flag2);
			}
		}

		private unsafe void KickRanges(DrawBufferRange* ranges, ref int rangesReady, ref int rangesStart, int rangesCount, Page curPage, CommandList commandList)
		{
			Debug.Assert(rangesReady > 0);
			bool flag = rangesStart + rangesReady <= rangesCount;
			if (flag)
			{
				this.DrawRanges(curPage.indices.gpuData, curPage.vertices.gpuData, UIRenderDevice.PtrToSlice<DrawBufferRange>((void*)(ranges + rangesStart), rangesReady), commandList);
				this.m_DrawStats.drawRangeCallCount = this.m_DrawStats.drawRangeCallCount + 1U;
			}
			else
			{
				int num = rangesCount - rangesStart;
				int num2 = rangesReady - num;
				this.DrawRanges(curPage.indices.gpuData, curPage.vertices.gpuData, UIRenderDevice.PtrToSlice<DrawBufferRange>((void*)(ranges + rangesStart), num), commandList);
				this.DrawRanges(curPage.indices.gpuData, curPage.vertices.gpuData, UIRenderDevice.PtrToSlice<DrawBufferRange>((void*)ranges, num2), commandList);
				this.m_DrawStats.drawRangeCallCount = this.m_DrawStats.drawRangeCallCount + 2U;
			}
			rangesStart = (rangesStart + rangesReady) & (rangesCount - 1);
			rangesReady = 0;
		}

		private unsafe void DrawRanges(Utility.GPUBuffer<ushort> ib, Utility.GPUBuffer<Vertex> vb, NativeSlice<DrawBufferRange> ranges, CommandList commandList)
		{
			bool flag = commandList != null;
			checked
			{
				if (flag)
				{
					commandList.DrawRanges(ib, vb, ranges);
				}
				else
				{
					IntPtr* ptr = stackalloc IntPtr[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
					*ptr = vb.BufferPointer;
					Utility.DrawRanges(ib.BufferPointer, ptr, 1, new IntPtr(ranges.GetUnsafePtr<DrawBufferRange>()), ranges.Length, this.m_VertexDecl);
				}
			}
		}

		internal void WaitOnAllCpuFences()
		{
			for (int i = 0; i < this.m_Fences.Length; i++)
			{
				this.WaitOnCpuFence(this.m_Fences[i]);
			}
		}

		private void WaitOnCpuFence(uint fence)
		{
			bool flag = fence != 0U && !Utility.CPUFencePassed(fence);
			if (flag)
			{
				Utility.WaitForCPUFencePassed(fence);
			}
		}

		public void AdvanceFrame()
		{
			this.m_FrameIndex += 1U;
			this.m_DrawStats.currentFrameIndex = (int)this.m_FrameIndex;
			int num = (int)((ulong)this.m_FrameIndex % (ulong)((long)this.m_Fences.Length));
			uint num2 = this.m_Fences[num];
			this.WaitOnCpuFence(num2);
			this.m_Fences[num] = 0U;
			this.m_CommandListManager.AdvanceFrame();
			this.m_NextUpdateID = 1U;
			List<UIRenderDevice.AllocToFree> list = this.m_DeferredFrees[(int)(this.m_FrameIndex % (uint)this.m_DeferredFrees.Count)];
			foreach (UIRenderDevice.AllocToFree allocToFree in list)
			{
				bool vertices = allocToFree.vertices;
				if (vertices)
				{
					allocToFree.page.vertices.allocator.Free(allocToFree.alloc);
				}
				else
				{
					allocToFree.page.indices.allocator.Free(allocToFree.alloc);
				}
			}
			list.Clear();
			List<UIRenderDevice.AllocToUpdate> list2 = this.m_Updates[(int)(this.m_FrameIndex % (uint)this.m_DeferredFrees.Count)];
			foreach (UIRenderDevice.AllocToUpdate allocToUpdate in list2)
			{
				bool flag = allocToUpdate.meshHandle.updateAllocID == allocToUpdate.id && allocToUpdate.meshHandle.allocTime == allocToUpdate.allocTime;
				if (flag)
				{
					NativeSlice<Vertex> nativeSlice = new NativeSlice<Vertex>(allocToUpdate.meshHandle.allocPage.vertices.cpuData, (int)allocToUpdate.meshHandle.allocVerts.start, (int)allocToUpdate.meshHandle.allocVerts.size);
					NativeSlice<Vertex> nativeSlice2 = new NativeSlice<Vertex>(allocToUpdate.permPage.vertices.cpuData, (int)allocToUpdate.permAllocVerts.start, (int)allocToUpdate.meshHandle.allocVerts.size);
					nativeSlice2.CopyFrom(nativeSlice);
					allocToUpdate.permPage.vertices.RegisterUpdate(allocToUpdate.permAllocVerts.start, allocToUpdate.meshHandle.allocVerts.size);
					bool copyBackIndices = allocToUpdate.copyBackIndices;
					if (copyBackIndices)
					{
						NativeSlice<ushort> nativeSlice3 = new NativeSlice<ushort>(allocToUpdate.meshHandle.allocPage.indices.cpuData, (int)allocToUpdate.meshHandle.allocIndices.start, (int)allocToUpdate.meshHandle.allocIndices.size);
						NativeSlice<ushort> nativeSlice4 = new NativeSlice<ushort>(allocToUpdate.permPage.indices.cpuData, (int)allocToUpdate.permAllocIndices.start, (int)allocToUpdate.meshHandle.allocIndices.size);
						int length = nativeSlice4.Length;
						int num3 = (int)(allocToUpdate.permAllocVerts.start - allocToUpdate.meshHandle.allocVerts.start);
						for (int i = 0; i < length; i++)
						{
							nativeSlice4[i] = (ushort)((int)nativeSlice3[i] + num3);
						}
						allocToUpdate.permPage.indices.RegisterUpdate(allocToUpdate.permAllocIndices.start, allocToUpdate.meshHandle.allocIndices.size);
					}
					list.Add(new UIRenderDevice.AllocToFree
					{
						alloc = allocToUpdate.meshHandle.allocVerts,
						page = allocToUpdate.meshHandle.allocPage,
						vertices = true
					});
					list.Add(new UIRenderDevice.AllocToFree
					{
						alloc = allocToUpdate.meshHandle.allocIndices,
						page = allocToUpdate.meshHandle.allocPage,
						vertices = false
					});
					allocToUpdate.meshHandle.allocVerts = allocToUpdate.permAllocVerts;
					allocToUpdate.meshHandle.allocIndices = allocToUpdate.permAllocIndices;
					allocToUpdate.meshHandle.allocPage = allocToUpdate.permPage;
					allocToUpdate.meshHandle.updateAllocID = 0U;
				}
			}
			list2.Clear();
			this.PruneUnusedPages();
		}

		private void PruneUnusedPages()
		{
			Page page4;
			Page page3;
			Page page2;
			Page page = (page2 = (page3 = (page4 = null)));
			Page next;
			for (Page page5 = this.m_FirstPage; page5 != null; page5 = next)
			{
				bool flag = !page5.isEmpty;
				if (flag)
				{
					page5.framesEmpty = 0;
				}
				else
				{
					page5.framesEmpty++;
				}
				bool flag2 = page5.framesEmpty < 60;
				if (flag2)
				{
					bool flag3 = page2 != null;
					if (flag3)
					{
						page.next = page5;
					}
					else
					{
						page2 = page5;
					}
					page = page5;
				}
				else
				{
					bool flag4 = page3 != null;
					if (flag4)
					{
						page4.next = page5;
					}
					else
					{
						page3 = page5;
					}
					page4 = page5;
				}
				next = page5.next;
				page5.next = null;
			}
			this.m_FirstPage = page2;
			Page next2;
			for (Page page5 = page3; page5 != null; page5 = next2)
			{
				next2 = page5.next;
				page5.next = null;
				page5.Dispose();
			}
		}

		internal static void PrepareForGfxDeviceRecreate()
		{
			UIRenderDevice.m_ActiveDeviceCount++;
		}

		internal static void WrapUpGfxDeviceRecreate()
		{
			UIRenderDevice.m_ActiveDeviceCount--;
		}

		internal static void FlushAllPendingDeviceDisposes()
		{
			Utility.SyncRenderThread();
			UIRenderDevice.ProcessDeviceFreeQueue();
		}

		internal UIRenderDevice.AllocationStatistics GatherAllocationStatistics()
		{
			UIRenderDevice.AllocationStatistics allocationStatistics = default(UIRenderDevice.AllocationStatistics);
			allocationStatistics.freesDeferred = new int[this.m_DeferredFrees.Count];
			for (int i = 0; i < this.m_DeferredFrees.Count; i++)
			{
				allocationStatistics.freesDeferred[i] = this.m_DeferredFrees[i].Count;
			}
			int num = 0;
			for (Page page = this.m_FirstPage; page != null; page = page.next)
			{
				num++;
			}
			allocationStatistics.pages = new UIRenderDevice.AllocationStatistics.PageStatistics[num];
			num = 0;
			for (Page page = this.m_FirstPage; page != null; page = page.next)
			{
				allocationStatistics.pages[num].vertices = page.vertices.allocator.GatherStatistics();
				allocationStatistics.pages[num].indices = page.indices.allocator.GatherStatistics();
				num++;
			}
			return allocationStatistics;
		}

		internal UIRenderDevice.DrawStatistics GatherDrawStatistics()
		{
			return this.m_DrawStats;
		}

		public static void ProcessDeviceFreeQueue()
		{
			bool synchronousFree = UIRenderDevice.m_SynchronousFree;
			if (synchronousFree)
			{
				Utility.SyncRenderThread();
			}
			for (LinkedListNode<UIRenderDevice.DeviceToFree> linkedListNode = UIRenderDevice.m_DeviceFreeQueue.First; linkedListNode != null; linkedListNode = UIRenderDevice.m_DeviceFreeQueue.First)
			{
				bool flag = !Utility.CPUFencePassed(linkedListNode.Value.handle);
				if (flag)
				{
					break;
				}
				linkedListNode.Value.Dispose();
				UIRenderDevice.m_DeviceFreeQueue.RemoveFirst();
			}
			Debug.Assert(!UIRenderDevice.m_SynchronousFree || UIRenderDevice.m_DeviceFreeQueue.Count == 0);
			bool flag2 = UIRenderDevice.m_ActiveDeviceCount == 0 && UIRenderDevice.m_SubscribedToNotifications;
			if (flag2)
			{
				Utility.NotifyOfUIREvents(false);
				UIRenderDevice.m_SubscribedToNotifications = false;
			}
		}

		private static void OnEngineUpdateGlobal()
		{
			UIRenderDevice.ProcessDeviceFreeQueue();
		}

		private static void OnFlushPendingResources()
		{
			UIRenderDevice.m_SynchronousFree = true;
			UIRenderDevice.ProcessDeviceFreeQueue();
		}

		internal const uint k_MaxQueuedFrameCount = 4U;

		internal const int k_PruneEmptyPageFrameCount = 60;

		private IntPtr m_DefaultStencilState;

		private IntPtr m_VertexDecl;

		private Page m_FirstPage;

		private uint m_NextPageVertexCount;

		private uint m_LargeMeshVertexCount;

		private float m_IndexToVertexCountRatio;

		private List<List<UIRenderDevice.AllocToFree>> m_DeferredFrees;

		private List<List<UIRenderDevice.AllocToUpdate>> m_Updates;

		private CommandListManager m_CommandListManager;

		private uint[] m_Fences;

		private MaterialPropertyBlock m_ConstantProps;

		private MaterialPropertyBlock m_BatchProps;

		private uint m_FrameIndex;

		private uint m_NextUpdateID = 1U;

		private UIRenderDevice.DrawStatistics m_DrawStats;

		private readonly LinkedPool<MeshHandle> m_MeshHandles = new LinkedPool<MeshHandle>(() => new MeshHandle(), delegate(MeshHandle mh)
		{
		}, 10000);

		private readonly DrawParams m_DrawParams = new DrawParams();

		private readonly TextureSlotManager m_TextureSlotManager = new TextureSlotManager();

		private HashSet<Material> m_ScreenSpaceAlteredMaterials = new HashSet<Material>();

		private static LinkedList<UIRenderDevice.DeviceToFree> m_DeviceFreeQueue = new LinkedList<UIRenderDevice.DeviceToFree>();

		private static int m_ActiveDeviceCount = 0;

		private static bool m_SubscribedToNotifications;

		private static bool m_SynchronousFree;

		private static readonly int s_GradientSettingsTexID = Shader.PropertyToID("_GradientSettingsTex");

		private static readonly int s_ShaderInfoTexID = Shader.PropertyToID("_ShaderInfoTex");

		private static ProfilerMarker s_MarkerAllocate = new ProfilerMarker("UIR.Allocate");

		private static ProfilerMarker s_MarkerFree = new ProfilerMarker("UIR.Free");

		private static ProfilerMarker s_MarkerAdvanceFrame = new ProfilerMarker("UIR.AdvanceFrame");

		private static ProfilerMarker s_MarkerFence = new ProfilerMarker("UIR.WaitOnFence");

		private static ProfilerMarker s_MarkerBeforeDraw = new ProfilerMarker("UIR.BeforeDraw");

		private static readonly int[] s_EvaluationFlagsToTextureSlotCount = new int[] { -1, 1, 2, 4, 8, -1, -1, -1 };

		private static readonly int[] s_TextureSlotCountToEvaluationFlags = new int[] { -1, 1, 2, -1, 3, -1, -1, -1, 4 };

		public static class Testing
		{
			public static CommandListManager GetCommandListManager(UIRenderDevice device)
			{
				return device.m_CommandListManager;
			}
		}

		internal struct AllocToUpdate
		{
			public uint id;

			public uint allocTime;

			public MeshHandle meshHandle;

			public Alloc permAllocVerts;

			public Alloc permAllocIndices;

			public Page permPage;

			public bool copyBackIndices;
		}

		private struct AllocToFree
		{
			public Alloc alloc;

			public Page page;

			public bool vertices;
		}

		private struct DeviceToFree
		{
			public void Dispose()
			{
				while (this.page != null)
				{
					Page page = this.page;
					this.page = this.page.next;
					page.Dispose();
				}
				this.commandListManager.Dispose();
			}

			public uint handle;

			public Page page;

			public CommandListManager commandListManager;
		}

		[Flags]
		private enum EvaluationFlags
		{
			None = 0,
			MustApplyMaterial = 1,
			MustApplyBatchProps = 2,
			MustApplyStencil = 4,
			ForceRenderTypeBitOffset = 3,
			ForceRenderTypeSolid = 8,
			ForceRenderTypeTextured = 16,
			ForceRenderTypeText = 24,
			ForceRenderTypeSvgGradient = 32,
			ForceRenderTypeBits = 56,
			TextureSlotCountBitOffset = 6,
			TextureSlotCount1 = 64,
			TextureSlotCount2 = 128,
			TextureSlotCount4 = 192,
			TextureSlotCount8 = 256,
			TextureSlotCountBits = 448,
			IsSerializing = 512,
			IsRenderingNestedTreeRT = 1024
		}

		private struct EvaluationState
		{
			public CommandList activeCommandList;

			public MaterialPropertyBlock constantProps;

			public MaterialPropertyBlock batchProps;

			public MaterialPropertyBlock userProps;

			public Material material;

			public int stencilRef;

			public Page curPage;

			public UIRenderDevice.EvaluationFlags flags;

			public VisualElement commandListOwner;
		}

		internal struct AllocationStatistics
		{
			public UIRenderDevice.AllocationStatistics.PageStatistics[] pages;

			public int[] freesDeferred;

			public struct PageStatistics
			{
				internal HeapStatistics vertices;

				internal HeapStatistics indices;
			}
		}

		internal struct DrawStatistics
		{
			public int currentFrameIndex;

			public uint totalIndices;

			public uint commandCount;

			public uint skippedCommandCount;

			public uint drawCommandCount;

			public uint disableCommandCount;

			public uint materialSetCount;

			public uint drawRangeCount;

			public uint drawRangeCallCount;

			public uint immediateDraws;

			public uint stencilRefChanges;
		}
	}
}
