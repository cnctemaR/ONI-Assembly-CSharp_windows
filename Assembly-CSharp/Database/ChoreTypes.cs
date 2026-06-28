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
			this.Die = this.Add("Die", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DIE.NAME, DUPLICANTS.CHORES.DIE.STATUS, DUPLICANTS.CHORES.DIE.TOOLTIP, false);
			this.Entombed = this.Add("Entombed", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.ENTOMBED.NAME, DUPLICANTS.CHORES.ENTOMBED.STATUS, DUPLICANTS.CHORES.ENTOMBED.TOOLTIP, false);
			this.SuitMarker = this.Add("SuitMarker", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false);
			this.Checkpoint = this.Add("Checkpoint", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.CHECKPOINT.NAME, DUPLICANTS.CHORES.CHECKPOINT.STATUS, DUPLICANTS.CHORES.CHECKPOINT.TOOLTIP, false);
			this.TravelTubeEntrance = this.Add("TravelTubeEntrance", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.NAME, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.STATUS, DUPLICANTS.CHORES.TRAVELTUBEENTRANCE.TOOLTIP, false);
			this.WashHands = this.Add("WashHands", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.WASHHANDS.NAME, DUPLICANTS.CHORES.WASHHANDS.STATUS, DUPLICANTS.CHORES.WASHHANDS.TOOLTIP, false);
			this.HealCritical = this.Add("HealCritical", new string[0], "HealCritical", new string[] { "Vomit", "Cough" }, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false);
			this.BeIncapacitated = this.Add("BeIncapacitated", new string[0], "BeIncapacitated", new string[0], DUPLICANTS.CHORES.BEINCAPACITATED.NAME, DUPLICANTS.CHORES.BEINCAPACITATED.STATUS, DUPLICANTS.CHORES.BEINCAPACITATED.TOOLTIP, false);
			this.GeneShuffle = this.Add("GeneShuffle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.GENESHUFFLE.NAME, DUPLICANTS.CHORES.GENESHUFFLE.STATUS, DUPLICANTS.CHORES.GENESHUFFLE.TOOLTIP, false);
			this.DebugGoTo = this.Add("DebugGoTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DEBUGGOTO.NAME, DUPLICANTS.CHORES.DEBUGGOTO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false);
			this.MoveTo = this.Add("MoveTo", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.MOVETO.NAME, DUPLICANTS.CHORES.MOVETO.STATUS, DUPLICANTS.CHORES.MOVETO.TOOLTIP, false);
			this.DropUnusedInventory = this.Add("DropUnusedInventory", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.NAME, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.STATUS, DUPLICANTS.CHORES.DROPUNUSEDINVENTORY.TOOLTIP, false);
			this.Pee = this.Add("Pee", new string[0], "Pee", new string[0], DUPLICANTS.CHORES.PEE.NAME, DUPLICANTS.CHORES.PEE.STATUS, DUPLICANTS.CHORES.PEE.TOOLTIP, false);
			this.RecoverBreath = this.Add("RecoverBreath", new string[0], "RecoverBreath", new string[0], DUPLICANTS.CHORES.RECOVERBREATH.NAME, DUPLICANTS.CHORES.RECOVERBREATH.STATUS, DUPLICANTS.CHORES.RECOVERBREATH.TOOLTIP, false);
			this.Flee = this.Add("Flee", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.FLEE.NAME, DUPLICANTS.CHORES.FLEE.STATUS, DUPLICANTS.CHORES.FLEE.TOOLTIP, false);
			this.MoveToQuarantine = this.Add("MoveToQuarantine", new string[0], "MoveToQuarantine", new string[0], DUPLICANTS.CHORES.MOVETOQUARANTINE.NAME, DUPLICANTS.CHORES.MOVETOQUARANTINE.STATUS, DUPLICANTS.CHORES.MOVETOQUARANTINE.TOOLTIP, false);
			this.Attack = this.Add("Attack", new string[] { "Combat" }, string.Empty, new string[0], DUPLICANTS.CHORES.ATTACK.NAME, DUPLICANTS.CHORES.ATTACK.STATUS, DUPLICANTS.CHORES.ATTACK.TOOLTIP, false);
			this.Emote = this.Add("Emote", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false);
			this.EmoteHighPriority = this.Add("EmoteHighPriority", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false);
			this.StressEmote = this.Add("StressEmote", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.NAME, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.STATUS, DUPLICANTS.CHORES.EMOTEHIGHPRIORITY.TOOLTIP, false);
			this.StressVomit = this.Add("StressVomit", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSVOMIT.NAME, DUPLICANTS.CHORES.STRESSVOMIT.STATUS, DUPLICANTS.CHORES.STRESSVOMIT.TOOLTIP, false);
			this.UglyCry = this.Add("UglyCry", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.UGLY_CRY.NAME, DUPLICANTS.CHORES.UGLY_CRY.STATUS, DUPLICANTS.CHORES.UGLY_CRY.TOOLTIP, false);
			this.BingeEat = this.Add("BingeEat", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.BINGE_EAT.NAME, DUPLICANTS.CHORES.BINGE_EAT.STATUS, DUPLICANTS.CHORES.BINGE_EAT.TOOLTIP, false);
			this.StressActingOut = this.Add("StressActingOut", new string[0], string.Empty, new string[] { "MoveTo" }, DUPLICANTS.CHORES.STRESSACTINGOUT.NAME, DUPLICANTS.CHORES.STRESSACTINGOUT.STATUS, DUPLICANTS.CHORES.STRESSACTINGOUT.TOOLTIP, false);
			this.Vomit = this.Add("Vomit", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.VOMIT.NAME, DUPLICANTS.CHORES.VOMIT.STATUS, DUPLICANTS.CHORES.VOMIT.TOOLTIP, false);
			this.Cough = this.Add("Cough", new string[0], "EmoteHighPriority", new string[0], DUPLICANTS.CHORES.COUGH.NAME, DUPLICANTS.CHORES.COUGH.STATUS, DUPLICANTS.CHORES.COUGH.TOOLTIP, false);
			this.StressIdle = this.Add("StressIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.STRESSIDLE.NAME, DUPLICANTS.CHORES.STRESSIDLE.STATUS, DUPLICANTS.CHORES.STRESSIDLE.TOOLTIP, false);
			this.RescueIncapacitated = this.Add("RescueIncapacitated", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RESCUEINCAPACITATED.NAME, DUPLICANTS.CHORES.RESCUEINCAPACITATED.STATUS, DUPLICANTS.CHORES.RESCUEINCAPACITATED.TOOLTIP, false);
			this.UseToilet = this.Add("UseToilet", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.USETOILET.NAME, DUPLICANTS.CHORES.USETOILET.STATUS, DUPLICANTS.CHORES.USETOILET.TOOLTIP, false);
			this.Eat = this.Add("Eat", new string[0], "Eat", new string[0], DUPLICANTS.CHORES.EAT.NAME, DUPLICANTS.CHORES.EAT.STATUS, DUPLICANTS.CHORES.EAT.TOOLTIP, false);
			this.Narcolepsy = this.Add("Narcolepsy", new string[0], "Narcolepsy", new string[0], DUPLICANTS.CHORES.NARCOLEPSY.NAME, DUPLICANTS.CHORES.NARCOLEPSY.STATUS, DUPLICANTS.CHORES.NARCOLEPSY.TOOLTIP, false);
			this.ReturnSuitUrgent = this.Add("ReturnSuitUrgent", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false);
			this.SleepDueToDisease = this.Add("SleepDueToDisease", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false);
			this.Sleep = this.Add("Sleep", new string[0], "Sleep", new string[0], DUPLICANTS.CHORES.SLEEP.NAME, DUPLICANTS.CHORES.SLEEP.STATUS, DUPLICANTS.CHORES.SLEEP.TOOLTIP, false);
			this.RestDueToDisease = this.Add("RestDueToDisease", new string[0], "RestDueToDisease", new string[0], DUPLICANTS.CHORES.RESTDUETODISEASE.NAME, DUPLICANTS.CHORES.RESTDUETODISEASE.STATUS, DUPLICANTS.CHORES.RESTDUETODISEASE.TOOLTIP, false);
			this.TakeMedicine = this.Add("TakeMedicine", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.TAKEMEDICINE.NAME, DUPLICANTS.CHORES.TAKEMEDICINE.STATUS, DUPLICANTS.CHORES.TAKEMEDICINE.TOOLTIP, false);
			this.ScrubOre = this.Add("ScrubOre", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SCRUBORE.NAME, DUPLICANTS.CHORES.SCRUBORE.STATUS, DUPLICANTS.CHORES.SCRUBORE.TOOLTIP, false);
			this.DeliverFood = this.Add("DeliverFood", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.DELIVERFOOD.NAME, DUPLICANTS.CHORES.DELIVERFOOD.STATUS, DUPLICANTS.CHORES.DELIVERFOOD.TOOLTIP, false);
			this.FetchCritical = this.Add("FetchCritical", new string[] { "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.Sigh = this.Add("Sigh", new string[0], "Emote", new string[0], DUPLICANTS.CHORES.SIGH.NAME, DUPLICANTS.CHORES.SIGH.STATUS, DUPLICANTS.CHORES.SIGH.TOOLTIP, false);
			this.Heal = this.Add("Heal", new string[0], "Heal", new string[] { "Vomit" }, DUPLICANTS.CHORES.HEAL.NAME, DUPLICANTS.CHORES.HEAL.STATUS, DUPLICANTS.CHORES.HEAL.TOOLTIP, false);
			this.Doctor = this.Add("DoctorChore", new string[] { "MedicalAid" }, "Doctor", new string[0], DUPLICANTS.CHORES.DOCTOR.NAME, DUPLICANTS.CHORES.DOCTOR.STATUS, DUPLICANTS.CHORES.DOCTOR.TOOLTIP, false);
			this.Shower = this.Add("Shower", new string[0], "Shower", new string[0], DUPLICANTS.CHORES.SHOWER.NAME, DUPLICANTS.CHORES.SHOWER.STATUS, DUPLICANTS.CHORES.SHOWER.TOOLTIP, false);
			this.Relax = this.Add("Relax", new string[0], "Relax", new string[0], DUPLICANTS.CHORES.RELAX.NAME, DUPLICANTS.CHORES.RELAX.STATUS, DUPLICANTS.CHORES.RELAX.TOOLTIP, false);
			this.Equip = this.Add("Equip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EQUIP.NAME, DUPLICANTS.CHORES.EQUIP.STATUS, DUPLICANTS.CHORES.EQUIP.TOOLTIP, false);
			this.Recharge = this.Add("Recharge", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RECHARGE.NAME, DUPLICANTS.CHORES.RECHARGE.STATUS, DUPLICANTS.CHORES.RECHARGE.TOOLTIP, false);
			this.SwitchHat = this.Add("SwitchHat", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.SWITCHROLE.NAME, DUPLICANTS.CHORES.SWITCHROLE.STATUS, DUPLICANTS.CHORES.SWITCHROLE.TOOLTIP, false);
			this.SwitchRole = this.Add("SwitchRole", new string[0], "SwitchRole", new string[0], DUPLICANTS.CHORES.SWITCHROLE.NAME, DUPLICANTS.CHORES.SWITCHROLE.STATUS, DUPLICANTS.CHORES.SWITCHROLE.TOOLTIP, false);
			this.Unequip = this.Add("Unequip", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.UNEQUIP.NAME, DUPLICANTS.CHORES.UNEQUIP.STATUS, DUPLICANTS.CHORES.UNEQUIP.TOOLTIP, false);
			this.Warmup = this.Add("Warmup", new string[0], "WarmUp", new string[0], DUPLICANTS.CHORES.WARMUP.NAME, DUPLICANTS.CHORES.WARMUP.STATUS, DUPLICANTS.CHORES.WARMUP.TOOLTIP, false);
			this.Cooldown = this.Add("Cooldown", new string[0], "CoolDown", new string[0], DUPLICANTS.CHORES.COOLDOWN.NAME, DUPLICANTS.CHORES.COOLDOWN.STATUS, DUPLICANTS.CHORES.COOLDOWN.TOOLTIP, false);
			this.EmptyStorage = this.Add("EmptyStorage", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.EMPTYSTORAGE.NAME, DUPLICANTS.CHORES.EMPTYSTORAGE.STATUS, DUPLICANTS.CHORES.EMPTYSTORAGE.TOOLTIP, false);
			this.Art = this.Add("Art", new string[] { "Art" }, string.Empty, new string[0], DUPLICANTS.CHORES.ART.NAME, DUPLICANTS.CHORES.ART.STATUS, DUPLICANTS.CHORES.ART.TOOLTIP, false);
			this.Mop = this.Add("Mop", new string[] { "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.MOP.NAME, DUPLICANTS.CHORES.MOP.STATUS, DUPLICANTS.CHORES.MOP.TOOLTIP, false);
			this.Relocate = this.Add("Relocate", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RELOCATE.NAME, DUPLICANTS.CHORES.RELOCATE.STATUS, DUPLICANTS.CHORES.RELOCATE.TOOLTIP, false);
			this.Toggle = this.Add("Toggle", new string[] { "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.TOGGLE.NAME, DUPLICANTS.CHORES.TOGGLE.STATUS, DUPLICANTS.CHORES.TOGGLE.TOOLTIP, false);
			this.Disinfect = this.Add("Disinfect", new string[] { "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.DISINFECT.NAME, DUPLICANTS.CHORES.DISINFECT.STATUS, DUPLICANTS.CHORES.DISINFECT.TOOLTIP, false);
			this.Repair = this.Add("Repair", new string[] { "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.REPAIR.NAME, DUPLICANTS.CHORES.REPAIR.STATUS, DUPLICANTS.CHORES.REPAIR.TOOLTIP, false);
			this.Deconstruct = this.Add("Deconstruct", new string[] { "Build" }, string.Empty, new string[0], DUPLICANTS.CHORES.DECONSTRUCT.NAME, DUPLICANTS.CHORES.DECONSTRUCT.STATUS, DUPLICANTS.CHORES.DECONSTRUCT.TOOLTIP, false);
			this.Research = this.Add("Research", new string[] { "Research" }, string.Empty, new string[0], DUPLICANTS.CHORES.RESEARCH.NAME, DUPLICANTS.CHORES.RESEARCH.STATUS, DUPLICANTS.CHORES.RESEARCH.TOOLTIP, false);
			this.ResearchFetch = this.Add("ResearchFetch", new string[] { "Research", "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.GeneratePower = this.Add("GeneratePower", new string[] { "MachineOperating" }, string.Empty, new string[] { "Relax" }, DUPLICANTS.CHORES.GENERATEPOWER.NAME, DUPLICANTS.CHORES.GENERATEPOWER.STATUS, DUPLICANTS.CHORES.GENERATEPOWER.TOOLTIP, false);
			this.CropTend = this.Add("CropTend", new string[] { "Farming" }, string.Empty, new string[] { "Relax" }, DUPLICANTS.CHORES.CROP_TEND.NAME, DUPLICANTS.CHORES.CROP_TEND.STATUS, DUPLICANTS.CHORES.CROP_TEND.TOOLTIP, false);
			this.PowerTinker = this.Add("PowerTinker", new string[] { "MachineOperating" }, string.Empty, new string[] { "Relax" }, DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false);
			this.MachineTinker = this.Add("MachineTinker", new string[] { "MachineOperating" }, string.Empty, new string[] { "Relax" }, DUPLICANTS.CHORES.POWER_TINKER.NAME, DUPLICANTS.CHORES.POWER_TINKER.STATUS, DUPLICANTS.CHORES.POWER_TINKER.TOOLTIP, false);
			this.TinkerFetch = this.Add("TinkerFetch", new string[] { "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.Harvest = this.Add("Harvest", new string[] { "Farming" }, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS, DUPLICANTS.CHORES.HARVEST.TOOLTIP, false);
			this.FarmFetch = this.Add("FarmFetch", new string[] { "Farming", "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.HARVEST.NAME, DUPLICANTS.CHORES.HARVEST.STATUS, DUPLICANTS.CHORES.HARVEST.TOOLTIP, false);
			this.Uproot = this.Add("Uproot", new string[] { "Farming" }, string.Empty, new string[0], DUPLICANTS.CHORES.UPROOT.NAME, DUPLICANTS.CHORES.UPROOT.STATUS, DUPLICANTS.CHORES.UPROOT.TOOLTIP, false);
			this.CleanToilet = this.Add("CleanToilet", new string[] { "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.CLEANTOILET.NAME, DUPLICANTS.CHORES.CLEANTOILET.STATUS, DUPLICANTS.CHORES.CLEANTOILET.TOOLTIP, false);
			this.LiquidCooledFan = this.Add("LiquidCooledFan", new string[] { "MachineOperating" }, string.Empty, new string[0], DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.NAME, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.STATUS, DUPLICANTS.CHORES.LIQUIDCOOLEDFAN.TOOLTIP, false);
			this.Cook = this.Add("Cook", new string[] { "Cook" }, string.Empty, new string[0], DUPLICANTS.CHORES.COOK.NAME, DUPLICANTS.CHORES.COOK.STATUS, DUPLICANTS.CHORES.COOK.TOOLTIP, false);
			this.CookFetch = this.Add("CookFetch", new string[] { "Cook", "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.PowerFetch = this.Add("PowerFetch", new string[] { "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.Mush = this.Add("Mush", new string[] { "Cook" }, string.Empty, new string[0], DUPLICANTS.CHORES.MUSH.NAME, DUPLICANTS.CHORES.MUSH.STATUS, DUPLICANTS.CHORES.MUSH.TOOLTIP, false);
			this.MushFetch = this.Add("MushFetch", new string[] { "Cook", "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.CompostWorkable = this.Add("CompostWorkable", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.COMPOSTWORKABLE.NAME, DUPLICANTS.CHORES.COMPOSTWORKABLE.STATUS, DUPLICANTS.CHORES.COMPOSTWORKABLE.TOOLTIP, false);
			this.FlipCompost = this.Add("FlipCompost", new string[] { "Farming" }, string.Empty, new string[0], DUPLICANTS.CHORES.FLIPCOMPOST.NAME, DUPLICANTS.CHORES.FLIPCOMPOST.STATUS, DUPLICANTS.CHORES.FLIPCOMPOST.TOOLTIP, false);
			this.Depressurize = this.Add("Depressurize", new string[] { "MachineOperating" }, string.Empty, new string[0], DUPLICANTS.CHORES.DEPRESSURIZE.NAME, DUPLICANTS.CHORES.DEPRESSURIZE.STATUS, DUPLICANTS.CHORES.DEPRESSURIZE.TOOLTIP, false);
			this.FarmingFabricate = this.Add("FarmingFabricate", new string[] { "Farming" }, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false);
			this.PowerFabricate = this.Add("PowerFabricate", new string[] { "MachineOperating" }, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false);
			this.Fabricate = this.Add("Fabricate", new string[] { "MachineOperating" }, string.Empty, new string[0], DUPLICANTS.CHORES.FABRICATE.NAME, DUPLICANTS.CHORES.FABRICATE.STATUS, DUPLICANTS.CHORES.FABRICATE.TOOLTIP, false);
			this.FabricateFetch = this.Add("FabricateFetch", new string[] { "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.Transport = this.Add("Transport", new string[] { "Hauling", "Basekeeping" }, string.Empty, new string[0], DUPLICANTS.CHORES.TRANSPORT.NAME, DUPLICANTS.CHORES.TRANSPORT.STATUS, DUPLICANTS.CHORES.TRANSPORT.TOOLTIP, false);
			this.Build = this.Add("Build", new string[] { "Build" }, string.Empty, new string[0], DUPLICANTS.CHORES.BUILD.NAME, DUPLICANTS.CHORES.BUILD.STATUS, DUPLICANTS.CHORES.BUILD.TOOLTIP, true);
			this.BuildFetch = this.Add("BuildFetch", new string[] { "Build", "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.BUILDFETCH.NAME, DUPLICANTS.CHORES.BUILDFETCH.STATUS, DUPLICANTS.CHORES.BUILDFETCH.TOOLTIP, true);
			this.Dig = this.Add("Dig", new string[] { "Dig" }, string.Empty, new string[0], DUPLICANTS.CHORES.DIG.NAME, DUPLICANTS.CHORES.DIG.STATUS, DUPLICANTS.CHORES.DIG.TOOLTIP, false);
			this.Fetch = this.Add("Fetch", new string[] { "Hauling" }, string.Empty, new string[0], DUPLICANTS.CHORES.FETCH.NAME, DUPLICANTS.CHORES.FETCH.STATUS, DUPLICANTS.CHORES.FETCH.TOOLTIP, false);
			this.MoveToSafety = this.Add("MoveToSafety", new string[0], "MoveToSafety", new string[0], DUPLICANTS.CHORES.MOVETOSAFETY.NAME, DUPLICANTS.CHORES.MOVETOSAFETY.STATUS, DUPLICANTS.CHORES.MOVETOSAFETY.TOOLTIP, false);
			this.ReturnSuitIdle = this.Add("ReturnSuitIdle", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.RETURNSUIT.NAME, DUPLICANTS.CHORES.RETURNSUIT.STATUS, DUPLICANTS.CHORES.RETURNSUIT.TOOLTIP, false);
			this.Idle = this.Add("IdleChore", new string[0], string.Empty, new string[0], DUPLICANTS.CHORES.IDLE.NAME, DUPLICANTS.CHORES.IDLE.STATUS, DUPLICANTS.CHORES.IDLE.TOOLTIP, false);
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
				new ChoreType[] { this.EmoteHighPriority, this.StressActingOut, this.Vomit, this.Cough, this.Pee, this.StressIdle, this.RescueIncapacitated, this.SwitchHat },
				new ChoreType[] { this.MoveToQuarantine },
				new ChoreType[] { this.Attack },
				new ChoreType[] { this.Flee },
				new ChoreType[] { this.SwitchRole, this.UseToilet, this.Eat },
				new ChoreType[] { this.Heal, this.SleepDueToDisease, this.RestDueToDisease },
				new ChoreType[] { this.Sleep, this.Narcolepsy, this.Warmup, this.Cooldown },
				new ChoreType[] { this.Emote },
				new ChoreType[] { this.Relax },
				new ChoreType[] { this.Equip, this.Unequip },
				new ChoreType[]
				{
					this.DeliverFood, this.Sigh, this.EmptyStorage, this.Repair, this.Disinfect, this.Shower, this.CleanToilet, this.LiquidCooledFan, this.SuitMarker, this.Checkpoint,
					this.TravelTubeEntrance, this.WashHands, this.TakeMedicine, this.Doctor, this.Recharge, this.FetchCritical, this.ScrubOre, this.MoveToSafety, this.Relocate, this.Research,
					this.ResearchFetch, this.Mop, this.Toggle, this.Deconstruct, this.Fetch, this.Transport, this.Art, this.GeneratePower, this.CropTend, this.PowerTinker,
					this.MachineTinker, this.CompostWorkable, this.DropUnusedInventory, this.Harvest, this.FarmFetch, this.Uproot, this.FarmingFabricate, this.PowerFabricate, this.Fabricate, this.Mush,
					this.Cook, this.Build, this.Dig, this.FlipCompost, this.Depressurize, this.BuildFetch, this.CookFetch, this.TinkerFetch, this.PowerFetch, this.MushFetch,
					this.FabricateFetch, this.StressEmote, this.ReturnSuitUrgent
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

		public ChoreType GetByHash(HashedString id_hash)
		{
			int num = this.resources.FindIndex((ChoreType item) => item.IdHash == id_hash);
			if (num != -1)
			{
				return this.resources[num];
			}
			return null;
		}

		private ChoreType Add(string id, string[] chore_groups, string urge, string[] interrupt_exclusion, string name, string status_message, string tooltip, bool skip_priority_change)
		{
			List<Tag> list = new List<Tag>();
			for (int i = 0; i < interrupt_exclusion.Length; i++)
			{
				list.Add(TagManager.Create(interrupt_exclusion[i], null));
			}
			ChoreType choreType = new ChoreType(id, this, chore_groups, urge, name, status_message, tooltip, list.ToArray(), this.nextPriority);
			if (!skip_priority_change)
			{
				this.nextPriority -= 100;
			}
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

		public ChoreType FetchCritical;

		public ChoreType ResearchFetch;

		public ChoreType FarmFetch;

		public ChoreType FabricateFetch;

		public ChoreType MushFetch;

		public ChoreType CookFetch;

		public ChoreType PowerFetch;

		public ChoreType BuildFetch;

		public ChoreType Disinfect;

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

		public ChoreType FarmingFabricate;

		public ChoreType PowerFabricate;

		public ChoreType Mush;

		public ChoreType Cook;

		public ChoreType Build;

		public ChoreType Dig;

		public ChoreType FlipCompost;

		public ChoreType PowerTinker;

		public ChoreType MachineTinker;

		public ChoreType TinkerFetch;

		public ChoreType CropTend;

		public ChoreType Depressurize;

		public ChoreType Transport;

		public ChoreType DropUnusedInventory;

		public ChoreType StressVomit;

		public ChoreType MoveTo;

		public ChoreType UglyCry;

		public ChoreType BingeEat;

		public ChoreType StressIdle;

		public ChoreType ScrubOre;

		public ChoreType SuitMarker;

		public ChoreType ReturnSuitUrgent;

		public ChoreType ReturnSuitIdle;

		public ChoreType Checkpoint;

		public ChoreType TravelTubeEntrance;

		public ChoreType SwitchRole;

		public ChoreType SwitchHat;

		private int nextPriority = 10000;
	}
}
