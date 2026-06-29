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
		if (permittedRotations != PermittedRotations.R360 && permittedRotations != PermittedRotations.R90)
		{
			return 0f;
		}
		return -90f * (float)this.orientation;
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
		float num = 0f;
		switch (orientation)
		{
		case Orientation.R90:
			num = -90f;
			goto IL_011B;
		case Orientation.R180:
			num = -180f;
			goto IL_011B;
		case Orientation.R270:
			num = -270f;
			goto IL_011B;
		case Orientation.FlipH:
			component.offset = new Vector2(-0.5f * (float)this.width, 0.5f * (float)this.height);
			component.size = new Vector2((float)this.width, (float)this.height);
			goto IL_011B;
		case Orientation.FlipV:
			component.offset = new Vector2(0f, -0.5f * (float)(this.height - 2));
			component.size = new Vector2((float)this.width, (float)this.height);
			goto IL_011B;
		}
		component.offset = new Vector2(0f, 0.5f * (float)this.height);
		component.size = new Vector2((float)this.width, (float)this.height);
		IL_011B:
		if (num != 0f)
		{
			Matrix2x3 matrix2x = Matrix2x3.Translate(-this.pivot);
			Matrix2x3 matrix2x2 = Matrix2x3.Rotate(num * 0.017453292f);
			Matrix2x3 matrix2x3 = Matrix2x3.Translate(this.pivot);
			Matrix2x3 matrix2x4 = matrix2x3 * matrix2x2 * matrix2x;
			Vector2 vector = new Vector2(-0.5f * (float)this.width, 0f);
			Vector2 vector2 = new Vector2(0.5f * (float)this.width, (float)this.height);
			Vector2 vector3 = new Vector2(0f, 0.5f * (float)this.height);
			vector = matrix2x4.MultiplyPoint(vector);
			vector2 = matrix2x4.MultiplyPoint(vector2);
			vector3 = matrix2x4.MultiplyPoint(vector3);
			float num2 = Mathf.Min(vector.x, vector2.x);
			float num3 = Mathf.Max(vector.x, vector2.x);
			float num4 = Mathf.Min(vector.y, vector2.y);
			float num5 = Mathf.Max(vector.y, vector2.y);
			component.offset = vector3;
			component.size = new Vector2(num3 - num2, num5 - num4);
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
			return this.orientation != Orientation.Neutral;
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
