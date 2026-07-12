using System;
using System.Collections.Generic;
using UnityEngine;

public static class SequenceUtil
{
	public static YieldInstruction WaitForNextFrame
	{
		get
		{
			return null;
		}
	}

	public static YieldInstruction WaitForEndOfFrame
	{
		get
		{
			if (SequenceUtil.waitForEndOfFrame == null)
			{
				SequenceUtil.waitForEndOfFrame = new WaitForEndOfFrame();
			}
			return SequenceUtil.waitForEndOfFrame;
		}
	}

	public static YieldInstruction WaitForFixedUpdate
	{
		get
		{
			if (SequenceUtil.waitForFixedUpdate == null)
			{
				SequenceUtil.waitForFixedUpdate = new WaitForFixedUpdate();
			}
			return SequenceUtil.waitForFixedUpdate;
		}
	}

	public static YieldInstruction WaitForSeconds(float duration)
	{
		WaitForSeconds waitForSeconds;
		if (!SequenceUtil.scaledTimeCache.TryGetValue(duration, out waitForSeconds))
		{
			waitForSeconds = (SequenceUtil.scaledTimeCache[duration] = new WaitForSeconds(duration));
		}
		return waitForSeconds;
	}

	public static WaitForSecondsRealtime WaitForSecondsRealtime(float duration)
	{
		WaitForSecondsRealtime waitForSecondsRealtime;
		if (!SequenceUtil.reailTimeWaitCache.TryGetValue(duration, out waitForSecondsRealtime))
		{
			waitForSecondsRealtime = (SequenceUtil.reailTimeWaitCache[duration] = new WaitForSecondsRealtime(duration));
		}
		return waitForSecondsRealtime;
	}

	private static WaitForEndOfFrame waitForEndOfFrame = null;

	private static WaitForFixedUpdate waitForFixedUpdate = null;

	private static Dictionary<float, WaitForSeconds> scaledTimeCache = new Dictionary<float, WaitForSeconds>();

	private static Dictionary<float, WaitForSecondsRealtime> reailTimeWaitCache = new Dictionary<float, WaitForSecondsRealtime>();
}
