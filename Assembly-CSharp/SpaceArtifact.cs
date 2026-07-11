using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SpaceArtifact")]
public class SpaceArtifact : KMonoBehaviour, IEffectDescriptor, IGameObjectEffectDescriptor
{
	public void SetArtifactTier(ArtifactTier tier)
	{
		this.artifactTier = tier;
	}

	public ArtifactTier GetArtifactTier()
	{
		return this.artifactTier;
	}

	public void SetUIAnim(string anim)
	{
		this.ui_anim = anim;
	}

	public string GetUIAnim()
	{
		return this.ui_anim;
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = new Descriptor(string.Format("This is an artifact from space", Array.Empty<object>()), string.Format("This is the tooltip string", Array.Empty<object>()), Descriptor.DescriptorType.Information, false);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetEffectDescriptions();
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.GetEffectDescriptions();
	}

	public const string ID = "SpaceArtifact";

	[SerializeField]
	private string ui_anim;

	[SerializeField]
	private ArtifactTier artifactTier;
}
