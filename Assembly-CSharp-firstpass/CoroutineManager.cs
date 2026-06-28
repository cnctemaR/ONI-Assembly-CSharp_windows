using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
	public CoroutineManager()
	{
		this.mGreedyCoroutines = new List<CoroutineManager.GreedyCoroutine>();
	}

	public void SetTimeout(float timeout)
	{
		this.mTimeout = timeout;
	}

	public IEnumerator UpdateCoroutines()
	{
		for (;;)
		{
			yield return new WaitForEndOfFrame();
			foreach (CoroutineManager.GreedyCoroutine greedyCoroutine in this.mGreedyCoroutines)
			{
				greedyCoroutine.Advance();
			}
			bool is_work_remaining = true;
			while (is_work_remaining)
			{
				is_work_remaining = false;
				foreach (CoroutineManager.GreedyCoroutine greedyCoroutine2 in this.mGreedyCoroutines)
				{
					if (!greedyCoroutine2.IsWorkComplete())
					{
						float num = Time.realtimeSinceStartup - Time.unscaledTime;
						if (num <= this.mTimeout)
						{
							is_work_remaining = true;
							greedyCoroutine2.Advance();
						}
					}
				}
			}
		}
		yield break;
	}

	public void StartGreedyCoroutine(IEnumerator coroutine)
	{
		this.mGreedyCoroutines.Add(new CoroutineManager.GreedyCoroutine(coroutine));
	}

	public static float GAME_TIMEOUT = 0.016f;

	public static float LOADING_TIMEOUT = 0.1f;

	public List<CoroutineManager.GreedyCoroutine> mGreedyCoroutines;

	private float mTimeout = CoroutineManager.GAME_TIMEOUT;

	public class GreedyCoroutine
	{
		public GreedyCoroutine(IEnumerator coroutine)
		{
			this.mCoroutine = coroutine;
			this.mWorkComplete = false;
		}

		public void Advance()
		{
			this.mCoroutine.MoveNext();
			if (this.mCoroutine.Current != null)
			{
				this.mWorkComplete = (bool)this.mCoroutine.Current;
			}
			else
			{
				this.mWorkComplete = false;
			}
		}

		public bool IsWorkComplete()
		{
			return this.mWorkComplete;
		}

		public IEnumerator mCoroutine;

		public bool mWorkComplete;
	}
}
