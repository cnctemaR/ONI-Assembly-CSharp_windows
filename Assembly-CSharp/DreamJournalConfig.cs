using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DreamJournalConfig : IEntityConfig
{
	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public GameObject CreatePrefab()
	{
		KAnimFile anim = Assets.GetAnim("dream_journal_kanim");
		GameObject gameObject = EntityTemplates.CreateLooseEntity(DreamJournalConfig.ID.Name, ITEMS.DREAMJOURNAL.NAME, ITEMS.DREAMJOURNAL.DESC, 1f, true, anim, "object", Grid.SceneLayer.BuildingFront, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.StoryTraitResource,
			GameTags.PedestalDisplayable
		});
		gameObject.AddOrGet<EntitySplitter>().maxStackSize = 25f;
		return gameObject;
	}

	public static Tag ID = new Tag("DreamJournal");

	public const float MASS = 1f;

	public const int FABRICATION_TIME_SECONDS = 300;

	private const string ANIM_FILE = "dream_journal_kanim";

	private const string INITIAL_ANIM = "object";

	public const int MAX_STACK_SIZE = 25;
}
