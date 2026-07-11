using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class ConduitFlowVisualizer
{
	public ConduitFlowVisualizer(ConduitFlow flow_manager, Game.ConduitVisInfo vis_info, string overlay_sound, ConduitFlowVisualizer.Tuning tuning)
	{
		this.flowManager = flow_manager;
		this.visInfo = vis_info;
		this.overlaySound = overlay_sound;
		this.tuning = tuning;
		this.movingBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
		this.staticBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
	}

	public void FreeResources()
	{
		this.movingBallMesh.Cleanup();
		this.staticBallMesh.Cleanup();
	}

	private float CalculateMassScale(float mass)
	{
		float num = (mass - this.visInfo.overlayMassScaleRange.x) / (this.visInfo.overlayMassScaleRange.y - this.visInfo.overlayMassScaleRange.x);
		return Mathf.Lerp(this.visInfo.overlayMassScaleValues.x, this.visInfo.overlayMassScaleValues.y, num);
	}

	private Color32 GetContentsColor(Element element, Color32 default_color)
	{
		if (element != null)
		{
			Color color = element.substance.overlayColour;
			color.a = 128f;
			return color;
		}
		return default_color;
	}

	private Color32 GetTintColour()
	{
		return (!this.showContents) ? this.visInfo.tint : this.visInfo.overlayTint;
	}

	private Color32 GetInsulatedTintColour()
	{
		return (!this.showContents) ? this.visInfo.insulatedTint : this.visInfo.overlayInsulatedTint;
	}

	private Color32 GetRadiantTintColour()
	{
		return (!this.showContents) ? this.visInfo.radiantTint : this.visInfo.overlayRadiantTint;
	}

	private Color32 GetCellTintColour(int cell)
	{
		Color32 color;
		if (this.insulatedCells.Contains(cell))
		{
			color = this.GetInsulatedTintColour();
		}
		else if (this.radiantCells.Contains(cell))
		{
			color = this.GetRadiantTintColour();
		}
		else
		{
			color = this.GetTintColour();
		}
		return color;
	}

	public void Render(float z, int render_layer, float lerp_percent, bool trigger_audio = false)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I vector2I = new Vector2I(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
		Vector2I vector2I2 = new Vector2I(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
		this.animTime += (double)Time.deltaTime;
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
		Vector3 position = CameraController.Instance.transform.GetPosition();
		Element element = null;
		if (this.tuning.renderMesh)
		{
			float num = 0f;
			if (this.showContents)
			{
				num = 1f;
			}
			int num2 = (int)(this.animTime / (1.0 / (double)this.tuning.framesPerSecond)) % (int)this.tuning.spriteCount;
			float num3 = (float)num2 * (1f / this.tuning.spriteCount);
			this.movingBallMesh.Begin();
			this.movingBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
			this.movingBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
			this.movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num, num3));
			this.movingBallMesh.SetVector("_Highlight", new Vector4((float)this.highlightColour.r / 255f, (float)this.highlightColour.g / 255f, (float)this.highlightColour.b / 255f, 0f));
			this.staticBallMesh.Begin();
			this.staticBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
			this.staticBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
			this.staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num, 0f));
			this.staticBallMesh.SetVector("_Highlight", new Vector4((float)this.highlightColour.r / 255f, (float)this.highlightColour.g / 255f, (float)this.highlightColour.b / 255f, 0f));
			for (int j = 0; j < this.flowManager.soaInfo.NumEntries; j++)
			{
				int cell = this.flowManager.soaInfo.GetCell(j);
				Vector2I vector2I3 = Grid.CellToXY(cell);
				if (!(vector2I3 < vector2I) && !(vector2I3 > vector2I2))
				{
					ConduitFlow.Conduit conduit = this.flowManager.soaInfo.GetConduit(j);
					ConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(this.flowManager);
					ConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(this.flowManager);
					if (lastFlowInfo.contents.mass > 0f)
					{
						int cell2 = conduit.GetCell(this.flowManager);
						int cellFromDirection = ConduitFlow.GetCellFromDirection(cell2, lastFlowInfo.direction);
						Vector2I vector2I4 = Grid.CellToXY(cell2);
						Vector2I vector2I5 = Grid.CellToXY(cellFromDirection);
						Vector2 vector = vector2I4;
						if (cell2 != -1)
						{
							vector = Vector2.Lerp(new Vector2((float)vector2I4.x, (float)vector2I4.y), new Vector2((float)vector2I5.x, (float)vector2I5.y), lerp_percent);
						}
						Color32 cellTintColour = this.GetCellTintColour(cell2);
						Color32 cellTintColour2 = this.GetCellTintColour(cellFromDirection);
						Color32 color = Color32.Lerp(cellTintColour, cellTintColour2, lerp_percent);
						Vector2I vector2I6 = new Vector2I(0, 0);
						Vector2I vector2I7 = new Vector2I(0, 1);
						Vector2I vector2I8 = new Vector2I(1, 0);
						Vector2I vector2I9 = new Vector2I(1, 1);
						switch (lastFlowInfo.direction)
						{
						case ConduitFlow.FlowDirection.Up:
						case ConduitFlow.FlowDirection.Down:
							vector2I6 = new Vector2I(1, 0);
							vector2I7 = new Vector2I(0, 0);
							vector2I8 = new Vector2I(1, 1);
							vector2I9 = new Vector2I(0, 1);
							break;
						}
						IL_04B5:
						float num4 = 0f;
						if (this.showContents)
						{
							if (lastFlowInfo.contents.mass >= initialContents.mass)
							{
								this.movingBallMesh.AddQuad(vector, color, this.tuning.size, 0f, 0f, vector2I6, vector2I7, vector2I8, vector2I9);
							}
							if (element == null || lastFlowInfo.contents.element != element.id)
							{
								element = ElementLoader.FindElementByHash(lastFlowInfo.contents.element);
							}
						}
						else
						{
							element = null;
							int num5 = Grid.PosToCell(new Vector3(vector.x + ConduitFlowVisualizer.GRID_OFFSET.x, vector.y + ConduitFlowVisualizer.GRID_OFFSET.y, 0f));
							if (num5 == this.highlightedCell)
							{
								num4 = 1f;
							}
						}
						Color32 contentsColor = this.GetContentsColor(element, color);
						float num6 = 1f;
						if (this.showContents || lastFlowInfo.contents.mass < initialContents.mass)
						{
							num6 = this.CalculateMassScale(lastFlowInfo.contents.mass);
						}
						this.movingBallMesh.AddQuad(vector, contentsColor, this.tuning.size * num6, 1f, num4, vector2I6, vector2I7, vector2I8, vector2I9);
						if (trigger_audio)
						{
							this.AddAudioSource(conduit, position);
							goto IL_0615;
						}
						goto IL_0615;
						goto IL_04B5;
					}
					IL_0615:
					if (initialContents.mass > lastFlowInfo.contents.mass && initialContents.mass > 0f)
					{
						int cell3 = conduit.GetCell(this.flowManager);
						Vector2I vector2I10 = Grid.CellToXY(cell3);
						Vector2 vector2 = vector2I10;
						float num7 = initialContents.mass - lastFlowInfo.contents.mass;
						Vector2I vector2I11 = new Vector2I(0, 0);
						Vector2I vector2I12 = new Vector2I(0, 1);
						Vector2I vector2I13 = new Vector2I(1, 0);
						Vector2I vector2I14 = new Vector2I(1, 1);
						float num8 = 0f;
						Color32 cellTintColour3 = this.GetCellTintColour(cell3);
						float num9 = this.CalculateMassScale(num7);
						if (this.showContents)
						{
							this.staticBallMesh.AddQuad(vector2, cellTintColour3, this.tuning.size * num9, 0f, 0f, vector2I11, vector2I12, vector2I13, vector2I14);
							if (element == null || initialContents.element != element.id)
							{
								element = ElementLoader.FindElementByHash(initialContents.element);
							}
						}
						else
						{
							element = null;
							if (cell3 == this.highlightedCell)
							{
								num8 = 1f;
							}
						}
						Color32 contentsColor2 = this.GetContentsColor(element, cellTintColour3);
						this.staticBallMesh.AddQuad(vector2, contentsColor2, this.tuning.size * num9, 1f, num8, vector2I11, vector2I12, vector2I13, vector2I14);
					}
				}
			}
			this.movingBallMesh.End(z, this.layer);
			this.staticBallMesh.End(z, this.layer);
		}
		if (trigger_audio)
		{
			this.TriggerAudio();
		}
	}

	public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
	{
		this.showContents = show_contents;
		this.layer = ((!show_contents || !move_to_overlay_layer) ? 0 : LayerMask.NameToLayer("MaskedOverlay"));
	}

	private void AddAudioSource(ConduitFlow.Conduit conduit, Vector3 camera_pos)
	{
		using (new KProfiler.Region("AddAudioSource", null))
		{
			UtilityNetwork network = this.flowManager.GetNetwork(conduit);
			if (network != null)
			{
				Vector3 vector = Grid.CellToPosCCC(conduit.GetCell(this.flowManager), Grid.SceneLayer.Building);
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
		if (SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
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

	public void AddThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			this.insulatedCells.Add(cell);
		}
		else if (conductivity > 1f)
		{
			this.radiantCells.Add(cell);
		}
	}

	public void RemoveThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			this.insulatedCells.Remove(cell);
		}
		else if (conductivity > 1f)
		{
			this.radiantCells.Remove(cell);
		}
	}

	public void SetHighlightedCell(int cell)
	{
		this.highlightedCell = cell;
	}

	private ConduitFlow flowManager;

	private string overlaySound;

	private bool showContents;

	private double animTime;

	private int layer;

	private static Vector2 GRID_OFFSET = new Vector2(0.5f, 0.5f);

	private List<ConduitFlowVisualizer.AudioInfo> audioInfo;

	private HashSet<int> insulatedCells = new HashSet<int>();

	private HashSet<int> radiantCells = new HashSet<int>();

	private Game.ConduitVisInfo visInfo;

	private ConduitFlowVisualizer.ConduitFlowMesh movingBallMesh;

	private ConduitFlowVisualizer.ConduitFlowMesh staticBallMesh;

	private int highlightedCell = -1;

	private Color32 highlightColour = new Color(0.2f, 0.2f, 0.2f, 0.2f);

	private ConduitFlowVisualizer.Tuning tuning;

	[Serializable]
	public class Tuning
	{
		public bool renderMesh;

		public float size;

		public float spriteCount;

		public float framesPerSecond;

		public Texture2D backgroundTexture;

		public Texture2D foregroundTexture;
	}

	private class ConduitFlowMesh
	{
		public ConduitFlowMesh()
		{
			this.mesh = new Mesh();
			this.mesh.name = "ConduitMesh";
			this.material = new Material(Shader.Find("Klei/ConduitBall"));
		}

		public void AddQuad(Vector2 pos, Color32 color, float size, float is_foreground, float highlight, Vector2I uvbl, Vector2I uvtl, Vector2I uvbr, Vector2I uvtr)
		{
			float num = size * 0.5f;
			this.positions.Add(new Vector3(pos.x - num, pos.y - num, 0f));
			this.positions.Add(new Vector3(pos.x - num, pos.y + num, 0f));
			this.positions.Add(new Vector3(pos.x + num, pos.y - num, 0f));
			this.positions.Add(new Vector3(pos.x + num, pos.y + num, 0f));
			this.uvs.Add(new Vector4((float)uvbl.x, (float)uvbl.y, is_foreground, highlight));
			this.uvs.Add(new Vector4((float)uvtl.x, (float)uvtl.y, is_foreground, highlight));
			this.uvs.Add(new Vector4((float)uvbr.x, (float)uvbr.y, is_foreground, highlight));
			this.uvs.Add(new Vector4((float)uvtr.x, (float)uvtr.y, is_foreground, highlight));
			this.colors.Add(color);
			this.colors.Add(color);
			this.colors.Add(color);
			this.colors.Add(color);
			this.triangles.Add(this.quadIndex * 4);
			this.triangles.Add(this.quadIndex * 4 + 1);
			this.triangles.Add(this.quadIndex * 4 + 2);
			this.triangles.Add(this.quadIndex * 4 + 2);
			this.triangles.Add(this.quadIndex * 4 + 1);
			this.triangles.Add(this.quadIndex * 4 + 3);
			this.quadIndex++;
		}

		public void SetTexture(string id, Texture2D texture)
		{
			this.material.SetTexture(id, texture);
		}

		public void SetVector(string id, Vector4 data)
		{
			this.material.SetVector(id, data);
		}

		public void Begin()
		{
			this.positions.Clear();
			this.uvs.Clear();
			this.triangles.Clear();
			this.colors.Clear();
			this.quadIndex = 0;
		}

		public void End(float z, int layer)
		{
			this.mesh.Clear();
			this.mesh.SetVertices(this.positions);
			this.mesh.SetUVs(0, this.uvs);
			this.mesh.SetColors(this.colors);
			this.mesh.SetTriangles(this.triangles, 0, false);
			Graphics.DrawMesh(this.mesh, new Vector3(ConduitFlowVisualizer.GRID_OFFSET.x, ConduitFlowVisualizer.GRID_OFFSET.y, z - 0.1f), Quaternion.identity, this.material, layer);
		}

		public void Cleanup()
		{
			global::UnityEngine.Object.Destroy(this.mesh);
			this.mesh = null;
			global::UnityEngine.Object.Destroy(this.material);
			this.material = null;
		}

		private Mesh mesh;

		private Material material;

		private List<Vector3> positions = new List<Vector3>();

		private List<Vector4> uvs = new List<Vector4>();

		private List<int> triangles = new List<int>();

		private List<Color32> colors = new List<Color32>();

		private int quadIndex;
	}

	private struct AudioInfo
	{
		public int networkID;

		public int blobCount;

		public float distance;

		public Vector3 position;
	}
}
