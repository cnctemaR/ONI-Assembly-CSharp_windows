using System;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(true, "Noise/Base Noise Node", new Type[] { typeof(NoiseNodeCanvas) })]
public class BaseNodeEditor : Node
{
	public virtual Type GetObjectType
	{
		get
		{
			return typeof(BaseNodeEditor);
		}
	}

	public override string GetID
	{
		get
		{
			return "baseNodeEditor";
		}
	}

	public virtual NoiseBase GetTarget()
	{
		return null;
	}

	protected SampleSettings settings
	{
		get
		{
			NoiseNodeCanvas noiseNodeCanvas = NodeEditor.curNodeCanvas as NoiseNodeCanvas;
			SampleSettings sampleSettings;
			if (noiseNodeCanvas != null)
			{
				sampleSettings = noiseNodeCanvas.settings;
			}
			else
			{
				sampleSettings = null;
			}
			return sampleSettings;
		}
	}

	public override Node Create(Vector2 pos)
	{
		return null;
	}

	protected override void NodeGUI()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (this.Inputs != null)
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			foreach (NodeInput nodeInput in this.Inputs)
			{
				nodeInput.DisplayLayout();
			}
			GUILayout.EndVertical();
		}
		if (this.Outputs != null)
		{
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			foreach (NodeOutput nodeOutput in this.Outputs)
			{
				nodeOutput.DisplayLayout();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
		if (GUI.changed)
		{
			NodeEditor.RecalculateFrom(this);
		}
	}

	private const string Id = "baseNodeEditor";
}
