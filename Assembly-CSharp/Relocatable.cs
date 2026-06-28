using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Relocatable : Workable, ISaveLoadable
{
	private Relocatable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public Constructable Target
	{
		get
		{
			return this.target.Get();
		}
		set
		{
			this.target.Set(value);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-111137758, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Relocating;
		this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
		this.multitoolContext = "build";
		this.multitoolHitEffectTag = "fx_build_splash";
		base.SetWorkTime(4f);
	}

	protected override void OnStartWork(Worker worker)
	{
		this.progressBar.barColor = Color.red;
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		PrimaryElement primary_element = base.GetComponent<PrimaryElement>();
		Building building = base.GetComponent<Building>();
		SimCellOccupier component = base.GetComponent<SimCellOccupier>();
		if (DetailsScreen.Instance != null && DetailsScreen.Instance.CompareTargetWith(base.gameObject))
		{
			DetailsScreen.Instance.Show(false);
		}
		if (component != null)
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (Grid.Objects[num, (int)building.Def.TileLayer] == base.gameObject)
			{
				Grid.Objects[num, (int)building.Def.ObjectLayer] = null;
				Grid.Objects[num, (int)building.Def.TileLayer] = null;
				Grid.Foundation[num] = false;
				TileVisualizer.RefreshCell(num, building.Def.TileLayer);
			}
			component.DestroySelf(delegate
			{
				this.SpawnPackage(this.transform.GetPosition(), building.Def, primary_element);
				this.gameObject.DeleteObject();
			});
		}
		else
		{
			this.SpawnPackage(base.transform.GetPosition(), building.Def, primary_element);
			base.gameObject.DeleteObject();
		}
		base.Trigger(-702296337, this);
	}

	public void QueueRelocation(Constructable target)
	{
		if (this.Target == null)
		{
			if (this.deconstruct && this.chore == null)
			{
				this.chore = new WorkChore<Relocatable>(Db.Get().ChoreTypes.Relocate, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, this);
			}
			this.Target = target;
			this.Target.Source = base.gameObject;
			this.Target.isRelocating = true;
		}
	}

	private void OpenRelocateTool()
	{
		Building component = base.GetComponent<Building>();
		PrimaryElement component2 = base.GetComponent<PrimaryElement>();
		RelocateTool.Instance.Activate(component.Def, new List<Element> { component2.Element }, this);
	}

	private void SpawnPackage(Vector3 position, BuildingDef def, PrimaryElement primaryElement)
	{
		int num = Grid.PosToCell(position);
		CellOffset[] placementOffsets = def.PlacementOffsets;
		int num2 = Grid.OffsetCell(num, placementOffsets[0]);
		Vector3 vector = Grid.CellToPosCBC(num2, Grid.SceneLayer.Ore);
		GameObject gameObject = Util.KInstantiate(def.BuildingPackage, vector, Quaternion.identity, SceneOrganizer.Instance.GetFolder(Folder.Loot), null, true, 0);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.ElementID = primaryElement.ElementID;
		component.Temperature = primaryElement.Temperature;
		component.Mass = primaryElement.Mass;
		gameObject.SetActive(true);
		Relocatable component2 = gameObject.GetComponent<Relocatable>();
		component2.QueueRelocation(this.Target);
	}

	private void OnRefreshUserMenu(object data)
	{
	}

	private void CancelRelocation()
	{
		if (this.Target != null)
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Cancelled relocation");
				this.chore = null;
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, false);
				base.ShowProgressBar(false);
			}
			this.Target.Trigger(2127324410, null);
			this.Target = null;
		}
	}

	private void OnCancel(object data)
	{
		this.CancelRelocation();
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private Ref<Constructable> target = new Ref<Constructable>();

	public bool deconstruct = true;
}
