using System;

namespace STRINGS
{
	public class ELEMENTS
	{
		public static LocString STATESOLID = "Solid";

		public static LocString STATELIQUID = "Liquid";

		public static LocString STATEGAS = "Gas";

		public static LocString STATEVACUUM = "None";

		public static LocString ELEMENTDESCSOLID = "Resource Type: {0}\nMelting point: {1}\nHardness: {2}";

		public static LocString ELEMENTDESCLIQUID = "Resource Type: {0}\nFreezing point: {1}\nEvaporation point: {2}";

		public static LocString ELEMENTDESCGAS = "Resource Type: {0}\nCondensation point: {1}";

		public static LocString ELEMENTDESCVACUUM = "Resource Type: {0}";

		public static LocString BREATHABLEDESC = "<color=#{0}>({1})</color>";

		public static LocString ELEMENTPROPERTIES = "Properties: {0}";

		public class HARDNESS
		{
			public static LocString NA = "N/A";

			public static LocString SOFT = "{0} (Soft)";

			public static LocString VERYSOFT = "{0} (Very Soft)";

			public static LocString FIRM = "{0} (Firm)";

			public static LocString VERYFIRM = "{0} (Very Firm)";

			public static LocString NEARLYIMPENETRABLE = "{0} (Nearly Impenetrable)";

			public static LocString IMPENETRABLE = "{0} (Impenetrable)";
		}

		public class ALGAE
		{
			public static LocString NAME = "Algae";

			public static LocString DESC = "Algae is a cluster of non-motile, single-celled lifeforms.\n\nIt can be used as a renewable source of <style=\"oxygen\">Oxygen</style> when grown domestically in Algae Terrariums.";
		}

		public class BLEACHSTONE
		{
			public static LocString NAME = "Bleach Stone";

			public static LocString DESC = "Bleach stone is an unstable compound that emits toxic <style=\"gas\">Chlorine Gas</style>.\n\nIt is useful in <style=\"hygiene\">Hygienic</style> processes.";
		}

		public class BOTTLEDWATER
		{
			public static LocString NAME = "Water";

			public static LocString DESC = "Clean <style=\"liquid\">Water</style>, prepped for transport.";
		}

		public class CARBON
		{
			public static LocString NAME = "Coal";

			public static LocString DESC = "Coal is a combustible fossil fuel composed of carbon.\n\nIt is useful in <style=\"power\">Power</style> production.";
		}

		public class CARBONDIOXIDE
		{
			public static LocString NAME = "Carbon Dioxide";

			public static LocString DESC = "(CO2) Carbon Dioxide toxic, atomically heavy chemical compound in a <style=\"gas\">Gaseous</style> state.\n\nIt tends to sink below other gases.";
		}

		public class CARBONFIBRE
		{
			public static LocString NAME = "Carbon Fiber";

			public static LocString DESC = "Carbon Fiber is a <style=\"RefinedMineral\">Manufactured Material</style> with high tensile strength.";
		}

		public class CARBONGAS
		{
			public static LocString NAME = "Carbon";

			public static LocString DESC = "(C) Carbon is an abundant, versatile element heated into a <style=\"gas\">Gaseous</style> state.";
		}

		public class CHLORINE
		{
			public static LocString NAME = "Chlorine";

			public static LocString DESC = "(Cl) Chlorine is an extremely toxic element in a <style=\"solid\">Solid</style> state.";
		}

		public class CHLORINEGAS
		{
			public static LocString NAME = "Chlorine";

			public static LocString DESC = "(Cl) Chlorine is an extremely toxic chemical in a <style=\"gas\">Gaseous</style> state.";
		}

		public class CLAY
		{
			public static LocString NAME = "Clay";

			public static LocString DESC = "Clay is a soft, naturally occurring composite of stone and soil that hardens at high <style=\"heat\">Temperatures</style>.\n\nIt is a reliable <style=\"RawMineral\">Construction Material</style>.";
		}

