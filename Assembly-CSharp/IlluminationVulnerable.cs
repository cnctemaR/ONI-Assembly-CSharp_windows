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

	public void Configure(bool prefersDarkness = false)
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
		bool flag;
		if (this.prefersDarkness)
		{
			flag = Grid.LightCount[cell] == 0;
		}
		else
		{
			flag = Grid.LightCount[cell] > 0;
		}
		return flag;
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
			string text;
			if (base.smi.IsInsideState(base.smi.sm.too_bright))
			{
				text = Db.Get().CreatureStatusItems.Crop_Too_Bright.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_BRIGHT.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.too_dark))
			{
				text = Db.Get().CreatureStatusItems.Crop_Too_Dark.resolveStringCallback(CREATURES.STATUSITEMS.CROP_TOO_DARK.NAME, this);
			}
			else
			{
				text = "";
			}
			return text;
		}
	}

	public bool IsComfortable()
	{
		return base.smi.IsInsideState(base.smi.sm.comfortable);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list;
		if (this.prefersDarkness)
		{
			list = new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, Descriptor.DescriptorType.Requirement, false)
			};
		}
		else
		{
			list = new List<Descriptor>
			{
				new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT, Descriptor.DescriptorType.Requirement, false)
			};
		}
		return list;
	}

	private OccupyArea _occupyArea;

	private SchedulerHandle handle;

	public bool prefersDarkness = false;

	public class StatesInstance : GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.GameInstance
	{
		public StatesInstance(IlluminationVulnerable master)
			: base(master)
		{
		}

		public bool hasMaturity = false;
	}

	public class States : GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.comfortable;
			this.root.Update(delegate(IlluminationVulnerable.StatesInstance smi)
			{
				smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue((float)Grid.LightCount[Grid.PosToCell(smi.master.gameObject)]);
			});
			this.comfortable.Transition(this.too_bright, (IlluminationVulnerable.StatesInstance smi) => smi.master.prefersDarkness && !smi.master.IsCellSafe(Grid.PosToCell(smi.master.gameObject))).Transition(this.too_dark, (IlluminationVulnerable.StatesInstance smi) => !smi.master.prefersDarkness && !smi.master.IsCellSafe(Grid.PosToCell(smi.master.gameObject))).Enter(delegate(IlluminationVulnerable.StatesInstance smi)
			{
				smi.master.Trigger(1113102781, null);
			});
			this.too_dark.Transition(this.comfortable, (IlluminationVulnerable.StatesInstance smi) => smi.master.IsCellSafe(Grid.PosToCell(smi.master.gameObject))).TriggerOnEnter(GameHashes.IlluminationDiscomfort, null);
			this.too_bright.Transition(this.comfortable, (IlluminationVulnerable.StatesInstance smi) => smi.master.IsCellSafe(Grid.PosToCell(smi.master.gameObject))).TriggerOnEnter(GameHashes.IlluminationDiscomfort, null);
		}

		public StateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.BoolParameter illuminated;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State comfortable;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_dark;

		public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_bright;
	}
}
