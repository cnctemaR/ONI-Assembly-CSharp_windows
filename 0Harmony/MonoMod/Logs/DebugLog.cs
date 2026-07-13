using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MonoMod.Utils;

namespace MonoMod.Logs
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DebugLog
	{
		public static bool IsFinalizing
		{
			get
			{
				return Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload();
			}
		}

		private DebugLog.LogMessage MakeMessage(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] global::System.ReadOnlyMemory<MessageHole> holes)
		{
			try
			{
				if (this.replayQueue == null && !DebugLog.IsFinalizing)
				{
					WeakReference<DebugLog.LogMessage> weakReference;
					while (DebugLog.messageObjectCache.TryTake(out weakReference))
					{
						DebugLog.LogMessage logMessage;
						if (weakReference.TryGetTarget(out logMessage))
						{
							logMessage.Init(source, time, level, formatted, holes);
							DebugLog.weakRefCache.Add(weakReference);
							return logMessage;
						}
						DebugLog.weakRefCache.Add(weakReference);
					}
				}
			}
			catch
			{
			}
			return new DebugLog.LogMessage(source, time, level, formatted, holes);
		}

		private void ReturnMessage(DebugLog.LogMessage message)
		{
			message.Clear();
			try
			{
				if (this.replayQueue == null && !DebugLog.IsFinalizing)
				{
					WeakReference<DebugLog.LogMessage> weakReference;
					if (DebugLog.weakRefCache.TryTake(out weakReference))
					{
						weakReference.SetTarget(message);
						DebugLog.messageObjectCache.Add(weakReference);
					}
					else
					{
						DebugLog.messageObjectCache.Add(new WeakReference<DebugLog.LogMessage>(message));
					}
				}
			}
			catch
			{
			}
		}

		public static bool IsWritingLog
		{
			get
			{
				return DebugLog.Instance.ShouldLog;
			}
		}

		internal bool AlwaysLog
		{
			get
			{
				return this.replayQueue != null || Debugger.IsAttached;
			}
		}

		internal bool ShouldLog
		{
			get
			{
				return this.subscriptions.ActiveLevels != LogLevelFilter.None || this.AlwaysLog;
			}
		}

		internal bool RecordHoles
		{
			get
			{
				return this.recordHoles || this.subscriptions.DetailLevels != LogLevelFilter.None;
			}
		}

		private void PostMessage(DebugLog.LogMessage message)
		{
			if (Debugger.IsAttached)
			{
				try
				{
					int level = (int)message.Level;
					string source = message.Source;
					FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(6, 3);
					formatInterpolatedStringHandler.AppendLiteral("[");
					formatInterpolatedStringHandler.AppendFormatted(message.Source);
					formatInterpolatedStringHandler.AppendLiteral("] ");
					formatInterpolatedStringHandler.AppendFormatted(message.Level.FastToString(null));
					formatInterpolatedStringHandler.AppendLiteral(": ");
					formatInterpolatedStringHandler.AppendFormatted(message.FormattedMessage);
					formatInterpolatedStringHandler.AppendLiteral("\n");
					Debugger.Log(level, source, DebugFormatter.Format(ref formatInterpolatedStringHandler));
				}
				catch
				{
				}
			}
			try
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.subscriptions;
				int level2 = (int)message.Level;
				DebugLog.OnLogMessage onLogMessage = levelSubscriptions.SimpleRegs[level2];
				if (onLogMessage != null)
				{
					message.ReportTo(onLogMessage);
				}
				DebugLog.OnLogMessageDetailed onLogMessageDetailed = levelSubscriptions.DetailedRegs[level2];
				if (onLogMessageDetailed != null)
				{
					message.ReportTo(onLogMessageDetailed);
				}
				if (!DebugLog.IsFinalizing)
				{
					ConcurrentQueue<DebugLog.LogMessage> concurrentQueue = this.replayQueue;
					if (concurrentQueue != null)
					{
						concurrentQueue.Enqueue(message);
						while (concurrentQueue.Count > this.replayQueueLength)
						{
							DebugLog.LogMessage logMessage;
							if (!concurrentQueue.TryDequeue(out logMessage))
							{
								break;
							}
						}
					}
					else
					{
						this.ReturnMessage(message);
					}
				}
			}
			catch
			{
			}
		}

		internal bool ShouldLogLevel(LogLevel level)
		{
			return ((1 << (int)level) & (int)this.subscriptions.ActiveLevels) != 0 || (((1 << (int)level) & (int)this.globalFilter) != 0 && this.AlwaysLog);
		}

		internal bool ShouldLevelRecordHoles(LogLevel level)
		{
			return this.recordHoles || ((1 << (int)level) & (int)this.subscriptions.DetailLevels) != 0;
		}

		public void Write(string source, DateTime time, LogLevel level, string message)
		{
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.PostMessage(this.MakeMessage(source, time, level, message, default(global::System.ReadOnlyMemory<MessageHole>)));
		}

		public void Write(string source, DateTime time, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			if (!message.enabled)
			{
				return;
			}
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			global::System.ReadOnlyMemory<MessageHole> readOnlyMemory;
			string text = message.ToStringAndClear(out readOnlyMemory);
			this.PostMessage(this.MakeMessage(source, time, level, text, readOnlyMemory));
		}

		internal void LogCore(string source, LogLevel level, string message)
		{
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.Write(source, DateTime.UtcNow, level, message);
		}

		internal void LogCore(string source, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			if (!message.enabled)
			{
				return;
			}
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.Write(source, DateTime.UtcNow, level, ref message);
		}

		public static void Log(string source, LogLevel level, string message)
		{
			DebugLog instance = DebugLog.Instance;
			if (!instance.ShouldLogLevel(level))
			{
				return;
			}
			instance.Write(source, DateTime.UtcNow, level, message);
		}

		public static void Log(string source, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			DebugLog instance = DebugLog.Instance;
			if (!message.enabled)
			{
				return;
			}
			if (!instance.ShouldLogLevel(level))
			{
				return;
			}
			instance.Write(source, DateTime.UtcNow, level, ref message);
		}

		[return: Nullable(new byte[] { 2, 1 })]
		private static string[] GetListEnvVar(string text)
		{
			string text2 = text.Trim();
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			string[] array = text2.Split(DebugLog.listEnvSeparator, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			return array;
		}

		private DebugLog()
		{
			bool flag;
			this.recordHoles = Switches.TryGetSwitchEnabled("LogRecordHoles", out flag) && flag;
			this.replayQueueLength = 0;
			object obj;
			if (Switches.TryGetSwitchValue("LogReplayQueueLength", out obj))
			{
				this.replayQueueLength = (obj as int?).GetValueOrDefault();
			}
			if (Switches.TryGetSwitchEnabled("LogSpam", out flag) && flag)
			{
				this.globalFilter |= LogLevelFilter.Spam;
			}
			if (this.replayQueueLength > 0)
			{
				this.replayQueue = new ConcurrentQueue<DebugLog.LogMessage>();
			}
			string text = (Switches.TryGetSwitchValue("LogToFile", out obj) ? (obj as string) : null);
			string[] array = null;
			if (Switches.TryGetSwitchValue("LogToFileFilter", out obj))
			{
				string[] array2 = obj as string[];
				string[] array3;
				if (array2 == null)
				{
					string text2 = obj as string;
					if (text2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = DebugLog.GetListEnvVar(text2);
					}
				}
				else
				{
					array3 = array2;
				}
				array = array3;
			}
			if (text != null)
			{
				this.TryInitializeLogToFile(text, array, this.globalFilter);
			}
			if (Switches.TryGetSwitchEnabled("LogInMemory", out flag) && flag)
			{
				this.TryInitializeMemoryLog(this.globalFilter);
			}
		}

		private void TryInitializeLogToFile(string file, [Nullable(new byte[] { 2, 1 })] string[] sourceFilter, LogLevelFilter filter)
		{
			try
			{
				StringComparer comparer = StringComparerEx.FromComparison(StringComparison.OrdinalIgnoreCase);
				if (sourceFilter != null)
				{
					Array.Sort<string>(sourceFilter, comparer);
				}
				object sync = new object();
				TextWriter writer;
				if (file == "-")
				{
					writer = Console.Out;
				}
				else
				{
					FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write);
					writer = new StreamWriter(fileStream, Encoding.UTF8)
					{
						AutoFlush = true
					};
				}
				this.SubscribeCore(filter, delegate(string source, DateTime time, LogLevel level, string msg)
				{
					if (sourceFilter != null && sourceFilter.AsSpan<string>().BinarySearch(source, comparer) < 0)
					{
						return;
					}
					DateTime dateTime = time.ToLocalTime();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 4);
					defaultInterpolatedStringHandler.AppendLiteral("[");
					defaultInterpolatedStringHandler.AppendFormatted(source);
					defaultInterpolatedStringHandler.AppendLiteral("](");
					defaultInterpolatedStringHandler.AppendFormatted<DateTime>(dateTime);
					defaultInterpolatedStringHandler.AppendLiteral(") ");
					defaultInterpolatedStringHandler.AppendFormatted(level.FastToString(null));
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(msg);
					string text = defaultInterpolatedStringHandler.ToStringAndClear();
					object sync2 = sync;
					lock (sync2)
					{
						writer.WriteLine(text);
					}
				});
			}
			catch (Exception ex)
			{
				LogLevel logLevel = LogLevel.Error;
				LogLevel logLevel2 = logLevel;
				bool flag;
				DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(61, 1, logLevel, out flag);
				if (flag)
				{
					debugLogInterpolatedStringHandler.AppendLiteral("Exception while trying to initialize writing logs to a file: ");
					debugLogInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				}
				DebugLog.Instance.LogCore("DebugLog", logLevel2, ref debugLogInterpolatedStringHandler);
			}
		}

		private void TryInitializeMemoryLog(LogLevelFilter filter)
		{
			try
			{
				DebugLog.memlogPos = 0;
				DebugLog.memlog = new byte[4096];
				object sync = new object();
				Encoding utf = Encoding.UTF8;
				this.SubscribeCore(filter, delegate(string source, DateTime time, LogLevel level, string msg)
				{
					byte b = (byte)level;
					long ticks = time.Ticks;
					if (source.Length > 255)
					{
						source = source.Substring(0, 255);
					}
					byte b2 = (byte)source.Length;
					int length = msg.Length;
					int num = (int)(14 + b2 * 2) + length * 2;
					object sync2 = sync;
					lock (sync2)
					{
						if (DebugLog.memlog.Length - DebugLog.memlogPos < num)
						{
							int num2 = DebugLog.memlog.Length * 4;
							while (num2 - DebugLog.memlogPos < num)
							{
								num2 *= 4;
							}
							Array.Resize<byte>(ref DebugLog.memlog, num2);
						}
						ref byte reference = ref global::System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(DebugLog.memlog.AsSpan<byte>().Slice(DebugLog.memlogPos));
						int num3 = 0;
						Unsafe.WriteUnaligned<byte>(Unsafe.Add<byte>(ref reference, num3), b);
						num3++;
						Unsafe.WriteUnaligned<long>(Unsafe.Add<byte>(ref reference, num3), ticks);
						num3 += 8;
						Unsafe.WriteUnaligned<byte>(Unsafe.Add<byte>(ref reference, num3), b2);
						num3++;
						Unsafe.CopyBlock(Unsafe.Add<byte>(ref reference, num3), Unsafe.As<char, byte>(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<char>(source.AsSpan())), (uint)(b2 * 2));
						num3 += (int)(b2 * 2);
						Unsafe.WriteUnaligned<int>(Unsafe.Add<byte>(ref reference, num3), length);
						num3 += 4;
						Unsafe.CopyBlock(Unsafe.Add<byte>(ref reference, num3), Unsafe.As<char, byte>(global::System.Runtime.InteropServices.MemoryMarshal.GetReference<char>(msg.AsSpan())), (uint)(length * 2));
						num3 += length * 2;
						DebugLog.memlogPos += num3;
					}
				});
			}
			catch (Exception ex)
			{
				LogLevel logLevel = LogLevel.Error;
				LogLevel logLevel2 = logLevel;
				bool flag;
				DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(45, 1, logLevel, out flag);
				if (flag)
				{
					debugLogInterpolatedStringHandler.AppendLiteral("Exception while initializing the memory log: ");
					debugLogInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				}
				DebugLog.Instance.LogCore("DebugLog", logLevel2, ref debugLogInterpolatedStringHandler);
			}
		}

		private void MaybeReplayTo(LogLevelFilter filter, DebugLog.OnLogMessage del)
		{
			if (this.replayQueue == null || filter == LogLevelFilter.None)
			{
				return;
			}
			foreach (DebugLog.LogMessage logMessage in this.replayQueue.ToArray())
			{
				if (((1 << (int)logMessage.Level) & (int)filter) != 0)
				{
					logMessage.ReportTo(del);
				}
			}
		}

		private void MaybeReplayTo(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
		{
			if (this.replayQueue == null || filter == LogLevelFilter.None)
			{
				return;
			}
			foreach (DebugLog.LogMessage logMessage in this.replayQueue.ToArray())
			{
				if (((1 << (int)logMessage.Level) & (int)filter) != 0)
				{
					logMessage.ReportTo(del);
				}
			}
		}

		public static IDisposable Subscribe(LogLevelFilter filter, DebugLog.OnLogMessage value)
		{
			return DebugLog.Instance.SubscribeCore(filter, value);
		}

		private IDisposable SubscribeCore(LogLevelFilter filter, DebugLog.OnLogMessage value)
		{
			DebugLog.LevelSubscriptions levelSubscriptions;
			DebugLog.LevelSubscriptions levelSubscriptions2;
			do
			{
				levelSubscriptions = this.subscriptions;
				levelSubscriptions2 = levelSubscriptions.AddSimple(filter, value);
			}
			while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.subscriptions, levelSubscriptions2, levelSubscriptions) != levelSubscriptions);
			this.MaybeReplayTo(filter, value);
			return new DebugLog.LogSubscriptionSimple(this, value, filter);
		}

		public static IDisposable Subscribe(LogLevelFilter filter, DebugLog.OnLogMessageDetailed value)
		{
			return DebugLog.Instance.SubscribeCore(filter, value);
		}

		private IDisposable SubscribeCore(LogLevelFilter filter, DebugLog.OnLogMessageDetailed value)
		{
			DebugLog.LevelSubscriptions levelSubscriptions;
			DebugLog.LevelSubscriptions levelSubscriptions2;
			do
			{
				levelSubscriptions = this.subscriptions;
				levelSubscriptions2 = levelSubscriptions.AddDetailed(filter, value);
			}
			while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.subscriptions, levelSubscriptions2, levelSubscriptions) != levelSubscriptions);
			this.MaybeReplayTo(filter, value);
			return new DebugLog.LogSubscriptionDetailed(this, value, filter);
		}

		public static event DebugLog.OnLogMessage OnLog
		{
			add
			{
				IDisposable res = DebugLog.Subscribe(DebugLog.Instance.globalFilter, value);
				DebugLog.simpleRegDict.AddOrUpdate(value, res, delegate(DebugLog.OnLogMessage _, IDisposable d)
				{
					d.Dispose();
					return res;
				});
			}
			remove
			{
				IDisposable disposable;
				if (DebugLog.simpleRegDict.TryRemove(value, out disposable))
				{
					disposable.Dispose();
				}
			}
		}

		internal static readonly DebugLog Instance = new DebugLog();

		private static readonly ConcurrentBag<WeakReference<DebugLog.LogMessage>> weakRefCache = new ConcurrentBag<WeakReference<DebugLog.LogMessage>>();

		private static readonly ConcurrentBag<WeakReference<DebugLog.LogMessage>> messageObjectCache = new ConcurrentBag<WeakReference<DebugLog.LogMessage>>();

		private static readonly char[] listEnvSeparator = new char[] { ' ', ';', ',' };

		private readonly bool recordHoles;

		private readonly int replayQueueLength;

		[Nullable(new byte[] { 2, 1 })]
		private readonly ConcurrentQueue<DebugLog.LogMessage> replayQueue;

		private LogLevelFilter globalFilter = LogLevelFilter.DefaultFilter;

		[Nullable(2)]
		private static byte[] memlog;

		private static int memlogPos;

		private DebugLog.LevelSubscriptions subscriptions = DebugLog.LevelSubscriptions.None;

		private static readonly ConcurrentDictionary<DebugLog.OnLogMessage, IDisposable> simpleRegDict = new ConcurrentDictionary<DebugLog.OnLogMessage, IDisposable>();

		[NullableContext(0)]
		public delegate void OnLogMessage(string source, DateTime time, LogLevel level, string message);

		[NullableContext(0)]
		public delegate void OnLogMessageDetailed(string source, DateTime time, LogLevel level, string formattedMessage, [Nullable(0)] global::System.ReadOnlyMemory<MessageHole> holes);

		[Nullable(0)]
		private sealed class LogMessage
		{
			public string Source { get; private set; }

			public DateTime Time { get; private set; }

			public LogLevel Level { get; private set; }

			public string FormattedMessage { get; private set; }

			[Nullable(0)]
			public global::System.ReadOnlyMemory<MessageHole> FormatHoles
			{
				[NullableContext(0)]
				get;
				[NullableContext(0)]
				private set;
			}

			public LogMessage(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] global::System.ReadOnlyMemory<MessageHole> holes)
			{
				this.Source = source;
				this.Time = time;
				this.Level = level;
				this.FormattedMessage = formatted;
				this.FormatHoles = holes;
			}

			public void Clear()
			{
				this.Source = "";
				this.Time = default(DateTime);
				this.Level = LogLevel.Spam;
				this.FormattedMessage = "";
				this.FormatHoles = default(global::System.ReadOnlyMemory<MessageHole>);
			}

			public void Init(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] global::System.ReadOnlyMemory<MessageHole> holes)
			{
				this.Source = source;
				this.Time = time;
				this.Level = level;
				this.FormattedMessage = formatted;
				this.FormatHoles = holes;
			}

			public void ReportTo(DebugLog.OnLogMessage del)
			{
				try
				{
					del(this.Source, this.Time, this.Level, this.FormattedMessage);
				}
				catch (Exception ex)
				{
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", "Exception caught while reporting to message handler");
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", ex.ToString());
				}
			}

			public void ReportTo(DebugLog.OnLogMessageDetailed del)
			{
				try
				{
					del(this.Source, this.Time, this.Level, this.FormattedMessage, this.FormatHoles);
				}
				catch (Exception ex)
				{
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", "Exception caught while reporting to message handler");
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", ex.ToString());
				}
			}
		}

		[Nullable(0)]
		private sealed class LevelSubscriptions
		{
			private LevelSubscriptions(LogLevelFilter active, LogLevelFilter detail, [Nullable(new byte[] { 1, 2 })] DebugLog.OnLogMessage[] simple, [Nullable(new byte[] { 1, 2 })] DebugLog.OnLogMessageDetailed[] detailed)
			{
				this.ActiveLevels = active | detail;
				this.DetailLevels = detail;
				this.SimpleRegs = simple;
				this.DetailedRegs = detailed;
			}

			private LevelSubscriptions()
			{
				this.ActiveLevels = LogLevelFilter.None;
				this.DetailLevels = LogLevelFilter.None;
				this.SimpleRegs = new DebugLog.OnLogMessage[6];
				this.DetailedRegs = new DebugLog.OnLogMessageDetailed[this.SimpleRegs.Length];
			}

			private DebugLog.LevelSubscriptions Clone(bool changingDetail)
			{
				DebugLog.OnLogMessage[] array = this.SimpleRegs;
				DebugLog.OnLogMessageDetailed[] array2 = this.DetailedRegs;
				if (!changingDetail)
				{
					array = new DebugLog.OnLogMessage[this.SimpleRegs.Length];
					Array.Copy(this.SimpleRegs, array, array.Length);
				}
				else
				{
					array2 = new DebugLog.OnLogMessageDetailed[this.DetailedRegs.Length];
					Array.Copy(this.DetailedRegs, array2, array2.Length);
				}
				return new DebugLog.LevelSubscriptions(this.ActiveLevels, this.DetailLevels, array, array2);
			}

			private void FixFilters()
			{
				this.ActiveLevels &= LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;
				this.DetailLevels &= LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;
			}

			public DebugLog.LevelSubscriptions AddSimple(LogLevelFilter filter, DebugLog.OnLogMessage del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(false);
				levelSubscriptions.ActiveLevels |= filter;
				for (int i = 0; i < levelSubscriptions.SimpleRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None)
					{
						Helpers.EventAdd<DebugLog.OnLogMessage>(ref levelSubscriptions.SimpleRegs[i], del);
					}
				}
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			public DebugLog.LevelSubscriptions RemoveSimple(LogLevelFilter filter, DebugLog.OnLogMessage del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(false);
				for (int i = 0; i < levelSubscriptions.SimpleRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None && Helpers.EventRemove<DebugLog.OnLogMessage>(ref levelSubscriptions.SimpleRegs[i], del) == null)
					{
						levelSubscriptions.ActiveLevels &= (LogLevelFilter)(~(LogLevelFilter)(1 << i));
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			public DebugLog.LevelSubscriptions AddDetailed(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(true);
				levelSubscriptions.DetailLevels |= filter;
				for (int i = 0; i < levelSubscriptions.DetailedRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None)
					{
						Helpers.EventAdd<DebugLog.OnLogMessageDetailed>(ref levelSubscriptions.DetailedRegs[i], del);
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			public DebugLog.LevelSubscriptions RemoveDetailed(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(true);
				for (int i = 0; i < levelSubscriptions.DetailedRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None && Helpers.EventRemove<DebugLog.OnLogMessageDetailed>(ref levelSubscriptions.DetailedRegs[i], del) == null)
					{
						levelSubscriptions.DetailLevels &= (LogLevelFilter)(~(LogLevelFilter)(1 << i));
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			public LogLevelFilter ActiveLevels;

			public LogLevelFilter DetailLevels;

			[Nullable(new byte[] { 1, 2 })]
			public readonly DebugLog.OnLogMessage[] SimpleRegs;

			[Nullable(new byte[] { 1, 2 })]
			public readonly DebugLog.OnLogMessageDetailed[] DetailedRegs;

			private const LogLevelFilter ValidFilter = LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;

			public static readonly DebugLog.LevelSubscriptions None = new DebugLog.LevelSubscriptions();
		}

		[Nullable(0)]
		private sealed class LogSubscriptionSimple : IDisposable
		{
			public LogSubscriptionSimple(DebugLog log, DebugLog.OnLogMessage del, LogLevelFilter filter)
			{
				this.log = log;
				this.del = del;
				this.filter = filter;
			}

			public void Dispose()
			{
				DebugLog.LevelSubscriptions subscriptions;
				DebugLog.LevelSubscriptions levelSubscriptions;
				do
				{
					subscriptions = this.log.subscriptions;
					levelSubscriptions = subscriptions.RemoveSimple(this.filter, this.del);
				}
				while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.log.subscriptions, levelSubscriptions, subscriptions) != subscriptions);
			}

			private readonly DebugLog log;

			private readonly DebugLog.OnLogMessage del;

			private readonly LogLevelFilter filter;
		}

		[Nullable(0)]
		private sealed class LogSubscriptionDetailed : IDisposable
		{
			public LogSubscriptionDetailed(DebugLog log, DebugLog.OnLogMessageDetailed del, LogLevelFilter filter)
			{
				this.log = log;
				this.del = del;
				this.filter = filter;
			}

			public void Dispose()
			{
				DebugLog.LevelSubscriptions subscriptions;
				DebugLog.LevelSubscriptions levelSubscriptions;
				do
				{
					subscriptions = this.log.subscriptions;
					levelSubscriptions = subscriptions.RemoveDetailed(this.filter, this.del);
				}
				while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.log.subscriptions, levelSubscriptions, subscriptions) != subscriptions);
			}

			private readonly DebugLog log;

			private readonly DebugLog.OnLogMessageDetailed del;

			private readonly LogLevelFilter filter;
		}
	}
}
