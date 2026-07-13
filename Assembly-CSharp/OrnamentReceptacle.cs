using System;
using UnityEngine;

public class OrnamentReceptacle : SingleEntityReceptacle
{
	public bool IsHoldingOrnament
	{
		get
		{
			return base.Occupant != null && base.Occupant.HasTag(GameTags.Ornament);
		}
	}

	public bool IsOperational
	{
		get
		{
			return this.operational == null || this.operational.IsOperational;
		}
	}

	protected override void OnPrefabInit()
	{
		this.ornamentDisabledStatusItem = Db.Get().BuildingStatusItems.OrnamentDisabled;
		this.noItemDisplayedStatusItem = Db.Get().BuildingStatusItems.PedestalNoItemDisplayed;
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.AddAdditionalCriteria((GameObject obj) => obj.HasTag(GameTags.PedestalDisplayable));
		if (base.occupyingObject == null && this.storage.MassStored() > 0f)
		{
			this.OnDepositObject(this.storage.items[0]);
			return;
		}
		this.RefreshDecorTag();
	}

	protected override void ClearOccupant()
	{
		base.ClearOccupant();
		this.RefreshDecorTag();
		int num = Grid.PosToCell(base.gameObject);
		Game.Instance.roomProber.TriggerBuildingChangedEvent(num, base.gameObject);
	}

	protected override void OnDepositObject(GameObject depositedObject)
	{
		base.OnDepositObject(depositedObject);
		this.RefreshDecorTag();
		int num = Grid.PosToCell(base.gameObject);
		Game.Instance.roomProber.TriggerBuildingChangedEvent(num, base.gameObject);
	}

	protected override void OnOperationalChanged(object data)
	{
		base.OnOperationalChanged(data);
		this.RefreshDecorTag();
		int num = Grid.PosToCell(base.gameObject);
		Game.Instance.roomProber.TriggerBuildingChangedEvent(num, base.gameObject);
		base.UpdateStatusItem();
	}

	protected override void PositionOccupyingObject()
	{
		base.PositionOccupyingObject();
		this.refreshAnims = true;
	}

	public override void Render1000ms(float dt)
	{
		base.Render1000ms(dt);
		if (this.refreshAnims)
		{
			if (base.Occupant != null)
			{
				KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
				component.enabled = false;
				component.enabled = true;
			}
			KBatchedAnimController component2 = base.GetComponent<KBatchedAnimController>();
			component2.enabled = false;
			component2.enabled = true;
			this.refreshAnims = false;
		}
	}

	protected override void UpdateStatusItem(KSelectable selectable)
	{
		base.UpdateStatusItem(selectable);
		if (this.operational != null && this.IsHoldingOrnament && !this.operational.IsOperational)
		{
			selectable.AddStatusItem(this.ornamentDisabledStatusItem, null);
		}
		else
		{
			selectable.RemoveStatusItem(this.ornamentDisabledStatusItem, false);
		}
		if (base.Occupant == null && (this.operational == null || this.operational.IsOperational))
		{
			selectable.AddStatusItem(this.noItemDisplayedStatusItem, null);
			return;
		}
		selectable.RemoveStatusItem(this.noItemDisplayedStatusItem, false);
	}

	public virtual void RefreshDecorTag()
	{
		KPrefabID component = base.gameObject.GetComponent<KPrefabID>();
		bool flag = component.HasTag(GameTags.Decoration);
		bool flag2 = base.Occupant != null && (this.operational == null || this.operational.IsOperational);
		if (flag2)
		{
			component.AddTag(GameTags.Decoration, false);
		}
		else
		{
			component.RemoveTag(GameTags.Decoration);
		}
		if (flag != flag2)
		{
			Game.Instance.roomProber.TriggerBuildingChangedEvent(Grid.PosToCell(base.gameObject), component);
		}
	}

	protected StatusItem ornamentDisabledStatusItem;

	protected StatusItem noItemDisplayedStatusItem;

	private bool refreshAnims;
}
