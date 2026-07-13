using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
	[RequiredByNativeCode]
	[ExcludeFromPreset]
	public sealed class Mesh : Object
	{
		[FreeFunction("MeshScripting::CreateMesh")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Mesh mono);

		[RequiredByNativeCode]
		public Mesh()
		{
			Mesh.Internal_Create(this);
		}

		[FreeFunction("MeshScripting::MeshFromInstanceId")]
		internal static Mesh FromInstanceID(EntityId id)
		{
			return Unmarshal.UnmarshalUnityObject<Mesh>(Mesh.FromInstanceID_Injected(ref id));
		}

		public IndexFormat indexFormat
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_indexFormat_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Mesh.set_indexFormat_Injected(intPtr, value);
			}
		}

		internal uint GetTotalIndexCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetTotalIndexCount_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::SetIndexBufferParams", HasExplicitThis = true, ThrowsException = true)]
		public void SetIndexBufferParams(int indexCount, IndexFormat format)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetIndexBufferParams_Injected(intPtr, indexCount, format);
		}

		[FreeFunction(Name = "MeshScripting::InternalSetIndexBufferData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetIndexBufferData(IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.InternalSetIndexBufferData_Injected(intPtr, data, dataStart, meshBufferStart, count, elemSize, flags);
		}

		[FreeFunction(Name = "MeshScripting::InternalSetIndexBufferDataFromArray", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetIndexBufferDataFromArray(Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.InternalSetIndexBufferDataFromArray_Injected(intPtr, data, dataStart, meshBufferStart, count, elemSize, flags);
		}

		[FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromPtr", HasExplicitThis = true, ThrowsException = true)]
		private void SetVertexBufferParamsFromPtr(int vertexCount, IntPtr attributesPtr, int attributesCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetVertexBufferParamsFromPtr_Injected(intPtr, vertexCount, attributesPtr, attributesCount);
		}

		[FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromArray", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetVertexBufferParamsFromArray(int vertexCount, params VertexAttributeDescriptor[] attributes)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
			fixed (VertexAttributeDescriptor* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Mesh.SetVertexBufferParamsFromArray_Injected(intPtr, vertexCount, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "MeshScripting::InternalSetVertexBufferData", HasExplicitThis = true)]
		private void InternalSetVertexBufferData(int stream, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.InternalSetVertexBufferData_Injected(intPtr, stream, data, dataStart, meshBufferStart, count, elemSize, flags);
		}

		[FreeFunction(Name = "MeshScripting::InternalSetVertexBufferDataFromArray", HasExplicitThis = true)]
		private void InternalSetVertexBufferDataFromArray(int stream, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.InternalSetVertexBufferDataFromArray_Injected(intPtr, stream, data, dataStart, meshBufferStart, count, elemSize, flags);
		}

		[FreeFunction(Name = "MeshScripting::GetVertexAttributesAlloc", HasExplicitThis = true)]
		private Array GetVertexAttributesAlloc()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributesAlloc_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetVertexAttributesArray", HasExplicitThis = true)]
		private unsafe int GetVertexAttributesArray([NotNull] VertexAttributeDescriptor[] attributes)
		{
			if (attributes == null)
			{
				ThrowHelper.ThrowArgumentNullException(attributes, "attributes");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
			int vertexAttributesArray_Injected;
			fixed (VertexAttributeDescriptor* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				vertexAttributesArray_Injected = Mesh.GetVertexAttributesArray_Injected(intPtr, ref managedSpanWrapper);
			}
			return vertexAttributesArray_Injected;
		}

		[FreeFunction(Name = "MeshScripting::GetVertexAttributesList", HasExplicitThis = true)]
		private unsafe int GetVertexAttributesList([NotNull] List<VertexAttributeDescriptor> attributes)
		{
			if (attributes == null)
			{
				ThrowHelper.ThrowArgumentNullException(attributes, "attributes");
			}
			int vertexAttributesList_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (VertexAttributeDescriptor[] array = NoAllocHelpers.ExtractArrayFromList<VertexAttributeDescriptor>(attributes))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, attributes.Count);
					vertexAttributesList_Injected = Mesh.GetVertexAttributesList_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<VertexAttributeDescriptor>(attributes);
			}
			return vertexAttributesList_Injected;
		}

		[FreeFunction(Name = "MeshScripting::GetVertexAttributesCount", HasExplicitThis = true)]
		private int GetVertexAttributeCountImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributeCountImpl_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetVertexAttributeByIndex", HasExplicitThis = true, ThrowsException = true)]
		public VertexAttributeDescriptor GetVertexAttribute(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VertexAttributeDescriptor vertexAttributeDescriptor;
			Mesh.GetVertexAttribute_Injected(intPtr, index, out vertexAttributeDescriptor);
			return vertexAttributeDescriptor;
		}

		[FreeFunction(Name = "MeshScripting::GetIndexStart", HasExplicitThis = true)]
		private uint GetIndexStartImpl(int submesh, int meshlod)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetIndexStartImpl_Injected(intPtr, submesh, meshlod);
		}

		[FreeFunction(Name = "MeshScripting::GetIndexCount", HasExplicitThis = true)]
		private uint GetIndexCountImpl(int submesh, int meshlod)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetIndexCountImpl_Injected(intPtr, submesh, meshlod);
		}

		[FreeFunction(Name = "MeshScripting::GetTrianglesCount", HasExplicitThis = true)]
		private uint GetTrianglesCountImpl(int submesh, int meshlod)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetTrianglesCountImpl_Injected(intPtr, submesh, meshlod);
		}

		[FreeFunction(Name = "MeshScripting::GetBaseVertex", HasExplicitThis = true)]
		private uint GetBaseVertexImpl(int submesh)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBaseVertexImpl_Injected(intPtr, submesh);
		}

		[FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
		private int[] GetTrianglesImpl(int submesh, bool applyBaseVertex, int meshlod)
		{
			int[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Mesh.GetTrianglesImpl_Injected(intPtr, submesh, applyBaseVertex, meshlod, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
		private int[] GetIndicesImpl(int submesh, bool applyBaseVertex, int meshlod)
		{
			int[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Mesh.GetIndicesImpl_Injected(intPtr, submesh, applyBaseVertex, meshlod, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true, ThrowsException = true)]
		private void SetIndicesImpl(int submesh, MeshTopology topology, IndexFormat indicesFormat, Array indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetIndicesImpl_Injected(intPtr, submesh, topology, indicesFormat, indices, arrayStart, arraySize, calculateBounds, baseVertex, meshlod);
		}

		[FreeFunction(Name = "SetMeshIndicesFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private void SetIndicesNativeArrayImpl(int submesh, MeshTopology topology, IndexFormat indicesFormat, IntPtr indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetIndicesNativeArrayImpl_Injected(intPtr, submesh, topology, indicesFormat, indices, arrayStart, arraySize, calculateBounds, baseVertex, meshlod);
		}

		[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray", HasExplicitThis = true)]
		private unsafe void GetTrianglesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (int[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetTrianglesNonAllocImpl_Injected(intPtr, out blittableArrayWrapper, submesh, applyBaseVertex, meshlod);
			}
			finally
			{
				int[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<int>(ref array);
			}
		}

		[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray16", HasExplicitThis = true)]
		private unsafe void GetTrianglesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (ushort[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetTrianglesNonAllocImpl16_Injected(intPtr, out blittableArrayWrapper, submesh, applyBaseVertex, meshlod);
			}
			finally
			{
				ushort[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<ushort>(ref array);
			}
		}

		[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray", HasExplicitThis = true)]
		private unsafe void GetIndicesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (int[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetIndicesNonAllocImpl_Injected(intPtr, out blittableArrayWrapper, submesh, applyBaseVertex, meshlod);
			}
			finally
			{
				int[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<int>(ref array);
			}
		}

		[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray16", HasExplicitThis = true)]
		private unsafe void GetIndicesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (ushort[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetIndicesNonAllocImpl16_Injected(intPtr, out blittableArrayWrapper, submesh, applyBaseVertex, meshlod);
			}
			finally
			{
				ushort[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<ushort>(ref array);
			}
		}

		[FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
		private void PrintErrorCantAccessChannel(VertexAttribute ch)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.PrintErrorCantAccessChannel_Injected(intPtr, ch);
		}

		[FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
		public bool HasVertexAttribute(VertexAttribute attr)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.HasVertexAttribute_Injected(intPtr, attr);
		}

		[FreeFunction(Name = "MeshScripting::GetChannelDimension", HasExplicitThis = true)]
		public int GetVertexAttributeDimension(VertexAttribute attr)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributeDimension_Injected(intPtr, attr);
		}

		[FreeFunction(Name = "MeshScripting::GetChannelFormat", HasExplicitThis = true)]
		public VertexAttributeFormat GetVertexAttributeFormat(VertexAttribute attr)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributeFormat_Injected(intPtr, attr);
		}

		[FreeFunction(Name = "MeshScripting::GetChannelStream", HasExplicitThis = true)]
		public int GetVertexAttributeStream(VertexAttribute attr)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributeStream_Injected(intPtr, attr);
		}

		[FreeFunction(Name = "MeshScripting::GetChannelOffset", HasExplicitThis = true)]
		public int GetVertexAttributeOffset(VertexAttribute attr)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexAttributeOffset_Injected(intPtr, attr);
		}

		[FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
		private void SetArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetArrayForChannelImpl_Injected(intPtr, channel, format, dim, values, arraySize, valuesStart, valuesCount, flags);
		}

		[FreeFunction(Name = "SetMeshComponentFromNativeArrayFromScript", HasExplicitThis = true)]
		private void SetNativeArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetNativeArrayForChannelImpl_Injected(intPtr, channel, format, dim, values, arraySize, valuesStart, valuesCount, flags);
		}

		[FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
		private Array GetAllocArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetAllocArrayFromChannelImpl_Injected(intPtr, channel, format, dim);
		}

		[FreeFunction(Name = "ExtractMeshComponentFromScript", HasExplicitThis = true)]
		private void GetArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.GetArrayFromChannelImpl_Injected(intPtr, channel, format, dim, values);
		}

		public int vertexBufferCount
		{
			[FreeFunction(Name = "MeshScripting::GetVertexBufferCount", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_vertexBufferCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "MeshScripting::GetVertexBufferStride", HasExplicitThis = true)]
		public int GetVertexBufferStride(int stream)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetVertexBufferStride_Injected(intPtr, stream);
		}

		[NativeThrows]
		[FreeFunction(Name = "MeshScripting::GetNativeVertexBufferPtr", HasExplicitThis = true)]
		public IntPtr GetNativeVertexBufferPtr(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetNativeVertexBufferPtr_Injected(intPtr, index);
		}

		[FreeFunction(Name = "MeshScripting::GetNativeIndexBufferPtr", HasExplicitThis = true)]
		public IntPtr GetNativeIndexBufferPtr()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetNativeIndexBufferPtr_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetVertexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
		private GraphicsBuffer GetVertexBufferImpl(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr vertexBufferImpl_Injected = Mesh.GetVertexBufferImpl_Injected(intPtr, index);
			return (vertexBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(vertexBufferImpl_Injected);
		}

		[FreeFunction(Name = "MeshScripting::GetIndexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
		private GraphicsBuffer GetIndexBufferImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr indexBufferImpl_Injected = Mesh.GetIndexBufferImpl_Injected(intPtr);
			return (indexBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(indexBufferImpl_Injected);
		}

		[FreeFunction(Name = "MeshScripting::GetBoneWeightBufferPtr", HasExplicitThis = true, ThrowsException = true)]
		private GraphicsBuffer GetBoneWeightBufferImpl(int bonesPerVertex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr boneWeightBufferImpl_Injected = Mesh.GetBoneWeightBufferImpl_Injected(intPtr, bonesPerVertex);
			return (boneWeightBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(boneWeightBufferImpl_Injected);
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeBufferPtr", HasExplicitThis = true, ThrowsException = true)]
		private GraphicsBuffer GetBlendShapeBufferImpl(int layout)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr blendShapeBufferImpl_Injected = Mesh.GetBlendShapeBufferImpl_Injected(intPtr, layout);
			return (blendShapeBufferImpl_Injected == 0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(blendShapeBufferImpl_Injected);
		}

		public GraphicsBuffer.Target vertexBufferTarget
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_vertexBufferTarget_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Mesh.set_vertexBufferTarget_Injected(intPtr, value);
			}
		}

		public GraphicsBuffer.Target indexBufferTarget
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_indexBufferTarget_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Mesh.set_indexBufferTarget_Injected(intPtr, value);
			}
		}

		public int blendShapeCount
		{
			[NativeMethod(Name = "GetBlendShapeChannelCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_blendShapeCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "MeshScripting::ClearBlendShapes", HasExplicitThis = true)]
		public void ClearBlendShapes()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.ClearBlendShapes_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeName", HasExplicitThis = true, ThrowsException = true)]
		public string GetBlendShapeName(int shapeIndex)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Mesh.GetBlendShapeName_Injected(intPtr, shapeIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeIndex", HasExplicitThis = true, ThrowsException = true)]
		public unsafe int GetBlendShapeIndex(string blendShapeName)
		{
			int blendShapeIndex_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(blendShapeName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = blendShapeName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				blendShapeIndex_Injected = Mesh.GetBlendShapeIndex_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return blendShapeIndex_Injected;
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameCount", HasExplicitThis = true, ThrowsException = true)]
		public int GetBlendShapeFrameCount(int shapeIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBlendShapeFrameCount_Injected(intPtr, shapeIndex);
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameWeight", HasExplicitThis = true, ThrowsException = true)]
		public float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBlendShapeFrameWeight_Injected(intPtr, shapeIndex, frameIndex);
		}

		[FreeFunction(Name = "GetBlendShapeFrameVerticesFromScript", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3> span = new Span<Vector3>(deltaVertices);
			fixed (Vector3* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<Vector3> span2 = new Span<Vector3>(deltaNormals);
				fixed (Vector3* ptr2 = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span2.Length);
					Span<Vector3> span3 = new Span<Vector3>(deltaTangents);
					fixed (Vector3* pinnableReference = span3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
						Mesh.GetBlendShapeFrameVertices_Injected(intPtr, shapeIndex, frameIndex, ref managedSpanWrapper, ref managedSpanWrapper2, ref managedSpanWrapper3);
						ptr = null;
						ptr2 = null;
					}
				}
			}
		}

		[FreeFunction(Name = "AddBlendShapeFrameFromScript", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void AddBlendShapeFrame(string shapeName, float frameWeight, ReadOnlySpan<Vector3> deltaVertices, ReadOnlySpan<Vector3> deltaNormals, ReadOnlySpan<Vector3> deltaTangents)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(shapeName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = shapeName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ReadOnlySpan<Vector3> readOnlySpan2 = deltaVertices;
				fixed (Vector3* ptr2 = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					ReadOnlySpan<Vector3> readOnlySpan3 = deltaNormals;
					fixed (Vector3* ptr3 = readOnlySpan3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, readOnlySpan3.Length);
						ReadOnlySpan<Vector3> readOnlySpan4 = deltaTangents;
						fixed (Vector3* ptr4 = readOnlySpan4.GetPinnableReference())
						{
							ManagedSpanWrapper managedSpanWrapper4 = new ManagedSpanWrapper((void*)ptr4, readOnlySpan4.Length);
							Mesh.AddBlendShapeFrame_Injected(intPtr, ref managedSpanWrapper, frameWeight, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4);
						}
					}
				}
			}
			finally
			{
				char* ptr = null;
				Vector3* ptr2 = null;
				Vector3* ptr3 = null;
				Vector3* ptr4 = null;
			}
		}

		public void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
		{
			this.AddBlendShapeFrame(shapeName, frameWeight, new ReadOnlySpan<Vector3>(deltaVertices), new ReadOnlySpan<Vector3>(deltaNormals), new ReadOnlySpan<Vector3>(deltaTangents));
		}

		[FreeFunction(Name = "MeshScripting::GetBlendShapeOffset", HasExplicitThis = true)]
		private BlendShape GetBlendShapeOffsetInternal(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BlendShape blendShape;
			Mesh.GetBlendShapeOffsetInternal_Injected(intPtr, index, out blendShape);
			return blendShape;
		}

		[NativeMethod("HasBoneWeights")]
		private bool HasBoneWeights()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.HasBoneWeights_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetBoneWeights", HasExplicitThis = true)]
		private BoneWeight[] GetBoneWeightsImpl()
		{
			BoneWeight[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Mesh.GetBoneWeightsImpl_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BoneWeight[] array;
				blittableArrayWrapper.Unmarshal<BoneWeight>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
		private unsafe void SetBoneWeightsImpl(BoneWeight[] weights)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<BoneWeight> span = new Span<BoneWeight>(weights);
			fixed (BoneWeight* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Mesh.SetBoneWeightsImpl_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		public void SetBoneWeights(NativeArray<byte> bonesPerVertex, NativeArray<BoneWeight1> weights)
		{
			this.InternalSetBoneWeights((IntPtr)bonesPerVertex.GetUnsafeReadOnlyPtr<byte>(), bonesPerVertex.Length, (IntPtr)weights.GetUnsafeReadOnlyPtr<BoneWeight1>(), weights.Length);
		}

		[SecurityCritical]
		[FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
		private void InternalSetBoneWeights(IntPtr bonesPerVertex, int bonesPerVertexSize, IntPtr weights, int weightsSize)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.InternalSetBoneWeights_Injected(intPtr, bonesPerVertex, bonesPerVertexSize, weights, weightsSize);
		}

		public unsafe NativeArray<BoneWeight1> GetAllBoneWeights()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BoneWeight1>((void*)this.GetAllBoneWeightsArray(), this.GetAllBoneWeightsArraySize(), Allocator.None);
		}

		public unsafe NativeArray<byte> GetBonesPerVertex()
		{
			int num = (this.HasBoneWeights() ? this.vertexCount : 0);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)this.GetBonesPerVertexArray(), num, Allocator.None);
		}

		[FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArraySize", HasExplicitThis = true)]
		private int GetAllBoneWeightsArraySize()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetAllBoneWeightsArraySize_Injected(intPtr);
		}

		[NativeMethod("GetBoneWeightBufferDimension")]
		private int GetBoneWeightBufferLayoutInternal()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBoneWeightBufferLayoutInternal_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArray", HasExplicitThis = true)]
		[SecurityCritical]
		private IntPtr GetAllBoneWeightsArray()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetAllBoneWeightsArray_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::GetBonesPerVertexArray", HasExplicitThis = true)]
		[SecurityCritical]
		private IntPtr GetBonesPerVertexArray()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBonesPerVertexArray_Injected(intPtr);
		}

		public int bindposeCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_bindposeCount_Injected(intPtr);
			}
		}

		[NativeName("BindPosesFromScript")]
		public unsafe Matrix4x4[] bindposes
		{
			get
			{
				Matrix4x4[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Mesh.get_bindposes_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Matrix4x4[] array;
					blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
					array2 = array;
				}
				return array2;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Matrix4x4> span = new Span<Matrix4x4>(value);
				fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Mesh.set_bindposes_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		public unsafe NativeArray<Matrix4x4> GetBindposes()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)this.GetBindposesArray(), this.bindposeCount, Allocator.None);
		}

		public void SetBindposes(NativeArray<Matrix4x4> poses)
		{
			bool flag = !poses.IsCreated || poses.Length == 0;
			if (flag)
			{
				throw new ArgumentException("Cannot set bindposes as the native poses array is empty.", "poses");
			}
			this.SetBindposesFromScript_NativeArray((IntPtr)poses.GetUnsafeReadOnlyPtr<Matrix4x4>(), poses.Length);
		}

		[NativeMethod("SetBindposes")]
		private void SetBindposesFromScript_NativeArray(IntPtr posesPtr, int posesCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetBindposesFromScript_NativeArray_Injected(intPtr, posesPtr, posesCount);
		}

		[FreeFunction(Name = "MeshScripting::GetBindposesArray", HasExplicitThis = true)]
		[SecurityCritical]
		private IntPtr GetBindposesArray()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetBindposesArray_Injected(intPtr);
		}

		[FreeFunction(Name = "MeshScripting::ExtractBoneWeightsIntoArray", HasExplicitThis = true)]
		private unsafe void GetBoneWeightsNonAllocImpl([Out] BoneWeight[] values)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (BoneWeight[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetBoneWeightsNonAllocImpl_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BoneWeight[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<BoneWeight>(ref array);
			}
		}

		[FreeFunction(Name = "MeshScripting::ExtractBindPosesIntoArray", HasExplicitThis = true)]
		private unsafe void GetBindposesNonAllocImpl([Out] Matrix4x4[] values)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (values != null)
				{
					fixed (Matrix4x4[] array = values)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetBindposesNonAllocImpl_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				Matrix4x4[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Matrix4x4>(ref array);
			}
		}

		public bool isReadable
		{
			[NativeMethod("GetIsReadable")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_isReadable_Injected(intPtr);
			}
		}

		internal bool canAccess
		{
			[NativeMethod("CanAccessFromScript")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_canAccess_Injected(intPtr);
			}
		}

		public int vertexCount
		{
			[NativeMethod("GetVertexCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_vertexCount_Injected(intPtr);
			}
		}

		public int subMeshCount
		{
			[NativeMethod(Name = "GetSubMeshCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Mesh.get_subMeshCount_Injected(intPtr);
			}
			[FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Mesh.set_subMeshCount_Injected(intPtr, value);
			}
		}

		[FreeFunction("MeshScripting::SetSubMesh", HasExplicitThis = true, ThrowsException = true)]
		public void SetSubMesh(int index, SubMeshDescriptor desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetSubMesh_Injected(intPtr, index, ref desc, flags);
		}

		[FreeFunction("MeshScripting::GetSubMesh", HasExplicitThis = true, ThrowsException = true)]
		public SubMeshDescriptor GetSubMesh(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SubMeshDescriptor subMeshDescriptor;
			Mesh.GetSubMesh_Injected(intPtr, index, out subMeshDescriptor);
			return subMeshDescriptor;
		}

		[FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromArray", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetAllSubMeshesAtOnceFromArray(SubMeshDescriptor[] desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<SubMeshDescriptor> span = new Span<SubMeshDescriptor>(desc);
			fixed (SubMeshDescriptor* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Mesh.SetAllSubMeshesAtOnceFromArray_Injected(intPtr, ref managedSpanWrapper, start, count, flags);
			}
		}

		[FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private void SetAllSubMeshesAtOnceFromNativeArray(IntPtr desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetAllSubMeshesAtOnceFromNativeArray_Injected(intPtr, desc, start, count, flags);
		}

		[FreeFunction("MeshScripting::SetLodCount", HasExplicitThis = true, ThrowsException = true)]
		private void SetLodCount(int numLevels)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetLodCount_Injected(intPtr, numLevels);
		}

		[FreeFunction("MeshScripting::SetLodSelectionCurve", HasExplicitThis = true, ThrowsException = true)]
		private void SetLodSelectionCurve(Mesh.LodSelectionCurve lodSelectionCurve)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetLodSelectionCurve_Injected(intPtr, ref lodSelectionCurve);
		}

		[FreeFunction("MeshScripting::SetLods", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void SetLodsFromArray(MeshLodRange[] levelRanges, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<MeshLodRange> span = new Span<MeshLodRange>(levelRanges);
			fixed (MeshLodRange* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Mesh.SetLodsFromArray_Injected(intPtr, ref managedSpanWrapper, start, count, submesh, flags);
			}
		}

		[FreeFunction("MeshScripting::SetLodsFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
		private void SetLodsFromNativeArray(IntPtr lodLevels, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetLodsFromNativeArray_Injected(intPtr, lodLevels, count, submesh, flags);
		}

		[FreeFunction("MeshScripting::SetLod", HasExplicitThis = true, ThrowsException = true)]
		private void SetLodImpl(int subMeshIndex, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.SetLodImpl_Injected(intPtr, subMeshIndex, level, ref levelRange, flags);
		}

		[FreeFunction("MeshScripting::GetLods", HasExplicitThis = true, ThrowsException = true)]
		private MeshLodRange[] GetLodsAlloc(int subMeshIndex)
		{
			MeshLodRange[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Mesh.GetLodsAlloc_Injected(intPtr, subMeshIndex, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				MeshLodRange[] array;
				blittableArrayWrapper.Unmarshal<MeshLodRange>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "MeshScripting::GetLodsNonAlloc", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void GetLodsNonAlloc([Out] MeshLodRange[] levels, int subMeshIndex)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				if (levels != null)
				{
					fixed (MeshLodRange[] array = levels)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				Mesh.GetLodsNonAlloc_Injected(intPtr, out blittableArrayWrapper, subMeshIndex);
			}
			finally
			{
				MeshLodRange[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<MeshLodRange>(ref array);
			}
		}

		[FreeFunction("MeshScripting::GetLodCount", HasExplicitThis = true)]
		private int GetLodCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetLodCount_Injected(intPtr);
		}

		[FreeFunction("MeshScripting::GetLodSelectionCurve", HasExplicitThis = true)]
		private Mesh.LodSelectionCurve GetLodSelectionCurve()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.LodSelectionCurve lodSelectionCurve;
			Mesh.GetLodSelectionCurve_Injected(intPtr, out lodSelectionCurve);
			return lodSelectionCurve;
		}

		[FreeFunction("MeshScripting::GetLod", HasExplicitThis = true, ThrowsException = true)]
		public MeshLodRange GetLod(int subMeshIndex, int levelIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			MeshLodRange meshLodRange;
			Mesh.GetLod_Injected(intPtr, subMeshIndex, levelIndex, out meshLodRange);
			return meshLodRange;
		}

		public Bounds bounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				Mesh.get_bounds_Injected(intPtr, out bounds);
				return bounds;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Mesh.set_bounds_Injected(intPtr, ref value);
			}
		}

		[NativeMethod("Clear")]
		private void ClearImpl(bool keepVertexLayout)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.ClearImpl_Injected(intPtr, keepVertexLayout);
		}

		[NativeMethod("RecalculateBounds")]
		private void RecalculateBoundsImpl(MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.RecalculateBoundsImpl_Injected(intPtr, flags);
		}

		[NativeMethod("RecalculateNormals")]
		private void RecalculateNormalsImpl(MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.RecalculateNormalsImpl_Injected(intPtr, flags);
		}

		[NativeMethod("RecalculateTangents")]
		private void RecalculateTangentsImpl(MeshUpdateFlags flags)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.RecalculateTangentsImpl_Injected(intPtr, flags);
		}

		[NativeMethod("MarkDynamic")]
		private void MarkDynamicImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.MarkDynamicImpl_Injected(intPtr);
		}

		[NativeMethod("MarkModified")]
		public void MarkModified()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.MarkModified_Injected(intPtr);
		}

		[NativeMethod("UploadMeshData")]
		private void UploadMeshDataImpl(bool markNoLongerReadable)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.UploadMeshDataImpl_Injected(intPtr, markNoLongerReadable);
		}

		[FreeFunction(Name = "MeshScripting::GetPrimitiveType", HasExplicitThis = true)]
		private MeshTopology GetTopologyImpl(int submesh)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetTopologyImpl_Injected(intPtr, submesh);
		}

		[NativeMethod("RecalculateMeshMetric")]
		private void RecalculateUVDistributionMetricImpl(int uvSetIndex, float uvAreaThreshold)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.RecalculateUVDistributionMetricImpl_Injected(intPtr, uvSetIndex, uvAreaThreshold);
		}

		[NativeMethod("RecalculateMeshMetrics")]
		private void RecalculateUVDistributionMetricsImpl(float uvAreaThreshold)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.RecalculateUVDistributionMetricsImpl_Injected(intPtr, uvAreaThreshold);
		}

		[NativeMethod("GetMeshMetric")]
		public float GetUVDistributionMetric(int uvSetIndex)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Mesh.GetUVDistributionMetric_Injected(intPtr, uvSetIndex);
		}

		[NativeMethod(Name = "MeshScripting::CombineMeshes", IsFreeFunction = true, ThrowsException = true, HasExplicitThis = true)]
		private unsafe void CombineMeshesImpl(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<CombineInstance> span = new Span<CombineInstance>(combine);
			fixed (CombineInstance* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Mesh.CombineMeshesImpl_Injected(intPtr, ref managedSpanWrapper, mergeSubMeshes, useMatrices, hasLightmapData);
			}
		}

		[NativeMethod("Optimize")]
		private void OptimizeImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.OptimizeImpl_Injected(intPtr);
		}

		[NativeMethod("OptimizeIndexBuffers")]
		private void OptimizeIndexBuffersImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.OptimizeIndexBuffersImpl_Injected(intPtr);
		}

		[NativeMethod("OptimizeReorderVertexBuffer")]
		private void OptimizeReorderVertexBufferImpl()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Mesh.OptimizeReorderVertexBufferImpl_Injected(intPtr);
		}

		internal static VertexAttribute GetUVChannel(int uvIndex)
		{
			bool flag = uvIndex < 0 || uvIndex > 7;
			if (flag)
			{
				throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
			}
			return VertexAttribute.TexCoord0 + uvIndex;
		}

		internal static int DefaultDimensionForChannel(VertexAttribute channel)
		{
			bool flag = channel == VertexAttribute.Position || channel == VertexAttribute.Normal;
			int num;
			if (flag)
			{
				num = 3;
			}
			else
			{
				bool flag2 = channel >= VertexAttribute.TexCoord0 && channel <= VertexAttribute.TexCoord7;
				if (flag2)
				{
					num = 2;
				}
				else
				{
					bool flag3 = channel == VertexAttribute.Tangent || channel == VertexAttribute.Color;
					if (!flag3)
					{
						throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
					}
					num = 4;
				}
			}
			return num;
		}

		private T[] GetAllocArrayFromChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				bool flag = this.HasVertexAttribute(channel);
				if (flag)
				{
					return (T[])this.GetAllocArrayFromChannelImpl(channel, format, dim);
				}
			}
			else
			{
				this.PrintErrorCantAccessChannel(channel);
			}
			return new T[0];
		}

		private T[] GetAllocArrayFromChannel<T>(VertexAttribute channel)
		{
			return this.GetAllocArrayFromChannel<T>(channel, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(channel));
		}

		private void SetSizedArrayForChannel(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int valuesArrayLength, int valuesStart, int valuesCount, MeshUpdateFlags flags)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				bool flag = valuesStart < 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start index can't be negative.");
				}
				bool flag2 = valuesCount < 0;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("valuesCount", valuesCount, "Mesh data array length can't be negative.");
				}
				bool flag3 = valuesStart >= valuesArrayLength && valuesCount != 0;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start is outside of array size.");
				}
				bool flag4 = valuesStart + valuesCount > valuesArrayLength;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("valuesCount", valuesStart + valuesCount, "Mesh data array start+count is outside of array size.");
				}
				bool flag5 = values == null;
				if (flag5)
				{
					valuesStart = 0;
				}
				this.SetArrayForChannelImpl(channel, format, dim, values, valuesArrayLength, valuesStart, valuesCount, flags);
			}
			else
			{
				this.PrintErrorCantAccessChannel(channel);
			}
		}

		private void SetSizedNativeArrayForChannel(VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int valuesArrayLength, int valuesStart, int valuesCount, MeshUpdateFlags flags)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				bool flag = valuesStart < 0;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start index can't be negative.");
				}
				bool flag2 = valuesCount < 0;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException("valuesCount", valuesCount, "Mesh data array length can't be negative.");
				}
				bool flag3 = valuesStart >= valuesArrayLength && valuesCount != 0;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start is outside of array size.");
				}
				bool flag4 = valuesStart + valuesCount > valuesArrayLength;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("valuesCount", valuesStart + valuesCount, "Mesh data array start+count is outside of array size.");
				}
				this.SetNativeArrayForChannelImpl(channel, format, dim, values, valuesArrayLength, valuesStart, valuesCount, flags);
			}
			else
			{
				this.PrintErrorCantAccessChannel(channel);
			}
		}

		private void SetArrayForChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim, T[] values, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			int num = NoAllocHelpers.SafeLength(values);
			this.SetSizedArrayForChannel(channel, format, dim, values, num, 0, num, flags);
		}

		private void SetArrayForChannel<T>(VertexAttribute channel, T[] values, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			int num = NoAllocHelpers.SafeLength(values);
			this.SetSizedArrayForChannel(channel, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(channel), values, num, 0, num, flags);
		}

		private void SetListForChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim, List<T> values, int start, int length, MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(channel, format, dim, NoAllocHelpers.ExtractArrayFromList<T>(values), NoAllocHelpers.SafeLength<T>(values), start, length, flags);
		}

		private void SetListForChannel<T>(VertexAttribute channel, List<T> values, int start, int length, MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(channel, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(channel), NoAllocHelpers.ExtractArrayFromList<T>(values), NoAllocHelpers.SafeLength<T>(values), start, length, flags);
		}

		private void GetListForChannel<T>(List<T> buffer, int capacity, VertexAttribute channel, int dim)
		{
			this.GetListForChannel<T>(buffer, capacity, channel, dim, VertexAttributeFormat.Float32);
		}

		private void GetListForChannel<T>(List<T> buffer, int capacity, VertexAttribute channel, int dim, VertexAttributeFormat channelType)
		{
			buffer.Clear();
			bool flag = !this.canAccess;
			if (flag)
			{
				this.PrintErrorCantAccessChannel(channel);
			}
			else
			{
				bool flag2 = !this.HasVertexAttribute(channel);
				if (!flag2)
				{
					NoAllocHelpers.EnsureListElemCount<T>(buffer, capacity);
					this.GetArrayFromChannelImpl(channel, channelType, dim, NoAllocHelpers.ExtractArrayFromList<T>(buffer));
				}
			}
		}

		public Vector3[] vertices
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(VertexAttribute.Position);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(VertexAttribute.Position, value, MeshUpdateFlags.Default);
			}
		}

		public Vector3[] normals
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(VertexAttribute.Normal);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(VertexAttribute.Normal, value, MeshUpdateFlags.Default);
			}
		}

		public Vector4[] tangents
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector4>(VertexAttribute.Tangent);
			}
			set
			{
				this.SetArrayForChannel<Vector4>(VertexAttribute.Tangent, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord0);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord0, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv2
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord1);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord1, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv3
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord2);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord2, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv4
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord3);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord3, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv5
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord4);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord4, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv6
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord5);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord5, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv7
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord6);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord6, value, MeshUpdateFlags.Default);
			}
		}

		public Vector2[] uv8
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord7);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(VertexAttribute.TexCoord7, value, MeshUpdateFlags.Default);
			}
		}

		public Color[] colors
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color>(VertexAttribute.Color);
			}
			set
			{
				this.SetArrayForChannel<Color>(VertexAttribute.Color, value, MeshUpdateFlags.Default);
			}
		}

		public Color32[] colors32
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color32>(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4);
			}
			set
			{
				this.SetArrayForChannel<Color32>(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, value, MeshUpdateFlags.Default);
			}
		}

		public int lodCount
		{
			get
			{
				return this.GetLodCount();
			}
			set
			{
				bool flag = value < 1;
				if (flag)
				{
					throw new ArgumentException("The number of Mesh LODs must be greater than zero.");
				}
				bool flag2 = value > 1;
				if (flag2)
				{
					for (int i = 0; i < this.subMeshCount; i++)
					{
						bool flag3 = this.GetSubMesh(i).topology > MeshTopology.Triangles;
						if (flag3)
						{
							throw new InvalidOperationException("Mesh LOD selection only works for triangle topology. The LOD count value cannot be higher than 1 if the topology is not set to triangles for all submeshes.");
						}
					}
				}
				this.SetLodCount(value);
			}
		}

		internal bool isLodSelectionActive
		{
			get
			{
				return this.lodCount > 1;
			}
		}

		public Mesh.LodSelectionCurve lodSelectionCurve
		{
			get
			{
				return this.GetLodSelectionCurve();
			}
			set
			{
				this.SetLodSelectionCurve(value);
			}
		}

		public void GetVertices(List<Vector3> vertices)
		{
			bool flag = vertices == null;
			if (flag)
			{
				throw new ArgumentNullException("vertices", "The result vertices list cannot be null.");
			}
			this.GetListForChannel<Vector3>(vertices, this.vertexCount, VertexAttribute.Position, Mesh.DefaultDimensionForChannel(VertexAttribute.Position));
		}

		public void SetVertices(List<Vector3> inVertices)
		{
			this.SetVertices(inVertices, 0, NoAllocHelpers.SafeLength<Vector3>(inVertices));
		}

		[ExcludeFromDocs]
		public void SetVertices(List<Vector3> inVertices, int start, int length)
		{
			this.SetVertices(inVertices, start, length, MeshUpdateFlags.Default);
		}

		public void SetVertices(List<Vector3> inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetListForChannel<Vector3>(VertexAttribute.Position, inVertices, start, length, flags);
		}

		public void SetVertices(Vector3[] inVertices)
		{
			this.SetVertices(inVertices, 0, NoAllocHelpers.SafeLength(inVertices));
		}

		[ExcludeFromDocs]
		public void SetVertices(Vector3[] inVertices, int start, int length)
		{
			this.SetVertices(inVertices, start, length, MeshUpdateFlags.Default);
		}

		public void SetVertices(Vector3[] inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(VertexAttribute.Position, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(VertexAttribute.Position), inVertices, NoAllocHelpers.SafeLength(inVertices), start, length, flags);
		}

		public void SetVertices<T>(NativeArray<T> inVertices) where T : struct
		{
			this.SetVertices<T>(inVertices, 0, inVertices.Length);
		}

		[ExcludeFromDocs]
		public void SetVertices<T>(NativeArray<T> inVertices, int start, int length) where T : struct
		{
			this.SetVertices<T>(inVertices, start, length, MeshUpdateFlags.Default);
		}

		public void SetVertices<T>(NativeArray<T> inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
		{
			bool flag = UnsafeUtility.SizeOf<T>() != 12;
			if (flag)
			{
				throw new ArgumentException("SetVertices with NativeArray should use struct type that is 12 bytes (3x float) in size");
			}
			this.SetSizedNativeArrayForChannel(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, (IntPtr)inVertices.GetUnsafeReadOnlyPtr<T>(), inVertices.Length, start, length, flags);
		}

		public void GetNormals(List<Vector3> normals)
		{
			bool flag = normals == null;
			if (flag)
			{
				throw new ArgumentNullException("normals", "The result normals list cannot be null.");
			}
			this.GetListForChannel<Vector3>(normals, this.vertexCount, VertexAttribute.Normal, Mesh.DefaultDimensionForChannel(VertexAttribute.Normal));
		}

		public void SetNormals(List<Vector3> inNormals)
		{
			this.SetNormals(inNormals, 0, NoAllocHelpers.SafeLength<Vector3>(inNormals));
		}

		[ExcludeFromDocs]
		public void SetNormals(List<Vector3> inNormals, int start, int length)
		{
			this.SetNormals(inNormals, start, length, MeshUpdateFlags.Default);
		}

		public void SetNormals(List<Vector3> inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetListForChannel<Vector3>(VertexAttribute.Normal, inNormals, start, length, flags);
		}

		public void SetNormals(Vector3[] inNormals)
		{
			this.SetNormals(inNormals, 0, NoAllocHelpers.SafeLength(inNormals));
		}

		[ExcludeFromDocs]
		public void SetNormals(Vector3[] inNormals, int start, int length)
		{
			this.SetNormals(inNormals, start, length, MeshUpdateFlags.Default);
		}

		public void SetNormals(Vector3[] inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(VertexAttribute.Normal, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(VertexAttribute.Normal), inNormals, NoAllocHelpers.SafeLength(inNormals), start, length, flags);
		}

		public void SetNormals<T>(NativeArray<T> inNormals) where T : struct
		{
			this.SetNormals<T>(inNormals, 0, inNormals.Length);
		}

		[ExcludeFromDocs]
		public void SetNormals<T>(NativeArray<T> inNormals, int start, int length) where T : struct
		{
			this.SetNormals<T>(inNormals, start, length, MeshUpdateFlags.Default);
		}

		public void SetNormals<T>(NativeArray<T> inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
		{
			bool flag = UnsafeUtility.SizeOf<T>() != 12;
			if (flag)
			{
				throw new ArgumentException("SetNormals with NativeArray should use struct type that is 12 bytes (3x float) in size");
			}
			this.SetSizedNativeArrayForChannel(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, (IntPtr)inNormals.GetUnsafeReadOnlyPtr<T>(), inNormals.Length, start, length, flags);
		}

		public void GetTangents(List<Vector4> tangents)
		{
			bool flag = tangents == null;
			if (flag)
			{
				throw new ArgumentNullException("tangents", "The result tangents list cannot be null.");
			}
			this.GetListForChannel<Vector4>(tangents, this.vertexCount, VertexAttribute.Tangent, Mesh.DefaultDimensionForChannel(VertexAttribute.Tangent));
		}

		public void SetTangents(List<Vector4> inTangents)
		{
			this.SetTangents(inTangents, 0, NoAllocHelpers.SafeLength<Vector4>(inTangents));
		}

		[ExcludeFromDocs]
		public void SetTangents(List<Vector4> inTangents, int start, int length)
		{
			this.SetTangents(inTangents, start, length, MeshUpdateFlags.Default);
		}

		public void SetTangents(List<Vector4> inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetListForChannel<Vector4>(VertexAttribute.Tangent, inTangents, start, length, flags);
		}

		public void SetTangents(Vector4[] inTangents)
		{
			this.SetTangents(inTangents, 0, NoAllocHelpers.SafeLength(inTangents));
		}

		[ExcludeFromDocs]
		public void SetTangents(Vector4[] inTangents, int start, int length)
		{
			this.SetTangents(inTangents, start, length, MeshUpdateFlags.Default);
		}

		public void SetTangents(Vector4[] inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(VertexAttribute.Tangent, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(VertexAttribute.Tangent), inTangents, NoAllocHelpers.SafeLength(inTangents), start, length, flags);
		}

		public void SetTangents<T>(NativeArray<T> inTangents) where T : struct
		{
			this.SetTangents<T>(inTangents, 0, inTangents.Length);
		}

		[ExcludeFromDocs]
		public void SetTangents<T>(NativeArray<T> inTangents, int start, int length) where T : struct
		{
			this.SetTangents<T>(inTangents, start, length, MeshUpdateFlags.Default);
		}

		public void SetTangents<T>(NativeArray<T> inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
		{
			bool flag = UnsafeUtility.SizeOf<T>() != 16;
			if (flag)
			{
				throw new ArgumentException("SetTangents with NativeArray should use struct type that is 16 bytes (4x float) in size");
			}
			this.SetSizedNativeArrayForChannel(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, (IntPtr)inTangents.GetUnsafeReadOnlyPtr<T>(), inTangents.Length, start, length, flags);
		}

		public void GetColors(List<Color> colors)
		{
			bool flag = colors == null;
			if (flag)
			{
				throw new ArgumentNullException("colors", "The result colors list cannot be null.");
			}
			this.GetListForChannel<Color>(colors, this.vertexCount, VertexAttribute.Color, Mesh.DefaultDimensionForChannel(VertexAttribute.Color));
		}

		public void SetColors(List<Color> inColors)
		{
			this.SetColors(inColors, 0, NoAllocHelpers.SafeLength<Color>(inColors));
		}

		[ExcludeFromDocs]
		public void SetColors(List<Color> inColors, int start, int length)
		{
			this.SetColors(inColors, start, length, MeshUpdateFlags.Default);
		}

		public void SetColors(List<Color> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetListForChannel<Color>(VertexAttribute.Color, inColors, start, length, flags);
		}

		public void SetColors(Color[] inColors)
		{
			this.SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
		}

		[ExcludeFromDocs]
		public void SetColors(Color[] inColors, int start, int length)
		{
			this.SetColors(inColors, start, length, MeshUpdateFlags.Default);
		}

		public void SetColors(Color[] inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(VertexAttribute.Color, VertexAttributeFormat.Float32, Mesh.DefaultDimensionForChannel(VertexAttribute.Color), inColors, NoAllocHelpers.SafeLength(inColors), start, length, flags);
		}

		public void GetColors(List<Color32> colors)
		{
			bool flag = colors == null;
			if (flag)
			{
				throw new ArgumentNullException("colors", "The result colors list cannot be null.");
			}
			this.GetListForChannel<Color32>(colors, this.vertexCount, VertexAttribute.Color, 4, VertexAttributeFormat.UNorm8);
		}

		public void SetColors(List<Color32> inColors)
		{
			this.SetColors(inColors, 0, NoAllocHelpers.SafeLength<Color32>(inColors));
		}

		[ExcludeFromDocs]
		public void SetColors(List<Color32> inColors, int start, int length)
		{
			this.SetColors(inColors, start, length, MeshUpdateFlags.Default);
		}

		public void SetColors(List<Color32> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetListForChannel<Color32>(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, inColors, start, length, flags);
		}

		public void SetColors(Color32[] inColors)
		{
			this.SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
		}

		[ExcludeFromDocs]
		public void SetColors(Color32[] inColors, int start, int length)
		{
			this.SetColors(inColors, start, length, MeshUpdateFlags.Default);
		}

		public void SetColors(Color32[] inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetSizedArrayForChannel(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, inColors, NoAllocHelpers.SafeLength(inColors), start, length, flags);
		}

		public void SetColors<T>(NativeArray<T> inColors) where T : struct
		{
			this.SetColors<T>(inColors, 0, inColors.Length);
		}

		[ExcludeFromDocs]
		public void SetColors<T>(NativeArray<T> inColors, int start, int length) where T : struct
		{
			this.SetColors<T>(inColors, start, length, MeshUpdateFlags.Default);
		}

		public void SetColors<T>(NativeArray<T> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			bool flag = num != 16 && num != 4;
			if (flag)
			{
				throw new ArgumentException("SetColors with NativeArray should use struct type that is 16 bytes (4x float) or 4 bytes (4x unorm) in size");
			}
			this.SetSizedNativeArrayForChannel(VertexAttribute.Color, (num == 4) ? VertexAttributeFormat.UNorm8 : VertexAttributeFormat.Float32, 4, (IntPtr)inColors.GetUnsafeReadOnlyPtr<T>(), inColors.Length, start, length, flags);
		}

		private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs, int start, int length, MeshUpdateFlags flags)
		{
			bool flag = uvIndex < 0 || uvIndex > 7;
			if (flag)
			{
				Debug.LogError("The uv index is invalid. Must be in the range 0 to 7.");
			}
			else
			{
				this.SetListForChannel<T>(Mesh.GetUVChannel(uvIndex), VertexAttributeFormat.Float32, dim, uvs, start, length, flags);
			}
		}

		public void SetUVs(int channel, List<Vector2> uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength<Vector2>(uvs));
		}

		public void SetUVs(int channel, List<Vector3> uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength<Vector3>(uvs));
		}

		public void SetUVs(int channel, List<Vector4> uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength<Vector4>(uvs));
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, List<Vector2> uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, List<Vector2> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl<Vector2>(channel, 2, uvs, start, length, flags);
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, List<Vector3> uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, List<Vector3> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl<Vector3>(channel, 3, uvs, start, length, flags);
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, List<Vector4> uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, List<Vector4> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl<Vector4>(channel, 4, uvs, start, length, flags);
		}

		private void SetUvsImpl(int uvIndex, int dim, Array uvs, int arrayStart, int arraySize, MeshUpdateFlags flags)
		{
			bool flag = uvIndex < 0 || uvIndex > 7;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("uvIndex", uvIndex, "The uv index is invalid. Must be in the range 0 to 7.");
			}
			this.SetSizedArrayForChannel(Mesh.GetUVChannel(uvIndex), VertexAttributeFormat.Float32, dim, uvs, NoAllocHelpers.SafeLength(uvs), arrayStart, arraySize, flags);
		}

		public void SetUVs(int channel, Vector2[] uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
		}

		public void SetUVs(int channel, Vector3[] uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
		}

		public void SetUVs(int channel, Vector4[] uvs)
		{
			this.SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, Vector2[] uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, Vector2[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl(channel, 2, uvs, start, length, flags);
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, Vector3[] uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, Vector3[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl(channel, 3, uvs, start, length, flags);
		}

		[ExcludeFromDocs]
		public void SetUVs(int channel, Vector4[] uvs, int start, int length)
		{
			this.SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs(int channel, Vector4[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			this.SetUvsImpl(channel, 4, uvs, start, length, flags);
		}

		public void SetUVs<T>(int channel, NativeArray<T> uvs) where T : struct
		{
			this.SetUVs<T>(channel, uvs, 0, uvs.Length);
		}

		[ExcludeFromDocs]
		public void SetUVs<T>(int channel, NativeArray<T> uvs, int start, int length) where T : struct
		{
			this.SetUVs<T>(channel, uvs, start, length, MeshUpdateFlags.Default);
		}

		public void SetUVs<T>(int channel, NativeArray<T> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
		{
			bool flag = channel < 0 || channel > 7;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
			}
			int num = UnsafeUtility.SizeOf<T>();
			bool flag2 = (num & 3) != 0;
			if (flag2)
			{
				throw new ArgumentException("SetUVs with NativeArray should use struct type that is multiple of 4 bytes in size");
			}
			int num2 = num / 4;
			bool flag3 = num2 < 1 || num2 > 4;
			if (flag3)
			{
				throw new ArgumentException("SetUVs with NativeArray should use struct type that is 1..4 floats in size");
			}
			this.SetSizedNativeArrayForChannel(Mesh.GetUVChannel(channel), VertexAttributeFormat.Float32, num2, (IntPtr)uvs.GetUnsafeReadOnlyPtr<T>(), uvs.Length, start, length, flags);
		}

		private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
		{
			bool flag = uvs == null;
			if (flag)
			{
				throw new ArgumentNullException("uvs", "The result uvs list cannot be null.");
			}
			bool flag2 = uvIndex < 0 || uvIndex > 7;
			if (flag2)
			{
				throw new IndexOutOfRangeException("The uv index is invalid. Must be in the range 0 to 7.");
			}
			this.GetListForChannel<T>(uvs, this.vertexCount, Mesh.GetUVChannel(uvIndex), dim);
		}

		public void GetUVs(int channel, List<Vector2> uvs)
		{
			this.GetUVsImpl<Vector2>(channel, uvs, 2);
		}

		public void GetUVs(int channel, List<Vector3> uvs)
		{
			this.GetUVsImpl<Vector3>(channel, uvs, 3);
		}

		public void GetUVs(int channel, List<Vector4> uvs)
		{
			this.GetUVsImpl<Vector4>(channel, uvs, 4);
		}

		public int vertexAttributeCount
		{
			get
			{
				return this.GetVertexAttributeCountImpl();
			}
		}

		public VertexAttributeDescriptor[] GetVertexAttributes()
		{
			return (VertexAttributeDescriptor[])this.GetVertexAttributesAlloc();
		}

		public int GetVertexAttributes(VertexAttributeDescriptor[] attributes)
		{
			return this.GetVertexAttributesArray(attributes);
		}

		public int GetVertexAttributes(List<VertexAttributeDescriptor> attributes)
		{
			return this.GetVertexAttributesList(attributes);
		}

		public void SetVertexBufferParams(int vertexCount, params VertexAttributeDescriptor[] attributes)
		{
			this.SetVertexBufferParamsFromArray(vertexCount, attributes);
		}

		public void SetVertexBufferParams(int vertexCount, NativeArray<VertexAttributeDescriptor> attributes)
		{
			this.SetVertexBufferParamsFromPtr(vertexCount, (IntPtr)attributes.GetUnsafeReadOnlyPtr<VertexAttributeDescriptor>(), attributes.Length);
		}

		public void SetVertexBufferData<T>(NativeArray<T> data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
			}
			bool flag2 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
			}
			this.InternalSetVertexBufferData(stream, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
		}

		public void SetVertexBufferData<T>(T[] data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException("Array passed to SetVertexBufferData must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
			}
			bool flag3 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
			}
			this.InternalSetVertexBufferDataFromArray(stream, data, dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
		}

		public void SetVertexBufferData<T>(List<T> data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "SetVertexBufferData", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag3 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Count;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
			}
			this.InternalSetVertexBufferDataFromArray(stream, NoAllocHelpers.ExtractArrayFromList<T>(data), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
		}

		public static Mesh.MeshDataArray AcquireReadOnlyMeshData(Mesh mesh)
		{
			return new Mesh.MeshDataArray(mesh, true, false);
		}

		public static Mesh.MeshDataArray AcquireReadOnlyMeshData(Mesh[] meshes)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh array is null");
			}
			return new Mesh.MeshDataArray(meshes, meshes.Length, true, false);
		}

		public static Mesh.MeshDataArray AcquireReadOnlyMeshData(List<Mesh> meshes)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh list is null");
			}
			return new Mesh.MeshDataArray(NoAllocHelpers.ExtractArrayFromList<Mesh>(meshes), meshes.Count, true, false);
		}

		public static Mesh.MeshDataArray AllocateWritableMeshData(int meshCount)
		{
			return new Mesh.MeshDataArray(meshCount);
		}

		public static Mesh.MeshDataArray AllocateWritableMeshData(Mesh mesh)
		{
			return new Mesh.MeshDataArray(mesh, true, true);
		}

		public static Mesh.MeshDataArray AllocateWritableMeshData(Mesh[] meshes)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh array is null");
			}
			return new Mesh.MeshDataArray(meshes, meshes.Length, true, true);
		}

		public static Mesh.MeshDataArray AllocateWritableMeshData(List<Mesh> meshes)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh list is null");
			}
			return new Mesh.MeshDataArray(NoAllocHelpers.ExtractArrayFromList<Mesh>(meshes), meshes.Count, true, true);
		}

		public static void ApplyAndDisposeWritableMeshData(Mesh.MeshDataArray data, Mesh mesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			bool flag = mesh == null;
			if (flag)
			{
				throw new ArgumentNullException("mesh", "Mesh is null");
			}
			bool flag2 = data.Length != 1;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("{0} length must be 1 to apply to one mesh, was {1}", "MeshDataArray", data.Length));
			}
			data.ApplyToMeshAndDispose(mesh, flags);
		}

		public static void ApplyAndDisposeWritableMeshData(Mesh.MeshDataArray data, Mesh[] meshes, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh array is null");
			}
			bool flag2 = data.Length != meshes.Length;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("{0} length ({1}) must match destination meshes array length ({2})", "MeshDataArray", data.Length, meshes.Length));
			}
			data.ApplyToMeshesAndDispose(meshes, flags);
		}

		public static void ApplyAndDisposeWritableMeshData(Mesh.MeshDataArray data, List<Mesh> meshes, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			bool flag = meshes == null;
			if (flag)
			{
				throw new ArgumentNullException("meshes", "Mesh list is null");
			}
			bool flag2 = data.Length != meshes.Count;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("{0} length ({1}) must match destination meshes list length ({2})", "MeshDataArray", data.Length, meshes.Count));
			}
			data.ApplyToMeshesAndDispose(NoAllocHelpers.ExtractArrayFromList<Mesh>(meshes), flags);
		}

		public GraphicsBuffer GetVertexBuffer(int index)
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			return this.GetVertexBufferImpl(index);
		}

		public GraphicsBuffer GetIndexBuffer()
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			return this.GetIndexBufferImpl();
		}

		public GraphicsBuffer GetBoneWeightBuffer(SkinWeights layout)
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = layout == SkinWeights.None;
			GraphicsBuffer graphicsBuffer;
			if (flag2)
			{
				Debug.LogError(string.Format("Only possible to access bone weights buffer for values: {0}, {1}, {2} and {3}.", new object[]
				{
					SkinWeights.OneBone,
					SkinWeights.TwoBones,
					SkinWeights.FourBones,
					SkinWeights.Unlimited
				}));
				graphicsBuffer = null;
			}
			else
			{
				GraphicsBuffer boneWeightBufferImpl = this.GetBoneWeightBufferImpl((int)layout);
				graphicsBuffer = boneWeightBufferImpl;
			}
			return graphicsBuffer;
		}

		public GraphicsBuffer GetBlendShapeBuffer(BlendShapeBufferLayout layout)
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = !SystemInfo.supportsComputeShaders;
			GraphicsBuffer graphicsBuffer;
			if (flag2)
			{
				Debug.LogError("Only possible to access Blend Shape buffer on platforms that supports compute shaders.");
				graphicsBuffer = null;
			}
			else
			{
				GraphicsBuffer blendShapeBufferImpl = this.GetBlendShapeBufferImpl((int)layout);
				graphicsBuffer = blendShapeBufferImpl;
			}
			return graphicsBuffer;
		}

		public GraphicsBuffer GetBlendShapeBuffer()
		{
			bool flag = this == null;
			if (flag)
			{
				throw new NullReferenceException();
			}
			bool flag2 = !SystemInfo.supportsComputeShaders;
			GraphicsBuffer graphicsBuffer;
			if (flag2)
			{
				Debug.LogError("Only possible to access Blend Shape buffer on platforms that supports compute shaders.");
				graphicsBuffer = null;
			}
			else
			{
				GraphicsBuffer blendShapeBufferImpl = this.GetBlendShapeBufferImpl(0);
				graphicsBuffer = blendShapeBufferImpl;
			}
			return graphicsBuffer;
		}

		public BlendShapeBufferRange GetBlendShapeBufferRange(int blendShapeIndex)
		{
			bool flag = blendShapeIndex >= this.blendShapeCount || blendShapeIndex < 0;
			BlendShapeBufferRange blendShapeBufferRange;
			if (flag)
			{
				Debug.LogError("Incorrect index used to get blend shape buffer range");
				blendShapeBufferRange = default(BlendShapeBufferRange);
			}
			else
			{
				BlendShape blendShapeOffsetInternal = this.GetBlendShapeOffsetInternal(blendShapeIndex);
				blendShapeBufferRange = new BlendShapeBufferRange
				{
					startIndex = blendShapeOffsetInternal.firstVertex,
					endIndex = blendShapeOffsetInternal.firstVertex + blendShapeOffsetInternal.vertexCount - 1U
				};
			}
			return blendShapeBufferRange;
		}

		private void PrintErrorCantAccessIndices()
		{
			Debug.LogError(string.Format("Not allowed to access triangles/indices on mesh '{0}' (isReadable is false; Read/Write must be enabled in import settings)", base.name));
		}

		private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
		{
			bool flag = !this.canAccess;
			bool flag2;
			if (flag)
			{
				this.PrintErrorCantAccessIndices();
				flag2 = false;
			}
			else
			{
				bool flag3 = submesh < 0 || submesh >= this.subMeshCount;
				if (flag3)
				{
					Debug.LogError(string.Format("Failed getting {0}. Submesh index is out of bounds.", errorAboutTriangles ? "triangles" : "indices"), this);
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
			}
			return flag2;
		}

		private bool CheckCanAccessSubmeshTriangles(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, true);
		}

		private bool CheckCanAccessSubmeshIndices(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, false);
		}

		public int[] triangles
		{
			get
			{
				bool canAccess = this.canAccess;
				int[] array;
				if (canAccess)
				{
					array = this.GetTrianglesImpl(-1, true, 0);
				}
				else
				{
					this.PrintErrorCantAccessIndices();
					array = new int[0];
				}
				return array;
			}
			set
			{
				bool canAccess = this.canAccess;
				if (canAccess)
				{
					this.SetTrianglesImpl(-1, IndexFormat.UInt32, value, NoAllocHelpers.SafeLength(value), 0, NoAllocHelpers.SafeLength(value), true, 0, 0);
				}
				else
				{
					this.PrintErrorCantAccessIndices();
				}
			}
		}

		public int[] GetTriangles(int submesh)
		{
			return this.GetTriangles(submesh, true);
		}

		public int[] GetTriangles(int submesh, [DefaultValue("true")] bool applyBaseVertex)
		{
			return this.GetTriangles(submesh, 0, applyBaseVertex);
		}

		public int[] GetTriangles(int submesh, int meshLod, bool applyBaseVertex)
		{
			bool flag = !this.CheckCanAccessSubmeshTriangles(submesh);
			int[] array;
			if (flag)
			{
				array = new int[0];
			}
			else
			{
				bool flag2 = meshLod >= this.lodCount;
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("The Mesh LOD index ({0}) must be less than the lodCount value ({1}).", meshLod, this.lodCount));
				}
				array = this.GetTrianglesImpl(submesh, applyBaseVertex, meshLod);
			}
			return array;
		}

		public void GetTriangles(List<int> triangles, int submesh)
		{
			this.GetTriangles(triangles, submesh, 0, true);
		}

		public void GetTriangles(List<int> triangles, int submesh, [DefaultValue("true")] bool applyBaseVertex)
		{
			this.GetTriangles(triangles, submesh, 0, applyBaseVertex);
		}

		public void GetTriangles(List<int> triangles, int submesh, int meshLod, bool applyBaseVertex = true)
		{
			bool flag = triangles == null;
			if (flag)
			{
				throw new ArgumentNullException("triangles", "The result triangles list cannot be null.");
			}
			bool flag2 = submesh < 0 || submesh >= this.subMeshCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			bool flag3 = meshLod >= this.lodCount;
			if (flag3)
			{
				throw new IndexOutOfRangeException(string.Format("The Mesh LOD index ({0}) must be less than the lodCount value ({1}).", meshLod, this.lodCount));
			}
			NoAllocHelpers.EnsureListElemCount<int>(triangles, (int)(3U * this.GetTrianglesCountImpl(submesh, meshLod)));
			this.GetTrianglesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList<int>(triangles), submesh, applyBaseVertex, meshLod);
		}

		public void GetTriangles(List<ushort> triangles, int submesh, bool applyBaseVertex = true)
		{
			this.GetTriangles(triangles, submesh, 0, applyBaseVertex);
		}

		public void GetTriangles(List<ushort> triangles, int submesh, int meshLod, bool applyBaseVertex = true)
		{
			bool flag = triangles == null;
			if (flag)
			{
				throw new ArgumentNullException("triangles", "The result triangles list cannot be null.");
			}
			bool flag2 = submesh < 0 || submesh >= this.subMeshCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			bool flag3 = meshLod >= this.lodCount;
			if (flag3)
			{
				throw new IndexOutOfRangeException(string.Format("The Mesh LOD index ({0}) must be less than the lodCount value ({1}).", meshLod, this.lodCount));
			}
			NoAllocHelpers.EnsureListElemCount<ushort>(triangles, (int)(3U * this.GetTrianglesCountImpl(submesh, meshLod)));
			this.GetTrianglesNonAllocImpl16(NoAllocHelpers.ExtractArrayFromList<ushort>(triangles), submesh, applyBaseVertex, meshLod);
		}

		[ExcludeFromDocs]
		public int[] GetIndices(int submesh)
		{
			return this.GetIndices(submesh, 0, true);
		}

		public int[] GetIndices(int submesh, [DefaultValue("true")] bool applyBaseVertex)
		{
			return this.GetIndices(submesh, 0, applyBaseVertex);
		}

		public int[] GetIndices(int submesh, int meshLod, bool applyBaseVertex = true)
		{
			bool flag = !this.CheckCanAccessSubmeshIndices(submesh);
			int[] array;
			if (flag)
			{
				array = new int[0];
			}
			else
			{
				bool flag2 = meshLod >= this.lodCount;
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("The Mesh LOD index ({0}) must be less than the lodCount value ({1}).", meshLod, this.lodCount));
				}
				array = this.GetIndicesImpl(submesh, applyBaseVertex, meshLod);
			}
			return array;
		}

		[ExcludeFromDocs]
		public void GetIndices(List<int> indices, int submesh)
		{
			this.GetIndices(indices, submesh, 0, true);
		}

		public void GetIndices(List<int> indices, int submesh, [DefaultValue("true")] bool applyBaseVertex)
		{
			this.GetIndices(indices, submesh, 0, applyBaseVertex);
		}

		public void GetIndices(List<int> indices, int submesh, int meshLod, bool applyBaseVertex = false)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices", "The result indices list cannot be null.");
			}
			bool flag2 = submesh < 0 || submesh >= this.subMeshCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			bool flag3 = meshLod >= this.lodCount;
			if (flag3)
			{
				throw new IndexOutOfRangeException(string.Format("The Mesh LOD index ({0}) must be less than the lodCount value ({1}).", meshLod, this.lodCount));
			}
			NoAllocHelpers.EnsureListElemCount<int>(indices, (int)this.GetIndexCount(submesh, meshLod));
			this.GetIndicesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList<int>(indices), submesh, applyBaseVertex, meshLod);
		}

		public void GetIndices(List<ushort> indices, int submesh, bool applyBaseVertex = true)
		{
			this.GetIndices(indices, submesh, 0, applyBaseVertex);
		}

		public void GetIndices(List<ushort> indices, int submesh, int meshLod, bool applyBaseVertex = true)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices", "The result indices list cannot be null.");
			}
			bool flag2 = submesh < 0 || submesh >= this.subMeshCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			NoAllocHelpers.EnsureListElemCount<ushort>(indices, (int)this.GetIndexCount(submesh, meshLod));
			this.GetIndicesNonAllocImpl16(NoAllocHelpers.ExtractArrayFromList<ushort>(indices), submesh, applyBaseVertex, meshLod);
		}

		public void SetIndexBufferData<T>(NativeArray<T> data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				this.PrintErrorCantAccessIndices();
			}
			else
			{
				bool flag2 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length;
				if (flag2)
				{
					throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
				}
				this.InternalSetIndexBufferData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
			}
		}

		public void SetIndexBufferData<T>(T[] data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				this.PrintErrorCantAccessIndices();
			}
			else
			{
				bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
				if (flag2)
				{
					throw new ArgumentException("Array passed to SetIndexBufferData must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
				}
				bool flag3 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
				}
				this.InternalSetIndexBufferDataFromArray(data, dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
			}
		}

		public void SetIndexBufferData<T>(List<T> data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = !this.canAccess;
			if (flag)
			{
				this.PrintErrorCantAccessIndices();
			}
			else
			{
				bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
				if (flag2)
				{
					throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "SetIndexBufferData", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
				}
				bool flag3 = dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Count;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (dataStart:{0} meshBufferStart:{1} count:{2})", dataStart, meshBufferStart, count));
				}
				this.InternalSetIndexBufferDataFromArray(NoAllocHelpers.ExtractArrayFromList<T>(data), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
			}
		}

		public uint GetIndexStart(int submesh)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetIndexStartImpl(submesh, 0);
		}

		public uint GetIndexStart(int submesh, int meshLod)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			bool flag2 = meshLod >= this.lodCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be less than the lodCount value ({1}).", meshLod, this.lodCount));
			}
			return this.GetIndexStartImpl(submesh, meshLod);
		}

		public uint GetIndexCount(int submesh)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetIndexCountImpl(submesh, 0);
		}

		public uint GetIndexCount(int submesh, int meshLod)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			bool flag2 = meshLod >= this.lodCount;
			if (flag2)
			{
				throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be less than the lodCount value ({1}).", meshLod, this.lodCount));
			}
			return this.GetIndexCountImpl(submesh, meshLod);
		}

		public uint GetBaseVertex(int submesh)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetBaseVertexImpl(submesh);
		}

		private void CheckIndicesArrayRange(int valuesLength, int start, int length)
		{
			bool flag = start < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("start", start, "Mesh indices array start can't be negative.");
			}
			bool flag2 = length < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("length", length, "Mesh indices array length can't be negative.");
			}
			bool flag3 = start >= valuesLength && length != 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("start", start, "Mesh indices array start is outside of array size.");
			}
			bool flag4 = start + length > valuesLength;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("length", start + length, "Mesh indices array start+count is outside of array size.");
			}
		}

		private void SetTrianglesImpl(int submesh, IndexFormat indicesFormat, Array triangles, int trianglesArrayLength, int start, int length, bool calculateBounds, int baseVertex, int meshLod)
		{
			this.CheckIndicesArrayRange(trianglesArrayLength, start, length);
			this.SetIndicesImpl(submesh, MeshTopology.Triangles, indicesFormat, triangles, start, length, calculateBounds, baseVertex, meshLod);
		}

		[ExcludeFromDocs]
		public void SetTriangles(int[] triangles, int submesh)
		{
			this.SetTriangles(triangles, submesh, true, 0);
		}

		[ExcludeFromDocs]
		public void SetTriangles(int[] triangles, int submesh, bool calculateBounds)
		{
			this.SetTriangles(triangles, submesh, calculateBounds, 0);
		}

		public void SetTriangles(int[] triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
		}

		public void SetTriangles(int[] triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetTriangles(int[] triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetTriangles(int[] triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshTriangles(submesh);
			if (flag)
			{
				this.SetTrianglesImpl(submesh, IndexFormat.UInt32, triangles, NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetTriangles(ushort[] triangles, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
		}

		public void SetTriangles(ushort[] triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetTriangles(ushort[] triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetTriangles(ushort[] triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshTriangles(submesh);
			if (flag)
			{
				this.SetTrianglesImpl(submesh, IndexFormat.UInt16, triangles, NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		[ExcludeFromDocs]
		public void SetTriangles(List<int> triangles, int submesh)
		{
			this.SetTriangles(triangles, submesh, true, 0);
		}

		[ExcludeFromDocs]
		public void SetTriangles(List<int> triangles, int submesh, bool calculateBounds)
		{
			this.SetTriangles(triangles, submesh, calculateBounds, 0);
		}

		public void SetTriangles(List<int> triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength<int>(triangles), submesh, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<int> triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength<int>(triangles), submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<int> triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<int> triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshTriangles(submesh);
			if (flag)
			{
				this.SetTrianglesImpl(submesh, IndexFormat.UInt32, NoAllocHelpers.ExtractArrayFromList<int>(triangles), NoAllocHelpers.SafeLength<int>(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetTriangles(List<ushort> triangles, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength<ushort>(triangles), submesh, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<ushort> triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, 0, NoAllocHelpers.SafeLength<ushort>(triangles), submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<ushort> triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetTriangles(List<ushort> triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshTriangles(submesh);
			if (flag)
			{
				this.SetTrianglesImpl(submesh, IndexFormat.UInt16, NoAllocHelpers.ExtractArrayFromList<ushort>(triangles), NoAllocHelpers.SafeLength<ushort>(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		[ExcludeFromDocs]
		public void SetIndices(int[] indices, MeshTopology topology, int submesh)
		{
			this.SetIndices(indices, topology, submesh, true, 0);
		}

		[ExcludeFromDocs]
		public void SetIndices(int[] indices, MeshTopology topology, int submesh, bool calculateBounds)
		{
			this.SetIndices(indices, topology, submesh, calculateBounds, 0);
		}

		public void SetIndices(int[] indices, MeshTopology topology, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
		}

		public void SetIndices(int[] indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetIndices(int[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetIndices(int[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshIndices(submesh);
			if (flag)
			{
				this.CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
				this.SetIndicesImpl(submesh, topology, IndexFormat.UInt32, indices, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetIndices(ushort[] indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
		}

		public void SetIndices(ushort[] indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetIndices(ushort[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetIndices(ushort[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshIndices(submesh);
			if (flag)
			{
				this.CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
				this.SetIndicesImpl(submesh, topology, IndexFormat.UInt16, indices, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetIndices<T>(NativeArray<T> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0) where T : struct
		{
			this.SetIndices<T>(indices, 0, indices.Length, topology, submesh, calculateBounds, baseVertex);
		}

		public void SetIndices<T>(NativeArray<T> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0) where T : struct
		{
			this.SetIndices<T>(indices, 0, indices.Length, topology, submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetIndices<T>(NativeArray<T> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0) where T : struct
		{
			this.SetIndices<T>(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetIndices<T>(NativeArray<T> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0) where T : struct
		{
			bool flag = this.CheckCanAccessSubmeshIndices(submesh);
			if (flag)
			{
				int num = UnsafeUtility.SizeOf<T>();
				bool flag2 = num != 2 && num != 4;
				if (flag2)
				{
					throw new ArgumentException("SetIndices with NativeArray should use type is 2 or 4 bytes in size");
				}
				this.CheckIndicesArrayRange(indices.Length, indicesStart, indicesLength);
				this.SetIndicesNativeArrayImpl(submesh, topology, (num == 2) ? IndexFormat.UInt16 : IndexFormat.UInt32, (IntPtr)indices.GetUnsafeReadOnlyPtr<T>(), indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetIndices(List<int> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength<int>(indices), topology, submesh, calculateBounds, baseVertex);
		}

		public void SetIndices(List<int> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength<int>(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetIndices(List<int> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetIndices(List<int> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshIndices(submesh);
			if (flag)
			{
				int[] array = NoAllocHelpers.ExtractArrayFromList<int>(indices);
				this.CheckIndicesArrayRange(NoAllocHelpers.SafeLength<int>(indices), indicesStart, indicesLength);
				this.SetIndicesImpl(submesh, topology, IndexFormat.UInt32, array, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetIndices(List<ushort> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength<ushort>(indices), topology, submesh, calculateBounds, baseVertex);
		}

		public void SetIndices(List<ushort> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, 0, NoAllocHelpers.SafeLength<ushort>(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
		}

		public void SetIndices(List<ushort> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
		{
			this.SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
		}

		public void SetIndices(List<ushort> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
		{
			bool flag = this.CheckCanAccessSubmeshIndices(submesh);
			if (flag)
			{
				ushort[] array = NoAllocHelpers.ExtractArrayFromList<ushort>(indices);
				this.CheckIndicesArrayRange(NoAllocHelpers.SafeLength<ushort>(indices), indicesStart, indicesLength);
				this.SetIndicesImpl(submesh, topology, IndexFormat.UInt16, array, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
			}
		}

		public void SetSubMeshes(SubMeshDescriptor[] desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			bool flag = count > 0 && desc == null;
			if (flag)
			{
				throw new ArgumentNullException("desc", "Array of submeshes cannot be null unless count is zero.");
			}
			int num = ((desc != null) ? desc.Length : 0);
			bool flag2 = start < 0 || count < 0 || start + count > num;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1} desc.Length:{2})", start, count, num));
			}
			for (int i = start; i < start + count; i++)
			{
				MeshTopology topology = desc[i].topology;
				bool flag3 = topology < MeshTopology.Triangles || topology > MeshTopology.Points;
				if (flag3)
				{
					throw new ArgumentException("desc", string.Format("{0}-th submesh descriptor has invalid topology ({1}).", i, (int)topology));
				}
				bool flag4 = topology == (MeshTopology)1;
				if (flag4)
				{
					throw new ArgumentException("desc", string.Format("{0}-th submesh descriptor has triangles strip topology, which is no longer supported.", i));
				}
				bool flag5 = this.isLodSelectionActive && topology > MeshTopology.Triangles;
				if (flag5)
				{
					throw new ArgumentException("desc", string.Format("Submesh descriptor with index {0} has topology {1} which is not supported by Mesh LOD.", i, topology));
				}
			}
			this.SetAllSubMeshesAtOnceFromArray(desc, start, count, flags);
		}

		public void SetSubMeshes(SubMeshDescriptor[] desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.SetSubMeshes(desc, 0, (desc != null) ? desc.Length : 0, flags);
		}

		public void SetSubMeshes(List<SubMeshDescriptor> desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.SetSubMeshes(NoAllocHelpers.ExtractArrayFromList<SubMeshDescriptor>(desc), start, count, flags);
		}

		public void SetSubMeshes(List<SubMeshDescriptor> desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.SetSubMeshes(NoAllocHelpers.ExtractArrayFromList<SubMeshDescriptor>(desc), 0, (desc != null) ? desc.Count : 0, flags);
		}

		public void SetSubMeshes<T>(NativeArray<T> desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			bool flag = UnsafeUtility.SizeOf<T>() != UnsafeUtility.SizeOf<SubMeshDescriptor>();
			if (flag)
			{
				throw new ArgumentException(string.Format("{0} with NativeArray should use struct type that is {1} bytes in size", "SetSubMeshes", UnsafeUtility.SizeOf<SubMeshDescriptor>()));
			}
			bool flag2 = start < 0 || count < 0 || start + count > desc.Length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad start/count arguments (start:{0} count:{1} desc.Length:{2})", start, count, desc.Length));
			}
			this.SetAllSubMeshesAtOnceFromNativeArray((IntPtr)desc.GetUnsafeReadOnlyPtr<T>(), start, count, flags);
		}

		public void SetSubMeshes<T>(NativeArray<T> desc, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
		{
			this.SetSubMeshes<T>(desc, 0, desc.Length, flags);
		}

		private void ValidateLodIndex(int level)
		{
			int lodCount = this.lodCount;
			bool flag = level < 0 || level >= lodCount;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be greater or equal to 0 and less than the lodCount value ({1}).", level, lodCount));
			}
		}

		private void ValidateSubMeshIndex(int submesh)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			if (flag)
			{
				throw new IndexOutOfRangeException(string.Format("Specified submesh index ({0}) is out of range. Must be greater or equal to 0 and less than the subMeshCount value ({1}).", submesh, this.subMeshCount));
			}
		}

		private void ValidateCanWriteToLods()
		{
			bool flag = !this.isLodSelectionActive;
			if (flag)
			{
				throw new InvalidOperationException("Unable to modify LOD0. Please enable Mesh LOD selection first by setting lodCount to a value greater than 1 or modify the submesh descriptors directly.");
			}
		}

		public void SetLod(int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			this.ValidateLodIndex(level);
			this.SetLodImpl(submesh, level, levelRange, flags);
		}

		public void SetLods(List<MeshLodRange> levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = levels == null;
			if (flag)
			{
				throw new ArgumentNullException("levels", "The result levelRanges list cannot be null.");
			}
			int num = NoAllocHelpers.SafeLength<MeshLodRange>(levels);
			bool flag2 = num > this.lodCount;
			if (flag2)
			{
				throw new ArgumentException("levels", string.Format("The number of levels ({0}) in the list cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels.", num, this.lodCount));
			}
			this.SetLods(NoAllocHelpers.ExtractArrayFromList<MeshLodRange>(levels), 0, num, submesh, flags);
		}

		public void SetLods(List<MeshLodRange> levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = levels == null;
			if (flag)
			{
				throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
			}
			int num = NoAllocHelpers.SafeLength<MeshLodRange>(levels);
			bool flag2 = start < 0 || count < 0 || start + count > num;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("start", string.Format("The start ({0}) and the count ({1}) values must be greater than 0, the combined value ({2}) must be less than the list length ({3}).", new object[]
				{
					start,
					count,
					start + count,
					num
				}));
			}
			bool flag3 = count > this.lodCount;
			if (flag3)
			{
				throw new ArgumentException("count", string.Format("The count value ({0}) cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels of detail.", num, this.lodCount));
			}
			this.SetLodsFromArray(NoAllocHelpers.ExtractArrayFromList<MeshLodRange>(levels), start, count, submesh, flags);
		}

		public void SetLods(MeshLodRange[] levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = levels == null;
			if (flag)
			{
				throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
			}
			int num = NoAllocHelpers.SafeLength(levels);
			bool flag2 = num > this.lodCount;
			if (flag2)
			{
				throw new ArgumentException("levels", string.Format("The array length ({0}) cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels.", num, this.lodCount));
			}
			this.SetLodsFromArray(levels, 0, num, submesh, flags);
		}

		public void SetLods(MeshLodRange[] levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = levels == null;
			if (flag)
			{
				throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
			}
			int num = NoAllocHelpers.SafeLength(levels);
			bool flag2 = start < 0 || count < 0 || start + count > num;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("start", string.Format("The start ({0}) and the count ({1}) values must be greater than 0, the combined value ({2}) must be less than the list length ({3}).", new object[]
				{
					start,
					count,
					start + count,
					num
				}));
			}
			bool flag3 = count > this.lodCount;
			if (flag3)
			{
				throw new ArgumentException("count", string.Format("The count value ({0}) cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels.", count, this.lodCount));
			}
			this.SetLodsFromArray(levels, start, count, submesh, flags);
		}

		public void SetLods(NativeArray<MeshLodRange> levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = !levels.IsCreated;
			if (flag)
			{
				throw new ArgumentException("levels", "The NativeArray levels is not created.");
			}
			int length = levels.Length;
			bool flag2 = length > this.lodCount;
			if (flag2)
			{
				throw new ArgumentException("levels", string.Format("The array length ({0}) cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels.", length, this.lodCount));
			}
			this.SetLodsFromNativeArray((IntPtr)levels.GetUnsafeReadOnlyPtr<MeshLodRange>(), length, submesh, flags);
		}

		public unsafe void SetLods(NativeArray<MeshLodRange> levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			this.ValidateCanWriteToLods();
			this.ValidateSubMeshIndex(submesh);
			bool flag = !levels.IsCreated;
			if (flag)
			{
				throw new ArgumentException("levels", "The NativeArray levels is not created.");
			}
			int length = levels.Length;
			bool flag2 = start < 0 || count < 0 || start + count > length;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("start", string.Format("The start ({0}) and the count ({1}) values must be greater than 0, the combined value ({2}) must be less than the list length ({3}).", new object[]
				{
					start,
					count,
					start + count,
					length
				}));
			}
			bool flag3 = count > this.lodCount;
			if (flag3)
			{
				throw new ArgumentException("levels", string.Format("The count value ({0}) cannot exceed the lodCount value ({1}) of the mesh. Please increase the lodCount value first if you need additional levels.", count, this.lodCount));
			}
			this.SetLodsFromNativeArray((IntPtr)levels.GetUnsafeReadOnlyPtr<MeshLodRange>() + start * sizeof(MeshLodRange), count, submesh, flags);
		}

		public MeshLodRange[] GetLods(int submesh)
		{
			this.ValidateSubMeshIndex(submesh);
			return this.GetLodsAlloc(submesh);
		}

		public void GetLods(List<MeshLodRange> levels, int submesh)
		{
			bool flag = levels == null;
			if (flag)
			{
				throw new ArgumentNullException("levels", "The result levels list cannot be null.");
			}
			this.ValidateSubMeshIndex(submesh);
			NoAllocHelpers.EnsureListElemCount<MeshLodRange>(levels, this.lodCount);
			this.GetLodsNonAlloc(NoAllocHelpers.ExtractArrayFromList<MeshLodRange>(levels), submesh);
		}

		public void GetBindposes(List<Matrix4x4> bindposes)
		{
			bool flag = bindposes == null;
			if (flag)
			{
				throw new ArgumentNullException("bindposes", "The result bindposes list cannot be null.");
			}
			NoAllocHelpers.EnsureListElemCount<Matrix4x4>(bindposes, this.bindposeCount);
			this.GetBindposesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList<Matrix4x4>(bindposes));
		}

		public void GetBoneWeights(List<BoneWeight> boneWeights)
		{
			bool flag = boneWeights == null;
			if (flag)
			{
				throw new ArgumentNullException("boneWeights", "The result boneWeights list cannot be null.");
			}
			bool flag2 = this.HasBoneWeights();
			if (flag2)
			{
				NoAllocHelpers.EnsureListElemCount<BoneWeight>(boneWeights, this.vertexCount);
			}
			this.GetBoneWeightsNonAllocImpl(NoAllocHelpers.ExtractArrayFromList<BoneWeight>(boneWeights));
		}

		public BoneWeight[] boneWeights
		{
			get
			{
				return this.GetBoneWeightsImpl();
			}
			set
			{
				this.SetBoneWeightsImpl(value);
			}
		}

		public SkinWeights skinWeightBufferLayout
		{
			get
			{
				return (SkinWeights)this.GetBoneWeightBufferLayoutInternal();
			}
		}

		public void Clear([DefaultValue("true")] bool keepVertexLayout)
		{
			this.ClearImpl(keepVertexLayout);
		}

		[ExcludeFromDocs]
		public void Clear()
		{
			this.ClearImpl(true);
		}

		[ExcludeFromDocs]
		public void RecalculateBounds()
		{
			this.RecalculateBounds(MeshUpdateFlags.Default);
		}

		[ExcludeFromDocs]
		public void RecalculateNormals()
		{
			this.RecalculateNormals(MeshUpdateFlags.Default);
		}

		[ExcludeFromDocs]
		public void RecalculateTangents()
		{
			this.RecalculateTangents(MeshUpdateFlags.Default);
		}

		public void RecalculateBounds([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.RecalculateBoundsImpl(flags);
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateBounds() on mesh '{0}'", base.name));
			}
		}

		public void RecalculateNormals([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.RecalculateNormalsImpl(flags);
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateNormals() on mesh '{0}'", base.name));
			}
		}

		public void RecalculateTangents([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.RecalculateTangentsImpl(flags);
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateTangents() on mesh '{0}'", base.name));
			}
		}

		public void RecalculateUVDistributionMetric(int uvSetIndex, float uvAreaThreshold = 1E-09f)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.RecalculateUVDistributionMetricImpl(uvSetIndex, uvAreaThreshold);
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateUVDistributionMetric() on mesh '{0}'", base.name));
			}
		}

		public void RecalculateUVDistributionMetrics(float uvAreaThreshold = 1E-09f)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.RecalculateUVDistributionMetricsImpl(uvAreaThreshold);
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateUVDistributionMetrics() on mesh '{0}'", base.name));
			}
		}

		public void MarkDynamic()
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.MarkDynamicImpl();
			}
		}

		public void UploadMeshData(bool markNoLongerReadable)
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.UploadMeshDataImpl(markNoLongerReadable);
			}
		}

		public void Optimize()
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.OptimizeImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call Optimize() on mesh '{0}'", base.name));
			}
		}

		public void OptimizeIndexBuffers()
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.OptimizeIndexBuffersImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call OptimizeIndexBuffers() on mesh '{0}'", base.name));
			}
		}

		public void OptimizeReorderVertexBuffer()
		{
			bool canAccess = this.canAccess;
			if (canAccess)
			{
				this.OptimizeReorderVertexBufferImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call OptimizeReorderVertexBuffer() on mesh '{0}'", base.name));
			}
		}

		public MeshTopology GetTopology(int submesh)
		{
			bool flag = submesh < 0 || submesh >= this.subMeshCount;
			MeshTopology meshTopology;
			if (flag)
			{
				Debug.LogError("Failed getting topology. Submesh index is out of bounds.", this);
				meshTopology = MeshTopology.Triangles;
			}
			else
			{
				meshTopology = this.GetTopologyImpl(submesh);
			}
			return meshTopology;
		}

		public void CombineMeshes(CombineInstance[] combine, [DefaultValue("true")] bool mergeSubMeshes, [DefaultValue("true")] bool useMatrices, [DefaultValue("false")] bool hasLightmapData)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData);
		}

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, false);
		}

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, true, false);
		}

		[ExcludeFromDocs]
		public void CombineMeshes(CombineInstance[] combine)
		{
			this.CombineMeshesImpl(combine, true, true, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FromInstanceID_Injected([In] ref EntityId id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IndexFormat get_indexFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_indexFormat_Injected(IntPtr _unity_self, IndexFormat value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetTotalIndexCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIndexBufferParams_Injected(IntPtr _unity_self, int indexCount, IndexFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetIndexBufferData_Injected(IntPtr _unity_self, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetIndexBufferDataFromArray_Injected(IntPtr _unity_self, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVertexBufferParamsFromPtr_Injected(IntPtr _unity_self, int vertexCount, IntPtr attributesPtr, int attributesCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVertexBufferParamsFromArray_Injected(IntPtr _unity_self, int vertexCount, params ManagedSpanWrapper attributes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetVertexBufferData_Injected(IntPtr _unity_self, int stream, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetVertexBufferDataFromArray_Injected(IntPtr _unity_self, int stream, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array GetVertexAttributesAlloc_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributesArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper attributes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributesList_Injected(IntPtr _unity_self, ref BlittableListWrapper attributes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributeCountImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVertexAttribute_Injected(IntPtr _unity_self, int index, out VertexAttributeDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetIndexStartImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetIndexCountImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetTrianglesCountImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetBaseVertexImpl_Injected(IntPtr _unity_self, int submesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrianglesImpl_Injected(IntPtr _unity_self, int submesh, bool applyBaseVertex, int meshlod, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesImpl_Injected(IntPtr _unity_self, int submesh, bool applyBaseVertex, int meshlod, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIndicesImpl_Injected(IntPtr _unity_self, int submesh, MeshTopology topology, IndexFormat indicesFormat, Array indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIndicesNativeArrayImpl_Injected(IntPtr _unity_self, int submesh, MeshTopology topology, IndexFormat indicesFormat, IntPtr indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrianglesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTrianglesNonAllocImpl16_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesNonAllocImpl16_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PrintErrorCantAccessChannel_Injected(IntPtr _unity_self, VertexAttribute ch);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVertexAttribute_Injected(IntPtr _unity_self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributeDimension_Injected(IntPtr _unity_self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VertexAttributeFormat GetVertexAttributeFormat_Injected(IntPtr _unity_self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributeStream_Injected(IntPtr _unity_self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexAttributeOffset_Injected(IntPtr _unity_self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetArrayForChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNativeArrayForChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array GetAllocArrayFromChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetArrayFromChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, Array values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_vertexBufferCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexBufferStride_Injected(IntPtr _unity_self, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeVertexBufferPtr_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeIndexBufferPtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetVertexBufferImpl_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetIndexBufferImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBoneWeightBufferImpl_Injected(IntPtr _unity_self, int bonesPerVertex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBlendShapeBufferImpl_Injected(IntPtr _unity_self, int layout);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsBuffer.Target get_vertexBufferTarget_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vertexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsBuffer.Target get_indexBufferTarget_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_indexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_blendShapeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearBlendShapes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBlendShapeName_Injected(IntPtr _unity_self, int shapeIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBlendShapeIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper blendShapeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBlendShapeFrameCount_Injected(IntPtr _unity_self, int shapeIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetBlendShapeFrameWeight_Injected(IntPtr _unity_self, int shapeIndex, int frameIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBlendShapeFrameVertices_Injected(IntPtr _unity_self, int shapeIndex, int frameIndex, ref ManagedSpanWrapper deltaVertices, ref ManagedSpanWrapper deltaNormals, ref ManagedSpanWrapper deltaTangents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddBlendShapeFrame_Injected(IntPtr _unity_self, ref ManagedSpanWrapper shapeName, float frameWeight, ref ManagedSpanWrapper deltaVertices, ref ManagedSpanWrapper deltaNormals, ref ManagedSpanWrapper deltaTangents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBlendShapeOffsetInternal_Injected(IntPtr _unity_self, int index, out BlendShape ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasBoneWeights_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBoneWeightsImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoneWeightsImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper weights);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBoneWeights_Injected(IntPtr _unity_self, IntPtr bonesPerVertex, int bonesPerVertexSize, IntPtr weights, int weightsSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAllBoneWeightsArraySize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBoneWeightBufferLayoutInternal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAllBoneWeightsArray_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBonesPerVertexArray_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_bindposeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bindposes_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bindposes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBindposesFromScript_NativeArray_Injected(IntPtr _unity_self, IntPtr posesPtr, int posesCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBindposesArray_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBoneWeightsNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBindposesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isReadable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_canAccess_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_vertexCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_subMeshCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_subMeshCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSubMesh_Injected(IntPtr _unity_self, int index, [In] ref SubMeshDescriptor desc, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSubMesh_Injected(IntPtr _unity_self, int index, out SubMeshDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAllSubMeshesAtOnceFromArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper desc, int start, int count, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAllSubMeshesAtOnceFromNativeArray_Injected(IntPtr _unity_self, IntPtr desc, int start, int count, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodCount_Injected(IntPtr _unity_self, int numLevels);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodSelectionCurve_Injected(IntPtr _unity_self, [In] ref Mesh.LodSelectionCurve lodSelectionCurve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodsFromArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper levelRanges, int start, int count, int submesh, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodsFromNativeArray_Injected(IntPtr _unity_self, IntPtr lodLevels, int count, int submesh, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodImpl_Injected(IntPtr _unity_self, int subMeshIndex, int level, [In] ref MeshLodRange levelRange, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLodsAlloc_Injected(IntPtr _unity_self, int subMeshIndex, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLodsNonAlloc_Injected(IntPtr _unity_self, out BlittableArrayWrapper levels, int subMeshIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLodCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLodSelectionCurve_Injected(IntPtr _unity_self, out Mesh.LodSelectionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLod_Injected(IntPtr _unity_self, int subMeshIndex, int levelIndex, out MeshLodRange ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearImpl_Injected(IntPtr _unity_self, bool keepVertexLayout);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecalculateBoundsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecalculateNormalsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecalculateTangentsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkDynamicImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkModified_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UploadMeshDataImpl_Injected(IntPtr _unity_self, bool markNoLongerReadable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MeshTopology GetTopologyImpl_Injected(IntPtr _unity_self, int submesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecalculateUVDistributionMetricImpl_Injected(IntPtr _unity_self, int uvSetIndex, float uvAreaThreshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RecalculateUVDistributionMetricsImpl_Injected(IntPtr _unity_self, float uvAreaThreshold);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetUVDistributionMetric_Injected(IntPtr _unity_self, int uvSetIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CombineMeshesImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OptimizeImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OptimizeIndexBuffersImpl_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OptimizeReorderVertexBufferImpl_Injected(IntPtr _unity_self);

		[UsedByNativeCode]
		[Serializable]
		public struct LodSelectionCurve
		{
			public LodSelectionCurve(float slope, float bias)
			{
				this.m_LodSlope = slope;
				this.m_LodBias = bias;
			}

			public bool IsValid()
			{
				return this.m_LodSlope > 0.001f;
			}

			public float lodSlope
			{
				get
				{
					return this.m_LodSlope;
				}
				set
				{
					this.m_LodSlope = value;
				}
			}

			public float lodBias
			{
				get
				{
					return this.m_LodBias;
				}
				set
				{
					this.m_LodBias = value;
				}
			}

			[SerializeField]
			private float m_LodSlope;

			[SerializeField]
			private float m_LodBias;
		}

		[StaticAccessor("MeshDataBindings", StaticAccessorType.DoubleColon)]
		[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
		public struct MeshData
		{
			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern bool HasVertexAttribute(IntPtr self, VertexAttribute attr);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexAttributeDimension(IntPtr self, VertexAttribute attr);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern VertexAttributeFormat GetVertexAttributeFormat(IntPtr self, VertexAttribute attr);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexAttributeStream(IntPtr self, VertexAttribute attr);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexAttributeOffset(IntPtr self, VertexAttribute attr);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexCount(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexBufferCount(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetVertexDataPtr(IntPtr self, int stream);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ulong GetVertexDataSize(IntPtr self, int stream);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetVertexBufferStride(IntPtr self, int stream);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void CopyAttributeIntoPtr(IntPtr self, VertexAttribute attr, VertexAttributeFormat format, int dim, IntPtr dst);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void CopyIndicesIntoPtr(IntPtr self, int submesh, int meshLod, bool applyBaseVertex, int dstStride, IntPtr dst);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IndexFormat GetIndexFormat(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetIndexCount(IntPtr self, int submesh, int meshlod);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetIndexDataPtr(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern ulong GetIndexDataSize(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetSubMeshCount(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetLodCount(IntPtr self);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLodCount(IntPtr self, int count);

			[NativeMethod(IsThreadSafe = true)]
			private static Mesh.LodSelectionCurve GetLodSelectionCurve(IntPtr self)
			{
				Mesh.LodSelectionCurve lodSelectionCurve;
				Mesh.MeshData.GetLodSelectionCurve_Injected(self, out lodSelectionCurve);
				return lodSelectionCurve;
			}

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			private static void SetLodSelectionCurve(IntPtr self, Mesh.LodSelectionCurve lodSelectionCurve)
			{
				Mesh.MeshData.SetLodSelectionCurve_Injected(self, ref lodSelectionCurve);
			}

			[NativeMethod(IsThreadSafe = true)]
			private static MeshLodRange GetLod(IntPtr self, int submesh, int level)
			{
				MeshLodRange meshLodRange;
				Mesh.MeshData.GetLod_Injected(self, submesh, level, out meshLodRange);
				return meshLodRange;
			}

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			private static void SetLod(IntPtr self, int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags)
			{
				Mesh.MeshData.SetLod_Injected(self, submesh, level, ref levelRange, flags);
			}

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			private static SubMeshDescriptor GetSubMesh(IntPtr self, int index)
			{
				SubMeshDescriptor subMeshDescriptor;
				Mesh.MeshData.GetSubMesh_Injected(self, index, out subMeshDescriptor);
				return subMeshDescriptor;
			}

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVertexBufferParamsFromPtr(IntPtr self, int vertexCount, IntPtr attributesPtr, int attributesCount);

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			private unsafe static void SetVertexBufferParamsFromArray(IntPtr self, int vertexCount, params VertexAttributeDescriptor[] attributes)
			{
				Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
				fixed (VertexAttributeDescriptor* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Mesh.MeshData.SetVertexBufferParamsFromArray_Injected(self, vertexCount, ref managedSpanWrapper);
				}
			}

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetIndexBufferParamsImpl(IntPtr self, int indexCount, IndexFormat indexFormat);

			[NativeMethod(IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubMeshCount(IntPtr self, int count);

			[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
			private static void SetSubMeshImpl(IntPtr self, int index, SubMeshDescriptor desc, MeshUpdateFlags flags)
			{
				Mesh.MeshData.SetSubMeshImpl_Injected(self, index, ref desc, flags);
			}

			public int vertexCount
			{
				get
				{
					return Mesh.MeshData.GetVertexCount(this.m_Ptr);
				}
			}

			public int vertexBufferCount
			{
				get
				{
					return Mesh.MeshData.GetVertexBufferCount(this.m_Ptr);
				}
			}

			public int GetVertexBufferStride(int stream)
			{
				return Mesh.MeshData.GetVertexBufferStride(this.m_Ptr, stream);
			}

			public bool HasVertexAttribute(VertexAttribute attr)
			{
				return Mesh.MeshData.HasVertexAttribute(this.m_Ptr, attr);
			}

			public int GetVertexAttributeDimension(VertexAttribute attr)
			{
				return Mesh.MeshData.GetVertexAttributeDimension(this.m_Ptr, attr);
			}

			public VertexAttributeFormat GetVertexAttributeFormat(VertexAttribute attr)
			{
				return Mesh.MeshData.GetVertexAttributeFormat(this.m_Ptr, attr);
			}

			public int GetVertexAttributeStream(VertexAttribute attr)
			{
				return Mesh.MeshData.GetVertexAttributeStream(this.m_Ptr, attr);
			}

			public int GetVertexAttributeOffset(VertexAttribute attr)
			{
				return Mesh.MeshData.GetVertexAttributeOffset(this.m_Ptr, attr);
			}

			public void GetVertices(NativeArray<Vector3> outVertices)
			{
				this.CopyAttributeInto<Vector3>(outVertices, VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
			}

			public void GetNormals(NativeArray<Vector3> outNormals)
			{
				this.CopyAttributeInto<Vector3>(outNormals, VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);
			}

			public void GetTangents(NativeArray<Vector4> outTangents)
			{
				this.CopyAttributeInto<Vector4>(outTangents, VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4);
			}

			public void GetColors(NativeArray<Color> outColors)
			{
				this.CopyAttributeInto<Color>(outColors, VertexAttribute.Color, VertexAttributeFormat.Float32, 4);
			}

			public void GetColors(NativeArray<Color32> outColors)
			{
				this.CopyAttributeInto<Color32>(outColors, VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4);
			}

			public void GetUVs(int channel, NativeArray<Vector2> outUVs)
			{
				bool flag = channel < 0 || channel > 7;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
				}
				this.CopyAttributeInto<Vector2>(outUVs, Mesh.GetUVChannel(channel), VertexAttributeFormat.Float32, 2);
			}

			public void GetUVs(int channel, NativeArray<Vector3> outUVs)
			{
				bool flag = channel < 0 || channel > 7;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
				}
				this.CopyAttributeInto<Vector3>(outUVs, Mesh.GetUVChannel(channel), VertexAttributeFormat.Float32, 3);
			}

			public void GetUVs(int channel, NativeArray<Vector4> outUVs)
			{
				bool flag = channel < 0 || channel > 7;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
				}
				this.CopyAttributeInto<Vector4>(outUVs, Mesh.GetUVChannel(channel), VertexAttributeFormat.Float32, 4);
			}

			public unsafe NativeArray<T> GetVertexData<T>([DefaultValue("0")] int stream = 0) where T : struct
			{
				bool flag = stream < 0 || stream >= this.vertexBufferCount;
				if (flag)
				{
					throw new ArgumentOutOfRangeException(string.Format("{0} out of bounds, should be below {1} but was {2}", "stream", this.vertexBufferCount, stream));
				}
				ulong vertexDataSize = Mesh.MeshData.GetVertexDataSize(this.m_Ptr, stream);
				ulong num = (ulong)((long)UnsafeUtility.SizeOf<T>());
				bool flag2 = vertexDataSize % num > 0UL;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Type passed to {0} can't capture the vertex buffer. Mesh vertex buffer size is {1} which is not a multiple of type size {2}", "GetVertexData", vertexDataSize, num));
				}
				ulong num2 = vertexDataSize / num;
				return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)Mesh.MeshData.GetVertexDataPtr(this.m_Ptr, stream), (int)num2, Allocator.None);
			}

			private void CopyAttributeInto<T>(NativeArray<T> buffer, VertexAttribute channel, VertexAttributeFormat format, int dim) where T : struct
			{
				bool flag = !this.HasVertexAttribute(channel);
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Mesh data does not have {0} vertex component", channel));
				}
				bool flag2 = buffer.Length < this.vertexCount;
				if (flag2)
				{
					throw new InvalidOperationException(string.Format("Not enough space in output buffer (need {0}, has {1})", this.vertexCount, buffer.Length));
				}
				Mesh.MeshData.CopyAttributeIntoPtr(this.m_Ptr, channel, format, dim, (IntPtr)buffer.GetUnsafePtr<T>());
			}

			public void SetVertexBufferParams(int vertexCount, params VertexAttributeDescriptor[] attributes)
			{
				Mesh.MeshData.SetVertexBufferParamsFromArray(this.m_Ptr, vertexCount, attributes);
			}

			public void SetVertexBufferParams(int vertexCount, NativeArray<VertexAttributeDescriptor> attributes)
			{
				Mesh.MeshData.SetVertexBufferParamsFromPtr(this.m_Ptr, vertexCount, (IntPtr)attributes.GetUnsafeReadOnlyPtr<VertexAttributeDescriptor>(), attributes.Length);
			}

			public void SetIndexBufferParams(int indexCount, IndexFormat format)
			{
				Mesh.MeshData.SetIndexBufferParamsImpl(this.m_Ptr, indexCount, format);
			}

			public IndexFormat indexFormat
			{
				get
				{
					return Mesh.MeshData.GetIndexFormat(this.m_Ptr);
				}
			}

			public void GetIndices(NativeArray<ushort> outIndices, int submesh, [DefaultValue("true")] bool applyBaseVertex = true)
			{
				this.GetIndices(outIndices, submesh, 0, applyBaseVertex);
			}

			public void GetIndices(NativeArray<ushort> outIndices, int submesh, int meshlod, [DefaultValue("true")] bool applyBaseVertex = true)
			{
				bool flag = submesh < 0 || submesh >= this.subMeshCount;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Specified submesh ({0}) is out of range. Must be greater or equal to 0 and less than subMeshCount ({1}).", submesh, this.subMeshCount));
				}
				bool flag2 = meshlod > 0 && meshlod >= this.lodCount;
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be less than the lodCount value ({1})", meshlod, this.lodCount));
				}
				int indexCount = Mesh.MeshData.GetIndexCount(this.m_Ptr, submesh, meshlod);
				bool flag3 = outIndices.Length < indexCount;
				if (flag3)
				{
					throw new InvalidOperationException(string.Format("Not enough space in output buffer (need {0}, has {1})", indexCount, outIndices.Length));
				}
				Mesh.MeshData.CopyIndicesIntoPtr(this.m_Ptr, submesh, meshlod, applyBaseVertex, 2, (IntPtr)outIndices.GetUnsafePtr<ushort>());
			}

			public void GetIndices(NativeArray<int> outIndices, int submesh, [DefaultValue("true")] bool applyBaseVertex = true)
			{
				this.GetIndices(outIndices, submesh, 0, applyBaseVertex);
			}

			public void GetIndices(NativeArray<int> outIndices, int submesh, int meshlod, [DefaultValue("true")] bool applyBaseVertex = true)
			{
				bool flag = submesh < 0 || submesh >= this.subMeshCount;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Specified submesh ({0}) is out of range. Must be greater or equal to 0 and less than subMeshCount ({1}).", submesh, this.subMeshCount));
				}
				bool flag2 = meshlod > 0 && meshlod >= this.lodCount;
				if (flag2)
				{
					throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be less than the lodCount value ({1})", meshlod, this.lodCount));
				}
				int indexCount = Mesh.MeshData.GetIndexCount(this.m_Ptr, submesh, meshlod);
				bool flag3 = outIndices.Length < indexCount;
				if (flag3)
				{
					throw new InvalidOperationException(string.Format("Not enough space in output buffer (need {0}, has {1})", indexCount, outIndices.Length));
				}
				Mesh.MeshData.CopyIndicesIntoPtr(this.m_Ptr, submesh, meshlod, applyBaseVertex, 4, (IntPtr)outIndices.GetUnsafePtr<int>());
			}

			public unsafe NativeArray<T> GetIndexData<T>() where T : struct
			{
				ulong indexDataSize = Mesh.MeshData.GetIndexDataSize(this.m_Ptr);
				ulong num = (ulong)((long)UnsafeUtility.SizeOf<T>());
				bool flag = indexDataSize % num > 0UL;
				if (flag)
				{
					throw new ArgumentException(string.Format("Type passed to {0} can't capture the index buffer. Mesh index buffer size is {1} which is not a multiple of type size {2}", "GetIndexData", indexDataSize, num));
				}
				ulong num2 = indexDataSize / num;
				return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)Mesh.MeshData.GetIndexDataPtr(this.m_Ptr), (int)num2, Allocator.None);
			}

			public MeshLodRange GetLod(int submesh, int level)
			{
				this.ValidateSubMeshIndex(submesh);
				this.ValidateLodIndex(level);
				return Mesh.MeshData.GetLod(this.m_Ptr, submesh, level);
			}

			public void SetLod(int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
			{
				bool flag = !this.isLodSelectionActive;
				if (flag)
				{
					throw new InvalidOperationException("Unable to modify LOD0. Please enable Mesh LOD selection first by setting lodCount to a value greater than 1 or modify the submesh descriptors directly.");
				}
				this.ValidateSubMeshIndex(submesh);
				this.ValidateLodIndex(level);
				Mesh.MeshData.SetLod(this.m_Ptr, submesh, level, levelRange, flags);
			}

			public int subMeshCount
			{
				get
				{
					return Mesh.MeshData.GetSubMeshCount(this.m_Ptr);
				}
				set
				{
					Mesh.MeshData.SetSubMeshCount(this.m_Ptr, value);
				}
			}

			public SubMeshDescriptor GetSubMesh(int index)
			{
				return Mesh.MeshData.GetSubMesh(this.m_Ptr, index);
			}

			public void SetSubMesh(int index, SubMeshDescriptor desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
			{
				Mesh.MeshData.SetSubMeshImpl(this.m_Ptr, index, desc, flags);
			}

			public int lodCount
			{
				get
				{
					return Mesh.MeshData.GetLodCount(this.m_Ptr);
				}
				set
				{
					bool flag = value < 1;
					if (flag)
					{
						throw new ArgumentException("LOD count must be greater than zero.");
					}
					bool flag2 = value > 1;
					if (flag2)
					{
						for (int i = 0; i < this.subMeshCount; i++)
						{
							bool flag3 = this.GetSubMesh(i).topology > MeshTopology.Triangles;
							if (flag3)
							{
								throw new InvalidOperationException("Mesh LOD selection only works for triangle topology. The LOD count value cannot be higher than 1 if the topology is not set to triangles for all submeshes.");
							}
						}
					}
					Mesh.MeshData.SetLodCount(this.m_Ptr, value);
				}
			}

			internal bool isLodSelectionActive
			{
				get
				{
					return this.lodCount > 1;
				}
			}

			public Mesh.LodSelectionCurve lodSelectionCurve
			{
				get
				{
					return Mesh.MeshData.GetLodSelectionCurve(this.m_Ptr);
				}
				set
				{
					Mesh.MeshData.SetLodSelectionCurve(this.m_Ptr, value);
				}
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckReadAccess()
			{
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckWriteAccess()
			{
			}

			private void ValidateSubMeshIndex(int submesh)
			{
				bool flag = submesh < 0 || submesh >= this.subMeshCount;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Specified submesh index ({0}) is out of range. Must be greater or equal to 0 and less than the subMeshCount value ({1}).", submesh, this.subMeshCount));
				}
			}

			private void ValidateLodIndex(int level)
			{
				int lodCount = this.lodCount;
				bool flag = level < 0 || level >= lodCount;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("Specified Mesh LOD index ({0}) is out of range. Must be greater than or equal to 0 and less than the lodCount value ({1}).", level, lodCount));
				}
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLodSelectionCurve_Injected(IntPtr self, out Mesh.LodSelectionCurve ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLodSelectionCurve_Injected(IntPtr self, [In] ref Mesh.LodSelectionCurve lodSelectionCurve);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetLod_Injected(IntPtr self, int submesh, int level, out MeshLodRange ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetLod_Injected(IntPtr self, int submesh, int level, [In] ref MeshLodRange levelRange, MeshUpdateFlags flags);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void GetSubMesh_Injected(IntPtr self, int index, out SubMeshDescriptor ret);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetVertexBufferParamsFromArray_Injected(IntPtr self, int vertexCount, params ManagedSpanWrapper attributes);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void SetSubMeshImpl_Injected(IntPtr self, int index, [In] ref SubMeshDescriptor desc, MeshUpdateFlags flags);

			[NativeDisableUnsafePtrRestriction]
			internal IntPtr m_Ptr;
		}

		[NativeContainerSupportsMinMaxWriteRestriction]
		[StaticAccessor("MeshDataArrayBindings", StaticAccessorType.DoubleColon)]
		[NativeContainer]
		public struct MeshDataArray : IDisposable
		{
			private unsafe static void AcquireReadOnlyMeshData([NotNull] Mesh mesh, IntPtr* datas)
			{
				if (mesh == null)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				Mesh.MeshDataArray.AcquireReadOnlyMeshData_Injected(intPtr, datas);
			}

			private unsafe static void AcquireReadOnlyMeshDatas([NotNull] Mesh[] meshes, IntPtr* datas, int count)
			{
				if (meshes == null)
				{
					ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
				}
				Mesh.MeshDataArray.AcquireReadOnlyMeshDatas_Injected(meshes, datas, count);
			}

			private unsafe static void AcquireMeshDataCopy([NotNull] Mesh mesh, IntPtr* datas)
			{
				if (mesh == null)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				Mesh.MeshDataArray.AcquireMeshDataCopy_Injected(intPtr, datas);
			}

			private unsafe static void AcquireMeshDatasCopy([NotNull] Mesh[] meshes, IntPtr* datas, int count)
			{
				if (meshes == null)
				{
					ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
				}
				Mesh.MeshDataArray.AcquireMeshDatasCopy_Injected(meshes, datas, count);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void ReleaseMeshDatas(IntPtr* datas, int count);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void CreateNewMeshDatas(IntPtr* datas, int count);

			[NativeThrows]
			private unsafe static void ApplyToMeshesImpl([NotNull] Mesh[] meshes, IntPtr* datas, int count, MeshUpdateFlags flags)
			{
				if (meshes == null)
				{
					ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
				}
				Mesh.MeshDataArray.ApplyToMeshesImpl_Injected(meshes, datas, count, flags);
			}

			[NativeThrows]
			private static void ApplyToMeshImpl([NotNull] Mesh mesh, IntPtr data, MeshUpdateFlags flags)
			{
				if (mesh == null)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
				}
				Mesh.MeshDataArray.ApplyToMeshImpl_Injected(intPtr, data, flags);
			}

			public int Length
			{
				get
				{
					return this.m_Length;
				}
			}

			public unsafe Mesh.MeshData this[int index]
			{
				get
				{
					Mesh.MeshData meshData;
					meshData.m_Ptr = this.m_Ptrs[(IntPtr)index * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
					return meshData;
				}
			}

			public unsafe void Dispose()
			{
				UnsafeUtility.LeakErase((IntPtr)((void*)this.m_Ptrs), LeakCategory.MeshDataArray);
				bool flag = this.m_Length != 0;
				if (flag)
				{
					Mesh.MeshDataArray.ReleaseMeshDatas(this.m_Ptrs, this.m_Length);
					UnsafeUtility.Free((void*)this.m_Ptrs, Allocator.Persistent);
				}
				this.m_Ptrs = null;
				this.m_Length = 0;
			}

			internal unsafe void ApplyToMeshAndDispose(Mesh mesh, MeshUpdateFlags flags)
			{
				bool flag = !mesh.canAccess;
				if (flag)
				{
					throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + mesh.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
				}
				Mesh.MeshDataArray.ApplyToMeshImpl(mesh, *this.m_Ptrs, flags);
				this.Dispose();
			}

			internal void ApplyToMeshesAndDispose(Mesh[] meshes, MeshUpdateFlags flags)
			{
				for (int i = 0; i < this.m_Length; i++)
				{
					Mesh mesh = meshes[i];
					bool flag = mesh == null;
					if (flag)
					{
						throw new ArgumentNullException("meshes", string.Format("Mesh at index {0} is null", i));
					}
					bool flag2 = !mesh.canAccess;
					if (flag2)
					{
						throw new InvalidOperationException(string.Format("Not allowed to access vertex data on mesh '{0}' at array index {1} (isReadable is false; Read/Write must be enabled in import settings)", mesh.name, i));
					}
				}
				Mesh.MeshDataArray.ApplyToMeshesImpl(meshes, this.m_Ptrs, this.m_Length, flags);
				this.Dispose();
			}

			internal unsafe MeshDataArray(Mesh mesh, bool checkReadWrite = true, bool createAsCopy = false)
			{
				bool flag = mesh == null;
				if (flag)
				{
					throw new ArgumentNullException("mesh", "Mesh is null");
				}
				bool flag2 = checkReadWrite && !mesh.canAccess;
				if (flag2)
				{
					throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + mesh.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
				}
				this.m_Length = 1;
				int num = UnsafeUtility.SizeOf<IntPtr>();
				this.m_Ptrs = (IntPtr*)UnsafeUtility.Malloc((long)num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
				if (createAsCopy)
				{
					Mesh.MeshDataArray.AcquireMeshDataCopy(mesh, this.m_Ptrs);
				}
				else
				{
					Mesh.MeshDataArray.AcquireReadOnlyMeshData(mesh, this.m_Ptrs);
				}
				UnsafeUtility.LeakRecord((IntPtr)((void*)this.m_Ptrs), LeakCategory.MeshDataArray, 0);
			}

			internal unsafe MeshDataArray(Mesh[] meshes, int meshesCount, bool checkReadWrite = true, bool createAsCopy = false)
			{
				bool flag = meshes.Length < meshesCount;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Meshes array size ({0}) is smaller than meshes count ({1})", meshes.Length, meshesCount));
				}
				for (int i = 0; i < meshesCount; i++)
				{
					Mesh mesh = meshes[i];
					bool flag2 = mesh == null;
					if (flag2)
					{
						throw new ArgumentNullException("meshes", string.Format("Mesh at index {0} is null", i));
					}
					bool flag3 = checkReadWrite && !mesh.canAccess;
					if (flag3)
					{
						throw new InvalidOperationException(string.Format("Not allowed to access vertex data on mesh '{0}' at array index {1} (isReadable is false; Read/Write must be enabled in import settings)", mesh.name, i));
					}
				}
				this.m_Length = meshesCount;
				int num = UnsafeUtility.SizeOf<IntPtr>() * meshesCount;
				this.m_Ptrs = (IntPtr*)UnsafeUtility.Malloc((long)num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
				if (createAsCopy)
				{
					Mesh.MeshDataArray.AcquireMeshDatasCopy(meshes, this.m_Ptrs, meshesCount);
				}
				else
				{
					Mesh.MeshDataArray.AcquireReadOnlyMeshDatas(meshes, this.m_Ptrs, meshesCount);
				}
			}

			internal unsafe MeshDataArray(int meshesCount)
			{
				bool flag = meshesCount < 0;
				if (flag)
				{
					throw new InvalidOperationException(string.Format("Mesh count can not be negative (was {0})", meshesCount));
				}
				this.m_Length = meshesCount;
				int num = UnsafeUtility.SizeOf<IntPtr>() * meshesCount;
				this.m_Ptrs = (IntPtr*)UnsafeUtility.Malloc((long)num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
				Mesh.MeshDataArray.CreateNewMeshDatas(this.m_Ptrs, meshesCount);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckElementReadAccess(int index)
			{
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void AcquireReadOnlyMeshData_Injected(IntPtr mesh, IntPtr* datas);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void AcquireReadOnlyMeshDatas_Injected(Mesh[] meshes, IntPtr* datas, int count);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void AcquireMeshDataCopy_Injected(IntPtr mesh, IntPtr* datas);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void AcquireMeshDatasCopy_Injected(Mesh[] meshes, IntPtr* datas, int count);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private unsafe static extern void ApplyToMeshesImpl_Injected(Mesh[] meshes, IntPtr* datas, int count, MeshUpdateFlags flags);

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void ApplyToMeshImpl_Injected(IntPtr mesh, IntPtr data, MeshUpdateFlags flags);

			[NativeDisableUnsafePtrRestriction]
			internal unsafe IntPtr* m_Ptrs;

			internal int m_Length;
		}
	}
}
