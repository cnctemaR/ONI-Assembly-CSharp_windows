using System;
using System.Collections.Generic;
using Klei;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using ProcGen.Noise;
using UnityEngine;

[NodeCanvasType("Noise Canvas")]
public class NoiseNodeCanvas : NodeCanvas
{
	[SerializeField]
	public SampleSettings settings { get; private set; }

	public static NoiseNodeCanvas CreateInstance()
	{
		NoiseNodeCanvas noiseNodeCanvas = ScriptableObject.CreateInstance<NoiseNodeCanvas>();
		noiseNodeCanvas.ntf = YamlIO<NoiseTreeFiles>.LoadFile(NoiseTreeFiles.GetPath());
		return noiseNodeCanvas;
	}

	public override void UpdateSettings(string sceneCanvasName)
	{
		if (this.settings == null)
		{
			this.settings = new SampleSettings();
			this.settings.name = sceneCanvasName;
		}
	}

	public override string DrawAdditionalSettings(string sceneCanvasName)
	{
		return sceneCanvasName;
	}

	public override void BeforeSavingCanvas()
	{
		foreach (Node node in this.nodes)
		{
			BaseNodeEditor baseNodeEditor = (BaseNodeEditor)node;
			NoiseBase target = baseNodeEditor.GetTarget();
			if (target != null)
			{
				target.pos = new Vector2f(baseNodeEditor.rect.position);
			}
		}
	}

