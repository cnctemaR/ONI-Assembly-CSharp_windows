using System;

public class SetDefaults
{
	public static void Initialize()
	{
		KButton.DefaultSounds[KButton.SoundType.OnMouseOver] = GlobalAssets.GetSound("HUD_Mouseover", false);
		KButton.DefaultSounds[KButton.SoundType.OnMouseClick] = GlobalAssets.GetSound("HUD_Click", false);
		KButton.DefaultSounds[KButton.SoundType.OnMouseClickNegative] = GlobalAssets.GetSound("Negative", false);
		KToggle.DefaultSounds[KToggle.SoundType.OnMouseOver] = GlobalAssets.GetSound("HUD_Mouseover", false);
		KScrollRect.DefaultSounds[KScrollRect.SoundType.OnMouseScroll] = GlobalAssets.GetSound("Mousewheel_Move", false);
	}
}
