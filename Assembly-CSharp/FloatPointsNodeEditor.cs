using System;
using Klei.Noise;
using NodeEditorFramework;
using UnityEngine;

[Node(false, "Noise/Terrace Control", new Type[] { typeof(NoiseNodeCanvas) })]
public class FloatPointsNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "floatPointsNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(FloatPointsNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		FloatPointsNodeEditor floatPointsNodeEditor = ScriptableObject.CreateInstance<FloatPointsNodeEditor>();
		floatPointsNodeEditor.rect = new Rect(pos.x, pos.y, 300f, FloatPointsNodeEditor.height);
		floatPointsNodeEditor.name = "Terrace Control";
		floatPointsNodeEditor.CreateOutput("Terrace", "FloatList", NodeSide.Right, 30f);
		return floatPointsNodeEditor;
	}

	public override bool Calculate()
	{
		this.Outputs[0].SetValue<FloatList>(this.target);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "floatPointsNodeEditor";

	[SerializeField]
	public FloatList target = new FloatList();

	private static float height = 100f;
}
