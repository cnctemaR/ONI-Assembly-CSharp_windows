using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Regex : ISerializable
	{
		protected Regex()
		{
			this.internalMatchTimeout = Regex.DefaultMatchTimeout;
		}

		public Regex(string pattern)
			: this(pattern, RegexOptions.None, Regex.DefaultMatchTimeout, false)
		{
		}

		public Regex(string pattern, RegexOptions options)
			: this(pattern, options, Regex.DefaultMatchTimeout, false)
		{
		}

		public Regex(string pattern, RegexOptions options, TimeSpan matchTimeout)
			: this(pattern, options, matchTimeout, false)
		{
		}

		private Regex(string pattern, RegexOptions options, TimeSpan matchTimeout, bool useCache)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			if (options < RegexOptions.None || options >> 10 != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			if ((options & RegexOptions.ECMAScript) != RegexOptions.None && (options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)) != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			Regex.ValidateMatchTimeout(matchTimeout);
			string text;
			if ((options & RegexOptions.CultureInvariant) != RegexOptions.None)
			{
				text = CultureInfo.InvariantCulture.ToString();
			}
			else
			{
				text = CultureInfo.CurrentCulture.ToString();
			}
			string[] array = new string[5];
			int num = 0;
			int num2 = (int)options;
			array[num] = num2.ToString(NumberFormatInfo.InvariantInfo);
			array[1] = ":";
			array[2] = text;
			array[3] = ":";
			array[4] = pattern;
			string text2 = string.Concat(array);
			CachedCodeEntry cachedCodeEntry = Regex.LookupCachedAndUpdate(text2);
			this.pattern = pattern;
			this.roptions = options;
			this.internalMatchTimeout = matchTimeout;
			if (cachedCodeEntry == null)
			{
				RegexTree regexTree = RegexParser.Parse(pattern, this.roptions);
				this.capnames = regexTree._capnames;
				this.capslist = regexTree._capslist;
				this.code = RegexWriter.Write(regexTree);
				this.caps = this.code._caps;
				this.capsize = this.code._capsize;
				this.InitializeReferences();
				if (useCache)
				{
					cachedCodeEntry = this.CacheCode(text2);
				}
			}
			else
			{
				this.caps = cachedCodeEntry._caps;
				this.capnames = cachedCodeEntry._capnames;
				this.capslist = cachedCodeEntry._capslist;
				this.capsize = cachedCodeEntry._capsize;
				this.code = cachedCodeEntry._code;
				this.factory = cachedCodeEntry._factory;
				this.runnerref = cachedCodeEntry._runnerref;
				this.replref = cachedCodeEntry._replref;
				this.refsInitialized = true;
			}
			if (this.UseOptionC() && this.factory == null)
			{
				this.factory = this.Compile(this.code, this.roptions);
				if (useCache && cachedCodeEntry != null)
				{
					cachedCodeEntry.AddCompiled(this.factory);
				}
				this.code = null;
			}
		}

		protected Regex(SerializationInfo info, StreamingContext context)
			: this(info.GetString("pattern"), (RegexOptions)info.GetInt32("options"))
		{
			try
			{
				long @int = info.GetInt64("matchTimeout");
				TimeSpan timeSpan = new TimeSpan(@int);
				Regex.ValidateMatchTimeout(timeSpan);
				this.internalMatchTimeout = timeSpan;
			}
			catch (SerializationException)
			{
			}
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("pattern", this.ToString());
			si.AddValue("options", this.Options);
			si.AddValue("matchTimeout", this.MatchTimeout.Ticks);
		}

		protected internal static void ValidateMatchTimeout(TimeSpan matchTimeout)
		{
			if (Regex.InfiniteMatchTimeout == matchTimeout)
			{
				return;
			}
			if (TimeSpan.Zero < matchTimeout && matchTimeout <= Regex.MaximumMatchTimeout)
			{
				return;
			}
			throw new ArgumentOutOfRangeException("matchTimeout");
		}

		private static TimeSpan InitDefaultMatchTimeout()
		{
			object data = AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT");
			if (data == null)
			{
				return Regex.FallbackDefaultMatchTimeout;
			}
			if (!(data is TimeSpan))
			{
				throw new InvalidCastException(global::SR.GetString("AppDomain data '{0}' contains an invalid value or object for specifying a default matching timeout for System.Text.RegularExpressions.Regex.", new object[] { "REGEX_DEFAULT_MATCH_TIMEOUT" }));
			}
			TimeSpan timeSpan = (TimeSpan)data;
			try
			{
				Regex.ValidateMatchTimeout(timeSpan);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new ArgumentOutOfRangeException(global::SR.GetString("AppDomain data '{0}' contains an invalid value or object for specifying a default matching timeout for System.Text.RegularExpressions.Regex.", new object[] { "REGEX_DEFAULT_MATCH_TIMEOUT" }));
			}
			return timeSpan;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal RegexRunnerFactory Compile(RegexCode code, RegexOptions roptions)
		{
			return RegexCompiler.Compile(code, roptions);
		}

		public static string Escape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return RegexParser.Escape(str);
		}

		public static string Unescape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return RegexParser.Unescape(str);
		}

		public static int CacheSize
		{
			get
			{
				return Regex.cacheSize;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				Regex.cacheSize = value;
				if (Regex.livecode.Count > Regex.cacheSize)
				{
					LinkedList<CachedCodeEntry> linkedList = Regex.livecode;
					lock (linkedList)
					{
						while (Regex.livecode.Count > Regex.cacheSize)
						{
							Regex.livecode.RemoveLast();
						}
					}
				}
			}
		}

		[CLSCompliant(false)]
		protected IDictionary Caps
		{
			get
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				foreach (object obj in this.caps.Keys)
				{
					int num = (int)obj;
					dictionary.Add(num, (int)this.caps[num]);
				}
				return dictionary;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.caps = new Hashtable(value.Count);
				foreach (object obj in value)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					this.caps[(int)dictionaryEntry.Key] = (int)dictionaryEntry.Value;
				}
			}
		}

		[CLSCompliant(false)]
		protected IDictionary CapNames
		{
			get
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in this.capnames.Keys)
				{
					string text = (string)obj;
					dictionary.Add(text, (int)this.capnames[text]);
				}
				return dictionary;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.capnames = new Hashtable(value.Count);
				foreach (object obj in value)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					this.capnames[(string)dictionaryEntry.Key] = (int)dictionaryEntry.Value;
				}
			}
		}

		public RegexOptions Options
		{
			get
			{
				return this.roptions;
			}
		}

		public TimeSpan MatchTimeout
		{
			get
			{
				return this.internalMatchTimeout;
			}
		}

		public bool RightToLeft
		{
			get
			{
				return this.UseOptionR();
			}
		}

		public override string ToString()
		{
			return this.pattern;
		}

		public string[] GetGroupNames()
		{
			string[] array;
			if (this.capslist == null)
			{
				int num = this.capsize;
				array = new string[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = Convert.ToString(i, CultureInfo.InvariantCulture);
				}
			}
			else
			{
				array = new string[this.capslist.Length];
				Array.Copy(this.capslist, 0, array, 0, this.capslist.Length);
			}
			return array;
		}

		public int[] GetGroupNumbers()
		{
			int[] array;
			if (this.caps == null)
			{
				int num = this.capsize;
				array = new int[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = i;
				}
			}
			else
			{
				array = new int[this.caps.Count];
				IDictionaryEnumerator enumerator = this.caps.GetEnumerator();
				while (enumerator.MoveNext())
				{
					array[(int)enumerator.Value] = (int)enumerator.Key;
				}
			}
			return array;
		}

		public string GroupNameFromNumber(int i)
		{
			if (this.capslist == null)
			{
				if (i >= 0 && i < this.capsize)
				{
					return i.ToString(CultureInfo.InvariantCulture);
				}
				return string.Empty;
			}
			else
			{
				if (this.caps != null)
				{
					object obj = this.caps[i];
					if (obj == null)
					{
						return string.Empty;
					}
					i = (int)obj;
				}
				if (i >= 0 && i < this.capslist.Length)
				{
					return this.capslist[i];
				}
				return string.Empty;
			}
		}

		public int GroupNumberFromName(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.capnames != null)
			{
				object obj = this.capnames[name];
				if (obj == null)
				{
					return -1;
				}
				return (int)obj;
			}
			else
			{
				int num = 0;
				foreach (char c in name)
				{
					if (c > '9' || c < '0')
					{
						return -1;
					}
					num *= 10;
					num += (int)(c - '0');
				}
				if (num >= 0 && num < this.capsize)
				{
					return num;
				}
				return -1;
			}
		}

		public static bool IsMatch(string input, string pattern)
		{
			return Regex.IsMatch(input, pattern, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static bool IsMatch(string input, string pattern, RegexOptions options)
		{
			return Regex.IsMatch(input, pattern, options, Regex.DefaultMatchTimeout);
		}

		public static bool IsMatch(string input, string pattern, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).IsMatch(input);
		}

		public bool IsMatch(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.IsMatch(input, this.UseOptionR() ? input.Length : 0);
		}

		public bool IsMatch(string input, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Run(true, -1, input, 0, input.Length, startat) == null;
		}

		public static Match Match(string input, string pattern)
		{
			return Regex.Match(input, pattern, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static Match Match(string input, string pattern, RegexOptions options)
		{
			return Regex.Match(input, pattern, options, Regex.DefaultMatchTimeout);
		}

		public static Match Match(string input, string pattern, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).Match(input);
		}

		public Match Match(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Match(input, this.UseOptionR() ? input.Length : 0);
		}

		public Match Match(string input, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Run(false, -1, input, 0, input.Length, startat);
		}

		public Match Match(string input, int beginning, int length)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Run(false, -1, input, beginning, length, this.UseOptionR() ? (beginning + length) : beginning);
		}

		public static MatchCollection Matches(string input, string pattern)
		{
			return Regex.Matches(input, pattern, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static MatchCollection Matches(string input, string pattern, RegexOptions options)
		{
			return Regex.Matches(input, pattern, options, Regex.DefaultMatchTimeout);
		}

		public static MatchCollection Matches(string input, string pattern, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).Matches(input);
		}

		public MatchCollection Matches(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Matches(input, this.UseOptionR() ? input.Length : 0);
		}

		public MatchCollection Matches(string input, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return new MatchCollection(this, input, 0, input.Length, startat);
		}

		public static string Replace(string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, replacement, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static string Replace(string input, string pattern, string replacement, RegexOptions options)
		{
			return Regex.Replace(input, pattern, replacement, options, Regex.DefaultMatchTimeout);
		}

		public static string Replace(string input, string pattern, string replacement, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).Replace(input, replacement);
		}

		public string Replace(string input, string replacement)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Replace(input, replacement, -1, this.UseOptionR() ? input.Length : 0);
		}

		public string Replace(string input, string replacement, int count)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Replace(input, replacement, count, this.UseOptionR() ? input.Length : 0);
		}

		public string Replace(string input, string replacement, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			RegexReplacement regexReplacement = (RegexReplacement)this.replref.Get();
			if (regexReplacement == null || !regexReplacement.Pattern.Equals(replacement))
			{
				regexReplacement = RegexParser.ParseReplacement(replacement, this.caps, this.capsize, this.capnames, this.roptions);
				this.replref.Cache(regexReplacement);
			}
			return regexReplacement.Replace(this, input, count, startat);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator)
		{
			return Regex.Replace(input, pattern, evaluator, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
		{
			return Regex.Replace(input, pattern, evaluator, options, Regex.DefaultMatchTimeout);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).Replace(input, evaluator);
		}

		public string Replace(string input, MatchEvaluator evaluator)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Replace(input, evaluator, -1, this.UseOptionR() ? input.Length : 0);
		}

		public string Replace(string input, MatchEvaluator evaluator, int count)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Replace(input, evaluator, count, this.UseOptionR() ? input.Length : 0);
		}

		public string Replace(string input, MatchEvaluator evaluator, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return RegexReplacement.Replace(evaluator, this, input, count, startat);
		}

		public static string[] Split(string input, string pattern)
		{
			return Regex.Split(input, pattern, RegexOptions.None, Regex.DefaultMatchTimeout);
		}

		public static string[] Split(string input, string pattern, RegexOptions options)
		{
			return Regex.Split(input, pattern, options, Regex.DefaultMatchTimeout);
		}

		public static string[] Split(string input, string pattern, RegexOptions options, TimeSpan matchTimeout)
		{
			return new Regex(pattern, options, matchTimeout, true).Split(input);
		}

		public string[] Split(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Split(input, 0, this.UseOptionR() ? input.Length : 0);
		}

		public string[] Split(string input, int count)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return RegexReplacement.Split(this, input, count, this.UseOptionR() ? input.Length : 0);
		}

		public string[] Split(string input, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return RegexReplacement.Split(this, input, count, startat);
		}

		public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, AssemblyName assemblyname)
		{
			Regex.CompileToAssemblyInternal(regexinfos, assemblyname, null, null);
		}

		public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, AssemblyName assemblyname, CustomAttributeBuilder[] attributes)
		{
			Regex.CompileToAssemblyInternal(regexinfos, assemblyname, attributes, null);
		}

		public static void CompileToAssembly(RegexCompilationInfo[] regexinfos, AssemblyName assemblyname, CustomAttributeBuilder[] attributes, string resourceFile)
		{
			Regex.CompileToAssemblyInternal(regexinfos, assemblyname, attributes, resourceFile);
		}

		private static void CompileToAssemblyInternal(RegexCompilationInfo[] regexinfos, AssemblyName assemblyname, CustomAttributeBuilder[] attributes, string resourceFile)
		{
			if (assemblyname == null)
			{
				throw new ArgumentNullException("assemblyname");
			}
			if (regexinfos == null)
			{
				throw new ArgumentNullException("regexinfos");
			}
			RegexCompiler.CompileToAssembly(regexinfos, assemblyname, attributes, resourceFile);
		}

		protected void InitializeReferences()
		{
			if (this.refsInitialized)
			{
				throw new NotSupportedException(global::SR.GetString("This operation is only allowed once per object."));
			}
			this.refsInitialized = true;
			this.runnerref = new ExclusiveReference();
			this.replref = new SharedReference();
		}

		internal Match Run(bool quick, int prevlen, string input, int beginning, int length, int startat)
		{
			RegexRunner regexRunner = null;
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("start", global::SR.GetString("Start index cannot be less than 0 or greater than input length."));
			}
			if (length < 0 || length > input.Length)
			{
				throw new ArgumentOutOfRangeException("length", global::SR.GetString("Length cannot be less than 0 or exceed input length."));
			}
			regexRunner = (RegexRunner)this.runnerref.Get();
			if (regexRunner == null)
			{
				if (this.factory != null)
				{
					regexRunner = this.factory.CreateInstance();
				}
				else
				{
					regexRunner = new RegexInterpreter(this.code, this.UseOptionInvariant() ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
				}
			}
			Match match;
			try
			{
				match = regexRunner.Scan(this, input, beginning, beginning + length, startat, prevlen, quick, this.internalMatchTimeout);
			}
			finally
			{
				this.runnerref.Release(regexRunner);
			}
			return match;
		}

		private static CachedCodeEntry LookupCachedAndUpdate(string key)
		{
			LinkedList<CachedCodeEntry> linkedList = Regex.livecode;
			lock (linkedList)
			{
				for (LinkedListNode<CachedCodeEntry> linkedListNode = Regex.livecode.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (linkedListNode.Value._key == key)
					{
						Regex.livecode.Remove(linkedListNode);
						Regex.livecode.AddFirst(linkedListNode);
						return linkedListNode.Value;
					}
				}
			}
			return null;
		}

		private CachedCodeEntry CacheCode(string key)
		{
			CachedCodeEntry cachedCodeEntry = null;
			LinkedList<CachedCodeEntry> linkedList = Regex.livecode;
			lock (linkedList)
			{
				for (LinkedListNode<CachedCodeEntry> linkedListNode = Regex.livecode.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
				{
					if (linkedListNode.Value._key == key)
					{
						Regex.livecode.Remove(linkedListNode);
						Regex.livecode.AddFirst(linkedListNode);
						return linkedListNode.Value;
					}
				}
				if (Regex.cacheSize != 0)
				{
					cachedCodeEntry = new CachedCodeEntry(key, this.capnames, this.capslist, this.code, this.caps, this.capsize, this.runnerref, this.replref);
					Regex.livecode.AddFirst(cachedCodeEntry);
					if (Regex.livecode.Count > Regex.cacheSize)
					{
						Regex.livecode.RemoveLast();
					}
				}
			}
			return cachedCodeEntry;
		}

		protected bool UseOptionC()
		{
			return false;
		}

		protected bool UseOptionR()
		{
			return (this.roptions & RegexOptions.RightToLeft) > RegexOptions.None;
		}

		internal bool UseOptionInvariant()
		{
			return (this.roptions & RegexOptions.CultureInvariant) > RegexOptions.None;
		}

		protected internal string pattern;

		protected internal RegexRunnerFactory factory;

		protected internal RegexOptions roptions;

		[NonSerialized]
		private static readonly TimeSpan MaximumMatchTimeout = TimeSpan.FromMilliseconds(2147483646.0);

		[NonSerialized]
		public static readonly TimeSpan InfiniteMatchTimeout = Timeout.InfiniteTimeSpan;

		[OptionalField(VersionAdded = 2)]
		protected internal TimeSpan internalMatchTimeout;

		private const string DefaultMatchTimeout_ConfigKeyName = "REGEX_DEFAULT_MATCH_TIMEOUT";

		[NonSerialized]
		internal static readonly TimeSpan FallbackDefaultMatchTimeout = Regex.InfiniteMatchTimeout;

		[NonSerialized]
		internal static readonly TimeSpan DefaultMatchTimeout = Regex.InitDefaultMatchTimeout();

		protected internal Hashtable caps;

		protected internal Hashtable capnames;

		protected internal string[] capslist;

		protected internal int capsize;

		internal ExclusiveReference runnerref;

		internal SharedReference replref;

		internal RegexCode code;

		internal bool refsInitialized;

		internal static LinkedList<CachedCodeEntry> livecode = new LinkedList<CachedCodeEntry>();

		internal static int cacheSize = 15;

		internal const int MaxOptionShift = 10;
	}
}
