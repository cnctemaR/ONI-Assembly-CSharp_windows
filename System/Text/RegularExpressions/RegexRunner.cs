using System;
using System.ComponentModel;

namespace System.Text.RegularExpressions
{
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
	[global::System.MonoTODO("RegexRunner is not supported by Mono.")]
	public abstract class RegexRunner
	{
		[global::System.MonoTODO]
		protected internal RegexRunner()
		{
		}

		protected abstract bool FindFirstChar();

		protected abstract void Go();

		protected abstract void InitTrackCount();

		[global::System.MonoTODO]
		protected void Capture(int capnum, int start, int end)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected static bool CharInClass(char ch, string charClass)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected static bool CharInSet(char ch, string set, string category)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void Crawl(int i)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected int Crawlpos()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void DoubleCrawl()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void DoubleStack()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void DoubleTrack()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void EnsureStorage()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected bool IsBoundary(int index, int startpos, int endpos)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected bool IsECMABoundary(int index, int startpos, int endpos)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected bool IsMatched(int cap)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected int MatchIndex(int cap)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected int MatchLength(int cap)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected int Popcrawl()
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void TransferCapture(int capnum, int uncapnum, int start, int end)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected void Uncapture()
		{
			throw new NotImplementedException();
		}

		protected internal Match Scan(Regex regex, string text, int textbeg, int textend, int textstart, int prevlen, bool quick)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		protected internal int[] runcrawl;

		[global::System.MonoTODO]
		protected internal int runcrawlpos;

		[global::System.MonoTODO]
		protected internal Match runmatch;

		[global::System.MonoTODO]
		protected internal Regex runregex;

		[global::System.MonoTODO]
		protected internal int[] runstack;

		[global::System.MonoTODO]
		protected internal int runstackpos;

		[global::System.MonoTODO]
		protected internal string runtext;

		[global::System.MonoTODO]
		protected internal int runtextbeg;

		[global::System.MonoTODO]
		protected internal int runtextend;

		[global::System.MonoTODO]
		protected internal int runtextpos;

		[global::System.MonoTODO]
		protected internal int runtextstart;

		[global::System.MonoTODO]
		protected internal int[] runtrack;

		[global::System.MonoTODO]
		protected internal int runtrackcount;

		[global::System.MonoTODO]
		protected internal int runtrackpos;
	}
}
