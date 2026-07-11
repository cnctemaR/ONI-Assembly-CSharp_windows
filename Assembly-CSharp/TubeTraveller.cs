using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class TubeTraveller : GameStateMachine<TubeTraveller, TubeTraveller.Instance>, OxygenBreather.IGasProvider
{
	public void InitModifiers()
	{
		this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.Insulation.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_INSULATION, global::STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.ThermalConductivityBarrier.Id, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_THERMAL_CONDUCTIVITY_BARRIER, global::STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		this.modifiers.Add(new AttributeModifier(Db.Get().Amounts.Bladder.deltaAttribute.Id, global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_BLADDER, global::STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		this.modifiers.Add(new AttributeModifier(Db.Get().Attributes.ScaldingThreshold.Id, (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_SCALDING, global::STRINGS.BUILDINGS.PREFABS.TRAVELTUBE.NAME, false, false, true));
		this.immunities.Add(Db.Get().effects.Get("SoakingWet"));
		this.immunities.Add(Db.Get().effects.Get("WetFeet"));
		this.immunities.Add(Db.Get().effects.Get("PoppedEarDrums"));
		this.immunities.Add(Db.Get().effects.Get("Unclean"));
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		this.InitModifiers();
		default_state = this.root;
		this.root.DoNothing();
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
	{
		return true;
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}

	public bool ShouldStoreCO2()
	{
		return false;
	}

	private List<Effect> immunities = new List<Effect>();

	private List<AttributeModifier> modifiers = new List<AttributeModifier>();

	public new class Instance : GameStateMachine<TubeTraveller, TubeTraveller.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		public int prefabInstanceID
		{
			get
			{
				return base.GetComponent<Navigator>().gameObject.GetComponent<KPrefabID>().InstanceID;
			}
		}

		public void OnPathAdvanced(object data)
		{
			this.UnreserveEntrances();
			this.ReserveEntrances();
		}

		public void ReserveEntrances()
		{
			PathFinder.Path path = base.GetComponent<Navigator>().path;
			if (path.nodes == null)
			{
				return;
			}
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				if (path.nodes[i].navType == NavType.Floor && path.nodes[i + 1].navType == NavType.Tube)
				{
					int cell = path.nodes[i].cell;
					if (Grid.HasUsableTubeEntrance(cell, this.prefabInstanceID))
					{
						GameObject gameObject = Grid.Objects[cell, 1];
						if (gameObject)
						{
							TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
							if (component)
							{
								component.Reserve(this, this.prefabInstanceID);
								this.reservations.Add(component);
							}
						}
					}
				}
			}
		}

		public void UnreserveEntrances()
		{
			foreach (TravelTubeEntrance travelTubeEntrance in this.reservations)
			{
				if (!(travelTubeEntrance == null))
				{
					travelTubeEntrance.Unreserve(this, this.prefabInstanceID);
				}
			}
			this.reservations.Clear();
		}

		public void OnTubeTransition(bool nowInTube)
		{
			if (nowInTube != this.inTube)
			{
				this.inTube = nowInTube;
				Effects component = base.GetComponent<Effects>();
				Attributes attributes = base.gameObject.GetAttributes();
				if (nowInTube)
				{
					this.hadSuitTank = this.HasSuitTank();
					if (!this.hadSuitTank)
					{
						base.GetComponent<OxygenBreather>().SetGasProvider(base.sm);
					}
					foreach (Effect effect in base.sm.immunities)
					{
						component.AddImmunity(effect);
					}
					foreach (AttributeModifier attributeModifier in base.sm.modifiers)
					{
						attributes.Add(attributeModifier);
					}
				}
				else
				{
					if (!this.hadSuitTank)
					{
						base.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
					}
					foreach (Effect effect2 in base.sm.immunities)
					{
						component.RemoveImmunity(effect2);
					}
					foreach (AttributeModifier attributeModifier2 in base.sm.modifiers)
					{
						attributes.Remove(attributeModifier2);
					}
				}
				CreatureSimTemperatureTransfer component2 = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
				if (component2 != null)
				{
					component2.RefreshRegistration();
				}
			}
		}

		private bool HasSuitTank()
		{
			Equipment equipment = base.GetComponent<MinionIdentity>().GetEquipment();
			AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
			if (slot != null && slot.assignable != null)
			{
				SuitTank component = slot.assignable.GetComponent<SuitTank>();
				return component != null;
			}
			return false;
		}

		private List<TravelTubeEntrance> reservations = new List<TravelTubeEntrance>();

		private bool inTube;

		private bool hadSuitTank;
	}
}
