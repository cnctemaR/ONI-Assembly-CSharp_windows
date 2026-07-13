using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ConduitFlowVisualizer
{
	public ConduitFlowVisualizer(ConduitFlow flow_manager, Game.ConduitVisInfo vis_info, EventReference overlay_sound, ConduitFlowVisualizer.Tuning tuning)
	{
		this.flowManager = flow_manager;
		this.visInfo = vis_info;
		this.overlaySound = overlay_sound;
		this.tuning = tuning;
		this.movingBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
		this.staticBallMesh = new ConduitFlowVisualizer.ConduitFlowMesh();
		ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.InitializeResources();
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
			Color color = element.substance.conduitColour;
			color.a = 128f;
			return color;
		}
		return default_color;
	}

	private Color32 GetTintColour()
	{
		if (!this.showContents)
		{
			return this.visInfo.tint;
		}
		return GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayTintName);
	}

	private Color32 GetInsulatedTintColour()
	{
		if (!this.showContents)
		{
			return this.visInfo.insulatedTint;
		}
		return GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayInsulatedTintName);
	}

	private Color32 GetRadiantTintColour()
	{
		if (!this.showContents)
		{
			return this.visInfo.radiantTint;
		}
		return GlobalAssets.Instance.colorSet.GetColorByName(this.visInfo.overlayRadiantTintName);
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
		if (this.tuning.renderMesh)
		{
			this.RenderMesh(z, render_layer, lerp_percent, trigger_audio);
		}
		if (trigger_audio)
		{
			this.TriggerAudio();
		}
	}

	private void RenderMesh(float z, int render_layer, float lerp_percent, bool trigger_audio)
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Vector2I vector2I = new Vector2I(Mathf.Max(0, visibleArea.Min.x - 1), Mathf.Max(0, visibleArea.Min.y - 1));
		Vector2I vector2I2 = new Vector2I(Mathf.Min(Grid.WidthInCells - 1, visibleArea.Max.x + 1), Mathf.Min(Grid.HeightInCells - 1, visibleArea.Max.y + 1));
		ConduitFlowVisualizer.RenderMeshContext renderMeshContext = new ConduitFlowVisualizer.RenderMeshContext(this, lerp_percent, vector2I, vector2I2);
		if (renderMeshContext.visible_conduits.Count == 0)
		{
			renderMeshContext.Finish();
			return;
		}
		ConduitFlowVisualizer.RenderMeshBatchJob.Instance.Reset(renderMeshContext);
		GlobalJobManager.Run(ConduitFlowVisualizer.RenderMeshBatchJob.Instance);
		float num = 0f;
		if (this.showContents)
		{
			num = 1f;
		}
		float num2 = (float)((int)(this.animTime / (1.0 / (double)this.tuning.framesPerSecond)) % (int)this.tuning.spriteCount) * (1f / this.tuning.spriteCount);
		this.movingBallMesh.Begin();
		this.movingBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
		this.movingBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
		this.movingBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num, num2));
		this.movingBallMesh.SetVector("_Highlight", new Vector4((float)this.highlightColour.r / 255f, (float)this.highlightColour.g / 255f, (float)this.highlightColour.b / 255f, 0f));
		this.staticBallMesh.Begin();
		this.staticBallMesh.SetTexture("_BackgroundTex", this.tuning.backgroundTexture);
		this.staticBallMesh.SetTexture("_ForegroundTex", this.tuning.foregroundTexture);
		this.staticBallMesh.SetVector("_SpriteSettings", new Vector4(1f / this.tuning.spriteCount, 1f, num, 0f));
		this.staticBallMesh.SetVector("_Highlight", new Vector4((float)this.highlightColour.r / 255f, (float)this.highlightColour.g / 255f, (float)this.highlightColour.b / 255f, 0f));
		Vector3 position = CameraController.Instance.transform.GetPosition();
		ConduitFlowVisualizer conduitFlowVisualizer = (trigger_audio ? this : null);
		ConduitFlowVisualizer.RenderMeshBatchJob.Instance.Finish(this.movingBallMesh, this.staticBallMesh, position, conduitFlowVisualizer);
		this.movingBallMesh.End(z, this.layer);
		this.staticBallMesh.End(z, this.layer);
		ConduitFlowVisualizer.RenderMeshBatchJob.Instance.Reset(ConduitFlowVisualizer.RenderMeshContext.EmptyContext);
	}

	public void ColourizePipeContents(bool show_contents, bool move_to_overlay_layer)
	{
		this.showContents = show_contents;
		this.layer = ((show_contents && move_to_overlay_layer) ? LayerMask.NameToLayer("MaskedOverlay") : 0);
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
				Vector3 position = audioInfo.position;
				position.z = 0f;
				EventInstance eventInstance = SoundEvent.BeginOneShot(this.overlaySound, position, 1f, false);
				eventInstance.setParameterByName("blobCount", (float)audioInfo.blobCount, false);
				eventInstance.setParameterByName("networkCount", (float)num, false);
				SoundEvent.EndOneShot(eventInstance);
			}
		}
	}

	public void AddThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			this.insulatedCells.Add(cell);
			return;
		}
		if (conductivity > 1f)
		{
			this.radiantCells.Add(cell);
		}
	}

	public void RemoveThermalConductivity(int cell, float conductivity)
	{
		if (conductivity < 1f)
		{
			this.insulatedCells.Remove(cell);
			return;
		}
		if (conductivity > 1f)
		{
			this.radiantCells.Remove(cell);
		}
	}

	public void SetHighlightedCell(int cell)
	{
		this.highlightedCell = cell;
	}

	private ConduitFlow flowManager;

	private EventReference overlaySound;

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

	private struct RenderMeshContext
	{
		public RenderMeshContext(ConduitFlowVisualizer outer, float lerp_percent, Vector2I min, Vector2I max)
		{
			this.outer = outer;
			this.lerp_percent = lerp_percent;
			this.visible_conduits = ListPool<int, ConduitFlowVisualizer>.Allocate();
			this.visible_conduits.Capacity = Math.Max(outer.flowManager.soaInfo.NumEntries, this.visible_conduits.Capacity);
			for (int num = 0; num != outer.flowManager.soaInfo.NumEntries; num++)
			{
				Vector2I vector2I = Grid.CellToXY(outer.flowManager.soaInfo.GetCell(num));
				if (min <= vector2I && vector2I <= max)
				{
					this.visible_conduits.Add(num);
				}
			}
		}

		public void Finish()
		{
			this.visible_conduits.Recycle();
		}

		public static ConduitFlowVisualizer.RenderMeshContext EmptyContext;

		public ListPool<int, ConduitFlowVisualizer>.PooledList visible_conduits;

		public ConduitFlowVisualizer outer;

		public float lerp_percent;
	}

	private class RenderMeshPerThreadData
	{
		public void Finish(ConduitFlowVisualizer.ConduitFlowMesh moving_ball_mesh, ConduitFlowVisualizer.ConduitFlowMesh static_ball_mesh, Vector3 camera_pos, ConduitFlowVisualizer visualizer)
		{
			for (int num = 0; num != this.moving_balls.Count; num++)
			{
				this.moving_balls[num].Consume(moving_ball_mesh);
			}
			this.moving_balls.Clear();
			for (int num2 = 0; num2 != this.static_balls.Count; num2++)
			{
				this.static_balls[num2].Consume(static_ball_mesh);
			}
			this.static_balls.Clear();
			if (visualizer != null)
			{
				foreach (ConduitFlow.Conduit conduit in this.moving_conduits)
				{
					visualizer.AddAudioSource(conduit, camera_pos);
				}
			}
			this.moving_conduits.Clear();
		}

		public List<ConduitFlowVisualizer.RenderMeshPerThreadData.Ball> moving_balls = new List<ConduitFlowVisualizer.RenderMeshPerThreadData.Ball>();

		public List<ConduitFlowVisualizer.RenderMeshPerThreadData.Ball> static_balls = new List<ConduitFlowVisualizer.RenderMeshPerThreadData.Ball>();

		public List<ConduitFlow.Conduit> moving_conduits = new List<ConduitFlow.Conduit>();

		public struct Ball
		{
			public Ball(ConduitFlow.FlowDirections direction, Vector2 pos, Color32 color, float size, bool foreground, bool highlight)
			{
				this.pos = pos;
				this.size = size;
				this.color = color;
				this.direction = direction;
				this.foreground = foreground;
				this.highlight = highlight;
			}

			public static void InitializeResources()
			{
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.None] = new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack
				{
					bl = new Vector2I(0, 0),
					tl = new Vector2I(0, 1),
					br = new Vector2I(1, 0),
					tr = new Vector2I(1, 1)
				};
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Left] = new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack
				{
					bl = new Vector2I(0, 0),
					tl = new Vector2I(0, 1),
					br = new Vector2I(1, 0),
					tr = new Vector2I(1, 1)
				};
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Right] = ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Left];
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Up] = new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack
				{
					bl = new Vector2I(1, 0),
					tl = new Vector2I(0, 0),
					br = new Vector2I(1, 1),
					tr = new Vector2I(0, 1)
				};
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Down] = ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[ConduitFlow.FlowDirections.Up];
			}

			private static ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack GetUVPack(ConduitFlow.FlowDirections direction)
			{
				return ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.uv_packs[direction];
			}

			public void Consume(ConduitFlowVisualizer.ConduitFlowMesh mesh)
			{
				ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack uvpack = ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.GetUVPack(this.direction);
				mesh.AddQuad(this.pos, this.color, this.size, (float)(this.foreground ? 1 : 0), (float)(this.highlight ? 1 : 0), uvpack.bl, uvpack.tl, uvpack.br, uvpack.tr);
			}

			private Vector2 pos;

			private float size;

			private Color32 color;

			private ConduitFlow.FlowDirections direction;

			private bool foreground;

			private bool highlight;

			private static Dictionary<ConduitFlow.FlowDirections, ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack> uv_packs = new Dictionary<ConduitFlow.FlowDirections, ConduitFlowVisualizer.RenderMeshPerThreadData.Ball.UVPack>();

			private class UVPack
			{
				public Vector2I bl;

				public Vector2I tl;

				public Vector2I br;

				public Vector2I tr;
			}
		}
	}

	private class RenderMeshBatchJob : WorkItemCollectionWithThreadContex<ConduitFlowVisualizer.RenderMeshContext, ConduitFlowVisualizer.RenderMeshPerThreadData>
	{
		public static ConduitFlowVisualizer.RenderMeshBatchJob Instance
		{
			get
			{
				if (ConduitFlowVisualizer.RenderMeshBatchJob.instance == null || ConduitFlowVisualizer.RenderMeshBatchJob.instance.threadContexts.Count != GlobalJobManager.ThreadCount)
				{
					ConduitFlowVisualizer.RenderMeshBatchJob.instance = new ConduitFlowVisualizer.RenderMeshBatchJob();
				}
				return ConduitFlowVisualizer.RenderMeshBatchJob.instance;
			}
		}

		public RenderMeshBatchJob()
		{
			this.threadContexts = new List<ConduitFlowVisualizer.RenderMeshPerThreadData>();
			for (int i = 0; i < GlobalJobManager.ThreadCount; i++)
			{
				this.threadContexts.Add(new ConduitFlowVisualizer.RenderMeshPerThreadData());
			}
		}

		public void Reset(ConduitFlowVisualizer.RenderMeshContext context)
		{
			this.sharedData = context;
			if (context.visible_conduits == null)
			{
				this.count = 0;
				return;
			}
			this.count = (context.visible_conduits.Count + 32 - 1) / 32;
		}

		public override void RunItem(int item, ref ConduitFlowVisualizer.RenderMeshContext shared_data, ConduitFlowVisualizer.RenderMeshPerThreadData thread_context, int threadIndex)
		{
			Element element = null;
			int num = item * 32;
			int num2 = Math.Min(shared_data.visible_conduits.Count, num + 32);
			for (int i = num; i < num2; i++)
			{
				ConduitFlow.Conduit conduit = shared_data.outer.flowManager.soaInfo.GetConduit(shared_data.visible_conduits[i]);
				ConduitFlow.ConduitFlowInfo lastFlowInfo = conduit.GetLastFlowInfo(shared_data.outer.flowManager);
				ConduitFlow.ConduitContents initialContents = conduit.GetInitialContents(shared_data.outer.flowManager);
				if (lastFlowInfo.contents.mass > 0f)
				{
					int cell = conduit.GetCell(shared_data.outer.flowManager);
					int cellFromDirection = ConduitFlow.GetCellFromDirection(cell, lastFlowInfo.direction);
					Vector2I vector2I = Grid.CellToXY(cell);
					Vector2I vector2I2 = Grid.CellToXY(cellFromDirection);
					Vector2 vector = ((cell == -1) ? vector2I : Vector2.Lerp(new Vector2((float)vector2I.x, (float)vector2I.y), new Vector2((float)vector2I2.x, (float)vector2I2.y), shared_data.lerp_percent));
					Color32 cellTintColour = shared_data.outer.GetCellTintColour(cell);
					Color32 cellTintColour2 = shared_data.outer.GetCellTintColour(cellFromDirection);
					Color32 color = Color32.Lerp(cellTintColour, cellTintColour2, shared_data.lerp_percent);
					bool flag = false;
					if (shared_data.outer.showContents)
					{
						if (lastFlowInfo.contents.mass >= initialContents.mass)
						{
							thread_context.moving_balls.Add(new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball(lastFlowInfo.direction, vector, color, shared_data.outer.tuning.size, false, false));
						}
						if (element == null || lastFlowInfo.contents.element != element.id)
						{
							element = ElementLoader.FindElementByHash(lastFlowInfo.contents.element);
						}
					}
					else
					{
						element = null;
						flag = Grid.PosToCell(new Vector3(vector.x + ConduitFlowVisualizer.GRID_OFFSET.x, vector.y + ConduitFlowVisualizer.GRID_OFFSET.y, 0f)) == shared_data.outer.highlightedCell;
					}
					Color32 contentsColor = shared_data.outer.GetContentsColor(element, color);
					float num3 = 1f;
					if (shared_data.outer.showContents || lastFlowInfo.contents.mass < initialContents.mass)
					{
						num3 = shared_data.outer.CalculateMassScale(lastFlowInfo.contents.mass);
					}
					thread_context.moving_balls.Add(new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball(lastFlowInfo.direction, vector, contentsColor, shared_data.outer.tuning.size * num3, true, flag));
					thread_context.moving_conduits.Add(conduit);
				}
				if (initialContents.mass > lastFlowInfo.contents.mass && initialContents.mass > 0f)
				{
					int cell2 = conduit.GetCell(shared_data.outer.flowManager);
					Vector2 vector2 = Grid.CellToXY(cell2);
					float num4 = initialContents.mass - lastFlowInfo.contents.mass;
					bool flag2 = false;
					Color32 cellTintColour3 = shared_data.outer.GetCellTintColour(cell2);
					float num5 = shared_data.outer.CalculateMassScale(num4);
					if (shared_data.outer.showContents)
					{
						thread_context.static_balls.Add(new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball(ConduitFlow.FlowDirections.None, vector2, cellTintColour3, shared_data.outer.tuning.size * num5, false, false));
						if (element == null || initialContents.element != element.id)
						{
							element = ElementLoader.FindElementByHash(initialContents.element);
						}
					}
					else
					{
						element = null;
						flag2 = cell2 == shared_data.outer.highlightedCell;
					}
					Color32 contentsColor2 = shared_data.outer.GetContentsColor(element, cellTintColour3);
					thread_context.static_balls.Add(new ConduitFlowVisualizer.RenderMeshPerThreadData.Ball(ConduitFlow.FlowDirections.None, vector2, contentsColor2, shared_data.outer.tuning.size * num5, true, flag2));
				}
			}
		}

		public void Finish(ConduitFlowVisualizer.ConduitFlowMesh moving_ball_mesh, ConduitFlowVisualizer.ConduitFlowMesh static_ball_mesh, Vector3 camera_pos, ConduitFlowVisualizer visualizer)
		{
			foreach (ConduitFlowVisualizer.RenderMeshPerThreadData renderMeshPerThreadData in this.threadContexts)
			{
				renderMeshPerThreadData.Finish(moving_ball_mesh, static_ball_mesh, camera_pos, visualizer);
			}
			this.sharedData.Finish();
		}

		private const int kBatchSize = 32;

		private static ConduitFlowVisualizer.RenderMeshBatchJob instance;
	}
}