		public class CONTAMINATEDOXYGEN
		{
			public static LocString NAME = "Polluted Oxygen";

			public static LocString DESC = "(O2) Polluted Oxygen vehicle of airborne <style=\"disease\">Disease</style>.\n\nIt is breathable.";
		}

		public class COPPER
		{
			public static LocString NAME = "Copper";

			public static LocString DESC = "(Cu) Copper is a conductive <style=\"RawMetal\">Metal</style>.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class COPPERGAS
		{
			public static LocString NAME = "Copper";

			public static LocString DESC = "(Cu) Copper is a conductive <style=\"RawMetal\">Metal</style> heated into a <style=\"gas\">Gas</style>.";
		}

		public class CREATURE
		{
			public static LocString NAME = "Genetic Ooze";

			public static LocString DESC = "(DuPe) Ooze is a slurry of water, carbon, and dozens and dozens of trace elements.\n\nDuplicants are printed from pure <style=\"solid\">Ooze</style>.";
		}

		public class CRUDEOIL
		{
			public static LocString NAME = "Crude Oil";

			public static LocString DESC = "Crude Oil is a raw <style=\"power\">Power</style> source composed of billions of dead, primordial organisms.";
		}

		public class CRUSHEDICE
		{
			public static LocString NAME = "Crushed Ice";

			public static LocString DESC = "(H20) A slush of crushed, semi-solid ice.";
		}

		public class CRUSHEDROCK
		{
			public static LocString NAME = "Crushed Rock";

			public static LocString DESC = "Crushed Rock is <style=\"solid\">Igneous Rock</style> crushed into a mechanical mixture.";
		}

		public class CUPRITE
		{
			public static LocString NAME = "Copper Ore";

			public static LocString DESC = "(Cu20) Copper Ore is a conductive <style=\"RawMetal\">Metal</style>.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class DIAMOND
		{
			public static LocString NAME = "Diamond";

			public static LocString DESC = "(C) Diamond are industrial-grade, high density carbon.\n\nIt is very difficult to excavate.";
		}

		public class DIRT
		{
			public static LocString NAME = "Dirt";

			public static LocString DESC = "Dirt is a soft, nutrient-rich substance capable of supporting life.\n\nIt is necessary in some forms of <style=\"food\">Food</style> production.";
		}

		public class DIRTYICE
		{
			public static LocString NAME = "Polluted Ice";

			public static LocString DESC = "Polluted Ice is <style=\"disease\">Disease</style> infested water frozen into a <style=\"solid\">Solid</style> state.";
		}

		public class DIRTYWATER
		{
			public static LocString NAME = "Polluted Water";

			public static LocString DESC = "Polluted Water is <style=\"disease\">Disease</style> infested <style=\"liquid\">Water</style>.\n\nIt is not fit for consumption.";
		}

		public class ELECTRUM
		{
			public static LocString NAME = "Electrum";

			public static LocString DESC = "Electrum is a conductive <style=\"RawMetal\">Metal</style> alloy composed of gold and silver.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class FERTILIZER
		{
			public static LocString NAME = "Fertilizer";

			public static LocString DESC = "Fertilizer is a processed mixture of biological nutrients.\n\nIt aids in the growth of certain <style=\"plant\">Plants</style>.";
		}

		public class PONDSCUM
		{
			public static LocString NAME = "Pondscum";

			public static LocString DESC = "Pondscum is a soft, naturally occurring composite of biological nutrients.\n\nIt may be processed into <style=\"solid\">Fertilizer</style> and aids in the growth of certain <style=\"plant\">Plants</style>.";
		}

		public class FOOLSGOLD
		{
			public static LocString NAME = "Pyrite";

