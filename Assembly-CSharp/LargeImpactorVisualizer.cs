using System;
using UnityEngine;

public class LargeImpactorVisualizer : KMonoBehaviour
{
	public bool Visible
	{
		get
		{
			return this.Active && !this.Folded;
		}
	}

	public bool Folded { get; private set; } = true;

	public float LastTimeSetToFolded { get; private set; }

	public bool ShouldResetEntryEffect { get; private set; }

	public float EntryEffectDuration { get; private set; } = 3f;

	public float FoldEffectDuration { get; private set; } = 1f;

	public void BeginEntryEffect(float duration)
	{
		this.EntryEffectDuration = duration;
		this.SetShouldResetEntryEffect(true);
	}

	public void SetShouldResetEntryEffect(bool shouldIt)
	{
		this.ShouldResetEntryEffect = shouldIt;
	}

	public void SetFoldedState(bool shouldBeFolded)
	{
		if (!this.Folded && shouldBeFolded)
		{
			this.LastTimeSetToFolded = Time.unscaledTime;
			if (this.Active)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Demolior_LandingZone_close_fx", false));
			}
		}
		this.Folded = shouldBeFolded;
		if (!shouldBeFolded)
		{
			this.LastTimeSetToFolded = float.MaxValue;
		}
	}

	public bool Active;

	private const string SFX_Fold = "HUD_Demolior_LandingZone_close_fx";

	public Vector2I OriginOffset;

	public Vector2 ScreenSpaceNotificationTogglePosition = Vector2.zero;

	public Vector2I RangeMin;

	public Vector2I RangeMax;

	public Vector2I TexSize = new Vector2I(64, 64);

	public bool TestLineOfSight;

	public bool BlockingTileVisible;

	public Func<int, bool> BlockingVisibleCb;

	public Func<int, bool> BlockingCb = new Func<int, bool>(Grid.IsSolidCell);

	public bool AllowLineOfSightInvalidCells;
}
