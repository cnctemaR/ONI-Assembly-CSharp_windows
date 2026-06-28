using System;
using UnityEngine;

public class GameScreenManager : KMonoBehaviour
{
	public static GameScreenManager Instance { get; private set; }

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
		GameScreenManager.Instance = this;
	}

	protected override void OnCleanUp()
	{
		GameScreenManager.Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Camera GetCamera(GameScreenManager.UIRenderTarget target)
	{
		Camera camera;
		switch (target)
		{
		case GameScreenManager.UIRenderTarget.WorldSpace:
			camera = this.worldSpaceCanvas.GetComponent<Canvas>().worldCamera;
			break;
		case GameScreenManager.UIRenderTarget.ScreenSpaceCamera:
			camera = this.ssCameraCanvas.GetComponent<Canvas>().worldCamera;
			break;
		case GameScreenManager.UIRenderTarget.ScreenSpaceOverlay:
			camera = this.ssOverlayCanvas.GetComponent<Canvas>().worldCamera;
			break;
		case GameScreenManager.UIRenderTarget.HoverTextScreen:
			camera = this.ssHoverTextCanvas.GetComponent<Canvas>().worldCamera;
			break;
		default:
			camera = base.gameObject.GetComponent<Canvas>().worldCamera;
			break;
		}
		return camera;
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
		}
		this.ssCameraCanvas.GetComponent<Canvas>().worldCamera = camera;
	}

	public GameObject GetParent(GameScreenManager.UIRenderTarget target)
	{
		GameObject gameObject;
		switch (target)
		{
		case GameScreenManager.UIRenderTarget.WorldSpace:
			gameObject = this.worldSpaceCanvas;
			break;
		case GameScreenManager.UIRenderTarget.ScreenSpaceCamera:
			gameObject = this.ssCameraCanvas;
			break;
		case GameScreenManager.UIRenderTarget.ScreenSpaceOverlay:
			gameObject = this.ssOverlayCanvas;
			break;
		case GameScreenManager.UIRenderTarget.HoverTextScreen:
			gameObject = this.ssHoverTextCanvas;
			break;
		default:
			gameObject = base.gameObject;
			break;
		}
		return gameObject;
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

	[SerializeField]
	private Color[] uiColors;

	public enum UIRenderTarget
	{
		WorldSpace,
		ScreenSpaceCamera,
		ScreenSpaceOverlay,
		HoverTextScreen
	}
}
