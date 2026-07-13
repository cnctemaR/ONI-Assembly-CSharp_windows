using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Graphics/BillboardRenderer.bindings.h")]
	[NativeHeader("Runtime/Graphics/Billboard/BillboardAsset.h")]
	public sealed class BillboardAsset : Object
	{
		public BillboardAsset()
		{
			BillboardAsset.Internal_Create(this);
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::Internal_Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] BillboardAsset obj);

		public float width
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_width_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BillboardAsset.set_width_Injected(intPtr, value);
			}
		}

		public float height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_height_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BillboardAsset.set_height_Injected(intPtr, value);
			}
		}

		public float bottom
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_bottom_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BillboardAsset.set_bottom_Injected(intPtr, value);
			}
		}

		public int imageCount
		{
			[NativeMethod("GetNumImages")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_imageCount_Injected(intPtr);
			}
		}

		public int vertexCount
		{
			[NativeMethod("GetNumVertices")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_vertexCount_Injected(intPtr);
			}
		}

		public int indexCount
		{
			[NativeMethod("GetNumIndices")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return BillboardAsset.get_indexCount_Injected(intPtr);
			}
		}

		public Material material
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(BillboardAsset.get_material_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BillboardAsset.set_material_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public void GetImageTexCoords(List<Vector4> imageTexCoords)
		{
			bool flag = imageTexCoords == null;
			if (flag)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.GetImageTexCoordsInternal(imageTexCoords);
		}

		[NativeMethod("GetBillboardDataReadonly().GetImageTexCoords")]
		public Vector4[] GetImageTexCoords()
		{
			Vector4[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				BillboardAsset.GetImageTexCoords_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector4[] array;
				blittableArrayWrapper.Unmarshal<Vector4>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::GetImageTexCoordsInternal", HasExplicitThis = true)]
		internal void GetImageTexCoordsInternal(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.GetImageTexCoordsInternal_Injected(intPtr, list);
		}

		public void SetImageTexCoords(List<Vector4> imageTexCoords)
		{
			bool flag = imageTexCoords == null;
			if (flag)
			{
				throw new ArgumentNullException("imageTexCoords");
			}
			this.SetImageTexCoordsInternalList(imageTexCoords);
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetImageTexCoords", HasExplicitThis = true)]
		public unsafe void SetImageTexCoords([NotNull] Vector4[] imageTexCoords)
		{
			if (imageTexCoords == null)
			{
				ThrowHelper.ThrowArgumentNullException(imageTexCoords, "imageTexCoords");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(imageTexCoords);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BillboardAsset.SetImageTexCoords_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetImageTexCoordsInternalList", HasExplicitThis = true)]
		internal void SetImageTexCoordsInternalList(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.SetImageTexCoordsInternalList_Injected(intPtr, list);
		}

		public void GetVertices(List<Vector2> vertices)
		{
			bool flag = vertices == null;
			if (flag)
			{
				throw new ArgumentNullException("vertices");
			}
			this.GetVerticesInternal(vertices);
		}

		[NativeMethod("GetBillboardDataReadonly().GetVertices")]
		public Vector2[] GetVertices()
		{
			Vector2[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				BillboardAsset.GetVertices_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector2[] array;
				blittableArrayWrapper.Unmarshal<Vector2>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::GetVerticesInternal", HasExplicitThis = true)]
		internal void GetVerticesInternal(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.GetVerticesInternal_Injected(intPtr, list);
		}

		public void SetVertices(List<Vector2> vertices)
		{
			bool flag = vertices == null;
			if (flag)
			{
				throw new ArgumentNullException("vertices");
			}
			this.SetVerticesInternalList(vertices);
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetVertices", HasExplicitThis = true)]
		public unsafe void SetVertices([NotNull] Vector2[] vertices)
		{
			if (vertices == null)
			{
				ThrowHelper.ThrowArgumentNullException(vertices, "vertices");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector2> span = new Span<Vector2>(vertices);
			fixed (Vector2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BillboardAsset.SetVertices_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetVerticesInternalList", HasExplicitThis = true)]
		internal void SetVerticesInternalList(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.SetVerticesInternalList_Injected(intPtr, list);
		}

		public void GetIndices(List<ushort> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.GetIndicesInternal(indices);
		}

		[NativeMethod("GetBillboardDataReadonly().GetIndices")]
		public ushort[] GetIndices()
		{
			ushort[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				BillboardAsset.GetIndices_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ushort[] array;
				blittableArrayWrapper.Unmarshal<ushort>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::GetIndicesInternal", HasExplicitThis = true)]
		internal void GetIndicesInternal(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.GetIndicesInternal_Injected(intPtr, list);
		}

		public void SetIndices(List<ushort> indices)
		{
			bool flag = indices == null;
			if (flag)
			{
				throw new ArgumentNullException("indices");
			}
			this.SetIndicesInternalList(indices);
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetIndices", HasExplicitThis = true)]
		public unsafe void SetIndices([NotNull] ushort[] indices)
		{
			if (indices == null)
			{
				ThrowHelper.ThrowArgumentNullException(indices, "indices");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<ushort> span = new Span<ushort>(indices);
			fixed (ushort* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BillboardAsset.SetIndices_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::SetIndicesInternalList", HasExplicitThis = true)]
		internal void SetIndicesInternalList(object list)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.SetIndicesInternalList_Injected(intPtr, list);
		}

		[FreeFunction(Name = "BillboardRenderer_Bindings::MakeMaterialProperties", HasExplicitThis = true)]
		internal void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<BillboardAsset>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BillboardAsset.MakeMaterialProperties_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), Object.MarshalledUnityObject.Marshal<Camera>(camera));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_width_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_width_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_height_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_bottom_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bottom_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_imageCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_vertexCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_indexCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_material_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetImageTexCoords_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetImageTexCoordsInternal_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetImageTexCoords_Injected(IntPtr _unity_self, ref ManagedSpanWrapper imageTexCoords);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetImageTexCoordsInternalList_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVertices_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVerticesInternal_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVertices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vertices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVerticesInternalList_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndices_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesInternal_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIndicesInternalList_Injected(IntPtr _unity_self, object list);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeMaterialProperties_Injected(IntPtr _unity_self, IntPtr properties, IntPtr camera);
	}
}
