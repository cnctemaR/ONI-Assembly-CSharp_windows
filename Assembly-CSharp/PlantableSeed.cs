using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class PlantableSeed : KMonoBehaviour, IHasSortOrder, IReceptacleDirection, IGameObjectEffectDescriptor
{
	public int sortOrder { get; set; }

	public SingleEntityReceptacle.ReceptacleDirection Direction
	{
		get
		{
			return this.direction;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
		base.Subscribe(1335436905, new Action<object>(this.OnSplit));
		this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
	}

	private void OnAbsorb(object data)
	{
	}

	private void OnSplit(object data)
	{
	}

	private void SimUpdate(float dt)
	{
		this.timeUntilSelfPlant -= dt;
		if (this.timeUntilSelfPlant <= 0f)
		{
			this.TryPlant();
		}
	}

	public void TryPlant()
	{
		this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
		int num = Grid.PosToCell(base.gameObject);
		if (this.TestSuitableGround(num, false))
		{
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.BuildingFront);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.PlantID), vector, Grid.SceneLayer.BuildingFront, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
			gameObject.SetActive(true);
			Pickupable component = base.GetComponent<Pickupable>();
			Pickupable pickupable = component.Take(1f);
			if (pickupable != null)
			{
				Crop component2 = gameObject.GetComponent<Crop>();
				if (component2 != null)
				{
				}
				Util.KDestroyGameObject(pickupable.gameObject);
			}
			else
			{
				KCrashReporter.Assert(false, "Seed has fractional total amount < 1f");
			}
		}
	}

	private bool TestSuitableGround(int cell, bool ignoreGround = false)
	{
		bool flag;
		if (!Grid.IsValidCell(cell))
		{
			flag = false;
		}
		else
		{
			GameObject prefab = Assets.GetPrefab(this.PlantID);
			EntombVulnerable component = prefab.GetComponent<EntombVulnerable>();
			if (component != null && !component.IsCellSafe(cell))
			{
				flag = false;
			}
			else
			{
				PressureVulnerable component2 = prefab.GetComponent<PressureVulnerable>();
				if (component2 != null && !component2.IsCellSafe(cell))
				{
					flag = false;
				}
				else
				{
					DrowningMonitor component3 = prefab.GetComponent<DrowningMonitor>();
					if (component3 != null && !component3.IsCellSafe(cell))
					{
						flag = false;
					}
					else
					{
						TemperatureVulnerable component4 = prefab.GetComponent<TemperatureVulnerable>();
						if (component4 != null && !component4.IsCellSafe(cell))
						{
							flag = false;
						}
						else
						{
							UprootedMonitor component5 = prefab.GetComponent<UprootedMonitor>();
							if (component5 != null && !component5.IsCellSafe(cell))
							{
								flag = false;
							}
							else
							{
								OccupyArea component6 = prefab.GetComponent<OccupyArea>();
								if (component6 != null && !component6.CanOccupyArea(cell, ObjectLayer.Building))
								{
									flag = false;
								}
								else
								{
									if (!ignoreGround)
									{
										int num = Grid.CellBelow(cell);
										if (Grid.Foundation[num] || (this.replantGroundTag.IsValid && !Grid.Element[num].HasTag(this.replantGroundTag)))
										{
											return false;
										}
									}
									flag = true;
								}
							}
						}
					}
				}
			}
		}
		return flag;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
		{
			Descriptor descriptor = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_CEILING, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_CEILING, Descriptor.DescriptorType.Requirement, false);
			list.Add(descriptor);
		}
		else if (this.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
		{
			Descriptor descriptor2 = new Descriptor(UI.GAMEOBJECTEFFECTS.SEED_REQUIREMENT_WALL, UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_REQUIREMENT_WALL, Descriptor.DescriptorType.Requirement, false);
			list.Add(descriptor2);
		}
		return list;
	}

	public Tag PlantID;

	public Tag PreviewID;

	[Serialize]
	public float timeUntilSelfPlant = 0f;

	public Tag replantGroundTag;

	public string domesticatedDescription;

	public SingleEntityReceptacle.ReceptacleDirection direction = SingleEntityReceptacle.ReceptacleDirection.Top;
}
