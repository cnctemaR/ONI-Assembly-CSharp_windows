using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Base class for authoring data on a grid with grid painting tools like paint, erase, pick, select and fill.</para>
	/// </summary>
	public abstract class GridBrushBase : ScriptableObject
	{
		/// <summary>
		///   <para>Paints data into a grid within the given bounds.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
		/// <param name="position">The coordinates of the cell to paint data to.</param>
		/// <param name="gridLayout"></param>
		public virtual void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
		}

		/// <summary>
		///   <para>Erases data on a grid within the given bounds.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
		/// <param name="position">The coordinates of the cell to erase data from.</param>
		/// <param name="gridLayout"></param>
		public virtual void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
		}

		/// <summary>
		///   <para>Box fills tiles and GameObjects into given bounds within the selected layers.</para>
		/// </summary>
		/// <param name="gridLayout">Grid used for layout.</param>
		/// <param name="brushTarget">Target of box fill operation. By default the currently selected GameObject.</param>
		/// <param name="position">The bounds to box fill data to.</param>
		public virtual void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
			for (int i = position.zMin; i < position.zMax; i++)
			{
				for (int j = position.yMin; j < position.yMax; j++)
				{
					for (int k = position.xMin; k < position.xMax; k++)
					{
						this.Paint(gridLayout, brushTarget, new Vector3Int(k, j, i));
					}
				}
			}
		}

		/// <summary>
		///   <para>Erases data on a grid within the given bounds.</para>
		/// </summary>
		/// <param name="gridLayout">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
		/// <param name="position">The bounds to erase data from.</param>
		public virtual void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
			for (int i = position.zMin; i < position.zMax; i++)
			{
				for (int j = position.yMin; j < position.yMax; j++)
				{
					for (int k = position.xMin; k < position.xMax; k++)
					{
						this.Erase(gridLayout, brushTarget, new Vector3Int(k, j, i));
					}
				}
			}
		}

		/// <summary>
		///   <para>Select an area of a grid.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Targets of paint operation. By default the currently selected GameObject.</param>
		/// <param name="position">Area to get selected.</param>
		/// <param name="gridLayout"></param>
		public virtual void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
		}

		/// <summary>
		///   <para>Flood fills data onto a grid given the starting coordinates of the cell.</para>
		/// </summary>
		/// <param name="gridLayout">Grid used for layout.</param>
		/// <param name="brushTarget">Targets of flood fill operation. By default the currently selected GameObject.</param>
		/// <param name="position">Starting position of the flood fill.</param>
		public virtual void FloodFill(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
		{
		}

		public virtual void Rotate(GridBrushBase.RotationDirection direction, GridLayout.CellLayout layout)
		{
		}

		public virtual void Flip(GridBrushBase.FlipAxis flip, GridLayout.CellLayout layout)
		{
		}

		/// <summary>
		///   <para>Picks data from a grid given the coordinates of the cells.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
		/// <param name="position">The coordinates of the cells to paint data from.</param>
		/// <param name="pivot">Pivot of the picking brush.</param>
		/// <param name="gridLayout"></param>
		public virtual void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
		{
		}

		/// <summary>
		///   <para>Move is called when user moves the area previously selected with the selection marquee.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the move operation. By default the currently selected GameObject.</param>
		/// <param name="from">Source bounds of the move.</param>
		/// <param name="to">Target bounds of the move.</param>
		/// <param name="gridLayout"></param>
		public virtual void Move(GridLayout gridLayout, GameObject brushTarget, BoundsInt from, BoundsInt to)
		{
		}

		/// <summary>
		///   <para>MoveEnd is called when user starts moving the area previously selected with the selection marquee.</para>
		/// </summary>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="brushTarget">Target of the move operation. By default the currently selected GameObject.</param>
		/// <param name="position">Position where the move operation has started.</param>
		/// <param name="gridLayout"></param>
		public virtual void MoveStart(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
		}

		/// <summary>
		///   <para>MoveEnd is called when user has ended the move of the area previously selected with the selection marquee.</para>
		/// </summary>
		/// <param name="position">Layers affected by the move operation.</param>
		/// <param name="brushTarget">Target of the move operation. By default the currently selected GameObject.</param>
		/// <param name="grid">Grid used for layout.</param>
		/// <param name="gridLayout"></param>
		public virtual void MoveEnd(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
		{
		}

		/// <summary>
		///   <para>Tool mode for the GridBrushBase.</para>
		/// </summary>
		public enum Tool
		{
			/// <summary>
			///   <para>Select.</para>
			/// </summary>
			Select,
			/// <summary>
			///   <para>Move.</para>
			/// </summary>
			Move,
			/// <summary>
			///   <para>Paint.</para>
			/// </summary>
			Paint,
			/// <summary>
			///   <para>Box Fill.</para>
			/// </summary>
			Box,
			/// <summary>
			///   <para>Pick.</para>
			/// </summary>
			Pick,
			/// <summary>
			///   <para>Erase.</para>
			/// </summary>
			Erase,
			/// <summary>
			///   <para>Flood Fill.</para>
			/// </summary>
			FloodFill
		}

		/// <summary>
		///   <para>Direction to rotate tiles in the GridBrushBase by.</para>
		/// </summary>
		public enum RotationDirection
		{
			/// <summary>
			///   <para>Rotates tiles clockwise.</para>
			/// </summary>
			Clockwise,
			/// <summary>
			///   <para>Rotates tiles counter-clockwise.</para>
			/// </summary>
			CounterClockwise
		}

		/// <summary>
		///   <para>Axis to flip tiles in the GridBrushBase by.</para>
		/// </summary>
		public enum FlipAxis
		{
			/// <summary>
			///   <para>Flip the brush in the X Axis.</para>
			/// </summary>
			X,
			/// <summary>
			///   <para>Flip the brush in the Y Axis.</para>
			/// </summary>
			Y
		}
	}
}
