using System;
using System.Collections;
using UnityEngine;

public readonly struct Updater : IEnumerator
{
	public Updater(Func<float, UpdaterResult> fn)
	{
		this.fn = fn;
	}

	public UpdaterResult Internal_Update(float deltaTime)
	{
		return this.fn(deltaTime);
	}

	object IEnumerator.Current
	{
		get
		{
			return null;
		}
	}

	bool IEnumerator.MoveNext()
	{
		return this.fn(Updater.GetDeltaTime()) == UpdaterResult.NotComplete;
	}

	void IEnumerator.Reset()
	{
	}

	public static implicit operator Updater(Promise promise)
	{
		return Updater.Until(() => promise.IsResolved);
	}

	public static Updater Until(Func<bool> fn)
	{
		return new Updater(delegate(float dt)
		{
			if (!fn())
			{
				return UpdaterResult.NotComplete;
			}
			return UpdaterResult.Complete;
		});
	}

	public static Updater While(Func<bool> fn)
	{
		return new Updater(delegate(float dt)
		{
			if (!fn())
			{
				return UpdaterResult.Complete;
			}
			return UpdaterResult.NotComplete;
		});
	}

	public static Updater None()
	{
		return Updater.WaitFrames(0);
	}

	public static Updater WaitOneFrame()
	{
		return Updater.WaitFrames(1);
	}

	public static Updater WaitFrames(int framesToWait)
	{
		int frame = 0;
		return new Updater(delegate(float dt)
		{
			if (framesToWait <= frame)
			{
				return UpdaterResult.Complete;
			}
			int frame2 = frame;
			frame = frame2 + 1;
			return UpdaterResult.NotComplete;
		});
	}

	public static Updater WaitForSeconds(float secondsToWait)
	{
		float currentSeconds = 0f;
		return new Updater(delegate(float dt)
		{
			if (secondsToWait <= currentSeconds)
			{
				return UpdaterResult.Complete;
			}
			currentSeconds += dt;
			return UpdaterResult.NotComplete;
		});
	}

	public static Updater Ease(Action<float> fn, float from, float to, float duration, Easing.EasingFn easing = null)
	{
		return Updater.GenericEase<float>(fn, new Func<float, float, float, float>(Mathf.LerpUnclamped), easing, from, to, duration);
	}

	public static Updater Ease(Action<Vector2> fn, Vector2 from, Vector2 to, float duration, Easing.EasingFn easing = null)
	{
		return Updater.GenericEase<Vector2>(fn, new Func<Vector2, Vector2, float, Vector2>(Vector2.LerpUnclamped), easing, from, to, duration);
	}

	public static Updater Ease(Action<Vector3> fn, Vector3 from, Vector3 to, float duration, Easing.EasingFn easing = null)
	{
		return Updater.GenericEase<Vector3>(fn, new Func<Vector3, Vector3, float, Vector3>(Vector3.LerpUnclamped), easing, from, to, duration);
	}

	public static Updater GenericEase<T>(Action<T> useFn, Func<T, T, float, T> interpolateFn, Easing.EasingFn easingFn, T from, T to, float duration)
	{
		if (easingFn == null)
		{
			easingFn = Easing.SmoothStep;
		}
		float currentSeconds = 0f;
		return new Updater(delegate(float dt)
		{
			float num = currentSeconds / duration;
			useFn(interpolateFn(from, to, easingFn(num)));
			if (num >= 1f)
			{
				return UpdaterResult.Complete;
			}
			currentSeconds += dt;
			return UpdaterResult.NotComplete;
		});
	}

	public static Updater Do(global::System.Action fn)
	{
		return new Updater(delegate(float dt)
		{
			fn();
			return UpdaterResult.Complete;
		});
	}

	public static Updater Do(Func<Updater> fn)
	{
		bool didInitalize = false;
		Updater target = default(Updater);
		return new Updater(delegate(float dt)
		{
			if (!didInitalize)
			{
				target = fn();
				didInitalize = true;
			}
			return target.Internal_Update(dt);
		});
	}

	public static Updater Parallel(params Updater[] updaters)
	{
		bool[] isCompleted = new bool[updaters.Length];
		return new Updater(delegate(float dt)
		{
			bool flag = false;
			for (int i = 0; i < updaters.Length; i++)
			{
				if (!isCompleted[i])
				{
					if (updaters[i].Internal_Update(dt) == UpdaterResult.Complete)
					{
						isCompleted[i] = true;
					}
					else
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return UpdaterResult.Complete;
			}
			return UpdaterResult.NotComplete;
		});
	}

	public static Updater Series(params Updater[] updaters)
	{
		int i = 0;
		return new Updater(delegate(float dt)
		{
			if (updaters[i].Internal_Update(dt) == UpdaterResult.Complete)
			{
				int j = i;
				i = j + 1;
			}
			if (i == updaters.Length)
			{
				return UpdaterResult.Complete;
			}
			return UpdaterResult.NotComplete;
		});
	}

	public static Promise RunRoutine(MonoBehaviour monoBehaviour, IEnumerator coroutine)
	{
		Updater.<>c__DisplayClass22_0 CS$<>8__locals1 = new Updater.<>c__DisplayClass22_0();
		CS$<>8__locals1.coroutine = coroutine;
		CS$<>8__locals1.willComplete = new Promise();
		monoBehaviour.StartCoroutine(CS$<>8__locals1.<RunRoutine>g__Routine|0());
		return CS$<>8__locals1.willComplete;
	}

	public static Promise Run(MonoBehaviour monoBehaviour, params Updater[] updaters)
	{
		return Updater.Run(monoBehaviour, Updater.Series(updaters));
	}

	public static Promise Run(MonoBehaviour monoBehaviour, Updater updater)
	{
		Updater.<>c__DisplayClass24_0 CS$<>8__locals1 = new Updater.<>c__DisplayClass24_0();
		CS$<>8__locals1.updater = updater;
		CS$<>8__locals1.willComplete = new Promise();
		monoBehaviour.StartCoroutine(CS$<>8__locals1.<Run>g__Routine|0());
		return CS$<>8__locals1.willComplete;
	}

	public static float GetDeltaTime()
	{
		return Time.unscaledDeltaTime;
	}

	public readonly Func<float, UpdaterResult> fn;
}
