using System;

namespace System.Text.RegularExpressions
{
	public abstract class RegexRunner
	{
		protected internal RegexRunner()
		{
		}

		protected internal Match Scan(Regex regex, string text, int textbeg, int textend, int textstart, int prevlen, bool quick)
		{
			return this.Scan(regex, text, textbeg, textend, textstart, prevlen, quick, regex.MatchTimeout);
		}

		protected internal Match Scan(Regex regex, string text, int textbeg, int textend, int textstart, int prevlen, bool quick, TimeSpan timeout)
		{
			bool flag = false;
			Regex.ValidateMatchTimeout(timeout);
			this._ignoreTimeout = Regex.InfiniteMatchTimeout == timeout;
			this._timeout = (this._ignoreTimeout ? ((int)Regex.InfiniteMatchTimeout.TotalMilliseconds) : ((int)(timeout.TotalMilliseconds + 0.5)));
			this.runregex = regex;
			this.runtext = text;
			this.runtextbeg = textbeg;
			this.runtextend = textend;
			this.runtextstart = textstart;
			int num = (this.runregex.RightToLeft ? (-1) : 1);
			int num2 = (this.runregex.RightToLeft ? this.runtextbeg : this.runtextend);
			this.runtextpos = textstart;
			if (prevlen == 0)
			{
				if (this.runtextpos == num2)
				{
					return Match.Empty;
				}
				this.runtextpos += num;
			}
			this.StartTimeoutWatch();
			for (;;)
			{
				if (this.FindFirstChar())
				{
					this.CheckTimeout();
					if (!flag)
					{
						this.InitMatch();
						flag = true;
					}
					this.Go();
					if (this.runmatch._matchcount[0] > 0)
					{
						break;
					}
					this.runtrackpos = this.runtrack.Length;
					this.runstackpos = this.runstack.Length;
					this.runcrawlpos = this.runcrawl.Length;
				}
				if (this.runtextpos == num2)
				{
					goto Block_9;
				}
				this.runtextpos += num;
			}
			return this.TidyMatch(quick);
			Block_9:
			this.TidyMatch(true);
			return Match.Empty;
		}

		private void StartTimeoutWatch()
		{
			if (this._ignoreTimeout)
			{
				return;
			}
			this._timeoutChecksToSkip = 1000;
			this._timeoutOccursAt = Environment.TickCount + this._timeout;
		}

		protected void CheckTimeout()
		{
			if (this._ignoreTimeout)
			{
				return;
			}
			this.DoCheckTimeout();
		}

		private void DoCheckTimeout()
		{
			int num = this._timeoutChecksToSkip - 1;
			this._timeoutChecksToSkip = num;
			if (num != 0)
			{
				return;
			}
			this._timeoutChecksToSkip = 1000;
			int tickCount = Environment.TickCount;
			if (tickCount < this._timeoutOccursAt)
			{
				return;
			}
			if (0 > this._timeoutOccursAt && 0 < tickCount)
			{
				return;
			}
			throw new RegexMatchTimeoutException(this.runtext, this.runregex.pattern, TimeSpan.FromMilliseconds((double)this._timeout));
		}

		protected abstract void Go();

		protected abstract bool FindFirstChar();

		protected abstract void InitTrackCount();

		private void InitMatch()
		{
			if (this.runmatch == null)
			{
				if (this.runregex.caps != null)
				{
					this.runmatch = new MatchSparse(this.runregex, this.runregex.caps, this.runregex.capsize, this.runtext, this.runtextbeg, this.runtextend - this.runtextbeg, this.runtextstart);
				}
				else
				{
					this.runmatch = new Match(this.runregex, this.runregex.capsize, this.runtext, this.runtextbeg, this.runtextend - this.runtextbeg, this.runtextstart);
				}
			}
			else
			{
				this.runmatch.Reset(this.runregex, this.runtext, this.runtextbeg, this.runtextend, this.runtextstart);
			}
			if (this.runcrawl != null)
			{
				this.runtrackpos = this.runtrack.Length;
				this.runstackpos = this.runstack.Length;
				this.runcrawlpos = this.runcrawl.Length;
				return;
			}
			this.InitTrackCount();
			int num = this.runtrackcount * 8;
			int num2 = this.runtrackcount * 8;
			if (num < 32)
			{
				num = 32;
			}
			if (num2 < 16)
			{
				num2 = 16;
			}
			this.runtrack = new int[num];
			this.runtrackpos = num;
			this.runstack = new int[num2];
			this.runstackpos = num2;
			this.runcrawl = new int[32];
			this.runcrawlpos = 32;
		}

		private Match TidyMatch(bool quick)
		{
			if (!quick)
			{
				Match match = this.runmatch;
				this.runmatch = null;
				match.Tidy(this.runtextpos);
				return match;
			}
			return null;
		}

