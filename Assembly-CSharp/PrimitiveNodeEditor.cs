using System;
using Klei.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using UnityEngine;

[Node(false, "Noise/Primitive", new Type[] { typeof(NoiseNodeCanvas) })]
public class PrimitiveNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "primitiveNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(PrimitiveNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		PrimitiveNodeEditor primitiveNodeEditor = ScriptableObject.CreateInstance<PrimitiveNodeEditor>();
		primitiveNodeEditor.target = new Primitive();
		primitiveNodeEditor.rect = new Rect(pos.x, pos.y, 250f, 125f);
		primitiveNodeEditor.name = "Primative";
		primitiveNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return primitiveNodeEditor;
	}

	public override bool Calculate()
	{
		this.Outputs[0].SetValue<IModule3D>(this.target.CreateModule());
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "primitiveNodeEditor";

	public Primitive target = new Primitive();
}
