using System;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class ChoreTypes : ResourceSet<ChoreType>
	{
		public ChoreTypes(ResourceSet parent)
			: base("ChoreTypes", parent)
		{
			this.Die = this.Add("Die", new string[0], string.Empty, DUPLICANTS.CHORES.DIE.NAME, DUPLICANTS.CHORES.DIE.STATUS);
			this.Entombed = this.Add("Entombed", new string[0], string.Empty, DUPLICANTS.CHORES.ENTOMBED.NAME, DUPLICANTS.CHORES.ENTOMBED.STATUS);
			this.HealCritical = this.Add("HealCritical", new string[0], "HealCritical", DUPLICANTS.CHORES.HEALCRITICAL.NAME, DUPLICANTS.CHORES.HEALCRITICAL.STATUS);
			this.BeIncapacitated = this.Add("BeIncapacitated", new string[0], "BeIncapacitated", DUPLICANTS.CHORES.BEINCAPACITATED.NAME, DUPLICANTS.CHORES.BEINCAPACITATED.STATUS);
			this.DebugGoTo = this.Add("DebugGoTo", new string[0], string.Empty, DUPLICANTS.CHORES.DEBUGGOTO.NAME, DUPLICANTS.CHORES.DEBUGGOTO.STATUS);
			this.MoveTo = this.Add("MoveTo", new string[0], string.Empty, DUPLICANTS.CHORES.MOVETO.NAME, DUPLICANTS.CHORES.MOVETO.STATUS);
			this.DropUnusedInventory = this.Add("DropUnusedInventory", new string[0], string.Empty, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.NAME, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.STATUS);
			this.Pee = this.Add("Pee", new string[0], "Pee", DUPLICANTS.CHORES.PEE.NAME, DUPLICANTS.CHORES.PEE.STATUS);
			this.StressVomit = this.Add("StressVomit", new string[0], string.Empty, DUPLICANTS.CHORES.STRESSVOMIT.NAME, DUPLICANTS.CHORES.STRESSVOMIT.STATUS);
			this.EmoteHighPriority = this.Add("EmoteHighPriority", new string[0], "EmoteHighPriority", DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS);
			this.ManualControlPrioritizeChore = this.Add("ManualControlPrioritizeChore", new string[0], string.Empty, DUPLICANTS.CHORES.MANUALCONTROLPRIORITIZECHORE.NAME, DUPLICANTS.CHORES.MANUALCONTROLPRIORITIZECHORE.STATUS);
			this.ManualControlGoTo = this.Add("ManualControlGoTo", new string[0], string.Empty, DUPLICANTS.CHORES.MANUALCONTROLGOTO.NAME, DUPLICANTS.CHORES.MANUALCONTROLGOTO.STATUS);
			this.ManualControlIdle = this.Add("ManualControlIdle", new string[0], string.Empty, DUPLICANTS.CHORES.MANUALCONTROLIDLE.NAME, DUPLICANTS.CHORES.MANUALCONTROLIDLE.STATUS);
			this.Flee = this.Add("Flee", new string[0], string.Empty, DUPLICANTS.CHORES.FLEE.NAME, DUPLICANTS.CHORES.FLEE.STATUS);
			this.MentalBreak = this.Add("MentalBreak", new string[0], string.Empty, DUPLICANTS.CHORES.MENTALBREAK.NAME, DUPLICANTS.CHORES.MENTALBREAK.STATUS);
			this.RecoverBreath = this.Add("RecoverBreath", new string[0], "RecoverBreath", DUPLICANTS.CHORES.RECOVERBREATH.NAME, DUPLICANTS.CHORES.RECOVERBREATH.STATUS);
			this.MoveToQuarantine = this.Add("MoveToQuarantine", new string[0], "MoveToQuarantine", DUPLICANTS.CHORES.MOVETOQUARANTINE.NAME, DUPLICANTS.CHORES.MOVETOQUARANTINE.STATUS);
			this.MoveToLocation = this.Add("MoveToLocation", new string[0], string.Empty, DUPLICANTS.CHORES.MOVETOLOCATION.NAME, DUPLICANTS.CHORES.MOVETOLOCATION.STATUS);
			this.Attack = this.Add("Attack", new string[] { "Combat" }, string.Empty, DUPLICANTS.CHORES.ATTACK.NAME, DUPLICANTS.CHORES.ATTACK.STATUS);
			this.UseToilet = this.Add("UseToilet", new string[0], string.Empty, DUPLICANTS.CHORES.USETOILET.NAME, DUPLICANTS.CHORES.USETOILET.STATUS);
			this.WashHands = this.Add("WashHands", new string[0], "WashHands", DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS);
			this.Eat = this.Add("Eat", new string[0], "Eat", DUPLICANTS.CHORES.EAT.NAME, DUPLICANTS.CHORES.EAT.STATUS);
			this.PrioritizeChore = this.Add("PrioritizeChore", new string[0], string.Empty, DUPLICANTS.CHORES.PRIORITIZECHORE.NAME, DUPLICANTS.CHORES.PRIORITIZECHORE.STATUS);
			this.Vomit = this.Add("Vomit", new string[0], "EmoteHighPriority", DUPLICANTS.CHORES.VOMIT.NAME, DUPLICANTS.CHORES.VOMIT.STATUS);
			this.Sleep = this.Add("Sleep", new string[0], "Sleep", DUPLICANTS.CHORES.SLEEP.NAME, DUPLICANTS.CHORES.SLEEP.STATUS);
			this.SleepOnFloor = this.Add("Sleep", new string[0], "Sleep", DUPLICANTS.CHORES.SLEEPONFLOOR.NAME, DUPLICANTS.CHORES.SLEEPONFLOOR.STATUS);
			this.TakeMedicine = this.Add("TakeMedicine", new string[0], "TakeMedicine", DUPLICANTS.CHORES.TAKEMEDICINE.NAME, DUPLICANTS.CHORES.TAKEMEDICINE.STATUS);
			this.Doctor = this.Add("DoctorChore", new string[0], "Doctor", DUPLICANTS.CHORES.DOCTOR.NAME, DUPLICANTS.CHORES.DOCTOR.STATUS);
			this.DeliverFood = this.Add("DeliverFood", new string[0], string.Empty, DUPLICANTS.CHORES.DELIVERFOOD.NAME, DUPLICANTS.CHORES.DELIVERFOOD.STATUS);
			this.FetchCritical = this.Add("FetchCritical", new string[] { "Deliver" }, string.Empty, DUPLICANTS.CHORES.FETCHCRITICAL.NAME, DUPLICANTS.CHORES.FETCHCRITICAL.STATUS);
			this.Shower = this.Add("Shower", new string[0], "Shower", DUPLICANTS.CHORES.SHOWER.NAME, DUPLICANTS.CHORES.SHOWER.STATUS);
			this.Sigh = this.Add("Sigh", new string[0], "Emote", DUPLICANTS.CHORES.SIGH.NAME, DUPLICANTS.CHORES.SIGH.STATUS);
			this.RestDueToDisease = this.Add("RestDueToDisease", new string[0], "RestDueToDisease", DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS);
			this.Heal = this.Add("Heal", new string[0], "Heal", DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS);
			this.StressActingOut = this.Add("StressActingOut", new string[0], "Aggression", DUPLICANTS.CHORES.STRESSACTINGOUT.NAME, DUPLICANTS.CHORES.STRESSACTINGOUT.STATUS);
			this.Relax = this.Add("Relax", new string[0], "Relax", DUPLICANTS.CHORES.RELAX.NAME, DUPLICANTS.CHORES.RELAX.STATUS);
			this.Equip = this.Add("Equip", new string[0], string.Empty, DUPLICANTS.CHORES.EQUIP.NAME, DUPLICANTS.CHORES.EQUIP.STATUS);
			this.Recharge = this.Add("Recharge", new string[0], string.Empty, DUPLICANTS.CHORES.RECHARGE.NAME, DUPLICANTS.CHORES.RECHARGE.STATUS);
			this.Unequip = this.Add("Unequip", new string[0], string.Empty, DUPLICANTS.CHORES.UNEQUIP.NAME, DUPLICANTS.CHORES.UNEQUIP.STATUS);
			this.Warmup = this.Add("Warmup", new string[0], "WarmUp", DUPLICANTS.CHORES.WARMUP.NAME, DUPLICANTS.CHORES.WARMUP.STATUS);
			this.EmptyStorage = this.Add("EmptyStorage", new string[0], string.Empty, DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, DUPLICANTS.CHORES.EMPTYSTORAGE.STATUS);
			this.Upgrade = this.Add("Upgrade", new string[] { "Build" }, string.Empty, DUPLICANTS.CHORES.UPGRADE.NAME, DUPLICANTS.CHORES.UPGRADE.STATUS);
			this.Art = this.Add("Art", new string[] { "Art" }, string.Empty, DUPLICANTS.CHORES.ART.NAME, DUPLICANTS.CHORES.ART.STATUS);
			this.Mop = this.Add("Mop", new string[0], string.Empty, DUPLICANTS.CHORES.MOP.NAME, DUPLICANTS.CHORES.MOP.STATUS);
			this.Relocate = this.Add("Relocate", new string[0], string.Empty, DUPLICANTS.CHORES.RELOCATE.NAME, DUPLICANTS.CHORES.RELOCATE.STATUS);
			this.Toggle = this.Add("Toggle", new string[0], string.Empty, DUPLICANTS.CHORES.TOGGLE.NAME, DUPLICANTS.CHORES.TOGGLE.STATUS);
			this.RescueIncapacitated = this.Add("RescueIncapacitated", new string[0], string.Empty, DUPLICANTS.CHORES.RESCUEINCAPACITATED.NAME, DUPLICANTS.CHORES.RESCUEINCAPACITATED.STATUS);
			this.Repair = this.Add("Repair", new string[] { "Build" }, string.Empty, DUPLICANTS.CHORES.REPAIR.NAME, DUPLICANTS.CHORES.REPAIR.STATUS);
			this.Deconstruct = this.Add("Deconstruct", new string[] { "Build" }, string.Empty, DUPLICANTS.CHORES.DECONSTRUCT.NAME, DUPLICANTS.CHORES.DECONSTRUCT.STATUS);
			this.Research = this.Add("Research", new string[] { "Research" }, string.Empty, DUPLICANTS.CHORES.RESEARCH.NAME, DUPLICANTS.CHORES.RESEARCH.STATUS);
			this.GeneratePower = this.Add("GeneratePower", new string[] { "GeneratePower" }, string.Empty, DUPLICANTS.CHORES.GENERATEPOWER.NAME, DUPLICANTS.CHORES.GENERATEPOWER.STATUS);
			this.Harvest = this.Add("Harvest", new string[] { "Harvest" }, string.Empty, DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS);
			this.Uproot = this.Add("Uproot", new string[] { "Harvest" }, string.Empty, DUPLICANTS.CHORES.UPROOT.NAME, DUPLICANTS.CHORES.UPROOT.STATUS);
			this.CleanToilet = this.Add("CleanToilet", new string[0], string.Empty, DUPLICANTS.CHORES.CLEANTOILET.NAME, DUPLICANTS.CHORES.CLEANTOILET.STATUS);
			this.LiquidCooledFan = this.Add("LiquidCooledFan", new string[0], string.Empty, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.NAME, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.STATUS);
			this.Cook = this.Add("Cook", new string[] { "Cook" }, string.Empty, DUPLICANTS.CHORES.COOK.NAME, DUPLICANTS.CHORES.COOK.STATUS);
			this.CookFetch = this.Add("CookFetch", new string[] { "Cook", "Deliver" }, string.Empty, DUPLICANTS.CHORES.COOKFETCH.NAME, DUPLICANTS.CHORES.COOKFETCH.STATUS);
			this.Mush = this.Add("Mush", new string[] { "Cook" }, string.Empty, DUPLICANTS.CHORES.MUSH.NAME, DUPLICANTS.CHORES.MUSH.STATUS);
			this.MushFetch = this.Add("MushFetch", new string[] { "Cook", "Deliver" }, string.Empty, DUPLICANTS.CHORES.MUSHFETCH.NAME, DUPLICANTS.CHORES.MUSHFETCH.STATUS);
			this.CompostWorkable = this.Add("CompostWorkable", new string[0], string.Empty, DUPLICANTS.CHORES.COMPOSTWORKABLE.NAME, DUPLICANTS.CHORES.COMPOSTWORKABLE.STATUS);
			this.FlipCompost = this.Add("FlipCompost", new string[] { "Compost" }, string.Empty, DUPLICANTS.CHORES.FLIPCOMPOST.NAME, DUPLICANTS.CHORES.FLIPCOMPOST.STATUS);
			this.Fabricate = this.Add("Fabricate", new string[0], string.Empty, DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS);
			this.FabricateFetch = this.Add("FabricateFetch", new string[] { "Deliver" }, string.Empty, DUPLICANTS.CHORES.FABRICATEFETCH.NAME, DUPLICANTS.CHORES.FABRICATEFETCH.STATUS);
			this.Transport = this.Add("Transport", new string[] { "Sweep" }, string.Empty, DUPLICANTS.CHORES.TRANSPORT.NAME, DUPLICANTS.CHORES.TRANSPORT.STATUS);
			this.Build = this.Add("Build", new string[] { "Build" }, string.Empty, DUPLICANTS.CHORES.BUILD.NAME, DUPLICANTS.CHORES.BUILD.STATUS);
			this.BuildDig = this.Add("BuildDig", new string[] { "Dig" }, string.Empty, DUPLICANTS.CHORES.BUILDDIG.NAME, DUPLICANTS.CHORES.BUILDDIG.STATUS);
			this.BuildFetch = this.Add("BuildFetch", new string[] { "Build", "Deliver" }, string.Empty, DUPLICANTS.CHORES.BUILDFETCH.NAME, DUPLICANTS.CHORES.BUILDFETCH.STATUS);
			this.Dig = this.Add("Dig", new string[] { "Dig" }, string.Empty, DUPLICANTS.CHORES.DIG.NAME, DUPLICANTS.CHORES.DIG.STATUS);
			this.Fetch = this.Add("Fetch", new string[] { "Deliver", "Sweep" }, string.Empty, DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS);
			this.MoveToSafety = this.Add("MoveToSafety", new string[0], "MoveToSafety", DUPLICANTS.CHORES.MOVETOSAFETY.NAME, DUPLICANTS.CHORES.MOVETOSAFETY.STATUS);
			this.Idle = this.Add("IdleChore", new string[0], string.Empty, DUPLICANTS.CHORES.IDLE.NAME, DUPLICANTS.CHORES.IDLE.STATUS);
			ChoreType[][] array = new ChoreType[][]
			{
				new ChoreType[] { this.Die },
				new ChoreType[] { this.Entombed },
				new ChoreType[] { this.HealCritical },
				new ChoreType[] { this.BeIncapacitated },
				new ChoreType[] { this.DebugGoTo },
				new ChoreType[] { this.MoveTo },
				new ChoreType[] { this.MentalBreak },
				new ChoreType[] { this.StressVomit },
				new ChoreType[] { this.RecoverBreath },
				new ChoreType[] { this.EmoteHighPriority, this.StressActingOut, this.Vomit, this.Pee },
				new ChoreType[] { this.MoveToLocation },
				new ChoreType[] { this.ManualControlPrioritizeChore },
				new ChoreType[] { this.ManualControlGoTo },
				new ChoreType[] { this.ManualControlIdle },
				new ChoreType[] { this.MoveToQuarantine },
				new ChoreType[] { this.Attack },
				new ChoreType[] { this.Flee },
				new ChoreType[] { this.UseToilet, this.Eat },
				new ChoreType[] { this.Heal, this.RestDueToDisease, this.Relax, this.Sleep, this.SleepOnFloor, this.Warmup },
				new ChoreType[] { this.PrioritizeChore },
				new ChoreType[] { this.Equip, this.Unequip },
				new ChoreType[]
				{
					this.DeliverFood, this.Sigh, this.EmptyStorage, this.Upgrade, this.RescueIncapacitated, this.Repair, this.Shower, this.CleanToilet, this.LiquidCooledFan, this.WashHands,
					this.TakeMedicine, this.Doctor, this.Recharge, this.FetchCritical, this.MoveToSafety, this.Relocate, this.Research, this.Mop, this.Toggle, this.Deconstruct,
					this.Fetch, this.Transport, this.Art, this.GeneratePower, this.CompostWorkable, this.DropUnusedInventory, this.Harvest, this.Uproot, this.Fabricate, this.Mush,
					this.Cook, this.Build, this.Dig, this.BuildDig, this.FlipCompost, this.BuildFetch, this.CookFetch, this.MushFetch, this.FabricateFetch
				},
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
				Debug.LogError(text);
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
				Debug.LogError(text2);
			}
		}

		private ChoreType Add(string id, string[] chore_groups, string urge, string name, string status_message)
		{
			ChoreType choreType = new ChoreType(id, this, chore_groups, urge, name, status_message, this.nextPriority);
			this.nextPriority -= 100;
			return choreType;
		}

		public ChoreType Attack;

		public ChoreType Flee;

		public ChoreType BeIncapacitated;

		public ChoreType DebugGoTo;

		public ChoreType DeliverFood;

		public ChoreType Die;

		public ChoreType Doctor;

		public ChoreType WashHands;

		public ChoreType Shower;

		public ChoreType Eat;

		public ChoreType Entombed;

		public ChoreType Idle;

		public ChoreType ManualControlPrioritizeChore;

		public ChoreType ManualControlGoTo;

		public ChoreType ManualControlIdle;

		public ChoreType MentalBreak;

		public ChoreType MoveToQuarantine;

		public ChoreType RescueIncapacitated;

		public ChoreType RecoverBreath;

		public ChoreType Sigh;

		public ChoreType Sleep;

		public ChoreType SleepOnFloor;

		public ChoreType Vomit;

		public ChoreType PrioritizeChore;

		public ChoreType Pee;

		public ChoreType TakeMedicine;

		public ChoreType RestDueToDisease;

		public ChoreType Heal;

		public ChoreType HealCritical;

		public ChoreType EmoteHighPriority;

		public ChoreType StressActingOut;

		public ChoreType Relax;

		public ChoreType MoveToSafety;

		public ChoreType Equip;

		public ChoreType Recharge;

		public ChoreType Unequip;

		public ChoreType Warmup;

		public ChoreType Mop;

		public ChoreType Relocate;

		public ChoreType Toggle;

		public ChoreType Fetch;

		public ChoreType Upgrade;

		public ChoreType Repair;

		public ChoreType EmptyStorage;

		public ChoreType Deconstruct;

		public ChoreType Art;

		public ChoreType Research;

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

		public ChoreType Transport;

		public ChoreType DropUnusedInventory;

		public ChoreType FetchCritical;

		public ChoreType MoveToLocation;

		public ChoreType StressVomit;

		public ChoreType MoveTo;

		private int nextPriority = 10000;
	}
}
