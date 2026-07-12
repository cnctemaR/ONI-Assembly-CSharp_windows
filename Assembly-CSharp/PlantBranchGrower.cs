using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class PlantBranchGrower : GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.wilt;
		this.worldgen.Update(new Action<PlantBranchGrower.Instance, float>(PlantBranchGrower.WorldGenUpdate), UpdateRate.RENDER_EVERY_TICK, false);
		this.wilt.TagTransition(GameTags.Wilting, this.maturing, true);
		this.maturing.TagTransition(GameTags.Wilting, this.wilt, false).EnterTransition(this.growingBranches, new StateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Transition.ConditionCallback(PlantBranchGrower.IsMature)).EventTransition(GameHashes.Grow, this.growingBranches, null);
		this.growingBranches.TagTransition(GameTags.Wilting, this.wilt, false).EventTransition(GameHashes.ConsumePlant, this.maturing, GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Not(new StateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Transition.ConditionCallback(PlantBranchGrower.IsMature))).EventTransition(GameHashes.TreeBranchCountChanged, this.fullyGrown, new StateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Transition.ConditionCallback(PlantBranchGrower.AllBranchesCreated))
			.ToggleStatusItem((PlantBranchGrower.Instance smi) => smi.def.growingBranchesStatusItem, null)
			.Update(new Action<PlantBranchGrower.Instance, float>(PlantBranchGrower.GrowBranchUpdate), UpdateRate.SIM_4000ms, false);
		this.fullyGrown.TagTransition(GameTags.Wilting, this.wilt, false).EventTransition(GameHashes.ConsumePlant, this.maturing, GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Not(new StateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Transition.ConditionCallback(PlantBranchGrower.IsMature))).EventTransition(GameHashes.TreeBranchCountChanged, this.growingBranches, new StateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.Transition.ConditionCallback(PlantBranchGrower.NotAllBranchesCreated));
	}

	public static bool NotAllBranchesCreated(PlantBranchGrower.Instance smi)
	{
		return smi.CurrentBranchCount < smi.MaxBranchesAllowedAtOnce;
	}

	public static bool AllBranchesCreated(PlantBranchGrower.Instance smi)
	{
		return smi.CurrentBranchCount >= smi.MaxBranchesAllowedAtOnce;
	}

	public static bool IsMature(PlantBranchGrower.Instance smi)
	{
		return smi.IsGrown;
	}

	public static void GrowBranchUpdate(PlantBranchGrower.Instance smi, float dt)
	{
		smi.SpawnRandomBranch(0f);
	}

	public static void WorldGenUpdate(PlantBranchGrower.Instance smi, float dt)
	{
		float num = global::UnityEngine.Random.Range(0f, 1f);
		if (!smi.SpawnRandomBranch(num))
		{
			smi.GoTo(smi.sm.defaultState);
		}
	}

	public GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.State worldgen;

	public GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.State wilt;

	public GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.State maturing;

	public GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.State growingBranches;

	public GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.State fullyGrown;

	public class Def : StateMachine.BaseDef
	{
		public string BRANCH_PREFAB_NAME;

		public int MAX_BRANCH_COUNT = -1;

		public CellOffset[] BRANCH_OFFSETS;

		public bool harvestOnDrown;

		public bool propagateHarvestDesignation = true;

		public Func<int, bool> additionalBranchGrowRequirements;

		public Action<PlantBranch.Instance, PlantBranchGrower.Instance> onBranchHarvested;

		public Action<PlantBranch.Instance, PlantBranchGrower.Instance> onBranchSpawned;

		public StatusItem growingBranchesStatusItem = Db.Get().MiscStatusItems.GrowingBranches;

		public Action<PlantBranchGrower.Instance> onEarlySpawn;
	}

	public new class Instance : GameStateMachine<PlantBranchGrower, PlantBranchGrower.Instance, IStateMachineTarget, PlantBranchGrower.Def>.GameInstance
	{
		public bool IsUprooted
		{
			get
			{
				return this.uprootMonitor != null && this.uprootMonitor.IsUprooted;
			}
		}

		public bool IsGrown
		{
			get
			{
				return this.growing == null || this.growing.PercentGrown() >= 1f;
			}
		}

		public int MaxBranchesAllowedAtOnce
		{
			get
			{
				if (base.def.MAX_BRANCH_COUNT >= 0)
				{
					return Mathf.Min(base.def.MAX_BRANCH_COUNT, base.def.BRANCH_OFFSETS.Length);
				}
				return base.def.BRANCH_OFFSETS.Length;
			}
		}

		public int CurrentBranchCount
		{
			get
			{
				int num = 0;
				if (this.branches != null)
				{
					int i = 0;
					while (i < this.branches.Length)
					{
						num += ((this.GetBranch(i++) != null) ? 1 : 0);
					}
				}
				return num;
			}
		}

		public GameObject GetBranch(int idx)
		{
			if (this.branches != null && this.branches[idx] != null)
			{
				KPrefabID kprefabID = this.branches[idx].Get();
				if (kprefabID != null)
				{
					return kprefabID.gameObject;
				}
			}
			return null;
		}

		protected override void OnCleanUp()
		{
			this.SetTrunkOccupyingCellsAsPlant(false);
			base.OnCleanUp();
		}

		public Instance(IStateMachineTarget master, PlantBranchGrower.Def def)
			: base(master, def)
		{
			this.growing = base.GetComponent<IManageGrowingStates>();
			this.growing = ((this.growing != null) ? this.growing : base.gameObject.GetSMI<IManageGrowingStates>());
			this.SetTrunkOccupyingCellsAsPlant(true);
			base.Subscribe(1119167081, new Action<object>(this.OnNewGameSpawn));
			base.Subscribe(144050788, new Action<object>(this.OnUpdateRoom));
		}

		public override void StartSM()
		{
			base.StartSM();
			Action<PlantBranchGrower.Instance> onEarlySpawn = base.def.onEarlySpawn;
			if (onEarlySpawn != null)
			{
				onEarlySpawn(this);
			}
			this.DefineBranchArray();
			base.Subscribe(-216549700, new Action<object>(this.OnUprooted));
			base.Subscribe(-266953818, delegate(object obj)
			{
				this.UpdateAutoHarvestValue(null);
			});
			if (base.def.harvestOnDrown)
			{
				base.Subscribe(-750750377, new Action<object>(this.OnUprooted));
			}
		}

		private void OnUpdateRoom(object data)
		{
			if (this.branches == null)
			{
				return;
			}
			this.ActionPerBranch(delegate(GameObject branch)
			{
				branch.Trigger(144050788, data);
			});
		}

		private void SetTrunkOccupyingCellsAsPlant(bool doSet)
		{
			CellOffset[] occupiedCellsOffsets = base.GetComponent<OccupyArea>().OccupiedCellsOffsets;
			int num = Grid.PosToCell(base.gameObject);
			for (int i = 0; i < occupiedCellsOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, occupiedCellsOffsets[i]);
				if (doSet)
				{
					Grid.Objects[num2, 5] = base.gameObject;
				}
				else if (Grid.Objects[num2, 5] == base.gameObject)
				{
					Grid.Objects[num2, 5] = null;
				}
			}
		}

		private void OnNewGameSpawn(object data)
		{
			this.DefineBranchArray();
			float num = 1f;
			if ((double)global::UnityEngine.Random.value < 0.1)
			{
				num = global::UnityEngine.Random.Range(0.75f, 0.99f);
			}
			else
			{
				this.GoTo(base.sm.worldgen);
			}
			this.growing.OverrideMaturityLevel(num);
		}

		public void ManuallyDefineBranchArray(KPrefabID[] _branches)
		{
			this.DefineBranchArray();
			for (int i = 0; i < Mathf.Min(this.branches.Length, _branches.Length); i++)
			{
				KPrefabID kprefabID = _branches[i];
				if (kprefabID != null)
				{
					if (this.branches[i] == null)
					{
						this.branches[i] = new Ref<KPrefabID>();
					}
					this.branches[i].Set(kprefabID);
				}
				else
				{
					this.branches[i] = null;
				}
			}
		}

		private void DefineBranchArray()
		{
			if (this.branches == null)
			{
				this.branches = new Ref<KPrefabID>[base.def.BRANCH_OFFSETS.Length];
			}
		}

		public void ActionPerBranch(Action<GameObject> action)
		{
			for (int i = 0; i < this.branches.Length; i++)
			{
				GameObject branch = this.GetBranch(i);
				if (branch != null && action != null)
				{
					action(branch.gameObject);
				}
			}
		}

		public GameObject[] GetExistingBranches()
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.branches.Length; i++)
			{
				GameObject branch = this.GetBranch(i);
				if (branch != null)
				{
					list.Add(branch.gameObject);
				}
			}
			return list.ToArray();
		}

		public void OnBranchRemoved(GameObject _branch)
		{
			for (int i = 0; i < this.branches.Length; i++)
			{
				GameObject branch = this.GetBranch(i);
				if (branch != null && branch == _branch)
				{
					this.branches[i] = null;
				}
			}
			base.gameObject.Trigger(-1586842875, null);
		}

		public void OnBrancHarvested(PlantBranch.Instance branch)
		{
			Action<PlantBranch.Instance, PlantBranchGrower.Instance> onBranchHarvested = base.def.onBranchHarvested;
			if (onBranchHarvested == null)
			{
				return;
			}
			onBranchHarvested(branch, this);
		}

		private void OnUprooted(object data = null)
		{
			for (int i = 0; i < this.branches.Length; i++)
			{
				GameObject branch = this.GetBranch(i);
				if (branch != null)
				{
					branch.Trigger(-216549700, null);
				}
			}
		}

		public List<int> GetAvailableSpawnPositions()
		{
			PlantBranchGrower.Instance.spawn_choices.Clear();
			int num = Grid.PosToCell(this);
			for (int i = 0; i < base.def.BRANCH_OFFSETS.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, base.def.BRANCH_OFFSETS[i]);
				if (this.GetBranch(i) == null && this.CanBranchGrowInCell(num2))
				{
					PlantBranchGrower.Instance.spawn_choices.Add(i);
				}
			}
			return PlantBranchGrower.Instance.spawn_choices;
		}

		public void RefreshBranchZPositionOffset(GameObject _branch)
		{
			if (this.branches != null)
			{
				for (int i = 0; i < this.branches.Length; i++)
				{
					GameObject branch = this.GetBranch(i);
					if (branch != null && branch == _branch)
					{
						Vector3 position = branch.transform.position;
						position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront) - 0.8f / (float)this.branches.Length * (float)i;
						branch.transform.SetPosition(position);
					}
				}
			}
		}

		public bool SpawnRandomBranch(float growth_percentage = 0f)
		{
			if (this.IsUprooted)
			{
				return false;
			}
			if (this.CurrentBranchCount >= this.MaxBranchesAllowedAtOnce)
			{
				return false;
			}
			List<int> availableSpawnPositions = this.GetAvailableSpawnPositions();
			availableSpawnPositions.Shuffle<int>();
			if (availableSpawnPositions.Count > 0)
			{
				int num = availableSpawnPositions[0];
				PlantBranch.Instance instance = this.SpawnBranchAtIndex(num);
				IManageGrowingStates manageGrowingStates = instance.GetComponent<IManageGrowingStates>();
				manageGrowingStates = ((manageGrowingStates != null) ? manageGrowingStates : instance.gameObject.GetSMI<IManageGrowingStates>());
				if (manageGrowingStates != null)
				{
					manageGrowingStates.OverrideMaturityLevel(growth_percentage);
				}
				instance.StartSM();
				base.gameObject.Trigger(-1586842875, instance);
				Action<PlantBranch.Instance, PlantBranchGrower.Instance> onBranchSpawned = base.def.onBranchSpawned;
				if (onBranchSpawned != null)
				{
					onBranchSpawned(instance, this);
				}
				return true;
			}
			return false;
		}

		private PlantBranch.Instance SpawnBranchAtIndex(int idx)
		{
			if (idx < 0 || idx >= this.branches.Length)
			{
				return null;
			}
			GameObject branch = this.GetBranch(idx);
			if (branch != null)
			{
				return branch.GetSMI<PlantBranch.Instance>();
			}
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this), base.def.BRANCH_OFFSETS[idx]), Grid.SceneLayer.BuildingFront);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(base.def.BRANCH_PREFAB_NAME), vector);
			gameObject.SetActive(true);
			PlantBranch.Instance smi = gameObject.GetSMI<PlantBranch.Instance>();
			MutantPlant component = base.GetComponent<MutantPlant>();
			if (component != null)
			{
				MutantPlant component2 = smi.GetComponent<MutantPlant>();
				if (component2 != null)
				{
					component.CopyMutationsTo(component2);
					PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = component2.GetSubSpeciesInfo();
					PlantSubSpeciesCatalog.Instance.DiscoverSubSpecies(subSpeciesInfo, component2);
					PlantSubSpeciesCatalog.Instance.IdentifySubSpecies(subSpeciesInfo.ID);
				}
			}
			this.UpdateAutoHarvestValue(smi);
			smi.SetTrunk(this);
			this.branches[idx] = new Ref<KPrefabID>();
			this.branches[idx].Set(smi.GetComponent<KPrefabID>());
			return smi;
		}

		private bool CanBranchGrowInCell(int cell)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (Grid.Solid[cell])
			{
				return false;
			}
			if (Grid.Objects[cell, 1] != null)
			{
				return false;
			}
			if (Grid.Objects[cell, 5] != null)
			{
				return false;
			}
			if (Grid.Foundation[cell])
			{
				return false;
			}
			int num = Grid.CellAbove(cell);
			return Grid.IsValidCell(num) && !Grid.IsSubstantialLiquid(num, 0.35f) && (base.def.additionalBranchGrowRequirements == null || base.def.additionalBranchGrowRequirements(cell));
		}

		public void UpdateAutoHarvestValue(PlantBranch.Instance specificBranch = null)
		{
			HarvestDesignatable component = base.GetComponent<HarvestDesignatable>();
			if (component != null && this.branches != null)
			{
				if (specificBranch != null)
				{
					HarvestDesignatable component2 = specificBranch.GetComponent<HarvestDesignatable>();
					if (component2 != null)
					{
						component2.SetHarvestWhenReady(component.HarvestWhenReady);
					}
					return;
				}
				if (base.def.propagateHarvestDesignation)
				{
					for (int i = 0; i < this.branches.Length; i++)
					{
						GameObject branch = this.GetBranch(i);
						if (branch != null)
						{
							HarvestDesignatable component3 = branch.GetComponent<HarvestDesignatable>();
							if (component3 != null)
							{
								component3.SetHarvestWhenReady(component.HarvestWhenReady);
							}
						}
					}
				}
			}
		}

		private IManageGrowingStates growing;

		[MyCmpGet]
		private UprootedMonitor uprootMonitor;

		[Serialize]
		private Ref<KPrefabID>[] branches;

		private static List<int> spawn_choices = new List<int>();
	}
}
