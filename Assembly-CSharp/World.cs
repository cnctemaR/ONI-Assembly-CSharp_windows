using System;
using System.Collections.Generic;
using Klei;
using Rendering;
using Rendering.World;
using UnityEngine;

public class World : KMonoBehaviour
{
	public static World Instance { get; private set; }

	public SubworldZoneRenderData zoneRenderData { get; private set; }

	protected override void OnPrefabInit()
	{
		World.Instance = this;
		this.blockTileRenderer = base.GetComponent<BlockTileRenderer>();
		this.regionTileRenderer = base.GetComponent<RegionTileRenderer>();
	}

	protected override void OnSpawn()
	{
		base.GetComponent<SimDebugView>().OnReset();
		base.GetComponent<PropertyTextures>().OnReset(null);
		this.zoneRenderData = base.GetComponent<SubworldZoneRenderData>();
		Grid.OnReveal = (Action<int>)Delegate.Combine(Grid.OnReveal, new Action<int>(this.OnReveal));
	}

	protected override void OnLoadLevel()
	{
		World.Instance = null;
		if (this.blockTileRenderer != null)
		{
			this.blockTileRenderer.FreeResources();
		}
		this.blockTileRenderer = null;
		if (this.regionTileRenderer != null)
		{
			this.regionTileRenderer.FreeResources();
		}
		this.regionTileRenderer = null;
		if (SpaceBorderTileRenderer.Instance != null)
		{
			SpaceBorderTileRenderer.Instance.FreeResources();
		}
		if (this.groundRenderer != null)
		{
			this.groundRenderer.FreeResources();
		}
		this.groundRenderer = null;
		this.revealedCells.Clear();
		this.revealedCells = null;
		base.OnLoadLevel();
	}

	public unsafe void UpdateCellInfo(List<SolidInfo> solidInfo, List<CallbackInfo> callbackInfo, int num_solid_substance_change_info, Sim.SolidSubstanceChangeInfo* solid_substance_change_info, int num_liquid_change_info, Sim.LiquidChangeInfo* liquid_change_info)
	{
		int count = solidInfo.Count;
		this.changedCells.Clear();
		for (int i = 0; i < count; i++)
		{
			SolidInfo solidInfo2 = solidInfo[i];
			bool isSolid = solidInfo2.isSolid;
			int cellIdx = solidInfo2.cellIdx;
			if (!this.changedCells.Contains(cellIdx))
			{
				this.changedCells.Add(cellIdx);
			}
			Pathfinding.Instance.AddDirtyNavGridCell(cellIdx);
			WorldDamage.Instance.OnSolidStateChanged(cellIdx);
			if (this.OnSolidChanged != null)
			{
				this.OnSolidChanged(cellIdx);
			}
		}
		GameScenePartitioner.Instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.solidChangedLayer, null);
		int count2 = callbackInfo.Count;
		for (int j = 0; j < count2; j++)
		{
			callbackInfo[j].Release();
		}
		for (int k = 0; k < num_solid_substance_change_info; k++)
		{
			int cellIdx2 = solid_substance_change_info[k].cellIdx;
			if (!Grid.IsValidCell(cellIdx2))
			{
				global::Debug.LogError(cellIdx2, null);
			}
			else
			{
				Grid.RenderedByWorld[cellIdx2] = Grid.Element[cellIdx2].substance.renderedByWorld && Grid.Objects[cellIdx2, 9] == null;
				this.groundRenderer.MarkDirty(cellIdx2);
			}
		}
		GameScenePartitioner instance = GameScenePartitioner.Instance;
		this.changedCells.Clear();
		for (int l = 0; l < num_liquid_change_info; l++)
		{
			int cellIdx3 = liquid_change_info[l].cellIdx;
			global::UnityEngine.Debug.Assert(Grid.IsValidCell(cellIdx3));
			this.changedCells.Add(cellIdx3);
			if (this.OnLiquidChanged != null)
			{
				this.OnLiquidChanged(cellIdx3);
			}
		}
		instance.TriggerEvent(this.changedCells, GameScenePartitioner.Instance.liquidChangedLayer, null);
	}

	private void OnReveal(int cell)
	{
		this.revealedCells.Add(cell);
	}

	private void LateUpdate()
	{
		if (Game.IsQuitting())
		{
			return;
		}
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		this.groundRenderer.Render(visibleArea.Min, visibleArea.Max);
		Vector2I vector2I;
		Vector2I vector2I2;
		KBatchedAnimUpdater.instance.GetVisibleArea(out vector2I, out vector2I2);
		KAnimBatchManager.Instance().UpdateActiveArea(vector2I, vector2I2);
		KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
		KAnimBatchManager.Instance().Render();
		if (Camera.main != null)
		{
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
			Shader.SetGlobalVector("_CursorPos", new Vector4(vector.x, vector.y, vector.z, 0f));
		}
		FallingWater.instance.UpdateParticles(Time.deltaTime);
		FallingWater.instance.Render();
		SpriteSheetAnimManager.instance.UpdateAnims(Time.deltaTime);
		SpriteSheetAnimManager.instance.Render();
		if (this.revealedCells.Count > 0)
		{
			GameScenePartitioner.Instance.TriggerEvent(this.revealedCells, GameScenePartitioner.Instance.fogOfWarChangedLayer, null);
			this.revealedCells.Clear();
		}
	}

	public Action<int> OnSolidChanged;

	public Action<int> OnLiquidChanged;

	public BlockTileRenderer blockTileRenderer;

	public RegionTileRenderer regionTileRenderer;

	[MyCmpGet]
	[NonSerialized]
	public GroundRenderer groundRenderer;

	private List<int> revealedCells = new List<int>();

	public static int DebugCellID = -1;

	private List<int> changedCells = new List<int>();
}
