using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Light2D : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public float IntensityAnimation { get; set; }

	protected override void OnPrefabInit()
	{
		base.Subscribe<Light2D>(-592767678, Light2D.OnOperationalChangedDelegate);
		this.IntensityAnimation = 1f;
	}

	protected override void OnCmpEnable()
	{
		this.materialPropertyBlock = new MaterialPropertyBlock();
		base.OnCmpEnable();
		Components.Light2Ds.Add(this);
		if (base.isSpawned)
		{
			this.Refresh();
		}
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged), "Light2D.OnCmpEnable");
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Refresh();
	}

	protected override void OnCmpDisable()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChanged));
		Components.Light2Ds.Remove(this);
		base.OnCmpDisable();
		this.Refresh();
	}

	protected override void OnCleanUp()
	{
		this.UnregisterLight();
		GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.liquidPartitionerEntry);
	}

	private void OnCellChanged()
	{
		base.GetComponent<Light2D>().Refresh();
	}

	private void UnregisterLight()
	{
		if (this.isRegistered && Grid.IsValidCell(this.cell))
		{
			GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
			GameScenePartitioner.Instance.Free(ref this.liquidPartitionerEntry);
			this.isRegistered = false;
		}
		if (this.emitter != null)
		{
			this.emitter.Remove();
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
		Vector3 position = base.transform.GetPosition();
		position = new Vector3(position.x + this.Offset.x, position.y + this.Offset.y, position.z);
		int num = Grid.PosToCell(position);
		if (Grid.IsValidCell(num))
		{
			Vector2I vector2I = Grid.CellToXY(num);
			int num2 = (int)this.Range;
			if (this.shape == LightShape.Circle)
			{
				Vector2I vector2I2 = new Vector2I(vector2I.x - num2, vector2I.y - num2);
				this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I2.x, vector2I2.y, 2 * num2, 2 * num2, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.TriggerRefresh));
				this.liquidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I2.x, vector2I2.y, 2 * num2, 2 * num2, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.TriggerRefresh));
			}
			else if (this.shape == LightShape.Cone)
			{
				Vector2I vector2I3 = new Vector2I(vector2I.x - num2, vector2I.y - num2);
				this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I3.x, vector2I3.y, 2 * num2, num2, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.TriggerRefresh));
				this.liquidPartitionerEntry = GameScenePartitioner.Instance.Add("Light2D", base.gameObject, vector2I3.x, vector2I3.y, 2 * num2, num2, GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.TriggerRefresh));
			}
			this.cell = num;
			this.litCells.Clear();
			this.emitter = new LightGridManager.LightGridEmitter(this.cell, this.litCells, this.Lux, this.Range, this.Color, this.shape, 0.5f);
			this.emitter.Add();
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

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, this.Range), UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT, Descriptor.DescriptorType.Effect, false)
		};
	}

	public Color Color = Color.white;

	public float Range = 5f;

	public float Angle;

	public int Lux = 1000;

	public Vector2 Direction;

	public Vector2 Offset;

	public bool drawOverlay;

	public Color overlayColour;

	public LightShape shape;

	private int cell = Grid.InvalidCell;

	public MaterialPropertyBlock materialPropertyBlock;

	private bool isRegistered;

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle liquidPartitionerEntry;

	private LightGridManager.LightGridEmitter emitter;

	private List<int> litCells = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<Light2D> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Light2D>(delegate(Light2D component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
