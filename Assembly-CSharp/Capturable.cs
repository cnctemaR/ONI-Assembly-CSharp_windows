using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Capturable : Workable, IGameObjectEffectDescriptor
{
	public bool IsMarkedForCapture
	{
		get
		{
			return this.markedForCapture;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Capturables.Add(this);
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
		this.resetProgressOnStop = true;
		this.faceTargetWhenWorking = true;
		this.multitoolContext = "capture";
		this.multitoolHitEffectTag = "fx_capture_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		if (this.markedForCapture)
		{
			Prioritizable.AddRef(base.gameObject);
		}
		this.UpdateStatusItem();
		this.UpdateChore();
		base.SetWorkTime(10f);
	}

	protected override void OnCleanUp()
	{
		Components.Capturables.Remove(this);
		base.OnCleanUp();
	}

	private void OnDeath(object data)
	{
		this.allowCapture = false;
		this.markedForCapture = false;
	}

	public void MarkForCapture(bool mark)
	{
		if (this.markedForCapture && !mark)
		{
			Prioritizable.RemoveRef(base.gameObject);
		}
		else if (!this.markedForCapture && mark)
		{
			Prioritizable.AddRef(base.gameObject);
		}
		this.markedForCapture = this.allowCapture && mark;
		this.UpdateStatusItem();
		this.UpdateChore();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.markedForCapture)
		{
			UserMenu userMenu = this.userMenu;
			string text = "action_capture";
			string text2 = UI.USERMENUACTIONS.CAPTURE.NAME;
			global::System.Action action = delegate
			{
				this.MarkForCapture(true);
			};
			string text3 = UI.USERMENUACTIONS.CAPTURE.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text3 = "action_capture";
			string text2 = UI.USERMENUACTIONS.CANCELCAPTURE.NAME;
			global::System.Action action = delegate
			{
				this.MarkForCapture(false);
			};
			string text = UI.USERMENUACTIONS.CANCELCAPTURE.TOOLTIP;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	private void UpdateStatusItem()
	{
		this.shouldShowRolePerkStatusItem = this.markedForCapture;
		base.UpdateStatusItem(null);
		if (this.markedForCapture)
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderCapture, this);
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderCapture, false);
		}
	}

	private void UpdateChore()
	{
		if (this.markedForCapture && this.chore == null)
		{
			ChoreType capture = Db.Get().ChoreTypes.Capture;
			Tag[] array = new Tag[] { GameTags.ChoreTypes.Ranching };
			this.chore = new WorkChore<Capturable>(capture, this, null, array, true, null, null, null, true, null, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		}
		else if (!this.markedForCapture && this.chore != null)
		{
			this.chore.Cancel("not marked for capture");
			this.chore = null;
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Stunned);
	}

	protected override void OnStopWork(Worker worker)
	{
		KPrefabID component = base.GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.Creatures.Stunned);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		int num = this.NaturalBuildingCell();
		if (Grid.Solid[num])
		{
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				num = num2;
			}
		}
		KPrefabID component = base.GetComponent<KPrefabID>();
		Tag baggedCreatureTag = EntityTemplates.GetBaggedCreatureTag(component.PrefabTag);
		GameObject prefab = Assets.GetPrefab(baggedCreatureTag);
		GameObject gameObject = Util.KInstantiate(prefab, Folder.Entities, Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
		gameObject.SetActive(true);
		Util.KDestroyGameObject(base.gameObject);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_WRANGLE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_WRANGLE, Descriptor.DescriptorType.Effect, false));
		return descriptors;
	}

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[MyCmpReq]
	private UserMenu userMenu;

	[Serialize]
	private bool allowCapture = true;

	[Serialize]
	private bool markedForCapture;

	private Chore chore;
}