			public static LocString DESC = "(FeS2) Pyrite is a conductive <style=\"RawMetal\">Metal</style>.\n\nAlso known as \"Fool's Gold\", is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class GOLD
		{
			public static LocString NAME = "Gold";

			public static LocString DESC = "(Au) Gold is a conductive precious <style=\"RawMetal\">Metal</style>.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class GOLDAMALGAM
		{
			public static LocString NAME = "Gold Amalgam";

			public static LocString DESC = "Gold Amalgam is a conductive amalgam of gold and mercury.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class GOLDGAS
		{
			public static LocString NAME = "Gold";

			public static LocString DESC = "(Au) Gold is a conductive precious <style=\"RawMetal\">Metal</style>, heated into a <style=\"gas\">Gas</style>.";
		}

		public class GRANITE
		{
			public static LocString NAME = "Granite";

			public static LocString DESC = "Granite is a dense composite of <style=\"RawMineral\">Igneous Rock</style>.\n\nIt is useful as a <style=\"RawMineral\">Construction Material</style>.";
		}

		public class HELIUM
		{
			public static LocString NAME = "Helium";

			public static LocString DESC = "(He) Helium is an atomically lightweight, chemical <style=\"gas\">Gas</style>.";
		}

		public class HYDROGEN
		{
			public static LocString NAME = "Hydrogen";

			public static LocString DESC = "(H) Hydrogen is the universe's most common and atomically light element in a <style=\"gas\">Gaseous</style> state.";
		}

		public class METHANE
		{
			public static LocString NAME = "Natural Gas";

			public static LocString DESC = "Natural Gas is a mixture of various alkanes in a <style=\"gas\">Gaseous</style> state.\n\nIt is useful in <style=\"power\">Power</style> production.";
		}

		public class ICE
		{
			public static LocString NAME = "Ice";

			public static LocString DESC = "(H20) Ice is clean water frozen into a <style=\"solid\">Solid</style> state.";
		}

		public class IGNEOUSROCK
		{
			public static LocString NAME = "Igneous Rock";

			public static LocString DESC = "Igneous Rock is a composite of solidified volcanic rock.\n\nIt is useful as a <style=\"RawMineral\">Construction Material</style>.";
		}

		public class IRON
		{
			public static LocString NAME = "Iron";

			public static LocString DESC = "(Fe) Iron is a common industrial <style=\"RawMetal\">Metal</style>.";
		}

		public class IRONINGOT
		{
			public static LocString NAME = "Iron";

			public static LocString DESC = "Iron is refined <style=\"RawMetal\">Iron Ore</style>.";
		}

		public class IRONGAS
		{
			public static LocString NAME = "Iron";

			public static LocString DESC = "(Fe) Iron is a common industrial <style=\"RawMetal\">Metal</style>, heated into a <style=\"gas\">Gas</style>.";
		}

		public class IRONORE
		{
			public static LocString NAME = "Iron Ore";

			public static LocString DESC = "(Fe) Iron Ore is a soft <style=\"RawMetal\">Metal</style> that can be strengthened for industrial purposes via smelting.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class KATAIRITE
		{
			public static LocString NAME = "Abyssalite";

			public static LocString DESC = "(Ab) Abyssalite is a resilient, crystalline element.";
		}

		public class LIQUIDCARBONDIOXIDE
		{
			public static LocString NAME = "Carbon Dioxide";

			public static LocString DESC = "(CO2) Carbon Dioxide is a toxic chemical compound. This selection is currently in a <style=\"liquid\">Liquid</style> state.";
		}

		public class LIQUIDHELIUM
		{
			public static LocString NAME = "Helium";

			public static LocString DESC = "(He) Helium is an atomically lightweight chemical element cooled into a <style=\"liquid\">Liquid</style> state.";
		}

		public class LIQUIDHYDROGEN
		{
			public static LocString NAME = "Hydrogen";

			public static LocString DESC = "(H) Hydrogen is a chemical <style=\"liquid\">Liquid</style>.\n\nIt freezes most substances that come into contact with it.";
		}

		public class LIQUIDOXYGEN
		{
			public static LocString NAME = "Oxygen";

			public static LocString DESC = "(O2) Oxygen is a breathable chemical.\n\nThis selection is in a <style=\"liquid\">Liquid</style> state.";
		}

		public class LIQUIDMETHANE
		{
			public static LocString NAME = "Methane";

			public static LocString DESC = "(CH4) Methane is an alkane.\n\nThis selection is in a <style=\"liquid\">Liquid</style> state.";
		}

		public class LIQUIDPHOSPHORUS
		{
			public static LocString NAME = "Phosphorus";

			public static LocString DESC = "(P) Phosphorus is a chemical element.\n\nThis selection is in a <style=\"liquid\">Liquid</style> state.";
		}

		public class LIQUIDPROPANE
		{
			public static LocString NAME = "Propane";

			public static LocString DESC = "(C3H8) Propane is an alkane in a <style=\"liquid\">Liquid</style> state.\n\nIt is useful in <style=\"power\">Power</style> production.";
		}

		public class MAGMA
		{
			public static LocString NAME = "Magma";

			public static LocString DESC = "Magma is a composite of <style=\"RawMineral\">Igneous Rock</style> heated into a molten, <style=\"liquid\">Liquid</style> state.";
		}

		public class MERCURY
		{
			public static LocString NAME = "Mercury";

			public static LocString DESC = "(Hg) Mercury is a toxic, <style=\"RawMetal\">Metallic</style> <style=\"liquid\">Liquid</style>.";
		}

		public class MERCURYGAS
		{
			public static LocString NAME = "Mercury";

			public static LocString DESC = "(Hg) Mercury is a toxic <style=\"RawMetal\">Metal</style> heated into a <style=\"gas\">Gaseous</style> state.";
		}

		public class MOLTENCARBON
		{
			public static LocString NAME = "Carbon";

			public static LocString DESC = "(C) Carbon is an abundant, versatile element heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENCOPPER
		{
			public static LocString NAME = "Copper";

			public static LocString DESC = "(Cu) Copper is a conductive <style=\"RawMetal\">Metal</style> heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENGLASS
		{
			public static LocString NAME = "Molten Glass";

			public static LocString DESC = "Molten Glass is a composite of granular rock heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENGOLD
		{
			public static LocString NAME = "Gold";

			public static LocString DESC = "(Au) Gold is a conductive precious <style=\"RawMetal\">Metal</style> heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENIRON
		{
			public static LocString NAME = "Iron";

			public static LocString DESC = "(Fe) Iron is a common industrial <style=\"RawMetal\">Metal</style> heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENTUNGSTEN
		{
			public static LocString NAME = "Tungsten";

			public static LocString DESC = "(W) Tungsten is a crystalline <style=\"RawMetal\">Metal</style> heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENTUNGSTENDISELENIDE
		{
			public static LocString NAME = "Tungsten Diselenide";

			public static LocString DESC = "(WSe2) Tungsten Diselenide is an inorganic <style=\"RawMetal\">Metal</style> compound heated into a <style=\"liquid\">Liquid</style> state.";
		}

		public class MOLTENSTEEL
		{
			public static LocString NAME = "Steel";

			public static LocString DESC = "Steel is a <style=\"RawMetal\">Metal</style> alloy of iron and carbon, heated into a hazardous <style=\"liquid\">Liquid</style> state.";
		}

		public class OBSIDIAN
		{
			public static LocString NAME = "Obsidian";

			public static LocString DESC = "Obsidian is a brittle composite of volcanic <style=\"solid\">Glass</style>.";
		}

		public class OXYGEN
		{
			public static LocString NAME = "Oxygen";

			public static LocString DESC = "(O2) Oxygen is an atomically lightweight and breathable <style=\"gas\">Gas</style>, necessary for sustaining life.\n\nIt tends to rise above other gases.";
		}

		public class OXYROCK
		{
			public static LocString NAME = "Oxylite";

			public static LocString DESC = "(Ir3O2) Oxylite is a chemical compound that slowly emits breathable <style=\"oxygen\">Oxygen</style>.\n\nThe rate of oxygen emission will increase when excavated, but it will lose mass and deplete more rapidly.";
		}

		public class PHOSPHATENODULES
		{
			public static LocString NAME = "Phosphate Nodules";

			public static LocString DESC = "(PO3-4) Nodules of sedimentary rock containing high concentrations of phosphate.";
		}

		public class PHOSPHORITE
		{
			public static LocString NAME = "Phosphorite";

			public static LocString DESC = "Phosphorite is a composite of sedimentary rock, saturated with phosphate.";
		}

		public class PHOSPHORUS
		{
			public static LocString NAME = "Phosphorus";

			public static LocString DESC = "(P) Phosphorus is a chemical element in its <style=\"solid\">Solid</style> state.";
		}

		public class PHOSPHORUSGAS
		{
			public static LocString NAME = "Phosphorus";

			public static LocString DESC = "(P) Phosphorus is a chemical element in a <style=\"gas\">Gaseous</style> state.";
		}

		public class PROPANE
		{
			public static LocString NAME = "Propane";

			public static LocString DESC = "(C3H8) Propane is a natural alkane <style=\"gas\">Gas</style>.\n\nIt is useful in <style=\"power\">Power</style> production.";
		}

		public class RADIUM
		{
			public static LocString NAME = "Radium";

			public static LocString DESC = "(Ra) Radium is a <style=\"light\">Light</style> emitting radioactive substance.\n\nIt is useful as a <style=\"power\">Power</style> source.";
		}

		public class ROCKGAS
		{
			public static LocString NAME = "Rock Gas";

			public static LocString DESC = "Rock Gas is rock that has been superheated into a <style=\"gas\">Gaseous</style> state.";
		}

		public class SAND
		{
			public static LocString NAME = "Sand";

			public static LocString DESC = "Sand is a composite of granular rock.\n\nIt is useful as a <style=\"misc\">Filtration Medium</style>.";
		}

		public class SANDSTONE
		{
			public static LocString NAME = "Sandstone";

			public static LocString DESC = "Sandstone is a composite of relatively soft sedimentary rock.\n\nIt is useful as a <style=\"RawMineral\">Construction Material</style>.";
		}

		public class SEDIMENTARYROCK
		{
			public static LocString NAME = "Sedimentary Rock";

			public static LocString DESC = "Sedimentary Rock is a hardened composite of sediment layers.\n\nIt is useful as a <style=\"RawMineral\">Construction Material</style>.";
		}

		public class SLIMEMOLD
		{
			public static LocString NAME = "Slime";

			public static LocString DESC = "Slime is a thick biomixture of algae, fungi, and mucopolysaccarides.\n\nIt can be distilled into <style=\"solid\">Algae</style> and is useful in some <style=\"oxygen\">Oxygen</style> production processes.";
		}

		public class SNOW
		{
			public static LocString NAME = "Snow";

			public static LocString DESC = "(H20) Snow is a mass of loose, crystalline ice particles.\n\nIt becomes <style=\"liquid\">Water</style> when melted.";
		}

		public class SOLIDCARBONDIOXIDE
		{
			public static LocString NAME = "Carbon Dioxide";

			public static LocString DESC = "(CO2) Carbon Dioxide is a toxic compound in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDCHLORINE
		{
			public static LocString NAME = "Chlorine";

			public static LocString DESC = "(Cl) Chlorine is a toxic chemical element in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDHYDROGEN
		{
			public static LocString NAME = "Hydrogen";

			public static LocString DESC = "(H) Hydrogen is the universe's most common element in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDMERCURY
		{
			public static LocString NAME = "Mercury";

			public static LocString DESC = "(Hg) Mercury is a toxic <style=\"RawMetal\">Metal</style> in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDOXYGEN
		{
			public static LocString NAME = "Oxygen";

			public static LocString DESC = "(O2) Oxygen is a breathable element in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDMETHANE
		{
			public static LocString NAME = "Methane";

			public static LocString DESC = "(CH4) Methane is an alkane in a <style=\"solid\">Solid</style> state.";
		}

		public class SOLIDPROPANE
		{
			public static LocString NAME = "Propane";

			public static LocString DESC = "(C3H8) Propane is a natural gas in a <style=\"solid\">Solid</style> state.";
		}

		public class STEAM
		{
			public static LocString NAME = "Steam";

			public static LocString DESC = "(H20) Steam is water that has been heated into a scalding <style=\"gas\">Gas</style>.";
		}

		public class STEEL
		{
			public static LocString NAME = "Steel";

			public static LocString DESC = "Steel is a <style=\"RefinedMetal\">Metal Alloy</style> composed of iron and carbon.";
		}

		public class STEELGAS
		{
			public static LocString NAME = "Steel";

			public static LocString DESC = "Steel is a superheated <style=\"RawMetal\">Metal</style> <style=\"gas\">Gas</style> composed of iron and carbon.";
		}

		public class TUNGSTEN
		{
			public static LocString NAME = "Tungsten";

			public static LocString DESC = "(W) Tungsten is an extremely tough crystalline <style=\"RawMetal\">Metal</style>.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class TUNGSTENGAS
		{
			public static LocString NAME = "Tungsten";

			public static LocString DESC = "(W) Tungsten is a superheated crystalline <style=\"RawMetal\">Metal</style> in a <style=\"gas\">Gaseous</style> state.";
		}

		public class TUNGSTENDISELENIDE
		{
			public static LocString NAME = "Tungsten Diselenide";

			public static LocString DESC = "(WSe2) Tungsten Diselenide is an inorganic <style=\"RawMetal\">Metal</style> compound with a crystalline structure.\n\nIt is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class TUNGSTENDISELENIDEGAS
		{
			public static LocString NAME = "Tungsten Diselenide";

			public static LocString DESC = "(WSe2) Tungsten Diselenide is a superheated <style=\"RawMetal\">Metal</style> compound in a <style=\"gas\">Gaseous</style> state.";
		}

		public class TOXICSAND
		{
			public static LocString NAME = "Polluted Dirt";

			public static LocString DESC = "Polluted Dirt is a <style=\"disease\">Disease</style>-infested amalgam of toxic waste.\n\nIt emits <style=\"disease\">Polluted Oxygen</style> over time.";
		}

		public class UNOBTANIUM
		{
			public static LocString NAME = "Neutronium";

			public static LocString DESC = "(Nt) Neutronium is a mysterious and extremely resilient <style=\"RawMetal\">Metallic</style> element.\n\nIt cannot be excavated by any Duplicant mining tool.";
		}

		public class VACUUM
		{
			public static LocString NAME = "Vacuum";

			public static LocString DESC = "A vacuum is a space devoid of all matter.";
		}

		public class VOID
		{
			public static LocString NAME = "The Void";

			public static LocString DESC = "Cold, infinite nothingness.";
		}

		public class WATER
		{
			public static LocString NAME = "Water";

			public static LocString DESC = "(H20) Clean <style=\"liquid\">Water</style>, suitable for consumption.";
		}

		public class WOLFRAMITE
		{
			public static LocString NAME = "Wolframite";

			public static LocString DESC = "((Fe,Mn)WO4) Wolframite is a dense <style=\"RawMetal\">Metallic</style> element in a <style=\"solid\">Solid</style> state.\n\nIt is a source of <style=\"RawMetal\">Tungsten</style> and is suitable for building <style=\"power\">Power</style> systems.";
		}

		public class STEELDOOR
		{
			public static LocString NAME = "Steel Door <DO NOT TRANSLATE>";

			public static LocString DESC = "<DO NOT TRANSLATE>";
		}
	}
}
