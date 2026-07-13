using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.UIR
{
	internal class CommandList : IDisposable
	{
		public CommandList(IntPtr vertexDecl, IntPtr stencilState)
		{
			this.m_VertexDecl = vertexDecl;
			this.m_StencilState = stencilState;
			this.m_DrawRanges = new NativeList<DrawBufferRange>(1024, CommandList.k_MemoryLabel);
			this.handle = GCHandle.Alloc(this);
		}

		public int Count
		{
			get
			{
				return this.m_Commands.Count;
			}
		}

		public void Reset()
		{
			this.m_Owner = null;
			this.m_Renderer = null;
			this.m_Material = null;
			this.m_Commands.Clear();
			this.m_DrawRanges.Clear();
			this.constantProps.Clear();
		}

		public void Init(VisualElement owner, Material material, CommandFlags commandFlags)
		{
			Debug.Assert(this.m_Owner == null);
			this.m_Owner = owner;
			UIDocumentRootElement uidocumentRootElement = owner as UIDocumentRootElement;
			this.m_Renderer = ((uidocumentRootElement != null) ? uidocumentRootElement.uiRenderer : null);
			this.m_Material = material;
			this.flags = commandFlags;
			for (int i = 0; i < this.m_GpuTextureData.Length; i++)
			{
				this.m_GpuTextureData[i] = Vector4.zero;
			}
		}

		public unsafe void Execute()
		{
			IntPtr* ptr;
			int num;
			int* ptr2;
			IntPtr* ptr3;
			IntPtr intPtr;
			checked
			{
				ptr = stackalloc IntPtr[unchecked((UIntPtr)1) * (UIntPtr)sizeof(IntPtr)];
				Utility.SetPropertyBlock(this.constantProps);
				Utility.SetStencilState(this.m_StencilState, 0);
				num = 0;
				ptr2 = stackalloc int[unchecked((UIntPtr)32)];
				ptr3 = stackalloc IntPtr[unchecked((UIntPtr)8) * (UIntPtr)sizeof(IntPtr)];
				intPtr = Utility.AllocateShaderPropertySheet();
			}
			try
			{
				for (int i = 0; i < this.m_Commands.Count; i++)
				{
					SerializedCommand serializedCommand = this.m_Commands[i];
					switch (serializedCommand.type)
					{
					case SerializedCommandType.DrawRanges:
						*ptr = serializedCommand.vertexBuffer;
						Utility.DrawRanges(serializedCommand.indexBuffer, ptr, 1, new IntPtr(this.m_DrawRanges.GetSlice(serializedCommand.firstRange, serializedCommand.rangeCount).GetUnsafePtr<DrawBufferRange>()), serializedCommand.rangeCount, this.m_VertexDecl);
						break;
					case SerializedCommandType.SetTexture:
						ptr2[num] = serializedCommand.textureName;
						ptr3[(IntPtr)num * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = serializedCommand.texturePtr;
						num++;
						this.m_GpuTextureData[serializedCommand.gpuDataOffset] = serializedCommand.gpuData0;
						this.m_GpuTextureData[serializedCommand.gpuDataOffset + 1] = serializedCommand.gpuData1;
						break;
					case SerializedCommandType.ApplyBatchProps:
						Utility.SetAllTextures(intPtr, new IntPtr((void*)ptr2), new IntPtr((void*)ptr3), num);
						num = 0;
						Utility.SetVectorArray(intPtr, TextureSlotManager.textureTableId, this.m_GpuTextureData);
						Utility.ApplyShaderPropertySheet(intPtr);
						break;
					case SerializedCommandType.ApplyUserProps:
						Utility.SetPropertyBlock(serializedCommand.userProps);
						break;
					default:
						throw new NotImplementedException();
					}
				}
			}
			finally
			{
				Utility.ReleasePropertySheet(intPtr);
			}
		}

		public void SetTexture(int name, Texture texture, int gpuDataOffset, Vector4 gpuData0, Vector4 gpuData1)
		{
			SerializedCommand serializedCommand = new SerializedCommand
			{
				type = SerializedCommandType.SetTexture,
				textureName = name,
				texturePtr = Object.MarshalledUnityObject.MarshalNotNull<Texture>(texture),
				gpuDataOffset = gpuDataOffset,
				gpuData0 = gpuData0,
				gpuData1 = gpuData1
			};
			this.m_Commands.Add(serializedCommand);
		}

		public void ApplyUserProps(MaterialPropertyBlock userProps)
		{
			SerializedCommand serializedCommand = new SerializedCommand
			{
				type = SerializedCommandType.ApplyUserProps,
				userProps = userProps
			};
			this.m_Commands.Add(serializedCommand);
		}

		public void ApplyBatchProps()
		{
			SerializedCommand serializedCommand = new SerializedCommand
			{
				type = SerializedCommandType.ApplyBatchProps
			};
			this.m_Commands.Add(serializedCommand);
		}

		public void DrawRanges(Utility.GPUBuffer<ushort> ib, Utility.GPUBuffer<Vertex> vb, NativeSlice<DrawBufferRange> ranges)
		{
			SerializedCommand serializedCommand = new SerializedCommand
			{
				type = SerializedCommandType.DrawRanges,
				vertexBuffer = vb.BufferPointer,
				indexBuffer = ib.BufferPointer,
				firstRange = this.m_DrawRanges.Count,
				rangeCount = ranges.Length
			};
			this.m_Commands.Add(serializedCommand);
			this.m_DrawRanges.Add(ranges);
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_DrawRanges.Dispose();
					this.m_DrawRanges = null;
					bool isAllocated = this.handle.IsAllocated;
					if (isAllocated)
					{
						this.handle.Free();
					}
				}
				this.disposed = true;
			}
		}

		private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.CommandList", Allocator.Persistent);

		public VisualElement m_Owner;

		public UIRenderer m_Renderer;

		private readonly IntPtr m_VertexDecl;

		private readonly IntPtr m_StencilState;

		public MaterialPropertyBlock constantProps = new MaterialPropertyBlock();

		public GCHandle handle;

		public Material m_Material;

		public CommandFlags flags;

		private List<SerializedCommand> m_Commands = new List<SerializedCommand>();

		private Vector4[] m_GpuTextureData = new Vector4[TextureSlotManager.k_SlotSize * TextureSlotManager.k_MaxSlotCount];

		private NativeList<DrawBufferRange> m_DrawRanges;
	}
}
