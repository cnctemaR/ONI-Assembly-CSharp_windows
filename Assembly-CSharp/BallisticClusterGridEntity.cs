using System;
using System.Collections.Generic;
using UnityEngine;

public class BallisticClusterGridEntity : ClusterGridEntity
{
	public override string Name
	{
		get
		{
			return Strings.Get(this.nameKey);
		}
	}

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.Payload;
		}
	}

	public override bool KeepRotationWhenSpacingOutInHex()
	{
		return this.keepRotationWhenSpacingOutInHex;
	}

	public override List<ClusterGridEntity.AnimConfig> AnimConfigs
	{
		get
		{
			return new List<ClusterGridEntity.AnimConfig>
			{
				new ClusterGridEntity.AnimConfig
				{
					animFile = Assets.GetAnim(this.clusterAnimName),
					initialAnim = "idle_loop",
					symbolSwapTarget = this.clusterAnimSymbolSwapTarget,
					symbolSwapSymbol = this.clusterAnimSymbolSwapSymbol
				}
			};
		}
	}

	public override bool IsVisible
	{
		get
		{
			return !base.gameObject.HasTag(GameTags.ClusterEntityGrounded);
		}
	}

	public override ClusterRevealLevel IsVisibleInFOW
	{
		get
		{
			return ClusterRevealLevel.Visible;
		}
	}

	public override bool SpaceOutInSameHex()
	{
		return true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.m_clusterTraveler.getSpeedCB = new Func<float>(this.GetSpeed);
		this.m_clusterTraveler.getCanTravelCB = new Func<bool, bool>(this.CanTravel);
		this.m_clusterTraveler.onTravelCB = null;
	}

	private float GetSpeed()
	{
		return 10f;
	}

	private bool CanTravel(bool tryingToLand)
	{
		return this.HasTag(GameTags.EntityInSpace);
	}

	public void Configure(AxialI source, AxialI destination)
	{
		this.m_location = source;
		this.m_destionationSelector.SetDestination(destination);
	}

	public override bool ShowPath()
	{
		return this.m_selectable.IsSelected;
	}

	public override bool ShowProgressBar()
	{
		return this.m_selectable.IsSelected && this.m_clusterTraveler.IsTraveling();
	}

	public override float GetProgress()
	{
		return this.m_clusterTraveler.GetMoveProgress();
	}

	public void SwapSymbolFromSameAnim(string targetSymbolName, string swappedSymbolName)
	{
		this.clusterAnimSymbolSwapTarget = targetSymbolName;
		this.clusterAnimSymbolSwapSymbol = swappedSymbolName;
	}

	[MyCmpReq]
	private ClusterDestinationSelector m_destionationSelector;

	[MyCmpReq]
	private ClusterTraveler m_clusterTraveler;

	[SerializeField]
	public string clusterAnimName;

	[SerializeField]
	public StringKey nameKey;

	private string clusterAnimSymbolSwapTarget;

	private string clusterAnimSymbolSwapSymbol;

	public bool keepRotationWhenSpacingOutInHex;
}
