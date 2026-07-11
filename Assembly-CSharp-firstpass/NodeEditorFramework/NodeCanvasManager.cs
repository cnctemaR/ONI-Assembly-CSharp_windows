using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public class NodeCanvasManager
	{
		public static void GetAllCanvasTypes()
		{
			NodeCanvasManager.TypeOfCanvases = new Dictionary<Type, NodeCanvasTypeData>();
			foreach (Assembly assembly2 in from assembly in AppDomain.CurrentDomain.GetAssemblies()
				where assembly.FullName.Contains("Assembly")
				select assembly)
			{
				foreach (Type type in from T in assembly2.GetTypes()
					where T.IsClass && !T.IsAbstract && T.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), false).Length != 0
					select T)
				{
					NodeCanvasTypeAttribute nodeCanvasTypeAttribute = type.GetCustomAttributes(typeof(NodeCanvasTypeAttribute), false)[0] as NodeCanvasTypeAttribute;
					NodeCanvasManager.TypeOfCanvases.Add(type, new NodeCanvasTypeData
					{
						CanvasType = type,
						DisplayString = nodeCanvasTypeAttribute.Name
					});
				}
			}
		}

		private static void CreateNewCanvas(object userdata)
		{
			NodeCanvasTypeData nodeCanvasTypeData = (NodeCanvasTypeData)userdata;
			NodeCanvasManager._callBack(nodeCanvasTypeData.CanvasType);
		}

		public static void PopulateMenu(ref GenericMenu menu, Action<Type> newNodeCanvas)
		{
			NodeCanvasManager._callBack = newNodeCanvas;
			foreach (KeyValuePair<Type, NodeCanvasTypeData> keyValuePair in NodeCanvasManager.TypeOfCanvases)
			{
				menu.AddItem(new GUIContent(keyValuePair.Value.DisplayString), false, new PopupMenu.MenuFunctionData(NodeCanvasManager.CreateNewCanvas), keyValuePair.Value);
			}
		}

		public static Dictionary<Type, NodeCanvasTypeData> TypeOfCanvases;

		private static Action<Type> _callBack;
	}
}
