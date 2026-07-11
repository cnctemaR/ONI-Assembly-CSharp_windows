using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.Security;

namespace System.Runtime
{
	internal class ExceptionTrace
	{
		public ExceptionTrace(string eventSourceName, EtwDiagnosticTrace diagnosticTrace)
		{
			this.eventSourceName = eventSourceName;
			this.diagnosticTrace = diagnosticTrace;
		}

		public void AsInformation(Exception exception)
		{
			TraceCore.HandledException(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
		}

		public void AsWarning(Exception exception)
		{
			TraceCore.HandledExceptionWarning(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
		}

		public Exception AsError(Exception exception)
		{
			AggregateException ex = exception as AggregateException;
			if (ex != null)
			{
				return this.AsError<Exception>(ex);
			}
			TargetInvocationException ex2 = exception as TargetInvocationException;
			if (ex2 != null && ex2.InnerException != null)
			{
				return this.AsError(ex2.InnerException);
			}
			return this.TraceException<Exception>(exception);
		}

		public Exception AsError(Exception exception, string eventSource)
		{
			AggregateException ex = exception as AggregateException;
			if (ex != null)
			{
				return this.AsError<Exception>(ex, eventSource);
			}
			TargetInvocationException ex2 = exception as TargetInvocationException;
			if (ex2 != null && ex2.InnerException != null)
			{
				return this.AsError(ex2.InnerException, eventSource);
			}
			return this.TraceException<Exception>(exception, eventSource);
		}

		public Exception AsError(TargetInvocationException targetInvocationException, string eventSource)
		{
			if (Fx.IsFatal(targetInvocationException))
			{
				return targetInvocationException;
			}
			Exception innerException = targetInvocationException.InnerException;
			if (innerException != null)
			{
				return this.AsError(innerException, eventSource);
			}
			return this.TraceException<Exception>(targetInvocationException, eventSource);
		}

		public Exception AsError<TPreferredException>(AggregateException aggregateException)
		{
			return this.AsError<TPreferredException>(aggregateException, this.eventSourceName);
		}

		public Exception AsError<TPreferredException>(AggregateException aggregateException, string eventSource)
		{
			if (Fx.IsFatal(aggregateException))
			{
				return aggregateException;
			}
			ReadOnlyCollection<Exception> innerExceptions = aggregateException.Flatten().InnerExceptions;
			if (innerExceptions.Count == 0)
			{
				return this.TraceException<AggregateException>(aggregateException, eventSource);
			}
			Exception ex = null;
			foreach (Exception ex2 in innerExceptions)
			{
				TargetInvocationException ex3 = ex2 as TargetInvocationException;
				Exception ex4 = ((ex3 != null && ex3.InnerException != null) ? ex3.InnerException : ex2);
				if (ex4 is TPreferredException && ex == null)
				{
					ex = ex4;
				}
				this.TraceException<Exception>(ex4, eventSource);
			}
			if (ex == null)
			{
				ex = innerExceptions[0];
			}
			return ex;
		}

		public ArgumentException Argument(string paramName, string message)
		{
			return this.TraceException<ArgumentException>(new ArgumentException(message, paramName));
		}

		public ArgumentNullException ArgumentNull(string paramName)
		{
			return this.TraceException<ArgumentNullException>(new ArgumentNullException(paramName));
		}

		public ArgumentNullException ArgumentNull(string paramName, string message)
		{
			return this.TraceException<ArgumentNullException>(new ArgumentNullException(paramName, message));
		}

		public ArgumentException ArgumentNullOrEmpty(string paramName)
		{
			return this.Argument(paramName, InternalSR.ArgumentNullOrEmpty(paramName));
		}

		public ArgumentOutOfRangeException ArgumentOutOfRange(string paramName, object actualValue, string message)
		{
			return this.TraceException<ArgumentOutOfRangeException>(new ArgumentOutOfRangeException(paramName, actualValue, message));
		}

		public ObjectDisposedException ObjectDisposed(string message)
		{
			return this.TraceException<ObjectDisposedException>(new ObjectDisposedException(null, message));
		}

		public void TraceUnhandledException(Exception exception)
		{
			TraceCore.UnhandledException(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
		}

		public void TraceHandledException(Exception exception, TraceEventType traceEventType)
		{
			if (traceEventType != TraceEventType.Error)
			{
				if (traceEventType != TraceEventType.Warning)
				{
					if (traceEventType != TraceEventType.Verbose)
					{
						if (TraceCore.HandledExceptionIsEnabled(this.diagnosticTrace))
						{
							TraceCore.HandledException(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
						}
					}
					else if (TraceCore.HandledExceptionVerboseIsEnabled(this.diagnosticTrace))
					{
						TraceCore.HandledExceptionVerbose(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
						return;
					}
				}
				else if (TraceCore.HandledExceptionWarningIsEnabled(this.diagnosticTrace))
				{
					TraceCore.HandledExceptionWarning(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
					return;
				}
			}
			else if (TraceCore.HandledExceptionErrorIsEnabled(this.diagnosticTrace))
			{
				TraceCore.HandledExceptionError(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
				return;
			}
		}

		public void TraceEtwException(Exception exception, TraceEventType eventType)
		{
			switch (eventType)
			{
			case TraceEventType.Critical:
				if (TraceCore.EtwUnhandledExceptionIsEnabled(this.diagnosticTrace))
				{
					TraceCore.EtwUnhandledException(this.diagnosticTrace, (exception != null) ? exception.ToString() : string.Empty, exception);
					return;
				}
				return;
			case TraceEventType.Error:
			case TraceEventType.Warning:
				if (TraceCore.ThrowingEtwExceptionIsEnabled(this.diagnosticTrace))
				{
					TraceCore.ThrowingEtwException(this.diagnosticTrace, this.eventSourceName, (exception != null) ? exception.ToString() : string.Empty, exception);
					return;
				}
				return;
			}
			if (TraceCore.ThrowingEtwExceptionVerboseIsEnabled(this.diagnosticTrace))
			{
				TraceCore.ThrowingEtwExceptionVerbose(this.diagnosticTrace, this.eventSourceName, (exception != null) ? exception.ToString() : string.Empty, exception);
			}
		}

		private TException TraceException<TException>(TException exception) where TException : Exception
		{
			return this.TraceException<TException>(exception, this.eventSourceName);
		}

		[SecuritySafeCritical]
		private TException TraceException<TException>(TException exception, string eventSource) where TException : Exception
		{
			if (TraceCore.ThrowingExceptionIsEnabled(this.diagnosticTrace))
			{
				TraceCore.ThrowingException(this.diagnosticTrace, eventSource, (exception != null) ? exception.ToString() : string.Empty, exception);
			}
			this.BreakOnException(exception);
			return exception;
		}

		[SecuritySafeCritical]
		private void BreakOnException(Exception exception)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void TraceFailFast(string message)
		{
			EventLogger eventLogger = new EventLogger(this.eventSourceName, this.diagnosticTrace);
			this.TraceFailFast(message, eventLogger);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void TraceFailFast(string message, EventLogger logger)
		{
			if (logger != null)
			{
				try
				{
					string text = null;
					try
					{
						text = new StackTrace().ToString();
					}
					catch (Exception ex)
					{
						text = ex.Message;
						if (Fx.IsFatal(ex))
						{
							throw;
						}
					}
					finally
					{
						logger.LogEvent(TraceEventType.Critical, 6, 3221291110U, new string[] { message, text });
					}
				}
				catch (Exception ex2)
				{
					logger.LogEvent(TraceEventType.Critical, 6, 3221291111U, new string[] { ex2.ToString() });
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
				}
			}
		}

		private const ushort FailFastEventLogCategory = 6;

		private string eventSourceName;

		private readonly EtwDiagnosticTrace diagnosticTrace;
	}
}
