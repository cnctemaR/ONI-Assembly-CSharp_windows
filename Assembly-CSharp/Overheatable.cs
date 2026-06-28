using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Overheatable : KMonoBehaviour
{
	public Overheatable()
	{
		Func<List<Notification>, object, string> func = new Func<List<Notification>, object, string>(Overheatable.OnBuildingListTooltip);
		this.BuildingCloseToMeltingPoint = new Notification(MISC.NOTIFICATIONS.BUILDINGCLOSETOMELTINGPOINT.NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
		func = new Func<List<Notification>, object, string>(Overheatable.OnBuildingListTooltip);
		this.BuildingMeltingDown = new Notification(MISC.NOTIFICATIONS.BUILDINGMELTINGDOWN.NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
		func = new Func<List<Notification>, object, string>(Overheatable.OnBuildingListTooltip);
		this.BuildingCollapse = new Notification(MISC.NOTIFICATIONS.BUILDINGCOLLAPSE.NAME, NotificationType.Bad, null, func, null, true, 0f, null, null, null);
		this.nextExplosion = 0.15f;
		base..ctor();
	}

	private float Temperature
	{
		get
		{
			return (!(this.structureTemperature != null)) ? Grid.Temperature[Grid.PosToCell(this.transform.position)] : this.structureTemperature.Temperature;
		}
	}

	public float MeltingPoint
	{
		get
		{
			return this.building.Def.BaseMeltingPoint;
		}
	}

	public float NearMeltingPoint
	{
		get
		{
			return this.MeltingPoint - 15f;
		}
	}

	public bool IsNearMeltingPoint
	{
		get
		{
			return this.Temperature >= this.NearMeltingPoint;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(1930836866, new EventSystem.EventHandler(this.OnMeltDown));
		this.Subscribe(-2009062694, new EventSystem.EventHandler(this.OnNearMelting));
		this.Subscribe(-702296337, new EventSystem.EventHandler(this.OnDeconstructComplete));
	}

	private void OnCollapse(object data)
	{
		if (this.meltdownStartTime == 3.4028235E+38f)
		{
			this.meltdownStartTime = Time.time;
			this.isBuildingDamaged = true;
			base.GetComponent<Notifier>().Add(this.BuildingCollapse, string.Empty);
			base.enabled = true;
		}
	}

	private void OnMeltDown(object data)
	{
		if (this.meltdownStartTime == 3.4028235E+38f)
		{
			this.meltdownStartTime = Time.time;
			this.isBuildingDamaged = true;
			base.GetComponent<Notifier>().Add(this.BuildingMeltingDown, string.Empty);
			base.enabled = true;
		}
	}

	private void OnNearMelting(object data)
	{
		Notifier component = base.GetComponent<Notifier>();
		bool flag = (bool)data;
		if (flag)
		{
			component.Add(this.BuildingCloseToMeltingPoint, string.Empty);
		}
		else
		{
			component.Remove(this.BuildingCloseToMeltingPoint);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		switch (this.building.Def.ExplosionSize)
		{
		case Overheatable.ExplosionSize.Small:
			this.explosionTime = 0.33f;
			this.explosionInterval = 0.33f;
			this.explosionHeat = 400f;
			this.explosionPressure = 300f;
			break;
		case Overheatable.ExplosionSize.Medium:
			this.explosionTime = 1f;
			this.explosionInterval = 0.15f;
			this.explosionHeat = 400f;
			this.explosionPressure = 300f;
			break;
		case Overheatable.ExplosionSize.Large:
			this.explosionTime = 3f;
			this.explosionInterval = 0.33f;
			this.explosionHeat = 1000f;
			this.explosionPressure = 1000f;
			break;
		}
		if (this.building.Def.ContinuouslyCheckFoundation)
		{
			Extents validPlacementExtents = this.building.GetValidPlacementExtents();
			int num = 0;
			num |= GameScenePartitioner.Instance.solidChangedMask.mask;
			num |= GameScenePartitioner.Instance.objectLayerMasks[3].mask;
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", base.gameObject, validPlacementExtents, num, new Action<object>(this.OnSolidChanged));
		}
		base.enabled = false;
	}

	private void OnSolidChanged(object data)
	{
		SimCellOccupier component = base.GetComponent<SimCellOccupier>();
		if (!this.isBuildingDamaged && (component == null || component.IsReady()))
		{
			string text = null;
			if (this.building.IsValidBuildLocation(this.transform.position, out text))
			{
				this.UpdateSolidState(true);
			}
			else
			{
				this.UpdateSolidState(false);
			}
		}
	}

	private void UpdateSolidState(bool is_solid)
	{
		if (this.solid != is_solid)
		{
			this.solid = is_solid;
			Operational component = base.GetComponent<Operational>();
			if (component != null)
			{
				component.SetFlag(Overheatable.solidFoundation, is_solid);
			}
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, this);
		}
	}

	private void SimUpdate(float dt)
	{
		if (this.isBuildingDamaged)
		{
			if (this.building.Def.ExplosionSize != Overheatable.ExplosionSize.None)
			{
				KSelectable component = base.GetComponent<KSelectable>();
				if (component != null)
				{
					component.AddStatusItem(Db.Get().BuildingStatusItems.MeltingDown, null);
				}
				this.nextExplosion -= Time.deltaTime;
				if (this.nextExplosion < 0f)
				{
					Bounds bounds = base.GetComponentInChildren<Collider2D>().bounds;
					float num = 0.95f;
					float num2 = bounds.center.x - global::UnityEngine.Random.Range(-bounds.extents.x * num, bounds.extents.x * num);
					float num3 = bounds.center.y - global::UnityEngine.Random.Range(-bounds.extents.y * num, bounds.extents.y * num);
					Vector3 vector = new Vector3(num2, num3, -Grid.CellSizeInMeters * 0.5f);
					this.PlayExplosion(vector);
					int num4 = Grid.PosToCell(vector);
					SimMessages.ModifyEnergy(num4, this.explosionHeat, SimMessages.EnergySourceID.Overheatable);
					SimMessages.AddRemoveSubstance(num4, SimHashes.Oxygen, CellEventLogger.Instance.OverheatableMeltingDown, this.explosionPressure / 101.3f, 293f, -1);
					this.nextExplosion = this.explosionInterval;
				}
				if (Time.time - this.meltdownStartTime > this.explosionTime)
				{
					int num5 = Grid.PosToCell(this);
					foreach (CellOffset cellOffset in this.building.Def.PlacementOffsets)
					{
						int num6 = Grid.OffsetCell(num5, cellOffset);
						Vector3 vector2 = Grid.CellToPosCCC(num6, Grid.SceneLayer.Building);
						vector2.z = -Grid.CellSizeInMeters * 0.5f;
						this.PlayExplosion(vector2);
					}
					base.PlaySound3D(Sounds.Instance.BlowUp_GenericMigrated);
					Vector3 vector3 = Grid.CellToPosCCC(num5, Grid.SceneLayer.Move);
					GameUtil.CreateExplosion(vector3);
					this.Trigger(1623392196, base.gameObject);
					base.gameObject.DeleteObject();
				}
			}
			else
			{
				base.gameObject.DeleteObject();
			}
		}
	}

	private void PlayExplosion(Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(EffectPrefabs.Instance.Explosion, SceneOrganizer.Instance.GetFolder(Folder.FX), null);
		Util.Reset(gameObject.transform);
		gameObject.transform.localRotation = EffectPrefabs.Instance.Explosion.transform.localRotation;
		gameObject.transform.localPosition = pos;
	}

	private void OnDeconstructComplete(object data)
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	private static string OnBuildingListTooltip(List<Notification> notifications, object data)
	{
		return string.Format(MISC.NOTIFICATIONS.BUILDINGCOLLAPSE.TOOLTIP, notifications.ReduceMessages(true));
	}

	public const float NearMeltingPointOffset = 15f;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private StructureTemperature structureTemperature;

	private bool solid = true;

	private bool isBuildingDamaged;

	private float meltdownStartTime = float.MaxValue;

	private Notification BuildingCloseToMeltingPoint;

	private Notification BuildingMeltingDown;

	private Notification BuildingCollapse;

	private GameScenePartitionerEntry partitionerEntry;

	public static Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	[SerializeField]
	private float nextExplosion;

	public float explosionTime;

	public float explosionInterval;

	public float explosionHeat;

	public float explosionPressure;

	public enum ExplosionSize
	{
		None,
		Small,
		Medium,
		Large
	}
}
