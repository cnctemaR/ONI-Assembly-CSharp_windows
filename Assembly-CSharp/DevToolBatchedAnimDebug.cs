using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;

public class DevToolBatchedAnimDebug : DevTool
{
	public DevToolBatchedAnimDebug()
	{
		this.drawFlags = ImGuiWindowFlags.MenuBar;
	}

	protected override void Render()
	{
		if (ImGui.BeginMenuBar())
		{
			ImGui.Checkbox("Lock selection", ref this.LockSelection);
			ImGui.EndMenuBar();
		}
		if (!this.LockSelection)
		{
			SelectTool instance = SelectTool.Instance;
			GameObject gameObject;
			if (instance == null)
			{
				gameObject = null;
			}
			else
			{
				KSelectable selected = instance.selected;
				gameObject = ((selected != null) ? selected.gameObject : null);
			}
			this.Selection = gameObject;
		}
		if (this.Selection == null)
		{
			ImGui.Text("No selection.");
			return;
		}
		KBatchedAnimController component = this.Selection.GetComponent<KBatchedAnimController>();
		if (component == null)
		{
			ImGui.Text("No anim controller.");
			return;
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(component.batchGroupID);
		SymbolOverrideController component2 = this.Selection.GetComponent<SymbolOverrideController>();
		if (ImGui.BeginTabBar("##tabs", ImGuiTabBarFlags.None))
		{
			if (ImGui.BeginTabItem("BatchGroup"))
			{
				KAnimBatchGroup group = component.GetBatch().group;
				ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
				ImGui.Text(string.Format("Group mesh.vertices.Count: {0}", group.mesh.vertices.Count<Vector3>()));
				ImGui.Text(string.Format("Group data.maxVisibleSymbols: {0}", group.data.maxVisibleSymbols));
				ImGui.Text(string.Format("Group maxGroupSize: {0}", group.maxGroupSize));
				ImGui.EndChild();
				ImGui.EndTabItem();
			}
			if (component2 != null && ImGui.BeginTabItem("SymbolOverrides"))
			{
				ImGui.InputText("Symbol Filter", ref this.Filter, 128U);
				int num = Hash.SDBMLower(this.Filter);
				ImGui.LabelText("Filter Hash", "0x" + num.ToString("X"));
				SymbolOverrideController.SymbolEntry[] getSymbolOverrides = component2.GetSymbolOverrides;
				ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
				for (int i = 0; i < getSymbolOverrides.Length; i++)
				{
					SymbolOverrideController.SymbolEntry symbolEntry = getSymbolOverrides[i];
					KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(symbolEntry.targetSymbol);
					if (symbolEntry.targetSymbol.HashValue == num || symbolEntry.sourceSymbol.hash.HashValue == num || this.StringContains(symbolEntry.sourceSymbol.hash.ToString(), this.Filter) || this.StringContains(symbol.hash.ToString(), this.Filter))
					{
						ImGui.Text(string.Format("[{0}] source: {1}, {2}, ({3}), priority: {4}", new object[]
						{
							i,
							symbolEntry.sourceSymbol.hash,
							symbolEntry.sourceSymbol.build.name,
							symbolEntry.sourceSymbol.build.GetTexture(0).name,
							symbolEntry.priority
						}));
						ImGui.Text(string.Format("       firstFrameIdx = {0}, numFrames = {1}", symbolEntry.sourceSymbol.firstFrameIdx, symbolEntry.sourceSymbol.numFrames));
						ImGui.Text(string.Format("   target: {0}", symbol.hash));
						ImGui.Text(string.Format("       firstFrameIdx = {0}, numFrames = {1}", symbol.firstFrameIdx, symbol.numFrames));
					}
				}
				ImGui.EndChild();
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem("Texture atlases"))
			{
				ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
				List<Texture2D> list = new List<Texture2D>(component.GetBatch().atlases.GetTextures());
				int num2 = list.Count<Texture2D>();
				if (component2 != null)
				{
					list.AddRange(component2.GetAtlasList().GetTextures());
				}
				for (int j = 0; j < list.Count; j++)
				{
					Texture2D texture2D = list[j];
					string text = ((j >= num2) ? "symbol override" : "base");
					ImGui.Text(string.Format("[{0}]: {1}, [{2},{3}] ({4})", new object[] { j, texture2D.name, texture2D.width, texture2D.height, text }));
					if (ImGui.IsItemHovered())
					{
						ImGui.BeginTooltip();
						ImGuiEx.Image(texture2D, new Vector2((float)texture2D.width, (float)texture2D.height));
						ImGui.EndTooltip();
					}
				}
				ImGui.EndChild();
				ImGui.EndTabItem();
			}
			ImGui.EndTabBar();
		}
	}

	private bool StringContains(string target, string query)
	{
		return this.Filter == "" || target.IndexOf(query, 0, StringComparison.CurrentCultureIgnoreCase) != -1;
	}

	private GameObject Selection;

	private bool LockSelection;

	private string Filter = "";
}
