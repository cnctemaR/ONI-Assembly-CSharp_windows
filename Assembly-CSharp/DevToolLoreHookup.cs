using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

public class DevToolLoreHookup : DevTool
{
	public DevToolLoreHookup()
	{
		this.RequiresGameRunning = true;
	}

	private void ValidateUnlockId(string unlockId)
	{
		this.cachedUnlockId = unlockId;
		if (unlockId.IsNullOrWhiteSpace())
		{
			this.unlockIdStatus = null;
			return;
		}
		foreach (KeyValuePair<string, string[]> keyValuePair in Game.Instance.unlocks.lockCollections)
		{
			string[] value = keyValuePair.Value;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == unlockId)
				{
					this.unlockIdStatus = "[" + keyValuePair.Key + "]";
					this.unlockIdStatusColor = new Vector4(0.5f, 1f, 0.5f, 1f);
					return;
				}
			}
		}
		if (CodexCache.GetEntryForLock(unlockId) != null)
		{
			this.unlockIdStatus = "[codex]";
			this.unlockIdStatusColor = new Vector4(0.5f, 0.8f, 1f, 1f);
			return;
		}
		this.unlockIdStatus = "[unknown ID]";
		this.unlockIdStatusColor = new Vector4(1f, 0.4f, 0.4f, 1f);
	}

	private void ValidateNextCollectionId(string collectionId)
	{
		this.cachedNextCollectionId = collectionId;
		if (collectionId.IsNullOrWhiteSpace())
		{
			this.nextCollectionIdStatus = null;
			return;
		}
		string[] array;
		if (Game.Instance.unlocks.lockCollections.TryGetValue(collectionId, out array))
		{
			int num = array.Length;
			this.nextCollectionIdStatus = string.Format("[{0} entries]", num);
			this.nextCollectionIdStatusColor = new Vector4(0.5f, 1f, 0.5f, 1f);
			return;
		}
		this.nextCollectionIdStatus = "[unknown collection]";
		this.nextCollectionIdStatusColor = new Vector4(1f, 0.4f, 0.4f, 1f);
	}

	private void ValidateDisplayText(string displayText)
	{
		this.cachedDisplayText = displayText;
		if (displayText.IsNullOrWhiteSpace())
		{
			this.displayTextPreview = null;
			return;
		}
		StringEntry stringEntry;
		if (Strings.TryGet(displayText, out stringEntry))
		{
			this.displayTextValid = true;
			this.displayTextPreview = stringEntry.String;
			return;
		}
		this.displayTextValid = false;
		this.displayTextPreview = "[string key not found]";
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (SelectTool.Instance == null || SelectTool.Instance.selected == null)
		{
			ImGui.Text("Select an entity in-game.");
			return;
		}
		GameObject gameObject = SelectTool.Instance.selected.gameObject;
		LoreBearer component = gameObject.GetComponent<LoreBearer>();
		ImGui.Text("Selected: " + gameObject.name);
		ImGui.Separator();
		if (component == null)
		{
			ImGui.Text("No LoreBearer component on this entity.");
			return;
		}
		ImGui.Text("LoreBearer Fields");
		ImGui.Separator();
		string text = component.poiOverrideLoreUnlockId ?? "";
		bool flag = false;
		if (ImGui.InputText("Override Unlock ID", ref text, 256U))
		{
			component.poiOverrideLoreUnlockId = (string.IsNullOrWhiteSpace(text) ? null : text);
			flag = true;
		}
		if (text != this.cachedUnlockId)
		{
			this.ValidateUnlockId(text);
		}
		if (this.unlockIdStatus != null)
		{
			ImGui.SameLine();
			ImGui.TextColored(this.unlockIdStatusColor, this.unlockIdStatus);
		}
		string text2 = component.poiOverrideLoreDisplayText ?? "";
		if (ImGui.InputText("Override Display Text (string key)", ref text2, 1024U))
		{
			component.poiOverrideLoreDisplayText = (string.IsNullOrWhiteSpace(text2) ? null : text2);
			flag = true;
		}
		if (text2 != this.cachedDisplayText)
		{
			this.ValidateDisplayText(text2);
		}
		if (this.displayTextPreview != null)
		{
			if (this.displayTextValid)
			{
				ImGui.TextWrapped(this.displayTextPreview);
			}
			else
			{
				ImGui.SameLine();
				ImGui.TextColored(new Vector4(1f, 0.4f, 0.4f, 1f), this.displayTextPreview);
			}
		}
		string text3 = component.poiOverrideNextCollectionId ?? "";
		if (ImGui.InputText("Override Next Collection ID (Optional)", ref text3, 256U))
		{
			component.poiOverrideNextCollectionId = (string.IsNullOrWhiteSpace(text3) ? null : text3);
			flag = true;
		}
		if (text3 != this.cachedNextCollectionId)
		{
			this.ValidateNextCollectionId(text3);
		}
		if (this.nextCollectionIdStatus != null)
		{
			ImGui.SameLine();
			ImGui.TextColored(this.nextCollectionIdStatusColor, this.nextCollectionIdStatus);
		}
		if (flag)
		{
			if (!string.IsNullOrEmpty(component.poiOverrideLoreUnlockId))
			{
				if (!string.IsNullOrEmpty(component.poiOverrideNextCollectionId))
				{
					component.Internal_SetContent(LoreBearerUtil.UnlockSpecificEntryThenNext(component.poiOverrideLoreUnlockId, component.poiOverrideLoreDisplayText, LoreBearerUtil.GetUnlockActionForCollection(component.poiOverrideNextCollectionId), false));
				}
				else
				{
					component.Internal_SetContent(LoreBearerUtil.UnlockSpecificEntry(component.poiOverrideLoreUnlockId, Strings.Get(component.poiOverrideLoreDisplayText), false));
				}
			}
			else
			{
				component.Internal_SetContent(null);
			}
		}
		ImGui.Separator();
		if (ImGui.Button("Make Uninspected"))
		{
			component.Debug_ResetSearched();
		}
		ImGui.SameLine();
		if (ImGui.Button("Clear Override"))
		{
			component.poiOverrideLoreUnlockId = null;
			component.poiOverrideLoreDisplayText = null;
			component.poiOverrideNextCollectionId = null;
			component.Internal_SetContent(null);
		}
	}

	private string cachedUnlockId;

	private string unlockIdStatus;

	private Vector4 unlockIdStatusColor;

	private string cachedDisplayText;

	private string displayTextPreview;

	private bool displayTextValid;

	private string cachedNextCollectionId;

	private string nextCollectionIdStatus;

	private Vector4 nextCollectionIdStatusColor;
}
