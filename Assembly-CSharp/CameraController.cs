using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class CameraController : KMonoBehaviour, IInputHandler
{
	public string handlerName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	public KInputHandler inputHandler { get; set; }

	public float targetOrthographicSize { get; private set; }

	public bool isTargetPosSet { get; set; }

	public Vector3 targetPos { get; private set; }

	public bool DisableUserCameraControl
	{
		get
		{
			return this.userCameraControlDisabled;
		}
		set
		{
			this.userCameraControlDisabled = value;
			if (this.userCameraControlDisabled)
			{
				this.panning = false;
				this.panLeft = false;
				this.panRight = false;
				this.panUp = false;
				this.panDown = false;
			}
		}
	}

	public static CameraController Instance { get; private set; }

	public static void DestroyInstance()
	{
		CameraController.Instance = null;
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		this.mrt.ToggleColouredOverlayView(enabled);
	}

	protected override void OnPrefabInit()
	{
		Util.Reset(base.transform);
		base.transform.SetLocalPosition(new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -100f));
		this.targetOrthographicSize = this.maxOrthographicSize;
		CameraController.Instance = this;
		this.DisableUserCameraControl = false;
		this.baseCamera = this.CopyCamera(Camera.main, "baseCamera");
		this.mrt = this.baseCamera.gameObject.AddComponent<MultipleRenderTarget>();
		this.mrt.onSetupComplete += this.OnMRTSetupComplete;
		this.baseCamera.gameObject.AddComponent<LightBufferCompositor>();
		this.baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		this.baseCamera.transform.parent = base.transform;
		Util.Reset(this.baseCamera.transform);
		int mask = LayerMask.GetMask(new string[] { "PlaceWithDepth", "Overlay" });
		int mask2 = LayerMask.GetMask(new string[] { "Construction" });
		this.cameras.Add(this.baseCamera);
		this.baseCamera.cullingMask &= ~mask;
		this.baseCamera.cullingMask |= mask2;
		this.baseCamera.tag = "Untagged";
		this.baseCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_LitTex";
		this.infraredCamera = this.CopyCamera(this.baseCamera, "Infrared");
		this.infraredCamera.cullingMask = 0;
		this.infraredCamera.clearFlags = CameraClearFlags.Color;
		this.infraredCamera.depth = this.baseCamera.depth - 1f;
		this.infraredCamera.transform.parent = base.transform;
		this.infraredCamera.gameObject.AddComponent<Infrared>();
		this.simOverlayCamera = this.CopyCamera(this.baseCamera, "SimOverlayCamera");
		this.simOverlayCamera.cullingMask = LayerMask.GetMask(new string[] { "SimDebugView" });
		this.simOverlayCamera.clearFlags = CameraClearFlags.Color;
		this.simOverlayCamera.depth = this.baseCamera.depth + 1f;
		this.simOverlayCamera.transform.parent = base.transform;
		this.simOverlayCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_SimDebugViewTex";
		this.overlayCamera = Camera.main;
		this.overlayCamera.name = "Overlay";
		this.overlayCamera.cullingMask = mask | mask2;
		this.overlayCamera.clearFlags = CameraClearFlags.Nothing;
		this.overlayCamera.transform.parent = base.transform;
		this.overlayCamera.depth = this.baseCamera.depth + 3f;
		this.overlayCamera.transform.SetLocalPosition(Vector3.zero);
		this.overlayCamera.transform.localRotation = Quaternion.identity;
		this.overlayCamera.renderingPath = RenderingPath.Forward;
		this.overlayCamera.allowHDR = false;
		this.overlayCamera.tag = "Untagged";
		CameraReferenceTexture cameraReferenceTexture = this.overlayCamera.gameObject.AddComponent<CameraReferenceTexture>();
		cameraReferenceTexture.referenceCamera = this.baseCamera;
		ColorCorrectionLookup component = this.overlayCamera.GetComponent<ColorCorrectionLookup>();
		component.Convert(this.dayColourCube, string.Empty);
		component.Convert2(this.nightColourCube, string.Empty);
		this.cameras.Add(this.overlayCamera);
		this.lightBufferCamera = this.CopyCamera(this.overlayCamera, "Light Buffer");
		this.lightBufferCamera.clearFlags = CameraClearFlags.Color;
		this.lightBufferCamera.cullingMask = LayerMask.GetMask(new string[] { "Lights" });
		this.lightBufferCamera.depth = this.baseCamera.depth - 1f;
		this.lightBufferCamera.transform.parent = base.transform;
		this.lightBufferCamera.transform.SetLocalPosition(Vector3.zero);
		this.lightBufferCamera.rect = new Rect(0f, 0f, 1f, 1f);
		LightBuffer lightBuffer = this.lightBufferCamera.gameObject.AddComponent<LightBuffer>();
		lightBuffer.Material = this.LightBufferMaterial;
		lightBuffer.CircleMaterial = this.LightCircleOverlay;
		lightBuffer.ConeMaterial = this.LightConeOverlay;
		this.overlayNoDepthCamera = this.CopyCamera(this.overlayCamera, "overlayNoDepth");
		int mask3 = LayerMask.GetMask(new string[] { "Overlay", "Place" });
		this.baseCamera.cullingMask &= ~mask3;
		this.overlayNoDepthCamera.clearFlags = CameraClearFlags.Depth;
		this.overlayNoDepthCamera.cullingMask = mask3;
		this.overlayNoDepthCamera.transform.parent = base.transform;
		this.overlayNoDepthCamera.transform.SetLocalPosition(Vector3.zero);
		this.overlayNoDepthCamera.depth = this.baseCamera.depth + 4f;
		this.overlayNoDepthCamera.tag = "MainCamera";
		this.overlayNoDepthCamera.gameObject.AddComponent<NavPathDrawer>();
		this.uiCamera = this.CopyCamera(this.overlayCamera, "uiCamera");
		this.uiCamera.clearFlags = CameraClearFlags.Depth;
		this.uiCamera.cullingMask = LayerMask.GetMask(new string[] { "UI" });
		this.uiCamera.transform.parent = base.transform;
		this.uiCamera.transform.SetLocalPosition(Vector3.zero);
		this.uiCamera.depth = this.baseCamera.depth + 5f;
		this.timelapseFreezeCamera = this.CopyCamera(this.uiCamera, "timelapseFreezeCamera");
		this.timelapseFreezeCamera.depth = this.uiCamera.depth + 3f;
		this.timelapseFreezeCamera.gameObject.AddComponent<FillRenderTargetEffect>();
		this.timelapseFreezeCamera.enabled = false;
		Camera camera = CameraController.CloneCamera(this.overlayCamera, "timelapseCamera");
		Timelapser timelapser = camera.gameObject.AddComponent<Timelapser>();
		camera.transparencySortMode = TransparencySortMode.Orthographic;
		Game.Instance.timelapser = timelapser;
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.ScreenSpaceCamera, this.uiCamera);
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.WorldSpace, this.uiCamera);
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.ScreenshotModeCamera, this.uiCamera);
		this.infoText = GameScreenManager.Instance.screenshotModeCanvas.GetComponentInChildren<LocText>();
	}

	public static Camera CloneCamera(Camera camera, string name)
	{
		Camera camera2 = new GameObject
		{
			name = name
		}.AddComponent<Camera>();
		camera2.CopyFrom(camera);
		return camera2;
	}

	private Camera CopyCamera(Camera camera, string name)
	{
		Camera camera2 = CameraController.CloneCamera(camera, name);
		this.cameras.Add(camera2);
		return camera2;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Restore();
	}

	public void FadeOut(float targetPercentage = 1f, float speed = 1f)
	{
		if (this.activeFadeRoutine != null)
		{
			base.StopCoroutine(this.activeFadeRoutine);
		}
		this.activeFadeRoutine = base.StartCoroutine(this.FadeToBlack(targetPercentage, speed));
	}

	public void FadeIn(float targetPercentage = 0f, float speed = 1f)
	{
		if (this.activeFadeRoutine != null)
		{
			base.StopCoroutine(this.activeFadeRoutine);
		}
		this.activeFadeRoutine = base.StartCoroutine(this.FadeInFromBlack(targetPercentage, speed));
	}

	public void SetWorldInteractive(bool state)
	{
		GameScreenManager.Instance.fadePlane.raycastTarget = !state;
	}

	private IEnumerator FadeToBlack(float targetBlackPercent = 1f, float speed = 1f)
	{
		float currentAlphaPercentage = Mathf.Max(0f, GameScreenManager.Instance.fadePlane.color.a);
		float duration = 1f;
		for (float i = 0f; i <= duration; i += Time.unscaledDeltaTime * speed)
		{
			currentAlphaPercentage = Mathf.Max(Mathf.Min(i / duration, targetBlackPercent), GameScreenManager.Instance.fadePlane.color.a);
			GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, currentAlphaPercentage);
			yield return 0;
		}
		GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, targetBlackPercent);
		this.activeFadeRoutine = null;
		yield return 0;
		yield break;
	}

	private IEnumerator FadeInFromBlack(float targetBlackPercent = 0f, float speed = 1f)
	{
		float alphaPercentage = Mathf.Min(1f, GameScreenManager.Instance.fadePlane.color.a);
		float duration = 1f;
		for (float i = 0f; i <= duration; i += Time.unscaledDeltaTime * speed)
		{
			alphaPercentage = Mathf.Min(Mathf.Max(1f - i / duration, targetBlackPercent), GameScreenManager.Instance.fadePlane.color.a);
			GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, alphaPercentage);
			yield return 0;
		}
		GameScreenManager.Instance.fadePlane.color = new Color(0f, 0f, 0f, targetBlackPercent);
		this.activeFadeRoutine = null;
		yield return 0;
		yield break;
	}

	public void EnableFreeCamera(bool enable)
	{
		this.FreeCameraEnabled = enable;
		this.SetInfoText("Screenshot Mode (ESC to exit)");
	}

	private static bool WithinInputField()
	{
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current == null)
		{
			return false;
		}
		bool flag = false;
		if (current.currentSelectedGameObject != null && (current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null || current.currentSelectedGameObject.GetComponent<InputField>() != null))
		{
			flag = true;
		}
		return flag;
	}

	private void SetInfoText(string text)
	{
		this.infoText.text = text;
		Color color = this.infoText.color;
		color.a = 0.5f;
		this.infoText.color = color;
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (this.DisableUserCameraControl)
		{
			return;
		}
		if (CameraController.WithinInputField())
		{
			return;
		}
		if (SaveGame.Instance != null && SaveGame.Instance.GetComponent<UserNavigation>().Handle(e))
		{
			return;
		}
		if (e.TryConsume(global::Action.ZoomIn))
		{
			float num = this.targetOrthographicSize - this.zoomFactor * this.targetOrthographicSize;
			this.targetOrthographicSize = Mathf.Max(num, this.minOrthographicSize);
			this.overrideZoomSpeed = 0f;
			this.isTargetPosSet = false;
		}
		else if (e.TryConsume(global::Action.ZoomOut))
		{
			float num2 = this.targetOrthographicSize + this.zoomFactor * this.targetOrthographicSize;
			this.targetOrthographicSize = Mathf.Min(num2, (!this.FreeCameraEnabled) ? this.maxOrthographicSize : TuningData<CameraController.Tuning>.Get().maxOrthographicSizeDebug);
			this.overrideZoomSpeed = 0f;
			this.isTargetPosSet = false;
		}
		else if (e.TryConsume(global::Action.MouseMiddle) || e.IsAction(global::Action.MouseRight))
		{
			this.panning = true;
			this.overrideZoomSpeed = 0f;
			this.isTargetPosSet = false;
		}
		else if (this.FreeCameraEnabled && e.TryConsume(global::Action.CinemaCamEnable))
		{
			this.cinemaCamEnabled = !this.cinemaCamEnabled;
			DebugUtil.LogArgs(new object[] { "Cinema Cam Enabled ", this.cinemaCamEnabled });
			this.SetInfoText((!this.cinemaCamEnabled) ? "Cinema Cam Disabled" : "Cinema Cam Enabled");
		}
		else if (this.FreeCameraEnabled && this.cinemaCamEnabled)
		{
			if (e.TryConsume(global::Action.CinemaToggleLock))
			{
				this.cinemaToggleLock = !this.cinemaToggleLock;
				DebugUtil.LogArgs(new object[] { "Cinema Toggle Lock ", this.cinemaToggleLock });
				this.SetInfoText((!this.cinemaToggleLock) ? "Cinema Input Lock OFF" : "Cinema Input Lock ON");
			}
			else if (e.TryConsume(global::Action.CinemaToggleEasing))
			{
				this.cinemaToggleEasing = !this.cinemaToggleEasing;
				DebugUtil.LogArgs(new object[] { "Cinema Toggle Easing ", this.cinemaToggleEasing });
				this.SetInfoText((!this.cinemaToggleEasing) ? "Cinema Easing OFF" : "Cinema Easing ON");
			}
			else if (e.TryConsume(global::Action.CinemaPanLeft))
			{
				this.cinemaPanLeft = !this.cinemaToggleLock || !this.cinemaPanLeft;
				this.cinemaPanRight = false;
			}
			else if (e.TryConsume(global::Action.CinemaPanRight))
			{
				this.cinemaPanRight = !this.cinemaToggleLock || !this.cinemaPanRight;
				this.cinemaPanLeft = false;
			}
			else if (e.TryConsume(global::Action.CinemaPanUp))
			{
				this.cinemaPanUp = !this.cinemaToggleLock || !this.cinemaPanUp;
				this.cinemaPanDown = false;
			}
			else if (e.TryConsume(global::Action.CinemaPanDown))
			{
				this.cinemaPanDown = !this.cinemaToggleLock || !this.cinemaPanDown;
				this.cinemaPanUp = false;
			}
			else if (e.TryConsume(global::Action.CinemaZoomIn))
			{
				this.cinemaZoomIn = !this.cinemaToggleLock || !this.cinemaZoomIn;
				this.cinemaZoomOut = false;
			}
			else if (e.TryConsume(global::Action.CinemaZoomOut))
			{
				this.cinemaZoomOut = !this.cinemaToggleLock || !this.cinemaZoomOut;
				this.cinemaZoomIn = false;
			}
			else if (e.TryConsume(global::Action.CinemaZoomSpeedPlus))
			{
				this.cinemaZoomSpeed++;
				DebugUtil.LogArgs(new object[] { "Cinema Zoom Speed ", this.cinemaZoomSpeed });
				this.SetInfoText("Cinema Zoom Speed: " + this.cinemaZoomSpeed);
			}
			else if (e.TryConsume(global::Action.CinemaZoomSpeedMinus))
			{
				this.cinemaZoomSpeed--;
				DebugUtil.LogArgs(new object[] { "Cinema Zoom Speed ", this.cinemaZoomSpeed });
				this.SetInfoText("Cinema Zoom Speed: " + this.cinemaZoomSpeed);
			}
		}
		else if (e.TryConsume(global::Action.PanLeft))
		{
			this.panLeft = true;
		}
		else if (e.TryConsume(global::Action.PanRight))
		{
			this.panRight = true;
		}
		else if (e.TryConsume(global::Action.PanUp))
		{
			this.panUp = true;
		}
		else if (e.TryConsume(global::Action.PanDown))
		{
			this.panDown = true;
		}
		if (!e.Consumed && OverlayMenu.Instance != null)
		{
			OverlayMenu.Instance.OnKeyDown(e);
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (this.DisableUserCameraControl)
		{
			return;
		}
		if (CameraController.WithinInputField())
		{
			return;
		}
		if (e.TryConsume(global::Action.MouseMiddle) || e.IsAction(global::Action.MouseRight))
		{
			this.panning = false;
		}
		else if (this.FreeCameraEnabled && this.cinemaCamEnabled)
		{
			if (e.TryConsume(global::Action.CinemaPanLeft))
			{
				this.cinemaPanLeft = this.cinemaToggleLock && this.cinemaPanLeft;
			}
			else if (e.TryConsume(global::Action.CinemaPanRight))
			{
				this.cinemaPanRight = this.cinemaToggleLock && this.cinemaPanRight;
			}
			else if (e.TryConsume(global::Action.CinemaPanUp))
			{
				this.cinemaPanUp = this.cinemaToggleLock && this.cinemaPanUp;
			}
			else if (e.TryConsume(global::Action.CinemaPanDown))
			{
				this.cinemaPanDown = this.cinemaToggleLock && this.cinemaPanDown;
			}
			else if (e.TryConsume(global::Action.CinemaZoomIn))
			{
				this.cinemaZoomIn = this.cinemaToggleLock && this.cinemaZoomIn;
			}
			else if (e.TryConsume(global::Action.CinemaZoomOut))
			{
				this.cinemaZoomOut = this.cinemaToggleLock && this.cinemaZoomOut;
			}
		}
		else if (e.TryConsume(global::Action.CameraHome))
		{
			this.CameraGoHome(2f);
		}
		else if (e.TryConsume(global::Action.PanLeft))
		{
			this.panLeft = false;
		}
		else if (e.TryConsume(global::Action.PanRight))
		{
			this.panRight = false;
		}
		else if (e.TryConsume(global::Action.PanUp))
		{
			this.panUp = false;
		}
		else if (e.TryConsume(global::Action.PanDown))
		{
			this.panDown = false;
		}
	}

	public void ForcePanningState(bool state)
	{
		this.panning = false;
	}

	public void CameraGoHome(float speed = 2f)
	{
		GameObject telepad = GameUtil.GetTelepad();
		if (telepad != null)
		{
			Vector3 vector = new Vector3(telepad.transform.GetPosition().x, telepad.transform.GetPosition().y + 1f, base.transform.GetPosition().z);
			this.SetTargetPos(vector, 10f, true);
			this.SetOverrideZoomSpeed(speed);
		}
	}

	public void CameraGoTo(Vector3 pos, float speed = 2f, bool playSound = true)
	{
		pos.z = base.transform.GetPosition().z;
		this.SetTargetPos(pos, 10f, playSound);
		this.SetOverrideZoomSpeed(speed);
	}

	public void SnapTo(Vector3 pos)
	{
		this.ClearFollowTarget();
		pos.z = -100f;
		base.transform.SetPosition(pos);
		this.keyPanDelta = Vector3.zero;
		this.SetOrthographicsSize(this.targetOrthographicSize);
	}

	public void SetOverrideZoomSpeed(float tempZoomSpeed)
	{
		this.overrideZoomSpeed = tempZoomSpeed;
	}

	public void SetTargetPos(Vector3 pos, float orthographic_size, bool playSound)
	{
		this.ClearFollowTarget();
		if (playSound && !this.isTargetPosSet)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Click_Notification", false));
		}
		this.isTargetPosSet = true;
		pos.z = -100f;
		this.targetPos = pos;
		this.targetOrthographicSize = orthographic_size;
	}

	public void SetMaxOrthographicSize(float size)
	{
		this.maxOrthographicSize = size;
	}

	public void SetOrthographicsSize(float size)
	{
		for (int i = 0; i < this.cameras.Count; i++)
		{
			this.cameras[i].orthographicSize = size;
		}
	}

	public void SetPosition(Vector3 pos)
	{
		base.transform.SetPosition(pos);
	}

	private Vector3 PointUnderCursor(Vector3 mousePos, Camera cam)
	{
		Ray ray = cam.ScreenPointToRay(mousePos);
		Vector3 direction = ray.direction;
		Vector3 vector = direction * Mathf.Abs(cam.transform.GetPosition().z / direction.z);
		return ray.origin + vector;
	}

	private void CinemaCamUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Camera main = Camera.main;
		Vector3 localPosition = base.transform.GetLocalPosition();
		float num = Mathf.Pow((float)this.cinemaZoomSpeed, 3f);
		if (this.cinemaZoomIn)
		{
			this.overrideZoomSpeed = -num / TuningData<CameraController.Tuning>.Get().cinemaZoomFactor;
			this.isTargetPosSet = false;
		}
		else if (this.cinemaZoomOut)
		{
			this.overrideZoomSpeed = num / TuningData<CameraController.Tuning>.Get().cinemaZoomFactor;
			this.isTargetPosSet = false;
		}
		else
		{
			this.overrideZoomSpeed = 0f;
		}
		if (this.cinemaToggleEasing)
		{
			this.cinemaZoomVelocity += (this.overrideZoomSpeed - this.cinemaZoomVelocity) * this.cinemaEasing;
		}
		else
		{
			this.cinemaZoomVelocity = this.overrideZoomSpeed;
		}
		if (this.cinemaZoomVelocity != 0f)
		{
			this.SetOrthographicsSize(main.orthographicSize + this.cinemaZoomVelocity * unscaledDeltaTime * (main.orthographicSize / 20f));
			this.targetOrthographicSize = main.orthographicSize;
		}
		float num2 = num / TuningData<CameraController.Tuning>.Get().cinemaZoomToFactor;
		float num3 = this.keyPanningSpeed / 20f * main.orthographicSize;
		float num4 = num3 * (num / TuningData<CameraController.Tuning>.Get().cinemaPanToFactor);
		if (!this.isTargetPosSet && this.targetOrthographicSize != main.orthographicSize)
		{
			float num5 = Mathf.Min(num2 * unscaledDeltaTime, 0.1f);
			this.SetOrthographicsSize(Mathf.Lerp(main.orthographicSize, this.targetOrthographicSize, num5));
		}
		Vector3 vector = Vector3.zero;
		if (this.isTargetPosSet)
		{
			float num6 = this.cinemaEasing * TuningData<CameraController.Tuning>.Get().targetZoomEasingFactor;
			float num7 = this.cinemaEasing * TuningData<CameraController.Tuning>.Get().targetPanEasingFactor;
			float num8 = this.targetOrthographicSize - main.orthographicSize;
			Vector3 vector2 = this.targetPos - localPosition;
			float num9;
			float num10;
			if (!this.cinemaToggleEasing)
			{
				num9 = num2 * unscaledDeltaTime;
				num10 = num4 * unscaledDeltaTime;
			}
			else
			{
				DebugUtil.LogArgs(new object[]
				{
					"Min zoom of:",
					num2 * unscaledDeltaTime,
					Mathf.Abs(num8) * num6 * unscaledDeltaTime
				});
				num9 = Mathf.Min(num2 * unscaledDeltaTime, Mathf.Abs(num8) * num6 * unscaledDeltaTime);
				DebugUtil.LogArgs(new object[]
				{
					"Min pan of:",
					num4 * unscaledDeltaTime,
					vector2.magnitude * num7 * unscaledDeltaTime
				});
				num10 = Mathf.Min(num4 * unscaledDeltaTime, vector2.magnitude * num7 * unscaledDeltaTime);
			}
			float num11;
			if (Mathf.Abs(num8) < num9)
			{
				num11 = num8;
			}
			else
			{
				num11 = Mathf.Sign(num8) * num9;
			}
			if (vector2.magnitude < num10)
			{
				vector = vector2;
			}
			else
			{
				vector = vector2.normalized * num10;
			}
			if (Mathf.Abs(num11) < 0.001f && vector.magnitude < 0.001f)
			{
				this.isTargetPosSet = false;
				num11 = num8;
				vector = vector2;
			}
			this.SetOrthographicsSize(main.orthographicSize + num11 * (main.orthographicSize / 20f));
		}
		if (!PlayerController.Instance.IsDragging())
		{
			this.panning = false;
		}
		Vector3 vector3 = Vector3.zero;
		if (this.panning)
		{
			vector3 = -PlayerController.Instance.GetWorldDragDelta();
			this.isTargetPosSet = false;
			if (vector3.magnitude > 0f)
			{
				this.ClearFollowTarget();
			}
			this.keyPanDelta = Vector3.zero;
		}
		else
		{
			float num12 = num / TuningData<CameraController.Tuning>.Get().cinemaPanFactor;
			Vector3 zero = Vector3.zero;
			if (this.cinemaPanLeft)
			{
				this.ClearFollowTarget();
				zero.x = -num3 * num12;
				this.isTargetPosSet = false;
			}
			if (this.cinemaPanRight)
			{
				this.ClearFollowTarget();
				zero.x = num3 * num12;
				this.isTargetPosSet = false;
			}
			if (this.cinemaPanUp)
			{
				this.ClearFollowTarget();
				zero.y = num3 * num12;
				this.isTargetPosSet = false;
			}
			if (this.cinemaPanDown)
			{
				this.ClearFollowTarget();
				zero.y = -num3 * num12;
				this.isTargetPosSet = false;
			}
			if (this.cinemaToggleEasing)
			{
				this.keyPanDelta += (zero - this.keyPanDelta) * this.cinemaEasing;
			}
			else
			{
				this.keyPanDelta = zero;
			}
		}
		Vector3 vector4 = localPosition + vector + vector3 + this.keyPanDelta * unscaledDeltaTime;
		if (this.followTarget != null)
		{
			vector4.x = this.followTargetPos.x;
			vector4.y = this.followTargetPos.y;
		}
		vector4.z = -100f;
		if ((double)(vector4 - base.transform.GetLocalPosition()).magnitude > 0.001)
		{
			base.transform.SetLocalPosition(vector4);
		}
	}

	private void NormalCamUpdate()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Camera main = Camera.main;
		float num = ((this.overrideZoomSpeed == 0f) ? this.zoomSpeed : this.overrideZoomSpeed);
		Vector3 localPosition = base.transform.GetLocalPosition();
		Vector3 vector = ((this.overrideZoomSpeed == 0f) ? KInputManager.GetMousePos() : new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		Vector3 vector2 = this.PointUnderCursor(vector, main);
		Vector3 vector3 = main.ScreenToViewportPoint(vector);
		float num2 = this.keyPanningSpeed / 20f * main.orthographicSize;
		float num3 = Mathf.Min(num * unscaledDeltaTime, 0.1f);
		this.SetOrthographicsSize(Mathf.Lerp(main.orthographicSize, this.targetOrthographicSize, num3));
		base.transform.SetLocalPosition(localPosition);
		Vector3 vector4 = main.WorldToViewportPoint(vector2);
		vector3.z = vector4.z;
		Vector3 vector5 = main.ViewportToWorldPoint(vector4) - main.ViewportToWorldPoint(vector3);
		if (this.isTargetPosSet)
		{
			vector5 = Vector3.Lerp(localPosition, this.targetPos, num * unscaledDeltaTime) - localPosition;
			if (vector5.magnitude < 0.001f)
			{
				this.isTargetPosSet = false;
				vector5 = this.targetPos - localPosition;
			}
		}
		if (!PlayerController.Instance.IsDragging())
		{
			this.panning = false;
		}
		Vector3 vector6 = Vector3.zero;
		if (this.panning)
		{
			vector6 = -PlayerController.Instance.GetWorldDragDelta();
			this.isTargetPosSet = false;
		}
		Vector3 vector7 = localPosition + vector5 + vector6;
		if (this.panning)
		{
			if (vector6.magnitude > 0f)
			{
				this.ClearFollowTarget();
			}
			this.keyPanDelta = Vector3.zero;
		}
		else if (!this.DisableUserCameraControl)
		{
			if (this.panLeft)
			{
				this.ClearFollowTarget();
				this.keyPanDelta.x = this.keyPanDelta.x - num2;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.panRight)
			{
				this.ClearFollowTarget();
				this.keyPanDelta.x = this.keyPanDelta.x + num2;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.panUp)
			{
				this.ClearFollowTarget();
				this.keyPanDelta.y = this.keyPanDelta.y + num2;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.panDown)
			{
				this.ClearFollowTarget();
				this.keyPanDelta.y = this.keyPanDelta.y - num2;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			Vector3 vector8 = new Vector3(Mathf.Lerp(0f, this.keyPanDelta.x, unscaledDeltaTime * this.keyPanningEasing), Mathf.Lerp(0f, this.keyPanDelta.y, unscaledDeltaTime * this.keyPanningEasing), 0f);
			this.keyPanDelta -= vector8;
			vector7.x += vector8.x;
			vector7.y += vector8.y;
		}
		if (this.followTarget != null)
		{
			vector7.x = this.followTargetPos.x;
			vector7.y = this.followTargetPos.y;
		}
		vector7.z = -100f;
		if ((double)(vector7 - base.transform.GetLocalPosition()).magnitude > 0.001)
		{
			base.transform.SetLocalPosition(vector7);
		}
	}

	private void Update()
	{
		if (!Game.Instance.timelapser.CapturingTimelapseScreenshot)
		{
			if (this.FreeCameraEnabled && this.cinemaCamEnabled)
			{
				this.CinemaCamUpdate();
			}
			else
			{
				this.NormalCamUpdate();
			}
		}
		if (this.infoText.color.a > 0f)
		{
			Color color = this.infoText.color;
			color.a = Mathf.Max(0f, this.infoText.color.a - Time.unscaledDeltaTime * 0.5f);
			this.infoText.color = color;
		}
		this.ConstrainToWorld();
		Vector3 vector = this.PointUnderCursor(KInputManager.GetMousePos(), Camera.main);
		Shader.SetGlobalVector("_WorldCameraPos", new Vector4(base.transform.GetPosition().x, base.transform.GetPosition().y, base.transform.GetPosition().z, Camera.main.orthographicSize));
		Shader.SetGlobalVector("_WorldCursorPos", new Vector4(vector.x, vector.y, 0f, 0f));
		this.VisibleArea.Update();
		this.soundCuller = SoundCuller.CreateCuller();
	}

	private Vector3 GetFollowPos()
	{
		if (this.followTarget != null)
		{
			Vector3 vector = this.followTarget.transform.GetPosition();
			KAnimControllerBase component = this.followTarget.GetComponent<KAnimControllerBase>();
			if (component != null)
			{
				vector = component.GetWorldPivot();
			}
			return vector;
		}
		return Vector3.zero;
	}

	private void ConstrainToWorld()
	{
		if (Game.Instance.IsLoading())
		{
			return;
		}
		if (this.FreeCameraEnabled)
		{
			return;
		}
		Camera main = Camera.main;
		Ray ray = main.ViewportPointToRay(Vector3.zero);
		Ray ray2 = main.ViewportPointToRay(Vector3.one);
		float num = Mathf.Abs(ray.origin.z / ray.direction.z);
		float num2 = Mathf.Abs(ray2.origin.z / ray2.direction.z);
		Vector3 point = ray.GetPoint(num);
		Vector3 point2 = ray2.GetPoint(num2);
		if (point2.x - point.x > Grid.WidthInMeters || point2.y - point.y > Grid.HeightInMeters)
		{
			return;
		}
		Vector3 vector = base.transform.GetPosition() - ray.origin;
		Vector3 vector2 = point;
		vector2.x = Mathf.Max(0f, vector2.x);
		vector2.y = Mathf.Max(0f, vector2.y);
		ray.origin = vector2;
		ray.direction = -ray.direction;
		vector2 = ray.GetPoint(num);
		base.transform.SetPosition(vector2 + vector);
		vector = base.transform.GetPosition() - ray2.origin;
		vector2 = point2;
		vector2.x = Mathf.Min(Grid.WidthInMeters, vector2.x);
		vector2.y = Mathf.Min(Grid.HeightInMeters * this.MAX_Y_SCALE, vector2.y);
		ray2.origin = vector2;
		ray2.direction = -ray2.direction;
		vector2 = ray2.GetPoint(num2);
		Vector3 vector3 = vector2 + vector;
		vector3.z = -100f;
		base.transform.SetPosition(vector3);
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(base.transform.GetPosition());
		writer.Write(base.transform.localScale);
		writer.Write(base.transform.rotation);
		writer.Write(this.targetOrthographicSize);
		CameraSaveData.position = base.transform.GetPosition();
		CameraSaveData.localScale = base.transform.localScale;
		CameraSaveData.rotation = base.transform.rotation;
	}

	private void Restore()
	{
		if (CameraSaveData.valid)
		{
			int num = Grid.PosToCell(CameraSaveData.position);
			if (Grid.IsValidCell(num) && !Grid.IsVisible(num))
			{
				global::Debug.LogWarning("Resetting Camera Position... camera was saved in an undiscovered area of the map.");
				this.CameraGoHome(2f);
			}
			else
			{
				base.transform.SetPosition(CameraSaveData.position);
				base.transform.localScale = CameraSaveData.localScale;
				base.transform.rotation = CameraSaveData.rotation;
				this.targetOrthographicSize = Mathf.Clamp(CameraSaveData.orthographicsSize, this.minOrthographicSize, (!this.FreeCameraEnabled) ? this.maxOrthographicSize : TuningData<CameraController.Tuning>.Get().maxOrthographicSizeDebug);
				this.SnapTo(base.transform.GetPosition());
			}
		}
	}

	private void OnMRTSetupComplete(Camera cam)
	{
		this.cameras.Add(cam);
	}

	public bool IsAudibleSound(Vector2 pos)
	{
		return this.soundCuller.IsAudible(pos);
	}

	public bool IsAudibleSound(Vector3 pos, string sound_path)
	{
		return this.soundCuller.IsAudible(pos, sound_path);
	}

	public Vector3 GetVerticallyScaledPosition(Vector2 pos)
	{
		return this.soundCuller.GetVerticallyScaledPosition(pos);
	}

	public bool IsVisiblePos(Vector3 pos)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		return visibleArea.Min <= pos && pos <= visibleArea.Max;
	}

	protected override void OnCleanUp()
	{
		CameraController.Instance = null;
	}

	public void SetFollowTarget(Transform follow_target)
	{
		this.ClearFollowTarget();
		if (follow_target == null)
		{
			return;
		}
		this.followTarget = follow_target;
		this.SetOrthographicsSize(6f);
		this.targetOrthographicSize = 6f;
		Vector3 followPos = this.GetFollowPos();
		this.followTargetPos = new Vector3(followPos.x, followPos.y, base.transform.GetPosition().z);
		base.transform.SetPosition(this.followTargetPos);
		this.followTarget.GetComponent<KMonoBehaviour>().Trigger(-1506069671, null);
	}

	public void ClearFollowTarget()
	{
		if (this.followTarget == null)
		{
			return;
		}
		this.followTarget.GetComponent<KMonoBehaviour>().Trigger(-485480405, null);
		this.followTarget = null;
	}

	public void UpdateFollowTarget()
	{
		if (this.followTarget != null)
		{
			Vector3 followPos = this.GetFollowPos();
			Vector2 vector = new Vector2(base.transform.GetLocalPosition().x, base.transform.GetLocalPosition().y);
			Vector2 vector2 = Vector2.Lerp(vector, followPos, Time.unscaledDeltaTime * 25f);
			this.followTargetPos = new Vector3(vector2.x, vector2.y, base.transform.GetLocalPosition().z);
		}
	}

	public void RenderForTimelapser(ref RenderTexture tex)
	{
		this.RenderCameraForTimelapse(this.baseCamera, ref tex, this.timelapseCameraCullingMask, -1f);
		CameraClearFlags clearFlags = this.overlayCamera.clearFlags;
		this.overlayCamera.clearFlags = CameraClearFlags.Nothing;
		this.RenderCameraForTimelapse(this.overlayCamera, ref tex, this.timelapseOverlayCameraCullingMask, -1f);
		this.overlayCamera.clearFlags = clearFlags;
	}

	private void RenderCameraForTimelapse(Camera cam, ref RenderTexture tex, LayerMask mask, float overrideAspect = -1f)
	{
		int cullingMask = cam.cullingMask;
		RenderTexture targetTexture = cam.targetTexture;
		cam.targetTexture = tex;
		cam.aspect = (float)tex.width / (float)tex.height;
		if (overrideAspect != -1f)
		{
			cam.aspect = overrideAspect;
		}
		if (mask != -1)
		{
			cam.cullingMask = mask;
		}
		cam.Render();
		cam.ResetAspect();
		cam.cullingMask = cullingMask;
		cam.targetTexture = targetTexture;
	}

	public const float DEFAULT_MAX_ORTHO_SIZE = 20f;

	public float MAX_Y_SCALE = 1.1f;

	public LocText infoText;

	private const float FIXED_Z = -100f;

	public bool FreeCameraEnabled;

	public float zoomSpeed;

	public float minOrthographicSize;

	public float zoomFactor;

	public float keyPanningSpeed;

	public float keyPanningEasing;

	public Texture2D dayColourCube;

	public Texture2D nightColourCube;

	public Material LightBufferMaterial;

	public Material LightCircleOverlay;

	public Material LightConeOverlay;

	public Transform followTarget;

	public Vector3 followTargetPos;

	public GridVisibleArea VisibleArea = new GridVisibleArea();

	private float maxOrthographicSize = 20f;

	private float overrideZoomSpeed;

	private bool panning;

	private Vector3 keyPanDelta;

	[SerializeField]
	private LayerMask timelapseCameraCullingMask;

	[SerializeField]
	private LayerMask timelapseOverlayCameraCullingMask;

	private bool userCameraControlDisabled;

	private bool panLeft;

	private bool panRight;

	private bool panUp;

	private bool panDown;

	[NonSerialized]
	public Camera baseCamera;

	[NonSerialized]
	public Camera overlayCamera;

	[NonSerialized]
	public Camera overlayNoDepthCamera;

	[NonSerialized]
	public Camera uiCamera;

	[NonSerialized]
	public Camera lightBufferCamera;

	[NonSerialized]
	public Camera simOverlayCamera;

	[NonSerialized]
	public Camera infraredCamera;

	[NonSerialized]
	public Camera timelapseFreezeCamera;

	public List<Camera> cameras = new List<Camera>();

	private MultipleRenderTarget mrt;

	public SoundCuller soundCuller;

	private bool cinemaCamEnabled;

	private bool cinemaToggleLock;

	private bool cinemaToggleEasing;

	private bool cinemaPanLeft;

	private bool cinemaPanRight;

	private bool cinemaPanUp;

	private bool cinemaPanDown;

	private bool cinemaZoomIn;

	private bool cinemaZoomOut;

	private int cinemaZoomSpeed = 10;

	private float cinemaEasing = 0.05f;

	private float cinemaZoomVelocity;

	private Coroutine activeFadeRoutine;

	public class Tuning : TuningData<CameraController.Tuning>
	{
		public float maxOrthographicSizeDebug;

		public float cinemaZoomFactor = 100f;

		public float cinemaPanFactor = 50f;

		public float cinemaZoomToFactor = 100f;

		public float cinemaPanToFactor = 50f;

		public float targetZoomEasingFactor = 400f;

		public float targetPanEasingFactor = 100f;
	}
}
