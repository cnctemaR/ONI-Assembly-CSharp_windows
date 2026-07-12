using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class IlluminationVulnerable : StateMachineComponent<IlluminationVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, IIlluminationTracker
{
	public int LightIntensityThreshold
	{
		get
		{
			if (this.minLuxAttributeInstance != null)
			{
				return Mathf.RoundToInt(this.minLuxAttributeInstance.GetTotalValue());
			}
			return Mathf.RoundToInt(base.GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.MinLightLux));
		}
	}

	public string GetIlluminationUITooltip()
	{
		if ((this.prefersDarkness && this.IsComfortable()) || (!this.prefersDarkness && !this.IsComfortable()))
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_DARK;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_LIGHT;
	}

	public string GetIlluminationUILabel()
	{
		return Db.Get().Amounts.Illumination.Name + "\n    • " + (this.prefersDarkness ? UI.GAMEOBJECTEFFECTS.DARKNESS.ToString() : GameUtil.GetFormattedLux(this.LightIntensityThreshold));
	}

	public bool ShouldIlluminationUICheckboxBeChecked()
	{
		return this.IsComfortable();
	}

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
		this.minLuxAttributeInstance = base.gameObject.GetAttributes().Add(Db.Get().PlantAttributes.MinLightLux);
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
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		if (this.prefersDarkness)
		{
			return Grid.LightIntensity[cell] == 0;
		}
		return Grid.LightIntensity[cell] >= this.LightIntensityThreshold;
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
				return Db.Get().CreatureStatusItems.Crop_Too_Bright.GetName(this);
			}
			if (base.smi.IsInsideState(base.smi.sm.too_dark))
			{
				return Db.Get().CreatureStatusItems.Crop_Too_Dark.GetName(this);
			}
			return "";
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
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.LightIntensityThreshold)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.LightIntensityThreshold)), Descriptor.DescriptorType.Requirement, false)
		};
	}

	private OccupyArea _occupyArea;

	private SchedulerHandle handle;

	public bool prefersDarkness;

	private AttributeInstance minLuxAttributeInstance;

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
			this.root.Update("Illumination", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num = Grid.PosToCell(smi.master.gameObject);
				if (Grid.IsValidCell(num))
				{
					smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue((float)Grid.LightCount[num]);
					return;
				}
				smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue(0f);
			}, UpdateRate.SIM_1000ms, false);
			this.comfortable.Update("Illumination.Comfortable", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num2 = Grid.PosToCell(smi.master.gameObject);
				if (!smi.master.IsCellSafe(num2))
				{
					GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State state = (smi.master.prefersDarkness ? this.too_bright : this.too_dark);
					smi.GoTo(state);
				}
			}, UpdateRate.SIM_1000ms, false).Enter(delegate(IlluminationVulnerable.StatesInstance smi)
			{
				smi.master.Trigger(1113102781, null);
			});
			this.too_dark.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_dark", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num3 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(num3))
				{
					smi.GoTo(this.comfortable);
				}
			}, UpdateRate.SIM_1000ms, false);
			this.too_bright.TriggerOnEnter(GameHashes.IlluminationDiscomfort, null).Update("Illumination.too_bright", delegate(IlluminationVulnerable.StatesInstance smi, float dt)
			{
				int num4 = Grid.PosToCell(smi.master.gameObject);
				if (smi.master.IsCellSafe(num4))
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
