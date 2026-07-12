using System;

public class DevToolObjectViewer<T> : DevTool
{
	public DevToolObjectViewer(Func<T> getValue)
	{
		this.getValue = getValue;
		this.Name = typeof(T).Name;
	}

	protected override void RenderTo(DevPanel panel)
	{
		T t = this.getValue();
		this.Name = t.GetType().Name;
		ImGuiEx.DrawObject(t, null);
	}

	private Func<T> getValue;
}
