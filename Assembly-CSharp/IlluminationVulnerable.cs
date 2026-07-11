using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class IlluminationVulnerable : StateMachineComponent<IlluminationVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause
{
	private OccupyArea occupyArea
	{
		get
		{
			if (this._occupyArea == null)
			{
				this._occupyArea = base.GetComponent<OccupyArea>();
			}
			return this._occupyArea;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Illumination, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetPrefersDarkness(bool prefersDarkness = false)
	{
		this.prefersDarkness = prefersDarkness;
	}

	protected override void OnCleanUp()
	{
		this.handle.ClearScheduler();
		base.OnCleanUp();
	}

	public bool IsCellSafe(int cell)
	{
		if (this.prefersDarkness)
		{
			return (float)Grid.LightIntensity[cell] <= this.lightIntensityThreshold;
		}
		return (float)Grid.LightIntensity[cell] > this.lightIntensityThreshold;
	}

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[]
			{
				WiltCondition.Condition.Darkness,
				WiltCondition.Condition.IlluminationComfort
			};
		}
	}

	public string WiltStateString
	{
		get
		{
			if (base.smi.IsInsideState(base.smi.sm.too_bright))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Bright.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_BRIGHT.NAME, this);
			}
			if (base.smi.IsInsideState(base.smi.sm.too_dark))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Dark.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_DARK.NAME, this);
			}
			return string.Empty;
		}
	}

	public bool IsComfortable()
	{
		return base.smi.IsInsideState(base.smi.sm.comfortable);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		if (this.prefersDarkness)
		{
			return new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, Descriptor.DescriptorType.Requirement, false)
			};
		}
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT, Descriptor.DescriptorType.Requirement, false)
		};
	}

	public float lightIntensityThreshold;

	private OccupyArea _occupyArea;

	private SchedulerHandle handle;

	public bool prefersDarkness;

	public class StatesInstance : GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.GameInstance
	{
		public StatesInstance(IlluminationVulnerable master)
			: base(master)
		{
		}

		public bool hasMaturity;
	}

	public class States : GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.comfortable;
			this.root.Update("Illumination.Comfortable", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue((float)Grid.LightCount[Grid.PosToCell(smi.master.gameObject)]);
			}, UpdateRate.SIM_1000ms, false);
			this.comfortable.Update("Illumination.Comfortable", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num = Grid.PosToCell(smi.master.gameObject);
				if (!smi.master.IsCellSafe(num))
				{
					GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State state = ((!smi.master.prefersDarkness) ? this.too_dark : this.too_bright);
					smi.GoTo(state);
				}
			}, UpdateRate.SIM_1000ms, false).Enter(delegate(IlluminationVulnerable.StatesInstance smi)
			{
				smi.master.Trigger(1113102781, null);
			});
			this.too_dark.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_dark", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num2 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(num2))
				{
					smi.GoTo(this.comfortable);
				}
			}, UpdateRate.SIM_1000ms, false);
			this.too_bright.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_bright", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num3 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(num3))
				{
					smi.GoTo(this.comfortable);
				}
			}, UpdateRate.SIM_1000ms, false);
		}

		public StateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.BoolParameter illuminated;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State comfortable;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_dark;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_bright;
	}
}
