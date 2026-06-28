using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class ChoreTypes : ResourceSet<ChoreType>
	{
		public ChoreTypes(ResourceSet parent)
			: base("ChoreTypes", parent)
		{
			this.Die = this.Add("Die", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DIE.NAME, DUPLICANTS.CHORES.DIE.STATUS);
			this.Entombed = this.Add("Entombed", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.ENTOMBED.NAME, DUPLICANTS.CHORES.ENTOMBED.STATUS);
			this.SuitMarker = this.Add("SuitMarker", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS);
			this.WashHands = this.Add("WashHands", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS);
			this.HealCritical = this.Add("HealCritical", new string[0], "HealCritical", new string[] { "Vomit", "Cough" }, DUPLICANTS.CHORES.HEALCRITICAL.NAME, DUPLICANTS.CHORES.HEALCRITICAL.STATUS);
			this.BeIncapacitated = this.Add("BeIncapacitated", new string[0], "BeIncapacitated", new string[0], DUPLICANTS.CHORES.BEINCAPACITATED.NAME, DUPLICANTS.CHORES.BEINCAPACITATED.STATUS);
			this.GeneShuffle = this.Add("GeneShuffle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.GENESHUFFLE.NAME, DUPLICANTS.CHORES.GENESHUFFLE.STATUS);
			this.DebugGoTo = this.Add("DebugGoTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DEBUGGOTO.NAME, DUPLICANTS.CHORES.DEBUGGOTO.STATUS);
			this.MoveTo = this.Add("MoveTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOVETO.NAME, DUPLICANTS.CHORES.MOVETO.STATUS);
			this.DropUnusedInventory = this.Add("DropUnusedInventory", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.NAME, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.STATUS);
			this.Pee = this.Add("Pee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.PEE.NAME, DUPLICANTS.CHORES.PEE.STATUS);
			this.RecoverBreath = this.Add("RecoverBreath", new string[0], "RecoverBreath", new string[0], DUPLICANTS.CHORES.RECOVERBREATH.NAME, DUPLICANTS.CHORES.RECOVERBREATH.STATUS);
			this.Flee = this.Add("Flee", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.FLEE.NAME, DUPLICANTS.CHORES.FLEE.STATUS);
			this.MoveToQuarantine = this.Add("MoveToQuarantine", new string[0], "MoveToQuarantine", new string[0], DUPLICANTS.CHORES.MOVETOQUARANTINE.NAME, DUPLICANTS.CHORES.MOVETOQUARANTINE.STATUS);
			this.Attack = this.Add("Attack", new string[] { "Combat" }, string.Empty, new string[0], DUPLICANTS.CHORES.ATTACK.NAME, DUPLICANTS.CHORES.ATTACK.STATUS);
			this.Emote = this.Add("Emote", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS);
			this.EmoteHighPriority = this.Add("EmoteHighPriority", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS);
			this.StressEmote = this.Add("StressEmote", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS);
			this.StressVomit = this.Add("StressVomit", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSVOMIT.NAME, DUPLICANTS.CHORES.STRESSVOMIT.STATUS);
			this.UglyCry = this.Add("UglyCry", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.UGLY_CRY.NAME, DUPLICANTS.CHORES.UGLY_CRY.STATUS);
			this.BingeEat = this.Add("BingeEat", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.BINGE_EAT.NAME, DUPLICANTS.CHORES.BINGE_EAT.STATUS);
			this.StressActingOut = this.Add("StressActingOut", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.STRESSACTINGOUT.NAME, DUPLICANTS.CHORES.STRESSACTINGOUT.STATUS);
			this.Vomit = this.Add("Vomit", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.VOMIT.NAME, DUPLICANTS.CHORES.VOMIT.STATUS);
			this.Cough = this.Add("Cough", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.COUGH.NAME, DUPLICANTS.CHORES.VOMIT.STATUS);
			this.StressIdle = this.Add("StressIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSIDLE.NAME, DUPLICANTS.CHORES.STRESSIDLE.STATUS);
			this.RescueIncapacitated = this.Add("RescueIncapacitated", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RESCUEINCAPACITATED.NAME, DUPLICANTS.CHORES.RESCUEINCAPACITATED.STATUS);
			this.UseToilet = this.Add("UseToilet", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.USETOILET.NAME, DUPLICANTS.CHORES.USETOILET.STATUS);
			this.Eat = this.Add("Eat", new string[0], "Eat", new string[0], DUPLICANTS.CHORES.EAT.NAME, DUPLICANTS.CHORES.EAT.STATUS);
			this.Narcolepsy = this.Add("Narcolepsy", new string[0], "Narcolepsy", new string[0], DUPLICANTS.CHORES.NARCOLEPSY.NAME, DUPLICANTS.CHORES.NARCOLEPSY.STATUS);
			this.ReturnSuitUrgent = this.Add("ReturnSuitUrgent", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS);
			this.SleepDueToDisease = this.Add("SleepDueToDisease", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS);
			this.Sleep = this.Add("Sleep", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.SLEEP.NAME, DUPLICANTS.CHORES.SLEEP.STATUS);
			this.RestDueToDisease = this.Add("RestDueToDisease", new string[0], "RestDueToDisease", new string[0], DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS);
			this.TakeMedicine = this.Add("TakeMedicine", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TAKEMEDICINE.NAME, DUPLICANTS.CHORES.TAKEMEDICINE.STATUS);
			this.ScrubOre = this.Add("ScrubOre", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SCRUBORE.NAME, DUPLICANTS.CHORES.SCRUBORE.STATUS);
			this.DeliverFood = this.Add("DeliverFood", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DELIVERFOOD.NAME, DUPLICANTS.CHORES.DELIVERFOOD.STATUS);
			this.FetchCritical = this.Add("FetchCritical", new string[] { "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCHCRITICAL.NAME, DUPLICANTS.CHORES.FETCHCRITICAL.STATUS);
			this.Sigh = this.Add("Sigh", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.SIGH.NAME, DUPLICANTS.CHORES.SIGH.STATUS);
			this.Heal = this.Add("Heal", new string[0], "Heal", new string[] { "Vomit" }, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS);
			this.Doctor = this.Add("DoctorChore", new string[] { "MedicalAid" }, "Doctor", new string[0], DUPLICANTS.CHORES.DOCTOR.NAME, DUPLICANTS.CHORES.DOCTOR.STATUS);
			this.Shower = this.Add("Shower", new string[0], "Shower", new string[0], DUPLICANTS.CHORES.SHOWER.NAME, DUPLICANTS.CHORES.SHOWER.STATUS);
			this.Relax = this.Add("Relax", new string[] { "Massage" }, "Relax", new string[0], DUPLICANTS.CHORES.RELAX.NAME, DUPLICANTS.CHORES.RELAX.STATUS);
			this.Equip = this.Add("Equip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EQUIP.NAME, DUPLICANTS.CHORES.EQUIP.STATUS);
			this.Recharge = this.Add("Recharge", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RECHARGE.NAME, DUPLICANTS.CHORES.RECHARGE.STATUS);
			this.Unequip = this.Add("Unequip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.UNEQUIP.NAME, DUPLICANTS.CHORES.UNEQUIP.STATUS);
			this.Warmup = this.Add("Warmup", new string[0], "WarmUp", new string[0], DUPLICANTS.CHORES.WARMUP.NAME, DUPLICANTS.CHORES.WARMUP.STATUS);
			this.Cooldown = this.Add("Cooldown", new string[0], "CoolDown", new string[0], DUPLICANTS.CHORES.COOLDOWN.NAME, DUPLICANTS.CHORES.COOLDOWN.STATUS);
			this.EmptyStorage = this.Add("EmptyStorage", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, DUPLICANTS.CHORES.EMPTYSTORAGE.STATUS);
			this.Upgrade = this.Add("Upgrade", new string[] { "Build" }, string.Empty, new string[0], DUPLICANTS.CHORES.UPGRADE.NAME, DUPLICANTS.CHORES.UPGRADE.STATUS);
			this.Art = this.Add("Art", new string[] { "Art" }, string.Empty, new string[0], DUPLICANTS.CHORES.ART.NAME, DUPLICANTS.CHORES.ART.STATUS);
			this.Mop = this.Add("Mop", new string[] { "Mop" }, string.Empty, new string[0], DUPLICANTS.CHORES.MOP.NAME, DUPLICANTS.CHORES.MOP.STATUS);
			this.Relocate = this.Add("Relocate", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RELOCATE.NAME, DUPLICANTS.CHORES.RELOCATE.STATUS);
			this.Toggle = this.Add("Toggle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TOGGLE.NAME, DUPLICANTS.CHORES.TOGGLE.STATUS);
			this.Disinfect = this.Add("Disinfect", new string[] { "Disinfect" }, string.Empty, new string[0], DUPLICANTS.CHORES.DISINFECT.NAME, DUPLICANTS.CHORES.DISINFECT.STATUS);
			this.Repair = this.Add("Repair", new string[] { "Repair" }, string.Empty, new string[0], DUPLICANTS.CHORES.REPAIR.NAME, DUPLICANTS.CHORES.REPAIR.STATUS);
			this.Deconstruct = this.Add("Deconstruct", new string[] { "Build" }, string.Empty, new string[0], DUPLICANTS.CHORES.DECONSTRUCT.NAME, DUPLICANTS.CHORES.DECONSTRUCT.STATUS);
			this.Research = this.Add("Research", new string[] { "Research" }, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCH.NAME, DUPLICANTS.CHORES.RESEARCH.STATUS);
			this.ResearchFetch = this.Add("ResearchFetch", new string[] { "Research", "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCHFETCH.NAME, DUPLICANTS.CHORES.RESEARCHFETCH.STATUS);
			this.GeneratePower = this.Add("GeneratePower", new string[] { "GeneratePower" }, string.Empty, new string[] { "Relax" }, DUPLICANTS.CHORES.GENERATEPOWER.NAME, DUPLICANTS.CHORES.GENERATEPOWER.STATUS);
			this.Harvest = this.Add("Harvest", new string[] { "Harvest" }, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS);
			this.Uproot = this.Add("Uproot", new string[] { "Harvest" }, string.Empty, new string[0], DUPLICANTS.CHORES.UPROOT.NAME, DUPLICANTS.CHORES.UPROOT.STATUS);
			this.CleanToilet = this.Add("CleanToilet", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.CLEANTOILET.NAME, DUPLICANTS.CHORES.CLEANTOILET.STATUS);
			this.LiquidCooledFan = this.Add("LiquidCooledFan", new string[] { "LiquidCooledFan" }, string.Empty, new string[0], DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.NAME, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.STATUS);
			this.Cook = this.Add("Cook", new string[] { "Cook" }, string.Empty, new string[0], DUPLICANTS.CHORES.COOK.NAME, DUPLICANTS.CHORES.COOK.STATUS);
			this.CookFetch = this.Add("CookFetch", new string[] { "Cook", "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.COOKFETCH.NAME, DUPLICANTS.CHORES.COOKFETCH.STATUS);
			this.Mush = this.Add("Mush", new string[] { "Cook" }, string.Empty, new string[0], DUPLICANTS.CHORES.MUSH.NAME, DUPLICANTS.CHORES.MUSH.STATUS);
			this.MushFetch = this.Add("MushFetch", new string[] { "Cook", "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.MUSHFETCH.NAME, DUPLICANTS.CHORES.MUSHFETCH.STATUS);
			this.CompostWorkable = this.Add("CompostWorkable", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.COMPOSTWORKABLE.NAME, DUPLICANTS.CHORES.COMPOSTWORKABLE.STATUS);
			this.FlipCompost = this.Add("FlipCompost", new string[] { "Compost" }, string.Empty, new string[0], DUPLICANTS.CHORES.FLIPCOMPOST.NAME, DUPLICANTS.CHORES.FLIPCOMPOST.STATUS);
			this.Depressurize = this.Add("Depressurize", new string[] { "Maintenance" }, string.Empty, new string[0], DUPLICANTS.CHORES.DEPRESSURIZE.NAME, DUPLICANTS.CHORES.DEPRESSURIZE.STATUS);
			this.Fabricate = this.Add("Fabricate", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS);
			this.FabricateFetch = this.Add("FabricateFetch", new string[] { "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATEFETCH.NAME, DUPLICANTS.CHORES.FABRICATEFETCH.STATUS);
			this.Transport = this.Add("Transport", new string[] { "Sweep" }, string.Empty, new string[0], DUPLICANTS.CHORES.TRANSPORT.NAME, DUPLICANTS.CHORES.TRANSPORT.STATUS);
			this.Build = this.Add("Build", new string[] { "Build" }, string.Empty, new string[0], DUPLICANTS.CHORES.BUILD.NAME, DUPLICANTS.CHORES.BUILD.STATUS);
			this.BuildDig = this.Add("BuildDig", new string[] { "Dig" }, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDDIG.NAME, DUPLICANTS.CHORES.BUILDDIG.STATUS);
			this.BuildFetch = this.Add("BuildFetch", new string[] { "Build", "Deliver" }, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDFETCH.NAME, DUPLICANTS.CHORES.BUILDFETCH.STATUS);
			this.Dig = this.Add("Dig", new string[] { "Dig" }, string.Empty, new string[0], DUPLICANTS.CHORES.DIG.NAME, DUPLICANTS.CHORES.DIG.STATUS);
			this.Fetch = this.Add("Fetch", new string[] { "Deliver", "Sweep" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS);
			this.MoveToSafety = this.Add("MoveToSafety", new string[0], "MoveToSafety", new string[0], DUPLICANTS.CHORES.MOVETOSAFETY.NAME, DUPLICANTS.CHORES.MOVETOSAFETY.STATUS);
			this.ReturnSuitIdle = this.Add("ReturnSuitIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS);
			this.Idle = this.Add("IdleChore", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.IDLE.NAME, DUPLICANTS.CHORES.IDLE.STATUS);
			ChoreType[][] array = new ChoreType[][]
			{
				new ChoreType[] { this.Die },
				new ChoreType[] { this.Entombed },
				new ChoreType[] { this.HealCritical },
				new ChoreType[] { this.BeIncapacitated, this.GeneShuffle },
				new ChoreType[] { this.DebugGoTo },
				new ChoreType[] { this.StressVomit },
				new ChoreType[] { this.MoveTo },
				new ChoreType[] { this.RecoverBreath },
				new ChoreType[] { this.UglyCry },
				new ChoreType[] { this.BingeEat },
				new ChoreType[] { this.EmoteHighPriority, this.StressActingOut, this.Vomit, this.Cough, this.Pee, this.StressIdle, this.RescueIncapacitated },
				new ChoreType[] { this.MoveToQuarantine },
				new ChoreType[] { this.Attack },
				new ChoreType[] { this.Flee },
				new ChoreType[] { this.UseToilet, this.Eat },
				new ChoreType[] { this.Heal, this.SleepDueToDisease, this.RestDueToDisease },
				new ChoreType[] { this.Sleep, this.Narcolepsy, this.Warmup, this.Cooldown },
				new ChoreType[] { this.Emote },
				new ChoreType[] { this.Relax },
				new ChoreType[] { this.Equip, this.Unequip },
				new ChoreType[]
				{
					this.DeliverFood, this.Sigh, this.EmptyStorage, this.Upgrade, this.Repair, this.Disinfect, this.Shower, this.CleanToilet, this.LiquidCooledFan, this.SuitMarker,
					this.WashHands, this.TakeMedicine, this.Doctor, this.Recharge, this.FetchCritical, this.ScrubOre, this.MoveToSafety, this.Relocate, this.Research, this.ResearchFetch,
					this.Mop, this.Toggle, this.Deconstruct, this.Fetch, this.Transport, this.Art, this.GeneratePower, this.CompostWorkable, this.DropUnusedInventory, this.Harvest,
					this.Uproot, this.Fabricate, this.Mush, this.Cook, this.Build, this.Dig, this.BuildDig, this.FlipCompost, this.Depressurize, this.BuildFetch,
					this.CookFetch, this.MushFetch, this.FabricateFetch, this.StressEmote, this.ReturnSuitUrgent
				},
				new ChoreType[] { this.ReturnSuitIdle },
				new ChoreType[] { this.Idle }
			};
			string text = string.Empty;
			int num = 100000;
			foreach (ChoreType[] array3 in array)
			{
				foreach (ChoreType choreType in array3)
				{
					if (choreType.interruptPriority != 0)
					{
						text = text + "Interrupt priority set more than once: " + choreType.Id;
					}
					choreType.interruptPriority = num;
				}
				num -= 100;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Debug.LogError(text, null);
			}
			string text2 = string.Empty;
			foreach (ChoreType choreType2 in this)
			{
				if (choreType2.interruptPriority == 0)
				{
					text2 = text2 + "Interrupt priority missing for: " + choreType2.Id + "\n";
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				Debug.LogError(text2, null);
			}
		}

		private ChoreType Add(string id, string[] chore_groups, string urge, string[] interrupt_exclusion, string name, string status_message)
		{
			List<Tag> list = new List<Tag>();
			for (int i = 0; i < interrupt_exclusion.Length; i++)
			{
				list.Add(TagManager.Create(interrupt_exclusion[i], null));
			}
			ChoreType choreType = new ChoreType(id, this, chore_groups, urge, name, status_message, list.ToArray(), this.nextPriority);
			this.nextPriority -= 100;
			return choreType;
		}

		public ChoreType Attack;

		public ChoreType Flee;

		public ChoreType BeIncapacitated;

		public ChoreType DebugGoTo;

		public ChoreType DeliverFood;

		public ChoreType Die;

		public ChoreType GeneShuffle;

		public ChoreType Doctor;

		public ChoreType WashHands;

		public ChoreType Shower;

		public ChoreType Eat;

		public ChoreType Entombed;

		public ChoreType Idle;

		public ChoreType MoveToQuarantine;

		public ChoreType RescueIncapacitated;

		public ChoreType RecoverBreath;

		public ChoreType Sigh;

		public ChoreType Sleep;

		public ChoreType Narcolepsy;

		public ChoreType Vomit;

		public ChoreType Cough;

		public ChoreType Pee;

		public ChoreType TakeMedicine;

		public ChoreType RestDueToDisease;

		public ChoreType SleepDueToDisease;

		public ChoreType Heal;

		public ChoreType HealCritical;

		public ChoreType Emote;

		public ChoreType EmoteHighPriority;

		public ChoreType StressEmote;

		public ChoreType StressActingOut;

		public ChoreType Relax;

		public ChoreType MoveToSafety;

		public ChoreType Equip;

		public ChoreType Recharge;

		public ChoreType Unequip;

		public ChoreType Warmup;

		public ChoreType Cooldown;

		public ChoreType Mop;

		public ChoreType Relocate;

		public ChoreType Toggle;

		public ChoreType Fetch;

		public ChoreType Upgrade;

		public ChoreType Disinfect;

		public ChoreType Repair;

		public ChoreType EmptyStorage;

		public ChoreType Deconstruct;

		public ChoreType Art;

		public ChoreType Research;

		public ChoreType ResearchFetch;

		public ChoreType GeneratePower;

		public ChoreType Harvest;

		public ChoreType Uproot;

		public ChoreType CleanToilet;

		public ChoreType UseToilet;

		public ChoreType LiquidCooledFan;

		public ChoreType CompostWorkable;

		public ChoreType Fabricate;

		public ChoreType FabricateFetch;

		public ChoreType Mush;

		public ChoreType MushFetch;

		public ChoreType Cook;

		public ChoreType CookFetch;

		public ChoreType Build;

		public ChoreType BuildFetch;

		public ChoreType BuildDig;

		public ChoreType Dig;

		public ChoreType FlipCompost;

		public ChoreType Depressurize;

		public ChoreType Transport;

		public ChoreType DropUnusedInventory;

		public ChoreType FetchCritical;

		public ChoreType StressVomit;

		public ChoreType MoveTo;

		public ChoreType UglyCry;

		public ChoreType BingeEat;

		public ChoreType StressIdle;

		public ChoreType ScrubOre;

		public ChoreType SuitMarker;

		public ChoreType ReturnSuitUrgent;

		public ChoreType ReturnSuitIdle;

		private int nextPriority = 10000;
	}
}
