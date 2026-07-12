using System;

public class FOODDEHYDRATORTUNING
{
	public const float INTERNAL_WORK_TIME = 250f;

	public const float DUPE_WORK_TIME = 50f;

	public const float GAS_CONSUMPTION_PER_SECOND = 0.020000001f;

	public const float REQUIRED_FUEL_AMOUNT = 5.0000005f;

	public const float CO2_EMIT_RATE = 0.0050000004f;

	public const float CO2_EMIT_TEMPERATURE = 348.15f;

	public const float PLASTIC_KG = 12f;

	public const float WATER_OUTPUT_KG = 6f;

	public const float FOOD_PACKETS = 6f;

	public const float KCAL_PER_PACKET = 1000f;

	public const float FOOD_KCAL = 6000000f;

	public static Tag FUEL_TAG = SimHashes.Methane.CreateTag();
}
