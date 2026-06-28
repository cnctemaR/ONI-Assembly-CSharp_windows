using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using UnityEngine;

public class BuildingComplete : Building
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onCleanUp;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(this.Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		Attributes attributes = this.GetAttributes();
		foreach (Klei.AI.Attribute attribute in this.Def.attributes)
		{
			attributes.Add(attribute);
		}
		foreach (AttributeModifier attributeModifier in this.Def.attributeModifiers)
		{
			Klei.AI.Attribute attribute2 = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
			if (attributes.Get(attribute2) == null)
			{
				attributes.Add(attribute2);
			}
			attributes.Add("Base", attributeModifier);
		}
		foreach (AttributeInstance attributeInstance in attributes)
		{
			AttributeModifier attributeModifier2 = new AttributeModifier(attributeInstance.Id, attributeInstance.GetTotalValue(), null, false, false, true);
			this.regionModifiers.Add(attributeModifier2);
		}
		if (this.Def.UseStructureTemperature)
		{
			GameComps.StructureTemperatures.Add(base.gameObject);
		}
		base.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
	}

	private void OnSelectObject(object data)
	{
		if (this.Def.SelectMode != SimViewMode.None)
		{
			bool flag = (bool)data;
			GameHashes gameHashes = ((!flag) ? GameHashes.DisableOverlay : GameHashes.EnableOverlay);
			EventSystem.Trigger(Game.Instance.gameObject, (int)gameHashes, this.Def.SelectMode);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		Rotatable component2 = base.GetComponent<Rotatable>();
		if (component != null && component2 == null)
		{
			component.Offset = this.Def.GetVisualizerOffset() + this.Def.placementPivot;
		}
		BoxCollider2D component3 = base.GetComponent<BoxCollider2D>();
		if (component3 != null)
		{
			Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		int num = Grid.PosToCell(base.transform.position);
		if (this.Def.IsFoundation)
		{
			foreach (int num2 in base.PlacementCells)
			{
				Grid.Foundation[num2] = true;
				Game.Instance.roomProber.SolidChangedEvent(num2, false);
			}
		}
		else
		{
			Game.Instance.roomProber.AddBuilding(this);
		}
		Vector3 vector = Grid.CellToPosCBC(num, this.Def.SceneLayer);
		base.transform.SetPosition(vector);
		PrimaryElement component4 = base.GetComponent<PrimaryElement>();
		if (component4 != null)
		{
			if (component4.Mass == 0f)
			{
				component4.Mass = this.Def.Mass[0];
			}
		}
		this.Def.MarkArea(num, base.Orientation, this.Def.ObjectLayer, base.gameObject);
		if (this.Def.IsTilePiece)
		{
			this.Def.MarkArea(num, base.Orientation, this.Def.TileLayer, base.gameObject);
			this.Def.RunOnArea(num, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, this.Def.TileLayer);
			});
		}
		SaveLoadRoot component5 = base.GetComponent<SaveLoadRoot>();
		if (component5 != null)
		{
			component5.folder = Folder.Entities;
		}
		if (!this.Def.IsTilePiece)
		{
		}
		Texture buildingTexture = component4.Element.substance.buildingTexture;
		if (buildingTexture != null)
		{
			RenderUtil.SetMaterialBlockTexture(base.transform, "_FillTex", buildingTexture);
		}
		base.RegisterBlockTileRenderer();
		for (int j = 0; j < base.PlacementCells.Length; j++)
		{
			Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(base.PlacementCells[j]);
			if (intersectionRegion != null)
			{
				intersectionRegion.AddBuilding(this, true);
				break;
			}
		}
		if (this.Def.PreventIdlingInFrontOfBuilding)
		{
			for (int k = 0; k < base.PlacementCells.Length; k++)
			{
				Grid.PreventIdlingOnCell[base.PlacementCells[k]] = true;
			}
		}
		KSelectable component6 = base.GetComponent<KSelectable>();
		if (component6 != null)
		{
			component6.SetStatusIndicatorOffset(this.Def.placementPivot);
		}
		Components.BuildingCompletes.Add(this);
	}

	private string GetInspectSound()
	{
		string text = "AI_Inspect_" + base.GetComponent<KPrefabID>().PrefabTag.Name;
		return GlobalAssets.GetSound(text, false);
	}

	protected override void OnCleanUp()
	{
		if (!Game.quitting)
		{
			if (this.Def.UseStructureTemperature)
			{
				GameComps.StructureTemperatures.Remove(base.gameObject);
			}
			base.OnCleanUp();
			int num = Grid.PosToCell(this);
			this.Def.UnmarkArea(num, base.Orientation, this.Def.ObjectLayer, base.gameObject);
			if (this.Def.IsFoundation)
			{
				foreach (CellOffset cellOffset in this.Def.PlacementOffsets)
				{
					int num2 = Grid.OffsetCell(num, cellOffset);
					Grid.Foundation[num2] = false;
					Game.Instance.roomProber.SolidChangedEvent(num2, false);
				}
			}
			Game.Instance.roomProber.RemoveBuilding(this);
			for (int j = 0; j < base.PlacementCells.Length; j++)
			{
				Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(base.PlacementCells[j]);
				if (intersectionRegion != null)
				{
					intersectionRegion.RemoveBuilding(this, true);
					break;
				}
			}
			if (this.Def.PreventIdlingInFrontOfBuilding)
			{
				for (int k = 0; k < base.PlacementCells.Length; k++)
				{
					Grid.PreventIdlingOnCell[base.PlacementCells[k]] = false;
				}
			}
			Components.BuildingCompletes.Remove(this);
			base.UnregisterBlockTileRenderer();
			if (this.onCleanUp != null)
			{
				this.onCleanUp();
			}
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpGet]
	public Assignable assignable;

	[MyCmpGet]
	public KPrefabID prefabid;

	public bool isManuallyOperated;

	public bool isArtable;

	public List<AttributeModifier> regionModifiers = new List<AttributeModifier>();
}
