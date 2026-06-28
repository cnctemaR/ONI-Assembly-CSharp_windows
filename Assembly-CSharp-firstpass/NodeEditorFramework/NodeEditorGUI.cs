using System;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	public static class NodeEditorGUI
	{
		public static bool Init(bool GUIFunction)
		{
			NodeEditorGUI.Background = ResourceManager.LoadTexture("Textures/background.png");
			NodeEditorGUI.AALineTex = ResourceManager.LoadTexture("Textures/AALine.png");
			NodeEditorGUI.GUIBox = ResourceManager.LoadTexture("Textures/NE_Box.png");
			NodeEditorGUI.GUIButton = ResourceManager.LoadTexture("Textures/NE_Button.png");
			NodeEditorGUI.GUIBoxSelection = ResourceManager.LoadTexture("Textures/BoxSelection.png");
			bool flag;
			if (!NodeEditorGUI.Background || !NodeEditorGUI.AALineTex || !NodeEditorGUI.GUIBox || !NodeEditorGUI.GUIButton)
			{
				flag = false;
			}
			else if (!GUIFunction)
			{
				flag = true;
			}
			else
			{
				NodeEditorGUI.nodeSkin = global::UnityEngine.Object.Instantiate<GUISkin>(GUI.skin);
				NodeEditorGUI.nodeSkin.label.normal.textColor = NodeEditorGUI.NE_TextColor;
				NodeEditorGUI.nodeLabel = NodeEditorGUI.nodeSkin.label;
				NodeEditorGUI.nodeSkin.box.normal.textColor = NodeEditorGUI.NE_TextColor;
				NodeEditorGUI.nodeSkin.box.normal.background = NodeEditorGUI.GUIBox;
				NodeEditorGUI.nodeBox = NodeEditorGUI.nodeSkin.box;
				NodeEditorGUI.nodeSkin.button.normal.textColor = NodeEditorGUI.NE_TextColor;
				NodeEditorGUI.nodeSkin.button.normal.background = NodeEditorGUI.GUIButton;
				NodeEditorGUI.nodeSkin.textArea.normal.background = NodeEditorGUI.GUIBox;
				NodeEditorGUI.nodeSkin.textArea.active.background = NodeEditorGUI.GUIBox;
				NodeEditorGUI.nodeLabelBold = new GUIStyle(NodeEditorGUI.nodeLabel);
				NodeEditorGUI.nodeLabelBold.fontStyle = FontStyle.Bold;
				NodeEditorGUI.nodeLabelSelected = new GUIStyle(NodeEditorGUI.nodeLabel);
				NodeEditorGUI.nodeLabelSelected.normal.background = RTEditorGUI.ColorToTex(1, NodeEditorGUI.NE_LightColor);
				NodeEditorGUI.nodeBoxBold = new GUIStyle(NodeEditorGUI.nodeBox);
				NodeEditorGUI.nodeBoxBold.fontStyle = FontStyle.Bold;
				flag = true;
			}
			return flag;
		}

		public static void StartNodeGUI()
		{
			if (GUI.skin != NodeEditorGUI.defaultSkin)
			{
				if (NodeEditorGUI.nodeSkin == null)
				{
					NodeEditorGUI.Init(true);
				}
				GUI.skin = NodeEditorGUI.nodeSkin;
			}
			OverlayGUI.StartOverlayGUI();
		}

		public static void EndNodeGUI()
		{
			OverlayGUI.EndOverlayGUI();
			if (GUI.skin == NodeEditorGUI.defaultSkin)
			{
				GUI.skin = NodeEditorGUI.defaultSkin;
			}
		}

		public static void DrawConnection(Vector2 startPos, Vector2 endPos, Color col)
		{
			Vector2 vector = ((startPos.x > endPos.x) ? Vector2.left : Vector2.right);
			NodeEditorGUI.DrawConnection(startPos, vector, endPos, -vector, col);
		}

		public static void DrawConnection(Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, Color col)
		{
			NodeEditorGUI.DrawConnection(startPos, startDir, endPos, endDir, ConnectionDrawMethod.Bezier, col);
		}

		public static void DrawConnection(Vector2 startPos, Vector2 startDir, Vector2 endPos, Vector2 endDir, ConnectionDrawMethod drawMethod, Color col)
		{
			if (drawMethod == ConnectionDrawMethod.Bezier)
			{
				float num = 80f;
				RTEditorGUI.DrawBezier(startPos, endPos, startPos + startDir * num, endPos + endDir * num, col * Color.gray, null, 3f);
			}
			else if (drawMethod == ConnectionDrawMethod.StraightLine)
			{
				RTEditorGUI.DrawLine(startPos, endPos, col * Color.gray, null, 3f);
			}
		}

		internal static Vector2 GetSecondConnectionVector(Vector2 startPos, Vector2 endPos, Vector2 firstVector)
		{
			Vector2 vector;
			if (firstVector.x != 0f && firstVector.y == 0f)
			{
				vector = ((startPos.x > endPos.x) ? firstVector : (-firstVector));
			}
			else if (firstVector.y != 0f && firstVector.x == 0f)
			{
				vector = ((startPos.y > endPos.y) ? firstVector : (-firstVector));
			}
			else
			{
				vector = -firstVector;
			}
			return vector;
		}

		public static int knobSize = 16;

		public static Color NE_LightColor = new Color(0.4f, 0.4f, 0.4f);

		public static Color NE_TextColor = new Color(0.7f, 0.7f, 0.7f);

		public static Texture2D Background;

		public static Texture2D AALineTex;

		public static Texture2D GUIBox;

		public static Texture2D GUIButton;

		public static Texture2D GUIBoxSelection;

		public static GUISkin nodeSkin;

		public static GUISkin defaultSkin;

		public static GUIStyle nodeLabel;

		public static GUIStyle nodeLabelBold;

		public static GUIStyle nodeLabelSelected;

		public static GUIStyle nodeBox;

		public static GUIStyle nodeBoxBold;
	}
}
