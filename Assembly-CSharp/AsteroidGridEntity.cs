using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;

public class AsteroidGridEntity : ClusterGridEntity
{
	public override bool ShowName()
	{
		return true;
	}

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
			return EntityLayer.Asteroid;
		}
	}

	public override List<ClusterGridEntity.AnimConfig> AnimConfigs
	{
		get
		{
			List<ClusterGridEntity.AnimConfig> list = new List<ClusterGridEntity.AnimConfig>();
			ClusterGridEntity.AnimConfig animConfig = new ClusterGridEntity.AnimConfig
			{
				animFile = Assets.GetAnim(this.m_asteroidAnim.IsNullOrWhiteSpace() ? AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM : this.m_asteroidAnim),
				initialAnim = "idle_loop"
			};
			list.Add(animConfig);
			animConfig = new ClusterGridEntity.AnimConfig
			{
				animFile = Assets.GetAnim("orbit_kanim"),
				initialAnim = "orbit"
			};
			list.Add(animConfig);
			animConfig = new ClusterGridEntity.AnimConfig
			{
				animFile = Assets.GetAnim("shower_asteroid_current_kanim"),
				initialAnim = "off",
				playMode = KAnim.PlayMode.Once
			};
			list.Add(animConfig);
			return list;
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

	public void Init(string name, AxialI location, string asteroidTypeId)
	{
		this.m_name = name;
		this.m_location = location;
		this.m_asteroidAnim = asteroidTypeId;
	}

	protected override void OnSpawn()
	{
		KAnimFile kanimFile;
		if (!Assets.TryGetAnim(this.m_asteroidAnim, out kanimFile))
		{
			this.m_asteroidAnim = AsteroidGridEntity.DEFAULT_ASTEROID_ICON_ANIM;
		}
		Game.Instance.Subscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
		Game.Instance.Subscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
		Game.Instance.Subscribe(78366336, new Action<object>(this.OnMeteorShowerEventChanged));
		Game.Instance.Subscribe(1749562766, new Action<object>(this.OnMeteorShowerEventChanged));
		if (ClusterGrid.Instance.IsCellVisible(this.m_location))
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(this.m_location, 1);
		}
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.Unsubscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
		Game.Instance.Unsubscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
		Game.Instance.Unsubscribe(78366336, new Action<object>(this.OnMeteorShowerEventChanged));
		Game.Instance.Unsubscribe(1749562766, new Action<object>(this.OnMeteorShowerEventChanged));
		base.OnCleanUp();
	}

	public void OnClusterLocationChanged(object data)
	{
		if (this.m_worldContainer.IsDiscovered)
		{
			return;
		}
		if (!ClusterGrid.Instance.IsCellVisible(base.Location))
		{
			return;
		}
		Clustercraft component = ((ClusterLocationChangedEvent)data).entity.GetComponent<Clustercraft>();
		if (component == null)
		{
			return;
		}
		if (component.GetOrbitAsteroid() == this)
		{
			this.m_worldContainer.SetDiscovered(true);
		}
	}

	public override void OnClusterMapIconShown(ClusterRevealLevel levelUsed)
	{
		base.OnClusterMapIconShown(levelUsed);
		if (levelUsed == ClusterRevealLevel.Visible)
		{
			this.RefreshMeteorShowerEffect();
		}
	}

	private void OnMeteorShowerEventChanged(object _worldID)
	{
		if ((int)_worldID == this.m_worldContainer.id)
		{
			this.RefreshMeteorShowerEffect();
		}
	}

	public void RefreshMeteorShowerEffect()
	{
		if (ClusterMapScreen.Instance == null)
		{
			return;
		}
		ClusterMapVisualizer entityVisAnim = ClusterMapScreen.Instance.GetEntityVisAnim(this);
		if (entityVisAnim == null)
		{
			return;
		}
		KBatchedAnimController animController = entityVisAnim.GetAnimController(2);
		if (animController != null)
		{
			List<GameplayEventInstance> list = new List<GameplayEventInstance>();
			GameplayEventManager.Instance.GetActiveEventsOfType<MeteorShowerEvent>(this.m_worldContainer.id, ref list);
			bool flag = false;
			string text = "off";
			foreach (GameplayEventInstance gameplayEventInstance in list)
			{
				if (gameplayEventInstance != null && gameplayEventInstance.smi is MeteorShowerEvent.StatesInstance)
				{
					MeteorShowerEvent.StatesInstance statesInstance = gameplayEventInstance.smi as MeteorShowerEvent.StatesInstance;
					if (statesInstance.IsInsideState(statesInstance.sm.running.bombarding))
					{
						flag = true;
						text = "idle_loop";
						break;
					}
				}
			}
			animController.Play(text, flag ? KAnim.PlayMode.Loop : KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public void OnFogOfWarRevealed(object data = null)
	{
		if (data == null)
		{
			return;
		}
		if ((AxialI)data != this.m_location)
		{
			return;
		}
		if (!ClusterGrid.Instance.IsCellVisible(base.Location))
		{
			return;
		}
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			WorldDetectedMessage worldDetectedMessage = new WorldDetectedMessage(this.m_worldContainer);
			MusicManager.instance.PlaySong("Stinger_WorldDetected", false);
			Messenger.Instance.QueueMessage(worldDetectedMessage);
			if (!this.m_worldContainer.IsDiscovered)
			{
				using (IEnumerator enumerator = Components.Clustercrafts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (((Clustercraft)enumerator.Current).GetOrbitAsteroid() == this)
						{
							this.m_worldContainer.SetDiscovered(true);
							break;
						}
					}
				}
			}
		}
	}

	public static string DEFAULT_ASTEROID_ICON_ANIM = "asteroid_sandstone_start_kanim";

	[MyCmpReq]
	private WorldContainer m_worldContainer;

	[Serialize]
	private string m_name;

	[Serialize]
	private string m_asteroidAnim;
}
