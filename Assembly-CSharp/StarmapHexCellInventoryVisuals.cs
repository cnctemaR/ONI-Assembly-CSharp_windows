using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class StarmapHexCellInventoryVisuals : ClusterGridEntity
{
	public override string Name
	{
		get
		{
			return UI.CLUSTERMAP.HEXCELL_INVENTORY.NAME;
		}
	}

	public override EntityLayer Layer
	{
		get
		{
			return EntityLayer.Debri;
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
					animFile = Assets.GetAnim("harvestable_elements_kanim"),
					initialAnim = "idle_6",
					playMode = KAnim.PlayMode.Loop,
					additionalInfo = this
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
			return ClusterRevealLevel.Hidden;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.inventory = base.GetComponent<StarmapHexCellInventory>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!this.inventory.RegisterInventory(base.Location))
		{
			StarmapHexCellInventory.AllInventories[base.Location].TransferAllItemsFromExternalInventory(this.inventory);
			base.gameObject.DeleteObject();
			return;
		}
		base.Subscribe(-1697596308, new Action<object>(this.RefreshVisuals));
		base.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
		this.RefreshVisuals(null);
	}

	private void OnSelectObject(object data)
	{
		this.ToggleSelectionGlow(((Boxed<bool>)data).value);
	}

	public void RefreshVisuals(object o)
	{
		this.RefreshVisuals();
	}

	public void RefreshVisuals()
	{
		if (ClusterMapScreen.Instance == null || !ClusterMapScreen.Instance.isActiveAndEnabled)
		{
			return;
		}
		bool flag = this.inventory.ItemCount > 0;
		if (this.animController != null)
		{
			int num = Mathf.Min(6, this.inventory.ItemCount);
			string text = "idle_" + num.ToString();
			this.animController.Play(text, KAnim.PlayMode.Loop, 1f, 0f);
			for (int i = 0; i < this.symbolAnimControllers.Length; i++)
			{
				KBatchedAnimController kbatchedAnimController = this.symbolAnimControllers[i];
				KBatchedAnimTracker component = kbatchedAnimController.GetComponent<KBatchedAnimTracker>();
				if (i < num)
				{
					GameObject prefab = Assets.GetPrefab(this.inventory.Items[i].ID);
					Element element = ElementLoader.GetElement(prefab.PrefabID());
					KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
					string text2 = ((element != null && element.IsLiquid) ? "idle2" : (string.IsNullOrEmpty(component2.initialAnim) ? "object" : component2.initialAnim));
					string text3;
					KAnimFile animFileFromPrefabWithTag = Def.GetAnimFileFromPrefabWithTag(prefab, text2, out text3);
					kbatchedAnimController.SwapAnims(new KAnimFile[] { animFileFromPrefabWithTag });
					kbatchedAnimController.Play(text2, KAnim.PlayMode.Once, 1f, 0f);
					if (element != null)
					{
						Color color = element.substance.colour;
						color.a = 1f;
						if (!element.IsSolid)
						{
							kbatchedAnimController.SetSymbolTint(new KAnimHashedString("substance_tinter"), color);
						}
						if (element.IsGas)
						{
							kbatchedAnimController.SetSymbolTint(new KAnimHashedString("substance_tinter_cap"), color);
						}
					}
					kbatchedAnimController.gameObject.SetActive(true);
					component.forceAlwaysVisible = true;
				}
				else
				{
					component.forceAlwaysVisible = false;
					kbatchedAnimController.gameObject.SetActive(false);
				}
			}
		}
		if (flag != this.m_selectable.IsSelectable)
		{
			this.m_selectable.IsSelectable = flag;
		}
	}

	public override void onClustermapVisualizerAnimCreated(KBatchedAnimController controller, ClusterGridEntity.AnimConfig config)
	{
		if (config.additionalInfo == this)
		{
			this.animController = controller;
			this.SetupAnimControllerAndSymbols();
			this.RefreshVisuals(null);
		}
	}

	private void ToggleSelectionGlow(bool glow)
	{
		this.animController.SetSymbolVisiblity(StarmapHexCellInventoryVisuals.GLOW_SYMBOL, glow);
	}

	private void SetupAnimControllerAndSymbols()
	{
		this.DeleteSymbolControllers();
		if (this.animController != null)
		{
			this.animController.SetSymbolVisiblity(StarmapHexCellInventoryVisuals.GLOW_SYMBOL, false);
			this.symbolAnimControllers = new KBatchedAnimController[6];
			for (int i = 0; i < this.symbolAnimControllers.Length; i++)
			{
				string text = "swap0" + (i + 1).ToString();
				KBatchedAnimController kbatchedAnimController = this.CreateSymbolController(text);
				this.symbolAnimControllers[i] = kbatchedAnimController;
			}
		}
	}

	private KBatchedAnimController CreateSymbolController(string symbolName)
	{
		KBatchedAnimController kbatchedAnimController = this.CreateEmptyKAnimController(symbolName);
		bool flag;
		Matrix4x4 symbolTransform = this.animController.GetSymbolTransform(symbolName, out flag);
		bool flag2;
		Matrix2x3 symbolLocalTransform = this.animController.GetSymbolLocalTransform(symbolName, out flag2);
		Vector3 vector = symbolTransform.GetColumn(3);
		Vector3 vector2 = Vector3.one * symbolLocalTransform.m00;
		kbatchedAnimController.transform.SetParent(this.animController.transform, false);
		kbatchedAnimController.transform.SetPosition(vector);
		Vector3 localPosition = kbatchedAnimController.transform.localPosition;
		localPosition.z = -0.0025f;
		kbatchedAnimController.transform.localPosition = localPosition;
		kbatchedAnimController.transform.localScale = vector2;
		KBatchedAnimTracker kbatchedAnimTracker = kbatchedAnimController.gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.controller = this.animController;
		kbatchedAnimTracker.symbol = new HashedString(symbolName);
		kbatchedAnimTracker.forceAlwaysVisible = false;
		kbatchedAnimController.gameObject.SetActive(false);
		this.animController.SetSymbolVisiblity(symbolName, false);
		return kbatchedAnimController;
	}

	private KBatchedAnimController CreateEmptyKAnimController(string name)
	{
		GameObject gameObject = new GameObject(base.gameObject.name + "-" + name);
		gameObject.SetActive(false);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("harvestable_elements_kanim") };
		kbatchedAnimController.materialType = KAnimBatchGroup.MaterialType.UI;
		kbatchedAnimController.animScale = ((this.animController == null) ? 0.08f : this.animController.animScale);
		kbatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
		kbatchedAnimController.sceneLayer = Grid.SceneLayer.NoLayer;
		return kbatchedAnimController;
	}

	private void DeleteSymbolControllers()
	{
		if (this.symbolAnimControllers != null)
		{
			for (int i = 0; i < this.symbolAnimControllers.Length; i++)
			{
				KBatchedAnimController kbatchedAnimController = this.symbolAnimControllers[i];
				if (kbatchedAnimController != null)
				{
					kbatchedAnimController.gameObject.DeleteObject();
				}
			}
			this.symbolAnimControllers = null;
		}
	}

	public const int MAX_VISUAL_ITEMS = 6;

	public const string ANIM_FILE_NAME = "harvestable_elements_kanim";

	public const string DEFAULT_ANIM_STATE_NAME = "idle_6";

	public const string ANIM_STATE_NAME_PREFIX = "idle_";

	public const string SYMBOL_SWAP_NAME_PREFIX = "swap0";

	private static readonly HashedString GLOW_SYMBOL = "glow";

	public StarmapHexCellInventory inventory;

	private KBatchedAnimController animController;

	private KBatchedAnimController[] symbolAnimControllers;
}
