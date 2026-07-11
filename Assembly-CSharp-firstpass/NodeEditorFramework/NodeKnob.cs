using System;
using System.Linq;
using NodeEditorFramework.Utilities;
using UnityEngine;

namespace NodeEditorFramework
{
	[Serializable]
	public class NodeKnob : ScriptableObject
	{
		protected virtual GUIStyle defaultLabelStyle
		{
			get
			{
				return GUI.skin.label;
			}
		}

		protected virtual NodeSide defaultSide
		{
			get
			{
				return NodeSide.Right;
			}
		}

		protected void InitBase(Node nodeBody, NodeSide nodeSide, float nodeSidePosition, string knobName)
		{
			this.body = nodeBody;
			this.side = nodeSide;
			this.sidePosition = nodeSidePosition;
			base.name = knobName;
			nodeBody.nodeKnobs.Add(this);
			this.ReloadKnobTexture();
		}

		public virtual void Delete()
		{
			this.body.nodeKnobs.Remove(this);
			global::UnityEngine.Object.DestroyImmediate(this, true);
		}

		internal void Check()
		{
			if (this.side == (NodeSide)0)
			{
				this.side = this.defaultSide;
			}
			if (this.knobTexture == null)
			{
				this.ReloadKnobTexture();
			}
		}

		protected void ReloadKnobTexture()
		{
			this.ReloadTexture();
			if (this.knobTexture == null)
			{
				throw new UnityException("Knob texture of " + base.name + " could not be loaded!");
			}
			if (this.side != this.defaultSide)
			{
				ResourceManager.SetDefaultResourcePath(NodeEditor.editorPath + "Resources/");
				int rotationStepsAntiCW = NodeKnob.getRotationStepsAntiCW(this.defaultSide, this.side);
				ResourceManager.MemoryTexture memoryTexture = ResourceManager.FindInMemory(this.knobTexture);
				if (memoryTexture != null)
				{
					string[] array = new string[memoryTexture.modifications.Length + 1];
					memoryTexture.modifications.CopyTo(array, 0);
					array[array.Length - 1] = "Rotation:" + rotationStepsAntiCW;
					Texture2D texture = ResourceManager.GetTexture(memoryTexture.path, array);
					if (texture != null)
					{
						this.knobTexture = texture;
						return;
					}
					this.knobTexture = RTEditorGUI.RotateTextureCCW(this.knobTexture, rotationStepsAntiCW);
					ResourceManager.AddTextureToMemory(memoryTexture.path, this.knobTexture, array.ToArray<string>());
					return;
				}
				else
				{
					this.knobTexture = RTEditorGUI.RotateTextureCCW(this.knobTexture, rotationStepsAntiCW);
				}
			}
		}

		protected virtual void ReloadTexture()
		{
			this.knobTexture = RTEditorGUI.ColorToTex(1, Color.red);
		}

		public virtual ScriptableObject[] GetScriptableObjects()
		{
			return new ScriptableObject[0];
		}

		protected internal virtual void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSerializableObject)
		{
		}

		public virtual void DrawKnob()
		{
			GUI.DrawTexture(this.GetGUIKnob(), this.knobTexture);
		}

		public void DisplayLayout()
		{
			this.DisplayLayout(new GUIContent(base.name), this.defaultLabelStyle);
		}

		public void DisplayLayout(GUIStyle style)
		{
			this.DisplayLayout(new GUIContent(base.name), style);
		}

		public void DisplayLayout(GUIContent content)
		{
			this.DisplayLayout(content, this.defaultLabelStyle);
		}

		public void DisplayLayout(GUIContent content, GUIStyle style)
		{
			GUILayout.Label(content, style, Array.Empty<GUILayoutOption>());
			if (Event.current.type == EventType.Repaint)
			{
				this.SetPosition();
			}
		}

		public void SetPosition(float position, NodeSide nodeSide)
		{
			if (this.side != nodeSide)
			{
				this.side = nodeSide;
				this.ReloadKnobTexture();
			}
			this.SetPosition(position);
		}

		public void SetPosition(float position)
		{
			this.sidePosition = position;
		}

		public void SetPosition()
		{
			Vector2 vector = GUILayoutUtility.GetLastRect().center + this.body.contentOffset;
			this.sidePosition = ((this.side == NodeSide.Bottom || this.side == NodeSide.Top) ? vector.x : vector.y);
		}

		public Rect GetGUIKnob()
		{
			Rect canvasSpaceKnob = this.GetCanvasSpaceKnob();
			canvasSpaceKnob.position += NodeEditor.curEditorState.zoomPanAdjust + NodeEditor.curEditorState.panOffset;
			return canvasSpaceKnob;
		}

		public Rect GetCanvasSpaceKnob()
		{
			this.Check();
			Vector2 vector = new Vector2((float)(this.knobTexture.width / this.knobTexture.height * NodeEditorGUI.knobSize), (float)(this.knobTexture.height / this.knobTexture.width * NodeEditorGUI.knobSize));
			Vector2 knobCenter = this.GetKnobCenter(vector);
			return new Rect(knobCenter.x - vector.x / 2f, knobCenter.y - vector.y / 2f, vector.x, vector.y);
		}

		private Vector2 GetKnobCenter(Vector2 knobSize)
		{
			if (this.side == NodeSide.Left)
			{
				return this.body.rect.position + new Vector2(-this.sideOffset - knobSize.x / 2f, this.sidePosition);
			}
			if (this.side == NodeSide.Right)
			{
				return this.body.rect.position + new Vector2(this.sideOffset + knobSize.x / 2f + this.body.rect.width, this.sidePosition);
			}
			if (this.side == NodeSide.Bottom)
			{
				return this.body.rect.position + new Vector2(this.sidePosition, this.sideOffset + knobSize.y / 2f + this.body.rect.height);
			}
			return this.body.rect.position + new Vector2(this.sidePosition, -this.sideOffset - knobSize.y / 2f);
		}

		public Vector2 GetDirection()
		{
			if (this.side == NodeSide.Right)
			{
				return Vector2.right;
			}
			if (this.side == NodeSide.Bottom)
			{
				return Vector2.up;
			}
			if (this.side != NodeSide.Top)
			{
				return Vector2.left;
			}
			return Vector2.down;
		}

		private static int getRotationStepsAntiCW(NodeSide sideA, NodeSide sideB)
		{
			return sideB - sideA + ((sideA > sideB) ? 4 : 0);
		}

		public virtual Node GetNodeAcrossConnection()
		{
			return null;
		}

		public Node body;

		[NonSerialized]
		protected internal Texture2D knobTexture;

		public NodeSide side;

		public float sidePosition;

		public float sideOffset;
	}
}
