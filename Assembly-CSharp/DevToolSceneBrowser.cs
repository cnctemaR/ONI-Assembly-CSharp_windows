using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevToolSceneBrowser : DevTool
{
	public DevToolSceneBrowser()
	{
		this.drawFlags = ImGuiWindowFlags.MenuBar;
		DevToolSceneBrowser.StackItem stackItem = new DevToolSceneBrowser.StackItem();
		stackItem.SceneRoot = true;
		stackItem.Filter = "";
		this.Stack.Add(stackItem);
	}

	private void PushGameObject(GameObject go)
	{
		if (this.StackIndex < this.Stack.Count && go == this.Stack[this.StackIndex].Root)
		{
			return;
		}
		if (this.Stack.Count > this.StackIndex + 1)
		{
			this.Stack.RemoveRange(this.StackIndex + 1, this.Stack.Count - (this.StackIndex + 1));
		}
		DevToolSceneBrowser.StackItem stackItem = new DevToolSceneBrowser.StackItem();
		stackItem.SceneRoot = go == null;
		stackItem.Root = go;
		stackItem.Filter = "";
		this.Stack.Add(stackItem);
		this.StackIndex++;
	}

	protected override void Render()
	{
		for (int i = this.Stack.Count - 1; i > 0; i--)
		{
			DevToolSceneBrowser.StackItem stackItem = this.Stack[i];
			if (!stackItem.SceneRoot && stackItem.Root.IsNullOrDestroyed())
			{
				this.Stack.RemoveAt(i);
				this.StackIndex = Math.Min(i - 1, this.StackIndex);
			}
		}
		bool flag = false;
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.BeginMenu("Utils"))
			{
				if (ImGui.MenuItem("Goto current selection"))
				{
					SelectTool instance = SelectTool.Instance;
					global::UnityEngine.Object @object;
					if (instance == null)
					{
						@object = null;
					}
					else
					{
						KSelectable selected = instance.selected;
						@object = ((selected != null) ? selected.gameObject : null);
					}
					if (@object != null)
					{
						SelectTool instance2 = SelectTool.Instance;
						GameObject gameObject;
						if (instance2 == null)
						{
							gameObject = null;
						}
						else
						{
							KSelectable selected2 = instance2.selected;
							gameObject = ((selected2 != null) ? selected2.gameObject : null);
						}
						this.PushGameObject(gameObject);
					}
				}
				if (ImGui.MenuItem("Search All"))
				{
					flag = true;
				}
				ImGui.EndMenu();
			}
			ImGui.EndMenuBar();
		}
		if (ImGui.Button(" < ") && this.StackIndex > 0)
		{
			this.StackIndex--;
		}
		ImGui.SameLine();
		if (ImGui.Button(" ^ ") && this.StackIndex > 0 && !this.Stack[this.StackIndex].SceneRoot)
		{
			Transform parent = this.Stack[this.StackIndex].Root.transform.parent;
			this.PushGameObject((parent != null) ? parent.gameObject : null);
		}
		ImGui.SameLine();
		if (ImGui.Button(" > ") && this.StackIndex + 1 < this.Stack.Count)
		{
			this.StackIndex++;
		}
		DevToolSceneBrowser.StackItem stackItem2 = this.Stack[this.StackIndex];
		if (!stackItem2.SceneRoot)
		{
			ImGui.SameLine();
			if (ImGui.Button("Inspect"))
			{
				DevToolManager.Instance.GetDevTool<DevToolSceneInspector>().PushObject(stackItem2.Root);
			}
		}
		List<GameObject> list;
		if (stackItem2.SceneRoot)
		{
			ImGui.Text("Scene root");
			Scene activeScene = SceneManager.GetActiveScene();
			list = new List<GameObject>(activeScene.rootCount);
			activeScene.GetRootGameObjects(list);
		}
		else
		{
			ImGui.LabelText("Selected object", stackItem2.Root.name);
			list = new List<GameObject>();
			foreach (object obj in stackItem2.Root.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.gameObject != stackItem2.Root)
				{
					list.Add(transform.gameObject);
				}
			}
		}
		if (ImGui.Button("Clear"))
		{
			stackItem2.Filter = "";
		}
		ImGui.SameLine();
		ImGui.InputText("Filter", ref stackItem2.Filter, 64U);
		ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
		for (int j = 0; j < list.Count; j++)
		{
			GameObject gameObject2 = list[j];
			if (!(stackItem2.Filter != "") || gameObject2.name.IndexOf(stackItem2.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
			{
				if (ImGui.Selectable(gameObject2.name, false, ImGuiSelectableFlags.AllowDoubleClick) && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
				{
					this.PushGameObject(gameObject2);
				}
				if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
				{
					DevToolSceneBrowser.SelectedIndex = j;
					ImGui.OpenPopup("RightClickMenu");
				}
			}
		}
		if (ImGui.BeginPopup("RightClickMenu"))
		{
			if (ImGui.MenuItem("Inspect"))
			{
				DevToolManager.Instance.GetDevTool<DevToolSceneInspector>().PushObject(list[DevToolSceneBrowser.SelectedIndex]);
				DevToolSceneBrowser.SelectedIndex = -1;
			}
			ImGui.EndPopup();
		}
		ImGui.EndChild();
		if (flag)
		{
			ImGui.OpenPopup("SearchAll");
		}
		if (ImGui.BeginPopupModal("SearchAll"))
		{
			ImGui.Text("Search all objects in the scene:");
			ImGui.Separator();
			if (ImGui.Button("Clear"))
			{
				DevToolSceneBrowser.SearchFilter = "";
			}
			ImGui.SameLine();
			if (ImGui.InputText("Filter", ref DevToolSceneBrowser.SearchFilter, 64U))
			{
				DevToolSceneBrowser.SearchResults = (from go in global::UnityEngine.Object.FindObjectsOfType<GameObject>()
					where go.name.IndexOf(DevToolSceneBrowser.SearchFilter, 0, StringComparison.CurrentCultureIgnoreCase) != -1
					orderby go.name
					select go).ToList<GameObject>();
			}
			ImGui.BeginChild("ScrollRegion", new Vector2(0f, 200f), true, ImGuiWindowFlags.None);
			int num = 0;
			using (List<GameObject>.Enumerator enumerator2 = DevToolSceneBrowser.SearchResults.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (ImGui.Selectable(enumerator2.Current.name, DevToolSceneBrowser.SearchSelectedIndex == num))
					{
						DevToolSceneBrowser.SearchSelectedIndex = num;
					}
					num++;
				}
			}
			ImGui.EndChild();
			bool flag2 = false;
			if (ImGui.Button("Browse") && DevToolSceneBrowser.SearchSelectedIndex >= 0)
			{
				this.PushGameObject(DevToolSceneBrowser.SearchResults[DevToolSceneBrowser.SearchSelectedIndex]);
				flag2 = true;
			}
			ImGui.SameLine();
			if (ImGui.Button("Inspect") && DevToolSceneBrowser.SearchSelectedIndex >= 0)
			{
				DevToolManager.Instance.GetDevTool<DevToolSceneInspector>().PushObject(DevToolSceneBrowser.SearchResults[DevToolSceneBrowser.SearchSelectedIndex]);
				flag2 = true;
			}
			ImGui.SameLine();
			if (ImGui.Button("Cancel"))
			{
				flag2 = true;
			}
			if (flag2)
			{
				DevToolSceneBrowser.SearchFilter = "";
				DevToolSceneBrowser.SearchResults.Clear();
				DevToolSceneBrowser.SearchSelectedIndex = -1;
				ImGui.CloseCurrentPopup();
			}
			ImGui.EndPopup();
		}
	}

	private List<DevToolSceneBrowser.StackItem> Stack = new List<DevToolSceneBrowser.StackItem>();

	private int StackIndex;

	private static int SelectedIndex = -1;

	private static string SearchFilter = "";

	private static List<GameObject> SearchResults = new List<GameObject>();

	private static int SearchSelectedIndex = -1;

	private class StackItem
	{
		public bool SceneRoot;

		public GameObject Root;

		public string Filter;
	}
}
