using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ArtifactPOIStates")]
public class ArtifactPOIStates : GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.enter;
		this.root.Enter(delegate(ArtifactPOIStates.Instance smi)
		{
			if (smi.configuration == null || smi.configuration.typeId == HashedString.Invalid)
			{
				smi.configuration = smi.GetComponent<ArtifactPOIConfigurator>().MakeConfiguration();
				smi.poiCharge = 1f;
			}
		});
		this.enter.ParamTransition<float>(this.poiCharge, this.spawnArtifact, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>.Callback(ArtifactPOIStates.IsFullyCharged)).ParamTransition<float>(this.poiCharge, this.waitingForPickup, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>.Callback(ArtifactPOIStates.IsNotFullyCharge));
		this.spawnArtifact.Enter(new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State.Callback(ArtifactPOIStates.SpawnArtifactOnHexCellIfFullyCharged)).EnterGoTo(this.waitingForPickup);
		this.waitingForPickup.OnSignal(this.OnHexCellInventoryChangedSignal, this.recharging, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.SignalParameter>.Callback(ArtifactPOIStates.ThereIsNoArtifactInHexCell)).EnterTransition(this.destroyOnArtifactSpawned, (ArtifactPOIStates.Instance smi) => ArtifactPOIStates.MarkedForDestroyAfterArtifactSpawned(smi) && ArtifactPOIStates.IsArtifactAvailableInHexCell(smi));
		this.recharging.OnSignal(this.OnHexCellInventoryChangedSignal, this.waitingForPickup, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.SignalParameter>.Callback(ArtifactPOIStates.IsArtifactAvailableInHexCell)).ParamTransition<float>(this.poiCharge, this.spawnArtifact, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Parameter<float>.Callback(ArtifactPOIStates.IsFullyCharged)).EventHandler(GameHashes.NewDay, (ArtifactPOIStates.Instance smi) => GameClock.Instance, new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State.Callback(ArtifactPOIStates.AddDayWothOfCharge));
		this.destroyOnArtifactSpawned.Enter(new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State.Callback(ArtifactPOIStates.SelfDestroy));
	}

	public static bool IsNotFullyCharge(ArtifactPOIStates.Instance smi, float f)
	{
		return !ArtifactPOIStates.IsFullyCharge(smi);
	}

	public static bool IsNotFullyCharge(ArtifactPOIStates.Instance smi)
	{
		return !ArtifactPOIStates.IsFullyCharge(smi);
	}

	public static bool IsFullyCharge(ArtifactPOIStates.Instance smi)
	{
		return smi.sm.poiCharge.Get(smi) >= 1f;
	}

	public static bool IsFullyCharged(ArtifactPOIStates.Instance smi, float f)
	{
		return smi.sm.poiCharge.Get(smi) >= 1f;
	}

	public static bool ThereIsNoArtifactInHexCell(ArtifactPOIStates.Instance smi, StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.SignalParameter param)
	{
		return ArtifactPOIStates.ThereIsNoArtifactInHexCell(smi);
	}

	public static bool ThereIsNoArtifactInHexCell(ArtifactPOIStates.Instance smi)
	{
		return !smi.HasArtifactAvailableInHexCell();
	}

	public static bool IsArtifactAvailableInHexCell(ArtifactPOIStates.Instance smi, StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.SignalParameter param)
	{
		return ArtifactPOIStates.IsArtifactAvailableInHexCell(smi);
	}

	public static bool IsArtifactAvailableInHexCell(ArtifactPOIStates.Instance smi)
	{
		return smi.HasArtifactAvailableInHexCell();
	}

	public static bool MarkedForDestroyAfterArtifactSpawned(ArtifactPOIStates.Instance smi)
	{
		return smi.configuration.DestroyOnHarvest();
	}

	public static void ResetRechargeProgress(ArtifactPOIStates.Instance smi)
	{
		smi.poiCharge = 0f;
	}

	public static void IncreaseArtifactSpawnedCount(ArtifactPOIStates.Instance smi)
	{
		smi.IncreaseArtifactsSpawnedCount();
	}

	public static void SelfDestroy(ArtifactPOIStates.Instance smi)
	{
		smi.gameObject.DeleteObject();
	}

	public static void AddDayWothOfCharge(ArtifactPOIStates.Instance smi)
	{
		smi.RechargePOI(600f);
	}

	public static void SpawnArtifactOnHexCellIfFullyCharged(ArtifactPOIStates.Instance smi)
	{
		if (ArtifactPOIStates.IsFullyCharge(smi))
		{
			smi.SpawnArtifactOnHexCell();
			ArtifactPOIStates.ResetRechargeProgress(smi);
			ArtifactPOIStates.IncreaseArtifactSpawnedCount(smi);
		}
	}

	public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State destroyOnArtifactSpawned;

	public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State enter;

	public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State waitingForPickup;

	public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State recharging;

	public GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.State spawnArtifact;

	public StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.Signal OnHexCellInventoryChangedSignal;

	public StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.FloatParameter poiCharge = new StateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.FloatParameter(1f);

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ArtifactPOIStates, ArtifactPOIStates.Instance, IStateMachineTarget, ArtifactPOIStates.Def>.GameInstance, IGameObjectEffectDescriptor
	{
		public StarmapHexCellInventory HexCellInventory
		{
			get
			{
				return this.GetHexCellInventory();
			}
		}

		public void IncreaseArtifactsSpawnedCount()
		{
			this.numHarvests++;
		}

		public float poiCharge
		{
			get
			{
				return this._poiCharge;
			}
			set
			{
				this._poiCharge = value;
				base.smi.sm.poiCharge.Set(value, base.smi, false);
			}
		}

		public Instance(IStateMachineTarget target, ArtifactPOIStates.Def def)
			: base(target, def)
		{
		}

		public override void StartSM()
		{
			this.HexCellInventory.Subscribe(-1697596308, new Action<object>(this.OnHexCellInventoryChanged));
			base.StartSM();
		}

		protected override void OnCleanUp()
		{
			this.HexCellInventory.Unsubscribe(-1697596308, new Action<object>(this.OnHexCellInventoryChanged));
			base.OnCleanUp();
		}

		private void OnHexCellInventoryChanged(object o)
		{
			base.sm.OnHexCellInventoryChangedSignal.Trigger(this);
		}

		public StarmapHexCellInventory GetHexCellInventory()
		{
			ClusterGridEntity component = base.GetComponent<ClusterGridEntity>();
			return ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location);
		}

		public bool HasArtifactAvailableInHexCell()
		{
			return this.HexCellInventory.Items.Find((StarmapHexCellInventory.SerializedItem i) => i.IsEntity && Assets.GetPrefab(i.ID).HasTag(GameTags.Artifact)) != null;
		}

		public void SpawnArtifactOnHexCell()
		{
			Tag tag = ((this.artifactToHarvest != null) ? this.artifactToHarvest : this.PickNewArtifactToHarvest());
			this.artifactToHarvest = null;
			this.HexCellInventory.AddItem(tag, 1f, Element.State.Vacuum);
		}

		public string PickNewArtifactToHarvest()
		{
			string text;
			if (this.numHarvests <= 0 && !string.IsNullOrEmpty(this.configuration.GetArtifactID()))
			{
				text = this.configuration.GetArtifactID();
				ArtifactSelector.Instance.ReserveArtifactID(text, ArtifactType.Any);
			}
			else
			{
				text = ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Space);
			}
			return text;
		}

		public void RechargePOI(float dt)
		{
			float num = dt / this.configuration.GetRechargeTime();
			this.poiCharge += num;
			this.poiCharge = Mathf.Min(1f, this.poiCharge);
		}

		public float RechargeTimeRemaining()
		{
			return (float)Mathf.CeilToInt((this.configuration.GetRechargeTime() - this.configuration.GetRechargeTime() * this.poiCharge) / 600f) * 600f;
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return new List<Descriptor>();
		}

		[Serialize]
		public ArtifactPOIConfigurator.ArtifactPOIInstanceConfiguration configuration;

		[Serialize]
		private float _poiCharge;

		[Serialize]
		private int numHarvests;

		[Serialize]
		public string artifactToHarvest;
	}
}
