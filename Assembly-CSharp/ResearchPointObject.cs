using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchPointObject : KMonoBehaviour, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		Research.Instance.AddResearchPoints(this.TypeID, 1f);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, Strings.Get("STRINGS.RESEARCH.TYPES." + this.TypeID.ToUpper() + ".NAME"), this.transform, 1.5f, false);
		Util.KDestroyGameObject(base.gameObject);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, Strings.Get("STRINGS.RESEARCH.TYPES." + this.TypeID.ToUpper() + ".NAME")), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.RESEARCHPOINT, Strings.Get("STRINGS.RESEARCH.TYPES." + this.TypeID.ToUpper() + ".NAME")), Descriptor.DescriptorType.Effect, false)
		};
	}

	public string TypeID = string.Empty;
}
