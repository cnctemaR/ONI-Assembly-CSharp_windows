using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ClusterFXEntity : ClusterGridEntity
{
	public override string Name
	{
		get
		{
			return UI.SPACEDESTINATIONS.TELESCOPE_TARGET.NAME;
		}
	}

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.FX;
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
					animFile = Assets.GetAnim(this.kAnimName),
					initialAnim = this.animName,
					playMode = this.animPlayMode,
					animOffset = this.animOffset
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
			return ClusterRevealLevel.Visible;
		}
	}

	public void Init(AxialI location, Vector3 animOffset)
	{
		base.Location = location;
		this.animOffset = animOffset;
	}

	[SerializeField]
	public string kAnimName;

	[SerializeField]
	public string animName;

	public KAnim.PlayMode animPlayMode = KAnim.PlayMode.Once;

	public Vector3 animOffset;
}
