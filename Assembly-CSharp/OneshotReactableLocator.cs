using System;
using UnityEngine;

public class OneshotReactableLocator : IEntityConfig
{
	public static EmoteReactable CreateOneshotReactable(GameObject source, float lifetime, string id, ChoreType chore_type, HashedString animset, int range_width = 15, int range_height = 15)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(OneshotReactableLocator.ID), source.transform.position);
		EmoteReactable emoteReactable = new EmoteReactable(gameObject, id, chore_type, animset, range_width, range_height, 100000f, 0f, float.PositiveInfinity);
		emoteReactable.AddPrecondition(OneshotReactableLocator.ReactorIsNotSource(source));
		OneshotReactableHost component = gameObject.GetComponent<OneshotReactableHost>();
		component.lifetime = lifetime;
		component.SetReactable(emoteReactable);
		gameObject.SetActive(true);
		return emoteReactable;
	}

	private static Reactable.ReactablePrecondition ReactorIsNotSource(GameObject source)
	{
		return (GameObject reactor, Navigator.ActiveTransition transition) => reactor != source;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(OneshotReactableLocator.ID, OneshotReactableLocator.ID, false);
		gameObject.AddOrGet<KPrefabID>().PrefabTags = new Tag[] { GameTags.NotAPrefab };
		gameObject.AddOrGet<OneshotReactableHost>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "OneshotReactableLocator";
}
