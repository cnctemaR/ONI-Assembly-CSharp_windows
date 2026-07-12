using System;

public class ArtifactTier
{
	public ArtifactTier(StringKey str_key, EffectorValues values, float payload_drop_chance)
	{
		this.decorValues = values;
		this.name_key = str_key;
		this.payloadDropChance = payload_drop_chance;
	}

	public EffectorValues decorValues;

	public StringKey name_key;

	public float payloadDropChance;
}
