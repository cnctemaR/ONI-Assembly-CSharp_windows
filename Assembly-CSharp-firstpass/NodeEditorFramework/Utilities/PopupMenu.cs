using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public class PopupMenu
	{
		public PopupMenu()
		{
			this.SetupGUI();
		}

		public void SetupGUI()
		{
			PopupMenu.backgroundStyle = new GUIStyle(GUI.skin.box);
			PopupMenu.backgroundStyle.contentOffset = new Vector2(2f, 2f);
			PopupMenu.expandRight = ResourceManager.LoadTexture("Textures/expandRight.png");
			PopupMenu.itemHeight = GUI.skin.label.CalcHeight(new GUIContent("text"), 100f);
			PopupMenu.selectedLabel = new GUIStyle(GUI.skin.label);
			PopupMenu.selectedLabel.normal.background = RTEditorGUI.ColorToTex(1, new Color(0.4f, 0.4f, 0.4f));
		}

		public void Show(Vector2 pos, float MinWidth = 40f)
		{
			this.minWidth = MinWidth;
			this.position = PopupMenu.calculateRect(pos, this.menuItems, this.minWidth);
			this.selectedPath = string.Empty;
			OverlayGUI.currentPopup = this;
		}

		public Vector2 Position
		{
			get
			{
				return this.position.position;
			}
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunctionData func, object userData)
		{
			string text;
			PopupMenu.MenuItem menuItem = this.AddHierarchy(ref content, out text);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new PopupMenu.MenuItem(text, content, func, userData));
			}
			else
			{
				this.menuItems.Add(new PopupMenu.MenuItem(text, content, func, userData));
			}
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunction func)
		{
			string text;
			PopupMenu.MenuItem menuItem = this.AddHierarchy(ref content, out text);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new PopupMenu.MenuItem(text, content, func));
			}
			else
			{
				this.menuItems.Add(new PopupMenu.MenuItem(text, content, func));
			}
		}

		public void AddSeparator(string path)
		{
			GUIContent guicontent = new GUIContent(path);
			PopupMenu.MenuItem menuItem = this.AddHierarchy(ref guicontent, out path);
			if (menuItem != null)
			{
				menuItem.subItems.Add(new PopupMenu.MenuItem());
			}
			else
			{
				this.menuItems.Add(new PopupMenu.MenuItem());
			}
		}

		private PopupMenu.MenuItem AddHierarchy(ref GUIContent content, out string path)
		{
			path = content.text;
			if (path.Contains("/"))
			{
				string[] array = path.Split(new char[] { '/' });
				string folderPath = array[0];
				PopupMenu.MenuItem menuItem = this.menuItems.Find((PopupMenu.MenuItem item) => item.content != null && item.content.text == folderPath && item.group);
				if (menuItem == null)
				{
					this.menuItems.Add(menuItem = new PopupMenu.MenuItem(folderPath, new GUIContent(folderPath), true));
				}
				for (int i = 1; i < array.Length - 1; i++)
				{
					string folder = array[i];
					folderPath = folderPath + "/" + folder;
					if (menuItem == null)
					{
						global::Debug.LogError("Parent is null!");
					}
					else if (menuItem.subItems == null)
					{
						global::Debug.LogError("Subitems of " + menuItem.content.text + " is null!");
					}
					PopupMenu.MenuItem menuItem2 = menuItem.subItems.Find((PopupMenu.MenuItem item) => item.content != null && item.content.text == folder && item.group);
					if (menuItem2 == null)
					{
						menuItem.subItems.Add(menuItem2 = new PopupMenu.MenuItem(folderPath, new GUIContent(folder), true));
					}
					menuItem = menuItem2;
				}
				path = content.text;
				content = new GUIContent(array[array.Length - 1], content.tooltip);
				return menuItem;
			}
			return null;
		}

		public void Draw()
		{
			bool flag = this.DrawGroup(this.position, this.menuItems);
			while (this.groupToDraw != null && !this.close)
			{
				PopupMenu.MenuItem menuItem = this.groupToDraw;
				this.groupToDraw = null;
				if (menuItem.group && this.DrawGroup(menuItem.groupPos, menuItem.subItems))
				{
					flag = true;
				}
			}
			if (!flag || this.close)
			{
				OverlayGUI.currentPopup = null;
			}
			NodeEditor.RepaintClients();
		}

		private bool DrawGroup(Rect pos, List<PopupMenu.MenuItem> menuItems)
		{
			Rect rect = PopupMenu.calculateRect(pos.position, menuItems, this.minWidth);
			Rect rect2 = new Rect(rect);
			rect2.xMax += 20f;
			rect2.xMin -= 20f;
			rect2.yMax += 20f;
			rect2.yMin -= 20f;
			bool flag = rect2.Contains(Event.current.mousePosition);
			this.currentItemHeight = PopupMenu.backgroundStyle.contentOffset.y;
			GUI.BeginGroup(PopupMenu.extendRect(rect, PopupMenu.backgroundStyle.contentOffset), GUIContent.none, PopupMenu.backgroundStyle);
			for (int i = 0; i < menuItems.Count; i++)
			{
				this.DrawItem(menuItems[i], rect);
				if (this.close)
				{
					break;
				}
			}
			GUI.EndGroup();
			return flag;
		}

		private void DrawItem(PopupMenu.MenuItem item, Rect groupRect)
		{
			if (item.separator)
			{
				if (Event.current.type == EventType.Repaint)
				{
					RTEditorGUI.Seperator(new Rect(PopupMenu.backgroundStyle.contentOffset.x + 1f, this.currentItemHeight + 1f, groupRect.width - 2f, 1f));
				}
				this.currentItemHeight += 3f;
			}
			else
			{
				Rect rect = new Rect(PopupMenu.backgroundStyle.contentOffset.x, this.currentItemHeight, groupRect.width, PopupMenu.itemHeight);
				if (rect.Contains(Event.current.mousePosition))
				{
					this.selectedPath = item.path;
				}
				bool flag = this.selectedPath == item.path || this.selectedPath.Contains(item.path + "/");
				GUI.Label(rect, item.content, (!flag) ? GUI.skin.label : PopupMenu.selectedLabel);
				if (item.group)
				{
					GUI.DrawTexture(new Rect(rect.x + rect.width - 12f, rect.y + (rect.height - 12f) / 2f, 12f, 12f), PopupMenu.expandRight);
					if (flag)
					{
						item.groupPos = new Rect(groupRect.x + groupRect.width + 4f, groupRect.y + this.currentItemHeight - 2f, 0f, 0f);
						this.groupToDraw = item;
					}
				}
				else if (flag && (Event.current.type == EventType.MouseDown || (Event.current.button != 1 && Event.current.type == EventType.MouseUp)))
				{
					item.Execute();
					this.close = true;
					Event.current.Use();
				}
				this.currentItemHeight += PopupMenu.itemHeight;
			}
		}

		private static Rect extendRect(Rect rect, Vector2 extendValue)
		{
			rect.x -= extendValue.x;
			rect.y -= extendValue.y;
			rect.width += extendValue.x + extendValue.x;
			rect.height += extendValue.y + extendValue.y;
			return rect;
		}

		private static Rect calculateRect(Vector2 position, List<PopupMenu.MenuItem> menuItems, float minWidth)
		{
			float num = minWidth;
			float num2 = 0f;
			for (int i = 0; i < menuItems.Count; i++)
			{
				PopupMenu.MenuItem menuItem = menuItems[i];
				if (menuItem.separator)
				{
					num2 += 3f;
				}
				else
				{
					num = Mathf.Max(num, GUI.skin.label.CalcSize(menuItem.content).x + (float)((!menuItem.group) ? 10 : 22));
					num2 += PopupMenu.itemHeight;
				}
			}
			Vector2 vector = new Vector2(num, num2);
			bool flag = position.y + vector.y <= (float)Screen.height;
			return new Rect(position.x, position.y - ((!flag) ? vector.y : 0f), vector.x, vector.y);
		}

		public List<PopupMenu.MenuItem> menuItems = new List<PopupMenu.MenuItem>();

		private Rect position;

		private string selectedPath;

		private PopupMenu.MenuItem groupToDraw;

		private float currentItemHeight;

		private bool close;

		public static GUIStyle backgroundStyle;

		public static Texture2D expandRight;

		public static float itemHeight;

		public static GUIStyle selectedLabel;

		public float minWidth;

		public delegate void MenuFunction();

		public delegate void MenuFunctionData(object userData);

		public class MenuItem
		{
			public MenuItem()
			{
				this.separator = true;
			}

			public MenuItem(string _path, GUIContent _content, bool _group)
			{
				this.path = _path;
				this.content = _content;
				this.group = _group;
				if (this.group)
				{
					this.subItems = new List<PopupMenu.MenuItem>();
				}
			}

			public MenuItem(string _path, GUIContent _content, PopupMenu.MenuFunction _func)
			{
				this.path = _path;
				this.content = _content;
				this.func = _func;
			}

			public MenuItem(string _path, GUIContent _content, PopupMenu.MenuFunctionData _func, object _userData)
			{
				this.path = _path;
				this.content = _content;
				this.funcData = _func;
				this.userData = _userData;
			}

			public void Execute()
			{
				if (this.funcData != null)
				{
					this.funcData(this.userData);
				}
				else if (this.func != null)
				{
					this.func();
				}
			}

			public string path;

			public GUIContent content;

			public PopupMenu.MenuFunction func;

			public PopupMenu.MenuFunctionData funcData;

			public object userData;

			public bool separator;

			public bool group;

			public Rect groupPos;

			public List<PopupMenu.MenuItem> subItems;
		}
	}
}
