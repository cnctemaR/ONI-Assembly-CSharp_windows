using System;
using ImGuiNET;

public class DevToolMenuFontSize
{
	public bool initialized { get; private set; }

	public void RefreshFontSize()
	{
		DevToolMenuFontSize.FontSizeCategory @int = (DevToolMenuFontSize.FontSizeCategory)KPlayerPrefs.GetInt("Imgui_font_size_category", 2);
		this.SetFontSizeCategory(@int);
	}

	public void InitializeIfNeeded()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			this.RefreshFontSize();
		}
	}

	public void DrawMenu()
	{
		if (ImGui.BeginMenu("Settings"))
		{
			bool flag = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Fabric;
			bool flag2 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Small;
			bool flag3 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Regular;
			bool flag4 = this.fontSizeCategory == DevToolMenuFontSize.FontSizeCategory.Large;
			if (ImGui.BeginMenu("Size"))
			{
				if (ImGui.Checkbox("Original Font", ref flag) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Fabric)
				{
					this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Fabric);
				}
				if (ImGui.Checkbox("Small Text", ref flag2) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Small)
				{
					this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Small);
				}
				if (ImGui.Checkbox("Regular Text", ref flag3) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Regular)
				{
					this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Regular);
				}
				if (ImGui.Checkbox("Large Text", ref flag4) && this.fontSizeCategory != DevToolMenuFontSize.FontSizeCategory.Large)
				{
					this.SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory.Large);
				}
				ImGui.EndMenu();
			}
			ImGui.EndMenu();
		}
	}

	public unsafe void SetFontSizeCategory(DevToolMenuFontSize.FontSizeCategory size)
	{
		this.fontSizeCategory = size;
		KPlayerPrefs.SetInt("Imgui_font_size_category", (int)size);
		ImGuiIOPtr io = ImGui.GetIO();
		if (size < (DevToolMenuFontSize.FontSizeCategory)io.Fonts.Fonts.Size)
		{
			ImFontPtr imFontPtr = *io.Fonts.Fonts[(int)size];
			io.NativePtr->FontDefault = imFontPtr;
		}
	}

	public const string SETTINGS_KEY_FONT_SIZE_CATEGORY = "Imgui_font_size_category";

	private DevToolMenuFontSize.FontSizeCategory fontSizeCategory;

	public enum FontSizeCategory
	{
		Fabric,
		Small,
		Regular,
		Large
	}
}
