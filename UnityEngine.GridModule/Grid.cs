using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Transform))]
	[NativeType(Header = "Modules/Grid/Public/Grid.h")]
	public sealed class Grid : GridLayout
	{
		public Vector3 GetCellCenterLocal(Vector3Int position)
		{
			return base.CellToLocalInterpolated(position + base.GetLayoutCellCenter());
		}

		public Vector3 GetCellCenterWorld(Vector3Int position)
		{
			return base.LocalToWorld(base.CellToLocalInterpolated(position + base.GetLayoutCellCenter()));
		}

		public new Vector3 cellSize
		{
			[FreeFunction("GridBindings::GetCellSize", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Grid.get_cellSize_Injected(intPtr, out vector);
				return vector;
			}
			[FreeFunction("GridBindings::SetCellSize", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Grid.set_cellSize_Injected(intPtr, ref value);
			}
		}

		public new Vector3 cellGap
		{
			[FreeFunction("GridBindings::GetCellGap", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				Grid.get_cellGap_Injected(intPtr, out vector);
				return vector;
			}
			[FreeFunction("GridBindings::SetCellGap", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Grid.set_cellGap_Injected(intPtr, ref value);
			}
		}

		public new GridLayout.CellLayout cellLayout
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Grid.get_cellLayout_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Grid.set_cellLayout_Injected(intPtr, value);
			}
		}

		public new GridLayout.CellSwizzle cellSwizzle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Grid.get_cellSwizzle_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Grid>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Grid.set_cellSwizzle_Injected(intPtr, value);
			}
		}

		[FreeFunction("GridBindings::CellSwizzle")]
		public static Vector3 Swizzle(GridLayout.CellSwizzle swizzle, Vector3 position)
		{
			Vector3 vector;
			Grid.Swizzle_Injected(swizzle, ref position, out vector);
			return vector;
		}

		[FreeFunction("GridBindings::InverseCellSwizzle")]
		public static Vector3 InverseSwizzle(GridLayout.CellSwizzle swizzle, Vector3 position)
		{
			Vector3 vector;
			Grid.InverseSwizzle_Injected(swizzle, ref position, out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cellSize_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cellSize_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_cellGap_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cellGap_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GridLayout.CellLayout get_cellLayout_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cellLayout_Injected(IntPtr _unity_self, GridLayout.CellLayout value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GridLayout.CellSwizzle get_cellSwizzle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cellSwizzle_Injected(IntPtr _unity_self, GridLayout.CellSwizzle value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Swizzle_Injected(GridLayout.CellSwizzle swizzle, [In] ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseSwizzle_Injected(GridLayout.CellSwizzle swizzle, [In] ref Vector3 position, out Vector3 ret);
	}
}
