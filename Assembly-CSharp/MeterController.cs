using System;
using UnityEngine;

public class MeterController
{
	public MeterController(KMonoBehaviour target, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		string[] array = new string[symbols_to_hide.Length + 1];
		Array.Copy(symbols_to_hide, array, symbols_to_hide.Length);
		array[array.Length - 1] = "meter_target";
		KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
		this.Initialize(component, "meter_target", "meter", front_back, user_specified_render_layer, Vector3.zero, array);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, params string[] symbols_to_hide)
	{
		this.Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, Vector3.zero, symbols_to_hide);
	}

	public MeterController(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		this.Initialize(building_controller, meter_target, meter_animation, front_back, user_specified_render_layer, tracker_offset, symbols_to_hide);
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
			building_controller.SetSymbolVisiblity(symbol_names[i], false);
		}
		KBatchedAnimTracker component = this.meterController.GetComponent<KBatchedAnimTracker>();
		component.symbol = new HashedString(symbol_names[0]);
	}

	public KBatchedAnimController meterController { get; private set; }

	private void Initialize(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		string text = building_controller.name + "." + meter_animation;
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(Assets.GetPrefab(MeterConfig.ID));
		gameObject.name = text;
		gameObject.SetActive(false);
		gameObject.transform.parent = building_controller.transform;
		this.gameObject = gameObject;
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.PrefabTag = new Tag(text);
		Vector3 position = building_controller.transform.GetPosition();
		if (front_back != Meter.Offset.Behind)
		{
			if (front_back != Meter.Offset.Infront)
			{
				if (front_back == Meter.Offset.UserSpecified)
				{
					position.z = Grid.GetLayerZ(user_specified_render_layer);
				}
			}
			else
			{
				position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront);
			}
		}
		else
		{
			position.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingBack);
		}
		gameObject.transform.SetPosition(position);
		KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
		component2.AnimFiles = new KAnimFile[] { building_controller.AnimFiles[0] };
		component2.initialAnim = meter_animation;
		component2.fgLayer = Grid.SceneLayer.NoLayer;
		component2.initialMode = KAnim.PlayMode.Paused;
		component2.isMovable = true;
		component2.FlipX = building_controller.FlipX;
		component2.FlipY = building_controller.FlipY;
		this.meterController = component2;
		KBatchedAnimTracker component3 = gameObject.GetComponent<KBatchedAnimTracker>();
		component3.offset = tracker_offset;
		component3.symbol = new HashedString(meter_target);
		gameObject.SetActive(true);
		building_controller.SetSymbolVisiblity(meter_target, false);
		if (symbols_to_hide != null)
		{
			for (int i = 0; i < symbols_to_hide.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbols_to_hide[i], false);
			}
		}
		this.link = new KAnimLink(building_controller, component2);
	}

	public void SetPositionPercent(float percent_full)
	{
		if (this.meterController == null)
		{
			return;
		}
		this.meterController.SetPositionPercent(percent_full);
	}

	public void SetSymbolTint(KAnimHashedString symbol, Color32 colour)
	{
		if (this.meterController != null)
		{
			this.meterController.SetSymbolTint(symbol, colour);
		}
	}

	public void SetRotation(float rot)
	{
		if (this.meterController == null)
		{
			return;
		}
		this.meterController.Rotation = rot;
	}

	public GameObject gameObject;

	private KAnimLink link;
}
