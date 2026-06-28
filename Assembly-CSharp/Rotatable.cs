using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Rotatable : KMonoBehaviour, ISaveLoadable
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.building != null)
		{
			BuildingDef def = base.GetComponent<Building>().Def;
			this.SetSize(def.WidthInCells, def.HeightInCells);
		}
		this.OrientVisualizer(this.orientation);
		this.OrientCollider(this.orientation);
	}

	public void SetSize(int width, int height)
	{
		this.width = width;
		this.height = height;
		bool flag = width % 2 == 0;
		if (flag)
		{
			this.pivot = new Vector3(-0.5f, 0.5f, 0f);
			this.visualizerOffset = new Vector3(0.5f, 0f, 0f);
		}
		else
		{
			this.pivot = new Vector3(0f, 0.5f, 0f);
			this.visualizerOffset = Vector3.zero;
		}
	}

	public Orientation Rotate()
	{
		switch (this.permittedRotations)
		{
		case PermittedRotations.R90:
			this.orientation = ((this.orientation != Orientation.Neutral) ? Orientation.Neutral : Orientation.R90);
			break;
		case PermittedRotations.R360:
			this.orientation = (this.orientation + 1) % Orientation.NumRotations;
			break;
		case PermittedRotations.FlipH:
			this.orientation = ((this.orientation != Orientation.Neutral) ? Orientation.Neutral : Orientation.FlipH);
			break;
		case PermittedRotations.FlipV:
			this.orientation = ((this.orientation != Orientation.Neutral) ? Orientation.Neutral : Orientation.FlipV);
			break;
		}
		this.OrientVisualizer(this.orientation);
		return this.orientation;
	}

	public void SetOrientation(Orientation new_orientation)
	{
		this.orientation = new_orientation;
		this.OrientVisualizer(new_orientation);
		this.OrientCollider(new_orientation);
	}

	public void Match(Rotatable other)
	{
		this.pivot = other.pivot;
		this.visualizerOffset = other.visualizerOffset;
		this.permittedRotations = other.permittedRotations;
		this.orientation = other.orientation;
		this.OrientVisualizer(this.orientation);
		this.OrientCollider(this.orientation);
	}

	public float GetVisualizerRotation()
	{
		PermittedRotations permittedRotations = this.permittedRotations;
		float num;
		if (permittedRotations != PermittedRotations.R360 && permittedRotations != PermittedRotations.R90)
		{
			num = 0f;
		}
		else
		{
			num = -90f * (float)this.orientation;
		}
		return num;
	}

	public bool GetVisualizerFlipX()
	{
		return this.orientation == Orientation.FlipH;
	}

	public bool GetVisualizerFlipY()
	{
		return this.orientation == Orientation.FlipV;
	}

	public Vector3 GetVisualizerPivot()
	{
		Vector3 vector = this.pivot;
		Orientation orientation = this.orientation;
		if (orientation != Orientation.FlipH)
		{
			if (orientation != Orientation.FlipV)
			{
			}
		}
		else
		{
			vector.x = -this.pivot.x;
		}
		return vector;
	}

	private Vector3 GetVisualizerOffset()
	{
		Orientation orientation = this.orientation;
		Vector3 vector;
		if (orientation != Orientation.FlipH)
		{
			if (orientation != Orientation.FlipV)
			{
				vector = this.visualizerOffset;
			}
			else
			{
				vector = new Vector3(this.visualizerOffset.x, 1f, this.visualizerOffset.z);
			}
		}
		else
		{
			vector = new Vector3(-this.visualizerOffset.x, this.visualizerOffset.y, this.visualizerOffset.z);
		}
		return vector;
	}

	private void OrientVisualizer(Orientation orientation)
	{
		float visualizerRotation = this.GetVisualizerRotation();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Pivot = this.GetVisualizerPivot();
		component.Rotation = visualizerRotation;
		component.Offset = this.GetVisualizerOffset();
		component.FlipX = this.GetVisualizerFlipX();
		component.FlipY = this.GetVisualizerFlipY();
		base.Trigger(-1643076535, this);
	}

	private void OrientCollider(Orientation orientation)
	{
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (!(component == null))
		{
			switch (orientation)
			{
			default:
				component.offset = new Vector2(0f, 0.5f * (float)this.height);
				component.size = new Vector2((float)this.width, (float)this.height);
				break;
			case Orientation.R90:
				component.offset = new Vector2(0.5f * (float)(this.height - 1), 0.5f);
				component.size = new Vector2((float)this.height, (float)this.width);
				break;
			case Orientation.R180:
				component.offset = new Vector2(0f, -0.5f * (float)(this.height - 2));
				component.size = new Vector2((float)this.width, (float)this.height);
				break;
			case Orientation.R270:
				component.offset = new Vector2(-0.5f * (float)(this.height - 1), 0.5f);
				component.size = new Vector2((float)this.height, (float)this.width);
				break;
			case Orientation.FlipH:
				component.offset = new Vector2(0f, 0.5f * (float)this.height);
				component.size = new Vector2((float)this.width, (float)this.height);
				break;
			case Orientation.FlipV:
				component.offset = new Vector2(0f, -0.5f * (float)(this.height - 2));
				component.size = new Vector2((float)this.width, (float)this.height);
				break;
			}
		}
	}

	public CellOffset GetRotatedCellOffset(CellOffset offset)
	{
		return Rotatable.GetRotatedCellOffset(offset, this.orientation);
	}

	public static CellOffset GetRotatedCellOffset(CellOffset offset, Orientation orientation)
	{
		CellOffset cellOffset;
		switch (orientation)
		{
		default:
			cellOffset = offset;
			break;
		case Orientation.R90:
			cellOffset = new CellOffset(offset.y, -offset.x);
			break;
		case Orientation.R180:
			cellOffset = new CellOffset(-offset.x, -offset.y);
			break;
		case Orientation.R270:
			cellOffset = new CellOffset(-offset.y, offset.x);
			break;
		case Orientation.FlipH:
			cellOffset = new CellOffset(-offset.x, offset.y);
			break;
		case Orientation.FlipV:
			cellOffset = new CellOffset(offset.x, -offset.y);
			break;
		}
		return cellOffset;
	}

	public Vector3 GetRotatedOffset(Vector3 offset)
	{
		return Rotatable.GetRotatedOffset(offset, this.orientation);
	}

	public static Vector3 GetRotatedOffset(Vector3 offset, Orientation orientation)
	{
		Vector3 vector;
		switch (orientation)
		{
		default:
			vector = offset;
			break;
		case Orientation.R90:
			vector = new Vector3(offset.y, -offset.x);
			break;
		case Orientation.R180:
			vector = new Vector3(-offset.x, -offset.y);
			break;
		case Orientation.R270:
			vector = new Vector3(-offset.y, offset.x);
			break;
		case Orientation.FlipH:
			vector = new Vector3(-offset.x, offset.y);
			break;
		case Orientation.FlipV:
			vector = new Vector3(offset.x, -offset.y);
			break;
		}
		return vector;
	}

	public Orientation GetOrientation()
	{
		return this.orientation;
	}

	public bool IsRotated
	{
		get
		{
			return this.orientation != Orientation.Neutral;
		}
	}

	[MyCmpReq]
	private KBatchedAnimController batchedAnimController;

	[MyCmpGet]
	private Building building;

	[Serialize]
	private Orientation orientation = Orientation.Neutral;

	[SerializeField]
	private Vector3 pivot = Vector3.zero;

	[SerializeField]
	private Vector3 visualizerOffset = Vector3.zero;

	public PermittedRotations permittedRotations = PermittedRotations.Unrotatable;

	[SerializeField]
	private int width;

	[SerializeField]
	private int height;
}
