using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimLayering
{
	public KAnimLayering(KAnimControllerBase controller, Grid.SceneLayer layer)
	{
		this.controller = controller;
		this.layer = layer;
	}

	public void SetLayer(Grid.SceneLayer layer)
	{
		this.layer = layer;
		if (this.layerControllers != null)
		{
			foreach (Component component in this.layerControllers.Values)
			{
				Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(layer) - this.controller.gameObject.transform.GetPosition().z - 0.1f);
				component.transform.SetLocalPosition(vector);
			}
		}
	}

	public void SetIsLayer(bool is_layer)
	{
		this.isLayer = is_layer;
	}

	public bool GetIsLayer()
	{
		return this.isLayer;
	}

	public void SetSyncLayeringTint(bool sync)
	{
		if (this.links == null)
		{
			return;
		}
		foreach (KeyValuePair<KAnim.SymbolFlags, KAnimLink> keyValuePair in this.links)
		{
			keyValuePair.Value.syncTint = sync;
		}
	}

	private static bool IsAnimLayered(KAnimFile[] anims, KAnim.SymbolFlags layer_flag)
	{
		for (int i = 0; i < anims.Length; i++)
		{
			if (KAnimLayering.IsAnimFileLayered(anims[i], layer_flag))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsAnimFileLayered(KAnimFile anim_file, KAnim.SymbolFlags layer_flag)
	{
		if (anim_file == null)
		{
			return false;
		}
		KAnimFileData data = anim_file.GetData();
		if (data.build == null)
		{
			return false;
		}
		KAnim.Build.Symbol[] symbols = data.build.symbols;
		for (int i = 0; i < symbols.Length; i++)
		{
			if ((symbols[i].flags & (int)layer_flag) != 0)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsOverrideAnimLayered(IReadOnlyList<KAnimControllerBase.OverrideAnimFileData> override_anims, KAnim.SymbolFlags layer_flag)
	{
		using (IEnumerator<KAnimControllerBase.OverrideAnimFileData> enumerator = override_anims.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (KAnimLayering.IsAnimFileLayered(enumerator.Current.file, layer_flag))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void HideSymbolsInternal(KAnim.SymbolFlags symbol_flag_to_hide)
	{
		foreach (KAnimFile kanimFile in this.controller.AnimFiles)
		{
			this.SetAnimVisibility(kanimFile, symbol_flag_to_hide);
		}
		IReadOnlyList<KAnimControllerBase.OverrideAnimFileData> overrideAnimFiles = this.controller.OverrideAnimFiles;
		for (int j = 0; j < overrideAnimFiles.Count; j++)
		{
			KAnimFile file = overrideAnimFiles[j].file;
			this.SetAnimVisibility(file, symbol_flag_to_hide);
		}
	}

	private void SetAnimVisibility(KAnimFile anim_file, KAnim.SymbolFlags symbol_flag)
	{
		if (anim_file == null)
		{
			return;
		}
		KAnimFileData data = anim_file.GetData();
		if (data.build == null)
		{
			return;
		}
		KAnim.Build.Symbol[] symbols = data.build.symbols;
		for (int i = 0; i < symbols.Length; i++)
		{
			if ((symbols[i].flags & (int)symbol_flag) != 0 != this.isLayer && !(symbols[i].hash == KAnimLayering.UI))
			{
				this.controller.SetSymbolVisiblity(symbols[i].hash, false);
			}
		}
	}

	public void HideSymbols()
	{
		if (EntityPrefabs.Instance == null)
		{
			return;
		}
		if (this.isLayer)
		{
			return;
		}
		foreach (KAnim.SymbolFlags symbolFlags in KAnimLayering.layerSettings.Keys)
		{
			bool flag = KAnimLayering.IsAnimLayered(this.controller.AnimFiles, symbolFlags);
			bool flag2 = KAnimLayering.IsOverrideAnimLayered(this.controller.OverrideAnimFiles, symbolFlags);
			flag = flag || flag2;
			if (flag && this.layer != Grid.SceneLayer.NoLayer)
			{
				bool flag3 = this.layerControllers == null || !this.layerControllers.ContainsKey(symbolFlags);
				if (flag3)
				{
					if (this.layerControllers == null)
					{
						this.layerControllers = new Dictionary<KAnim.SymbolFlags, KAnimControllerBase>();
					}
					if (this.links == null)
					{
						this.links = new Dictionary<KAnim.SymbolFlags, KAnimLink>();
					}
					GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, this.controller.gameObject, null);
					gameObject.name = this.controller.name + "_" + symbolFlags.ToString().ToLower();
					KAnimControllerBase component = gameObject.GetComponent<KAnimControllerBase>();
					if (flag2)
					{
						SymbolOverrideControllerUtil.AddToPrefab(gameObject).applySymbolOverridesEveryFrame = true;
					}
					this.layerControllers.Add(symbolFlags, component);
					this.links.Add(symbolFlags, new KAnimLink(this.controller, component));
					component.materialType = KAnimLayering.layerSettings[symbolFlags];
				}
				KAnimControllerBase kanimControllerBase = this.layerControllers[symbolFlags];
				kanimControllerBase.AnimFiles = this.controller.AnimFiles;
				kanimControllerBase.GetLayering().SetIsLayer(true);
				kanimControllerBase.initialAnim = this.controller.initialAnim;
				this.Dirty();
				KAnimSynchronizer synchronizer = this.controller.GetSynchronizer();
				if (flag3)
				{
					synchronizer.Add(kanimControllerBase, null);
				}
				else
				{
					this.RefreshForegroundBatchGroup();
				}
				synchronizer.Sync(kanimControllerBase);
				Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(this.layer) - this.controller.gameObject.transform.GetPosition().z - 0.1f);
				kanimControllerBase.gameObject.transform.SetLocalPosition(vector);
				kanimControllerBase.gameObject.SetActive(true);
				if (!flag2)
				{
					continue;
				}
				using (IEnumerator<KAnimControllerBase.OverrideAnimFileData> enumerator2 = this.controller.OverrideAnimFiles.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KAnimControllerBase.OverrideAnimFileData overrideAnimFileData = enumerator2.Current;
						kanimControllerBase.AddAnimOverrides(overrideAnimFileData.file, overrideAnimFileData.priority);
					}
					continue;
				}
			}
			KAnimControllerBase kanimControllerBase2;
			if (!flag && this.layerControllers != null && this.layerControllers.Count != 0 && this.layerControllers.TryGetValue(symbolFlags, out kanimControllerBase2))
			{
				this.controller.GetSynchronizer().Remove(kanimControllerBase2);
				kanimControllerBase2.gameObject.DeleteObject();
				this.layerControllers.Remove(symbolFlags);
				if (this.links != null)
				{
					this.links[symbolFlags].Unregister();
					this.links.Remove(symbolFlags);
				}
			}
		}
		if (this.layerControllers != null)
		{
			foreach (KeyValuePair<KAnim.SymbolFlags, KAnimControllerBase> keyValuePair in this.layerControllers)
			{
				this.HideSymbolsInternal(keyValuePair.Key);
				KAnimLayering layering = keyValuePair.Value.GetLayering();
				if (layering != null)
				{
					layering.HideSymbolsInternal(keyValuePair.Key);
				}
			}
		}
	}

	private void RefreshForegroundBatchGroup()
	{
		if (this.layerControllers == null)
		{
			return;
		}
		foreach (KeyValuePair<KAnim.SymbolFlags, KAnimControllerBase> keyValuePair in this.layerControllers)
		{
			foreach (KAnimControllerBase.OverrideAnimFileData overrideAnimFileData in new List<KAnimControllerBase.OverrideAnimFileData>(keyValuePair.Value.OverrideAnimFiles))
			{
				keyValuePair.Value.RemoveAnimOverrides(overrideAnimFileData.file);
			}
			keyValuePair.Value.GetComponent<KBatchedAnimController>().SwapAnims(keyValuePair.Value.AnimFiles);
		}
	}

	public void Dirty()
	{
		if (this.layerControllers == null)
		{
			return;
		}
		foreach (KeyValuePair<KAnim.SymbolFlags, KAnimControllerBase> keyValuePair in this.layerControllers)
		{
			keyValuePair.Value.Offset = this.controller.Offset;
			keyValuePair.Value.Pivot = this.controller.Pivot;
			keyValuePair.Value.Rotation = this.controller.Rotation;
			keyValuePair.Value.FlipX = this.controller.FlipX;
			keyValuePair.Value.FlipY = this.controller.FlipY;
		}
	}

	public static readonly KAnimHashedString UI = new KAnimHashedString("ui");

	private static Dictionary<KAnim.SymbolFlags, KAnimBatchGroup.MaterialType> layerSettings = new Dictionary<KAnim.SymbolFlags, KAnimBatchGroup.MaterialType>
	{
		{
			KAnim.SymbolFlags.FG,
			KAnimBatchGroup.MaterialType.Default
		},
		{
			KAnim.SymbolFlags.SH,
			KAnimBatchGroup.MaterialType.Shine
		}
	};

	private bool isLayer;

	private KAnimControllerBase controller;

	private Dictionary<KAnim.SymbolFlags, KAnimControllerBase> layerControllers;

	private Dictionary<KAnim.SymbolFlags, KAnimLink> links;

	private Grid.SceneLayer layer = Grid.SceneLayer.BuildingFront;
}
