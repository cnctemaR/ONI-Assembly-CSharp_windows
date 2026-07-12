using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
	internal struct CompositionResult
	{
		public CompositionResult(params CompositionError[] errors)
		{
			this = new CompositionResult(errors);
		}

		public CompositionResult(IEnumerable<CompositionError> errors)
		{
			this._errors = errors;
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

		public CompositionResult MergeResult(CompositionResult result)
		{
			if (this.Succeeded)
			{
				return result;
			}
			if (result.Succeeded)
			{
				return this;
			}
			return this.MergeErrors(result._errors);
		}

		public CompositionResult MergeError(CompositionError error)
		{
			return this.MergeErrors(new CompositionError[] { error });
		}

		public CompositionResult MergeErrors(IEnumerable<CompositionError> errors)
		{
			return new CompositionResult(this._errors.ConcatAllowingNull<CompositionError>(errors));
		}

		public CompositionResult<T> ToResult<T>(T value)
		{
			return new CompositionResult<T>(value, this._errors);
		}

		public void ThrowOnErrors()
		{
			this.ThrowOnErrors(null);
		}

		public void ThrowOnErrors(AtomicComposition atomicComposition)
		{
			if (this.Succeeded)
			{
				return;
			}
			if (atomicComposition == null)
			{
				throw new CompositionException(this._errors);
			}
			throw new ChangeRejectedException(this._errors);
		}

		public static readonly CompositionResult SucceededResult;

		private readonly IEnumerable<CompositionError> _errors;
	}
}
