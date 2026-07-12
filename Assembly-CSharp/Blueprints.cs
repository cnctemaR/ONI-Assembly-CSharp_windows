using System;

public class Blueprints
{
	public static Blueprints Get()
	{
		if (Blueprints.instance == null)
		{
			Blueprints.instance = new Blueprints();
			Blueprints.instance.all.AddBlueprintsFrom<Blueprints_Default>(new Blueprints_Default());
			foreach (BlueprintProvider blueprintProvider in Blueprints.instance.skinsReleaseProviders)
			{
				Blueprints.instance.skinsRelease.AddBlueprintsFrom<BlueprintProvider>(blueprintProvider);
			}
			Blueprints.instance.all.AddBlueprintsFrom(Blueprints.instance.skinsRelease);
			Blueprints.instance.skinsRelease.PostProcess();
			Blueprints.instance.all.PostProcess();
		}
		return Blueprints.instance;
	}

	public BlueprintCollection all = new BlueprintCollection();

	public BlueprintCollection skinsRelease = new BlueprintCollection();

	public BlueprintProvider[] skinsReleaseProviders = new BlueprintProvider[]
	{
		new Blueprints_U51AndBefore(),
		new Blueprints_DlcPack2(),
		new Blueprints_U53()
	};

	private static Blueprints instance;
}
