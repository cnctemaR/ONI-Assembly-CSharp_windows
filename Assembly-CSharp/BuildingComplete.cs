using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class BuildingComplete : Building
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.GetPosition();
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
			attributes.Add(attributeModifier);
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
		base.Subscribe<BuildingComplete>(-1503271301, BuildingComplete.OnSelectObjectDelegate);
		base.Subscribe<BuildingComplete>(1606648047, BuildingComplete.OnObjectReplacedDelegate);
	}

	private void OnSelectObject(object data)
	{
		if (this.Def.SelectMode != SimViewMode.None)
		{
			bool flag = (bool)data;
			GameHashes gameHashes = ((!flag) ? GameHashes.DisableOverlay : GameHashes.EnableOverlay);
			Game.Instance.gameObject.Trigger((int)gameHashes, this.Def.SelectMode);
		}
	}

	private void OnObjectReplaced(object data)
	{
		this.wasReplaced = true;
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
		KBoxCollider2D component3 = base.GetComponent<KBoxCollider2D>();
		if (component3 != null)
		{
			Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (this.Def.IsFoundation)
		{
			foreach (int num2 in base.PlacementCells)
			{
				Grid.Foundation[num2] = true;
				Game.Instance.roomProber.SolidChangedEvent(num2, false);
			}
		}
		Vector3 vector = Grid.CellToPosCBC(num, this.Def.SceneLayer);
		base.transform.SetPosition(vector);
		PrimaryElement component4 = base.GetComponent<PrimaryElement>();
		if (component4 != null && component4.Mass == 0f)
		{
			component4.Mass = this.Def.Mass[0];
		}
		this.Def.MarkArea(num, base.Orientation, this.Def.ObjectLayer, base.gameObject);
		if (this.Def.IsTilePiece)
		{
			this.Def.MarkArea(num, base.Orientation, this.Def.TileLayer, base.gameObject);
			this.Def.RunOnArea(num, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer);
			});
		}
		base.RegisterBlockTileRenderer();
		if (this.Def.PreventIdleTraversalPastBuilding)
		{
			for (int j = 0; j < base.PlacementCells.Length; j++)
			{
				Grid.PreventIdleTraversal[base.PlacementCells[j]] = true;
			}
		}
		KSelectable component5 = base.GetComponent<KSelectable>();
		if (component5 != null)
		{
			component5.SetStatusIndicatorOffset(this.Def.placementPivot);
		}
		Components.BuildingCompletes.Add(this);
		BuildingConfigManager.Instance.AddBuildingCompleteKComponents(base.gameObject, this.Def.Tag);
		this.hasSpawnedKComponents = true;
	}

	private string GetInspectSound()
	{
		string text = "AI_Inspect_" + base.GetComponent<KPrefabID>().PrefabTag.Name;
		return GlobalAssets.GetSound(text, false);
	}

	protected override void OnCleanUp()
	{
		if (Game.quitting)
		{
			return;
		}
		if (this.hasSpawnedKComponents)
		{
			BuildingConfigManager.Instance.DestroyBuildingCompleteKComponents(base.gameObject, this.Def.Tag);
		}
		if (this.Def.UseStructureTemperature)
		{
			GameComps.StructureTemperatures.Remove(base.gameObject);
		}
		base.OnCleanUp();
		if (!this.wasReplaced)
		{
			int num = Grid.PosToCell(this);
			this.Def.UnmarkArea(num, base.Orientation, this.Def.ObjectLayer, base.gameObject);
			if (this.Def.IsTilePiece)
			{
				this.Def.UnmarkArea(num, base.Orientation, this.Def.TileLayer, base.gameObject);
				this.Def.RunOnArea(num, base.Orientation, delegate(int c)
				{
					TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer);
				});
			}
			if (this.Def.IsFoundation)
			{
				foreach (int num2 in base.PlacementCells)
				{
					Grid.Foundation[num2] = false;
					Game.Instance.roomProber.SolidChangedEvent(num2, false);
				}
			}
			if (this.Def.PreventIdleTraversalPastBuilding)
			{
				for (int j = 0; j < base.PlacementCells.Length; j++)
				{
					Grid.PreventIdleTraversal[base.PlacementCells[j]] = false;
				}
			}
		}
		Components.BuildingCompletes.Remove(this);
		base.UnregisterBlockTileRenderer();
		base.Trigger(-21016276, this);
	}

	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpGet]
	public Assignable assignable;

	[MyCmpGet]
	public KPrefabID prefabid;

	public bool isManuallyOperated;

	public bool isArtable;

	private bool hasSpawnedKComponents;

	private bool wasReplaced;

	public List<AttributeModifier> regionModifiers = new List<AttributeModifier>();

	private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnSelectObjectDelegate = new EventSystem.IntraObjectHandler<BuildingComplete>(delegate(BuildingComplete component, object data)
	{
		component.OnSelectObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<BuildingComplete>(delegate(BuildingComplete component, object data)
	{
		component.OnObjectReplaced(data);
	});
}
