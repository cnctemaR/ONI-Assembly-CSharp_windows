using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Grid is the base class for plotting a layout of uniformly spaced points and lines.</para>
	/// </summary>
	[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
	[RequireComponent(typeof(Transform))]
	[NativeType(Header = "Modules/Grid/Public/Grid.h")]
	public sealed class Grid : GridLayout
	{
		/// <summary>
		///   <para>Get the logical center coordinate of a grid cell in local space.</para>
		/// </summary>
		/// <param name="position">Grid cell position.</param>
		/// <returns>
		///   <para>Center of the cell transformed into local space coordinates.</para>
		/// </returns>
		public Vector3 GetCellCenterLocal(Vector3Int position)
		{
			return base.CellToLocalInterpolated(position + base.GetLayoutCellCenter());
		}

		/// <summary>
		///   <para>Get the logical center coordinate of a grid cell in world space.</para>
		/// </summary>
		/// <param name="position">Grid cell position.</param>
		/// <returns>
		///   <para>Center of the cell transformed into world space coordinates.</para>
		/// </returns>
		public Vector3 GetCellCenterWorld(Vector3Int position)
		{
			return base.LocalToWorld(base.CellToLocalInterpolated(position + base.GetLayoutCellCenter()));
		}

		/// <summary>
		///   <para>The size of each cell in the Grid.</para>
		/// </summary>
		public new Vector3 cellSize
		{
			[FreeFunction("GridBindings::GetCellSize", HasExplicitThis = true)]
			get
			{
				Vector3 vector;
				this.get_cellSize_Injected(out vector);
				return vector;
			}
			[FreeFunction("GridBindings::SetCellSize", HasExplicitThis = true)]
			set
			{
				this.set_cellSize_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The size of the gap between each cell in the Grid.</para>
		/// </summary>
		public new Vector3 cellGap
		{
			[FreeFunction("GridBindings::GetCellGap", HasExplicitThis = true)]
			get
			{
				Vector3 vector;
				this.get_cellGap_Injected(out vector);
				return vector;
			}
			[FreeFunction("GridBindings::SetCellGap", HasExplicitThis = true)]
			set
			{
				this.set_cellGap_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The layout of the cells in the Grid.</para>
		/// </summary>
		public new extern GridLayout.CellLayout cellLayout
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The cell swizzle for the Grid.</para>
		/// </summary>
		public new extern GridLayout.CellSwizzle cellSwizzle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
		private extern void get_cellSize_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_cellSize_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_cellGap_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_cellGap_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Swizzle_Injected(GridLayout.CellSwizzle swizzle, ref Vector3 position, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InverseSwizzle_Injected(GridLayout.CellSwizzle swizzle, ref Vector3 position, out Vector3 ret);
	}
}
