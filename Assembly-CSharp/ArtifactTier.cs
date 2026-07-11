using System;

public class ArtifactTier
{
	public ArtifactTier(StringKey str_key, EffectorValues values)
	{
		this.decorValues = values;
		this.name_key = str_key;
	}

	public EffectorValues decorValues;

	public StringKey name_key;
}
