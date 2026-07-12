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

	protected override void RenderTo(DevPanel panel)
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
		ImGui.Text("Group: " + component.GetBatch().group.batchID.ToString() + ", Build: " + component.curBuild.name);
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
						if (symbol != null)
						{
							ImGui.Text(string.Format("   target: {0}", symbol.hash));
							ImGui.Text(string.Format("       firstFrameIdx = {0}, numFrames = {1}", symbol.firstFrameIdx, symbol.numFrames));
						}
						else
						{
							ImGui.Text(string.Format("   target: does not contain the symbol '{0}' to override", symbolEntry.sourceSymbol.hash));
						}
					}
				}
				ImGui.EndChild();
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem("Build Symbols"))
			{
				ImGui.InputText("Symbol Filter", ref this.Filter, 128U);
				int num2 = Hash.SDBMLower(this.Filter);
				ImGui.LabelText("Filter Hash", "0x" + num2.ToString("X"));
				ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
				KBatchGroupData data = component.GetBatch().group.data;
				for (int j = 0; j < data.GetSymbolCount(); j++)
				{
					KAnim.Build.Symbol symbol2 = data.GetSymbol(j);
					if (symbol2.hash.HashValue == num2 || this.StringContains(symbol2.hash.ToString(), this.Filter))
					{
						ImGui.Text(string.Format("[{0}]: {1}", symbol2.symbolIndexInSourceBuild, symbol2.hash));
					}
				}
				ImGui.EndChild();
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem("Anim Frame Data"))
			{
				ImGui.Text("Current anim: " + component.CurrentAnim.name);
				ImGui.Text("Current frame index: " + component.GetCurrentFrameIndex().ToString());
				ImGuiEx.InputIntRange("Frame Index", ref this.FrameIndex, 0, batchGroupData.GetAnimFrames().Count - 1);
				KAnim.Anim.Frame frame;
				batchGroupData.TryGetFrame(this.FrameIndex, out frame);
				ImGui.Text(string.Format("Frame [{0}]: firstElementIdx= {1} numElements= {2}", this.FrameIndex, frame.firstElementIdx, frame.numElements));
				ImGui.Text("Frame Elements: ");
				for (int k = 0; k < frame.numElements; k++)
				{
					KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + k);
					int symbolIndex = batchGroupData.GetSymbolIndex(frameElement.symbol);
					ImGui.Text(string.Format("FrameElement [{0}]: symbolIdx= {1} symbol= {2}", frame.firstElementIdx + k, symbolIndex, frameElement.symbol));
				}
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem("Texture atlases"))
			{
				ImGui.BeginChild("ScrollRegion", new Vector2(0f, 0f), true, ImGuiWindowFlags.None);
				List<Texture2D> list = new List<Texture2D>(component.GetBatch().atlases.GetTextures());
				int num3 = list.Count<Texture2D>();
				if (component2 != null)
				{
					list.AddRange(component2.GetAtlasList().GetTextures());
				}
				for (int l = 0; l < list.Count; l++)
				{
					Texture2D texture2D = list[l];
					string text = ((l >= num3) ? "symbol override" : "base");
					ImGui.Text(string.Format("[{0}]: {1}, [{2},{3}] ({4})", new object[] { l, texture2D.name, texture2D.width, texture2D.height, text }));
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

	private int FrameIndex;
}
