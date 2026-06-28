using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class NoisePolluter : KMonoBehaviour, IPolluter
{
	public static bool IsNoiseableCell(int cell)
	{
		return Grid.IsValidCell(cell) && (Grid.IsGas(cell) || !Grid.IsSubstantialLiquid(cell, 0.35f));
	}

	public void ResetCells()
	{
		if (this.radius == 0)
		{
			global::Debug.LogFormat("[{0}] has a 0 radius noise, this will disable it", new object[] { this.GetName() });
			return;
		}
		int num = (2 * this.radius + 5) * (2 * this.radius + 5);
		if (this.cells.Length < num)
		{
			this.cells = new Pair<int, int>[num];
		}
	}

	public void SetAttributes(Vector2 pos, int dB, string name)
	{
		this.sourceName = name;
		this.noise = dB;
	}

	public int GetRadius()
	{
		return this.radius;
	}

	public int GetNoise()
	{
		return this.noise;
	}

	public int GetCellCount()
	{
		return this.cellCount;
	}

	public void AddCell(Pair<int, int> cell)
	{
		for (int i = 0; i < this.cellCount; i++)
		{
		}
		this.cells[this.cellCount++] = cell;
	}

	public Pair<int, int> GetCell(int index)
	{
		return this.cells[index];
	}

	public void Clear()
	{
		this.cellCount = 0;
	}

	public Vector2 GetPosition()
	{
		return this.transform.position;
	}

	public string sourceName { get; private set; }

	public long noiseSplatID { get; private set; }

	public bool active { get; private set; }

	public void SetActive(bool active = true)
	{
		if (!active)
		{
			AudioEventManager.Get().ClearNoiseSplat(this.noiseSplatID);
		}
		this.active = active;
	}

	public void Refresh()
	{
		if (this.active)
		{
			AudioEventManager.Get().ClearNoiseSplat(this.noiseSplatID);
			this.noiseSplatID = AudioEventManager.Get().UpdateConstantNoiseSplat(this);
		}
	}

	private void OnActiveChanged(object data)
	{
		this.SetActive(this.operational.IsActive);
		this.Refresh();
	}

	public int GetNoisePollutionForCell(int cell)
	{
		for (int i = 0; i < this.cellCount; i++)
		{
			if (this.cells[i].first == cell)
			{
				return this.cells[i].second;
			}
		}
		return 0;
	}

	public void SetValues(EffectorValues values)
	{
		this.noise = values.amount;
		this.radius = values.radius;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.noiseSplatID = -1L;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.radius == 0 || this.noise == 0)
		{
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"Noisepollutor::OnSpawn [",
				this.GetName(),
				"] noise: [",
				this.noise,
				"] radius: [",
				this.radius,
				"]"
			}), null);
			global::UnityEngine.Object.Destroy(this);
			return;
		}
		this.ResetCells();
		if (this.operational != null)
		{
			this.Subscribe(824508782, new Action<object>(this.OnActiveChanged));
		}
		this.refreshCallback = new global::System.Action(this.Refresh);
		this.refreshPartionerCallback = delegate(object data)
		{
			this.Refresh();
		};
		this.onCollectNoisePollutersCallback = new Action<object>(this.OnCollectNoisePolluters);
		Attributes attributes = this.GetAttributes();
		Db db = Db.Get();
		this.dB = attributes.Add(db.BuildingAttributes.NoisePollution);
		this.dBRadius = attributes.Add(db.BuildingAttributes.NoisePollutionRadius);
		if (this.noise != 0 && this.radius != 0)
		{
			AttributeModifier attributeModifier = new AttributeModifier(db.BuildingAttributes.NoisePollution.Id, (float)this.noise, UI.TOOLTIPS.BASE_VALUE, false, false);
			AttributeModifier attributeModifier2 = new AttributeModifier(db.BuildingAttributes.NoisePollutionRadius.Id, (float)this.radius, UI.TOOLTIPS.BASE_VALUE, false, false);
			attributes.Add("Base", attributeModifier);
			attributes.Add("Base", attributeModifier2);
		}
		else
		{
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"Noisepollutor::OnSpawn [",
				this.GetName(),
				"] radius: [",
				this.radius,
				"] noise: [",
				this.noise,
				"]"
			}), null);
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.isMovable = component != null && component.isMovable;
		if (this.isMovable)
		{
			CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
		}
		AttributeInstance attributeInstance = this.dB;
		attributeInstance.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance.OnDirty, this.refreshCallback);
		AttributeInstance attributeInstance2 = this.dBRadius;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, this.refreshCallback);
		if (this.operational != null)
		{
			this.OnActiveChanged(null);
		}
	}

	private void OnCellChange(int previous_cell, int current_cell)
	{
		this.Refresh();
	}

	private void OnCollectNoisePolluters(object data)
	{
		List<NoisePolluter> list = (List<NoisePolluter>)data;
		list.Add(this);
	}

	public string GetName()
	{
		if (string.IsNullOrEmpty(this.sourceName))
		{
			this.sourceName = base.GetComponent<KSelectable>().GetName();
		}
		return this.sourceName;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			if (this.dB != null)
			{
				AttributeInstance attributeInstance = this.dB;
				attributeInstance.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance.OnDirty, this.refreshCallback);
				AttributeInstance attributeInstance2 = this.dBRadius;
				attributeInstance2.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance2.OnDirty, this.refreshCallback);
			}
			if (this.isMovable)
			{
				CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
			}
		}
		AudioEventManager.Get().ClearNoiseSplat(this.noiseSplatID);
	}

	public const string ID = "NoisePolluter";

	public int radius;

	public int noise;

	private Pair<int, int>[] cells = new Pair<int, int>[0];

	private int cellCount;

	public AttributeInstance dB;

	public AttributeInstance dBRadius;

	public global::System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectNoisePollutersCallback;

	public bool isMovable;

	[MyCmpReq]
	public OccupyArea occupyArea;

	[MyCmpGet]
	public SimCellOccupier simCellOccupier;

	[MyCmpGet]
	private Operational operational;
}
