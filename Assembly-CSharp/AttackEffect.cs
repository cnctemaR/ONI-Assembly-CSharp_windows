using System;

[Serializable]
public class AttackEffect
{
	public AttackEffect(string ID, float probability)
	{
		this.effectID = ID;
		this.effectProbability = probability;
	}

	public string effectID;

	public float effectProbability;
}
