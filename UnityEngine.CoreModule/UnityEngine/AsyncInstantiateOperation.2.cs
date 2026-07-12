using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	public class AsyncInstantiateOperation<T> : CustomYieldInstruction where T : Object
	{
		internal AsyncInstantiateOperation(AsyncInstantiateOperation op)
		{
			this.m_op = op;
		}

		public override bool keepWaiting
		{
			get
			{
				return !this.m_op.isDone;
			}
		}

		public AsyncInstantiateOperation GetOperation()
		{
			return this.m_op;
		}

		public static implicit operator AsyncInstantiateOperation(AsyncInstantiateOperation<T> generic)
		{
			return generic.m_op;
		}

		public bool IsWaitingForSceneActivation()
		{
			return this.m_op.IsWaitingForSceneActivation();
		}

		public event Action<AsyncOperation> completed
		{
			add
			{
				this.m_op.completed += value;
			}
			remove
			{
				this.m_op.completed -= value;
			}
		}

		public bool isDone
		{
			get
			{
				return this.m_op.isDone;
			}
		}

		public float progress
		{
			get
			{
				return this.m_op.progress;
			}
		}

		public bool allowSceneActivation
		{
			get
			{
				return this.m_op.allowSceneActivation;
			}
			set
			{
				this.m_op.allowSceneActivation = value;
			}
		}

		public void WaitForCompletion()
		{
			this.m_op.WaitForCompletion();
		}

		public void Cancel()
		{
			this.m_op.Cancel();
		}

		public unsafe T[] Result
		{
			get
			{
				Object[] result = this.m_op.Result;
				return *UnsafeUtility.As<Object[], T[]>(ref result);
			}
		}

		internal AsyncInstantiateOperation m_op;
	}
}