	public override void AdditionalSaveMethods(string sceneCanvasName, NodeCanvas.CompleteLoadCallback onComplete)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button(new GUIContent("Load Yaml", "Loads the Canvas from a Yaml Save File"), new GUILayoutOption[0]))
		{
			this.Load(sceneCanvasName, onComplete);
		}
		if (GUILayout.Button(new GUIContent("Save to Yaml", "Saves the Canvas to a Yaml file"), new GUILayoutOption[] { GUILayout.ExpandWidth(false) }))
		{
			this.BeforeSavingCanvas();
			Tree tree = this.BuildTreeFromCanvas();
			if (tree != null)
			{
				tree.ClearEmptyLists();
				string treeFilePath = NoiseTreeFiles.GetTreeFilePath(sceneCanvasName);
				tree.Save(treeFilePath);
			}
		}
		GUILayout.EndHorizontal();
		if (this.ntf == null)
		{
			this.ntf = YamlIO<NoiseTreeFiles>.LoadFile(NoiseTreeFiles.GetPath());
		}
		if (this.ntf != null && GUILayout.Button(new GUIContent("Load Tree", "Loads the Canvas from Trees list"), new GUILayoutOption[0]))
		{
			GenericMenu genericMenu = new GenericMenu();
			foreach (string text in this.ntf.tree_files)
			{
				genericMenu.AddItem(new GUIContent(text), false, delegate(object fileName)
				{
					this.Load((string)fileName, onComplete);
				}, text);
			}
			genericMenu.Show(this.lastRectPos.position, 40f);
		}
		if (Event.current.type == EventType.Repaint)
		{
			Rect lastRect = GUILayoutUtility.GetLastRect();
			this.lastRectPos = new Rect(lastRect.x + 2f, lastRect.yMax + 2f, lastRect.width - 4f, 0f);
		}
	}

	private void UpdateTerminator()
	{
		if (this.terminator == null)
		{
			foreach (Node node in this.nodes)
			{
				Type type = node.GetType();
				if (type == typeof(TerminalNodeEditor))
				{
					if (this.terminator == null)
					{
						this.terminator = node as TerminalNodeEditor;
					}
					else
					{
						node.Delete();
					}
				}
			}
			if (this.terminator == null)
			{
				this.terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
			}
		}
		Vector2 vector = this.terminator.rect.min + new Vector2(0f, -290f);
		DisplayNodeEditor displayNodeEditor = (DisplayNodeEditor)Node.Create("displayNodeEditor", vector);
		displayNodeEditor.Inputs[0].ApplyConnection(this.terminator.Outputs[0]);
	}

	private Link GetLink(Node node)
	{
		Link link = new Link();
		Type type = node.GetType();
		if (type == typeof(PrimitiveNodeEditor))
		{
			PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
			link.name = primitiveNodeEditor.target.name;
			link.type = Link.Type.Primitive;
		}
		else if (type == typeof(FilterNodeEditor))
		{
			FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
			link.name = filterNodeEditor.target.name;
			link.type = Link.Type.Filter;
		}
		else if (type == typeof(TransformerNodeEditor))
		{
			TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
			link.name = transformerNodeEditor.target.name;
			link.type = Link.Type.Transformer;
		}
		else if (type == typeof(SelectorModuleNodeEditor))
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
			link.name = selectorModuleNodeEditor.target.name;
			link.type = Link.Type.Selector;
		}
		else if (type == typeof(ModifierModuleNodeEditor))
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
			link.name = modifierModuleNodeEditor.target.name;
			link.type = Link.Type.Modifier;
		}
		else if (type == typeof(CombinerModuleNodeEditor))
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
			link.name = combinerModuleNodeEditor.target.name;
			link.type = Link.Type.Combiner;
		}
		else if (type == typeof(FloatPointsNodeEditor))
		{
			FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
			link.name = floatPointsNodeEditor.target.name;
			link.type = Link.Type.FloatPoints;
		}
		else if (type == typeof(ControlPointsNodeEditor))
		{
			ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
			link.name = controlPointsNodeEditor.target.name;
			link.type = Link.Type.ControlPoints;
		}
		else if (type == typeof(TerminalNodeEditor))
		{
			link.name = "TERMINATOR";
			link.type = Link.Type.Terminator;
		}
		return link;
	}

	public Tree BuildTreeFromCanvas()
	{
		Tree tree = new Tree();
		tree.settings = this.settings;
		foreach (Node node in this.nodes)
		{
			Type type = node.GetType();
			if (type == typeof(PrimitiveNodeEditor))
			{
				PrimitiveNodeEditor primitiveNodeEditor = node as PrimitiveNodeEditor;
				if (primitiveNodeEditor.target.name == null || primitiveNodeEditor.target.name == string.Empty || tree.primitives.ContainsKey(primitiveNodeEditor.target.name))
				{
					primitiveNodeEditor.target.name = "Primitive" + tree.primitives.Count;
				}
				tree.primitives.Add(primitiveNodeEditor.target.name, primitiveNodeEditor.target);
			}
			else if (type == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor = node as FilterNodeEditor;
				if (filterNodeEditor.target.name == null || filterNodeEditor.target.name == string.Empty || tree.filters.ContainsKey(filterNodeEditor.target.name))
				{
					filterNodeEditor.target.name = "Filter" + tree.filters.Count;
				}
				tree.filters.Add(filterNodeEditor.target.name, filterNodeEditor.target);
			}
			else if (type == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor = node as TransformerNodeEditor;
				if (transformerNodeEditor.target.name == null || transformerNodeEditor.target.name == string.Empty || tree.transformers.ContainsKey(transformerNodeEditor.target.name))
				{
					transformerNodeEditor.target.name = "Transformer" + tree.transformers.Count;
				}
				tree.transformers.Add(transformerNodeEditor.target.name, transformerNodeEditor.target);
			}
			else if (type == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor = node as SelectorModuleNodeEditor;
				if (selectorModuleNodeEditor.target.name == null || selectorModuleNodeEditor.target.name == string.Empty || tree.selectors.ContainsKey(selectorModuleNodeEditor.target.name))
				{
					selectorModuleNodeEditor.target.name = "Selector" + tree.selectors.Count;
				}
				tree.selectors.Add(selectorModuleNodeEditor.target.name, selectorModuleNodeEditor.target);
			}
			else if (type == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor = node as ModifierModuleNodeEditor;
				if (modifierModuleNodeEditor.target.name == null || modifierModuleNodeEditor.target.name == string.Empty || tree.modifiers.ContainsKey(modifierModuleNodeEditor.target.name))
				{
					modifierModuleNodeEditor.target.name = "Modifier" + tree.modifiers.Count;
				}
				tree.modifiers.Add(modifierModuleNodeEditor.target.name, modifierModuleNodeEditor.target);
			}
			else if (type == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor = node as CombinerModuleNodeEditor;
				if (combinerModuleNodeEditor.target.name == null || combinerModuleNodeEditor.target.name == string.Empty || tree.combiners.ContainsKey(combinerModuleNodeEditor.target.name))
				{
					combinerModuleNodeEditor.target.name = "Combiner" + tree.combiners.Count;
				}
				tree.combiners.Add(combinerModuleNodeEditor.target.name, combinerModuleNodeEditor.target);
			}
			else if (type == typeof(FloatPointsNodeEditor))
			{
				FloatPointsNodeEditor floatPointsNodeEditor = node as FloatPointsNodeEditor;
				if (floatPointsNodeEditor.target.name == null || floatPointsNodeEditor.target.name == string.Empty || tree.floats.ContainsKey(floatPointsNodeEditor.target.name))
				{
					floatPointsNodeEditor.target.name = "Terrace Control" + tree.combiners.Count;
				}
				tree.floats.Add(floatPointsNodeEditor.target.name, floatPointsNodeEditor.target);
			}
			else if (type == typeof(ControlPointsNodeEditor))
			{
				ControlPointsNodeEditor controlPointsNodeEditor = node as ControlPointsNodeEditor;
				if (controlPointsNodeEditor.target.name == null || controlPointsNodeEditor.target.name == string.Empty || tree.controlpoints.ContainsKey(controlPointsNodeEditor.target.name))
				{
					controlPointsNodeEditor.target.name = "Curve Control" + tree.combiners.Count;
				}
				tree.controlpoints.Add(controlPointsNodeEditor.target.name, controlPointsNodeEditor.target);
			}
			else if (type == typeof(TerminalNodeEditor) && this.terminator == null)
			{
				this.terminator = node as TerminalNodeEditor;
			}
		}
		foreach (Node node2 in this.nodes)
		{
			Type type2 = node2.GetType();
			if (type2 == typeof(FilterNodeEditor))
			{
				FilterNodeEditor filterNodeEditor2 = node2 as FilterNodeEditor;
				NodeLink nodeLink = new NodeLink();
				nodeLink.target = this.GetLink(node2);
				if (filterNodeEditor2.Inputs[0] != null && filterNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink.source0 = this.GetLink(filterNodeEditor2.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink);
			}
			else if (type2 == typeof(TransformerNodeEditor))
			{
				TransformerNodeEditor transformerNodeEditor2 = node2 as TransformerNodeEditor;
				NodeLink nodeLink2 = new NodeLink();
				nodeLink2.target = this.GetLink(node2);
				if (transformerNodeEditor2.Inputs[0] != null && transformerNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink2.source0 = this.GetLink(transformerNodeEditor2.Inputs[0].connection.body);
				}
				if (transformerNodeEditor2.Inputs[1] != null && transformerNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink2.source1 = this.GetLink(transformerNodeEditor2.Inputs[1].connection.body);
				}
				if (transformerNodeEditor2.Inputs[2] != null && transformerNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink2.source2 = this.GetLink(transformerNodeEditor2.Inputs[2].connection.body);
				}
				if (transformerNodeEditor2.Inputs[3] != null && transformerNodeEditor2.Inputs[3].connection != null)
				{
					nodeLink2.source3 = this.GetLink(transformerNodeEditor2.Inputs[3].connection.body);
				}
				tree.links.Add(nodeLink2);
			}
			else if (type2 == typeof(SelectorModuleNodeEditor))
			{
				SelectorModuleNodeEditor selectorModuleNodeEditor2 = node2 as SelectorModuleNodeEditor;
				NodeLink nodeLink3 = new NodeLink();
				nodeLink3.target = this.GetLink(node2);
				if (selectorModuleNodeEditor2.Inputs[0] != null && selectorModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink3.source0 = this.GetLink(selectorModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (selectorModuleNodeEditor2.Inputs[1] != null && selectorModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink3.source1 = this.GetLink(selectorModuleNodeEditor2.Inputs[1].connection.body);
				}
				if (selectorModuleNodeEditor2.Inputs[2] != null && selectorModuleNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink3.source2 = this.GetLink(selectorModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink3);
			}
			else if (type2 == typeof(ModifierModuleNodeEditor))
			{
				ModifierModuleNodeEditor modifierModuleNodeEditor2 = node2 as ModifierModuleNodeEditor;
				NodeLink nodeLink4 = new NodeLink();
				nodeLink4.target = this.GetLink(node2);
				if (modifierModuleNodeEditor2.Inputs[0] != null && modifierModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink4.source0 = this.GetLink(modifierModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (modifierModuleNodeEditor2.Inputs[1] != null && modifierModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink4.source1 = this.GetLink(modifierModuleNodeEditor2.Inputs[1].connection.body);
				}
				if (modifierModuleNodeEditor2.Inputs[2] != null && modifierModuleNodeEditor2.Inputs[2].connection != null)
				{
					nodeLink4.source2 = this.GetLink(modifierModuleNodeEditor2.Inputs[2].connection.body);
				}
				tree.links.Add(nodeLink4);
			}
			else if (type2 == typeof(CombinerModuleNodeEditor))
			{
				CombinerModuleNodeEditor combinerModuleNodeEditor2 = node2 as CombinerModuleNodeEditor;
				NodeLink nodeLink5 = new NodeLink();
				nodeLink5.target = this.GetLink(node2);
				if (combinerModuleNodeEditor2.Inputs[0] != null && combinerModuleNodeEditor2.Inputs[0].connection != null)
				{
					nodeLink5.source0 = this.GetLink(combinerModuleNodeEditor2.Inputs[0].connection.body);
				}
				if (combinerModuleNodeEditor2.Inputs[1] != null && combinerModuleNodeEditor2.Inputs[1].connection != null)
				{
					nodeLink5.source1 = this.GetLink(combinerModuleNodeEditor2.Inputs[1].connection.body);
				}
				tree.links.Add(nodeLink5);
			}
			else if (type2 == typeof(TerminalNodeEditor))
			{
				TerminalNodeEditor terminalNodeEditor = node2 as TerminalNodeEditor;
				NodeLink nodeLink6 = new NodeLink();
				nodeLink6.target = this.GetLink(node2);
				if (terminalNodeEditor.Inputs[0] != null && terminalNodeEditor.Inputs[0].connection != null)
				{
					nodeLink6.source0 = this.GetLink(terminalNodeEditor.Inputs[0].connection.body);
				}
				tree.links.Add(nodeLink6);
			}
		}
		return tree;
	}

	private NodeCanvas Load(string name, NodeCanvas.CompleteLoadCallback onComplete)
	{
		NodeCanvas nodeCanvas = null;
		string treeFilePath = NoiseTreeFiles.GetTreeFilePath(name);
		Tree tree = YamlIO<Tree>.LoadFile(treeFilePath);
		if (tree != null)
		{
			if (tree.settings.name == null || tree.settings.name == string.Empty)
			{
				tree.settings.name = name;
			}
			nodeCanvas = NoiseNodeCanvas.PopulateNoiseNodeEditor(tree);
		}
		onComplete(name, nodeCanvas);
		return nodeCanvas;
	}

	private Node GetNodeFromLink(Link link)
	{
		if (link == null)
		{
			return null;
		}
		switch (link.type)
		{
		case Link.Type.Primitive:
			if (this.primitiveLookup.ContainsKey(link.name))
			{
				return this.primitiveLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in primitives", null);
			break;
		case Link.Type.Filter:
			if (this.filterLookup.ContainsKey(link.name))
			{
				return this.filterLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in filters", null);
			break;
		case Link.Type.Transformer:
			if (this.transformerLookup.ContainsKey(link.name))
			{
				return this.transformerLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in transformers", null);
			break;
		case Link.Type.Selector:
			if (this.selectorLookup.ContainsKey(link.name))
			{
				return this.selectorLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in selectors", null);
			break;
		case Link.Type.Modifier:
			if (this.modifierLookup.ContainsKey(link.name))
			{
				return this.modifierLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in modifiers", null);
			break;
		case Link.Type.Combiner:
			if (this.combinerLookup.ContainsKey(link.name))
			{
				return this.combinerLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in combiners", null);
			break;
		case Link.Type.FloatPoints:
			if (this.floatlistLookup.ContainsKey(link.name))
			{
				return this.floatlistLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in float points", null);
			break;
		case Link.Type.ControlPoints:
			if (this.ctrlpointsLookup.ContainsKey(link.name))
			{
				return this.ctrlpointsLookup[link.name];
			}
			global::Debug.LogError("Couldnt find [" + link.name + "] in control points", null);
			break;
		case Link.Type.Terminator:
			if (this.terminator == null)
			{
				this.terminator = (TerminalNodeEditor)Node.Create("terminalNodeEditor", Vector2.zero);
				this.terminator.name = link.name;
			}
			return this.terminator;
		}
		global::Debug.LogError(string.Concat(new string[]
		{
			"Couldnt find link [",
			link.name,
			"] [",
			link.type.ToString(),
			"]"
		}), null);
		return null;
	}

	private static NoiseNodeCanvas PopulateNoiseNodeEditor(Tree tree)
	{
		NoiseNodeCanvas noiseNodeCanvas = NoiseNodeCanvas.CreateInstance();
		NodeEditor.curNodeCanvas = noiseNodeCanvas;
		noiseNodeCanvas.Populate(tree);
		return noiseNodeCanvas;
	}

	private void Populate(Tree tree)
	{
		this.settings = tree.settings;
		this.primitiveLookup.Clear();
		foreach (KeyValuePair<string, Primitive> keyValuePair in tree.primitives)
		{
			PrimitiveNodeEditor primitiveNodeEditor = (PrimitiveNodeEditor)Node.Create("primitiveNodeEditor", keyValuePair.Value.pos);
			primitiveNodeEditor.name = keyValuePair.Key;
			primitiveNodeEditor.target = keyValuePair.Value;
			this.primitiveLookup.Add(keyValuePair.Key, primitiveNodeEditor);
		}
		this.filterLookup.Clear();
		foreach (KeyValuePair<string, Filter> keyValuePair2 in tree.filters)
		{
			FilterNodeEditor filterNodeEditor = (FilterNodeEditor)Node.Create("filterNodeEditor", keyValuePair2.Value.pos);
			filterNodeEditor.name = keyValuePair2.Key;
			filterNodeEditor.target = keyValuePair2.Value;
			this.filterLookup.Add(keyValuePair2.Key, filterNodeEditor);
		}
		this.modifierLookup.Clear();
		foreach (KeyValuePair<string, ProcGen.Noise.Modifier> keyValuePair3 in tree.modifiers)
		{
			ModifierModuleNodeEditor modifierModuleNodeEditor = (ModifierModuleNodeEditor)Node.Create("modifierModuleNodeEditor", keyValuePair3.Value.pos);
			modifierModuleNodeEditor.name = keyValuePair3.Key;
			modifierModuleNodeEditor.target = keyValuePair3.Value;
			this.modifierLookup.Add(keyValuePair3.Key, modifierModuleNodeEditor);
		}
		this.selectorLookup.Clear();
		foreach (KeyValuePair<string, Selector> keyValuePair4 in tree.selectors)
		{
			SelectorModuleNodeEditor selectorModuleNodeEditor = (SelectorModuleNodeEditor)Node.Create("selectorModuleNodeEditor", keyValuePair4.Value.pos);
			selectorModuleNodeEditor.name = keyValuePair4.Key;
			selectorModuleNodeEditor.target = keyValuePair4.Value;
			this.selectorLookup.Add(keyValuePair4.Key, selectorModuleNodeEditor);
		}
		this.transformerLookup.Clear();
		foreach (KeyValuePair<string, Transformer> keyValuePair5 in tree.transformers)
		{
			TransformerNodeEditor transformerNodeEditor = (TransformerNodeEditor)Node.Create("transformerNodeEditor", keyValuePair5.Value.pos);
			transformerNodeEditor.name = keyValuePair5.Key;
			transformerNodeEditor.target = keyValuePair5.Value;
			this.transformerLookup.Add(keyValuePair5.Key, transformerNodeEditor);
		}
		this.combinerLookup.Clear();
		foreach (KeyValuePair<string, Combiner> keyValuePair6 in tree.combiners)
		{
			CombinerModuleNodeEditor combinerModuleNodeEditor = (CombinerModuleNodeEditor)Node.Create("combinerModuleNodeEditor", keyValuePair6.Value.pos);
			combinerModuleNodeEditor.name = keyValuePair6.Key;
			combinerModuleNodeEditor.target = keyValuePair6.Value;
			this.combinerLookup.Add(keyValuePair6.Key, combinerModuleNodeEditor);
		}
		this.floatlistLookup.Clear();
		foreach (KeyValuePair<string, FloatList> keyValuePair7 in tree.floats)
		{
			FloatPointsNodeEditor floatPointsNodeEditor = (FloatPointsNodeEditor)Node.Create("floatPointsNodeEditor", keyValuePair7.Value.pos);
			floatPointsNodeEditor.name = keyValuePair7.Key;
			floatPointsNodeEditor.target = keyValuePair7.Value;
			this.floatlistLookup.Add(keyValuePair7.Key, floatPointsNodeEditor);
		}
		this.ctrlpointsLookup.Clear();
		foreach (KeyValuePair<string, ControlPointList> keyValuePair8 in tree.controlpoints)
		{
			ControlPointsNodeEditor controlPointsNodeEditor = (ControlPointsNodeEditor)Node.Create("controlPointsNodeEditor", keyValuePair8.Value.pos);
			controlPointsNodeEditor.name = keyValuePair8.Key;
			controlPointsNodeEditor.target = keyValuePair8.Value;
			this.ctrlpointsLookup.Add(keyValuePair8.Key, controlPointsNodeEditor);
		}
		int i = 0;
		while (i < tree.links.Count)
		{
			NodeLink nodeLink = tree.links[i];
			Node nodeFromLink = this.GetNodeFromLink(nodeLink.target);
			Node node = null;
			Node node2 = null;
			Node node3 = null;
			Node node4 = null;
			switch (nodeLink.target.type)
			{
			case Link.Type.Filter:
			case Link.Type.Terminator:
				node = this.GetNodeFromLink(nodeLink.source0);
				break;
			case Link.Type.Transformer:
				node = this.GetNodeFromLink(nodeLink.source0);
				node2 = this.GetNodeFromLink(nodeLink.source1);
				node3 = this.GetNodeFromLink(nodeLink.source2);
				node4 = this.GetNodeFromLink(nodeLink.source3);
				break;
			case Link.Type.Selector:
			case Link.Type.Modifier:
				node = this.GetNodeFromLink(nodeLink.source0);
				node2 = this.GetNodeFromLink(nodeLink.source1);
				node3 = this.GetNodeFromLink(nodeLink.source2);
				break;
			case Link.Type.Combiner:
				node = this.GetNodeFromLink(nodeLink.source0);
				node2 = this.GetNodeFromLink(nodeLink.source1);
				break;
			}
			IL_05DA:
			if (node != null)
			{
				if (nodeFromLink.Inputs.Count == 0)
				{
					global::Debug.LogError(string.Concat(new object[]
					{
						"Target [",
						nodeFromLink.name,
						"][",
						nodeLink.target.type,
						"] doesnt have any inputs"
					}), null);
				}
				if (node.Outputs.Count == 0)
				{
					global::Debug.LogError(string.Concat(new object[]
					{
						"Source [",
						node.name,
						"][",
						nodeLink.source0.type,
						"] doesnt have any outputs"
					}), null);
				}
				nodeFromLink.Inputs[0].ApplyConnection(node.Outputs[0]);
			}
			if (node2 != null)
			{
				nodeFromLink.Inputs[1].ApplyConnection(node2.Outputs[0]);
			}
			if (node3 != null)
			{
				nodeFromLink.Inputs[2].ApplyConnection(node3.Outputs[0]);
			}
			if (node4 != null)
			{
				nodeFromLink.Inputs[3].ApplyConnection(node4.Outputs[0]);
			}
			i++;
			continue;
			goto IL_05DA;
		}
		this.UpdateTerminator();
	}

	private NoiseTreeFiles ntf;

	private TerminalNodeEditor terminator;

	private Rect lastRectPos;

	private Dictionary<string, PrimitiveNodeEditor> primitiveLookup = new Dictionary<string, PrimitiveNodeEditor>();

	private Dictionary<string, FilterNodeEditor> filterLookup = new Dictionary<string, FilterNodeEditor>();

	private Dictionary<string, ModifierModuleNodeEditor> modifierLookup = new Dictionary<string, ModifierModuleNodeEditor>();

	private Dictionary<string, SelectorModuleNodeEditor> selectorLookup = new Dictionary<string, SelectorModuleNodeEditor>();

	private Dictionary<string, TransformerNodeEditor> transformerLookup = new Dictionary<string, TransformerNodeEditor>();

	private Dictionary<string, CombinerModuleNodeEditor> combinerLookup = new Dictionary<string, CombinerModuleNodeEditor>();

	private Dictionary<string, FloatPointsNodeEditor> floatlistLookup = new Dictionary<string, FloatPointsNodeEditor>();

	private Dictionary<string, ControlPointsNodeEditor> ctrlpointsLookup = new Dictionary<string, ControlPointsNodeEditor>();
}
