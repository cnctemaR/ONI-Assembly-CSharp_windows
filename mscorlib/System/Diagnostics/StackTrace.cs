using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[MonoTODO("Serialized objects are not compatible with .NET")]
	[Serializable]
	public class StackTrace
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		public StackTrace()
		{
			this.init_frames(0, false);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public StackTrace(bool fNeedFileInfo)
		{
			this.init_frames(0, fNeedFileInfo);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public StackTrace(int skipFrames)
		{
			this.init_frames(skipFrames, false);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public StackTrace(int skipFrames, bool fNeedFileInfo)
		{
			this.init_frames(skipFrames, fNeedFileInfo);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void init_frames(int skipFrames, bool fNeedFileInfo)
		{
			if (skipFrames < 0)
			{
				throw new ArgumentOutOfRangeException("< 0", "skipFrames");
			}
			List<StackFrame> list = new List<StackFrame>();
			skipFrames += 2;
			StackFrame stackFrame;
			while ((stackFrame = new StackFrame(skipFrames, fNeedFileInfo)) != null && stackFrame.GetMethod() != null)
			{
				list.Add(stackFrame);
				skipFrames++;
			}
			this.debug_info = fNeedFileInfo;
			this.frames = list.ToArray();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern StackFrame[] get_trace(Exception e, int skipFrames, bool fNeedFileInfo);

		public StackTrace(Exception e)
			: this(e, 0, false)
		{
		}

		public StackTrace(Exception e, bool fNeedFileInfo)
			: this(e, 0, fNeedFileInfo)
		{
		}

		public StackTrace(Exception e, int skipFrames)
			: this(e, skipFrames, false)
		{
		}

		public StackTrace(Exception e, int skipFrames, bool fNeedFileInfo)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (skipFrames < 0)
			{
				throw new ArgumentOutOfRangeException("< 0", "skipFrames");
			}
			this.frames = StackTrace.get_trace(e, skipFrames, fNeedFileInfo);
			this.captured_traces = e.captured_traces;
		}

		public StackTrace(StackFrame frame)
		{
			this.frames = new StackFrame[1];
			this.frames[0] = frame;
		}

		[MonoLimitation("Not possible to create StackTraces from other threads")]
		[Obsolete]
		public StackTrace(Thread targetThread, bool needFileInfo)
		{
			if (targetThread == Thread.CurrentThread)
			{
				this.init_frames(0, needFileInfo);
				return;
			}
			throw new NotImplementedException();
		}

		internal StackTrace(StackFrame[] frames)
		{
			this.frames = frames;
		}

		public virtual int FrameCount
		{
			get
			{
				if (this.frames != null)
				{
					return this.frames.Length;
				}
				return 0;
			}
		}

		public virtual StackFrame GetFrame(int index)
		{
			if (index < 0 || index >= this.FrameCount)
			{
				return null;
			}
			return this.frames[index];
		}

		[ComVisible(false)]
		public virtual StackFrame[] GetFrames()
		{
			return this.frames;
		}

		private static string GetAotId()
		{
			if (!StackTrace.isAotidSet)
			{
				StackTrace.aotid = Assembly.GetAotId();
				if (StackTrace.aotid != null)
				{
					StackTrace.aotid = new Guid(StackTrace.aotid).ToString("N");
				}
				StackTrace.isAotidSet = true;
			}
			return StackTrace.aotid;
		}

		private bool AddFrames(StringBuilder sb)
		{
			string text = Locale.GetText("<unknown method>");
			string text2 = "  ";
			string text3 = Locale.GetText(" in {0}:{1} ");
			string text4 = string.Format("{0}{1}{2} ", Environment.NewLine, text2, Locale.GetText("at"));
			int i;
			for (i = 0; i < this.FrameCount; i++)
			{
				StackFrame frame = this.GetFrame(i);
				if (i == 0)
				{
					sb.AppendFormat("{0}{1} ", text2, Locale.GetText("at"));
				}
				else
				{
					sb.Append(text4);
				}
				if (frame.GetMethod() == null)
				{
					string internalMethodName = frame.GetInternalMethodName();
					if (internalMethodName != null)
					{
						sb.Append(internalMethodName);
					}
					else
					{
						sb.AppendFormat("<0x{0:x5} + 0x{1:x5}> {2}", frame.GetMethodAddress(), frame.GetNativeOffset(), text);
					}
				}
				else
				{
					this.GetFullNameForStackTrace(sb, frame.GetMethod());
					if (frame.GetILOffset() == -1)
					{
						sb.AppendFormat(" <0x{0:x5} + 0x{1:x5}>", frame.GetMethodAddress(), frame.GetNativeOffset());
						if (frame.GetMethodIndex() != 16777215U)
						{
							sb.AppendFormat(" {0}", frame.GetMethodIndex());
						}
					}
					else
					{
						sb.AppendFormat(" [0x{0:x5}]", frame.GetILOffset());
					}
					string text5 = frame.GetSecureFileName();
					if (text5[0] == '<')
					{
						string text6 = frame.GetMethod().Module.ModuleVersionId.ToString("N");
						string aotId = StackTrace.GetAotId();
						if (frame.GetILOffset() != -1 || aotId == null)
						{
							text5 = string.Format("<{0}>", text6);
						}
						else
						{
							text5 = string.Format("<{0}#{1}>", text6, aotId);
						}
					}
					sb.AppendFormat(text3, text5, frame.GetFileLineNumber());
				}
			}
			return i != 0;
		}

		internal void GetFullNameForStackTrace(StringBuilder sb, MethodBase mi)
		{
			Type type = mi.DeclaringType;
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				type = type.GetGenericTypeDefinition();
			}
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (methodInfo.MetadataToken == mi.MetadataToken)
				{
					mi = methodInfo;
					break;
				}
			}
			sb.Append(type.ToString());
			sb.Append(".");
			sb.Append(mi.Name);
			if (mi.IsGenericMethod)
			{
				Type[] genericArguments = mi.GetGenericArguments();
				sb.Append("[");
				for (int j = 0; j < genericArguments.Length; j++)
				{
					if (j > 0)
					{
						sb.Append(",");
					}
					sb.Append(genericArguments[j].Name);
				}
				sb.Append("]");
			}
			ParameterInfo[] parameters = mi.GetParameters();
			sb.Append(" (");
			for (int k = 0; k < parameters.Length; k++)
			{
				if (k > 0)
				{
					sb.Append(", ");
				}
				Type type2 = parameters[k].ParameterType;
				if (type2.IsGenericType && !type2.IsGenericTypeDefinition)
				{
					type2 = type2.GetGenericTypeDefinition();
				}
				sb.Append(type2.ToString());
				if (parameters[k].Name != null)
				{
					sb.Append(" ");
					sb.Append(parameters[k].Name);
				}
			}
			sb.Append(")");
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.captured_traces != null)
			{
				StackTrace[] array = this.captured_traces;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].AddFrames(stringBuilder))
					{
						stringBuilder.Append(Environment.NewLine);
						stringBuilder.Append("--- End of stack trace from previous location where exception was thrown ---");
						stringBuilder.Append(Environment.NewLine);
					}
				}
			}
			this.AddFrames(stringBuilder);
			return stringBuilder.ToString();
		}

		internal string ToString(StackTrace.TraceFormat traceFormat)
		{
			return this.ToString();
		}

		public const int METHODS_TO_SKIP = 0;

		private StackFrame[] frames;

		private readonly StackTrace[] captured_traces;

		private bool debug_info;

		private static bool isAotidSet;

		private static string aotid;

		internal enum TraceFormat
		{
			Normal,
			TrailingNewLine,
			NoResourceLookup
		}
	}
}
