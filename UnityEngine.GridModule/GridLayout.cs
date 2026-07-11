using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType(Header = "Modules/Grid/Public/Grid.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Transform))]
	public class GridLayout : Behaviour
	{
		public Vector3 cellSize
		{
			[FreeFunction("GridLayoutBindings::GetCellSize", HasExplicitThis = true)]
			get
			{
				Vector3 vector;
				this.get_cellSize_Injected(out vector);
				return vector;
			}
		}

		public Vector3 cellGap
		{
			[FreeFunction("GridLayoutBindings::GetCellGap", HasExplicitThis = true)]
			get
			{
				Vector3 vector;
				this.get_cellGap_Injected(out vector);
				return vector;
			}
		}

		public extern GridLayout.CellLayout cellLayout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern GridLayout.CellSwizzle cellSwizzle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("GridLayoutBindings::GetBoundsLocal", HasExplicitThis = true)]
		public Bounds GetBoundsLocal(Vector3Int cellPosition)
		{
			Bounds bounds;
			this.GetBoundsLocal_Injected(ref cellPosition, out bounds);
			return bounds;
		}

		public Bounds GetBoundsLocal(Vector3 origin, Vector3 size)
		{
			return this.GetBoundsLocalOriginSize(origin, size);
		}

		[FreeFunction("GridLayoutBindings::GetBoundsLocalOriginSize", HasExplicitThis = true)]
		private Bounds GetBoundsLocalOriginSize(Vector3 origin, Vector3 size)
		{
			Bounds bounds;
			this.GetBoundsLocalOriginSize_Injected(ref origin, ref size, out bounds);
			return bounds;
		}

		[FreeFunction("GridLayoutBindings::CellToLocal", HasExplicitThis = true)]
		public Vector3 CellToLocal(Vector3Int cellPosition)
		{
			Vector3 vector;
			this.CellToLocal_Injected(ref cellPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::LocalToCell", HasExplicitThis = true)]
		public Vector3Int LocalToCell(Vector3 localPosition)
		{
			Vector3Int vector3Int;
			this.LocalToCell_Injected(ref localPosition, out vector3Int);
			return vector3Int;
		}

		[FreeFunction("GridLayoutBindings::CellToLocalInterpolated", HasExplicitThis = true)]
		public Vector3 CellToLocalInterpolated(Vector3 cellPosition)
		{
			Vector3 vector;
			this.CellToLocalInterpolated_Injected(ref cellPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::LocalToCellInterpolated", HasExplicitThis = true)]
		public Vector3 LocalToCellInterpolated(Vector3 localPosition)
		{
			Vector3 vector;
			this.LocalToCellInterpolated_Injected(ref localPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::CellToWorld", HasExplicitThis = true)]
		public Vector3 CellToWorld(Vector3Int cellPosition)
		{
			Vector3 vector;
			this.CellToWorld_Injected(ref cellPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::WorldToCell", HasExplicitThis = true)]
		public Vector3Int WorldToCell(Vector3 worldPosition)
		{
			Vector3Int vector3Int;
			this.WorldToCell_Injected(ref worldPosition, out vector3Int);
			return vector3Int;
		}

		[FreeFunction("GridLayoutBindings::LocalToWorld", HasExplicitThis = true)]
		public Vector3 LocalToWorld(Vector3 localPosition)
		{
			Vector3 vector;
			this.LocalToWorld_Injected(ref localPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::WorldToLocal", HasExplicitThis = true)]
		public Vector3 WorldToLocal(Vector3 worldPosition)
		{
			Vector3 vector;
			this.WorldToLocal_Injected(ref worldPosition, out vector);
			return vector;
		}

		[FreeFunction("GridLayoutBindings::GetLayoutCellCenter", HasExplicitThis = true)]
		public Vector3 GetLayoutCellCenter()
		{
			Vector3 vector;
			this.GetLayoutCellCenter_Injected(out vector);
			return vector;
		}

		[RequiredByNativeCode]
		private void DoNothing()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cellSize_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cellGap_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetBoundsLocal_Injected(ref Vector3Int cellPosition, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetBoundsLocalOriginSize_Injected(ref Vector3 origin, ref Vector3 size, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CellToLocal_Injected(ref Vector3Int cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void LocalToCell_Injected(ref Vector3 localPosition, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CellToLocalInterpolated_Injected(ref Vector3 cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void LocalToCellInterpolated_Injected(ref Vector3 localPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CellToWorld_Injected(ref Vector3Int cellPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WorldToCell_Injected(ref Vector3 worldPosition, out Vector3Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void LocalToWorld_Injected(ref Vector3 localPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WorldToLocal_Injected(ref Vector3 worldPosition, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetLayoutCellCenter_Injected(out Vector3 ret);

		public enum CellLayout
		{
			Rectangle,
			Hexagon,
			Isometric,
			IsometricZAsY
		}

		public enum CellSwizzle
		{
			XYZ,
			XZY,
			YXZ,
			YZX,
			ZXY,
			ZYX
		}
	}
}
