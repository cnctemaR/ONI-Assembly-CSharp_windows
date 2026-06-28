using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraController : KMonoBehaviour, IInputHandler
{
	public KInputHandler inputHandler { get; set; }

	private float zoomScaledKeyPanningSpeed
	{
		get
		{
			return this.keyPanningSpeed / 20f * this.targetOrthographicSize;
		}
	}

	public bool DisableUserCameraControl { get; set; }

	public static CameraController Instance { get; private set; }

	public void ToggleColouredOverlayView(bool enabled)
	{
		this.mrt.ToggleColouredOverlayView(enabled);
	}

	protected override void OnPrefabInit()
	{
		global::Util.Reset(this.transform);
		this.transform.localPosition = new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, this.defaultDepth);
		this.targetOrthographicSize = this.maxOrthographicSize;
		CameraController.Instance = this;
		this.DisableUserCameraControl = false;
		this.baseCamera = this.CopyCamera(Camera.main, "baseCamera");
		this.mrt = this.baseCamera.gameObject.AddComponent<MultipleRenderTarget>();
		this.mrt.onSetupComplete += this.OnMRTSetupComplete;
		this.baseCamera.gameObject.AddComponent<GasEffect>().Material = this.GasMaterial;
		this.baseCamera.gameObject.AddComponent<LightBufferCompositor>();
		this.baseCamera.transparencySortMode = TransparencySortMode.Orthographic;
		this.baseCamera.transform.parent = this.transform;
		global::Util.Reset(this.baseCamera.transform);
		int mask = LayerMask.GetMask(new string[] { "PlaceWithDepth", "Overlay" });
		int mask2 = LayerMask.GetMask(new string[] { "Water" });
		int mask3 = LayerMask.GetMask(new string[] { "Construction" });
		this.cameras.Add(this.baseCamera);
		this.baseCamera.cullingMask &= ~mask;
		this.baseCamera.cullingMask &= ~mask2;
		this.baseCamera.cullingMask |= mask3;
		this.baseCamera.tag = "Untagged";
		this.baseCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_LitTex";
		this.infraredCamera = this.CopyCamera(this.baseCamera, "Infrared");
		this.infraredCamera.cullingMask = 0;
		this.infraredCamera.clearFlags = CameraClearFlags.Color;
		this.infraredCamera.depth = this.baseCamera.depth - 1f;
		this.infraredCamera.transform.parent = this.transform;
		this.infraredCamera.gameObject.AddComponent<Infrared>();
		this.simOverlayCamera = this.CopyCamera(this.baseCamera, "SimOverlayCamera");
		this.simOverlayCamera.cullingMask = LayerMask.GetMask(new string[] { "SimDebugView" });
		this.simOverlayCamera.clearFlags = CameraClearFlags.Color;
		this.simOverlayCamera.depth = this.baseCamera.depth + 1f;
		this.simOverlayCamera.transform.parent = this.transform;
		this.simOverlayCamera.gameObject.AddComponent<CameraRenderTexture>().TextureName = "_SimDebugViewTex";
		this.waterCamera = this.CopyCamera(this.baseCamera, "Water");
		this.waterCamera.cullingMask = mask2;
		this.waterCamera.clearFlags = CameraClearFlags.Depth;
		this.waterCamera.depth = this.baseCamera.depth + 2f;
		this.waterCamera.transform.parent = this.transform;
		CameraReferenceTexture cameraReferenceTexture = this.waterCamera.gameObject.AddComponent<CameraReferenceTexture>();
		cameraReferenceTexture.referenceCamera = this.baseCamera;
		this.overlayCamera = Camera.main;
		this.overlayCamera.name = "Overlay";
		this.overlayCamera.cullingMask = mask | mask3;
		this.overlayCamera.clearFlags = CameraClearFlags.Nothing;
		this.overlayCamera.transform.parent = this.transform;
		this.overlayCamera.depth = this.baseCamera.depth + 3f;
		this.overlayCamera.transform.localPosition = Vector3.zero;
		this.overlayCamera.transform.localRotation = Quaternion.identity;
		this.overlayCamera.renderingPath = RenderingPath.Forward;
		this.overlayCamera.hdr = false;
		this.overlayCamera.tag = "Untagged";
		ColorCorrectionLookup component = this.overlayCamera.GetComponent<ColorCorrectionLookup>();
		component.Convert(this.dayColourCube, string.Empty);
		component.Convert2(this.nightColourCube, string.Empty);
		this.cameras.Add(this.overlayCamera);
		this.lightBufferCamera = this.CopyCamera(this.overlayCamera, "Light Buffer");
		this.lightBufferCamera.clearFlags = CameraClearFlags.Color;
		this.lightBufferCamera.cullingMask = LayerMask.GetMask(new string[] { "Lights" });
		this.lightBufferCamera.depth = this.baseCamera.depth - 1f;
		this.lightBufferCamera.transform.parent = this.transform;
		this.lightBufferCamera.transform.localPosition = Vector3.zero;
		this.lightBufferCamera.rect = new Rect(0f, 0f, 1f, 1f);
		LightBuffer lightBuffer = this.lightBufferCamera.gameObject.AddComponent<LightBuffer>();
		lightBuffer.Material = this.LightBufferMaterial;
		lightBuffer.CircleMaterial = this.LightCircleOverlay;
		lightBuffer.ConeMaterial = this.LightConeOverlay;
		this.overlayNoDepthCamera = this.CopyCamera(this.overlayCamera, "overlayNoDepth");
		int mask4 = LayerMask.GetMask(new string[] { "Overlay", "Place" });
		this.baseCamera.cullingMask &= ~mask4;
		this.overlayNoDepthCamera.clearFlags = CameraClearFlags.Depth;
		this.overlayNoDepthCamera.cullingMask = mask4;
		this.overlayNoDepthCamera.transform.parent = this.transform;
		this.overlayNoDepthCamera.transform.localPosition = Vector3.zero;
		this.overlayNoDepthCamera.depth = this.baseCamera.depth + 4f;
		this.overlayNoDepthCamera.tag = "MainCamera";
		this.overlayNoDepthCamera.gameObject.AddComponent<NavPathDrawer>();
		this.uiCamera = this.CopyCamera(this.overlayCamera, "uiCamera");
		this.uiCamera.clearFlags = CameraClearFlags.Depth;
		this.uiCamera.cullingMask = LayerMask.GetMask(new string[] { "UI" });
		this.uiCamera.transform.parent = this.transform;
		this.uiCamera.transform.localPosition = Vector3.zero;
		this.uiCamera.depth = this.baseCamera.depth + 5f;
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.ScreenSpaceCamera, this.uiCamera);
		GameScreenManager.Instance.SetCamera(GameScreenManager.UIRenderTarget.WorldSpace, this.uiCamera);
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
		if (e.TryConsume(global::Action.ZoomIn))
		{
			float num = this.targetOrthographicSize - this.zoomFactor * this.targetOrthographicSize;
			this.targetOrthographicSize = Mathf.Max(num, this.minOrthographicSize);
			this.overrideZoomSpeed = 0f;
		}
		else if (e.TryConsume(global::Action.ZoomOut))
		{
			float num2 = this.targetOrthographicSize + this.zoomFactor * this.targetOrthographicSize;
			this.targetOrthographicSize = Mathf.Min(num2, (!DebugHandler.FreeCameraMode) ? this.maxOrthographicSize : this.maxOrthographicSizeDebug);
			this.overrideZoomSpeed = 0f;
		}
		else if (e.TryConsume(global::Action.MouseMiddle) || e.IsAction(global::Action.MouseRight))
		{
			this.panning = true;
			this.overrideZoomSpeed = 0f;
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (this.DisableUserCameraControl)
		{
			return;
		}
		if (e.TryConsume(global::Action.MouseMiddle) || e.IsAction(global::Action.MouseRight))
		{
			this.panning = false;
		}
		else if (e.TryConsume(global::Action.CameraHome))
		{
			this.CameraGoHome(2f);
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
			Vector3 vector = new Vector3(telepad.transform.position.x, telepad.transform.position.y + 1f, this.transform.position.z);
			this.SetTargetPos(vector, 10f, true);
			this.SetOverrideZoomSpeed(speed);
		}
	}

	public void CameraGoTo(Vector3 pos, float speed = 2f, bool playSound = true)
	{
		pos.z = this.transform.position.z;
		this.SetTargetPos(pos, 10f, playSound);
		this.SetOverrideZoomSpeed(speed);
	}

	public void SnapTo(Vector3 pos)
	{
		this.ClearFollowTarget();
		this.transform.SetPosition(pos);
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
		if (playSound && pos != this.targetPos)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Click_Notification", false));
		}
		this.isTargetPosSet = true;
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

	private Vector3 PointUnderCursor(Vector3 mousePos, Camera cam)
	{
		Ray ray = cam.ScreenPointToRay(mousePos);
		Vector3 direction = ray.direction;
		Vector3 vector = direction * Mathf.Abs(cam.transform.position.z / direction.z);
		return ray.origin + vector;
	}

	private void Update()
	{
		float num = ((this.overrideZoomSpeed <= 0f) ? this.zoomSpeed : this.overrideZoomSpeed);
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		Camera main = Camera.main;
		Vector3 vector = ((this.overrideZoomSpeed <= 0f) ? Input.mousePosition : new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		Vector3 vector2 = this.PointUnderCursor(vector, main);
		Vector3 vector3 = main.ScreenToViewportPoint(vector);
		Vector3 localPosition = this.transform.localPosition;
		this.SetOrthographicsSize(Mathf.Lerp(main.orthographicSize, this.targetOrthographicSize, num * unscaledDeltaTime));
		this.transform.localPosition = localPosition;
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
			if (this.inputHandler.IsActive(global::Action.PanLeft))
			{
				this.ClearFollowTarget();
				this.keyPanDelta.x = this.keyPanDelta.x - this.zoomScaledKeyPanningSpeed;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.inputHandler.IsActive(global::Action.PanRight))
			{
				this.ClearFollowTarget();
				this.keyPanDelta.x = this.keyPanDelta.x + this.zoomScaledKeyPanningSpeed;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.inputHandler.IsActive(global::Action.PanUp))
			{
				this.ClearFollowTarget();
				this.keyPanDelta.y = this.keyPanDelta.y + this.zoomScaledKeyPanningSpeed;
				this.isTargetPosSet = false;
				this.overrideZoomSpeed = 0f;
			}
			if (this.inputHandler.IsActive(global::Action.PanDown))
			{
				this.ClearFollowTarget();
				this.keyPanDelta.y = this.keyPanDelta.y - this.zoomScaledKeyPanningSpeed;
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
		if ((double)(vector7 - this.transform.localPosition).magnitude > 0.001)
		{
			this.transform.localPosition = vector7;
		}
		this.ConstrainToWorld();
		Shader.SetGlobalVector("_WorldCameraPos", new Vector4(this.transform.position.x, this.transform.position.y, this.transform.position.z, main.orthographicSize));
		this.VisibleArea.Update();
	}

	private Vector3 GetFollowPos()
	{
		if (this.followTarget != null)
		{
			Vector3 vector = this.followTarget.transform.position;
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
		Vector3 vector = this.transform.position - ray.origin;
		Vector3 vector2 = point;
		vector2.x = Mathf.Max(0f, vector2.x);
		vector2.y = Mathf.Max(0f, vector2.y);
		ray.origin = vector2;
		ray.direction = -ray.direction;
		vector2 = ray.GetPoint(num);
		this.transform.SetPosition(vector2 + vector);
		vector = this.transform.position - ray2.origin;
		vector2 = point2;
		vector2.x = Mathf.Min(Grid.WidthInMeters, vector2.x);
		vector2.y = Mathf.Min(Grid.HeightInMeters, vector2.y);
		ray2.origin = vector2;
		ray2.direction = -ray2.direction;
		vector2 = ray2.GetPoint(num2);
		this.transform.SetPosition(vector2 + vector);
	}

	public void Save(BinaryWriter writer)
	{
		writer.Write(this.transform.position);
		writer.Write(this.transform.localScale);
		writer.Write(this.transform.rotation);
		writer.Write(this.targetOrthographicSize);
		CameraSaveData.position = this.transform.position;
		CameraSaveData.localScale = this.transform.localScale;
		CameraSaveData.rotation = this.transform.rotation;
	}

	private void Restore()
	{
		if (CameraSaveData.valid)
		{
			this.transform.SetPosition(CameraSaveData.position);
			this.transform.localScale = CameraSaveData.localScale;
			this.transform.rotation = CameraSaveData.rotation;
			this.targetOrthographicSize = Mathf.Clamp(CameraSaveData.orthographicsSize, this.minOrthographicSize, (!DebugHandler.FreeCameraMode) ? this.maxOrthographicSize : this.maxOrthographicSizeDebug);
			this.SnapTo(this.transform.position);
		}
	}

	private void OnMRTSetupComplete(Camera cam)
	{
		this.cameras.Add(cam);
	}

	public bool IsAudibleSound(Vector3 pos, float modifiedAudibleDistanceScale = 0f)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2 vector = visibleArea.Max + visibleArea.Min;
		vector *= 0.5f;
		Vector2 vector2 = visibleArea.Max - visibleArea.Min;
		float num = this.maxAudibleDistanceScale;
		if (modifiedAudibleDistanceScale != 0f)
		{
			num = modifiedAudibleDistanceScale;
		}
		vector2 *= num;
		Vector2 vector3 = vector - vector2 * 0.5f;
		Vector2 vector4 = vector + vector2 * 0.5f;
		Vector2 vector5 = new Vector2(pos.x, pos.y);
		return vector3.LessEqual(vector5) && vector5.LessEqual(vector4);
	}

	public bool IsAudibleSound(Vector3 pos, string soundPath)
	{
		bool flag = false;
		if (soundPath != null && soundPath.Length > 0)
		{
			try
			{
				EventDescription soundEventDescription = GlobalAssets.GetSoundEventDescription(soundPath);
				if (soundEventDescription != null)
				{
					float num;
					soundEventDescription.getMaximumDistance(out num);
					num *= this.maxAudibleDistanceScale;
					float num2 = (pos.x - this.transform.position.x) * (pos.x - this.transform.position.x) + (pos.y - this.transform.position.y) * (pos.y - this.transform.position.y);
					flag = num2 < num * num;
				}
			}
			catch
			{
				Output.LogError(new object[] { "IsAudibleSound could not find event description for [" + ((soundPath == null) ? "null" : soundPath) + "]" });
			}
		}
		return flag;
	}

	public Vector3 GetVerticallyScaledPosition(Vector3 pos)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		bool flag = false;
		float num;
		if (pos.y > (float)visibleArea.Max.y)
		{
			num = Mathf.Abs(pos.y - (float)visibleArea.Max.y);
			flag = true;
		}
		else if (pos.y < (float)visibleArea.Min.y)
		{
			num = Mathf.Abs(pos.y - (float)visibleArea.Min.y);
			flag = false;
		}
		else
		{
			num = 0f;
		}
		Audio audio = Audio.Get();
		float orthographicSize = this.cameras[0].orthographicSize;
		float num2 = orthographicSize / (audio.listenerReferenceZ - audio.listenerMinZ);
		if (num2 <= 0f)
		{
			num2 = 2f;
		}
		else
		{
			num2 = 1f;
		}
		num = ((num >= 20f) ? 0f : num);
		float num3 = num * num / (4f * num2);
		if (num > 0f && !flag)
		{
			num3 *= -1f;
		}
		Vector3 vector = new Vector3(pos.x, pos.y + num3, pos.z);
		return vector;
	}

	public float GetZoom0To1()
	{
		float orthographicSize = this.cameras[0].orthographicSize;
		return Mathf.Clamp01((orthographicSize - this.minOrthographicSize) / (this.maxOrthographicSize - this.minOrthographicSize));
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
		this.followTargetPos = new Vector3(followPos.x, followPos.y, this.transform.position.z);
		this.transform.position = this.followTargetPos;
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
			Vector2 vector = new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
			Vector2 vector2 = Vector2.Lerp(vector, followPos, Time.unscaledDeltaTime * 25f);
			this.followTargetPos = new Vector3(vector2.x, vector2.y, this.transform.localPosition.z);
		}
	}

	public const float DEFAULT_MAX_ORTHO_SIZE = 20f;

	public float defaultDepth;

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

	public Material GasMaterial;

	public Transform followTarget;

	public Vector3 followTargetPos;

	public GridVisibleArea VisibleArea = new GridVisibleArea();

	[SerializeField]
	private float maxAudibleDistanceScale = 1.5f;

	private float targetOrthographicSize;

	private float maxOrthographicSize = 20f;

	private float maxOrthographicSizeDebug = 200f;

	private float overrideZoomSpeed;

	private bool panning;

	private Vector3 keyPanDelta;

	private bool isTargetPosSet;

	private Vector3 targetPos;

	[NonSerialized]
	public Camera baseCamera;

	[NonSerialized]
	public Camera waterCamera;

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

	private List<Camera> cameras = new List<Camera>();

	private MultipleRenderTarget mrt;
}
