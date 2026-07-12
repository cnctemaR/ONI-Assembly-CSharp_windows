using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuddingTrunk")]
public class BuddingTrunk : KMonoBehaviour, ISim4000ms
{
	public bool ExtraSeedAvailable
	{
		get
		{
			return this.hasExtraSeedAvailable;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.simRenderLoadBalance = true;
		this.growingBranchesStatusItem = new StatusItem("GROWINGBRANCHES", "MISC", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 129022, null);
		base.Subscribe<BuddingTrunk>(1119167081, BuddingTrunk.OnNewGameSpawnDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<BuddingTrunk>(-216549700, BuddingTrunk.OnUprootedDelegate);
		base.Subscribe<BuddingTrunk>(-750750377, BuddingTrunk.OnDrownedDelegate);
		base.Subscribe<BuddingTrunk>(-266953818, BuddingTrunk.OnHarvestDesignationChangedDelegate);
	}

	protected override void OnCleanUp()
	{
		if (this.newGameSpawnRoutine != null)
		{
			base.StopCoroutine(this.newGameSpawnRoutine);
		}
		base.OnCleanUp();
	}

	private void OnNewGameSpawn(object data)
	{
		float num = 1f;
		if ((double)global::UnityEngine.Random.value < 0.1)
		{
			num = global::UnityEngine.Random.Range(0.75f, 0.99f);
		}
		else
		{
			this.newGameSpawnRoutine = base.StartCoroutine(this.NewGameSproutBudRoutine());
		}
		this.growing.OverrideMaturityLevel(num);
	}

	private IEnumerator NewGameSproutBudRoutine()
	{
		int num2;
		for (int i = 0; i < this.buds.Length; i = num2 + 1)
		{
			yield return SequenceUtil.WaitForEndOfFrame;
			float num = global::UnityEngine.Random.Range(0f, 1f);
			this.TrySpawnRandomBud(null, num);
			num2 = i;
		}
		this.newGameSpawnRoutine = null;
		yield return SequenceUtil.WaitForNextFrame;
		yield break;
	}

	public void Sim4000ms(float dt)
	{
		if (this.growing.IsGrown() && !this.wilting.IsWilting())
		{
			this.TrySpawnRandomBud(null, 0f);
			base.GetComponent<KSelectable>().AddStatusItem(this.growingBranchesStatusItem, null);
			return;
		}
		base.GetComponent<KSelectable>().RemoveStatusItem(this.growingBranchesStatusItem, false);
	}

	private void OnUprooted(object data = null)
	{
		this.YieldWood();
	}

	private void YieldWood()
	{
		foreach (Ref<HarvestDesignatable> @ref in this.buds)
		{
			HarvestDesignatable harvestDesignatable = ((@ref != null) ? @ref.Get() : null);
			if (harvestDesignatable != null)
			{
				harvestDesignatable.Trigger(-216549700, null);
			}
		}
	}

	public float GetMaxBranchMaturity()
	{
		float num = 0f;
		this.GetMostMatureBranch(out num);
		return num;
	}

	public void ConsumeMass(float mass_to_consume)
	{
		float num;
		HarvestDesignatable mostMatureBranch = this.GetMostMatureBranch(out num);
		if (mostMatureBranch)
		{
			Growing component = mostMatureBranch.GetComponent<Growing>();
			if (component)
			{
				component.ConsumeMass(mass_to_consume);
			}
		}
	}

	private HarvestDesignatable GetMostMatureBranch(out float max_maturity)
	{
		max_maturity = 0f;
		HarvestDesignatable harvestDesignatable = null;
		foreach (Ref<HarvestDesignatable> @ref in this.buds)
		{
			HarvestDesignatable harvestDesignatable2 = ((@ref != null) ? @ref.Get() : null);
			if (harvestDesignatable2 != null)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(harvestDesignatable2);
				if (amountInstance != null)
				{
					float num = amountInstance.value / amountInstance.GetMax();
					if (num > max_maturity)
					{
						max_maturity = num;
						harvestDesignatable = harvestDesignatable2;
					}
				}
			}
		}
		return harvestDesignatable;
	}

	public void TrySpawnRandomBud(object data = null, float growth_percentage = 0f)
	{
		if (this.uprooted.IsUprooted)
		{
			return;
		}
		BuddingTrunk.spawn_choices.Clear();
		int num = 0;
		for (int i = 0; i < this.buds.Length; i++)
		{
			int num2 = Grid.PosToCell(this.GetBudPosition(i));
			if ((this.buds[i] == null || this.buds[i].Get() == null) && this.CanGrowInto(num2))
			{
				BuddingTrunk.spawn_choices.Add(i);
			}
			else if (this.buds[i] != null && this.buds[i].Get() != null)
			{
				num++;
			}
		}
		if (num >= this.maxBuds)
		{
			return;
		}
		BuddingTrunk.spawn_choices.Shuffle<int>();
		if (BuddingTrunk.spawn_choices.Count > 0)
		{
			int num3 = BuddingTrunk.spawn_choices[0];
			Vector3 budPosition = this.GetBudPosition(num3);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(this.budPrefabID), budPosition);
			gameObject.SetActive(true);
			gameObject.GetComponent<Growing>().OverrideMaturityLevel(growth_percentage);
			gameObject.GetComponent<TreeBud>().SetTrunkPosition(this, num3);
			HarvestDesignatable component = gameObject.GetComponent<HarvestDesignatable>();
			this.buds[num3] = new Ref<HarvestDesignatable>(component);
			this.UpdateBudHarvestState(component);
			this.TryRollNewSeed();
		}
	}

