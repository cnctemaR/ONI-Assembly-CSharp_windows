using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/KCanvasScaler")]
public class KCanvasScaler : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (KPlayerPrefs.HasKey(KCanvasScaler.UIScalePrefKey))
		{
			this.SetUserScale(KPlayerPrefs.GetFloat(KCanvasScaler.UIScalePrefKey) / 100f);
		}
		else
		{
			this.SetUserScale(1f);
		}
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
	}

	private void OnResize()
	{
		this.SetUserScale(this.userScale);
	}

	public void SetUserScale(float scale)
	{
		if (this.canvasScaler == null)
		{
			this.canvasScaler = base.GetComponent<CanvasScaler>();
		}
		this.userScale = scale;
		this.canvasScaler.scaleFactor = this.GetCanvasScale();
	}

	public float GetUserScale()
	{
		return this.userScale;
	}

	public float GetCanvasScale()
	{
		return this.userScale * this.ScreenRelativeScale();
	}

	private float ScreenRelativeScale()
	{
		float dpi = Screen.dpi;
		Camera camera = Camera.main;
		if (camera == null)
		{
			camera = global::UnityEngine.Object.FindFirstObjectByType<Camera>();
		}
		camera != null;
		float num = (float)Screen.width / (float)Screen.height;
		if ((float)Screen.height <= this.scaleSteps[0].maxRes_y || num < 1.6f)
		{
			return this.scaleSteps[0].scale;
		}
		if ((float)Screen.height > this.scaleSteps[this.scaleSteps.Length - 1].maxRes_y)
		{
			return this.scaleSteps[this.scaleSteps.Length - 1].scale;
		}
		for (int i = 0; i < this.scaleSteps.Length; i++)
		{
			if ((float)Screen.height > this.scaleSteps[i].maxRes_y && (float)Screen.height <= this.scaleSteps[i + 1].maxRes_y)
			{
				float num2 = ((float)Screen.height - this.scaleSteps[i].maxRes_y) / (this.scaleSteps[i + 1].maxRes_y - this.scaleSteps[i].maxRes_y);
				return Mathf.Lerp(this.scaleSteps[i].scale, this.scaleSteps[i + 1].scale, num2);
			}
		}
		return 1f;
	}

	[MyCmpReq]
	private CanvasScaler canvasScaler;

	public static string UIScalePrefKey = "UIScalePref";

	private float userScale = 1f;

	[Range(0.75f, 2f)]
	private KCanvasScaler.ScaleStep[] scaleSteps = new KCanvasScaler.ScaleStep[]
	{
		new KCanvasScaler.ScaleStep(720f, 0.86f),
		new KCanvasScaler.ScaleStep(1080f, 1f),
		new KCanvasScaler.ScaleStep(2160f, 1.33f)
	};

	[Serializable]
	public struct ScaleStep
	{
		public ScaleStep(float maxRes_y, float scale)
		{
			this.maxRes_y = maxRes_y;
			this.scale = scale;
		}

		public float scale;

		public float maxRes_y;
	}
}
