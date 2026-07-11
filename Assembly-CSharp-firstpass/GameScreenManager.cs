using System;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenManager : KMonoBehaviour
{
	public static GameScreenManager Instance { get; private set; }

	public static void DestroyInstance()
	{
		GameScreenManager.Instance = null;
	}

	public static Color[] UIColors
	{
		get
		{
			return GameScreenManager.Instance.uiColors;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		global::Debug.Assert(GameScreenManager.Instance == null);
		GameScreenManager.Instance = this;
	}

	protected override void OnCleanUp()
	{
		global::Debug.Assert(GameScreenManager.Instance != null);
		GameScreenManager.Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Camera GetCamera(GameScreenManager.UIRenderTarget target)
	{
		switch (target)
		{
		case GameScreenManager.UIRenderTarget.WorldSpace:
			return this.worldSpaceCanvas.GetComponent<Canvas>().worldCamera;
		case GameScreenManager.UIRenderTarget.ScreenSpaceCamera:
			return this.ssCameraCanvas.GetComponent<Canvas>().worldCamera;
		case GameScreenManager.UIRenderTarget.ScreenSpaceOverlay:
			return this.ssOverlayCanvas.GetComponent<Canvas>().worldCamera;
		case GameScreenManager.UIRenderTarget.HoverTextScreen:
			return this.ssHoverTextCanvas.GetComponent<Canvas>().worldCamera;
		case GameScreenManager.UIRenderTarget.ScreenshotModeCamera:
			return this.screenshotModeCanvas.GetComponent<Canvas>().worldCamera;
		default:
			return base.gameObject.GetComponent<Canvas>().worldCamera;
		}
	}

	public void SetCamera(GameScreenManager.UIRenderTarget target, Camera camera)
	{
		switch (target)
		{
		case GameScreenManager.UIRenderTarget.WorldSpace:
			this.worldSpaceCanvas.GetComponent<Canvas>().worldCamera = camera;
			return;
		case GameScreenManager.UIRenderTarget.ScreenSpaceOverlay:
			this.ssOverlayCanvas.GetComponent<Canvas>().worldCamera = camera;
			return;
		case GameScreenManager.UIRenderTarget.ScreenshotModeCamera:
			this.screenshotModeCanvas.GetComponent<Canvas>().worldCamera = camera;
			return;
		}
		this.ssCameraCanvas.GetComponent<Canvas>().worldCamera = camera;
	}

	public GameObject GetParent(GameScreenManager.UIRenderTarget target)
	{
		switch (target)
		{
		case GameScreenManager.UIRenderTarget.WorldSpace:
			return this.worldSpaceCanvas;
		case GameScreenManager.UIRenderTarget.ScreenSpaceCamera:
			return this.ssCameraCanvas;
		case GameScreenManager.UIRenderTarget.ScreenSpaceOverlay:
			return this.ssOverlayCanvas;
		case GameScreenManager.UIRenderTarget.HoverTextScreen:
			return this.ssHoverTextCanvas;
		case GameScreenManager.UIRenderTarget.ScreenshotModeCamera:
			return this.screenshotModeCanvas;
		default:
			return base.gameObject;
		}
	}

	public GameObject ActivateScreen(GameObject screen, GameObject parent = null, GameScreenManager.UIRenderTarget target = GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = this.GetParent(target);
		}
		KScreenManager.AddExistingChild(parent, screen);
		KScreen component = screen.GetComponent<KScreen>();
		component.Activate();
		return screen;
	}

	public KScreen InstantiateScreen(GameObject screenPrefab, GameObject parent = null, GameScreenManager.UIRenderTarget target = GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = this.GetParent(target);
		}
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		return gameObject.GetComponent<KScreen>();
	}

	public KScreen StartScreen(GameObject screenPrefab, GameObject parent = null, GameScreenManager.UIRenderTarget target = GameScreenManager.UIRenderTarget.ScreenSpaceOverlay)
	{
		if (parent == null)
		{
			parent = this.GetParent(target);
		}
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component;
	}

	public GameObject ssHoverTextCanvas;

	public GameObject ssCameraCanvas;

	public GameObject ssOverlayCanvas;

	public GameObject worldSpaceCanvas;

	public GameObject screenshotModeCanvas;

	[SerializeField]
	private Color[] uiColors;

	public Image fadePlane;

	public enum UIRenderTarget
	{
		WorldSpace,
		ScreenSpaceCamera,
		ScreenSpaceOverlay,
		HoverTextScreen,
		ScreenshotModeCamera
	}
}
