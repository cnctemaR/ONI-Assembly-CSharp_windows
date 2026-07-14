using System;
using UnityEngine;

public class MeterController
{
	public static float StandardLerp(float percentage, int frames)
	{
		return percentage;
	}

	public static float MinMaxStepLerp(float percentage, int frames)
	{
		if ((double)percentage <= 0.0 || frames <= 1)
		{
			return 0f;
		}
		if ((double)percentage >= 1.0 || frames == 2)
		{
			return 1f;
		}
		return (1f + percentage * (float)(frames - 2)) / (float)frames;
	}

	public KBatchedAnimController meterController { get; private set; }

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

	private void Initialize(KAnimControllerBase building_controller, string meter_target, string meter_animation, Meter.Offset front_back, Grid.SceneLayer user_specified_render_layer, Vector3 tracker_offset, params string[] symbols_to_hide)
	{
		if (building_controller.HasAnimation(meter_animation + "_cb") && !GlobalAssets.Instance.colorSet.IsDefaultColorSet())
		{
			meter_animation += "_cb";
		}
		string text = building_controller.name + "." + meter_animation;
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(Assets.GetPrefab(MeterConfig.ID));
		gameObject.name = text;
		gameObject.SetActive(false);
		gameObject.transform.parent = building_controller.transform;
		this.gameObject = gameObject;
		gameObject.GetComponent<KPrefabID>().PrefabTag = new Tag(text);
		Vector3 position = building_controller.transform.GetPosition();
		switch (front_back)
		{
		case Meter.Offset.Infront:
			position.z -= 0.1f;
			break;
		case Meter.Offset.Behind:
			position.z += 0.1f;
			break;
		case Meter.Offset.UserSpecified:
			position.z = Grid.GetLayerZ(user_specified_render_layer);
			break;
		}
		gameObject.transform.SetPosition(position);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.AnimFiles = new KAnimFile[] { building_controller.AnimFiles[0] };
		component.initialAnim = meter_animation;
		component.fgLayer = Grid.SceneLayer.NoLayer;
		component.initialMode = KAnim.PlayMode.Paused;
		component.isMovable = true;
		component.FlipX = building_controller.FlipX;
		component.FlipY = building_controller.FlipY;
		component.CopyBlendValue(building_controller);
		if (Meter.Offset.UserSpecified == front_back)
		{
			component.sceneLayer = user_specified_render_layer;
		}
		this.meterController = component;
		KBatchedAnimTracker component2 = gameObject.GetComponent<KBatchedAnimTracker>();
		component2.offset = tracker_offset;
		component2.symbol = new HashedString(meter_target);
		gameObject.SetActive(true);
		building_controller.SetSymbolVisiblity(meter_target, false);
		if (symbols_to_hide != null)
		{
			for (int i = 0; i < symbols_to_hide.Length; i++)
			{
				building_controller.SetSymbolVisiblity(symbols_to_hide[i], false);
			}
		}
		this.link = new KAnimLink(building_controller, component);
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
		this.meterController.GetComponent<KBatchedAnimTracker>().symbol = new HashedString(symbol_names[0]);
	}

	public void SetPositionPercent(float percent_full)
	{
		if (this.meterController == null)
		{
			return;
		}
		this.meterController.SetPositionPercent(this.interpolateFunction(percent_full, this.meterController.GetCurrentNumFrames()));
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

	public void Unlink()
	{
		if (this.link != null)
		{
			this.link.Unregister();
			this.link = null;
		}
	}

	public GameObject gameObject;

	public Func<float, int, float> interpolateFunction = new Func<float, int, float>(MeterController.MinMaxStepLerp);

	public bool supressKAnimLayers = true;

	private KAnimLink link;
}
