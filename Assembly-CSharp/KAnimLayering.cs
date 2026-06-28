using System;
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
		if (this.foregroundController != null)
		{
			Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(layer) - this.controller.gameObject.transform.position.z - 0.1f);
			this.foregroundController.transform.SetLocalPosition(vector);
		}
	}

	public void SetIsForeground(bool is_foreground)
	{
		this.isForeground = is_foreground;
	}

	public bool GetIsForeground()
	{
		return this.isForeground;
	}

	private static bool IsAnimLayered(KAnimFile[] anims)
	{
		foreach (KAnimFile kanimFile in anims)
		{
			if (!(kanimFile == null))
			{
				KAnimFileData data = kanimFile.GetData();
				if (data.build != null)
				{
					KAnim.Build.Symbol[] symbols = data.build.symbols;
					for (int j = 0; j < symbols.Length; j++)
					{
						if ((symbols[j].flags & 8) != 0)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private void HideSymbolsInternal()
	{
		foreach (KAnimFile kanimFile in this.controller.GetAnims())
		{
			if (!(kanimFile == null))
			{
				KAnimFileData data = kanimFile.GetData();
				if (data.build != null)
				{
					KAnim.Build.Symbol[] symbols = data.build.symbols;
					for (int j = 0; j < symbols.Length; j++)
					{
						bool flag = (symbols[j].flags & 8) != 0;
						if (flag != this.isForeground)
						{
							if (!(symbols[j].hash == KAnimLayering.UI))
							{
								this.controller.HideSymbol(symbols[j].hash, true);
							}
						}
					}
				}
			}
		}
	}

	public void HideSymbols()
	{
		if (EntityPrefabs.Instance == null)
		{
			return;
		}
		if (this.isForeground)
		{
			return;
		}
		KAnimFile[] anims = this.controller.GetAnims();
		bool flag = KAnimLayering.IsAnimLayered(anims);
		if (flag && this.foregroundController == null && this.layer != Grid.SceneLayer.NoLayer)
		{
			GameObject gameObject = Util.KInstantiate(EntityPrefabs.Instance.ForegroundLayer, this.controller.gameObject, null);
			gameObject.name = this.controller.name + "_fg";
			this.foregroundController = gameObject.GetComponent<KAnimControllerBase>();
			this.foregroundController.GetLayering().SetIsForeground(true);
			this.foregroundController.SetAnims(anims, true);
			this.foregroundController.initialAnim = this.controller.initialAnim;
			this.link = new KAnimLink(this.controller, this.foregroundController);
			this.Dirty();
			this.controller.GetSynchronizer().Add(this.foregroundController);
			Vector3 vector = new Vector3(0f, 0f, Grid.GetLayerZ(this.layer) - this.controller.gameObject.transform.position.z - 0.1f);
			gameObject.transform.SetLocalPosition(vector);
			gameObject.SetActive(true);
		}
		else if (!flag && this.foregroundController != null)
		{
			this.controller.GetSynchronizer().Remove(this.foregroundController);
			this.foregroundController.gameObject.DeleteObject();
			this.link = null;
		}
		if (this.foregroundController != null)
		{
			this.HideSymbolsInternal();
			KAnimLayering layering = this.foregroundController.GetLayering();
			if (layering != null)
			{
				layering.HideSymbolsInternal();
			}
		}
	}

	public void Dirty()
	{
		if (this.foregroundController == null)
		{
			return;
		}
		this.foregroundController.Offset = this.controller.Offset;
		this.foregroundController.Pivot = this.controller.Pivot;
		this.foregroundController.Rotation = this.controller.Rotation;
		this.foregroundController.FlipX = this.controller.FlipX;
		this.foregroundController.FlipY = this.controller.FlipY;
	}

	private bool isForeground;

	private KAnimControllerBase controller;

	private KAnimControllerBase foregroundController;

	private KAnimLink link;

	private Grid.SceneLayer layer = Grid.SceneLayer.BuildingFront;

	public static readonly KAnimHashedString UI = new KAnimHashedString("ui");
}
