using System;

public class CreatureBrain : Brain
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.GetComponent<Navigator>().SetAbilities(new CreaturePathFinderAbilities(base.GetComponent<Navigator>()));
	}

	public string symbolPrefix;

	public Tag species;
}
