using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/DecorProvider")]
public class DecorProvider : KMonoBehaviour, IGameObjectEffectDescriptor
{
	private void AddDecor()
	{
		this.currDecor = 0f;
		if (this.decor != null)
		{
			this.currDecor = this.decor.GetTotalValue();
		}
		if (this.prefabId.HasTag(GameTags.Stored))
		{
			this.currDecor = 0f;
		}
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (!Grid.Transparent[num] && Grid.Solid[num] && this.simCellOccupier == null)
		{
			this.currDecor = 0f;
		}
		if (this.currDecor == 0f)
		{
			return;
		}
		this.cellCount = 0;
		int num2 = 5;
		if (this.decorRadius != null)
		{
			num2 = (int)this.decorRadius.GetTotalValue();
		}
		Extents extents = this.occupyArea.GetExtents();
		extents.x = Mathf.Max(extents.x - num2, 0);
		extents.y = Mathf.Max(extents.y - num2, 0);
		extents.width = Mathf.Min(extents.width + num2 * 2, Grid.WidthInCells - 1);
		extents.height = Mathf.Min(extents.height + num2 * 2, Grid.HeightInCells - 1);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatCollectDecorProviders", base.gameObject, extents, GameScenePartitioner.Instance.decorProviderLayer, this.onCollectDecorProvidersCallback);
		this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("DecorProvider.SplatSolidCheck", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, this.refreshPartionerCallback);
		int num3 = extents.x + extents.width;
		int num4 = extents.y + extents.height;
		int num5 = extents.x;
		int num6 = extents.y;
		int num7;
		int num8;
		Grid.CellToXY(num, out num7, out num8);
		num3 = Math.Min(num3, Grid.WidthInCells);
		num4 = Math.Min(num4, Grid.HeightInCells);
		num5 = Math.Max(0, num5);
		num6 = Math.Max(0, num6);
		int num9 = (num3 - num5) * (num4 - num6);
		if (this.cells == null || this.cells.Length != num9)
		{
			this.cells = new int[num9];
		}
		for (int i = num5; i < num3; i++)
		{
			for (int j = num6; j < num4; j++)
			{
				if (Grid.VisibilityTest(num7, num8, i, j, false))
				{
					int num10 = Grid.XYToCell(i, j);
					if (Grid.IsValidCell(num10))
					{
						Grid.Decor[num10] += this.currDecor;
						int[] array = this.cells;
						int num11 = this.cellCount;
						this.cellCount = num11 + 1;
						array[num11] = num10;
					}
				}
			}
		}
	}

	public void Clear()
	{
		if (this.currDecor == 0f)
		{
			return;
		}
		this.RemoveDecor();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
	}

	private void RemoveDecor()
	{
		if (this.currDecor == 0f)
		{
			return;
		}
		for (int i = 0; i < this.cellCount; i++)
		{
			int num = this.cells[i];
			if (Grid.IsValidCell(num))
			{
				Grid.Decor[num] -= this.currDecor;
			}
		}
	}

	public void Refresh()
	{
		this.Clear();
		this.AddDecor();
	}

	public float GetDecorForCell(int cell)
	{
		for (int i = 0; i < this.cellCount; i++)
		{
			if (this.cells[i] == cell)
			{
				return this.currDecor;
			}
		}
		return 0f;
	}

	public void SetValues(EffectorValues values)
	{
		this.baseDecor = (float)values.amount;
		this.baseRadius = (float)values.radius;
		if (base.IsInitialized())
		{
			this.UpdateBaseDecorModifiers();
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.decor = this.GetAttributes().Add(Db.Get().BuildingAttributes.Decor);
		this.decorRadius = this.GetAttributes().Add(Db.Get().BuildingAttributes.DecorRadius);
		this.UpdateBaseDecorModifiers();
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
		this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, DecorProvider.RefreshDispatcher, this, "DecorProvider.OnSpawn");
		AttributeInstance attributeInstance = this.decor;
		attributeInstance.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance.OnDirty, this.refreshCallback);
		AttributeInstance attributeInstance2 = this.decorRadius;
		attributeInstance2.OnDirty = (global::System.Action)Delegate.Combine(attributeInstance2.OnDirty, this.refreshCallback);
		this.Refresh();
	}

	private void UpdateBaseDecorModifiers()
	{
		Attributes attributes = this.GetAttributes();
		if (this.baseDecorModifier != null)
		{
			attributes.Remove(this.baseDecorModifier);
			attributes.Remove(this.baseDecorRadiusModifier);
			this.baseDecorModifier = null;
			this.baseDecorRadiusModifier = null;
		}
		if (this.baseDecor != 0f)
		{
			this.baseDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, this.baseDecor, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			this.baseDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, this.baseRadius, UI.TOOLTIPS.BASE_VALUE, false, false, true);
			attributes.Add(this.baseDecorModifier);
			attributes.Add(this.baseDecorRadiusModifier);
		}
	}

	private void OnCollectDecorProviders(object data)
	{
		((List<DecorProvider>)data).Add(this);
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
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
		}
		this.Clear();
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.decor != null && this.decorRadius != null)
		{
			float totalValue = this.decor.GetTotalValue();
			float totalValue2 = this.decorRadius.GetTotalValue();
			string text = ((this.baseDecor > 0f) ? "produced" : "consumed");
			string text2 = ((this.baseDecor > 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED);
			text2 = text2 + "\n\n" + this.decor.GetAttributeValueTooltip();
			string text3 = GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f);
			Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.DECORPROVIDED, text, text3, totalValue2), string.Format(text2, text3, totalValue2), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
		}
		else if (this.baseDecor != 0f)
		{
			string text4 = ((this.baseDecor >= 0f) ? "produced" : "consumed");
			string text5 = ((this.baseDecor >= 0f) ? UI.BUILDINGEFFECTS.TOOLTIPS.DECORPROVIDED : UI.BUILDINGEFFECTS.TOOLTIPS.DECORDECREASED);
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

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.GetEffectDescriptions();
	}

	public const string ID = "DecorProvider";

	public float baseRadius;

	public float baseDecor;

	public string overrideName;

	public global::System.Action refreshCallback;

	public Action<object> refreshPartionerCallback;

	public Action<object> onCollectDecorProvidersCallback;

	public AttributeInstance decor;

	public AttributeInstance decorRadius;

	private AttributeModifier baseDecorModifier;

	private AttributeModifier baseDecorRadiusModifier;

	[MyCmpReq]
	private KPrefabID prefabId;

	[MyCmpReq]
	public OccupyArea occupyArea;

	[MyCmpGet]
	public SimCellOccupier simCellOccupier;

	private int[] cells;

	private int cellCount;

	public float currDecor;

	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle solidChangedPartitionerEntry;

	private ulong cellChangedHandlerID;

	private static Action<object> RefreshDispatcher = delegate(object obj)
	{
		Unsafe.As<DecorProvider>(obj).Refresh();
	};
}
