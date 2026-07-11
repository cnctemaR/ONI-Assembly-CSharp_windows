using System;
using UnityEngine;

public static class FXHelpers
{
	public static KBatchedAnimController CreateEffect(string anim_file_name, Vector3 position, Transform parent = null, bool update_looping_sounds_position = false, Grid.SceneLayer layer = Grid.SceneLayer.Front, bool set_inactive = false)
	{
		KBatchedAnimController component = GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.EffectTemplateId), position, layer, null, 0).GetComponent<KBatchedAnimController>();
		component.GetComponent<KPrefabID>().PrefabTag = TagManager.Create(anim_file_name);
		component.name = anim_file_name;
		if (parent != null)
		{
			component.transform.SetParent(parent, false);
		}
		component.transform.SetPosition(position);
		if (update_looping_sounds_position)
		{
			component.FindOrAddComponent<LoopingSounds>().updatePosition = true;
		}
		KAnimFile anim = Assets.GetAnim(anim_file_name);
		if (anim == null)
		{
			global::Debug.LogWarning("Missing effect anim: " + anim_file_name);
		}
		else
		{
			component.AnimFiles = new KAnimFile[] { anim };
		}
		if (!set_inactive)
		{
			component.gameObject.SetActive(true);
		}
		return component;
	}
}
