using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class ConduitFlowVisualizer
{
	public ConduitFlowVisualizer(ConduitFlow flow_manager, Game.ConduitVisInfo vis_info, string overlay_sound)
	{
		this.flowManager = flow_manager;
		this.visInfo = vis_info;
		this.overlaySound = overlay_sound;
		this.visInfo.prefab.SetActive(true);
		this.visualizerPool = new ObjectPool(new Func<GameObject>(this.InstantiateVisualizer), 32);
	}

	public void FreeResources()
	{
		this.visualizers.Clear();
		this.staticVisualizers.Clear();
		this.visualizerPool.Destroy();
	}

	private float CalculateMassScale(float mass)
	{
		float num = 1f;
		if (!this.showContents || this.moveToOverlayLayer)
		{
			float num2 = (mass - this.visInfo.overlayMassScaleRange.x) / (this.visInfo.overlayMassScaleRange.y - this.visInfo.overlayMassScaleRange.x);
			num = Mathf.Lerp(this.visInfo.overlayMassScaleValues.x, this.visInfo.overlayMassScaleValues.y, num2);
		}
		return num;
	}

	public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I vector2I = new Vector2I(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
		Vector2I vector2I2 = new Vector2I(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
		if (trigger_audio)
		{
			if (this.audioInfo == null)
			{
				this.audioInfo = new List<ConduitFlowVisualizer.AudioInfo>();
			}
			for (int i = 0; i < this.audioInfo.Count; i++)
			{
				ConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[i];
				audioInfo.distance = float.PositiveInfinity;
				audioInfo.position = Vector3.zero;
				audioInfo.blobCount = (audioInfo.blobCount + 1) % 10;
				this.audioInfo[i] = audioInfo;
			}
		}
		Vector3 position = CameraController.Instance.transform.position;
		Element element = null;
		IEnumerator<ConduitFlow.Conduit> enumerator = this.flowManager.VisibleConduitsEnumerator(vector2I, vector2I2);
		while (enumerator.MoveNext())
		{
			ConduitFlow.Conduit conduit = enumerator.Current;
			int cell = conduit.cell;
			int cellFromDirection = ConduitFlow.GetCellFromDirection(conduit.cell, conduit.lastFlowDirection);
			if (conduit.lastFlowContents.mass > 0f)
			{
				this.liveAnimatedCells.Add(conduit.cell);
				Quaternion quaternion;
				switch (conduit.lastFlowDirection)
				{
				case ConduitFlow.FlowDirection.Left:
				case ConduitFlow.FlowDirection.Right:
					goto IL_01BA;
				case ConduitFlow.FlowDirection.Up:
				case ConduitFlow.FlowDirection.Down:
					quaternion = ConduitFlowVisualizer.VerticalRotation;
					break;
				default:
					goto IL_01BA;
				}
				IL_01C6:
				Vector2I vector2I3 = Grid.CellToXY(cell);
				Vector2I vector2I4 = Grid.CellToXY(cellFromDirection);
				Vector2 vector = vector2I3;
				if (cell != -1)
				{
					vector = Vector2.Lerp(new Vector2((float)vector2I3.x, (float)vector2I3.y), new Vector2((float)vector2I4.x, (float)vector2I4.y), lerp_percent);
				}
				vector += ConduitFlowVisualizer.offset;
				float num = ((!this.insulatedCells.Contains(cell)) ? 0f : 1f);
				float num2 = ((!this.insulatedCells.Contains(cellFromDirection)) ? 0f : 1f);
				float num3 = Mathf.Lerp(num, num2, lerp_percent);
				if (element == null || conduit.lastFlowContents.element != element.id)
				{
					element = ElementLoader.FindElementByHash(conduit.lastFlowContents.element);
				}
				KBatchedAnimController kbatchedAnimController;
				if (this.visualizers.TryGetValue(conduit.cell, out kbatchedAnimController))
				{
					kbatchedAnimController.transform.position = new Vector3(vector.x, vector.y, z + -0.1f);
					this.Colourize(kbatchedAnimController, element, num3, conduit.lastFlowContents.diseaseIdx, conduit.lastFlowContents.diseaseCount);
				}
				else
				{
					kbatchedAnimController = this.AddAnimatedVisualNode(element, new Vector3(vector.x, vector.y, z + -0.1f), num3, conduit.lastFlowContents.diseaseIdx, conduit.lastFlowContents.diseaseCount);
					this.visualizers[conduit.cell] = kbatchedAnimController;
				}
				kbatchedAnimController.transform.localScale = Vector3.one;
				kbatchedAnimController.SetSymbolScale(ConduitFlowVisualizer.TintSymbol, 1f);
				float num4 = this.CalculateMassScale(conduit.lastFlowContents.mass);
				if (conduit.lastFlowContents.mass >= conduit.initialContents.mass)
				{
					kbatchedAnimController.transform.localScale = Vector3.one;
					kbatchedAnimController.SetSymbolScale(ConduitFlowVisualizer.TintSymbol, num4);
				}
				else
				{
					kbatchedAnimController.transform.localScale = new Vector3(num4, num4, num4);
					kbatchedAnimController.HideSymbol(true, ConduitFlowVisualizer.BGSymbols[0]);
				}
				kbatchedAnimController.transform.rotation = quaternion;
				if (trigger_audio)
				{
					this.AddAudioSource(conduit, position);
				}
				goto IL_043B;
				IL_01BA:
				quaternion = Quaternion.identity;
				goto IL_01C6;
			}
			IL_043B:
			if (conduit.initialContents.mass > conduit.lastFlowContents.mass && conduit.initialContents.mass > 0f)
			{
				if (element == null || conduit.initialContents.element != element.id)
				{
					element = ElementLoader.FindElementByHash(conduit.initialContents.element);
				}
				float num5 = conduit.initialContents.mass - conduit.lastFlowContents.mass;
				float num6 = num5 / conduit.initialContents.mass;
				int num7 = (int)(num6 * (float)conduit.initialContents.diseaseCount);
				this.RenderStaticBlob(conduit.cell, element, num5, conduit.initialContents.diseaseIdx, num7, z);
			}
		}
		this.ReleaseDeadVisualizers(this.visualizers, this.liveAnimatedCells);
		this.ReleaseDeadVisualizers(this.staticVisualizers, this.liveStaticCells);
		if (trigger_audio)
		{
			this.TriggerAudio();
		}
	}

	private void RenderStaticBlob(int cell, Element elem, float mass, byte disease_idx, int disease_count, float z)
	{
		this.liveStaticCells.Add(cell);
		float num = ((!this.insulatedCells.Contains(cell)) ? 0f : 1f);
		int num2 = cell % Grid.WidthInCells;
		int num3 = cell / Grid.WidthInCells;
		Vector3 vector = new Vector3((float)num2 + ConduitFlowVisualizer.offset.x, (float)num3 + ConduitFlowVisualizer.offset.y, z);
		KBatchedAnimController kbatchedAnimController;
		if (this.staticVisualizers.TryGetValue(cell, out kbatchedAnimController))
		{
			kbatchedAnimController.transform.position = vector;
			this.Colourize(kbatchedAnimController, elem, num, disease_idx, disease_count);
		}
		else
		{
			kbatchedAnimController = this.AddStaticVisualNode(elem, vector, num, disease_idx, disease_count);
			this.staticVisualizers[cell] = kbatchedAnimController;
		}
		kbatchedAnimController.transform.localScale = new Vector3(1f, 1f, 1f);
		kbatchedAnimController.SetSymbolScale(ConduitFlowVisualizer.TintSymbol, 1f);
		float num4 = this.CalculateMassScale(mass);
		kbatchedAnimController.transform.localScale = new Vector3(num4, num4, num4);
	}

	private void ReleaseDeadVisualizers(Dictionary<int, KBatchedAnimController> vis_list, HashSet<int> live_list)
	{
		foreach (int num in vis_list.Keys)
		{
			if (!live_list.Contains(num))
			{
				KBatchedAnimController kbatchedAnimController = vis_list[num];
				kbatchedAnimController.enabled = false;
				this.visualizerPool.ReleaseInstance(kbatchedAnimController.gameObject);
				this.removedCells.Add(num);
			}
		}
		foreach (int num2 in this.removedCells)
		{
			vis_list.Remove(num2);
		}
		live_list.Clear();
		this.removedCells.Clear();
	}

	private void Colourize(KBatchedAnimController controller, Element elem, float insulation_lerp, byte disease_idx, int disease_count)
	{
		controller.SetLayer(this.layer);
		if (this.showContents)
		{
			controller.TintColour = Color32.Lerp(this.visInfo.overlayTint, this.visInfo.overlayInsulatedTint, insulation_lerp);
			Color32 color = Color.white;
			if (elem != null && elem.substance != null)
			{
				color = elem.substance.overlayColour;
				color.a = 128;
			}
			controller.SetSymbolTint(KBatchedAnimController.SymbolTintIndex.Second, ConduitFlowVisualizer.TintSymbol, color);
			bool flag = !this.moveToOverlayLayer;
			controller.HideSymbol(flag, ConduitFlowVisualizer.BGSymbols[0]);
		}
		else
		{
			controller.TintColour = Color32.Lerp(this.visInfo.tint, this.visInfo.insulatedTint, insulation_lerp);
			int num = Grid.PosToCell(controller.transform.position);
			Color32 color2 = new Color32(0, 0, 0, 0);
			if (num == this.highlightedCell)
			{
				color2 = this.highlightColour;
			}
			controller.HighlightColour = color2;
		}
		controller.destroyOnAnimComplete = false;
		controller.HideSymbols(!this.showContents, ConduitFlowVisualizer.BGSymbols);
		this.UpdateControllerRenderQueueOverride(controller);
	}

	private KBatchedAnimController AddVisualNode(Element elem, Vector3 pos, float insulation_lerp, byte disease_idx, int disease_count)
	{
		GameObject instance = this.visualizerPool.GetInstance();
		Transform transform = instance.transform;
		transform.position = pos;
		transform.rotation = Quaternion.identity;
		KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
		this.ResetNode(component, this.layer);
		component.enabled = true;
		this.Colourize(component, elem, insulation_lerp, disease_idx, disease_count);
		return component;
	}

	private KBatchedAnimController AddAnimatedVisualNode(Element elem, Vector3 pos, float insulation_lerp, byte disease_idx, int disease_count)
	{
		KBatchedAnimController kbatchedAnimController = this.AddVisualNode(elem, pos, insulation_lerp, disease_idx, disease_count);
		kbatchedAnimController.Play("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
		return kbatchedAnimController;
	}

	private KBatchedAnimController AddStaticVisualNode(Element elem, Vector3 pos, float insulation_lerp, byte disease_idx, int disease_count)
	{
		KBatchedAnimController kbatchedAnimController = this.AddVisualNode(elem, pos, insulation_lerp, disease_idx, disease_count);
		kbatchedAnimController.Play("working_loop", KAnim.PlayMode.Once, 1f, 0f);
		return kbatchedAnimController;
	}

	private void AddAudioSource(ConduitFlow.Conduit conduit, Vector3 camera_pos)
	{
		using (new KProfiler.Region("AddAudioSource", null))
		{
			UtilityNetwork network = this.flowManager.GetNetwork(conduit);
			if (network != null)
			{
				Vector3 vector = Grid.CellToPosCCC(conduit.cell, Grid.SceneLayer.Building);
				float num = Vector3.SqrMagnitude(vector - camera_pos);
				bool flag = false;
				for (int i = 0; i < this.audioInfo.Count; i++)
				{
					ConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[i];
					if (audioInfo.networkID == network.id)
					{
						if (num < audioInfo.distance)
						{
							audioInfo.distance = num;
							audioInfo.position = vector;
							this.audioInfo[i] = audioInfo;
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ConduitFlowVisualizer.AudioInfo audioInfo2 = default(ConduitFlowVisualizer.AudioInfo);
					audioInfo2.networkID = network.id;
					audioInfo2.position = vector;
					audioInfo2.distance = num;
					audioInfo2.blobCount = 0;
					this.audioInfo.Add(audioInfo2);
				}
			}
		}
	}

	private void TriggerAudio()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			CameraController instance = CameraController.Instance;
			int num = 0;
			List<ConduitFlowVisualizer.AudioInfo> list = new List<ConduitFlowVisualizer.AudioInfo>();
			for (int i = 0; i < this.audioInfo.Count; i++)
			{
				if (instance.IsVisiblePos(this.audioInfo[i].position))
				{
					list.Add(this.audioInfo[i]);
					num++;
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				ConduitFlowVisualizer.AudioInfo audioInfo = list[j];
				if (audioInfo.distance != float.PositiveInfinity)
				{
					EventInstance eventInstance = SoundEvent.BeginOneShot(this.overlaySound, audioInfo.position);
					eventInstance.setParameterValue("blobCount", (float)audioInfo.blobCount);
					eventInstance.setParameterValue("networkCount", (float)num);
					SoundEvent.EndOneShot(eventInstance);
				}
			}
		}
	}

	private GameObject InstantiateVisualizer()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.visInfo.prefab, Grid.SceneLayer.BuildingFront, Folder.FX, null, 0);
		gameObject.SetActive(true);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.destroyOnAnimComplete = true;
		component.enabled = false;
		return gameObject;
	}

	public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
	{
		this.showContents = show_contents;
		this.moveToOverlayLayer = move_to_overlay_layer;
		this.layer = ((!show_contents || !move_to_overlay_layer) ? 0 : LayerMask.NameToLayer("MaskedOverlay"));
		foreach (KBatchedAnimController kbatchedAnimController in this.visualizers.Values)
		{
			this.ResetNode(kbatchedAnimController, this.layer);
		}
		foreach (KBatchedAnimController kbatchedAnimController2 in this.staticVisualizers.Values)
		{
			this.ResetNode(kbatchedAnimController2, this.layer);
		}
	}

	private void ResetNode(KBatchedAnimController controller, int layer)
	{
		controller.SetLayer(layer);
		controller.HideSymbols(!this.showContents, ConduitFlowVisualizer.BGSymbols);
		if (!this.showContents)
		{
			controller.UnsetSymbolTint(KBatchedAnimController.SymbolTintIndex.First);
			controller.UnsetSymbolTint(KBatchedAnimController.SymbolTintIndex.Second);
			controller.UnsetSymbolScale();
			controller.OverlayColour = new Color32(0, 0, 0, 0);
			controller.transform.localScale = Vector3.one;
		}
		this.UpdateControllerRenderQueueOverride(controller);
	}

	private void UpdateControllerRenderQueueOverride(KBatchedAnimController controller)
	{
		bool flag = this.showContents && !this.moveToOverlayLayer;
		if (flag)
		{
			if (controller.renderQueueOverride != 3800)
			{
				controller.SetRenderQueueOverride(3800);
				controller.enabled = false;
				controller.enabled = true;
			}
		}
		else if (controller.renderQueueOverride >= 0 && controller.renderQueueOverride != controller.originalRenderQueue)
		{
			controller.UnsetRenderQueueOverride();
			controller.enabled = false;
			controller.enabled = true;
		}
	}

	public void SetInsulated(int cell, bool insulated)
	{
		if (insulated)
		{
			this.insulatedCells.Add(cell);
		}
		else
		{
			this.insulatedCells.Remove(cell);
		}
	}

	public void SetHighlightedCell(int cell)
	{
		this.highlightedCell = cell;
	}

	private ConduitFlow flowManager;

	private ObjectPool visualizerPool;

	private string overlaySound;

	private static readonly Quaternion VerticalRotation = Quaternion.AngleAxis(-90f, Vector3.forward);

	private static readonly KAnimHashedString[] BGSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("base_BG")
	};

	private static readonly KAnimHashedString TintSymbol = new KAnimHashedString("base");

	private bool showContents = false;

	private bool moveToOverlayLayer = false;

	private int layer = 0;

	private static readonly Vector2 offset = new Vector2(0.5f, 0.5f);

	private List<ConduitFlowVisualizer.AudioInfo> audioInfo;

	private Dictionary<int, KBatchedAnimController> visualizers = new Dictionary<int, KBatchedAnimController>();

	private Dictionary<int, KBatchedAnimController> staticVisualizers = new Dictionary<int, KBatchedAnimController>();

	private HashSet<int> liveStaticCells = new HashSet<int>();

	private HashSet<int> liveAnimatedCells = new HashSet<int>();

	private HashSet<int> insulatedCells = new HashSet<int>();

	private List<int> removedCells = new List<int>();

	private Game.ConduitVisInfo visInfo;

	private int highlightedCell = -1;

	private Color32 highlightColour = new Color(0.2f, 0.2f, 0.2f, 0.2f);

	private struct AudioInfo
	{
		public int networkID;

		public int blobCount;

		public float distance;

		public Vector3 position;
	}
}
