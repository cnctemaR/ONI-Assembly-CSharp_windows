using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class MooTuning
{
	public static List<BeckoningMonitor.SongChance> BaseSongChances = new List<BeckoningMonitor.SongChance>
	{
		new BeckoningMonitor.SongChance
		{
			meteorID = GassyMooCometConfig.ID,
			singAnimPre = "beckoning_pre",
			singAnimLoop = "beckoning_loop",
			singAnimPst = "beckoning_pst",
			weight = 0.98f
		},
		new BeckoningMonitor.SongChance
		{
			meteorID = DieselMooCometConfig.ID,
			singAnimPre = "diesel_beckoning_pre",
			singAnimLoop = "diesel_beckoning_loop",
			singAnimPst = "diesel_beckoning_pst",
			weight = 0.02f
		}
	};

	public static List<BeckoningMonitor.SongChance> DieselSongChances = new List<BeckoningMonitor.SongChance>
	{
		new BeckoningMonitor.SongChance
		{
			meteorID = GassyMooCometConfig.ID,
			singAnimPre = "beckoning_pre",
			singAnimLoop = "beckoning_loop",
			singAnimPst = "beckoning_pst",
			weight = 0.3f
		},
		new BeckoningMonitor.SongChance
		{
			meteorID = DieselMooCometConfig.ID,
			singAnimPre = "diesel_beckoning_pre",
			singAnimLoop = "diesel_beckoning_loop",
			singAnimPst = "diesel_beckoning_pst",
			weight = 0.6f
		}
	};

	public static readonly float STANDARD_LIFESPAN = 75f;

	public static readonly float STANDARD_CALORIES_PER_CYCLE = 200000f;

	public static readonly float STANDARD_STARVE_CYCLES = 6f;

	public static readonly float STANDARD_STOMACH_SIZE = MooTuning.STANDARD_CALORIES_PER_CYCLE * MooTuning.STANDARD_STARVE_CYCLES;

	public static readonly int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER4;

	public static readonly float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 2f;

	public static float KG_SOLIDS_EATEN_PER_DAY = 200f;

	public static float CALORIES_PER_DAY_OF_SOLID_EATEN = MooTuning.STANDARD_CALORIES_PER_CYCLE / MooTuning.KG_SOLIDS_EATEN_PER_DAY;

	public static float CALORIES_PER_DAY_OF_PLANT_EATEN = MooTuning.STANDARD_CALORIES_PER_CYCLE / MooTuning.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	public static float KG_POOP_PER_DAY_OF_PLANT = 5f;

	public static float POOP_KG_COVERSION_RATE_FOR_SOLID_DIET = 10f / MooTuning.KG_SOLIDS_EATEN_PER_DAY;

	public static float MIN_POOP_SIZE_IN_KG = 1.5f;

	public static float MIN_POOP_SIZE_IN_CALORIES = MooTuning.CALORIES_PER_DAY_OF_PLANT_EATEN * MooTuning.MIN_POOP_SIZE_IN_KG / MooTuning.KG_POOP_PER_DAY_OF_PLANT;

	private static readonly float BECKONS_PER_LIFESPAN = 4f;

	private static readonly float BECKON_FUDGE_CYCLES = 11f;

	private static readonly float BECKON_CYCLES = Mathf.Floor((MooTuning.STANDARD_LIFESPAN - MooTuning.BECKON_FUDGE_CYCLES) / MooTuning.BECKONS_PER_LIFESPAN);

	public static readonly float WELLFED_EFFECT = 100f / (600f * MooTuning.BECKON_CYCLES);

	public static readonly float WELLFED_CALORIES_PER_CYCLE = MooTuning.STANDARD_CALORIES_PER_CYCLE * 0.9f;

	public static readonly float ELIGIBLE_MILKING_PERCENTAGE = 1f;

	public static readonly float MILK_PER_CYCLE = 50f;

	public static readonly float DIESEL_PER_CYCLE = 200f;

	private static readonly float CYCLES_UNTIL_MILKING = 4f;

	public static readonly float MILK_CAPACITY = MooTuning.MILK_PER_CYCLE * MooTuning.CYCLES_UNTIL_MILKING;

	public static readonly float MILK_AMOUNT_AT_MILKING = MooTuning.MILK_PER_CYCLE * MooTuning.CYCLES_UNTIL_MILKING;

	public static readonly float MILK_PRODUCTION_PERCENTAGE_PER_SECOND = 100f / (600f * MooTuning.CYCLES_UNTIL_MILKING);
}
