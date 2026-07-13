using System;
using System.Collections;
using UnityEngine;

public static class SequenceTools
{
	public static WaitUntil Interpolate(this MonoBehaviour owner, Action<float> action, float duration, global::System.Action then = null)
	{
		Coroutine coroutine;
		return owner.Interpolate(action, duration, out coroutine, then);
	}

	public static WaitUntil Interpolate(this MonoBehaviour owner, Action<float> action, float duration, out Coroutine coroutineOut, global::System.Action then = null)
	{
		bool completed = false;
		global::System.Action action2 = delegate
		{
			if (then != null)
			{
				then();
			}
			completed = true;
		};
		coroutineOut = owner.StartCoroutine(SequenceTools.InterpolateCoroutineLogic(action, duration, action2));
		return new WaitUntil(() => completed);
	}

	private static IEnumerator InterpolateCoroutineLogic(Action<float> action, float duration, global::System.Action then)
	{
		float timer = 0f;
		while (timer < duration)
		{
			float num = timer / duration;
			action(num);
			timer += Time.unscaledDeltaTime;
			yield return null;
		}
		action(1f);
		yield return null;
		if (then != null)
		{
			then();
		}
		yield break;
	}

	public static void TextEraser(LocText label, string text, float progress)
	{
		string text2 = text.Substring(0, Mathf.CeilToInt((float)text.Length * (1f - progress)));
		label.SetText(text2);
		label.ForceMeshUpdate();
	}

	public static void TextWriter(LocText label, string text, float progress)
	{
		string text2 = ((progress == 1f) ? text : text.Substring(0, Mathf.CeilToInt((float)text.Length * progress)));
		label.SetText(text2);
		label.ForceMeshUpdate();
	}
}
