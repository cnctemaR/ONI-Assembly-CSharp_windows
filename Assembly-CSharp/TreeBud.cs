using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class TreeBud : KMonoBehaviour, IWiltCause
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.simRenderLoadBalance = true;
		int num = Grid.PosToCell(base.gameObject);
		GameObject gameObject = Grid.Objects[num, 5];
		if (gameObject != null && gameObject != base.gameObject)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		else
		{
			this.SetOccupyGridSpace(true);
		}
		base.Subscribe<TreeBud>(1272413801, TreeBud.OnHarvestDelegate);
	}

	private void OnHarvest(object data)
	{
		if (this.buddingTrunk.Get() != null)
		{
			this.buddingTrunk.Get().TryRollNewSeed();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.buddingTrunk != null && this.buddingTrunk.Get() != null)
		{
			this.SubscribeToTrunk();
			this.UpdateAnimationSet();
		}
		else
		{
			global::Debug.LogWarning("TreeBud loaded with missing trunk reference. Destroying...");
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	protected override void OnCleanUp()
	{
		this.UnsubscribeToTrunk();
		this.SetOccupyGridSpace(false);
		base.OnCleanUp();
	}

	private void SetOccupyGridSpace(bool active)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (active)
		{
			GameObject gameObject = Grid.Objects[num, 5];
			if (gameObject != null && gameObject != base.gameObject)
			{
				global::Debug.LogWarningFormat(base.gameObject, "TreeBud.SetOccupyGridSpace already occupied by {0}", new object[] { gameObject });
			}
			Grid.Objects[num, 5] = base.gameObject;
		}
		else if (Grid.Objects[num, 5] == base.gameObject)
		{
			Grid.Objects[num, 5] = null;
		}
	}

	private void SubscribeToTrunk()
	{
		if (this.trunkWiltHandle != -1 || this.trunkWiltRecoverHandle != -1)
		{
			return;
		}
		global::Debug.Assert(this.buddingTrunk != null, "buddingTrunk null");
		BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
		global::Debug.Assert(buddingTrunk != null, "tree_trunk null");
		this.trunkWiltHandle = buddingTrunk.Subscribe(-724860998, new Action<object>(this.OnTrunkWilt));
		this.trunkWiltRecoverHandle = buddingTrunk.Subscribe(712767498, new Action<object>(this.OnTrunkRecover));
		base.Trigger(912965142, !buddingTrunk.GetComponent<WiltCondition>().IsWilting());
		ReceptacleMonitor component = base.GetComponent<ReceptacleMonitor>();
		ReceptacleMonitor component2 = buddingTrunk.GetComponent<ReceptacleMonitor>();
		PlantablePlot receptacle = component2.GetReceptacle();
		component.SetReceptacle(receptacle);
		Vector3 position = base.gameObject.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - 0.1f * (float)this.trunkPosition;
		base.gameObject.transform.SetPosition(position);
		base.GetComponent<BudUprootedMonitor>().SetParentObject(buddingTrunk.GetComponent<KPrefabID>());
	}

	private void UnsubscribeToTrunk()
	{
		if (this.buddingTrunk == null)
		{
			global::Debug.LogWarning("buddingTrunk null", base.gameObject);
			return;
		}
		BuddingTrunk buddingTrunk = this.buddingTrunk.Get();
		if (buddingTrunk == null)
		{
			global::Debug.LogWarning("tree_trunk null", base.gameObject);
			return;
		}
		buddingTrunk.Unsubscribe(this.trunkWiltHandle);
		buddingTrunk.Unsubscribe(this.trunkWiltRecoverHandle);
		buddingTrunk.OnBranchRemoved(this.trunkPosition, this);
	}

	public void SetTrunkPosition(BuddingTrunk budding_trunk, int idx)
	{
		this.buddingTrunk = new Ref<BuddingTrunk>(budding_trunk);
		this.trunkPosition = idx;
		this.SubscribeToTrunk();
		this.UpdateAnimationSet();
	}

	private void OnTrunkWilt(object data = null)
	{
		base.Trigger(912965142, false);
	}

	private void OnTrunkRecover(object data = null)
	{
		base.Trigger(912965142, true);
	}

	private void UpdateAnimationSet()
	{
		this.crop.anims = TreeBud.animSets[this.trunkPosition];
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Offset = TreeBud.animOffset[this.trunkPosition];
		component.Play(this.crop.anims.grow, KAnim.PlayMode.Paused, 1f, 0f);
		this.crop.RefreshPositionPercent();
	}

	public string WiltStateString
	{
		get
		{
			return "    • " + DUPLICANTS.STATS.TRUNKHEALTH.NAME;
		}
	}

	public WiltCondition.Condition[] Conditions
	{
		get
		{
			return new WiltCondition.Condition[] { WiltCondition.Condition.UnhealthyRoot };
		}
	}

	[MyCmpReq]
	private Growing growing;

	[MyCmpReq]
	private StandardCropPlant crop;

	[Serialize]
	public Ref<BuddingTrunk> buddingTrunk;

	[Serialize]
	private int trunkPosition;

	[Serialize]
	public int growingPos;

	private int trunkWiltHandle = -1;

	private int trunkWiltRecoverHandle = -1;

	private static StandardCropPlant.AnimSet[] animSets = new StandardCropPlant.AnimSet[]
	{
		new StandardCropPlant.AnimSet
		{
			grow = "branch_a_grow",
			grow_pst = "branch_a_grow_pst",
			idle_full = "branch_a_idle_full",
			wilt_base = "branch_a_wilt",
			harvest = "branch_a_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_b_grow",
			grow_pst = "branch_b_grow_pst",
			idle_full = "branch_b_idle_full",
			wilt_base = "branch_b_wilt",
			harvest = "branch_b_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_c_grow",
			grow_pst = "branch_c_grow_pst",
			idle_full = "branch_c_idle_full",
			wilt_base = "branch_c_wilt",
			harvest = "branch_c_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_d_grow",
			grow_pst = "branch_d_grow_pst",
			idle_full = "branch_d_idle_full",
			wilt_base = "branch_d_wilt",
			harvest = "branch_d_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_e_grow",
			grow_pst = "branch_e_grow_pst",
			idle_full = "branch_e_idle_full",
			wilt_base = "branch_e_wilt",
			harvest = "branch_e_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_f_grow",
			grow_pst = "branch_f_grow_pst",
			idle_full = "branch_f_idle_full",
			wilt_base = "branch_f_wilt",
			harvest = "branch_f_harvest"
		},
		new StandardCropPlant.AnimSet
		{
			grow = "branch_g_grow",
			grow_pst = "branch_g_grow_pst",
			idle_full = "branch_g_idle_full",
			wilt_base = "branch_g_wilt",
			harvest = "branch_g_harvest"
		}
	};

	private static Vector3[] animOffset = new Vector3[]
	{
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, -1f, 0f),
		new Vector3(1f, -2f, 0f),
		new Vector3(0f, -2f, 0f),
		new Vector3(-1f, -2f, 0f),
		new Vector3(-1f, -1f, 0f),
		new Vector3(-1f, 0f, 0f)
	};

	private static readonly EventSystem.IntraObjectHandler<TreeBud> OnHarvestDelegate = new EventSystem.IntraObjectHandler<TreeBud>(delegate(TreeBud component, object data)
	{
		component.OnHarvest(data);
	});
}
