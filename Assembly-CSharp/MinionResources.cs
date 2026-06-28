using System;
using UnityEngine;

public class MinionResources : ScriptableObject
{
	public static MinionResources Get()
	{
		if (MinionResources._Instance == null)
		{
			MinionResources._Instance = Resources.Load<MinionResources>("MinionResources");
			MinionResources._Instance.Initialize();
		}
		return MinionResources._Instance;
	}

	private void Initialize()
	{
		this.races.Initialize();
		foreach (Race race in this.races)
		{
			race.Name = Strings.Get(race.StringKey);
		}
	}

	public BodyAnim GetBody(BodyType body_type)
	{
		foreach (BodyAnim bodyAnim in this.bodyAnims)
		{
			if (bodyAnim.bodyType == body_type)
			{
				return bodyAnim;
			}
		}
		return this.bodyAnims[0];
	}

	public MinionResources.Races races;

	public MinionResources.BodyAnims bodyAnims;

	private static MinionResources _Instance;

	[Serializable]
	public class Races : ResourceSet<Race>
	{
	}

	[Serializable]
	public class BodyAnims : ResourceSet<BodyAnim>
	{
	}

	[Serializable]
	public class Anim : Resource
	{
		public KAnimFile file;
	}
}
