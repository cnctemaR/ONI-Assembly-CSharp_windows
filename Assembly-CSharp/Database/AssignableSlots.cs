using System;
using STRINGS;
using TUNING;

namespace Database
{
	public class AssignableSlots : ResourceSet<AssignableSlot>
	{
		public AssignableSlots()
		{
			this.Bed = base.Add(new OwnableSlot("Bed", MISC.TAGS.BED));
			this.MessStation = base.Add(new OwnableSlot("MessStation", MISC.TAGS.MESSSTATION));
			this.Clinic = base.Add(new OwnableSlot("Clinic", MISC.TAGS.CLINIC));
			this.MedicalBed = base.Add(new OwnableSlot("MedicalBed", MISC.TAGS.CLINIC));
			this.MedicalBed.showInUI = false;
			this.GeneShuffler = base.Add(new OwnableSlot("GeneShuffler", MISC.TAGS.GENE_SHUFFLER));
			this.GeneShuffler.showInUI = false;
			this.Toilet = base.Add(new OwnableSlot("Toilet", MISC.TAGS.TOILET));
			this.MassageTable = base.Add(new OwnableSlot("MassageTable", MISC.TAGS.MASSAGE_TABLE));
			this.RocketCommandModule = base.Add(new OwnableSlot("RocketCommandModule", MISC.TAGS.COMMAND_MODULE));
			this.HabitatModule = base.Add(new OwnableSlot("HabitatModule", MISC.TAGS.HABITAT_MODULE));
			this.ResetSkillsStation = base.Add(new OwnableSlot("ResetSkillsStation", "ResetSkillsStation"));
			this.WarpPortal = base.Add(new OwnableSlot("WarpPortal", MISC.TAGS.WARP_PORTAL));
			this.WarpPortal.showInUI = false;
			this.BionicUpgrade = base.Add(new OwnableSlot("BionicUpgrade", MISC.TAGS.BIONICUPGRADE));
			this.Toy = base.Add(new EquipmentSlot(global::TUNING.EQUIPMENT.TOYS.SLOT, MISC.TAGS.TOY, false));
			this.Suit = base.Add(new EquipmentSlot(global::TUNING.EQUIPMENT.SUITS.SLOT, MISC.TAGS.SUIT, true));
			this.Tool = base.Add(new EquipmentSlot(global::TUNING.EQUIPMENT.TOOLS.TOOLSLOT, MISC.TAGS.MULTITOOL, false));
			this.Outfit = base.Add(new EquipmentSlot(global::TUNING.EQUIPMENT.CLOTHING.SLOT, UI.StripLinkFormatting(MISC.TAGS.CLOTHES), true));
			this.Shoes = base.Add(new EquipmentSlot(global::TUNING.EQUIPMENT.SHOES.SLOT, UI.StripLinkFormatting(MISC.TAGS.SHOES), true));
		}

		public AssignableSlot Bed;

		public AssignableSlot MessStation;

		public AssignableSlot Clinic;

		public AssignableSlot GeneShuffler;

		public AssignableSlot MedicalBed;

		public AssignableSlot Toilet;

		public AssignableSlot MassageTable;

		public AssignableSlot RocketCommandModule;

		public AssignableSlot HabitatModule;

		public AssignableSlot ResetSkillsStation;

		public AssignableSlot WarpPortal;

		public AssignableSlot Toy;

		public AssignableSlot Suit;

		public AssignableSlot Tool;

		public AssignableSlot Outfit;

		public AssignableSlot Shoes;

		public AssignableSlot BionicUpgrade;
	}
}
