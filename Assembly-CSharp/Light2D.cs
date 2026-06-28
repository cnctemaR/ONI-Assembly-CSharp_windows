using System;
using UnityEngine;

public class Light2D : KMonoBehaviour
{
	public float IntensityAnimation { get; set; }

	protected override void OnPrefabInit()
	{
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.IntensityAnimation = 1f;
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Components.Light2Ds.Add(this);
		if (base.isSpawned)
		{
			this.Refresh();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Refresh();
	}

	protected override void OnCmpDisable()
	{
		Components.Light2Ds.Remove(this);
		base.OnCmpDisable();
		this.Refresh();
	}

	protected override void OnCleanUp()
	{
		this.UnregisterLight();
	}

	private void UnregisterLight()
	{
		if (this.isRegistered && Grid.IsValidCell(this.cell))
		{
			if (this.partitionerEntry != null)
			{
				this.partitionerEntry.Release();
			}
			LightGridManager.RemoveFromLightGrid(this.cell);
			this.isRegistered = false;
		}
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		this.UnregisterLight();
		Operational component = base.GetComponent<Operational>();
		if ((component != null && !component.IsOperational) || !base.isActiveAndEnabled)
		{
			return;
		}
		Vector3 position = this.transform.position;
		position = new Vector3(position.x + this.Offset.x, position.y + this.Offset.y, position.z);
		int num = Grid.PosToCell(position);
		if (Grid.IsValidCell(num))
		{
			Vector2I vector2I = Grid.CellToXY(num);
			int num2 = (int)this.Range;
			int num3 = num2 / 2;
			if (this.shape == LightShape.Circle)
			{
				Vector2I vector2I2 = new Vector2I(vector2I.x - num3, vector2I.y - num3);
				this.partitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I2.x, vector2I2.y, num2, num2, GameScenePartitioner.Instance.solidChangedMask.mask | GameScenePartitioner.Instance.liquidChangedMask.mask, new Action<object>(this.TriggerRefresh));
			}
			else if (this.shape == LightShape.Cone)
			{
				Vector2I vector2I3 = new Vector2I(vector2I.x - num2, vector2I.y - num2);
				this.partitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I3.x, vector2I3.y, 2 * num2, num2, GameScenePartitioner.Instance.solidChangedMask.mask | GameScenePartitioner.Instance.liquidChangedMask.mask, new Action<object>(this.TriggerRefresh));
			}
			else
			{
				global::UnityEngine.Debug.Assert(false);
			}
			this.cell = num;
			LightGridManager.AddToLightGrid(this.cell, num2, this.Range, this.Color, this.shape);
			this.isRegistered = true;
		}
	}

	private void TriggerRefresh(object data)
	{
		this.Refresh();
	}

	private void OnOperationalChanged(object data)
	{
		base.enabled = base.GetComponent<Operational>().IsOperational;
		this.Refresh();
	}

	public Color Color = Color.white;

	public float Intensity = 5f;

	public float Range = 5f;

	public float Angle;

	public Vector2 Direction;

	public Vector2 Offset;

	public bool drawOverlay;

	public Color overlayColour;

	public LightShape shape;

	private int cell = Grid.InvalidCell;

	private bool isRegistered;

	private GameScenePartitionerEntry partitionerEntry;
}
