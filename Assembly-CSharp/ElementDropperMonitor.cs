using System;
using UnityEngine;

public class ElementDropperMonitor : GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.satisfied;
		this.root.EventHandler(GameHashes.DeathAnimComplete, delegate(ElementDropperMonitor.Instance smi)
		{
			smi.DropDeathElement();
		});
		this.satisfied.OnSignal(this.cellChangedSignal, this.readytodrop, (ElementDropperMonitor.Instance smi) => smi.ShouldDropElement());
		this.readytodrop.ToggleBehaviour(GameTags.Creatures.WantsToDropElements, (ElementDropperMonitor.Instance smi) => true, delegate(ElementDropperMonitor.Instance smi)
		{
			smi.GoTo(this.satisfied);
		}).EventHandler(GameHashes.ObjectMovementStateChanged, delegate(ElementDropperMonitor.Instance smi, object d)
		{
			if ((GameHashes)d == GameHashes.ObjectMovementWakeUp)
			{
				smi.GoTo(this.satisfied);
			}
		});
	}

	public GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.State satisfied;

	public GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.State readytodrop;

	public StateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.Signal cellChangedSignal;

	public class Def : StateMachine.BaseDef
	{
		public SimHashes dirtyEmitElement;

		public float dirtyProbabilityPercent;

		public float dirtyCellToTargetMass;

		public float dirtyMassPerDirty;

		public float dirtyMassReleaseOnDeath;

		public byte emitDiseaseIdx = byte.MaxValue;

		public float emitDiseasePerKg;
	}

	public new class Instance : GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ElementDropperMonitor.Def def)
			: base(master, def)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "ElementDropperMonitor.Instance");
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		}

		private void OnCellChange()
		{
			base.sm.cellChangedSignal.Trigger(this);
		}

		public bool ShouldDropElement()
		{
			return this.IsValidDropCell() && global::UnityEngine.Random.Range(0f, 100f) < base.def.dirtyProbabilityPercent;
		}

		public void DropDeathElement()
		{
			this.DropElement(base.def.dirtyMassReleaseOnDeath, base.def.dirtyEmitElement, base.def.emitDiseaseIdx, Mathf.RoundToInt(base.def.dirtyMassReleaseOnDeath * base.def.dirtyMassPerDirty));
		}

		public void DropPeriodicElement()
		{
			this.DropElement(base.def.dirtyMassPerDirty, base.def.dirtyEmitElement, base.def.emitDiseaseIdx, Mathf.RoundToInt(base.def.emitDiseasePerKg * base.def.dirtyMassPerDirty));
		}

		public void DropElement(float mass, SimHashes element_id, byte disease_idx, int disease_count)
		{
			if (mass <= 0f)
			{
				return;
			}
			Element element = ElementLoader.FindElementByHash(element_id);
			float temperature = base.GetComponent<PrimaryElement>().Temperature;
			if (element.IsGas || element.IsLiquid)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				SimMessages.AddRemoveSubstance(num, element_id, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, disease_idx, disease_count, true, -1);
			}
			else if (element.IsSolid)
			{
				element.substance.SpawnResource(base.transform.GetPosition() + new Vector3(0f, 0.5f, 0f), mass, temperature, disease_idx, disease_count, false, true);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, base.gameObject.transform, 1.5f, false);
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			return Grid.IsValidCell(num) && Grid.IsGas(num) && Grid.Mass[num] <= 1f;
		}
	}
}
