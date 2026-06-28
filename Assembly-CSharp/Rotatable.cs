using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Rotatable : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		BuildingDef def = base.GetComponent<Building>().Def;
		int widthInCells = def.WidthInCells;
		this.pivot = new Vector3((float)((widthInCells + 1) % 2) * 0.5f, 0.5f, 0f);
		this.OrientVisualizer(this.orientation);
	}

	public Orientation Rotate()
	{
		Rotatable.PermittedRotations permittedRotations = this.permittedRotations;
		if (permittedRotations != Rotatable.PermittedRotations.R90)
		{
			if (permittedRotations == Rotatable.PermittedRotations.R360)
			{
				this.orientation = (this.orientation + 1) % Orientation.Num;
			}
		}
		else
		{
			this.orientation = ((this.orientation != Orientation.Up) ? Orientation.Up : Orientation.Right);
		}
		this.OrientVisualizer(this.orientation);
		return this.orientation;
	}

	public void SetOrientation(Orientation new_orientation)
	{
		this.orientation = new_orientation;
		this.OrientVisualizer(new_orientation);
	}

	private Quaternion GetRotation(Orientation orientation)
	{
		float visualizerRotation = this.GetVisualizerRotation();
		return Quaternion.Euler(0f, 0f, visualizerRotation);
	}

	public float GetVisualizerRotation()
	{
		return -90f * (float)this.orientation;
	}

	public Vector3 GetVisualizerPivot()
	{
		return this.pivot;
	}

	private void OrientVisualizer(Orientation orientation)
	{
		float visualizerRotation = this.GetVisualizerRotation();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Offset = base.GetComponent<Building>().Def.GetVisualizerOffset();
		component.Rotation = visualizerRotation;
		component.Pivot = this.GetVisualizerPivot();
		this.Trigger(-1643076535, this);
	}

	public CellOffset GetCellOffset()
	{
		return Rotatable.orientationOffsets[(int)this.orientation];
	}

	public static CellOffset GetCellOffset(Orientation orientation)
	{
		return Rotatable.orientationOffsets[(int)orientation];
	}

	public CellOffset GetRotatedCellOffset(Orientation direction)
	{
		if (direction != Orientation.None)
		{
			int num = (int)((this.orientation + (int)direction) % Orientation.Num);
			return Rotatable.orientationOffsets[num];
		}
		return CellOffset.none;
	}

	public CellOffset GetRotatedCellOffset(CellOffset offset)
	{
		return Rotatable.GetRotatedCellOffset(offset, this.orientation);
	}

	public static CellOffset GetRotatedCellOffset(CellOffset offset, Orientation orientation)
	{
		switch (orientation)
		{
		case Orientation.Right:
			offset = new CellOffset(offset.y, -offset.x);
			break;
		case Orientation.Down:
			offset = new CellOffset(-offset.x, -offset.y);
			break;
		case Orientation.Left:
			offset = new CellOffset(-offset.y, offset.x);
			break;
		}
		return offset;
	}

	public Orientation GetOrientation()
	{
		return this.orientation;
	}

	public Orientation GetRotatedOrientation(Orientation o)
	{
		return (o + (int)this.orientation) % Orientation.Num;
	}

	public bool IsRotated
	{
		get
		{
			return this.orientation != Orientation.Up;
		}
	}

	[Serialize]
	private Orientation orientation;

	[SerializeField]
	private Vector3 pivot = Vector3.zero;

	public Rotatable.PermittedRotations permittedRotations = Rotatable.PermittedRotations.R360;

	public string orientationText = "Orientation";

	private static readonly CellOffset[] orientationOffsets = new CellOffset[]
	{
		new CellOffset(0, 1),
		new CellOffset(1, 0),
		new CellOffset(0, -1),
		new CellOffset(-1, 0)
	};

	public enum PermittedRotations
	{
		Unrotatable,
		R90,
		R360
	}
}
