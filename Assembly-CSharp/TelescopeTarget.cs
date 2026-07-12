using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class TelescopeTarget : ClusterGridEntity
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
			return EntityLayer.Telescope;
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
					animFile = Assets.GetAnim("telescope_target_kanim"),
					initialAnim = "idle"
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

	public void Init(AxialI location)
	{
		base.Location = location;
	}

	public void SetTargetMeteorShower(ClusterMapMeteorShower.Instance meteorShower)
	{
		this.targetMeteorShower = meteorShower;
	}

	public override bool ShowName()
	{
		return true;
	}

	public override bool ShowProgressBar()
	{
		return true;
	}

	public override float GetProgress()
	{
		if (this.targetMeteorShower != null)
		{
			return this.targetMeteorShower.IdentifyingProgress;
		}
		return SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().GetRevealCompleteFraction(base.Location);
	}

	private ClusterMapMeteorShower.Instance targetMeteorShower;
}
