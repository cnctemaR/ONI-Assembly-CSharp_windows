using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text.RegularExpressions.Syntax;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Regex : ISerializable
	{
		protected Regex()
		{
		}

		public Regex(string pattern)
			: this(pattern, RegexOptions.None)
		{
		}

		public Regex(string pattern, RegexOptions options)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			Regex.validate_options(options);
			this.pattern = pattern;
			this.roptions = options;
			this.Init();
		}

		protected Regex(SerializationInfo info, StreamingContext context)
			: this(info.GetString("pattern"), (RegexOptions)((int)info.GetValue("options", typeof(RegexOptions))))
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("pattern", this.ToString(), typeof(string));
			info.AddValue("options", this.Options, typeof(RegexOptions));
		}

		[global::System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname)
		{
			Regex.CompileToAssembly(regexes, aname, new CustomAttributeBuilder[0], null);
		}

		[global::System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname, CustomAttributeBuilder[] attribs)
		{
			Regex.CompileToAssembly(regexes, aname, attribs, null);
		}

		[global::System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname, CustomAttributeBuilder[] attribs, string resourceFile)
		{
			throw new NotImplementedException();
		}

		public static string Escape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return global::System.Text.RegularExpressions.Syntax.Parser.Escape(str);
		}

		public static string Unescape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return global::System.Text.RegularExpressions.Syntax.Parser.Unescape(str);
		}

		public static bool IsMatch(string input, string pattern)
		{
			return Regex.IsMatch(input, pattern, RegexOptions.None);
		}

		public static bool IsMatch(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.IsMatch(input);
		}

		public static Match Match(string input, string pattern)
		{
			return Regex.Match(input, pattern, RegexOptions.None);
		}

		public static Match Match(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Match(input);
		}

		public static MatchCollection Matches(string input, string pattern)
		{
			return Regex.Matches(input, pattern, RegexOptions.None);
		}

		public static MatchCollection Matches(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Matches(input);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator)
		{
			return Regex.Replace(input, pattern, evaluator, RegexOptions.None);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Replace(input, evaluator);
		}

		public static string Replace(string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, replacement, RegexOptions.None);
		}

		public static string Replace(string input, string pattern, string replacement, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Replace(input, replacement);
		}

		public static string[] Split(string input, string pattern)
		{
			return Regex.Split(input, pattern, RegexOptions.None);
		}

		public static string[] Split(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Split(input);
		}

		public static int CacheSize
		{
			get
			{
				return Regex.cache.Capacity;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("CacheSize");
				}
				Regex.cache.Capacity = value;
			}
		}

		private static void validate_options(RegexOptions options)
		{
			if ((options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.RightToLeft | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)) != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			if ((options & RegexOptions.ECMAScript) != RegexOptions.None && (options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.ECMAScript)) != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
		}

		private void Init()
		{
			this.machineFactory = Regex.cache.Lookup(this.pattern, this.roptions);
			if (this.machineFactory == null)
			{
				this.InitNewRegex();
			}
			else
			{
				this.group_count = this.machineFactory.GroupCount;
				this.gap = this.machineFactory.Gap;
				this.mapping = this.machineFactory.Mapping;
				this.group_names = this.machineFactory.NamesMapping;
			}
		}

		private void InitNewRegex()
		{
			this.machineFactory = Regex.CreateMachineFactory(this.pattern, this.roptions);
			Regex.cache.Add(this.pattern, this.roptions, this.machineFactory);
			this.group_count = this.machineFactory.GroupCount;
			this.gap = this.machineFactory.Gap;
			this.mapping = this.machineFactory.Mapping;
			this.group_names = this.machineFactory.NamesMapping;
		}

		private static IMachineFactory CreateMachineFactory(string pattern, RegexOptions options)
		{
			global::System.Text.RegularExpressions.Syntax.Parser parser = new global::System.Text.RegularExpressions.Syntax.Parser();
			global::System.Text.RegularExpressions.Syntax.RegularExpression regularExpression = parser.ParseRegularExpression(pattern, options);
			ICompiler compiler;
			if (!Regex.old_rx)
			{
				if ((options & RegexOptions.Compiled) != RegexOptions.None)
				{
					compiler = new CILCompiler();
				}
				else
				{
					compiler = new RxCompiler();
				}
			}
			else
			{
				compiler = new PatternCompiler();
			}
			regularExpression.Compile(compiler, (options & RegexOptions.RightToLeft) != RegexOptions.None);
			IMachineFactory machineFactory = compiler.GetMachineFactory();
			Hashtable hashtable = new Hashtable();
			machineFactory.Gap = parser.GetMapping(hashtable);
			machineFactory.Mapping = hashtable;
			machineFactory.NamesMapping = Regex.GetGroupNamesArray(machineFactory.GroupCount, machineFactory.Mapping);
			return machineFactory;
		}

		public RegexOptions Options
		{
			get
			{
				return this.roptions;
			}
		}

		public bool RightToLeft
		{
			get
			{
				return (this.roptions & RegexOptions.RightToLeft) != RegexOptions.None;
			}
		}

		public string[] GetGroupNames()
		{
			string[] array = new string[1 + this.group_count];
			Array.Copy(this.group_names, array, 1 + this.group_count);
			return array;
		}

		public int[] GetGroupNumbers()
		{
			int[] array = new int[1 + this.group_count];
			Array.Copy(this.GroupNumbers, array, 1 + this.group_count);
			return array;
		}

		public string GroupNameFromNumber(int i)
		{
			i = this.GetGroupIndex(i);
			if (i < 0)
			{
				return string.Empty;
			}
			return this.group_names[i];
		}

		public int GroupNumberFromName(string name)
		{
			if (!this.mapping.Contains(name))
			{
				return -1;
			}
			int num = (int)this.mapping[name];
			if (num >= this.gap)
			{
				num = int.Parse(name);
			}
			return num;
		}

		internal int GetGroupIndex(int number)
		{
			if (number < this.gap)
			{
				return number;
			}
			if (this.gap > this.group_count)
			{
				return -1;
			}
			return Array.BinarySearch<int>(this.GroupNumbers, this.gap, this.group_count - this.gap + 1, number);
		}

		private int default_startat(string input)
		{
			return (!this.RightToLeft || input == null) ? 0 : input.Length;
		}

		public bool IsMatch(string input)
		{
			return this.IsMatch(input, this.default_startat(input));
		}

		public bool IsMatch(string input, int startat)
		{
			return this.Match(input, startat).Success;
		}

		public Match Match(string input)
		{
			return this.Match(input, this.default_startat(input));
		}

		public Match Match(string input, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return this.CreateMachine().Scan(this, input, startat, input.Length);
		}

		public Match Match(string input, int startat, int length)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			if (length < 0 || length > input.Length - startat)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			return this.CreateMachine().Scan(this, input, startat, startat + length);
		}

		public MatchCollection Matches(string input)
		{
			return this.Matches(input, this.default_startat(input));
		}

		public MatchCollection Matches(string input, int startat)
		{
			Match match = this.Match(input, startat);
			return new MatchCollection(match);
		}

		public string Replace(string input, MatchEvaluator evaluator)
		{
			return this.Replace(input, evaluator, int.MaxValue, this.default_startat(input));
		}

		public string Replace(string input, MatchEvaluator evaluator, int count)
		{
			return this.Replace(input, evaluator, count, this.default_startat(input));
		}

		public string Replace(string input, MatchEvaluator evaluator, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (evaluator == null)
			{
				throw new ArgumentNullException("evaluator");
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			BaseMachine baseMachine = (BaseMachine)this.CreateMachine();
			if (this.RightToLeft)
			{
				return baseMachine.RTLReplace(this, input, evaluator, count, startat);
			}
			Regex.Adapter adapter = new Regex.Adapter(evaluator);
			return baseMachine.LTRReplace(this, input, new BaseMachine.MatchAppendEvaluator(adapter.Evaluate), count, startat);
		}

		public string Replace(string input, string replacement)
		{
			return this.Replace(input, replacement, int.MaxValue, this.default_startat(input));
		}

		public string Replace(string input, string replacement, int count)
		{
			return this.Replace(input, replacement, count, this.default_startat(input));
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
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return this.CreateMachine().Replace(this, input, replacement, count, startat);
		}

		public string[] Split(string input)
		{
			return this.Split(input, int.MaxValue, this.default_startat(input));
		}

		public string[] Split(string input, int count)
		{
			return this.Split(input, count, this.default_startat(input));
		}

		public string[] Split(string input, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return this.CreateMachine().Split(this, input, count, startat);
		}

		protected void InitializeReferences()
		{
			if (this.refsInitialized)
			{
				throw new NotSupportedException("This operation is only allowed once per object.");
			}
			this.refsInitialized = true;
			this.Init();
		}

		protected bool UseOptionC()
		{
			return (this.roptions & RegexOptions.Compiled) != RegexOptions.None;
		}

		protected bool UseOptionR()
		{
			return (this.roptions & RegexOptions.RightToLeft) != RegexOptions.None;
		}

		public override string ToString()
		{
			return this.pattern;
		}

		internal int GroupCount
		{
			get
			{
				return this.group_count;
			}
		}

		internal int Gap
		{
			get
			{
				return this.gap;
			}
		}

		private IMachine CreateMachine()
		{
			return this.machineFactory.NewInstance();
		}

		private static string[] GetGroupNamesArray(int groupCount, IDictionary mapping)
		{
			string[] array = new string[groupCount + 1];
			IDictionaryEnumerator enumerator = mapping.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[(int)enumerator.Value] = (string)enumerator.Key;
			}
			return array;
		}

		private int[] GroupNumbers
		{
			get
			{
				if (this.group_numbers == null)
				{
					this.group_numbers = new int[1 + this.group_count];
					for (int i = 0; i < this.gap; i++)
					{
						this.group_numbers[i] = i;
					}
					for (int j = this.gap; j <= this.group_count; j++)
					{
						this.group_numbers[j] = int.Parse(this.group_names[j]);
					}
					return this.group_numbers;
				}
				return this.group_numbers;
			}
		}

		private static FactoryCache cache = new FactoryCache(15);

		private static readonly bool old_rx = Environment.GetEnvironmentVariable("MONO_NEW_RX") == null;

		private IMachineFactory machineFactory;

		private IDictionary mapping;

		private int group_count;

		private int gap;

		private bool refsInitialized;

		private string[] group_names;

		private int[] group_numbers;

		protected internal string pattern;

		protected internal RegexOptions roptions;

		[global::System.MonoTODO]
		protected internal Hashtable capnames;

		[global::System.MonoTODO]
		protected internal Hashtable caps;

		[global::System.MonoTODO]
		protected internal RegexRunnerFactory factory;

		[global::System.MonoTODO]
		protected internal int capsize;

		[global::System.MonoTODO]
		protected internal string[] capslist;

		private class Adapter
		{
			public Adapter(MatchEvaluator ev)
			{
				this.ev = ev;
			}

			public void Evaluate(Match m, StringBuilder sb)
			{
				sb.Append(this.ev(m));
			}

			private MatchEvaluator ev;
		}
	}
}
