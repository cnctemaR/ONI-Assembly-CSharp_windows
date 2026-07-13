using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HarvestablePOIClusterGridEntity : ClusterGridEntity
{
	public override string Name
	{
		get
		{
			return this.m_name;
		}
	}

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.POI;
		}
	}

	public override List<ClusterGridEntity.AnimConfig> AnimConfigs
	{
		get
		{
			return new List<ClusterGridEntity.AnimConfig>
			{
				new ClusterGridEntity.AnimConfig
				{
					animFile = Assets.GetAnim("harvestable_space_poi_kanim"),
					initialAnim = (this.m_Anim.IsNullOrWhiteSpace() ? "cloud" : this.m_Anim)
				}
			};
		}
	}

	public override bool IsVisible
	{
		get
		{
			return true;
		}
	}

	public override ClusterRevealLevel IsVisibleInFOW
	{
		get
		{
			return ClusterRevealLevel.Peeked;
		}
	}

	public void Init(AxialI location)
	{
		base.Location = location;
	}

	public override Sprite GetUISprite()
	{
		Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(this.AnimConfigs[0].animFile, this.AnimConfigs[0].initialAnim, false, "");
		return (uispriteFromMultiObjectAnim == null) ? base.GetUISprite() : uispriteFromMultiObjectAnim;
	}

	public override void onClustermapVisualizerAnimCreated(KBatchedAnimController controller, ClusterGridEntity.AnimConfig config)
	{
	}

	public string m_name;

	public string m_Anim;
}
