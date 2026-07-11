using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>An abstract class that defines a grid layout.</para>
	/// </summary>
	[NativeType(Header = "Modules/Grid/Public/Grid.h")]
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Transform))]
	public class GridLayout : Behaviour
	{
		/// <summary>
		///   <para>The size of each cell in the layout.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The size of the gap between each cell in the layout.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The layout of the cells.</para>
		/// </summary>
		public extern GridLayout.CellLayout cellLayout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The cell swizzle for the layout.</para>
		/// </summary>
		public extern GridLayout.CellSwizzle cellSwizzle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the local bounds for a cell at the location.</para>
		/// </summary>
		/// <param name="localPosition">Location of the cell.</param>
		/// <param name="cellPosition"></param>
		/// <returns>
		///   <para>Local bounds of cell at the position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::GetBoundsLocal", HasExplicitThis = true)]
		public Bounds GetBoundsLocal(Vector3Int cellPosition)
		{
			Bounds bounds;
			this.GetBoundsLocal_Injected(ref cellPosition, out bounds);
			return bounds;
		}

		/// <summary>
		///   <para>Converts a cell position to local position space.</para>
		/// </summary>
		/// <param name="cellPosition">Cell position to convert.</param>
		/// <returns>
		///   <para>Local position of the cell position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::CellToLocal", HasExplicitThis = true)]
		public Vector3 CellToLocal(Vector3Int cellPosition)
		{
			Vector3 vector;
			this.CellToLocal_Injected(ref cellPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Converts a local position to cell position.</para>
		/// </summary>
		/// <param name="localPosition">Local Position to convert.</param>
		/// <returns>
		///   <para>Cell position of the local position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::LocalToCell", HasExplicitThis = true)]
		public Vector3Int LocalToCell(Vector3 localPosition)
		{
			Vector3Int vector3Int;
			this.LocalToCell_Injected(ref localPosition, out vector3Int);
			return vector3Int;
		}

		/// <summary>
		///   <para>Converts an interpolated cell position in floats to local position space.</para>
		/// </summary>
		/// <param name="cellPosition">Interpolated cell position to convert.</param>
		/// <returns>
		///   <para>Local position of the cell position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::CellToLocalInterpolated", HasExplicitThis = true)]
		public Vector3 CellToLocalInterpolated(Vector3 cellPosition)
		{
			Vector3 vector;
			this.CellToLocalInterpolated_Injected(ref cellPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Converts a local position to cell position.</para>
		/// </summary>
		/// <param name="localPosition">Local Position to convert.</param>
		/// <returns>
		///   <para>Interpolated cell position of the local position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::LocalToCellInterpolated", HasExplicitThis = true)]
		public Vector3 LocalToCellInterpolated(Vector3 localPosition)
		{
			Vector3 vector;
			this.LocalToCellInterpolated_Injected(ref localPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Converts a cell position to world position space.</para>
		/// </summary>
		/// <param name="cellPosition">Cell position to convert.</param>
		/// <returns>
		///   <para>World position of the cell position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::CellToWorld", HasExplicitThis = true)]
		public Vector3 CellToWorld(Vector3Int cellPosition)
		{
			Vector3 vector;
			this.CellToWorld_Injected(ref cellPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Converts a world position to cell position.</para>
		/// </summary>
		/// <param name="worldPosition">World Position to convert.</param>
		/// <returns>
		///   <para>Cell position of the world position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::WorldToCell", HasExplicitThis = true)]
		public Vector3Int WorldToCell(Vector3 worldPosition)
		{
			Vector3Int vector3Int;
			this.WorldToCell_Injected(ref worldPosition, out vector3Int);
			return vector3Int;
		}

		/// <summary>
		///   <para>Converts a local position to world position.</para>
		/// </summary>
		/// <param name="localPosition">Local Position to convert.</param>
		/// <returns>
		///   <para>World position of the local position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::LocalToWorld", HasExplicitThis = true)]
		public Vector3 LocalToWorld(Vector3 localPosition)
		{
			Vector3 vector;
			this.LocalToWorld_Injected(ref localPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Converts a world position to local position.</para>
		/// </summary>
		/// <param name="worldPosition">World Position to convert.</param>
		/// <returns>
		///   <para>Local position of the world position.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::WorldToLocal", HasExplicitThis = true)]
		public Vector3 WorldToLocal(Vector3 worldPosition)
		{
			Vector3 vector;
			this.WorldToLocal_Injected(ref worldPosition, out vector);
			return vector;
		}

		/// <summary>
		///   <para>Get the default center coordinate of a cell for the set layout of the Grid.</para>
		/// </summary>
		/// <returns>
		///   <para>Cell Center coordinate.</para>
		/// </returns>
		[FreeFunction("GridLayoutBindings::GetLayoutCellCenter", HasExplicitThis = true)]
		public Vector3 GetLayoutCellCenter()
		{
			Vector3 vector;
			this.GetLayoutCellCenter_Injected(out vector);
			return vector;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cellSize_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cellGap_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetBoundsLocal_Injected(ref Vector3Int cellPosition, out Bounds ret);

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

		/// <summary>
		///   <para>The layout of the GridLayout.</para>
		/// </summary>
		public enum CellLayout
		{
			/// <summary>
			///   <para>Rectangular layout for cells in the GridLayout.</para>
			/// </summary>
			Rectangle,
			/// <summary>
			///   <para>Hexagonal layout for cells in the GridLayout.</para>
			/// </summary>
			Hexagon
		}

		/// <summary>
		///   <para>Swizzles cell positions to other positions.</para>
		/// </summary>
		public enum CellSwizzle
		{
			/// <summary>
			///   <para>Keeps the cell positions at XYZ.</para>
			/// </summary>
			XYZ,
			/// <summary>
			///   <para>Swizzles the cell positions from XYZ to XZY.</para>
			/// </summary>
			XZY,
			/// <summary>
			///   <para>Swizzles the cell positions from XYZ to YXZ.</para>
			/// </summary>
			YXZ,
			/// <summary>
			///   <para>Swizzles the cell positions from XYZ to YZX.</para>
			/// </summary>
			YZX,
			/// <summary>
			///   <para>Swizzles the cell positions from XYZ to ZXY.</para>
			/// </summary>
			ZXY,
			/// <summary>
			///   <para>Swizzles the cell positions from XYZ to ZYX.</para>
			/// </summary>
			ZYX
		}
	}
}
