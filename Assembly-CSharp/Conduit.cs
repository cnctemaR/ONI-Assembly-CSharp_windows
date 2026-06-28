using System;
using System.Collections;
using STRINGS;

[SkipSaveFileSerialization]
public class Conduit : KMonoBehaviour, IFirstFrameCallback
{
	public void SetFirstFrameCallback(global::System.Action ffCb)
	{
		this.firstFrameCallback = ffCb;
		base.StartCoroutine(this.RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return null;
		if (this.firstFrameCallback != null)
		{
			this.firstFrameCallback();
			this.firstFrameCallback = null;
		}
		yield return null;
		yield break;
	}

	protected override void OnPrefabInit()
	{
		this.Subscribe(-1201923725, new Action<object>(this.OnHighlighted));
		this.Subscribe(-700727624, new Action<object>(this.OnConduitFrozen));
		this.Subscribe(-1152799878, new Action<object>(this.OnConduitBoiling));
	}

	protected override void OnSpawn()
	{
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, this);
		if (this.IsInsulated)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.SetInsulated(Grid.PosToCell(this.transform.position), true);
		}
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.ForcePermanentDiseaseContainer();
	}

	protected override void OnCleanUp()
	{
		if (this.IsInsulated)
		{
			ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
			flowVisualizer.SetInsulated(Grid.PosToCell(this.transform.position), false);
		}
		int num = Grid.PosToCell(this.transform.position);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			IUtilityNetworkMgr networkManager = this.GetNetworkManager();
			networkManager.RemoveFromNetworks(num, this, false);
			networkManager.ConduitFlowManager.EmptyConduit(Grid.PosToCell(this.transform.position));
		}
		base.OnCleanUp();
	}

	private bool IsInsulated
	{
		get
		{
			return base.GetComponent<Building>().Def.Insulation < 1f;
		}
	}

	private ConduitFlowVisualizer GetFlowVisualizer()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidFlowVisualizer : Game.Instance.gasFlowVisualizer;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetFlowManager()
	{
		return (this.type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static ConduitFlow GetFlowManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	public static IUtilityNetworkMgr GetNetworkManager(ConduitType type)
	{
		return (type != ConduitType.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	private void OnHighlighted(object data)
	{
		bool flag = (bool)data;
		int num = ((!flag) ? (-1) : Grid.PosToCell(this.transform.position));
		ConduitFlowVisualizer flowVisualizer = this.GetFlowVisualizer();
		flowVisualizer.SetHighlightedCell(num);
	}

	private void OnConduitFrozen(object data)
	{
		this.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = int.MaxValue,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE
		});
		this.GetFlowManager().EmptyConduit(Grid.PosToCell(this.transform.position));
	}

	private void OnConduitBoiling(object data)
	{
		this.Trigger(-794517298, new BuildingHP.DamageSourceInfo
		{
			damage = int.MaxValue,
			source = BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
			popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED
		});
		this.GetFlowManager().EmptyConduit(Grid.PosToCell(this.transform.position));
	}

	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	public ConduitType type;

	private global::System.Action firstFrameCallback;
}
