using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using Unity;

namespace System.ComponentModel.Composition
{
	[DebuggerDisplay("{Message}")]
	[DebuggerTypeProxy(typeof(CompositionExceptionDebuggerProxy))]
	[Serializable]
	public class CompositionException : Exception
	{
		public CompositionException()
			: this(null, null, null)
		{
		}

		public CompositionException(string message)
			: this(message, null, null)
		{
		}

		public CompositionException(string message, Exception innerException)
			: this(message, innerException, null)
		{
		}

		internal CompositionException(CompositionError error)
			: this(new CompositionError[] { error })
		{
		}

		public CompositionException(IEnumerable<CompositionError> errors)
			: this(null, null, errors)
		{
		}

		internal CompositionException(string message, Exception innerException, IEnumerable<CompositionError> errors)
			: base(message, innerException)
		{
			Requires.NullOrNotNullElements<CompositionError>(errors, "errors");
			base.SerializeObjectState += delegate(object exception, SafeSerializationEventArgs eventArgs)
			{
				CompositionException.CompositionExceptionData compositionExceptionData = default(CompositionException.CompositionExceptionData);
				if (this._errors != null)
				{
					compositionExceptionData._errors = this._errors.Select<CompositionError, CompositionError>((CompositionError error) => new CompositionError(error.Id, error.Description, error.Element.ToSerializableElement(), error.Exception)).ToArray<CompositionError>();
				}
				else
				{
					compositionExceptionData._errors = new CompositionError[0];
				}
				eventArgs.AddSerializedState(compositionExceptionData);
			};
			this._errors = new ReadOnlyCollection<CompositionError>((errors == null) ? new CompositionError[0] : errors.ToArray<CompositionError>());
		}

		public ReadOnlyCollection<CompositionError> Errors
		{
			get
			{
				return this._errors;
			}
		}

		public override string Message
		{
			get
			{
				if (this.Errors.Count == 0)
				{
					return base.Message;
				}
				return this.BuildDefaultMessage();
			}
		}

		private string BuildDefaultMessage()
		{
			IEnumerable<IEnumerable<CompositionError>> enumerable = CompositionException.CalculatePaths(this);
			StringBuilder stringBuilder = new StringBuilder();
			CompositionException.WriteHeader(stringBuilder, this.Errors.Count, enumerable.Count<IEnumerable<CompositionError>>());
			CompositionException.WritePaths(stringBuilder, enumerable);
			return stringBuilder.ToString();
		}

		private static void WriteHeader(StringBuilder writer, int errorsCount, int pathCount)
		{
			if (errorsCount > 1 && pathCount > 1)
			{
				writer.AppendFormat(CultureInfo.CurrentCulture, Strings.CompositionException_MultipleErrorsWithMultiplePaths, pathCount);
			}
			else if (errorsCount == 1 && pathCount > 1)
			{
				writer.AppendFormat(CultureInfo.CurrentCulture, Strings.CompositionException_SingleErrorWithMultiplePaths, pathCount);
			}
			else
			{
				Assumes.IsTrue(errorsCount == 1);
				Assumes.IsTrue(pathCount == 1);
				writer.AppendFormat(CultureInfo.CurrentCulture, Strings.CompositionException_SingleErrorWithSinglePath, pathCount);
			}
			writer.Append(' ');
			writer.AppendLine(Strings.CompositionException_ReviewErrorProperty);
		}

		private static void WritePaths(StringBuilder writer, IEnumerable<IEnumerable<CompositionError>> paths)
		{
			int num = 0;
			foreach (IEnumerable<CompositionError> enumerable in paths)
			{
				num++;
				CompositionException.WritePath(writer, enumerable, num);
			}
		}

		private static void WritePath(StringBuilder writer, IEnumerable<CompositionError> path, int ordinal)
		{
			writer.AppendLine();
			writer.Append(ordinal.ToString(CultureInfo.CurrentCulture));
			writer.Append(Strings.CompositionException_PathsCountSeparator);
			writer.Append(' ');
			CompositionException.WriteError(writer, path.First<CompositionError>());
			foreach (CompositionError compositionError in path.Skip<CompositionError>(1))
			{
				writer.AppendLine();
				writer.Append(Strings.CompositionException_ErrorPrefix);
				writer.Append(' ');
				CompositionException.WriteError(writer, compositionError);
			}
		}

		private static void WriteError(StringBuilder writer, CompositionError error)
		{
			writer.AppendLine(error.Description);
			if (error.Element != null)
			{
				CompositionException.WriteElementGraph(writer, error.Element);
			}
		}

		private static void WriteElementGraph(StringBuilder writer, ICompositionElement element)
		{
			writer.AppendFormat(CultureInfo.CurrentCulture, Strings.CompositionException_ElementPrefix, element.DisplayName);
			while ((element = element.Origin) != null)
			{
				writer.AppendFormat(CultureInfo.CurrentCulture, Strings.CompositionException_OriginFormat, Strings.CompositionException_OriginSeparator, element.DisplayName);
			}
			writer.AppendLine();
		}

		private static IEnumerable<IEnumerable<CompositionError>> CalculatePaths(CompositionException exception)
		{
			List<IEnumerable<CompositionError>> paths = new List<IEnumerable<CompositionError>>();
			CompositionException.VisitCompositionException(exception, new CompositionException.VisitContext
			{
				Path = new Stack<CompositionError>(),
				LeafVisitor = delegate(Stack<CompositionError> path)
				{
					paths.Add(path.Copy<CompositionError>());
				}
			});
			return paths;
		}

		private static void VisitCompositionException(CompositionException exception, CompositionException.VisitContext context)
		{
			foreach (CompositionError compositionError in exception.Errors)
			{
				CompositionException.VisitError(compositionError, context);
			}
			if (exception.InnerException != null)
			{
				CompositionException.VisitException(exception.InnerException, context);
			}
		}

		private static void VisitError(CompositionError error, CompositionException.VisitContext context)
		{
			context.Path.Push(error);
			if (error.Exception == null)
			{
				context.LeafVisitor(context.Path);
			}
			else
			{
				CompositionException.VisitException(error.Exception, context);
			}
			context.Path.Pop();
		}

		private static void VisitException(Exception exception, CompositionException.VisitContext context)
		{
			CompositionException ex = exception as CompositionException;
			if (ex != null)
			{
				CompositionException.VisitCompositionException(ex, context);
				return;
			}
			CompositionException.VisitError(new CompositionError(exception.Message, exception.InnerException), context);
		}

		public ReadOnlyCollection<Exception> RootCauses
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		private const string ErrorsKey = "Errors";

		private ReadOnlyCollection<CompositionError> _errors;

		[Serializable]
		private struct CompositionExceptionData : ISafeSerializationData
		{
			void ISafeSerializationData.CompleteDeserialization(object obj)
			{
				(obj as CompositionException)._errors = new ReadOnlyCollection<CompositionError>(this._errors);
			}

			public CompositionError[] _errors;
		}

		private struct VisitContext
		{
			public Stack<CompositionError> Path;

			public Action<Stack<CompositionError>> LeafVisitor;
		}
	}
}
