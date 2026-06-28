using System;
using System.Collections.Generic;
using FileHelpers;
using Klei;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Upgradable : Workable, ISaveLoadable
{
	private Upgradable()
	{
		this.upgrades = new Dictionary<int, Upgradable.Upgrade>();
	}

	public BuildingDef GetBuildingDef
	{
		get
		{
			return this.building.Def;
		}
	}

	public Dictionary<int, Upgradable.Upgrade> upgrades { get; private set; }

	public Upgradable.Upgrade.Target UpgradeBitmask
	{
		get
		{
			Upgradable.Upgrade.Target target = Upgradable.Upgrade.Target.None;
			if (this.currentUpgrades != null)
			{
				foreach (int num in this.currentUpgrades)
				{
					if (this.upgrades.ContainsKey(num))
					{
						Upgradable.Upgrade upgrade = this.upgrades[num];
						target |= upgrade.type;
					}
				}
			}
			return target;
		}
	}

	public bool IsUpgraded(Upgradable.Upgrade.Target type)
	{
		bool flag;
		if (this.upgrades == null || this.upgrades.Count == 0)
		{
			flag = false;
		}
		else if (this.currentUpgrades == null || this.currentUpgrades.Count == 0)
		{
			flag = false;
		}
		else
		{
			bool flag2 = false;
			foreach (KeyValuePair<int, Upgradable.Upgrade> keyValuePair in this.upgrades)
			{
				if (keyValuePair.Value.type == type)
				{
					flag2 = this.currentUpgrades.Contains(keyValuePair.Key);
					break;
				}
			}
			flag = flag2;
		}
		return flag;
	}

	public bool CanUpgrade(Upgradable.Upgrade.Target type)
	{
		bool flag;
		if (this.upgrades == null || this.upgrades.Count == 0)
		{
			flag = false;
		}
		else
		{
			bool flag2 = false;
			foreach (Upgradable.Upgrade upgrade in this.upgrades.Values)
			{
				if (upgrade.type == type)
				{
					flag2 = true;
					break;
				}
			}
			flag = flag2;
		}
		return flag;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.faceTargetWhenWorking = true;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Upgrading;
		this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
	}

	public void ApplyCurrentUpgrades(KBatchedAnimController animControl)
	{
		List<Upgradable.UpgradableConfig> upgradesForBuilding = Upgradable.GetUpgradesForBuilding(this.building.PrefabID());
		for (int i = 0; i < upgradesForBuilding.Count; i++)
		{
			if (this.currentUpgrades.Contains(upgradesForBuilding[i].id))
			{
				this.ShowUpgradeFolder(animControl, upgradesForBuilding[i].id);
			}
		}
	}

	protected override void OnSpawn()
	{
		List<Upgradable.UpgradableConfig> upgradesForBuilding = Upgradable.GetUpgradesForBuilding(this.building.PrefabID());
		if (upgradesForBuilding.Count == 0)
		{
			global::UnityEngine.Object.Destroy(this);
		}
		else
		{
			for (int i = 0; i < upgradesForBuilding.Count; i++)
			{
				int num = this.AddUpgrade(upgradesForBuilding[i], null, null);
				if (!this.currentUpgrades.Contains(num))
				{
					this.availableUpgrades.Add(num);
					this.HideUpgradeFolder(this.animController, upgradesForBuilding[i].id);
				}
				else
				{
					this.ShowUpgradeFolder(this.animController, upgradesForBuilding[i].id);
				}
			}
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	protected override void OnCleanUp()
	{
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Cleanup");
		}
		base.OnCleanUp();
	}

	public void ShowUpgradeFolder(KBatchedAnimController animControl, int id)
	{
	}

	public void HideUpgradeFolder(KBatchedAnimController animControl, int id)
	{
	}

	public int AddUpgrade(Upgradable.UpgradableConfig upgradeConfig, Upgradable.FetchedCallback fetchedCB, Upgradable.CompleteCallback completeCB)
	{
		Upgradable.Upgrade upgrade = new Upgradable.Upgrade(upgradeConfig);
		upgrade.fetchedCB = fetchedCB;
		upgrade.completeCB = completeCB;
		this.upgrades.Add(upgradeConfig.id, upgrade);
		return upgradeConfig.id;
	}

	private float GetModifierForTarget(Upgradable.Upgrade.Target target)
	{
		return this.GetModifier(target, this.currentUpgrades);
	}

	public float GetNextModifierForTarget(Upgradable.Upgrade.Target target)
	{
		return this.GetModifier(target, this.availableUpgrades);
	}

	private float GetModifier(Upgradable.Upgrade.Target target, HashSet<int> targetUpgrades)
	{
		float num = 1f;
		foreach (KeyValuePair<int, Upgradable.Upgrade> keyValuePair in this.upgrades)
		{
			if (targetUpgrades.Contains(keyValuePair.Key))
			{
				Dictionary<int, Upgradable.Upgrade>.Enumerator enumerator;
				KeyValuePair<int, Upgradable.Upgrade> keyValuePair2 = enumerator.Current;
				foreach (Upgradable.UpgradableConfig.UpgradeModifier upgradeModifier in keyValuePair2.Value.config.modifiers)
				{
					if (upgradeModifier.modifier == target)
					{
						num *= upgradeModifier.modifierAmount;
					}
				}
			}
		}
		return num;
	}

	public float GetEnergyConsumptionMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.EnergyConsumption);
	}

	public float GetEnergyGenerationMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.EnergyGeneration);
	}

	public float GetLiquidConsumptionMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.MassConsumption);
	}

	public float GetMassGenerationMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.MassGeneration);
	}

	public float GetTemperatureUpgradeMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.HeatGeneration);
	}

	public float GetCapacityUpgradeMultiplier()
	{
		return this.GetModifierForTarget(Upgradable.Upgrade.Target.Capacity);
	}

	public void DoUpgrade(Upgradable.Upgrade upgrade, Recipe.Ingredient ingredient)
	{
		if (DebugHandler.InstantBuildMode)
		{
			this.currentUpgrade = upgrade;
			this.DoUpgradeComplete(upgrade);
		}
		else
		{
			if (upgrade.fetchList != null)
			{
				upgrade.fetchList.Cancel("Pending upgrade");
				upgrade.fetchList = null;
			}
			base.SetWorkTime(upgrade.buildTime);
			this.operational.SetFlag(Upgradable.notUpgradingFlag, false);
			upgrade.fetchList = new FetchList2(this.storage);
			upgrade.fetchList.Add(ingredient.tag, null, ingredient.amount, FetchOrder2.OperationalRequirement.None);
			this.userMenu.Refresh();
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingUpgrade, this);
			upgrade.fetchList.Submit(delegate
			{
				this.DoFetchComplete(upgrade);
			}, true);
			this.currentUpgrade = upgrade;
			if (this.currentUpgrade.startCB != null)
			{
				this.currentUpgrade.startCB();
			}
		}
	}

	private void Update()
	{
		if (this.currentUpgrade != null)
		{
			this.currentUpgrade.progress = this.GetPercentComplete();
		}
	}

	public bool IsCurrentUpgrade(Upgradable.Upgrade up)
	{
		return up == this.currentUpgrade;
	}

	public bool IsCurrentUpgrade(Upgradable.Upgrade.Target type)
	{
		return this.currentUpgrade != null && this.currentUpgrade.type == type;
	}

	public void CancelCurrentUpgrade()
	{
		if (this.currentUpgrade.fetchList != null)
		{
			this.currentUpgrade.fetchList.Cancel("User cancelled");
			this.currentUpgrade.fetchList = null;
		}
		if (this.currentUpgrade.buildChores != null)
		{
			foreach (WorkChore<Upgradable> workChore in this.currentUpgrade.buildChores)
			{
				workChore.Cancel("User cancelled");
			}
		}
		this.currentUpgrade.progress = 0f;
		this.operational.SetFlag(Upgradable.notUpgradingFlag, true);
		if (this.currentUpgrade.cancelCB != null)
		{
			this.currentUpgrade.cancelCB();
		}
		this.currentUpgrade = null;
		base.ShowProgressBar(false);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingUpgrade, false);
	}

	private void DoFetchComplete(Upgradable.Upgrade upgrade)
	{
		if (upgrade.fetchList != null)
		{
			upgrade.fetchList.Cancel("Fetch complete");
			upgrade.fetchList = null;
		}
		this.DoGenerateBuildChores(upgrade);
	}

	private void DoGenerateBuildChores(Upgradable.Upgrade upgrade)
	{
		Upgradable.<DoGenerateBuildChores>c__AnonStorey1 <DoGenerateBuildChores>c__AnonStorey = new Upgradable.<DoGenerateBuildChores>c__AnonStorey1();
		<DoGenerateBuildChores>c__AnonStorey.upgrade = upgrade;
		<DoGenerateBuildChores>c__AnonStorey.$this = this;
		<DoGenerateBuildChores>c__AnonStorey.upgrade.buildChoresRemain = <DoGenerateBuildChores>c__AnonStorey.upgrade.builderCount;
		<DoGenerateBuildChores>c__AnonStorey.upgrade.buildChores = new WorkChore<Upgradable>[<DoGenerateBuildChores>c__AnonStorey.upgrade.buildChoresRemain];
		int i;
		for (i = 0; i < <DoGenerateBuildChores>c__AnonStorey.upgrade.buildChoresRemain; i++)
		{
			<DoGenerateBuildChores>c__AnonStorey.upgrade.buildChores[i] = new WorkChore<Upgradable>(Db.Get().ChoreTypes.Upgrade, this, null, true, delegate
			{
				<DoGenerateBuildChores>c__AnonStorey.$this.DoBuildChoreComplete(<DoGenerateBuildChores>c__AnonStorey.upgrade, i);
			}, null, null, true, null, false, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
		}
	}

	private void DoBuildChoreComplete(Upgradable.Upgrade upgrade, int taskIdx)
	{
		upgrade.buildChoresRemain--;
		if (upgrade.buildChoresRemain <= 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
			for (int i = 0; i < upgrade.ingredients.Count; i++)
			{
				SimUtil.DiseaseInfo diseaseInfo2;
				float num;
				this.storage.ConsumeAndGetDisease(upgrade.ingredients[i], out diseaseInfo2, out num);
				diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo, diseaseInfo2);
			}
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			component.AddDisease(diseaseInfo.idx, diseaseInfo.count, "Upgradable.DoBuildChoreComplete");
			this.DoUpgradeComplete(upgrade);
			upgrade.buildChores = null;
		}
	}

	private void DoUpgradeComplete(Upgradable.Upgrade upgrade)
	{
		if (upgrade.completeCB != null)
		{
			upgrade.completeCB();
		}
		this.currentUpgrades.Add(upgrade.config.id);
		this.availableUpgrades.Remove(upgrade.config.id);
		this.ShowUpgradeFolder(this.animController, upgrade.config.id);
		this.userMenu.Refresh();
		this.operational.SetFlag(Upgradable.notUpgradingFlag, true);
		base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingUpgrade, false);
		if (upgrade == this.currentUpgrade)
		{
			this.currentUpgrade = null;
		}
		else
		{
			global::Debug.LogError("Something weird happened while trying to complete the upgrade.", null);
		}
		base.Trigger(-235298596, upgrade);
	}

	private static Upgradable.UpgradableConfig[] UpgradablesConfig
	{
		get
		{
			return Upgradable.upgradableConfigs.ToArray();
		}
	}

	public static void AddToUpgradableConfigs(Upgradable.UpgradableConfig newConfig)
	{
		Upgradable.UpgradableConfig upgradableConfig = Upgradable.upgradableConfigs.Find((Upgradable.UpgradableConfig uc) => uc.prefabID == newConfig.prefabID && uc.id == newConfig.id);
		if (upgradableConfig != null)
		{
			Upgradable.upgradableConfigs.Remove(upgradableConfig);
		}
		Upgradable.upgradableConfigs.Add(newConfig);
	}

	private static List<Upgradable.UpgradableConfig> GetUpgradesForBuilding(Tag kPrefabID)
	{
		List<Upgradable.UpgradableConfig> list = new List<Upgradable.UpgradableConfig>();
		for (int i = 0; i < Upgradable.UpgradablesConfig.Length; i++)
		{
			if (Upgradable.UpgradablesConfig[i].prefabID == kPrefabID.Name)
			{
				list.Add(Upgradable.UpgradablesConfig[i]);
			}
		}
		return list;
	}

	public static bool CanUpgrade(string buildingName, Upgradable.Upgrade.Target type)
	{
		for (int i = 0; i < Upgradable.UpgradablesConfig.Length; i++)
		{
			Upgradable.UpgradableConfig upgradableConfig = Upgradable.UpgradablesConfig[i];
			if (upgradableConfig.prefabID == buildingName)
			{
				foreach (Upgradable.UpgradableConfig.UpgradeModifier upgradeModifier in upgradableConfig.modifiers)
				{
					if (upgradeModifier.modifier == type)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MyCmpGet]
	private Building building;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpReq]
	private UserMenu userMenu;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private KBatchedAnimController animController;

	[Serialize]
	public HashSet<int> currentUpgrades = new HashSet<int>();

	[Serialize]
	public HashSet<int> availableUpgrades = new HashSet<int>();

	public static Operational.Flag notUpgradingFlag = new Operational.Flag("not_upgrading", Operational.Flag.Type.Requirement);

	private FetchList2 fetchList = null;

	private Upgradable.Upgrade currentUpgrade;

	private static List<Upgradable.UpgradableConfig> upgradableConfigs = new List<Upgradable.UpgradableConfig>();

	public delegate void FetchedCallback(object value);

	public delegate void StartCallback();

	public delegate void CompleteCallback();

	public delegate void CancelCallBack();

	public class Upgrade
	{
		public Upgrade(Upgradable.UpgradableConfig config)
		{
			this.config = config;
			Recipe.Ingredient ingredient = new Recipe.Ingredient(TagManager.Create(config.materialTags, null), (float)((int)config.materialMass));
			this.ingredients = new List<Recipe.Ingredient> { ingredient };
			this.builderCount = config.builderCount;
			this.buildTime = config.buildTime;
			this.id = config.id;
		}

		public Upgradable.UpgradableConfig config { get; private set; }

		public Upgradable.Upgrade.Target type
		{
			get
			{
				return this.config.GetModifierTarget(0);
			}
		}

		public List<Recipe.Ingredient> ingredients { get; private set; }

		public int builderCount { get; private set; }

		public float buildTime { get; private set; }

		public int id { get; private set; }

		public void ClearCallbacks()
		{
			this.fetchedCB = null;
			this.startCB = null;
			this.completeCB = null;
			this.cancelCB = null;
		}

		public Upgradable.FetchedCallback fetchedCB;

		public Upgradable.StartCallback startCB;

		public Upgradable.CompleteCallback completeCB;

		public Upgradable.CancelCallBack cancelCB;

		public float progress;

		public FetchList2 fetchList;

		public WorkChore<Upgradable>[] buildChores;

		public int buildChoresRemain;

		public enum Target
		{
			None,
			UNUSED,
			EnergyConsumption = 4,
			EnergyGeneration = 8,
			MassConsumption = 16,
			MassGeneration = 32,
			HeatGeneration = 64,
			Capacity = 128,
			ExecutionTime = 256
		}
	}

	[DelimitedRecord(",")]
	[IgnoreFirst(1)]
	[IgnoreEmptyLines]
	public class UpgradableConfig
	{
		public UpgradableConfig(string prefabID, int id, int builderCount, float buildTime, string materialTags, float materialMass, Upgradable.UpgradableConfig.UpgradeModifier[] modifiers)
		{
			this.prefabID = prefabID;
			this.id = id;
			this.builderCount = builderCount;
			this.buildTime = buildTime;
			this.materialTags = materialTags;
			this.materialMass = materialMass;
			this.modifiers = modifiers;
		}

		public string GetModifierString(int idx)
		{
			string text;
			if (idx < 0 || idx >= this.modifiers.Length)
			{
				text = "";
			}
			else if (this.modifiers[idx].modifier == Upgradable.Upgrade.Target.None)
			{
				text = "";
			}
			else
			{
				text = "";
			}
			return text;
		}

		public float GetModifierAmount(int idx)
		{
			float num;
			if (idx < 0 || idx > this.modifiers.Length)
			{
				num = 0f;
			}
			else
			{
				num = this.modifiers[idx].modifierAmount;
			}
			return num;
		}

		public Upgradable.Upgrade.Target GetModifierTarget(int idx)
		{
			Upgradable.Upgrade.Target target;
			if (idx < 0 || idx > this.modifiers.Length)
			{
				target = Upgradable.Upgrade.Target.None;
			}
			else
			{
				target = this.modifiers[idx].modifier;
			}
			return target;
		}

		public string prefabID;

		public int id;

		public int builderCount;

		public float buildTime;

		public string materialTags;

		public float materialMass;

		public Upgradable.UpgradableConfig.UpgradeModifier[] modifiers;

		public class UpgradeModifier
		{
			public UpgradeModifier(Upgradable.Upgrade.Target modifier, float modifierAmount)
			{
				this.modifier = modifier;
				this.modifierAmount = modifierAmount;
			}

			[FieldNullValue(Upgradable.Upgrade.Target.None)]
			public Upgradable.Upgrade.Target modifier;

			[FieldNullValue(0f)]
			public float modifierAmount;
		}
	}
}
