using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeTypes
	{
		public static void FetchNodes()
		{
			NodeTypes.nodes = new Dictionary<Node, NodeData>();
			foreach (Assembly assembly2 in from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly)
			{
				foreach (Type type in from T in assembly2.GetTypes()
					where T.IsClass && !T.IsAbstract && T.IsSubclassOf(typeof(Node))
					select T)
				{
					NodeAttribute nodeAttribute = type.GetCustomAttributes(typeof(NodeAttribute), false)[0] as NodeAttribute;
					if (nodeAttribute == null || !nodeAttribute.hide)
					{
						try
						{
							Node node = ScriptableObject.CreateInstance(type.Name) as Node;
							node = node.Create(Vector2.zero);
							NodeTypes.nodes.Add(node, new NodeData((nodeAttribute == null) ? node.name : nodeAttribute.contextText, nodeAttribute.typeOfNodeCanvas));
						}
						catch (Exception ex)
						{
							global::Debug.LogError(ex.Message + " " + type.Name);
						}
					}
				}
			}
		}

		public static NodeData getNodeData(Node node)
		{
			return NodeTypes.nodes[NodeTypes.getDefaultNode(node.GetID)];
		}

		public static Node getDefaultNode(string nodeID)
		{
			return NodeTypes.nodes.Keys.Single<Node>((Node node) => node.GetID == nodeID);
		}

		public static T getDefaultNode<T>() where T : Node
		{
			return NodeTypes.nodes.Keys.Single<Node>((Node node) => node.GetType() == typeof(T)) as T;
		}

		public static List<Node> getCompatibleNodes(NodeOutput nodeOutput)
		{
			if (nodeOutput == null)
			{
				throw new ArgumentNullException("nodeOutput");
			}
			List<Node> list = new List<Node>();
			foreach (Node node in NodeTypes.nodes.Keys)
			{
				for (int i = 0; i < node.Inputs.Count; i++)
				{
					NodeInput nodeInput = node.Inputs[i];
					if (nodeInput == null)
					{
						throw new UnityException("Input " + i + " is null!");
					}
					if (nodeInput.typeData.Type.IsAssignableFrom(nodeOutput.typeData.Type))
					{
						list.Add(node);
						break;
					}
				}
			}
			return list;
		}

		public static Dictionary<Node, NodeData> nodes;
	}
}
