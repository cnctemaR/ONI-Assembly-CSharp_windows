using System;
using System.Collections;

namespace UnityEngine
{
	/// <summary>
	///   <para>Base class for custom yield instructions to suspend coroutines.</para>
	/// </summary>
	public abstract class CustomYieldInstruction : IEnumerator
	{
		/// <summary>
		///   <para>Indicates if coroutine should be kept suspended.</para>
		/// </summary>
		public abstract bool keepWaiting { get; }

		public object Current
		{
			get
			{
				return null;
			}
		}

		public bool MoveNext()
		{
			return this.keepWaiting;
		}

		public void Reset()
		{
		}
	}
}
