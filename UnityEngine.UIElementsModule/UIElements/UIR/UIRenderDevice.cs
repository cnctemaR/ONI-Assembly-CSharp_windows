using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine.Rendering;

namespace UnityEngine.UIElements.UIR
{
	internal class UIRenderDevice : IDisposable
	{
		internal uint maxVerticesPerPage { get; } = 65535U;

		static UIRenderDevice()
		{
			Utility.EngineUpdate += UIRenderDevice.OnEngineUpdateGlobal;
			Utility.FlushPendingResources += UIRenderDevice.OnFlushPendingResources;
		}

		public UIRenderDevice(Shader defaultMaterialShader, uint initialVertexCapacity = 0U, uint initialIndexCapacity = 0U, UIRenderDevice.DrawingModes drawingMode = UIRenderDevice.DrawingModes.FlipY, int drawRangeRingSize = 1024)
			: this(defaultMaterialShader, initialVertexCapacity, initialIndexCapacity, drawingMode, drawRangeRingSize, false)
		{
		}

		protected UIRenderDevice(uint initialVertexCapacity = 0U, uint initialIndexCapacity = 0U, UIRenderDevice.DrawingModes drawingMode = UIRenderDevice.DrawingModes.FlipY, int drawRangeRingSize = 1024)
			: this(null, initialVertexCapacity, initialIndexCapacity, drawingMode, drawRangeRingSize, true)
		{
		}

		private UIRenderDevice(Shader defaultMaterialShader, uint initialVertexCapacity, uint initialIndexCapacity, UIRenderDevice.DrawingModes drawingMode, int drawRangeRingSize, bool mockDevice)
		{
			this.m_MockDevice = mockDevice;
			Debug.Assert(!UIRenderDevice.m_SynchronousFree);
			Debug.Assert(true);
			bool flag = UIRenderDevice.m_ActiveDeviceCount++ == 0;
			if (flag)
			{
				bool flag2 = !UIRenderDevice.m_SubscribedToNotifications && !this.m_MockDevice;
				if (flag2)
				{
					Utility.NotifyOfUIREvents(true);
					UIRenderDevice.m_SubscribedToNotifications = true;
				}
			}
			this.m_DefaultMaterialShader = defaultMaterialShader;
			this.m_DrawingMode = drawingMode;
			this.m_NextPageVertexCount = Math.Max(initialVertexCapacity, 2048U);
			this.m_LargeMeshVertexCount = this.m_NextPageVertexCount;
			this.m_IndexToVertexCountRatio = initialIndexCapacity / initialVertexCapacity;
			this.m_IndexToVertexCountRatio = Mathf.Max(this.m_IndexToVertexCountRatio, 2f);
			this.m_LazyCreationDrawRangeRingSize = (Mathf.IsPowerOfTwo(drawRangeRingSize) ? drawRangeRingSize : Mathf.NextPowerOfTwo(drawRangeRingSize));
			this.m_DeferredFrees = new List<List<UIRenderDevice.AllocToFree>>(4);
			this.m_Updates = new List<List<UIRenderDevice.AllocToUpdate>>(4);
			int num = 0;
			while ((long)num < 4L)
			{
				this.m_DeferredFrees.Add(new List<UIRenderDevice.AllocToFree>());
				this.m_Updates.Add(new List<UIRenderDevice.AllocToUpdate>());
				num++;
			}
			this.m_APIUsesStraightYCoordinateSystem = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3;
		}

		internal static Texture2D whiteTexel
		{
			get
			{
				bool flag = UIRenderDevice.s_WhiteTexel == null;
				if (flag)
				{
					UIRenderDevice.s_WhiteTexel = new Texture2D(1, 1, TextureFormat.RGBA32, false);
					UIRenderDevice.s_WhiteTexel.hideFlags = HideFlags.HideAndDontSave;
					UIRenderDevice.s_WhiteTexel.filterMode = FilterMode.Bilinear;
					UIRenderDevice.s_WhiteTexel.SetPixel(0, 0, Color.white);
					UIRenderDevice.s_WhiteTexel.Apply(false, true);
				}
				return UIRenderDevice.s_WhiteTexel;
			}
		}

