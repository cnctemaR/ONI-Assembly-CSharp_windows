using System;

namespace System.Text.RegularExpressions
{
	internal sealed class CompiledRegexRunner : RegexRunner
	{
		internal CompiledRegexRunner()
		{
		}

		internal void SetDelegates(NoParamDelegate go, FindFirstCharDelegate firstChar, NoParamDelegate trackCount)
		{
			this.goMethod = go;
			this.findFirstCharMethod = firstChar;
			this.initTrackCountMethod = trackCount;
		}

		protected override void Go()
		{
			this.goMethod(this);
		}

		protected override bool FindFirstChar()
		{
			return this.findFirstCharMethod(this);
		}

		protected override void InitTrackCount()
		{
			this.initTrackCountMethod(this);
		}

		private NoParamDelegate goMethod;

		private FindFirstCharDelegate findFirstCharMethod;

		private NoParamDelegate initTrackCountMethod;
	}
}
