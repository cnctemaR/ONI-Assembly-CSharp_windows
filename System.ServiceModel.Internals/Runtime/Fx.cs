using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System.Runtime
{
	internal static class Fx
	{
		public static ExceptionTrace Exception
		{
			get
			{
				if (Fx.exceptionTrace == null)
				{
					Fx.exceptionTrace = new ExceptionTrace("System.Runtime", Fx.Trace);
				}
				return Fx.exceptionTrace;
			}
		}

		public static EtwDiagnosticTrace Trace
		{
			get
			{
				if (Fx.diagnosticTrace == null)
				{
					Fx.diagnosticTrace = Fx.InitializeTracing();
				}
				return Fx.diagnosticTrace;
			}
		}

		[SecuritySafeCritical]
		private static EtwDiagnosticTrace InitializeTracing()
		{
			EtwDiagnosticTrace etwDiagnosticTrace = new EtwDiagnosticTrace("System.Runtime", EtwDiagnosticTrace.DefaultEtwProviderId);
			if (etwDiagnosticTrace.EtwProvider != null)
			{
				EtwDiagnosticTrace etwDiagnosticTrace2 = etwDiagnosticTrace;
				etwDiagnosticTrace2.RefreshState = (Action)Delegate.Combine(etwDiagnosticTrace2.RefreshState, new Action(delegate
				{
					Fx.UpdateLevel();
				}));
			}
			Fx.UpdateLevel(etwDiagnosticTrace);
			return etwDiagnosticTrace;
		}

		public static Fx.ExceptionHandler AsynchronousThreadExceptionHandler
		{
			[SecuritySafeCritical]
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return Fx.asynchronousThreadExceptionHandler;
			}
			[SecurityCritical]
			set
			{
				Fx.asynchronousThreadExceptionHandler = value;
			}
		}

		[Conditional("DEBUG")]
		public static void Assert(bool condition, string description)
		{
		}

		[Conditional("DEBUG")]
		public static void Assert(string description)
		{
			AssertHelper.FireAssert(description);
		}

		public static void AssertAndThrow(bool condition, string description)
		{
			if (!condition)
			{
				Fx.AssertAndThrow(description);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Exception AssertAndThrow(string description)
		{
			TraceCore.ShipAssertExceptionMessage(Fx.Trace, description);
			throw new Fx.InternalException(description);
		}

		public static void AssertAndThrowFatal(bool condition, string description)
		{
			if (!condition)
			{
				Fx.AssertAndThrowFatal(description);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Exception AssertAndThrowFatal(string description)
		{
			TraceCore.ShipAssertExceptionMessage(Fx.Trace, description);
			throw new Fx.FatalInternalException(description);
		}

		public static void AssertAndFailFast(bool condition, string description)
		{
			if (!condition)
			{
				Fx.AssertAndFailFast(description);
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Exception AssertAndFailFast(string description)
		{
			string text = InternalSR.FailFastMessage(description);
			try
			{
				try
				{
					Fx.Exception.TraceFailFast(text);
				}
				finally
				{
					Environment.FailFast(text);
				}
			}
			catch
			{
				throw;
			}
			return null;
		}

		public static bool IsFatal(Exception exception)
		{
			while (exception != null)
			{
				if (exception is FatalException || (exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) || exception is ThreadAbortException || exception is Fx.FatalInternalException)
				{
					return true;
				}
				if (exception is TypeInitializationException || exception is TargetInvocationException)
				{
					exception = exception.InnerException;
				}
				else
				{
					if (exception is AggregateException)
					{
						using (IEnumerator<Exception> enumerator = ((AggregateException)exception).InnerExceptions.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (Fx.IsFatal(enumerator.Current))
								{
									return true;
								}
							}
							break;
						}
						continue;
					}
					break;
				}
			}
			return false;
		}

		internal static bool AssertsFailFast
		{
			get
			{
				return false;
			}
		}

		internal static Type[] BreakOnExceptionTypes
		{
			get
			{
				return null;
			}
		}

		internal static bool FastDebug
		{
			get
			{
				return false;
			}
		}

		internal static bool StealthDebugger
		{
			get
			{
				return false;
			}
		}

		public static Action<T1> ThunkCallback<T1>(Action<T1> callback)
		{
			return new Fx.ActionThunk<T1>(callback).ThunkFrame;
		}

		public static AsyncCallback ThunkCallback(AsyncCallback callback)
		{
			return new Fx.AsyncThunk(callback).ThunkFrame;
		}

		public static WaitCallback ThunkCallback(WaitCallback callback)
		{
			return new Fx.WaitThunk(callback).ThunkFrame;
		}

		public static TimerCallback ThunkCallback(TimerCallback callback)
		{
			return new Fx.TimerThunk(callback).ThunkFrame;
		}

		public static WaitOrTimerCallback ThunkCallback(WaitOrTimerCallback callback)
		{
			return new Fx.WaitOrTimerThunk(callback).ThunkFrame;
		}

		public static SendOrPostCallback ThunkCallback(SendOrPostCallback callback)
		{
			return new Fx.SendOrPostThunk(callback).ThunkFrame;
		}

		[SecurityCritical]
		public static IOCompletionCallback ThunkCallback(IOCompletionCallback callback)
		{
			return new Fx.IOCompletionThunk(callback).ThunkFrame;
		}

		public static Guid CreateGuid(string guidString)
		{
			bool flag = false;
			Guid guid = Guid.Empty;
			try
			{
				guid = new Guid(guidString);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					Fx.AssertAndThrow("Creation of the Guid failed.");
				}
			}
			return guid;
		}

		public static bool TryCreateGuid(string guidString, out Guid result)
		{
			bool flag = false;
			result = Guid.Empty;
			try
			{
				result = new Guid(guidString);
				flag = true;
			}
			catch (ArgumentException)
			{
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			return flag;
		}

		public static byte[] AllocateByteArray(int size)
		{
			byte[] array;
			try
			{
				array = new byte[size];
			}
			catch (OutOfMemoryException ex)
			{
				throw Fx.Exception.AsError(new InsufficientMemoryException(InternalSR.BufferAllocationFailed(size), ex));
			}
			return array;
		}

		public static char[] AllocateCharArray(int size)
		{
			char[] array;
			try
			{
				array = new char[size];
			}
			catch (OutOfMemoryException ex)
			{
				throw Fx.Exception.AsError(new InsufficientMemoryException(InternalSR.BufferAllocationFailed(size * 2), ex));
			}
			return array;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static void TraceExceptionNoThrow(Exception exception)
		{
			try
			{
				Fx.Exception.TraceUnhandledException(exception);
			}
			catch
			{
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static bool HandleAtThreadBase(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}
			Fx.TraceExceptionNoThrow(exception);
			try
			{
				Fx.ExceptionHandler exceptionHandler = Fx.AsynchronousThreadExceptionHandler;
				return exceptionHandler != null && exceptionHandler.HandleException(exception);
			}
			catch (Exception ex)
			{
				Fx.TraceExceptionNoThrow(ex);
			}
			return false;
		}

		private static void UpdateLevel(EtwDiagnosticTrace trace)
		{
			if (trace == null)
			{
				return;
			}
			if (TraceCore.ActionItemCallbackInvokedIsEnabled(trace) || TraceCore.ActionItemScheduledIsEnabled(trace))
			{
				trace.SetEnd2EndActivityTracingEnabled(true);
			}
		}

		private static void UpdateLevel()
		{
			Fx.UpdateLevel(Fx.Trace);
		}

		private const string defaultEventSource = "System.Runtime";

		private static ExceptionTrace exceptionTrace;

		private static EtwDiagnosticTrace diagnosticTrace;

		[SecurityCritical]
		private static Fx.ExceptionHandler asynchronousThreadExceptionHandler;

		public abstract class ExceptionHandler
		{
			public abstract bool HandleException(Exception exception);
		}

		public static class Tag
		{
			public enum CacheAttrition
			{
				None,
				ElementOnTimer,
				ElementOnGC,
				ElementOnCallback,
				FullPurgeOnTimer,
				FullPurgeOnEachAccess,
				PartialPurgeOnTimer,
				PartialPurgeOnEachAccess
			}

			public enum ThrottleAction
			{
				Reject,
				Pause
			}

			public enum ThrottleMetric
			{
				Count,
				Rate,
				Other
			}

			public enum Location
			{
				InProcess,
				OutOfProcess,
				LocalSystem,
				LocalOrRemoteSystem,
				RemoteSystem
			}

			public enum SynchronizationKind
			{
				LockStatement,
				MonitorWait,
				MonitorExplicit,
				InterlockedNoSpin,
				InterlockedWithSpin,
				FromFieldType
			}

			[Flags]
			public enum BlocksUsing
			{
				MonitorEnter = 0,
				MonitorWait = 1,
				ManualResetEvent = 2,
				AutoResetEvent = 3,
				AsyncResult = 4,
				IAsyncResult = 5,
				PInvoke = 6,
				InputQueue = 7,
				ThreadNeutralSemaphore = 8,
				PrivatePrimitive = 9,
				OtherInternalPrimitive = 10,
				OtherFrameworkPrimitive = 11,
				OtherInterop = 12,
				Other = 13,
				NonBlocking = 14
			}

			public static class Strings
			{
				internal const string ExternallyManaged = "externally managed";

				internal const string AppDomain = "AppDomain";

				internal const string DeclaringInstance = "instance of declaring class";

				internal const string Unbounded = "unbounded";

				internal const string Infinite = "infinite";
			}

			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
			[Conditional("DEBUG")]
			public sealed class FriendAccessAllowedAttribute : Attribute
			{
				public FriendAccessAllowedAttribute(string assemblyName)
				{
					this.AssemblyName = assemblyName;
				}

				public string AssemblyName { get; set; }
			}

			public static class Throws
			{
				[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
				[Conditional("CODE_ANALYSIS_CDF")]
				public sealed class TimeoutAttribute : Fx.Tag.ThrowsAttribute
				{
					public TimeoutAttribute()
						: this("The operation timed out.")
					{
					}

					public TimeoutAttribute(string diagnosis)
						: base(typeof(TimeoutException), diagnosis)
					{
					}
				}
			}

			[AttributeUsage(AttributeTargets.Field)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class CacheAttribute : Attribute
			{
				public CacheAttribute(Type elementType, Fx.Tag.CacheAttrition cacheAttrition)
				{
					this.Scope = "instance of declaring class";
					this.SizeLimit = "unbounded";
					this.Timeout = "infinite";
					if (elementType == null)
					{
						throw Fx.Exception.ArgumentNull("elementType");
					}
					this.elementType = elementType;
					this.cacheAttrition = cacheAttrition;
				}

				public Type ElementType
				{
					get
					{
						return this.elementType;
					}
				}

				public Fx.Tag.CacheAttrition CacheAttrition
				{
					get
					{
						return this.cacheAttrition;
					}
				}

				public string Scope { get; set; }

				public string SizeLimit { get; set; }

				public string Timeout { get; set; }

				private readonly Type elementType;

				private readonly Fx.Tag.CacheAttrition cacheAttrition;
			}

			[AttributeUsage(AttributeTargets.Field)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class QueueAttribute : Attribute
			{
				public QueueAttribute(Type elementType)
				{
					this.Scope = "instance of declaring class";
					this.SizeLimit = "unbounded";
					if (elementType == null)
					{
						throw Fx.Exception.ArgumentNull("elementType");
					}
					this.elementType = elementType;
				}

				public Type ElementType
				{
					get
					{
						return this.elementType;
					}
				}

				public string Scope { get; set; }

				public string SizeLimit { get; set; }

				public bool StaleElementsRemovedImmediately { get; set; }

				public bool EnqueueThrowsIfFull { get; set; }

				private readonly Type elementType;
			}

			[AttributeUsage(AttributeTargets.Field)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class ThrottleAttribute : Attribute
			{
				public ThrottleAttribute(Fx.Tag.ThrottleAction throttleAction, Fx.Tag.ThrottleMetric throttleMetric, string limit)
				{
					this.Scope = "AppDomain";
					if (string.IsNullOrEmpty(limit))
					{
						throw Fx.Exception.ArgumentNullOrEmpty("limit");
					}
					this.throttleAction = throttleAction;
					this.throttleMetric = throttleMetric;
					this.limit = limit;
				}

				public Fx.Tag.ThrottleAction ThrottleAction
				{
					get
					{
						return this.throttleAction;
					}
				}

				public Fx.Tag.ThrottleMetric ThrottleMetric
				{
					get
					{
						return this.throttleMetric;
					}
				}

				public string Limit
				{
					get
					{
						return this.limit;
					}
				}

				public string Scope { get; set; }

				private readonly Fx.Tag.ThrottleAction throttleAction;

				private readonly Fx.Tag.ThrottleMetric throttleMetric;

				private readonly string limit;
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class ExternalResourceAttribute : Attribute
			{
				public ExternalResourceAttribute(Fx.Tag.Location location, string description)
				{
					this.location = location;
					this.description = description;
				}

				public Fx.Tag.Location Location
				{
					get
					{
						return this.location;
					}
				}

				public string Description
				{
					get
					{
						return this.description;
					}
				}

				private readonly Fx.Tag.Location location;

				private readonly string description;
			}

			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class SynchronizationObjectAttribute : Attribute
			{
				public SynchronizationObjectAttribute()
				{
					this.Blocking = true;
					this.Scope = "instance of declaring class";
					this.Kind = Fx.Tag.SynchronizationKind.FromFieldType;
				}

				public bool Blocking { get; set; }

				public string Scope { get; set; }

				public Fx.Tag.SynchronizationKind Kind { get; set; }
			}

			[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class SynchronizationPrimitiveAttribute : Attribute
			{
				public SynchronizationPrimitiveAttribute(Fx.Tag.BlocksUsing blocksUsing)
				{
					this.blocksUsing = blocksUsing;
				}

				public Fx.Tag.BlocksUsing BlocksUsing
				{
					get
					{
						return this.blocksUsing;
					}
				}

				public bool SupportsAsync { get; set; }

				public bool Spins { get; set; }

				public string ReleaseMethod { get; set; }

				private readonly Fx.Tag.BlocksUsing blocksUsing;
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class BlockingAttribute : Attribute
			{
				public string CancelMethod { get; set; }

				public Type CancelDeclaringType { get; set; }

				public string Conditional { get; set; }
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class GuaranteeNonBlockingAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class NonThrowingAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public class ThrowsAttribute : Attribute
			{
				public ThrowsAttribute(Type exceptionType, string diagnosis)
				{
					if (exceptionType == null)
					{
						throw Fx.Exception.ArgumentNull("exceptionType");
					}
					if (string.IsNullOrEmpty(diagnosis))
					{
						throw Fx.Exception.ArgumentNullOrEmpty("diagnosis");
					}
					this.exceptionType = exceptionType;
					this.diagnosis = diagnosis;
				}

				public Type ExceptionType
				{
					get
					{
						return this.exceptionType;
					}
				}

				public string Diagnosis
				{
					get
					{
						return this.diagnosis;
					}
				}

				private readonly Type exceptionType;

				private readonly string diagnosis;
			}

			[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class InheritThrowsAttribute : Attribute
			{
				public Type FromDeclaringType { get; set; }

				public string From { get; set; }
			}

			[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class KnownXamlExternalAttribute : Attribute
			{
			}

			[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class XamlVisibleAttribute : Attribute
			{
				public XamlVisibleAttribute()
					: this(true)
				{
				}

				public XamlVisibleAttribute(bool visible)
				{
					this.Visible = visible;
				}

				public bool Visible { get; private set; }
			}

			[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
			[Conditional("CODE_ANALYSIS_CDF")]
			public sealed class SecurityNoteAttribute : Attribute
			{
				public string Critical { get; set; }

				public string Safe { get; set; }

				public string Miscellaneous { get; set; }
			}
		}

		private abstract class Thunk<T> where T : class
		{
			[SecuritySafeCritical]
			protected Thunk(T callback)
			{
				this.callback = callback;
			}

			internal T Callback
			{
				[SecuritySafeCritical]
				get
				{
					return this.callback;
				}
			}

			[SecurityCritical]
			private T callback;
		}

		private sealed class ActionThunk<T1> : Fx.Thunk<Action<T1>>
		{
			public ActionThunk(Action<T1> callback)
				: base(callback)
			{
			}

			public Action<T1> ThunkFrame
			{
				get
				{
					return new Action<T1>(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(T1 result)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(result);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		private sealed class AsyncThunk : Fx.Thunk<AsyncCallback>
		{
			public AsyncThunk(AsyncCallback callback)
				: base(callback)
			{
			}

			public AsyncCallback ThunkFrame
			{
				get
				{
					return new AsyncCallback(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(IAsyncResult result)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(result);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		private sealed class WaitThunk : Fx.Thunk<WaitCallback>
		{
			public WaitThunk(WaitCallback callback)
				: base(callback)
			{
			}

			public WaitCallback ThunkFrame
			{
				get
				{
					return new WaitCallback(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(object state)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(state);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		private sealed class TimerThunk : Fx.Thunk<TimerCallback>
		{
			public TimerThunk(TimerCallback callback)
				: base(callback)
			{
			}

			public TimerCallback ThunkFrame
			{
				get
				{
					return new TimerCallback(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(object state)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(state);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		private sealed class WaitOrTimerThunk : Fx.Thunk<WaitOrTimerCallback>
		{
			public WaitOrTimerThunk(WaitOrTimerCallback callback)
				: base(callback)
			{
			}

			public WaitOrTimerCallback ThunkFrame
			{
				get
				{
					return new WaitOrTimerCallback(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(object state, bool timedOut)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(state, timedOut);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		private sealed class SendOrPostThunk : Fx.Thunk<SendOrPostCallback>
		{
			public SendOrPostThunk(SendOrPostCallback callback)
				: base(callback)
			{
			}

			public SendOrPostCallback ThunkFrame
			{
				get
				{
					return new SendOrPostCallback(this.UnhandledExceptionFrame);
				}
			}

			[SecuritySafeCritical]
			private void UnhandledExceptionFrame(object state)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					base.Callback(state);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}
		}

		[SecurityCritical]
		private sealed class IOCompletionThunk
		{
			public IOCompletionThunk(IOCompletionCallback callback)
			{
				this.callback = callback;
			}

			public IOCompletionCallback ThunkFrame
			{
				get
				{
					return new IOCompletionCallback(this.UnhandledExceptionFrame);
				}
			}

			private unsafe void UnhandledExceptionFrame(uint error, uint bytesRead, NativeOverlapped* nativeOverlapped)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					this.callback(error, bytesRead, nativeOverlapped);
				}
				catch (Exception ex)
				{
					if (!Fx.HandleAtThreadBase(ex))
					{
						throw;
					}
				}
			}

			private IOCompletionCallback callback;
		}

		[Serializable]
		private class InternalException : SystemException
		{
			public InternalException(string description)
				: base(InternalSR.ShipAssertExceptionMessage(description))
			{
			}

			protected InternalException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}

		[Serializable]
		private class FatalInternalException : Fx.InternalException
		{
			public FatalInternalException(string description)
				: base(description)
			{
			}

			protected FatalInternalException(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}
		}
	}
}
