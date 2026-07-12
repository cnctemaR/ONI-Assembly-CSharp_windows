using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
	internal struct CompositionResult<T>
	{
		public CompositionResult(T value)
		{
			this = new CompositionResult<T>(value, null);
		}

		public CompositionResult(params CompositionError[] errors)
		{
			this = new CompositionResult<T>(default(T), errors);
		}

		public CompositionResult(IEnumerable<CompositionError> errors)
		{
			this = new CompositionResult<T>(default(T), errors);
		}

		internal CompositionResult(T value, IEnumerable<CompositionError> errors)
		{
			this._errors = errors;
			this._value = value;
		}

		public bool Succeeded
		{
			get
			{
				return this._errors == null || !this._errors.FastAny<CompositionError>();
			}
		}

		public IEnumerable<CompositionError> Errors
		{
			get
			{
				return this._errors ?? Enumerable.Empty<CompositionError>();
			}
		}

		public T Value
		{
			get
			{
				this.ThrowOnErrors();
				return this._value;
			}
		}

		internal CompositionResult<TValue> ToResult<TValue>()
		{
			return new CompositionResult<TValue>(this._errors);
		}

		internal CompositionResult ToResult()
		{
			return new CompositionResult(this._errors);
		}

		private void ThrowOnErrors()
		{
			if (!this.Succeeded)
			{
				throw new CompositionException(this._errors);
			}
		}

		private readonly IEnumerable<CompositionError> _errors;

		private readonly T _value;
	}
}
