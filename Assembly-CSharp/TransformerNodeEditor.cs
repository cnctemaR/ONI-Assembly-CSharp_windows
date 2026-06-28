using System;
using Klei.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using UnityEngine;

[Node(false, "Noise/Transformer", new Type[] { typeof(NoiseNodeCanvas) })]
public class TransformerNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "transformerNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(TransformerNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		TransformerNodeEditor transformerNodeEditor = ScriptableObject.CreateInstance<TransformerNodeEditor>();
		transformerNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 200f);
		transformerNodeEditor.name = "Transformer";
		transformerNodeEditor.CreateInput("Source", "IModule3D", NodeSide.Left, 10f);
		transformerNodeEditor.CreateInput("X", "IModule3D", NodeSide.Left, 30f);
		transformerNodeEditor.CreateInput("Y", "IModule3D", NodeSide.Left, 40f);
		transformerNodeEditor.CreateInput("Z", "IModule3D", NodeSide.Left, 50f);
		transformerNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return transformerNodeEditor;
	}

	public override bool Calculate()
	{
		IModule3D value = this.Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		IModule3D value2 = this.Inputs[1].GetValue<IModule3D>();
		IModule3D value3 = this.Inputs[2].GetValue<IModule3D>();
		IModule3D value4 = this.Inputs[3].GetValue<IModule3D>();
		if (this.target.transformerType != Transformer.TransformerType.RotatePoint)
		{
			if (value2 == null)
			{
				return false;
			}
			if (value3 == null)
			{
				return false;
			}
			if (value4 == null)
			{
				return false;
			}
		}
		IModule3D module3D = this.target.CreateModule(value, value2, value3, value4);
		if (module3D == null)
		{
			return false;
		}
		this.Outputs[0].SetValue<IModule3D>(module3D);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "transformerNodeEditor";

	[SerializeField]
	public Transformer target = new Transformer();
}
