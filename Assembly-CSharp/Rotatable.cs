using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Rotatable")]
public class Rotatable : KMonoBehaviour, ISaveLoadable
{
	public Orientation Orientation
	{
		get
		{
			return this.orientation;
		}
	}

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
		if (width % 2 == 0)
		{
			this.pivot = new Vector3(-0.5f, 0.5f, 0f);
			this.visualizerOffset = new Vector3(0.5f, 0f, 0f);
			return;
		}
		this.pivot = new Vector3(0f, 0.5f, 0f);
		this.visualizerOffset = Vector3.zero;
	}

	public Orientation Rotate()
	{
		switch (this.permittedRotations)
		{
		case PermittedRotations.R90:
			this.orientation = ((this.orientation == Orientation.Neutral) ? Orientation.R90 : Orientation.Neutral);
			break;
		case PermittedRotations.R360:
			this.orientation = (this.orientation + 1) % Orientation.NumRotations;
			break;
		case PermittedRotations.FlipH:
			this.orientation = ((this.orientation == Orientation.Neutral) ? Orientation.FlipH : Orientation.Neutral);
			break;
		case PermittedRotations.FlipV:
			this.orientation = ((this.orientation == Orientation.Neutral) ? Orientation.FlipV : Orientation.Neutral);
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
		if (permittedRotations - PermittedRotations.R90 <= 1)
		{
			return -90f * (float)this.orientation;
		}
		return 0f;
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
		KBoxCollider2D component = base.GetComponent<KBoxCollider2D>();
		if (component == null)
		{
			return;
		}
		float num = 0.5f * (float)((this.width + 1) % 2);
		float num2 = 0f;
		switch (orientation)
		{
		case Orientation.R90:
			num2 = -90f;
			goto IL_011B;
		case Orientation.R180:
			num2 = -180f;
			goto IL_011B;
		case Orientation.R270:
			num2 = -270f;
			goto IL_011B;
		case Orientation.FlipH:
			component.offset = new Vector2(num + (float)(this.width % 2) - 1f, 0.5f * (float)this.height);
			component.size = new Vector2((float)this.width, (float)this.height);
			goto IL_011B;
		case Orientation.FlipV:
			component.offset = new Vector2(num, -0.5f * (float)(this.height - 2));
			component.size = new Vector2((float)this.width, (float)this.height);
			goto IL_011B;
		}
		component.offset = new Vector2(num, 0.5f * (float)this.height);
		component.size = new Vector2((float)this.width, (float)this.height);
		IL_011B:
		if (num2 != 0f)
		{
			Matrix2x3 matrix2x = Matrix2x3.Translate(-this.pivot);
			Matrix2x3 matrix2x2 = Matrix2x3.Rotate(num2 * 0.017453292f);
			Matrix2x3 matrix2x3 = Matrix2x3.Translate(this.pivot + new Vector3(num, 0f, 0f)) * matrix2x2 * matrix2x;
			Vector2 vector = new Vector2(-0.5f * (float)this.width, 0f);
			Vector2 vector2 = new Vector2(0.5f * (float)this.width, (float)this.height);
			Vector2 vector3 = new Vector2(0f, 0.5f * (float)this.height);
			vector = matrix2x3.MultiplyPoint(vector);
			vector2 = matrix2x3.MultiplyPoint(vector2);
			vector3 = matrix2x3.MultiplyPoint(vector3);
			float num3 = Mathf.Min(vector.x, vector2.x);
			float num4 = Mathf.Max(vector.x, vector2.x);
			float num5 = Mathf.Min(vector.y, vector2.y);
			float num6 = Mathf.Max(vector.y, vector2.y);
			component.offset = vector3;
			component.size = new Vector2(num4 - num3, num6 - num5);
		}
	}

	public CellOffset GetRotatedCellOffset(CellOffset offset)
	{
		return Rotatable.GetRotatedCellOffset(offset, this.orientation);
	}

	public static CellOffset GetRotatedCellOffset(CellOffset offset, Orientation orientation)
	{
		switch (orientation)
		{
		default:
			return offset;
		case Orientation.R90:
			return new CellOffset(offset.y, -offset.x);
		case Orientation.R180:
			return new CellOffset(-offset.x, -offset.y);
		case Orientation.R270:
			return new CellOffset(-offset.y, offset.x);
		case Orientation.FlipH:
			return new CellOffset(-offset.x, offset.y);
		case Orientation.FlipV:
			return new CellOffset(offset.x, -offset.y);
		}
	}

	public static CellOffset GetRotatedCellOffset(int x, int y, Orientation orientation)
	{
		return Rotatable.GetRotatedCellOffset(new CellOffset(x, y), orientation);
	}

	public Vector3 GetRotatedOffset(Vector3 offset)
	{
		return Rotatable.GetRotatedOffset(offset, this.orientation);
	}

	public static Vector3 GetRotatedOffset(Vector3 offset, Orientation orientation)
	{
		switch (orientation)
		{
		default:
			return offset;
		case Orientation.R90:
			return new Vector3(offset.y, -offset.x);
		case Orientation.R180:
			return new Vector3(-offset.x, -offset.y);
		case Orientation.R270:
			return new Vector3(-offset.y, offset.x);
		case Orientation.FlipH:
			return new Vector3(-offset.x, offset.y);
		case Orientation.FlipV:
			return new Vector3(offset.x, -offset.y);
		}
	}

	public Orientation GetOrientation()
	{
		return this.orientation;
	}

	public bool IsRotated
	{
		get
		{
			return this.orientation > Orientation.Neutral;
		}
	}

	[MyCmpReq]
	private KBatchedAnimController batchedAnimController;

	[MyCmpGet]
	private Building building;

	[Serialize]
	[SerializeField]
	private Orientation orientation;

	[SerializeField]
	private Vector3 pivot = Vector3.zero;

	[SerializeField]
	private Vector3 visualizerOffset = Vector3.zero;

	public PermittedRotations permittedRotations;

	[SerializeField]
	private int width;

	[SerializeField]
	private int height;
}
