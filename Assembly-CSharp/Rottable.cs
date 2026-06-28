using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class Rottable : GameStateMachine<Rottable, Rottable.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Fresh;
		base.serializable = true;
		this.root.TagTransition(GameTags.Preserved, this.Preserved, false).TagTransition(GameTags.Entombed, this.Preserved, false);
		this.Fresh.ToggleStatusItem(Db.Get().CreatureStatusItems.Fresh, (Rottable.Instance smi) => smi).ParamTransition<float>(this.rotParameter, this.Stale_Pre, (Rottable.Instance smi, float p) => p <= smi.SpoilTime - (smi.SpoilTime - smi.StaleTime)).ToggleSchedulePeriodic("Rot", 1f, delegate(Rottable.Instance smi)
		{
			smi.Rot(smi, 1f);
		});
		this.Preserved.TagTransition(new Tag[]
		{
			GameTags.Preserved,
			GameTags.Entombed
		}, this.Fresh, true).Enter("RefreshModifiers", delegate(Rottable.Instance smi)
		{
			smi.RefreshModifiers(0f);
		});
		this.Stale_Pre.Enter(delegate(Rottable.Instance smi)
		{
			smi.GoTo(this.Stale);
		});
		this.Stale.ToggleStatusItem(Db.Get().CreatureStatusItems.Stale, (Rottable.Instance smi) => smi).ParamTransition<float>(this.rotParameter, this.Fresh, (Rottable.Instance smi, float p) => p > smi.SpoilTime - (smi.SpoilTime - smi.StaleTime)).ParamTransition<float>(this.rotParameter, this.Spoiled, (Rottable.Instance smi, float p) => p <= 0f)
			.ToggleSchedulePeriodic("Rot", 1f, delegate(Rottable.Instance smi)
			{
				smi.Rot(smi, 1f);
			});
		this.Spoiled.Enter(delegate(Rottable.Instance smi)
		{
			GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(smi.master.gameObject), 0, 0, "RotPile", Grid.SceneLayer.Ore, Folder.Entities);
			gameObject.gameObject.GetComponent<KSelectable>().SetName(UI.GAMEOBJECTEFFECTS.ROTTEN + " " + smi.master.gameObject.GetProperName());
			gameObject.transform.SetPosition(smi.master.transform.position);
			gameObject.GetComponent<PrimaryElement>().Mass = smi.master.GetComponent<PrimaryElement>().Mass;
			gameObject.GetComponent<PrimaryElement>().Temperature = smi.master.GetComponent<PrimaryElement>().Temperature;
			gameObject.SetActive(true);
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ITEMS.FOOD.ROTPILE.NAME, gameObject.transform, 1.5f, false);
			Edible component = smi.GetComponent<Edible>();
			if (component != null)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, string.Format(UI.ENDOFDAYREPORT.NOTES.ROTTED, smi.gameObject.GetProperName()), UI.ENDOFDAYREPORT.NOTES.ROTTED_CONTEXT);
			}
			Util.KDestroyGameObject(smi.gameObject);
		});
	}

	private static string OnStaleTooltip(List<Notification> notifications, object data)
	{
		string text = "\n";
		foreach (Notification notification in notifications)
		{
			if (notification.tooltipData != null)
			{
				GameObject gameObject = (GameObject)notification.tooltipData;
				if (gameObject != null)
				{
					text = text + "\n" + gameObject.GetProperName();
				}
			}
		}
		return string.Format(MISC.NOTIFICATIONS.FOODSTALE.TOOLTIP, text);
	}

	public static void SetStatusItems(KSelectable selectable, bool refrigerated, Rottable.RotAtmosphereQuality atmoshpere)
	{
		selectable.SetStatusItem(Db.Get().StatusItemCategories.PreservationTemperature, (!refrigerated) ? Db.Get().CreatureStatusItems.Unrefrigerated : Db.Get().CreatureStatusItems.Refrigerated, selectable);
		if (atmoshpere != Rottable.RotAtmosphereQuality.Normal)
		{
			if (atmoshpere != Rottable.RotAtmosphereQuality.Contaminating)
			{
				if (atmoshpere == Rottable.RotAtmosphereQuality.Sterilizing)
				{
					selectable.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.SterilizingAtmosphere, null);
				}
			}
			else
			{
				selectable.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, Db.Get().CreatureStatusItems.ContaminatedAtmosphere, null);
			}
		}
		else
		{
			selectable.SetStatusItem(Db.Get().StatusItemCategories.PreservationAtmosphere, null, null);
		}
	}

	public static bool IsRefrigerated(GameObject gameObject)
	{
		int num = Grid.PosToCell(gameObject);
		if (Grid.IsValidCell(num))
		{
			if (Grid.Temperature[num] < 277.15f)
			{
				return true;
			}
			Pickupable component = gameObject.GetComponent<Pickupable>();
			if (component != null && component.storage != null)
			{
				Refrigerator component2 = component.storage.GetComponent<Refrigerator>();
				return component2 != null && component2.IsActive();
			}
		}
		return false;
	}

	public static Rottable.RotAtmosphereQuality AtmosphereQuality(GameObject gameObject)
	{
		int num = Grid.PosToCell(gameObject);
		int num2 = Grid.CellAbove(num);
		SimHashes id = Grid.Element[num].id;
		Rottable.RotAtmosphereQuality rotAtmosphereQuality = Rottable.RotAtmosphereQuality.Normal;
		Rottable.AtmosphereModifier.TryGetValue((int)id, out rotAtmosphereQuality);
		Rottable.RotAtmosphereQuality rotAtmosphereQuality2 = Rottable.RotAtmosphereQuality.Normal;
		if (Grid.IsValidCell(num2))
		{
			SimHashes id2 = Grid.Element[num2].id;
			if (!Rottable.AtmosphereModifier.TryGetValue((int)id2, out rotAtmosphereQuality2))
			{
				rotAtmosphereQuality2 = rotAtmosphereQuality;
			}
		}
		else
		{
			rotAtmosphereQuality2 = rotAtmosphereQuality;
		}
		Rottable.RotAtmosphereQuality rotAtmosphereQuality3;
		if (rotAtmosphereQuality == rotAtmosphereQuality2)
		{
			rotAtmosphereQuality3 = rotAtmosphereQuality;
		}
		else if (rotAtmosphereQuality == Rottable.RotAtmosphereQuality.Contaminating || rotAtmosphereQuality2 == Rottable.RotAtmosphereQuality.Contaminating)
		{
			rotAtmosphereQuality3 = Rottable.RotAtmosphereQuality.Contaminating;
		}
		else if (rotAtmosphereQuality == Rottable.RotAtmosphereQuality.Normal || rotAtmosphereQuality2 == Rottable.RotAtmosphereQuality.Normal)
		{
			rotAtmosphereQuality3 = Rottable.RotAtmosphereQuality.Normal;
		}
		else
		{
			rotAtmosphereQuality3 = Rottable.RotAtmosphereQuality.Sterilizing;
		}
		return rotAtmosphereQuality3;
	}

	public StateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.FloatParameter rotParameter;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.State Preserved;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.State Fresh;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.State Stale_Pre;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.State Stale;

	public GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.State Spoiled;

	public static Dictionary<int, Rottable.RotAtmosphereQuality> AtmosphereModifier = new Dictionary<int, Rottable.RotAtmosphereQuality>
	{
		{
			721531317,
			Rottable.RotAtmosphereQuality.Contaminating
		},
		{
			1887387588,
			Rottable.RotAtmosphereQuality.Contaminating
		},
		{
			-1528777920,
			Rottable.RotAtmosphereQuality.Normal
		},
		{
			1836671383,
			Rottable.RotAtmosphereQuality.Normal
		},
		{
			1960575215,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-899515856,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-1554872654,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-1858722091,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			758759285,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-1046145888,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-1324664829,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-1406916018,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-432557516,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			-805366663,
			Rottable.RotAtmosphereQuality.Sterilizing
		},
		{
			1966552544,
			Rottable.RotAtmosphereQuality.Sterilizing
		}
	};

	public new class Instance : GameStateMachine<Rottable, Rottable.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, float spoilTime, float staleTime)
			: base(master)
		{
			this.pickupable = base.gameObject.RequireComponent<Pickupable>();
			base.master.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
			base.master.Subscribe(1335436905, new Action<object>(this.OnSplitFromChunk));
			this.primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			this.SpoilTime = spoilTime;
			this.StaleTime = staleTime;
			Amounts amounts = master.gameObject.GetAmounts();
			this.RotAmountInstance = amounts.Add(new AmountInstance(Db.Get().Amounts.Rot, master.gameObject));
			this.RotAmountInstance.maxAttribute.ClearModifiers();
			this.RotAmountInstance.maxAttribute.Add("SpoilTime", new AttributeModifier("Rot", this.SpoilTime, null, false, false, true));
			this.RotAmountInstance.SetValue(this.SpoilTime);
			base.sm.rotParameter.Set(this.RotAmountInstance.value, base.smi);
			this.UnrefrigeratedModifier = new AttributeModifier("Rot", 0f, DUPLICANTS.MODIFIERS.ROTTEMPERATURE.NAME, false, false, false);
			this.ContaminatedAtmosphere = new AttributeModifier("Rot", 0f, DUPLICANTS.MODIFIERS.ROTATMOSPHERE.NAME, false, false, false);
			this.RotAmountInstance.deltaAttribute.Add("UnrefrigeratedModifier", this.UnrefrigeratedModifier);
			this.RotAmountInstance.deltaAttribute.Add("ContaminatedAtmosphereModifier ", this.ContaminatedAtmosphere);
			this.RefreshModifiers(0f);
		}

		public float RotValue
		{
			get
			{
				return this.RotAmountInstance.value;
			}
			set
			{
				base.sm.rotParameter.Set(value, this);
				this.RotAmountInstance.SetValue(value);
			}
		}

		public float RotConstitutionPercentage
		{
			get
			{
				return this.RotValue / this.SpoilTime;
			}
		}

		public string StateString()
		{
			string text = "";
			if (base.smi.GetCurrentState() == base.sm.Fresh)
			{
				text = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback(CREATURES.STATUSITEMS.FRESH.NAME, this);
			}
			if (base.smi.GetCurrentState() == base.sm.Stale)
			{
				text = Db.Get().CreatureStatusItems.Fresh.resolveStringCallback(CREATURES.STATUSITEMS.STALE.NAME, this);
			}
			return text;
		}

		public void Rot(Rottable.Instance smi, float deltaTime)
		{
			smi.sm.rotParameter.Set(this.RotAmountInstance.value, smi);
			this.RefreshModifiers(deltaTime);
			if (smi.pickupable.storage != null)
			{
				smi.pickupable.storage.Trigger(-1197125120, null);
			}
		}

		public void RefreshModifiers(float dt)
		{
			IStateMachineTarget master = this.GetMaster();
			if (!master.isNull)
			{
				KSelectable component = base.GetComponent<KSelectable>();
				if (Grid.Solid[Grid.PosToCell(base.gameObject)])
				{
					this.UnrefrigeratedModifier.SetValue(0f);
					this.ContaminatedAtmosphere.SetValue(0f);
				}
				else
				{
					this.UnrefrigeratedModifier.SetValue(this.rotTemperatureModifier());
					this.ContaminatedAtmosphere.SetValue(this.rotAtmosphereModifier());
				}
				Rottable.RotAtmosphereQuality rotAtmosphereQuality;
				if (this.ContaminatedAtmosphere.Value == 0f)
				{
					rotAtmosphereQuality = Rottable.RotAtmosphereQuality.Normal;
				}
				else
				{
					rotAtmosphereQuality = ((this.ContaminatedAtmosphere.Value <= 0f) ? Rottable.RotAtmosphereQuality.Contaminating : Rottable.RotAtmosphereQuality.Sterilizing);
				}
				Rottable.SetStatusItems(component, this.UnrefrigeratedModifier.Value == 0f, rotAtmosphereQuality);
				this.RotAmountInstance.deltaAttribute.ClearModifiers();
				if (this.UnrefrigeratedModifier.Value != 0f && this.ContaminatedAtmosphere.Value != 0.5f)
				{
					this.RotAmountInstance.deltaAttribute.Add("UnrefrigeratedModifier", this.UnrefrigeratedModifier);
				}
				if (this.ContaminatedAtmosphere.Value != 0f && this.ContaminatedAtmosphere.Value != 0.5f)
				{
					this.RotAmountInstance.deltaAttribute.Add("ContaminatedAtmosphere", this.ContaminatedAtmosphere);
				}
			}
		}

		private float rotTemperatureModifier()
		{
			return (!Rottable.IsRefrigerated(base.gameObject)) ? (-0.5f) : 0f;
		}

		private float rotAtmosphereModifier()
		{
			float num = 1f;
			Rottable.RotAtmosphereQuality rotAtmosphereQuality = Rottable.AtmosphereQuality(base.gameObject);
			if (rotAtmosphereQuality != Rottable.RotAtmosphereQuality.Normal)
			{
				if (rotAtmosphereQuality != Rottable.RotAtmosphereQuality.Contaminating)
				{
					if (rotAtmosphereQuality == Rottable.RotAtmosphereQuality.Sterilizing)
					{
						num = 0.5f;
					}
				}
				else
				{
					num = -0.5f;
				}
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		private void OnAbsorb(object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (pickupable != null)
			{
				PrimaryElement component = base.gameObject.GetComponent<PrimaryElement>();
				PrimaryElement primaryElement = pickupable.PrimaryElement;
				Rottable.Instance smi = pickupable.gameObject.GetSMI<Rottable.Instance>();
				if (component != null && primaryElement != null && smi != null)
				{
					float num = component.Units * base.sm.rotParameter.Get(base.smi);
					float num2 = primaryElement.Units * base.sm.rotParameter.Get(smi);
					float num3 = (num + num2) / (component.Units + primaryElement.Units);
					base.sm.rotParameter.Set(num3, base.smi);
				}
			}
		}

		public string GetToolTip()
		{
			return this.RotAmountInstance.GetTooltip();
		}

		private void OnSplitFromChunk(object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (pickupable != null)
			{
				Rottable.Instance smi = pickupable.GetSMI<Rottable.Instance>();
				if (smi != null)
				{
					this.RotValue = smi.RotValue;
				}
			}
		}

		public void OnPreserved(object data)
		{
			if ((bool)data)
			{
				base.smi.GoTo(base.sm.Preserved);
			}
			else
			{
				base.smi.GoTo(base.sm.Fresh);
			}
		}

		private AmountInstance RotAmountInstance;

		private AttributeModifier UnrefrigeratedModifier;

		private AttributeModifier ContaminatedAtmosphere;

		public float SpoilTime;

		public float StaleTime;

		public float RotTemperature = 277.15f;

		public PrimaryElement primaryElement;

		public Pickupable pickupable;
	}

	public enum RotAtmosphereQuality
	{
		Normal,
		Sterilizing,
		Contaminating
	}
}
