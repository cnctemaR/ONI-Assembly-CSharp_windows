using System;
using Unity.Collections;
using UnityEngine;

public class RocketLaunchConditionVisualizerEffect : VisualizerEffect
{
	protected override void SetupMaterial()
	{
		this.material = new Material(Shader.Find("Klei/PostFX/RocketLaunchCondition"));
	}

	protected override void SetupOcclusionTex()
	{
		this.OcclusionTex = new Texture2D(512, 1, TextureFormat.RGFloat, false);
		this.OcclusionTex.filterMode = FilterMode.Point;
		this.OcclusionTex.wrapMode = TextureWrapMode.Clamp;
	}

	protected override void OnPostRender()
	{
		RocketLaunchConditionVisualizer rocketLaunchConditionVisualizer = null;
		if (SelectTool.Instance.selected != null)
		{
			rocketLaunchConditionVisualizer = SelectTool.Instance.selected.GetComponent<RocketLaunchConditionVisualizer>();
			if (rocketLaunchConditionVisualizer == null)
			{
				RocketModuleCluster component = SelectTool.Instance.selected.GetComponent<RocketModuleCluster>();
				if (component != null)
				{
					PassengerRocketModule passengerModule = component.CraftInterface.GetPassengerModule();
					if (passengerModule != null)
					{
						rocketLaunchConditionVisualizer = passengerModule.gameObject.GetComponent<RocketLaunchConditionVisualizer>();
					}
				}
			}
		}
		if (rocketLaunchConditionVisualizer != null)
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			RocketLaunchConditionVisualizerEffect.FindWorldBounds(out vector2I, out vector2I2);
			if (vector2I2.x - vector2I.x > this.OcclusionTex.width)
			{
				return;
			}
			NativeArray<float> pixelData = this.OcclusionTex.GetPixelData<float>(0);
			for (int i = 0; i < this.OcclusionTex.width; i++)
			{
				pixelData[2 * i] = 0f;
				pixelData[2 * i + 1] = 0f;
			}
			for (int j = 0; j < RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn.Length; j++)
			{
				RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn[j] = RocketLaunchConditionVisualizerEffect.EvaluationState.NotEvaluated;
			}
			for (int k = 0; k < rocketLaunchConditionVisualizer.moduleVisualizeData.Length; k++)
			{
				RocketLaunchConditionVisualizerEffect.ComputeVisibility(rocketLaunchConditionVisualizer.moduleVisualizeData[k], pixelData, vector2I, vector2I2);
			}
			this.OcclusionTex.Apply(false, false);
			Vector2I vector2I3 = vector2I;
			Vector2I vector2I4 = vector2I2;
			if (this.myCamera == null)
			{
				this.myCamera = base.GetComponent<Camera>();
				if (this.myCamera == null)
				{
					return;
				}
			}
			Ray ray = this.myCamera.ViewportPointToRay(Vector3.zero);
			float num = Mathf.Abs(ray.origin.z / ray.direction.z);
			Vector3 vector = ray.GetPoint(num);
			Vector4 vector2;
			vector2.x = vector.x;
			vector2.y = vector.y;
			ray = this.myCamera.ViewportPointToRay(Vector3.one);
			num = Mathf.Abs(ray.origin.z / ray.direction.z);
			vector = ray.GetPoint(num);
			vector2.z = vector.x - vector2.x;
			vector2.w = vector.y - vector2.y;
			this.material.SetVector("_UVOffsetScale", vector2);
			Vector4 vector3;
			vector3.x = (float)vector2I3.x;
			vector3.y = (float)vector2I3.y;
			vector3.z = (float)vector2I4.x;
			vector3.w = (float)vector2I4.y;
			this.material.SetVector("_RangeParams", vector3);
			this.material.SetColor("_HighlightColor", this.highlightColor);
			this.material.SetColor("_HighlightColor2", this.highlightColor2);
			Vector4 vector4;
			vector4.x = 1f / (float)this.OcclusionTex.width;
			vector4.y = 1f / (float)this.OcclusionTex.height;
			vector4.z = 0f;
			vector4.w = 0f;
			this.material.SetVector("_OcclusionParams", vector4);
			this.material.SetTexture("_OcclusionTex", this.OcclusionTex);
			Vector4 vector5;
			vector5.x = (float)Grid.WidthInCells;
			vector5.y = (float)Grid.HeightInCells;
			vector5.z = 1f / (float)Grid.WidthInCells;
			vector5.w = 1f / (float)Grid.HeightInCells;
			this.material.SetVector("_WorldParams", vector5);
			GL.PushMatrix();
			this.material.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(5);
			GL.Color(Color.white);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(0f, 1f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}

	private static void ComputeVisibility(RocketLaunchConditionVisualizer.RocketModuleVisualizeData moduleData, NativeArray<float> pixels, Vector2I world_min, Vector2I world_max)
	{
		Vector2I vector2I = Grid.PosToXY(moduleData.Module.transform.GetPosition());
		int rangeMin = moduleData.RangeMin;
		int rangeMax = moduleData.RangeMax;
		Vector2I vector2I2 = vector2I + moduleData.OriginOffset;
		for (int i = 0; i >= rangeMin; i--)
		{
			int num = vector2I2.x + i;
			int y = vector2I2.y;
			RocketLaunchConditionVisualizerEffect.EvaluationState evaluationState = RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn[RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn_middleIndex + i];
			RocketLaunchConditionVisualizerEffect.ComputeVisibility(num, y, pixels, world_min, world_max, ref evaluationState);
			RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn[RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn_middleIndex + i] = evaluationState;
		}
		for (int j = 0; j <= rangeMax; j++)
		{
			int num2 = vector2I2.x + j;
			int y2 = vector2I2.y;
			RocketLaunchConditionVisualizerEffect.EvaluationState evaluationState2 = RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn[RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn_middleIndex + j];
			RocketLaunchConditionVisualizerEffect.ComputeVisibility(num2, y2, pixels, world_min, world_max, ref evaluationState2);
			RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn[RocketLaunchConditionVisualizerEffect.clearPathToSpaceColumn_middleIndex + j] = evaluationState2;
		}
	}

	private static void ComputeVisibility(int x_abs, int y_abs, NativeArray<float> pixels, Vector2I world_min, Vector2I world_max, ref RocketLaunchConditionVisualizerEffect.EvaluationState clearPathEvaluation)
	{
		int num = x_abs - world_min.x;
		if (x_abs < world_min.x || x_abs > world_max.x || y_abs < world_min.y || y_abs >= world_max.y)
		{
			return;
		}
		int num2 = Grid.XYToCell(x_abs, y_abs);
		if (clearPathEvaluation == RocketLaunchConditionVisualizerEffect.EvaluationState.NotEvaluated)
		{
			clearPathEvaluation = (RocketLaunchConditionVisualizerEffect.HasClearPathToSpace(num2, world_max) ? RocketLaunchConditionVisualizerEffect.EvaluationState.Clear : RocketLaunchConditionVisualizerEffect.EvaluationState.Obstructed);
		}
		bool flag = clearPathEvaluation == RocketLaunchConditionVisualizerEffect.EvaluationState.Clear;
		if (pixels[2 * num] == 2f)
		{
			if (flag)
			{
				pixels[2 * num + 1] = Mathf.Min(pixels[2 * num + 1], (float)y_abs);
			}
			return;
		}
		pixels[2 * num] = (float)(flag ? 2 : 1);
		if (pixels[2 * num] == 1f && pixels[2 * num + 1] != 0f)
		{
			pixels[2 * num + 1] = Mathf.Min(pixels[2 * num + 1], (float)y_abs);
			return;
		}
		pixels[2 * num + 1] = (float)y_abs;
	}

	private static void FindWorldBounds(out Vector2I world_min, out Vector2I world_max)
	{
		if (ClusterManager.Instance != null)
		{
			WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
			world_min = activeWorld.WorldOffset;
			world_max = activeWorld.WorldOffset + activeWorld.WorldSize;
			return;
		}
		world_min.x = 0;
		world_min.y = 0;
		world_max.x = Grid.WidthInCells;
		world_max.y = Grid.HeightInCells;
	}

	private static bool HasClearPathToSpace(int cell, Vector2I worldMax)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num = cell;
		while (!Grid.IsSolidCell(num) && Grid.CellToXY(num).y < worldMax.y)
		{
			num = Grid.CellAbove(num);
		}
		return !Grid.IsSolidCell(num) && Grid.CellToXY(num).y == worldMax.y;
	}

	public Color highlightColor = new Color(0f, 1f, 0.8f, 1f);

	public Color highlightColor2 = new Color(1f, 0.32f, 0f, 1f);

	private static RocketLaunchConditionVisualizerEffect.EvaluationState[] clearPathToSpaceColumn = new RocketLaunchConditionVisualizerEffect.EvaluationState[7];

	private static int clearPathToSpaceColumn_middleIndex = Mathf.FloorToInt(3.5f);

	public enum EvaluationState : byte
	{
		NotEvaluated,
		Clear,
		Obstructed
	}
}