		internal static Texture2D defaultShaderInfoTexFloat
		{
			get
			{
				bool flag = UIRenderDevice.s_DefaultShaderInfoTexFloat == null;
				if (flag)
				{
					UIRenderDevice.s_DefaultShaderInfoTexFloat = new Texture2D(64, 64, TextureFormat.RGBAFloat, false);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.hideFlags = HideFlags.HideAndDontSave;
					UIRenderDevice.s_DefaultShaderInfoTexFloat.filterMode = FilterMode.Point;
					UIRenderDevice.s_DefaultShaderInfoTexFloat.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y, UIRVEShaderInfoAllocator.identityTransformRow0Value);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y + 1, UIRVEShaderInfoAllocator.identityTransformRow1Value);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.SetPixel(UIRVEShaderInfoAllocator.identityTransformTexel.x, UIRVEShaderInfoAllocator.identityTransformTexel.y + 2, UIRVEShaderInfoAllocator.identityTransformRow2Value);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.SetPixel(UIRVEShaderInfoAllocator.infiniteClipRectTexel.x, UIRVEShaderInfoAllocator.infiniteClipRectTexel.y, UIRVEShaderInfoAllocator.infiniteClipRectValue);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.SetPixel(UIRVEShaderInfoAllocator.fullOpacityTexel.x, UIRVEShaderInfoAllocator.fullOpacityTexel.y, UIRVEShaderInfoAllocator.fullOpacityValue);
					UIRenderDevice.s_DefaultShaderInfoTexFloat.Apply(false, true);
				}
				return UIRenderDevice.s_DefaultShaderInfoTexFloat;
			}
		}

		internal static Texture2D defaultShaderInfoTexARGB8
		{
			get
			{
				bool flag = UIRenderDevice.s_DefaultShaderInfoTexARGB8 == null;
				if (flag)
				{
					UIRenderDevice.s_DefaultShaderInfoTexARGB8 = new Texture2D(64, 64, TextureFormat.RGBA32, false);
					UIRenderDevice.s_DefaultShaderInfoTexARGB8.hideFlags = HideFlags.HideAndDontSave;
					UIRenderDevice.s_DefaultShaderInfoTexARGB8.filterMode = FilterMode.Point;
					UIRenderDevice.s_DefaultShaderInfoTexARGB8.SetPixel(UIRVEShaderInfoAllocator.fullOpacityTexel.x, UIRVEShaderInfoAllocator.fullOpacityTexel.y, UIRVEShaderInfoAllocator.fullOpacityValue);
					UIRenderDevice.s_DefaultShaderInfoTexARGB8.Apply(false, true);
				}
				return UIRenderDevice.s_DefaultShaderInfoTexARGB8;
			}
		}

		internal static bool vertexTexturingIsAvailable
		{
			get
			{
				bool flag = UIRenderDevice.s_VertexTexturingIsAvailable == null;
				if (flag)
				{
					Shader shader = Shader.Find(UIRUtility.k_DefaultShaderName);
					Material material = new Material(shader);
					material.hideFlags |= HideFlags.DontSaveInEditor;
					string tag = material.GetTag("UIE_VertexTexturingIsAvailable", false);
					UIRUtility.Destroy(material);
					UIRenderDevice.s_VertexTexturingIsAvailable = new bool?(tag == "1");
				}
				return UIRenderDevice.s_VertexTexturingIsAvailable.Value;
			}
		}

		private void CompleteCreation()
		{
			bool isCreated = this.m_DrawRanges.IsCreated;
			if (!isCreated)
			{
				this.m_DrawRanges = new NativeArray<DrawBufferRange>(this.m_LazyCreationDrawRangeRingSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.m_Fences = (this.m_MockDevice ? null : new uint[4]);
			}
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
					bool flag = this.m_DefaultMaterial != null;
					if (flag)
					{
						bool isPlaying = Application.isPlaying;
						if (isPlaying)
						{
							Object.Destroy(this.m_DefaultMaterial);
						}
						else
						{
							Object.DestroyImmediate(this.m_DefaultMaterial);
						}
					}
					UIRenderDevice.DeviceToFree deviceToFree = new UIRenderDevice.DeviceToFree
					{
						handle = (this.m_MockDevice ? 0U : Utility.InsertCPUFence()),
						page = this.m_FirstPage,
						drawRanges = this.m_DrawRanges
					};
					bool flag2 = deviceToFree.handle == 0U;
					if (flag2)
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
			}
			else
			{
				UIRenderDevice.AllocToUpdate allocToUpdate;
				this.UpdateAfterGPUUsedData(mesh, vertexCount, indexCount, out vertexData, out indexData, out indexOffset, out allocToUpdate, true);
			}
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
				else
				{
					this.CompleteCreation();
				}
				bool flag4 = alloc2.size == 0U;
				if (flag4)
				{
					this.m_NextPageVertexCount <<= 1;
					this.m_NextPageVertexCount = Math.Max(this.m_NextPageVertexCount, vertexCount * 2U);
					this.m_NextPageVertexCount = Math.Min(this.m_NextPageVertexCount, this.maxVerticesPerPage);
					uint num = (uint)(this.m_NextPageVertexCount * this.m_IndexToVertexCountRatio + 0.5f);
					num = Math.Max(num, indexCount * 2U);
					Debug.Assert(((page != null) ? page.next : null) == null);
					page = new Page(this.m_NextPageVertexCount, num, 4U, this.m_MockDevice);
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
				this.CompleteCreation();
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
					uint num5 = ((vertexCount > this.maxVerticesPerPage) ? 2U : vertexCount);
					Debug.Assert(vertexCount <= this.maxVerticesPerPage, string.Format("Requested Vertex count ({0}) is above the limit ({1}). Alloc will fail.", vertexCount, this.maxVerticesPerPage));
					page = new Page(num5, indexCount, 4U, this.m_MockDevice);
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
			Debug.Assert(alloc.size == vertexCount, string.Format("Vertices allocated ({0}) != Vertices requested ({1})", alloc.size, vertexCount));
			Debug.Assert(alloc2.size == indexCount, string.Format("Indices allocated ({0}) != Indices requested ({1})", alloc2.size, indexCount));
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

		public Shader standardShader
		{
			get
			{
				return this.m_DefaultMaterialShader;
			}
			set
			{
				bool flag = this.m_DefaultMaterialShader != value;
				if (flag)
				{
					this.m_DefaultMaterialShader = value;
					UIRUtility.Destroy(this.m_DefaultMaterial);
					this.m_DefaultMaterial = null;
				}
			}
		}

		public Material GetStandardMaterial()
		{
			bool flag = this.m_DefaultMaterial == null && this.m_DefaultMaterialShader != null;
			if (flag)
			{
				this.m_DefaultMaterial = new Material(this.m_DefaultMaterialShader);
				UIRenderDevice.SetupStandardMaterial(this.m_DefaultMaterial, this.m_DrawingMode);
			}
			return this.m_DefaultMaterial;
		}

		private static void SetupStandardMaterial(Material material, UIRenderDevice.DrawingModes mode)
		{
			material.hideFlags |= HideFlags.DontSaveInEditor;
			bool flag = mode == UIRenderDevice.DrawingModes.StraightY;
			if (flag)
			{
				material.SetInt("_StencilCompFront", 3);
				material.SetInt("_StencilPassFront", 0);
				material.SetInt("_StencilZFailFront", 1);
				material.SetInt("_StencilFailFront", 0);
				material.SetInt("_StencilCompBack", 8);
				material.SetInt("_StencilPassBack", 0);
				material.SetInt("_StencilZFailBack", 2);
				material.SetInt("_StencilFailBack", 0);
			}
			else
			{
				bool flag2 = mode == UIRenderDevice.DrawingModes.FlipY;
				if (flag2)
				{
					material.SetInt("_StencilCompFront", 8);
					material.SetInt("_StencilPassFront", 0);
					material.SetInt("_StencilZFailFront", 2);
					material.SetInt("_StencilFailFront", 0);
					material.SetInt("_StencilCompBack", 3);
					material.SetInt("_StencilPassBack", 0);
					material.SetInt("_StencilZFailBack", 1);
					material.SetInt("_StencilFailBack", 0);
				}
				else
				{
					bool flag3 = mode == UIRenderDevice.DrawingModes.DisableClipping;
					if (flag3)
					{
						material.SetInt("_StencilCompFront", 8);
						material.SetInt("_StencilPassFront", 0);
						material.SetInt("_StencilZFailFront", 0);
						material.SetInt("_StencilFailFront", 0);
						material.SetInt("_StencilCompBack", 8);
						material.SetInt("_StencilPassBack", 0);
						material.SetInt("_StencilZFailBack", 0);
						material.SetInt("_StencilFailBack", 0);
					}
				}
			}
		}

		private static void Set1PixelSizeOnMaterial(DrawParams drawParams, Material mat)
		{
			Vector4 vector = default(Vector4);
			RectInt activeViewport = Utility.GetActiveViewport();
			vector.x = 2f / (float)activeViewport.width;
			vector.y = 2f / (float)activeViewport.height;
			Vector3 vector2 = (drawParams.projection * drawParams.view.Peek().transform).inverse.MultiplyVector(new Vector3(vector.x, vector.y));
			vector.z = 1f / (Mathf.Abs(vector2.x) + Mathf.Epsilon);
			vector.w = 1f / (Mathf.Abs(vector2.y) + Mathf.Epsilon);
			mat.SetVector(UIRenderDevice.s_1PixelClipInvViewPropID, vector);
		}

		private void BeforeDraw()
		{
			this.AdvanceFrame();
			this.m_DrawStats = default(UIRenderDevice.DrawStatistics);
			this.m_DrawStats.currentFrameIndex = (int)this.m_FrameIndex;
			this.m_DrawStats.currentDrawRangeStart = this.m_DrawRangeStart;
			for (Page page = this.m_FirstPage; page != null; page = page.next)
			{
				page.vertices.SendUpdates();
				page.indices.SendUpdates();
			}
		}

		private void EvaluateChain(RenderChainCommand head, Rect viewport, Matrix4x4 projection, PanelClearFlags clearFlags, Texture atlas, Texture gradientSettings, Texture shaderInfo, float pixelsPerPoint, NativeArray<Transform3x4> transforms, NativeArray<Vector4> clipRects, ref Exception immediateException)
		{
			bool flag = this.m_APIUsesStraightYCoordinateSystem;
			bool invertProjectionMatrix = Utility.GetInvertProjectionMatrix();
			if (invertProjectionMatrix)
			{
				flag = !flag;
			}
			DrawParams drawParams = this.m_DrawParams;
			drawParams.Reset(viewport, projection);
			Material material = null;
			bool flag2 = !this.m_MockDevice;
			if (flag2)
			{
				material = this.GetStandardMaterial();
				material.mainTexture = atlas;
				material.SetTexture(UIRenderDevice.s_GradientSettingsTexID, gradientSettings);
				material.SetTexture(UIRenderDevice.s_ShaderInfoTexID, shaderInfo);
				bool flag3 = transforms.Length > 0;
				if (flag3)
				{
					Utility.SetVectorArray<Transform3x4>(material, UIRenderDevice.s_TransformsPropID, transforms);
				}
				bool flag4 = clipRects.Length > 0;
				if (flag4)
				{
					Utility.SetVectorArray<Vector4>(material, UIRenderDevice.s_ClipRectsPropID, clipRects);
				}
				UIRenderDevice.Set1PixelSizeOnMaterial(drawParams, material);
				material.SetVector(UIRenderDevice.s_PixelClipRectPropID, drawParams.view.Peek().clipRect);
				bool flag5 = clearFlags > PanelClearFlags.None;
				if (flag5)
				{
					GL.Clear((clearFlags & PanelClearFlags.Depth) > PanelClearFlags.None, (clearFlags & PanelClearFlags.Color) > PanelClearFlags.None, Color.clear, 0.99f);
				}
				GL.modelview = drawParams.view.Peek().transform;
				GL.LoadProjectionMatrix(drawParams.projection);
			}
			NativeArray<DrawBufferRange> drawRanges = this.m_DrawRanges;
			int length = drawRanges.Length;
			int num = drawRanges.Length - 1;
			int drawRangeStart = this.m_DrawRangeStart;
			int num2 = 0;
			DrawBufferRange drawBufferRange = default(DrawBufferRange);
			Page page = null;
			State state = new State
			{
				material = this.m_DefaultMaterial
			};
			int num3 = -1;
			int num4 = 0;
			while (head != null)
			{
				this.m_DrawStats.commandCount = this.m_DrawStats.commandCount + 1U;
				this.m_DrawStats.drawCommandCount = this.m_DrawStats.drawCommandCount + ((head.type == CommandType.Draw) ? 1U : 0U);
				bool flag6 = head.type > CommandType.Draw;
				bool flag7 = true;
				bool flag8 = false;
				bool flag9 = !flag6;
				if (flag9)
				{
					flag8 = head.state.material != state.material;
					state.material = head.state.material;
					bool flag10 = head.state.custom != null;
					if (flag10)
					{
						flag8 |= head.state.custom != state.custom;
						state.custom = head.state.custom;
					}
					bool flag11 = head.state.font != null;
					if (flag11)
					{
						flag8 |= head.state.font != state.font;
						state.font = head.state.font;
					}
					flag6 = flag8 || head.mesh.allocPage != page;
					bool flag12 = !flag6;
					if (flag12)
					{
						flag7 = (long)num3 != (long)((ulong)head.mesh.allocIndices.start + (ulong)((long)head.indexOffset));
					}
				}
				bool flag13 = flag7;
				if (flag13)
				{
					bool flag14 = drawBufferRange.indexCount > 0;
					if (flag14)
					{
						int num5 = (drawRangeStart + num2++) & num;
						drawRanges[num5] = drawBufferRange;
						bool flag15 = num2 == length;
						if (flag15)
						{
							this.KickRanges(drawRanges, ref num2, ref drawRangeStart, length, page);
						}
						drawBufferRange = default(DrawBufferRange);
						this.m_DrawStats.drawRangeCount = this.m_DrawStats.drawRangeCount + 1U;
					}
					bool flag16 = head.type == CommandType.Draw;
					if (flag16)
					{
						drawBufferRange.firstIndex = (int)(head.mesh.allocIndices.start + (uint)head.indexOffset);
						drawBufferRange.indexCount = head.indexCount;
						drawBufferRange.vertsReferenced = (int)(head.mesh.allocVerts.start + head.mesh.allocVerts.size);
						drawBufferRange.minIndexVal = (int)head.mesh.allocVerts.start;
						num3 = drawBufferRange.firstIndex + head.indexCount;
						num4 = drawBufferRange.vertsReferenced + drawBufferRange.minIndexVal;
						this.m_DrawStats.totalIndices = this.m_DrawStats.totalIndices + (uint)head.indexCount;
					}
					bool flag17 = flag6;
					if (flag17)
					{
						this.KickRanges(drawRanges, ref num2, ref drawRangeStart, length, page);
						bool flag18 = head.type > CommandType.Draw;
						if (flag18)
						{
							bool flag19 = !this.m_MockDevice;
							if (flag19)
							{
								head.ExecuteNonDrawMesh(drawParams, flag, pixelsPerPoint, ref immediateException);
							}
							bool flag20 = head.type == CommandType.Immediate;
							if (flag20)
							{
								state.material = this.m_DefaultMaterial;
								this.m_DrawStats.immediateDraws = this.m_DrawStats.immediateDraws + 1U;
							}
						}
						else
						{
							page = head.mesh.allocPage;
						}
						bool flag21 = flag8;
						if (flag21)
						{
							bool flag22 = !this.m_MockDevice;
							if (flag22)
							{
								Material material2 = ((state.material != null) ? state.material : material);
								bool flag23 = material2 != material;
								if (flag23)
								{
									material2.mainTexture = atlas;
									material2.SetTexture(UIRenderDevice.s_GradientSettingsTexID, gradientSettings);
									material2.SetTexture(UIRenderDevice.s_ShaderInfoTexID, shaderInfo);
									bool flag24 = transforms.Length > 0;
									if (flag24)
									{
										Utility.SetVectorArray<Transform3x4>(material2, UIRenderDevice.s_TransformsPropID, transforms);
									}
									bool flag25 = clipRects.Length > 0;
									if (flag25)
									{
										Utility.SetVectorArray<Vector4>(material2, UIRenderDevice.s_ClipRectsPropID, clipRects);
									}
									UIRenderDevice.Set1PixelSizeOnMaterial(drawParams, material2);
									material2.SetVector(UIRenderDevice.s_PixelClipRectPropID, drawParams.view.Peek().clipRect);
								}
								else
								{
									bool flag26 = head.type == CommandType.PushView || head.type == CommandType.PopView;
									if (flag26)
									{
										UIRenderDevice.Set1PixelSizeOnMaterial(drawParams, material2);
										material2.SetVector(UIRenderDevice.s_PixelClipRectPropID, drawParams.view.Peek().clipRect);
									}
								}
								material2.SetTexture(UIRenderDevice.s_CustomTexPropID, state.custom);
								material2.SetTexture(UIRenderDevice.s_FontTexPropID, state.font);
								material2.SetPass(0);
							}
							this.m_DrawStats.materialSetCount = this.m_DrawStats.materialSetCount + 1U;
						}
						else
						{
							bool flag27 = head.type == CommandType.PushView || head.type == CommandType.PopView;
							if (flag27)
							{
								Material material3 = ((state.material != null) ? state.material : material);
								bool flag28 = !this.m_MockDevice;
								if (flag28)
								{
									UIRenderDevice.Set1PixelSizeOnMaterial(drawParams, material3);
									material3.SetVector(UIRenderDevice.s_PixelClipRectPropID, drawParams.view.Peek().clipRect);
									material3.SetPass(0);
								}
								this.m_DrawStats.materialSetCount = this.m_DrawStats.materialSetCount + 1U;
							}
						}
					}
					head = head.next;
				}
				else
				{
					bool flag29 = drawBufferRange.indexCount == 0;
					if (flag29)
					{
						num3 = (drawBufferRange.firstIndex = (int)(head.mesh.allocIndices.start + (uint)head.indexOffset));
					}
					num4 = Math.Max(num4, (int)(head.mesh.allocVerts.size + head.mesh.allocVerts.start));
					drawBufferRange.indexCount += head.indexCount;
					drawBufferRange.minIndexVal = Math.Min(drawBufferRange.minIndexVal, (int)head.mesh.allocVerts.start);
					drawBufferRange.vertsReferenced = num4 - drawBufferRange.minIndexVal;
					num3 += head.indexCount;
					this.m_DrawStats.totalIndices = this.m_DrawStats.totalIndices + (uint)head.indexCount;
					head = head.next;
				}
			}
			bool flag30 = drawBufferRange.indexCount > 0;
			if (flag30)
			{
				int num6 = (drawRangeStart + num2++) & num;
				drawRanges[num6] = drawBufferRange;
			}
			bool flag31 = num2 > 0;
			if (flag31)
			{
				this.KickRanges(drawRanges, ref num2, ref drawRangeStart, length, page);
			}
			this.m_DrawRangeStart = drawRangeStart;
		}

		private void KickRanges(NativeArray<DrawBufferRange> ranges, ref int rangesReady, ref int rangesStart, int rangesCount, Page curPage)
		{
			bool flag = rangesReady > 0;
			if (flag)
			{
				bool flag2 = rangesStart + rangesReady <= rangesCount;
				if (flag2)
				{
					bool flag3 = !this.m_MockDevice;
					if (flag3)
					{
						Utility.DrawRanges<ushort, Vertex>(curPage.indices.gpuData, curPage.vertices.gpuData, new NativeSlice<DrawBufferRange>(ranges, rangesStart, rangesReady));
					}
					this.m_DrawStats.drawRangeCallCount = this.m_DrawStats.drawRangeCallCount + 1U;
				}
				else
				{
					int num = ranges.Length - rangesStart;
					int num2 = rangesReady - num;
					bool flag4 = !this.m_MockDevice;
					if (flag4)
					{
						Utility.DrawRanges<ushort, Vertex>(curPage.indices.gpuData, curPage.vertices.gpuData, new NativeSlice<DrawBufferRange>(ranges, rangesStart, num));
						Utility.DrawRanges<ushort, Vertex>(curPage.indices.gpuData, curPage.vertices.gpuData, new NativeSlice<DrawBufferRange>(ranges, 0, num2));
					}
					this.m_DrawStats.drawRangeCallCount = this.m_DrawStats.drawRangeCallCount + 2U;
				}
				rangesStart = (rangesStart + rangesReady) & (rangesCount - 1);
				rangesReady = 0;
			}
		}

		public void DrawChain(RenderChainCommand head, Rect viewport, Matrix4x4 projection, PanelClearFlags clearFlags, Texture atlas, Texture gradientSettings, Texture shaderInfo, float pixelsPerPoint, NativeArray<Transform3x4> transforms, NativeArray<Vector4> clipRects, ref Exception immediateException)
		{
			bool flag = head == null;
			if (!flag)
			{
				this.BeforeDraw();
				Utility.ProfileDrawChainBegin();
				this.EvaluateChain(head, viewport, projection, clearFlags, atlas, gradientSettings, shaderInfo, pixelsPerPoint, transforms, clipRects, ref immediateException);
				Utility.ProfileDrawChainEnd();
				bool flag2 = this.m_Fences != null;
				if (flag2)
				{
					this.m_Fences[(int)((ulong)this.m_FrameIndex % (ulong)((long)this.m_Fences.Length))] = Utility.InsertCPUFence();
				}
			}
		}

		public void AdvanceFrame()
		{
			this.m_FrameIndex += 1U;
			this.m_DrawStats.currentFrameIndex = (int)this.m_FrameIndex;
			bool flag = this.m_Fences != null;
			if (flag)
			{
				int num = (int)((ulong)this.m_FrameIndex % (ulong)((long)this.m_Fences.Length));
				uint num2 = this.m_Fences[num];
				bool flag2 = num2 != 0U && !Utility.CPUFencePassed(num2);
				if (flag2)
				{
					Utility.WaitForCPUFencePassed(num2);
				}
				this.m_Fences[num] = 0U;
			}
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
				bool flag3 = allocToUpdate.meshHandle.updateAllocID == allocToUpdate.id && allocToUpdate.meshHandle.allocTime == allocToUpdate.allocTime;
				if (flag3)
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
			bool flag = UIRenderDevice.s_WhiteTexel != null;
			if (flag)
			{
				UIRUtility.Destroy(UIRenderDevice.s_WhiteTexel);
				UIRenderDevice.s_WhiteTexel = null;
			}
			bool flag2 = UIRenderDevice.s_DefaultShaderInfoTexFloat != null;
			if (flag2)
			{
				UIRUtility.Destroy(UIRenderDevice.s_DefaultShaderInfoTexFloat);
				UIRenderDevice.s_DefaultShaderInfoTexFloat = null;
			}
			bool flag3 = UIRenderDevice.s_DefaultShaderInfoTexARGB8 != null;
			if (flag3)
			{
				UIRUtility.Destroy(UIRenderDevice.s_DefaultShaderInfoTexARGB8);
				UIRenderDevice.s_DefaultShaderInfoTexARGB8 = null;
			}
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
			allocationStatistics.completeInit = this.m_DrawRanges.IsCreated;
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

		private static void ProcessDeviceFreeQueue()
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
				bool flag3 = UIRenderDevice.s_WhiteTexel != null;
				if (flag3)
				{
					UIRUtility.Destroy(UIRenderDevice.s_WhiteTexel);
					UIRenderDevice.s_WhiteTexel = null;
				}
				bool flag4 = UIRenderDevice.s_DefaultShaderInfoTexFloat != null;
				if (flag4)
				{
					UIRUtility.Destroy(UIRenderDevice.s_DefaultShaderInfoTexFloat);
					UIRenderDevice.s_DefaultShaderInfoTexFloat = null;
				}
				bool flag5 = UIRenderDevice.s_DefaultShaderInfoTexARGB8 != null;
				if (flag5)
				{
					UIRUtility.Destroy(UIRenderDevice.s_DefaultShaderInfoTexARGB8);
					UIRenderDevice.s_DefaultShaderInfoTexARGB8 = null;
				}
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

		private readonly bool m_MockDevice;

		private int m_LazyCreationDrawRangeRingSize;

		private Shader m_DefaultMaterialShader;

		private Material m_DefaultMaterial;

		private UIRenderDevice.DrawingModes m_DrawingMode;

		private Page m_FirstPage;

		private uint m_NextPageVertexCount;

		private uint m_LargeMeshVertexCount;

		private float m_IndexToVertexCountRatio;

		private List<List<UIRenderDevice.AllocToFree>> m_DeferredFrees;

		private List<List<UIRenderDevice.AllocToUpdate>> m_Updates;

		private uint[] m_Fences;

		private NativeArray<DrawBufferRange> m_DrawRanges;

		private int m_DrawRangeStart;

		private uint m_FrameIndex;

		private uint m_NextUpdateID = 1U;

		private UIRenderDevice.DrawStatistics m_DrawStats;

		private bool m_APIUsesStraightYCoordinateSystem;

		private readonly Pool<MeshHandle> m_MeshHandles = new Pool<MeshHandle>();

		private readonly DrawParams m_DrawParams = new DrawParams();

		private static LinkedList<UIRenderDevice.DeviceToFree> m_DeviceFreeQueue = new LinkedList<UIRenderDevice.DeviceToFree>();

		private static int m_ActiveDeviceCount = 0;

		private static bool m_SubscribedToNotifications;

		private static bool m_SynchronousFree;

		private static readonly int s_FontTexPropID = Shader.PropertyToID("_FontTex");

		private static readonly int s_CustomTexPropID = Shader.PropertyToID("_CustomTex");

		private static readonly int s_1PixelClipInvViewPropID = Shader.PropertyToID("_1PixelClipInvView");

		private static readonly int s_GradientSettingsTexID = Shader.PropertyToID("_GradientSettingsTex");

		private static readonly int s_ShaderInfoTexID = Shader.PropertyToID("_ShaderInfoTex");

		private static readonly int s_PixelClipRectPropID = Shader.PropertyToID("_PixelClipRect");

		private static readonly int s_TransformsPropID = Shader.PropertyToID("_Transforms");

		private static readonly int s_ClipRectsPropID = Shader.PropertyToID("_ClipRects");

		private static ProfilerMarker s_MarkerAllocate = new ProfilerMarker("UIR.Allocate");

		private static ProfilerMarker s_MarkerFree = new ProfilerMarker("UIR.Free");

		private static ProfilerMarker s_MarkerAdvanceFrame = new ProfilerMarker("UIR.AdvanceFrame");

		private static ProfilerMarker s_MarkerFence = new ProfilerMarker("UIR.WaitOnFence");

		private static ProfilerMarker s_MarkerBeforeDraw = new ProfilerMarker("UIR.BeforeDraw");

		private static bool? s_VertexTexturingIsAvailable;

		private const string k_VertexTexturingIsAvailableTag = "UIE_VertexTexturingIsAvailable";

		private const string k_VertexTexturingIsAvailableTrue = "1";

		private static Texture2D s_WhiteTexel;

		private static Texture2D s_DefaultShaderInfoTexFloat;

		private static Texture2D s_DefaultShaderInfoTexARGB8;

		private struct AllocToUpdate
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
				bool isCreated = this.drawRanges.IsCreated;
				if (isCreated)
				{
					this.drawRanges.Dispose();
				}
			}

			public uint handle;

			public Page page;

			public NativeArray<DrawBufferRange> drawRanges;
		}

		public enum DrawingModes
		{
			FlipY,
			StraightY,
			DisableClipping
		}

		internal struct AllocationStatistics
		{
			public UIRenderDevice.AllocationStatistics.PageStatistics[] pages;

			public int[] freesDeferred;

			public bool completeInit;

			public struct PageStatistics
			{
				internal HeapStatistics vertices;

				internal HeapStatistics indices;
			}
		}

		internal struct DrawStatistics
		{
			public int currentFrameIndex;

			public int currentDrawRangeStart;

			public uint totalIndices;

			public uint commandCount;

			public uint drawCommandCount;

			public uint materialSetCount;

			public uint drawRangeCount;

			public uint drawRangeCallCount;

			public uint immediateDraws;
		}
	}
}
