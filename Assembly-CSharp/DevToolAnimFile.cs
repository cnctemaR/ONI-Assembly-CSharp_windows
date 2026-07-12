using System;

public class DevToolAnimFile : DevTool
{
	public DevToolAnimFile(KAnimFile animFile)
	{
		this.animFile = animFile;
		this.Name = "Anim File: \"" + animFile.name + "\"";
	}

	protected override void RenderTo(DevPanel panel)
	{
		ImGuiEx.DrawObject(this.animFile, null);
		ImGuiEx.DrawObject(this.animFile.GetData(), null);
	}

	private KAnimFile animFile;
}
