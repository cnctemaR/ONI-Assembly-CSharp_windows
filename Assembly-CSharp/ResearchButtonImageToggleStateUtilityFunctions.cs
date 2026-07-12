using System;
using UnityEngine;
using UnityEngine.UI;

public static class ResearchButtonImageToggleStateUtilityFunctions
{
	public static void Opacity(this Graphic graphic, float opacity)
	{
		Color color = graphic.color;
		color.a = opacity;
		graphic.color = color;
	}

	public static WaitUntil FadeAway(this Graphic graphic, float duration, Func<bool> assertCondition = null)
	{
		float timer = 0f;
		float startingOpacity = graphic.color.a;
		return new WaitUntil(delegate
		{
			if (timer >= duration || (assertCondition != null && !assertCondition()))
			{
				graphic.Opacity(0f);
				return true;
			}
			float num = timer / duration;
			num = 1f - num;
			graphic.Opacity(startingOpacity * num);
			timer += Time.unscaledDeltaTime;
			return false;
		});
	}

	public static WaitUntil FadeToVisible(this Graphic graphic, float duration, Func<bool> assertCondition = null)
	{
		float timer = 0f;
		float startingOpacity = graphic.color.a;
		float remainingOpacity = 1f - graphic.color.a;
		return new WaitUntil(delegate
		{
			if (timer >= duration || (assertCondition != null && !assertCondition()))
			{
				graphic.Opacity(1f);
				return true;
			}
			float num = timer / duration;
			graphic.Opacity(startingOpacity + remainingOpacity * num);
			timer += Time.unscaledDeltaTime;
			return false;
		});
	}
}
