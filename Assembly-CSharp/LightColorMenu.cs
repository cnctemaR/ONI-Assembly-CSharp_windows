using System;
using UnityEngine;

public class LightColorMenu : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.SetColor(0);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (this.lightColors.Length > 0)
		{
			int num = this.lightColors.Length;
			for (int i = 0; i < num; i++)
			{
				if (i != this.currentColor)
				{
					int new_color = i;
					this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo(this.lightColors[i].name, this.lightColors[i].name, delegate
					{
						this.SetColor(new_color);
					}, global::Action.NumActions, null, null, null, string.Empty, true), 1f);
				}
			}
		}
	}

	private void SetColor(int color_index)
	{
		if (this.lightColors.Length > 0 && color_index < this.lightColors.Length)
		{
			foreach (Light2D light2D in base.GetComponentsInChildren<Light2D>(true))
			{
				light2D.Color = this.lightColors[color_index].color;
			}
			foreach (MeshRenderer meshRenderer in base.GetComponentsInChildren<MeshRenderer>(true))
			{
				foreach (Material material in meshRenderer.materials)
				{
					if (material.name.StartsWith("matScriptedGlow01"))
					{
						material.color = this.lightColors[color_index].color;
					}
				}
			}
		}
		this.currentColor = color_index;
	}

	public LightColorMenu.LightColor[] lightColors;

	private int currentColor;

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serializable]
	public struct LightColor
	{
		public LightColor(string name, Color color)
		{
			this.name = name;
			this.color = color;
		}

		public string name;

		public Color color;
	}
}
