using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIStates")]
public class HarvestablePOIStates : GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.root.Enter(delegate(HarvestablePOIStates.Instance smi)
		{
			if (smi.configuration == null || smi.configuration.typeId == HashedString.Invalid)
			{
				smi.configuration = smi.GetComponent<HarvestablePOIConfigurator>().MakeConfiguration();
				smi.poiCapacity = global::UnityEngine.Random.Range(0f, smi.configuration.GetMaxCapacity());
			}
		});
		this.idle.ParamTransition<float>(this.poiCapacity, this.recharging, (HarvestablePOIStates.Instance smi, float f) => f < smi.configuration.GetMaxCapacity());
		this.recharging.EventHandler(GameHashes.NewDay, (HarvestablePOIStates.Instance smi) => GameClock.Instance, delegate(HarvestablePOIStates.Instance smi)
		{
			smi.RechargePOI(600f);
		}).ParamTransition<float>(this.poiCapacity, this.idle, (HarvestablePOIStates.Instance smi, float f) => f >= smi.configuration.GetMaxCapacity());
	}

	public GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State idle;

	public GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.State recharging;

	public StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.FloatParameter poiCapacity = new StateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.FloatParameter(1f);

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<HarvestablePOIStates, HarvestablePOIStates.Instance, IStateMachineTarget, HarvestablePOIStates.Def>.GameInstance, IGameObjectEffectDescriptor
	{
		public float poiCapacity
		{
			get
			{
				return this._poiCapacity;
			}
			set
			{
				this._poiCapacity = value;
				base.smi.sm.poiCapacity.Set(value, base.smi, false);
			}
		}

		public Instance(IStateMachineTarget target, HarvestablePOIStates.Def def)
			: base(target, def)
		{
		}

		public void RechargePOI(float dt)
		{
			float num = dt / this.configuration.GetRechargeTime();
			float num2 = this.configuration.GetMaxCapacity() * num;
			this.DeltaPOICapacity(num2);
		}

		public void DeltaPOICapacity(float delta)
		{
			this.poiCapacity += delta;
			this.poiCapacity = Mathf.Min(this.configuration.GetMaxCapacity(), this.poiCapacity);
		}

		public bool POICanBeHarvested()
		{
			return this.poiCapacity > 0f;
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			foreach (KeyValuePair<SimHashes, float> keyValuePair in this.configuration.GetElementsWithWeights())
			{
				SimHashes key = keyValuePair.Key;
				string text = ElementLoader.FindElementByHash(key).tag.ProperName();
				list.Add(new Descriptor(string.Format(UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION, text), string.Format(UI.SPACEDESTINATIONS.HARVESTABLE_POI.POI_PRODUCTION_TOOLTIP, key.ToString()), Descriptor.DescriptorType.Effect, false));
			}
			list.Add(new Descriptor(string.Format("{0}/{1}", GameUtil.GetFormattedMass(this.poiCapacity, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), GameUtil.GetFormattedMass(this.configuration.GetMaxCapacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), "Capacity", Descriptor.DescriptorType.Effect, false));
			return list;
		}

		[Serialize]
		public HarvestablePOIConfigurator.HarvestablePOIInstanceConfiguration configuration;

		[Serialize]
		private float _poiCapacity;
	}
}
