using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPaintElementScreen : KScreen
{
	public static DebugPaintElementScreen Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugPaintElementScreen.Instance = this;
		this.massPressureInput.onValueChanged.AddListener(delegate
		{
			this.OnChangeMassPressure();
			this.blockInput = true;
		});
		this.temperatureInput.onValueChanged.AddListener(delegate
		{
			this.OnChangeTemperature();
			this.blockInput = true;
		});
		this.temperatureInput.onEndEdit.AddListener(delegate
		{
			this.blockInput = false;
		});
		this.massPressureInput.onEndEdit.AddListener(delegate
		{
			this.blockInput = false;
		});
		base.gameObject.SetActive(false);
	}

	public override float GetSortKey()
	{
		return 5f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.element = SimHashes.Ice;
		if (this.paintButton != null)
		{
			this.paintButton.onClick += this.OnClickPaint;
		}
		if (this.elementButton != null)
		{
			this.elementButton.onClick += this.OnClickElement;
		}
		List<string> list = new List<string>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.name != "Element Not Loaded")
			{
				list.Add(element.id.ToString());
			}
		}
		list.Sort();
		list.Insert(0, ElementLoader.FindElementByHash(SimHashes.Vacuum).id.ToString());
		list.Insert(0, ElementLoader.FindElementByHash(SimHashes.Dirt).id.ToString());
		list.Insert(0, ElementLoader.FindElementByHash(SimHashes.CarbonDioxide).id.ToString());
		list.Insert(0, ElementLoader.FindElementByHash(SimHashes.Water).id.ToString());
		list.Insert(0, ElementLoader.FindElementByHash(SimHashes.Oxygen).id.ToString());
		this.menu = base.GetComponentInChildren<KPopupMenu>(true);
		this.menu.SetOptions(list.ToArray());
		KPopupMenu kpopupMenu = this.menu;
		kpopupMenu.OnSelect = (Action<string>)Delegate.Combine(kpopupMenu.OnSelect, new Action<string>(this.OnSelect));
		this.menu.SelectOption(this.element.ToString());
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!this.blockInput || e.TryConsume(global::Action.Plan1) || e.TryConsume(global::Action.Plan2) || e.TryConsume(global::Action.Plan3) || e.TryConsume(global::Action.Plan4) || e.TryConsume(global::Action.Plan5) || e.TryConsume(global::Action.Plan6) || e.TryConsume(global::Action.Plan7) || e.TryConsume(global::Action.Plan8) || e.TryConsume(global::Action.Plan9) || e.TryConsume(global::Action.Plan10) || e.TryConsume(global::Action.DebugToggle))
		{
		}
		base.OnKeyDown(e);
	}

	private void OnClickPaint()
	{
		this.OnChangeMassPressure();
		this.OnChangeTemperature();
		DebugTool.Instance.Activate(DebugTool.Type.ReplaceSubstance);
	}

	private void OnClickElement()
	{
		if (this.ElementsMenu.transform.parent != this.transform.parent)
		{
			this.ElementsMenu.transform.SetParent(this.transform.parent);
			this.ElementsMenu.transform.localScale = Vector3.one;
			this.ElementsMenu.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
		}
		this.ElementsMenu.SetActive(!this.ElementsMenu.activeSelf);
		if (!this.menu.gameObject.activeSelf)
		{
			this.menu.OnClick();
		}
	}

	private void OnSelect(string str)
	{
		this.element = (SimHashes)((int)Enum.Parse(typeof(SimHashes), str));
		this.elementButton.GetComponentInChildren<LocText>().text = str;
		this.ElementsMenu.SetActive(false);
	}

	public void OnChangeMassPressure()
	{
		float num;
		try
		{
			num = Convert.ToSingle(this.massPressureInput.text);
		}
		catch
		{
			num = -1f;
		}
		this.mass = num;
	}

	public void OnChangeTemperature()
	{
		float num;
		try
		{
			num = Convert.ToSingle(this.temperatureInput.text);
		}
		catch
		{
			num = -1f;
		}
		this.temperature = num;
	}

	public SimHashes element;

	[NonSerialized]
	public float mass = 1000f;

	[NonSerialized]
	public float temperature = -1f;

	[SerializeField]
	private InputField massPressureInput;

	[SerializeField]
	private InputField temperatureInput;

	private KPopupMenu menu;

	public GameObject ElementsMenu;

	public KButton elementButton;

	public KButton paintButton;

	public Toggle paintElement;

	public Toggle paintMass;

	public Toggle paintTemperature;

	public Toggle AffectBuildings;

	private bool blockInput;
}
