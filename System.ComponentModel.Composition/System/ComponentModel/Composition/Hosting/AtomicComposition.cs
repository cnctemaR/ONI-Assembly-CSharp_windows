using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	public class AtomicComposition : IDisposable
	{
		public AtomicComposition()
			: this(null)
		{
		}

		public AtomicComposition(AtomicComposition outerAtomicComposition)
		{
			if (outerAtomicComposition != null)
			{
				this._outerAtomicComposition = outerAtomicComposition;
				this._outerAtomicComposition.ContainsInnerAtomicComposition = true;
			}
		}

		public void SetValue(object key, object value)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<object>(key, "key");
			this.SetValueInternal(key, value);
		}

		public bool TryGetValue<T>(object key, out T value)
		{
			return this.TryGetValue<T>(key, false, out value);
		}

		public bool TryGetValue<T>(object key, bool localAtomicCompositionOnly, out T value)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			Requires.NotNull<object>(key, "key");
			return this.TryGetValueInternal<T>(key, localAtomicCompositionOnly, out value);
		}

		public void AddCompleteAction(Action completeAction)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<Action>(completeAction, "completeAction");
			if (this._completeActionList == null)
			{
				this._completeActionList = new List<Action>();
			}
			this._completeActionList.Add(completeAction);
		}

		public void AddRevertAction(Action revertAction)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<Action>(revertAction, "revertAction");
			if (this._revertActionList == null)
			{
				this._revertActionList = new List<Action>();
			}
			this._revertActionList.Add(revertAction);
		}

		public void Complete()
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			if (this._outerAtomicComposition == null)
			{
				this.FinalComplete();
			}
			else
			{
				this.CopyComplete();
			}
			this._isCompleted = true;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.ThrowIfDisposed();
			this._isDisposed = true;
			if (this._outerAtomicComposition != null)
			{
				this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
			}
			if (!this._isCompleted && this._revertActionList != null)
			{
				for (int i = this._revertActionList.Count - 1; i >= 0; i--)
				{
					this._revertActionList[i]();
				}
				this._revertActionList = null;
			}
		}

		private void FinalComplete()
		{
			if (this._completeActionList != null)
			{
				foreach (Action action in this._completeActionList)
				{
					action();
				}
				this._completeActionList = null;
			}
		}

		private void CopyComplete()
		{
			Assumes.NotNull<AtomicComposition>(this._outerAtomicComposition);
			this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
			if (this._completeActionList != null)
			{
				foreach (Action action in this._completeActionList)
				{
					this._outerAtomicComposition.AddCompleteAction(action);
				}
			}
			if (this._revertActionList != null)
			{
				foreach (Action action2 in this._revertActionList)
				{
					this._outerAtomicComposition.AddRevertAction(action2);
				}
			}
			for (int i = 0; i < this._valueCount; i++)
			{
				this._outerAtomicComposition.SetValueInternal(this._values[i].Key, this._values[i].Value);
			}
		}

		private bool ContainsInnerAtomicComposition
		{
			set
			{
				if (value && this._containsInnerAtomicComposition)
				{
					throw new InvalidOperationException(Strings.AtomicComposition_AlreadyNested);
				}
				this._containsInnerAtomicComposition = value;
			}
		}

		private bool TryGetValueInternal<T>(object key, bool localAtomicCompositionOnly, out T value)
		{
			for (int i = 0; i < this._valueCount; i++)
			{
				if (this._values[i].Key == key)
				{
					value = (T)((object)this._values[i].Value);
					return true;
				}
			}
			if (!localAtomicCompositionOnly && this._outerAtomicComposition != null)
			{
				return this._outerAtomicComposition.TryGetValueInternal<T>(key, localAtomicCompositionOnly, out value);
			}
			value = default(T);
			return false;
		}

		private void SetValueInternal(object key, object value)
		{
			for (int i = 0; i < this._valueCount; i++)
			{
				if (this._values[i].Key == key)
				{
					this._values[i] = new KeyValuePair<object, object>(key, value);
					return;
				}
			}
			if (this._values == null || this._valueCount == this._values.Length)
			{
				KeyValuePair<object, object>[] array = new KeyValuePair<object, object>[(this._valueCount == 0) ? 5 : (this._valueCount * 2)];
				if (this._values != null)
				{
					Array.Copy(this._values, array, this._valueCount);
				}
				this._values = array;
			}
			this._values[this._valueCount] = new KeyValuePair<object, object>(key, value);
			this._valueCount++;
		}

		[DebuggerStepThrough]
		private void ThrowIfContainsInnerAtomicComposition()
		{
			if (this._containsInnerAtomicComposition)
			{
				throw new InvalidOperationException(Strings.AtomicComposition_PartOfAnotherAtomicComposition);
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfCompleted()
		{
			if (this._isCompleted)
			{
				throw new InvalidOperationException(Strings.AtomicComposition_AlreadyCompleted);
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private readonly AtomicComposition _outerAtomicComposition;

		private KeyValuePair<object, object>[] _values;

		private int _valueCount;

		private List<Action> _completeActionList;

		private List<Action> _revertActionList;

		private bool _isDisposed;

		private bool _isCompleted;

		private bool _containsInnerAtomicComposition;
	}
}
