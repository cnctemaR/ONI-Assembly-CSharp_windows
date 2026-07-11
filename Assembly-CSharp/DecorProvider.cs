using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class DecorProvider : KMonoBehaviour, IEffectDescriptor, IGameObjectEffectDescriptor
{
	public void Refresh()
	{
		this.splat.Clear();
		this.splat = new DecorProvider.Splat(this);
		KPrefabID component = base.GetComponent<KPrefabID>();
		bool flag = component.HasTag(RoomConstraints.ConstraintTags.Decor20);
		bool flag2 = this.decor.GetTotalValue() >= 20f;
		if (flag != flag2)
		{
			if (flag2)
			{
				component.AddTag(RoomConstraints.ConstraintTags.Decor20);
			}
			else
			{
				component.RemoveTag(RoomConstraints.ConstraintTags.Decor20);
			}
			Game.Instance.roomProber.SolidChangedEvent(Grid.PosToCell(this), true);
		}
	}

	public float GetDecorForCell(int cell)
	{
		for (int i = 0; i < this.cellCount; i++)
		{
			if (this.cells[i] == cell)
			{
				return this.splat.decor;
			}
		}
		return 0f;
	}

	public void SetValues(EffectorValues values)
	{
		this.baseDecor = (float)values.amount;
		this.baseRadius = (float)values.radius;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.decor = this.GetAttributes().Add(Db.Get().BuildingAttributes.Decor);
		this.decorRadius = this.GetAttributes().Add(Db.Get().BuildingAttributes.DecorRadius);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.refreshCallback = new global::System.Action(this.Refresh);
		this.refreshPartionerCallback = delegate(object data)
		{
			this.Refresh();
		};
		this.onCollectDecorProvidersCallback = new Action<object>(this.OnCollectDecorProviders);
		if (this.baseDecor != 0f)
		{
			AttributeModifier attributeModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, this.baseDecor, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			AttributeModifier attributeModifier2 = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, this.baseRadius, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			this.GetAttributes().Add(attributeModifier);
			this.GetAttributes().Add(attributeModifier2);
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.isMovable = component != null && component.isMovable;
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "DecorProvider.OnSpawn");
		AttributeInstance attributeInstance = this.decor;
		attributeInstance.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance.OnDirty, this.refreshCallback);
		AttributeInstance attributeInstance2 = this.decorRadius;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, this.refreshCallback);
		this.Refresh();
	}

	private void OnCellChange()
	{
		this.Refresh();
	}

	private void OnCollectDecorProviders(object data)
	{
		List<DecorProvider> list = (List<DecorProvider>)data;
		list.Add(this);
	}

	public string GetName()
	{
		if (string.IsNullOrEmpty(this.overrideName))
		{
			return base.GetComponent<KSelectable>().GetName();
		}
		return this.overrideName;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			AttributeInstance attributeInstance = this.decor;
			attributeInstance.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance.OnDirty, this.refreshCallback);
			AttributeInstance attributeInstance2 = this.decorRadius;
			attributeInstance2.OnDirty = (global::System.Action)Delegate.Remove(attributeInstance2.OnDirty, this.refreshCallback);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		}
		this.splat.Clear();
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.decor != null && this.decorRadius != null)
		{
			float totalValue = this.decor.GetTotalValue();
			float totalValue2 = this.decorRadius.GetTotalValue();
			string text = ((this.baseDecor <= 0f) ? "consumed" : "produced");
			string text2 = ((this.baseDecor <= 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED);
			text2 = text2 + "\n\n" + this.decor.GetAttributeValueTooltip();
			string text3 = GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f);
			Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, text, text3, totalValue2), string.Format(text2, text3, totalValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
		}
		else if (this.baseDecor != 0f)
		{
			string text4 = ((this.baseDecor < 0f) ? "consumed" : "produced");
			string text5 = ((this.baseDecor < 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED);
			string text6 = GameUtil.AddPositiveSign(this.baseDecor.ToString(), this.baseDecor > 0f);
			Descriptor descriptor2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, text4, text6, this.baseRadius), string.Format(text5, text6, this.baseRadius), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor2);
		}
		return list;
	}

	public static int GetLightDecorBonus(int cell)
	{
		if (Grid.LightIntensity[cell] > 0)
		{
			return DECOR.LIT_BONUS;
		}
		return 0;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetEffectDescriptions();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.GetEffectDescriptions();
	}

	public const string ID = "DecorProvider";

	private int width;

	private int height;

	private int previousDecor;

	public float baseRadius;

	public float baseDecor;

	public string overrideName;

	private GameScenePartitionerEntry partitionerEntry;

	public global::System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectDecorProvidersCallback;

	public AttributeInstance decor;

	public AttributeInstance decorRadius;

	public bool isMovable;

	[MyCmpReq]
	public OccupyArea occupyArea;

	[MyCmpGet]
	public Pickupable pickupable;

	[MyCmpGet]
	public SimCellOccupier simCellOccupier;

	private int[] cells = new int[512];

	private int cellCount;

	[MyCmpReq]
	private Modifiers modifiers;

	private DecorProvider.Splat splat;

	private struct Splat
	{
		public Splat(DecorProvider provider)
		{
			this = default(DecorProvider.Splat);
			AttributeInstance decor = provider.decor;
			this.decor = 0f;
			if (decor != null)
			{
				this.decor = decor.GetTotalValue();
			}
			if (provider.HasTag(GameTags.Stored))
			{
				this.decor = 0f;
			}
			int num = Grid.PosToCell(provider.gameObject);
			if (!Grid.IsValidCell(num))
			{
				return;
			}
			if (!Grid.Transparent[num] && Grid.Solid[num] && provider.simCellOccupier == null)
			{
				this.decor = 0f;
			}
			if (this.decor == 0f)
			{
				return;
			}
			provider.cellCount = 0;
			OccupyArea occupyArea = provider.occupyArea;
			int widthInCells = occupyArea.GetWidthInCells();
			int heightInCells = occupyArea.GetHeightInCells();
			this.provider = provider;
			this.radius = 5;
			AttributeInstance decorRadius = provider.decorRadius;
			if (decorRadius != null)
			{
				this.radius = (int)decorRadius.GetTotalValue();
			}
			int num2 = 0;
			int num3 = 0;
			Grid.CellToXY(num, out num2, out num3);
			Vector2I vector2I = new Vector2I(num2 - this.radius, num3 - this.radius);
			Vector2I vector2I2 = vector2I + new Vector2I(this.radius * 2 + widthInCells, this.radius * 2 + heightInCells);
			vector2I = Vector2I.Max(vector2I, Vector2I.zero);
			vector2I2 = Vector2I.Min(vector2I2, new Vector2I(Grid.WidthInCells - 1, Grid.HeightInCells - 1));
			this.extents = new Extents(vector2I.x, vector2I.y, vector2I2.x - vector2I.x, vector2I2.y - vector2I.y);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatCollectDecorProviders", provider.gameObject, this.extents, GameScenePartitioner.Instance.decorProviderLayer, provider.onCollectDecorProvidersCallback);
			this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatSolidCheck", provider.gameObject, this.extents, GameScenePartitioner.Instance.solidChangedLayer, provider.refreshPartionerCallback);
			this.AddDecor();
		}

		public float decor { get; private set; }

		public void Clear()
		{
			if (this.decor == 0f)
			{
				return;
			}
			this.RemoveDecor();
			if (this.partitionerEntry != null)
			{
				this.partitionerEntry.Release();
				this.partitionerEntry = null;
			}
			if (this.solidChangedPartitionerEntry != null)
			{
				this.solidChangedPartitionerEntry.Release();
				this.solidChangedPartitionerEntry = null;
			}
		}

		private void AddDecor()
		{
			int num = Grid.PosToCell(this.provider);
			int num2 = this.extents.x + this.extents.width;
			int num3 = this.extents.y + this.extents.height;
			int num4 = this.extents.x;
			int num5 = this.extents.y;
			int num6 = 0;
			int num7 = 0;
			Grid.CellToXY(num, out num6, out num7);
			num2 = Math.Min(num2, Grid.WidthInCells);
			num3 = Math.Min(num3, Grid.HeightInCells);
			num4 = Math.Max(0, num4);
			num5 = Math.Max(0, num5);
			for (int i = num4; i < num2; i++)
			{
				for (int j = num5; j < num3; j++)
				{
					if (Grid.VisibilityTest(num6, num7, i, j, false, false))
					{
						int num8 = Grid.XYToCell(i, j);
						if (Grid.IsValidCell(num8))
						{
							Grid.Decor[num8] += this.decor;
							this.provider.cells[this.provider.cellCount++] = num8;
						}
					}
				}
			}
		}

		private void RemoveDecor()
		{
			if (this.decor == 0f)
			{
				return;
			}
			if (this.provider == null)
			{
				return;
			}
			for (int i = 0; i < this.provider.cellCount; i++)
			{
				int num = this.provider.cells[i];
				if (Grid.IsValidCell(num))
				{
					Grid.Decor[num] -= this.decor;
				}
			}
		}

		private DecorProvider provider;

		private int radius;

		private Extents extents;

		private GameScenePartitionerEntry partitionerEntry;

		private GameScenePartitionerEntry solidChangedPartitionerEntry;
	}
}
