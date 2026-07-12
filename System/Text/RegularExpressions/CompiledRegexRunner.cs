using System;

namespace System.Text.RegularExpressions
{
	internal sealed class CompiledRegexRunner : RegexRunner
	{
		public void SetDelegates(Action<RegexRunner> go, Func<RegexRunner, bool> firstChar, Action<RegexRunner> trackCount)
		{
			this._goMethod = go;
			this._findFirstCharMethod = firstChar;
			this._initTrackCountMethod = trackCount;
		}

		protected override void Go()
		{
			this._goMethod(this);
		}

		protected override bool FindFirstChar()
		{
			return this._findFirstCharMethod(this);
		}

		protected override void InitTrackCount()
		{
			this._initTrackCountMethod(this);
		}

		private Action<RegexRunner> _goMethod;

		private Func<RegexRunner, bool> _findFirstCharMethod;

		private Action<RegexRunner> _initTrackCountMethod;
	}
}