		protected void EnsureStorage()
		{
			if (this.runstackpos < this.runtrackcount * 4)
			{
				this.DoubleStack();
			}
			if (this.runtrackpos < this.runtrackcount * 4)
			{
				this.DoubleTrack();
			}
		}

		protected bool IsBoundary(int index, int startpos, int endpos)
		{
			return (index > startpos && RegexCharClass.IsWordChar(this.runtext[index - 1])) != (index < endpos && RegexCharClass.IsWordChar(this.runtext[index]));
		}

		protected bool IsECMABoundary(int index, int startpos, int endpos)
		{
			return (index > startpos && RegexCharClass.IsECMAWordChar(this.runtext[index - 1])) != (index < endpos && RegexCharClass.IsECMAWordChar(this.runtext[index]));
		}

		protected static bool CharInSet(char ch, string set, string category)
		{
			string text = RegexCharClass.ConvertOldStringsToClass(set, category);
			return RegexCharClass.CharInClass(ch, text);
		}

		protected static bool CharInClass(char ch, string charClass)
		{
			return RegexCharClass.CharInClass(ch, charClass);
		}

		protected void DoubleTrack()
		{
			int[] array = new int[this.runtrack.Length * 2];
			Array.Copy(this.runtrack, 0, array, this.runtrack.Length, this.runtrack.Length);
			this.runtrackpos += this.runtrack.Length;
			this.runtrack = array;
		}

		protected void DoubleStack()
		{
			int[] array = new int[this.runstack.Length * 2];
			Array.Copy(this.runstack, 0, array, this.runstack.Length, this.runstack.Length);
			this.runstackpos += this.runstack.Length;
			this.runstack = array;
		}

		protected void DoubleCrawl()
		{
			int[] array = new int[this.runcrawl.Length * 2];
			Array.Copy(this.runcrawl, 0, array, this.runcrawl.Length, this.runcrawl.Length);
			this.runcrawlpos += this.runcrawl.Length;
			this.runcrawl = array;
		}

		protected void Crawl(int i)
		{
			if (this.runcrawlpos == 0)
			{
				this.DoubleCrawl();
			}
			int[] array = this.runcrawl;
			int num = this.runcrawlpos - 1;
			this.runcrawlpos = num;
			array[num] = i;
		}

		protected int Popcrawl()
		{
			int[] array = this.runcrawl;
			int num = this.runcrawlpos;
			this.runcrawlpos = num + 1;
			return array[num];
		}

		protected int Crawlpos()
		{
			return this.runcrawl.Length - this.runcrawlpos;
		}

		protected void Capture(int capnum, int start, int end)
		{
			if (end < start)
			{
				int num = end;
				end = start;
				start = num;
			}
			this.Crawl(capnum);
			this.runmatch.AddMatch(capnum, start, end - start);
		}

		protected void TransferCapture(int capnum, int uncapnum, int start, int end)
		{
			if (end < start)
			{
				int num = end;
				end = start;
				start = num;
			}
			int num2 = this.MatchIndex(uncapnum);
			int num3 = num2 + this.MatchLength(uncapnum);
			if (start >= num3)
			{
				end = start;
				start = num3;
			}
			else if (end <= num2)
			{
				start = num2;
			}
			else
			{
				if (end > num3)
				{
					end = num3;
				}
				if (num2 > start)
				{
					start = num2;
				}
			}
			this.Crawl(uncapnum);
			this.runmatch.BalanceMatch(uncapnum);
			if (capnum != -1)
			{
				this.Crawl(capnum);
				this.runmatch.AddMatch(capnum, start, end - start);
			}
		}

		protected void Uncapture()
		{
			int num = this.Popcrawl();
			this.runmatch.RemoveMatch(num);
		}

		protected bool IsMatched(int cap)
		{
			return this.runmatch.IsMatched(cap);
		}

		protected int MatchIndex(int cap)
		{
			return this.runmatch.MatchIndex(cap);
		}

		protected int MatchLength(int cap)
		{
			return this.runmatch.MatchLength(cap);
		}

		protected internal int runtextbeg;

		protected internal int runtextend;

		protected internal int runtextstart;

		protected internal string runtext;

		protected internal int runtextpos;

		protected internal int[] runtrack;

		protected internal int runtrackpos;

		protected internal int[] runstack;

		protected internal int runstackpos;

		protected internal int[] runcrawl;

		protected internal int runcrawlpos;

		protected internal int runtrackcount;

		protected internal Match runmatch;

		protected internal Regex runregex;

		private int _timeout;

		private bool _ignoreTimeout;

		private int _timeoutOccursAt;

		private const int TimeoutCheckFrequency = 1000;

		private int _timeoutChecksToSkip;
	}
}
