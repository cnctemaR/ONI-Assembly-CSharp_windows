using System;
using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
	public Promise Run(IEnumerator routine)
	{
		return new Promise(delegate(global::System.Action resolve)
		{
			this.StartCoroutine(this.RunRoutine(routine, resolve));
		});
	}

	public ValueTuple<Promise, global::System.Action> RunCancellable(IEnumerator routine)
	{
		Promise promise = new Promise();
		Coroutine coroutine = base.StartCoroutine(this.RunRoutine(routine, new global::System.Action(promise.Resolve)));
		global::System.Action action = delegate
		{
			this.StopCoroutine(coroutine);
		};
		return new ValueTuple<Promise, global::System.Action>(promise, action);
	}

	private IEnumerator RunRoutine(IEnumerator routine, global::System.Action completedCallback)
	{
		yield return routine;
		completedCallback();
		yield break;
	}

	public static CoroutineRunner Create()
	{
		return new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
	}

	public static Promise RunOne(IEnumerator routine)
	{
		CoroutineRunner runner = CoroutineRunner.Create();
		return runner.Run(routine).Then(delegate
		{
			global::UnityEngine.Object.Destroy(runner.gameObject);
		});
	}
}
