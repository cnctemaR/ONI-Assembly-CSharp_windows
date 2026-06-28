using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class ConduitFlowVisualizer
{
	public ConduitFlowVisualizer(ConduitFlow flow_manager, GameObject visualizer_prefab, Color32 tint, Color32 insulated_tint, string overlay_sound)
	{
		this.flowManager = flow_manager;
		this.visualizerPrefab = visualizer_prefab;
		this.tint = tint;
		this.insulatedTint = insulated_tint;
		this.overlaySound = overlay_sound;
		this.visualizerPrefab.SetActive(false);
		this.visualizerPool = new ObjectPool(new Func<GameObject>(this.InstantiateVisualizer), 32);
	}

	public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I vector2I = new Vector2I(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
		Vector2I vector2I2 = new Vector2I(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Max(Grid.HeightInCells - 1, visibleArea.Min.y + 1));
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
		IEnumerator<ConduitFlow.Conduit> enumerator = this.flowManager.VisibleConduitsEnumerator(vector2I, vector2I2);
		while (enumerator.MoveNext())
		{
			ConduitFlow.Conduit conduit = enumerator.Current;
			int num = conduit.cell % Grid.WidthInCells;
			int num2 = conduit.cell / Grid.WidthInCells;
			if (conduit.lastFlowDirection != ConduitFlow.FlowDirection.None && conduit.lastFlowElement != SimHashes.Vacuum)
			{
				this.liveAnimatedCells.Add(conduit.cell);
				GameObject gameObject;
				if (this.staticVisualizers.TryGetValue(conduit.cell, out gameObject))
				{
					gameObject.SetActive(false);
					this.visualizerPool.ReleaseInstance(gameObject);
					this.staticVisualizers.Remove(conduit.cell);
				}
				int cellFromDirection = ConduitFlow.GetCellFromDirection(conduit.cell, conduit.lastFlowDirection);
				Quaternion quaternion;
				switch (conduit.lastFlowDirection)
				{
				case ConduitFlow.FlowDirection.Left:
				case ConduitFlow.FlowDirection.Right:
					goto IL_0205;
				case ConduitFlow.FlowDirection.Up:
				case ConduitFlow.FlowDirection.Down:
					quaternion = ConduitFlowVisualizer.VerticalRotation;
					break;
				default:
					goto IL_0205;
				}
				IL_0211:
				Vector2I vector2I3 = Grid.CellToXY(cellFromDirection);
				Vector2 vector = new Vector2((float)num, (float)num2);
				if (cellFromDirection != -1)
				{
					vector = Vector2.Lerp(new Vector2((float)vector2I3.x, (float)vector2I3.y), new Vector2((float)num, (float)num2), lerp_percent);
				}
				vector += ConduitFlowVisualizer.offset;
				float num3 = ((!this.insulatedCells.Contains(conduit.cell)) ? 0f : 1f);
				float num4 = ((!this.insulatedCells.Contains(cellFromDirection)) ? 0f : 1f);
				float num5 = Mathf.Lerp(num4, num3, lerp_percent);
				if (this.visualizers.TryGetValue(conduit.cell, out gameObject))
				{
					gameObject.transform.position = new Vector3(vector.x, vector.y, z);
					Element element = ElementLoader.FindElementByHash(conduit.lastFlowElement);
					this.Colourize(gameObject, element, num5);
				}
				else
				{
					Element element2 = ElementLoader.FindElementByHash(conduit.lastFlowElement);
					gameObject = this.AddAnimatedVisualNode(element2, new Vector3(vector.x, vector.y, z), num5);
					this.visualizers[conduit.cell] = gameObject;
				}
				gameObject.transform.rotation = quaternion;
				if (trigger_audio)
				{
					this.AddAudioSource(conduit, position);
				}
				continue;
				IL_0205:
				quaternion = Quaternion.identity;
				goto IL_0211;
			}
			ConduitFlow.ConduitContents contents = conduit.GetContents();
			if (conduit.initialElement != SimHashes.Vacuum && contents.element != SimHashes.Vacuum && contents.mass > 0f)
			{
				this.liveStaticCells.Add(conduit.cell);
				Vector3 vector2 = new Vector3((float)num + ConduitFlowVisualizer.offset.x, (float)num2 + ConduitFlowVisualizer.offset.y, z);
				float num6 = ((!this.insulatedCells.Contains(conduit.cell)) ? 0f : 1f);
				GameObject gameObject2;
				if (this.staticVisualizers.TryGetValue(conduit.cell, out gameObject2))
				{
					gameObject2.transform.position = vector2;
					Element element3 = ElementLoader.FindElementByHash(contents.element);
					this.Colourize(gameObject2, element3, num6);
				}
				else
				{
					Element element4 = ElementLoader.FindElementByHash(contents.element);
					gameObject2 = this.AddStaticVisualNode(element4, vector2, num6);
					this.staticVisualizers[conduit.cell] = gameObject2;
				}
			}
		}
		this.ReleaseDeadVisualizers(this.visualizers, this.liveAnimatedCells);
		this.ReleaseDeadVisualizers(this.staticVisualizers, this.liveStaticCells);
		if (trigger_audio)
		{
			this.TriggerAudio();
		}
	}

	private void ReleaseDeadVisualizers(Dictionary<int, GameObject> vis_list, HashSet<int> live_list)
	{
		foreach (int num in vis_list.Keys)
		{
			if (!live_list.Contains(num))
			{
				GameObject gameObject = vis_list[num];
				gameObject.SetActive(false);
				this.visualizerPool.ReleaseInstance(gameObject);
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

	private KAnimControllerBase Colourize(GameObject go, Element elem, float insulation_lerp)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.SetLayer(this.layer);
		if (this.showContents)
		{
			Color32 color = Color.white;
			if (elem != null && elem.substance != null)
			{
				color = elem.substance.overlayColour;
				color.a = 128;
			}
			component.TintColour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			component.SetSymbolTint(KBatchedAnimController.SymbolTintIndex.Second, ConduitFlowVisualizer.TintSymbol, color);
		}
		else
		{
			component.TintColour = Color32.Lerp(this.tint, this.insulatedTint, insulation_lerp);
			int num = Grid.PosToCell(go.transform.position);
			Color32 color2 = new Color32(0, 0, 0, 0);
			if (num == this.highlightedCell)
			{
				color2 = this.highlightColour;
			}
			component.HighlightColour = color2;
		}
		component.destroyOnAnimComplete = false;
		component.HideSymbols(!this.showContents, ConduitFlowVisualizer.BGSymbols);
		return component;
	}

	private KAnimControllerBase AddVisualNode(Element elem, Vector3 pos, float insulation_lerp)
	{
		GameObject instance = this.visualizerPool.GetInstance();
		Transform transform = instance.transform;
		transform.position = pos;
		transform.rotation = Quaternion.identity;
		this.ResetNode(instance, this.layer);
		instance.SetActive(true);
		return this.Colourize(instance, elem, insulation_lerp);
	}

	private GameObject AddAnimatedVisualNode(Element elem, Vector3 pos, float insulation_lerp)
	{
		KAnimControllerBase kanimControllerBase = this.AddVisualNode(elem, pos, insulation_lerp);
		kanimControllerBase.Play("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
		return kanimControllerBase.gameObject;
	}

	private GameObject AddStaticVisualNode(Element elem, Vector3 pos, float insulation_lerp)
	{
		KAnimControllerBase kanimControllerBase = this.AddVisualNode(elem, pos, insulation_lerp);
		kanimControllerBase.Play("working_loop", KAnim.PlayMode.Once, 1f, 0f);
		return kanimControllerBase.gameObject;
	}

	private void AddAudioSource(ConduitFlow.Conduit conduit, Vector3 camera_pos)
	{
		UtilityNetwork network = this.flowManager.GetNetwork(conduit);
		if (network == null)
		{
			return;
		}
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

	private void TriggerAudio()
	{
		if (SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
		for (int i = 0; i < this.audioInfo.Count; i++)
		{
			ConduitFlowVisualizer.AudioInfo audioInfo = this.audioInfo[i];
			if (audioInfo.distance != float.PositiveInfinity)
			{
				EventInstance eventInstance = SoundEvent.BeginOneShot(this.overlaySound, audioInfo.position);
				eventInstance.setParameterValue("blobCount", (float)audioInfo.blobCount);
				SoundEvent.EndOneShot(eventInstance);
			}
		}
	}

	private GameObject InstantiateVisualizer()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.visualizerPrefab, Grid.SceneLayer.BuildingFront, Folder.FX, null, 0);
		gameObject.SetActive(false);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.destroyOnAnimComplete = true;
		return gameObject;
	}

	public void ColourizePipeContents(bool show_contents)
	{
		this.showContents = show_contents;
		this.layer = ((!show_contents) ? 0 : LayerMask.NameToLayer("MaskedOverlay"));
		foreach (GameObject gameObject in this.visualizers.Values)
		{
			this.ResetNode(gameObject, this.layer);
		}
		foreach (GameObject gameObject2 in this.staticVisualizers.Values)
		{
			this.ResetNode(gameObject2, this.layer);
		}
	}

	public void ResetNode(GameObject go, int layer)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.SetLayer(layer);
		component.HideSymbols(!this.showContents, ConduitFlowVisualizer.BGSymbols);
		if (!this.showContents)
		{
			component.UnsetSymbolTint(KBatchedAnimController.SymbolTintIndex.First);
			component.UnsetSymbolTint(KBatchedAnimController.SymbolTintIndex.Second);
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

	private GameObject visualizerPrefab;

	private string overlaySound;

	private static readonly Quaternion VerticalRotation = Quaternion.AngleAxis(-90f, Vector3.forward);

	private static readonly KAnimHashedString[] BGSymbols = new KAnimHashedString[]
	{
		new KAnimHashedString("base_BG")
	};

	private static readonly HashedString TintSymbol = new HashedString("base");

	private bool showContents;

	private int layer;

	private static readonly Vector2 offset = new Vector2(0.5f, 0.5f);

	private List<ConduitFlowVisualizer.AudioInfo> audioInfo;

	private Dictionary<int, GameObject> visualizers = new Dictionary<int, GameObject>();

	private Dictionary<int, GameObject> staticVisualizers = new Dictionary<int, GameObject>();

	private HashSet<int> liveStaticCells = new HashSet<int>();

	private HashSet<int> liveAnimatedCells = new HashSet<int>();

	private HashSet<int> insulatedCells = new HashSet<int>();

	private List<int> removedCells = new List<int>();

	private Color32 tint = Color.white;

	private Color32 insulatedTint = Color.white;

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
