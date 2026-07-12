using System;
using System.Collections.Generic;
using KSerialization;
using ProcGen;
using UnityEngine;

public abstract class ClusterGridEntity : KMonoBehaviour
{
	public abstract string Name { get; }

	public abstract EntityLayer Layer { get; }

	public abstract List<ClusterGridEntity.AnimConfig> AnimConfigs { get; }

	public abstract bool IsVisible { get; }

	public virtual bool ShowName()
	{
		return false;
	}

	public virtual bool ShowProgressBar()
	{
		return false;
	}

	public virtual float GetProgress()
	{
		return 0f;
	}

	public virtual bool SpaceOutInSameHex()
	{
		return false;
	}

	public virtual bool ShowPath()
	{
		return true;
	}

	public abstract ClusterRevealLevel IsVisibleInFOW { get; }

	public AxialI Location
	{
		get
		{
			return this.m_location;
		}
		set
		{
			if (value != this.m_location)
			{
				AxialI location = this.m_location;
				this.m_location = value;
				this.SendClusterLocationChangedEvent(location, this.m_location);
			}
		}
	}

	protected override void OnSpawn()
	{
		ClusterGrid.Instance.RegisterEntity(this);
		if (this.m_selectable != null)
		{
			this.m_selectable.SetName(this.Name);
		}
		if (!this.isWorldEntity)
		{
			this.m_transform.SetLocalPosition(new Vector3(-1f, 0f, 0f));
		}
	}

	protected override void OnCleanUp()
	{
		ClusterGrid.Instance.UnregisterEntity(this);
	}

	public virtual Sprite GetUISprite()
	{
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			List<ClusterGridEntity.AnimConfig> animConfigs = this.AnimConfigs;
			if (animConfigs.Count > 0)
			{
				return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile, "ui", false, "");
			}
		}
		else
		{
			WorldContainer component = base.GetComponent<WorldContainer>();
			if (component != null)
			{
				global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(component.worldName);
				if (worldData == null)
				{
					return null;
				}
				return Assets.GetSprite(worldData.asteroidIcon);
			}
		}
		return null;
	}

	public void SendClusterLocationChangedEvent(AxialI oldLocation, AxialI newLocation)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = new ClusterLocationChangedEvent
		{
			entity = this,
			oldLocation = oldLocation,
			newLocation = newLocation
		};
		base.Trigger(-1298331547, clusterLocationChangedEvent);
		Game.Instance.Trigger(-1298331547, clusterLocationChangedEvent);
		if (base.GetComponent<KSelectable>().IsSelected)
		{
			DetailsScreen.Instance.Refresh(base.gameObject);
		}
	}

	[Serialize]
	protected AxialI m_location;

	[MyCmpGet]
	private KSelectable m_selectable;

	[MyCmpReq]
	private Transform m_transform;

	public bool isWorldEntity;

	public struct AnimConfig
	{
		public KAnimFile animFile;

		public string initialAnim;
	}
}
