using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpaceArtifact")]
public class SpaceArtifact : KMonoBehaviour, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.loadCharmed && DlcManager.IsExpansion1Active())
		{
			base.gameObject.AddTag(GameTags.CharmedArtifact);
			this.SetEntombedDecor();
		}
		else
		{
			this.loadCharmed = false;
			this.SetAnalyzedDecor();
		}
		this.UpdateStatusItem();
		Components.SpaceArtifacts.Add(this);
		this.UpdateAnim();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.SpaceArtifacts.Remove(this);
	}

	public void RemoveCharm()
	{
		base.gameObject.RemoveTag(GameTags.CharmedArtifact);
		this.UpdateStatusItem();
		this.loadCharmed = false;
		this.UpdateAnim();
		this.SetAnalyzedDecor();
	}

	private void SetEntombedDecor()
	{
		base.GetComponent<DecorProvider>().SetValues(DECOR.BONUS.TIER0);
	}

	private void SetAnalyzedDecor()
	{
		base.GetComponent<DecorProvider>().SetValues(this.artifactTier.decorValues);
	}

	public void UpdateStatusItem()
	{
		if (base.gameObject.HasTag(GameTags.CharmedArtifact))
		{
			base.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed, null);
			return;
		}
		base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed, false);
	}

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
		if (base.gameObject.HasTag(GameTags.CharmedArtifact))
		{
			Descriptor descriptor = new Descriptor(global::STRINGS.BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE.Replace("{chance}", GameUtil.GetFormattedPercent(this.artifactTier.payloadDropChance * 100f, GameUtil.TimeSlice.None)), global::STRINGS.BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE_TOOLTIP.Replace("{chance}", GameUtil.GetFormattedPercent(this.artifactTier.payloadDropChance * 100f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false);
			list.Add(descriptor);
		}
		Descriptor descriptor2 = new Descriptor(string.Format("This is an artifact from space", Array.Empty<object>()), string.Format("This is the tooltip string", Array.Empty<object>()), Descriptor.DescriptorType.Information, false);
		list.Add(descriptor2);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return this.GetEffectDescriptions();
	}

	private void UpdateAnim()
	{
		string text;
		if (base.gameObject.HasTag(GameTags.CharmedArtifact))
		{
			text = "entombed_" + this.uniqueAnimNameFragment.Replace("idle_", "");
		}
		else
		{
			text = this.uniqueAnimNameFragment;
		}
		base.GetComponent<KBatchedAnimController>().Play(text, KAnim.PlayMode.Loop, 1f, 0f);
	}

	[OnDeserialized]
	public void OnDeserialize()
	{
		Pickupable component = base.GetComponent<Pickupable>();
		if (component != null)
		{
			component.deleteOffGrid = false;
		}
	}

	public const string ID = "SpaceArtifact";

	private const string charmedPrefix = "entombed_";

	private const string idlePrefix = "idle_";

	[SerializeField]
	private string ui_anim;

	[Serialize]
	private bool loadCharmed = true;

	public ArtifactTier artifactTier;

	public ArtifactType artifactType;

	public string uniqueAnimNameFragment;
}
