using System;
using UnityEngine;

public class MeterController
{
	public MeterController(KMonoBehaviour target, Meter.Offset front_back, params string[] symbols_to_hide)
	{
		string[] array = new string[symbols_to_hide.Length + 1];
		Array.Copy(symbols_to_hide, array, symbols_to_hide.Length);
		array[array.Length - 1] = "meter_target";
		KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
		this.Initialize(component, "meter_target", "meter", front_back, Vector3.zero, array);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, params string[] symbols_to_hide)
	{
		this.Initialize(building_controller, meter_target, meter_animation, front_back, Vector3.zero, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		this.Initialize(building_controller, meter_target, meter_animation, front_back, tracker_offset, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, KBatchedAnimController meter_controller, params string[] symbol_names)
	{
		if (meter_controller == null)
		{
			return;
		}
		this.meterController = meter_controller;
		this.link = new KAnimLink(building_controller, meter_controller);
		for (int i = 0; i < symbol_names.Length; i++)
		{
			building_controller.HideSymbol(new KAnimHashedString(symbol_names[i]), true);
		}
		KBatchedAnimTracker component = this.meterController.GetComponent<KBatchedAnimTracker>();
		component.symbol = new HashedString(symbol_names[0]);
	}

	public KBatchedAnimController meterController { get; private set; }

	private void Initialize(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		string text = building_controller.name + "." + meter_animation;
		GameObject gameObject = new GameObject(text);
		gameObject.SetActive(false);
		gameObject.transform.parent = building_controller.transform;
		gameObject.transform.localPosition = Vector3.zero;
		this.gameObject = gameObject;
		KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
		kprefabID.PrefabTag = new Tag(text);
		Meter meter = gameObject.AddComponent<Meter>();
		meter.offset = front_back;
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.initialAnim = meter_animation;
		kbatchedAnimController.AddAnims(new KAnimFile[] { building_controller.GetAnims()[0] });
		kbatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
		kbatchedAnimController.initialMode = KAnim.PlayMode.Paused;
		kbatchedAnimController.isMovable = true;
		this.meterController = kbatchedAnimController;
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.offset = tracker_offset;
		kbatchedAnimTracker.symbol = new HashedString(meter_target);
		gameObject.SetActive(true);
		building_controller.HideSymbol(new KAnimHashedString(meter_target), true);
		for (int i = 0; i < symbols_to_hide.Length; i++)
		{
			building_controller.HideSymbol(new KAnimHashedString(symbols_to_hide[i]), true);
		}
		this.link = new KAnimLink(building_controller, kbatchedAnimController);
	}

	public void SetPositionPercent(float percent_full)
	{
		if (this.meterController == null)
		{
			return;
		}
		this.meterController.SetPositionPercent(percent_full);
	}

	public void SetFilterByAnim(bool filter_by_anim)
	{
		KBatchedAnimTracker component = this.gameObject.GetComponent<KBatchedAnimTracker>();
		component.filterByAnim = filter_by_anim;
	}

	public void SetVisible(bool visible)
	{
		if (this.gameObject != null)
		{
			this.gameObject.SetActive(visible);
		}
	}

	public void SetSymbolTint(KBatchedAnimController.SymbolTintIndex symbol_tint_idx, KAnimHashedString symbol, Color32 colour)
	{
		if (this.meterController != null)
		{
			this.meterController.SetSymbolTint(symbol_tint_idx, symbol, colour);
		}
	}

	public GameObject gameObject;

	private KAnimLink link;
}
