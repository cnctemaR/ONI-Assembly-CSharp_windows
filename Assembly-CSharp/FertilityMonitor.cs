using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FertilityMonitor : GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.fertile;
		base.serializable = true;
		this.root.DefaultState(this.fertile);
		this.fertile.ToggleBehaviour(GameTags.Creatures.Fertile, (FertilityMonitor.Instance smi) => smi.IsReadyToLayEgg(), null).ToggleEffect((FertilityMonitor.Instance smi) => smi.fertileEffect).Transition(this.infertile, GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Not(new StateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Transition.ConditionCallback(FertilityMonitor.IsFertile)), UpdateRate.SIM_1000ms);
		this.infertile.Transition(this.fertile, new StateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.Transition.ConditionCallback(FertilityMonitor.IsFertile), UpdateRate.SIM_1000ms);
	}

	public static bool IsFertile(FertilityMonitor.Instance smi)
	{
		return !smi.HasTag(GameTags.Creatures.Confined) && !smi.HasTag(GameTags.Creatures.Expecting);
	}

	private GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State fertile;

	private GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.State infertile;

	[Serializable]
	public class BreedingChance
	{
		public Tag egg;

		public float weight;
	}

	public class Def : StateMachine.BaseDef
	{
		public override void Configure(GameObject prefab)
		{
			prefab.AddOrGet<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Fertility.Id);
		}

		public Tag eggPrefab;

		public List<FertilityMonitor.BreedingChance> initialBreedingWeights;

		public float baseFertileCycles;
	}

	public new class Instance : GameStateMachine<FertilityMonitor, FertilityMonitor.Instance, IStateMachineTarget, FertilityMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FertilityMonitor.Def def)
			: base(master, def)
		{
			this.fertility = Db.Get().Amounts.Fertility.Lookup(base.gameObject);
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				this.fertility.deltaAttribute.Add(new AttributeModifier(this.fertility.deltaAttribute.Id, 33.333332f, null, false, false, true));
			}
			float num = 100f / (def.baseFertileCycles * 600f);
			this.fertileEffect = new Effect("Fertile", CREATURES.MODIFIERS.BASE_FERTILITY.NAME, CREATURES.MODIFIERS.BASE_FERTILITY.TOOLTIP, 0f, false, false, false, null, 0f, null);
			this.fertileEffect.Add(new AttributeModifier(Db.Get().Amounts.Fertility.deltaAttribute.Id, num, CREATURES.MODIFIERS.BASE_FERTILITY.NAME, false, false, true));
			this.breedingChances = new List<FertilityMonitor.BreedingChance>();
			if (def.initialBreedingWeights != null)
			{
				foreach (FertilityMonitor.BreedingChance breedingChance in def.initialBreedingWeights)
				{
					this.breedingChances.Add(new FertilityMonitor.BreedingChance
					{
						egg = breedingChance.egg,
						weight = breedingChance.weight
					});
					List<FertilityModifier> forTag = Db.Get().FertilityModifiers.GetForTag(breedingChance.egg);
					foreach (FertilityModifier fertilityModifier in forTag)
					{
						fertilityModifier.ApplyFunction(this, breedingChance.egg);
					}
				}
				this.NormalizeBreedingChances();
			}
		}

		public void ShowEgg()
		{
			if (this.egg != null)
			{
				bool flag;
				Vector3 vector = base.GetComponent<KBatchedAnimController>().GetSymbolTransform(FertilityMonitor.Instance.targetEggSymbol, out flag).MultiplyPoint3x4(Vector3.zero);
				vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
				if (!Grid.Solid[Grid.PosToCell(vector)])
				{
					this.egg.transform.SetPosition(vector);
				}
				this.egg.SetActive(true);
				Db.Get().Amounts.Wildness.Copy(this.egg, base.gameObject);
				this.egg = null;
			}
		}

		public void LayEgg()
		{
			this.fertility.value = 0f;
			Vector3 position = base.smi.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			float num = global::UnityEngine.Random.value;
			Tag invalid = Tag.Invalid;
			foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
			{
				num -= breedingChance.weight;
				if (num <= 0f)
				{
					invalid = breedingChance.egg;
					break;
				}
			}
			if (GenericGameSettings.instance.acceleratedLifecycle)
			{
				float num2 = 0f;
				foreach (FertilityMonitor.BreedingChance breedingChance2 in this.breedingChances)
				{
					if (breedingChance2.weight > num2)
					{
						num2 = breedingChance2.weight;
						invalid = breedingChance2.egg;
					}
				}
			}
			GameObject prefab = Assets.GetPrefab(invalid);
			GameObject gameObject = Util.KInstantiate(prefab, position);
			this.egg = gameObject;
			SymbolOverrideController component = base.GetComponent<SymbolOverrideController>();
			string text = "egg01";
			IncubationMonitor.Def def = prefab.GetDef<IncubationMonitor.Def>();
			CreatureBrain component2 = Assets.GetPrefab(def.spawnedCreature).GetComponent<CreatureBrain>();
			if (!string.IsNullOrEmpty(component2.symbolPrefix))
			{
				text = component2.symbolPrefix + "egg01";
			}
			KAnim.Build.Symbol symbol = this.egg.GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(text);
			if (symbol != null)
			{
				component.AddSymbolOverride(FertilityMonitor.Instance.targetEggSymbol, symbol, 0);
			}
		}

		public bool IsReadyToLayEgg()
		{
			return base.smi.fertility.value >= base.smi.fertility.GetMax();
		}

		public void AddBreedingChance(Tag type, float addedPercentChance)
		{
			foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
			{
				if (breedingChance.egg == type)
				{
					float num = Mathf.Min(1f - breedingChance.weight, Mathf.Max(0f - breedingChance.weight, addedPercentChance));
					breedingChance.weight += num;
				}
			}
			this.NormalizeBreedingChances();
			base.master.Trigger(1059811075, this.breedingChances);
		}

		private void NormalizeBreedingChances()
		{
			float num = 0f;
			foreach (FertilityMonitor.BreedingChance breedingChance in this.breedingChances)
			{
				num += breedingChance.weight;
			}
			foreach (FertilityMonitor.BreedingChance breedingChance2 in this.breedingChances)
			{
				breedingChance2.weight /= num;
			}
		}

		public AmountInstance fertility;

		private GameObject egg;

		[Serialize]
		public List<FertilityMonitor.BreedingChance> breedingChances;

		public Effect fertileEffect;

		private static HashedString targetEggSymbol = "snapto_egg";
	}
}