	public void TryRollNewSeed()
	{
		if (!this.hasExtraSeedAvailable && global::UnityEngine.Random.Range(0, 100) < 5)
		{
			this.hasExtraSeedAvailable = true;
		}
	}

	public TreeBud GetBranchAtPosition(int idx)
	{
		if (this.buds[idx] == null)
		{
			return null;
		}
		HarvestDesignatable harvestDesignatable = this.buds[idx].Get();
		if (!(harvestDesignatable != null))
		{
			return null;
		}
		return harvestDesignatable.GetComponent<TreeBud>();
	}

	public void ExtractExtraSeed()
	{
		if (!this.hasExtraSeedAvailable)
		{
			return;
		}
		this.hasExtraSeedAvailable = false;
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		Util.KInstantiate(Assets.GetPrefab("ForestTreeSeed"), position).SetActive(true);
	}

	private void UpdateBudHarvestState(HarvestDesignatable bud)
	{
		HarvestDesignatable component = base.GetComponent<HarvestDesignatable>();
		bud.SetHarvestWhenReady(component.HarvestWhenReady);
	}

	public void OnBranchRemoved(int idx, TreeBud treeBud)
	{
		if (idx < 0 || idx >= this.buds.Length)
		{
			global::Debug.Assert(false, "invalid branch index " + idx.ToString());
		}
		HarvestDesignatable component = treeBud.GetComponent<HarvestDesignatable>();
		HarvestDesignatable harvestDesignatable = ((this.buds[idx] != null) ? this.buds[idx].Get() : null);
		if (component != harvestDesignatable)
		{
			global::Debug.LogWarningFormat(base.gameObject, "OnBranchRemoved branch {0} does not match known branch {1}", new object[] { component, harvestDesignatable });
		}
		this.buds[idx] = null;
	}

	private void UpdateAllBudsHarvestStatus(object data = null)
	{
		foreach (Ref<HarvestDesignatable> @ref in this.buds)
		{
			if (@ref != null)
			{
				HarvestDesignatable harvestDesignatable = @ref.Get();
				if (harvestDesignatable == null)
				{
					global::Debug.LogWarning("harvest_designatable was null");
				}
				else
				{
					this.UpdateBudHarvestState(harvestDesignatable);
				}
			}
		}
	}

	public bool CanGrowInto(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (Grid.Solid[cell])
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		return Grid.IsValidCell(num) && !Grid.IsSubstantialLiquid(num, 0.35f) && !(Grid.Objects[cell, 1] != null) && !(Grid.Objects[cell, 5] != null) && !Grid.Foundation[cell];
	}

	private Vector3 GetBudPosition(int idx)
	{
		Vector3 vector = base.transform.position;
		switch (idx)
		{
		case 0:
			vector = base.transform.position + Vector3.left;
			break;
		case 1:
			vector = base.transform.position + Vector3.left + Vector3.up;
			break;
		case 2:
			vector = base.transform.position + Vector3.left + Vector3.up + Vector3.up;
			break;
		case 3:
			vector = base.transform.position + Vector3.up + Vector3.up;
			break;
		case 4:
			vector = base.transform.position + Vector3.right + Vector3.up + Vector3.up;
			break;
		case 5:
			vector = base.transform.position + Vector3.right + Vector3.up;
			break;
		case 6:
			vector = base.transform.position + Vector3.right;
			break;
		}
		return vector;
	}

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private WiltCondition wilting;

	[MyCmpReq]
	private UprootedMonitor uprooted;

	public string budPrefabID;

	public int maxBuds = 5;

	[Serialize]
	private Ref<HarvestDesignatable>[] buds = new Ref<HarvestDesignatable>[7];

	private StatusItem growingBranchesStatusItem;

	[Serialize]
	private bool hasExtraSeedAvailable;

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnNewGameSpawnDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnNewGameSpawn(data);
	});

	private Coroutine newGameSpawnRoutine;

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnUprootedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnDrownedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.OnUprooted(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuddingTrunk> OnHarvestDesignationChangedDelegate = new EventSystem.IntraObjectHandler<BuddingTrunk>(delegate(BuddingTrunk component, object data)
	{
		component.UpdateAllBudsHarvestStatus(data);
	});

	private static List<int> spawn_choices = new List<int>();
}
