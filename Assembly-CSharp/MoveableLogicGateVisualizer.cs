using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[SkipSaveFileSerialization]
public class MoveableLogicGateVisualizer : LogicGateBase
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.cell = -1;
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Combine(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
		this.OnOverlayChanged(OverlayScreen.Instance.mode);
		base.Subscribe<MoveableLogicGateVisualizer>(-1643076535, MoveableLogicGateVisualizer.OnRotatedDelegate);
	}

	protected override void OnCleanUp()
	{
		OverlayScreen instance = OverlayScreen.Instance;
		instance.OnOverlayChanged = (Action<HashedString>)Delegate.Remove(instance.OnOverlayChanged, new Action<HashedString>(this.OnOverlayChanged));
		this.Unregister();
		base.OnCleanUp();
	}

	private void OnOverlayChanged(HashedString mode)
	{
		if (mode == OverlayModes.Logic.ID)
		{
			this.Register();
			return;
		}
		this.Unregister();
	}

	private void OnRotated(object data)
	{
		this.Unregister();
		this.OnOverlayChanged(OverlayScreen.Instance.mode);
	}

	private void Update()
	{
		if (this.visChildren.Count <= 0)
		{
			return;
		}
		int num = Grid.PosToCell(base.transform.GetPosition());
		if (num == this.cell)
		{
			return;
		}
		this.cell = num;
		this.Unregister();
		this.Register();
	}

	private GameObject CreateUIElem(int cell, bool is_input)
	{
		GameObject gameObject = Util.KInstantiate(LogicGateBase.uiSrcData.prefab, Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas, null, true, 0);
		Image component = gameObject.GetComponent<Image>();
		component.sprite = (is_input ? LogicGateBase.uiSrcData.inputSprite : LogicGateBase.uiSrcData.outputSprite);
		component.raycastTarget = false;
		return gameObject;
	}

	private void Register()
	{
		if (this.visChildren.Count > 0)
		{
			return;
		}
		base.enabled = true;
		this.visChildren.Add(this.CreateUIElem(base.OutputCellOne, false));
		if (base.RequiresFourOutputs)
		{
			this.visChildren.Add(this.CreateUIElem(base.OutputCellTwo, false));
			this.visChildren.Add(this.CreateUIElem(base.OutputCellThree, false));
			this.visChildren.Add(this.CreateUIElem(base.OutputCellFour, false));
		}
		this.visChildren.Add(this.CreateUIElem(base.InputCellOne, true));
		if (base.RequiresTwoInputs)
		{
			this.visChildren.Add(this.CreateUIElem(base.InputCellTwo, true));
		}
		else if (base.RequiresFourInputs)
		{
			this.visChildren.Add(this.CreateUIElem(base.InputCellTwo, true));
			this.visChildren.Add(this.CreateUIElem(base.InputCellThree, true));
			this.visChildren.Add(this.CreateUIElem(base.InputCellFour, true));
		}
		if (base.RequiresControlInputs)
		{
			this.visChildren.Add(this.CreateUIElem(base.ControlCellOne, true));
			this.visChildren.Add(this.CreateUIElem(base.ControlCellTwo, true));
		}
	}

	private void Unregister()
	{
		if (this.visChildren.Count <= 0)
		{
			return;
		}
		base.enabled = false;
		this.cell = -1;
		foreach (GameObject gameObject in this.visChildren)
		{
			Util.KDestroyGameObject(gameObject);
		}
		this.visChildren.Clear();
	}

	private int cell;

	protected List<GameObject> visChildren = new List<GameObject>();

	private static readonly EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MoveableLogicGateVisualizer>(delegate(MoveableLogicGateVisualizer component, object data)
	{
		component.OnRotated(data);
	});
}
