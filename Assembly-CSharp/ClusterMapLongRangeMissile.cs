using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ClusterMapLongRangeMissile : GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.initialization;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.ToggleTag(GameTags.EntityInSpace);
		this.initialization.Enter(delegate(ClusterMapLongRangeMissile.StatesInstance smi)
		{
			if (smi.exploded)
			{
				smi.GoTo(smi.sm.cleanup);
				return;
			}
			if (this.targetObject.Get(smi) != null)
			{
				smi.GoTo(smi.sm.travelling.moving);
				return;
			}
			smi.GoTo(smi.sm.contact);
		});
		this.travelling.ToggleStatusItem(Db.Get().MiscStatusItems.LongRangeMissileTTI, null).OnTargetLost(this.targetObject, this.contact).Target(this.targetObject)
			.EventHandler(GameHashes.ClusterLocationChanged, new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State.Callback(ClusterMapLongRangeMissile.UpdatePath))
			.Target(this.masterTarget);
		this.travelling.moving.ToggleTag(GameTags.LongRangeMissileMoving).EnterTransition(this.travelling.idle, (ClusterMapLongRangeMissile.StatesInstance smi) => !smi.IsTraveling()).EventTransition(GameHashes.ClusterDestinationReached, this.travelling.idle, null);
		this.travelling.idle.ToggleTag(GameTags.LongRangeMissileIdle).Transition(this.contact, new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Transition.ConditionCallback(ClusterMapLongRangeMissile.HitTarget), UpdateRate.SIM_1000ms).Transition(this.contact, GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Not(new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Transition.ConditionCallback(ClusterMapLongRangeMissile.CanHitTarget)), UpdateRate.SIM_1000ms);
		this.contact.Enter(new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State.Callback(ClusterMapLongRangeMissile.TriggerDamage)).EnterTransition(this.exploding_with_visual, new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Transition.ConditionCallback(ClusterMapLongRangeMissile.HasVisualizer)).EnterTransition(this.cleanup, GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Not(new StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.Transition.ConditionCallback(ClusterMapLongRangeMissile.HasVisualizer)));
		this.exploding_with_visual.ToggleTag(GameTags.LongRangeMissileExploding).EventTransition(GameHashes.RocketExploded, this.cleanup, null);
		this.cleanup.Enter(delegate(ClusterMapLongRangeMissile.StatesInstance smi)
		{
			smi.gameObject.DeleteObject();
		}).GoTo(null);
	}

	private static bool HasVisualizer(ClusterMapLongRangeMissile.StatesInstance smi)
	{
		return smi != null && ClusterMapScreen.Instance.GetEntityVisAnim(smi.GetComponent<ClusterGridEntity>()) != null;
	}

	public static void TriggerDamage(ClusterMapLongRangeMissile.StatesInstance smi)
	{
		GameObject gameObject = smi.sm.targetObject.Get(smi);
		if (gameObject != null && ClusterMapLongRangeMissile.CanHitTarget(smi))
		{
			gameObject.Trigger(-2056344675, MissileLongRangeConfig.DamageEventPayload.sharedInstance);
		}
		smi.exploded = true;
	}

	public static bool HitTarget(ClusterMapLongRangeMissile.StatesInstance smi)
	{
		ClusterGridEntity clusterGridEntity = smi.sm.targetObject.Get<ClusterGridEntity>(smi);
		return !(clusterGridEntity == null) && clusterGridEntity.Location == smi.sm.destinationHex.Get(smi);
	}

	public static bool CanHitTarget(ClusterMapLongRangeMissile.StatesInstance smi)
	{
		return smi.sm.targetObject.Get(smi) != null;
	}

	private static void UpdatePath(ClusterMapLongRangeMissile.StatesInstance smi)
	{
		ClusterDestinationSelector component = smi.GetComponent<ClusterDestinationSelector>();
		if (component == null)
		{
			return;
		}
		ClusterGridEntity clusterGridEntity = smi.sm.targetObject.Get<ClusterGridEntity>(smi);
		if (clusterGridEntity == null)
		{
			return;
		}
		ClusterGridEntity component2 = smi.GetComponent<ClusterGridEntity>();
		AxialI axialI = ClusterMapLongRangeMissile.StatesInstance.FindInterceptPoint(component2.Location, clusterGridEntity, component, 99999);
		if (axialI != smi.sm.destinationHex.Get(smi))
		{
			smi.Travel(component2.Location, axialI);
		}
	}

	public StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.TargetParameter targetObject;

	public StateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.AxialIParameter destinationHex;

	public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State initialization;

	public ClusterMapLongRangeMissile.TravellingStates travelling;

	public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State contact;

	public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State exploding_with_visual;

	public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State cleanup;

	public class Def : StateMachine.BaseDef
	{
	}

	public class TravellingStates : GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State
	{
		public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State moving;

		public GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.State idle;
	}

	public class StatesInstance : GameStateMachine<ClusterMapLongRangeMissile, ClusterMapLongRangeMissile.StatesInstance, IStateMachineTarget, ClusterMapLongRangeMissile.Def>.GameInstance
	{
		public StatesInstance(IStateMachineTarget master, ClusterMapLongRangeMissile.Def def)
			: base(master, def)
		{
			this.animController = base.GetComponent<KBatchedAnimController>();
		}

		public void Setup(AxialI source, ClusterGridEntity target, MissileLongRangeProjectile.Def projectile_def)
		{
			BallisticClusterGridEntity component = base.GetComponent<BallisticClusterGridEntity>();
			component.nameKey = new StringKey(projectile_def.missileName);
			base.GetComponent<InfoDescription>().description = Strings.Get(projectile_def.missileDesc);
			component.SwapSymbolFromSameAnim("payload", projectile_def.starmapOverrideSymbol);
			KAnim.Build.Symbol symbol = this.animController.AnimFiles[0].GetData().build.GetSymbol(projectile_def.starmapOverrideSymbol);
			this.animController.GetComponent<SymbolOverrideController>().AddSymbolOverride("payload", symbol, 0);
			base.sm.targetObject.Set(target.gameObject, this, false);
			this.Travel(source, ClusterMapLongRangeMissile.StatesInstance.FindInterceptPoint(source, target, base.GetComponent<ClusterDestinationSelector>(), 99999));
		}

		public static AxialI FindInterceptPoint(AxialI source, ClusterGridEntity target, ClusterDestinationSelector selector, int maxGridRange = 99999)
		{
			ClusterTraveler component = target.GetComponent<ClusterTraveler>();
			if (component != null)
			{
				List<AxialI> currentPath = component.CurrentPath;
				AxialI axialI = target.Location;
				foreach (AxialI axialI2 in currentPath)
				{
					float num = component.TravelETA(axialI2);
					List<AxialI> path = ClusterGrid.Instance.GetPath(source, axialI2, selector);
					if (path != null && path.Count != 0 && path.Count <= maxGridRange && (float)path.Count * 600f / 10f < num)
					{
						return axialI;
					}
					axialI = axialI2;
				}
			}
			return target.Location;
		}

		public float InterceptETA()
		{
			ClusterTraveler component = base.GetComponent<ClusterTraveler>();
			float num = 0f;
			float num2 = component.TravelETA();
			GameObject gameObject = base.sm.targetObject.Get(this);
			if (gameObject != null)
			{
				ClusterTraveler component2 = gameObject.GetComponent<ClusterTraveler>();
				if (component2 != null && component.CurrentPath != null)
				{
					num = component2.TravelETA(component.Destination);
				}
			}
			return Mathf.Max(num, num2);
		}

		public void Travel(AxialI source, AxialI destination)
		{
			base.GetComponent<BallisticClusterGridEntity>().Configure(source, destination);
			base.sm.destinationHex.Set(destination, this, false);
			this.GoTo(base.sm.travelling.moving);
		}

		public bool IsTraveling()
		{
			ClusterTraveler component = base.GetComponent<ClusterTraveler>();
			return component.CurrentPath != null && component.CurrentPath.Count != 0;
		}

		[Serialize]
		public bool exploded;

		public KBatchedAnimController animController;
	}
}
