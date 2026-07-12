using System;
using ImGuiNET;
using UnityEngine;

public class ImGuiTest : MonoBehaviour
{
	private void Update()
	{
		ImGui.Begin("HELLO HELLO HELLO");
		ImGui.Text("WTF STRING AFLASKDFLK");
		ImGui.Text("This is some useful text.");
		ImGui.Checkbox("Demo Window", ref this.show_demo_window);
		ImGui.Checkbox("Another Window", ref this.show_another_window);
		ImGui.SliderFloat("float", ref this.f, 0f, 1f);
		ImGui.ColorEdit3("clear color", ref this.clear_color);
		if (ImGui.Button("Button"))
		{
			this.counter++;
		}
		ImGui.SameLine();
		ImGui.Text(string.Format("counter = {0}", this.counter));
		ImGui.End();
	}

	private bool show_demo_window;

	private bool show_another_window;

	private int counter;

	private float f;

	private Vector3 clear_color;
}
