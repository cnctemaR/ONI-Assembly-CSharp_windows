using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	[MonoTODO("Serialized objects are not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public class StackTrace
	{
		public StackTrace()
		{
			this.init_frames(0, false);
		}

		public StackTrace(bool fNeedFileInfo)
		{
			this.init_frames(0, fNeedFileInfo);
		}

		public StackTrace(int skipFrames)
		{
			this.init_frames(skipFrames, false);
		}

		public StackTrace(int skipFrames, bool fNeedFileInfo)
		{
			this.init_frames(skipFrames, fNeedFileInfo);
		}

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
			: this(e, skipFrames, fNeedFileInfo, false)
		{
		}

		internal StackTrace(Exception e, int skipFrames, bool fNeedFileInfo, bool returnNativeFrames)
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
			if (!returnNativeFrames)
			{
				bool flag = false;
				for (int i = 0; i < this.frames.Length; i++)
				{
					if (this.frames[i].GetMethod() == null)
					{
						flag = true;
					}
				}
				if (flag)
				{
					ArrayList arrayList = new ArrayList();
					for (int j = 0; j < this.frames.Length; j++)
					{
						if (this.frames[j].GetMethod() != null)
						{
							arrayList.Add(this.frames[j]);
						}
					}
					this.frames = (StackFrame[])arrayList.ToArray(typeof(StackFrame));
				}
			}
		}

		public StackTrace(StackFrame frame)
		{
			this.frames = new StackFrame[1];
			this.frames[0] = frame;
		}

		[MonoTODO("Not possible to create StackTraces from other threads")]
		public StackTrace(Thread targetThread, bool needFileInfo)
		{
			throw new NotImplementedException();
		}

		private void init_frames(int skipFrames, bool fNeedFileInfo)
		{
			if (skipFrames < 0)
			{
				throw new ArgumentOutOfRangeException("< 0", "skipFrames");
			}
			ArrayList arrayList = new ArrayList();
			skipFrames += 2;
			StackFrame stackFrame;
			while ((stackFrame = new StackFrame(skipFrames, fNeedFileInfo)) != null && stackFrame.GetMethod() != null)
			{
				arrayList.Add(stackFrame);
				skipFrames++;
			}
			this.debug_info = fNeedFileInfo;
			this.frames = (StackFrame[])arrayList.ToArray(typeof(StackFrame));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern StackFrame[] get_trace(Exception e, int skipFrames, bool fNeedFileInfo);

		public virtual int FrameCount
		{
			get
			{
				return (this.frames != null) ? this.frames.Length : 0;
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

		public override string ToString()
		{
			string text = string.Format("{0}   {1} ", Environment.NewLine, Locale.GetText("at"));
			string text2 = Locale.GetText("<unknown method>");
			string text3 = Locale.GetText(" in {0}:line {1}");
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.FrameCount; i++)
			{
				StackFrame frame = this.GetFrame(i);
				if (i > 0)
				{
					stringBuilder.Append(text);
				}
				else
				{
					stringBuilder.AppendFormat("   {0} ", Locale.GetText("at"));
				}
				MethodBase method = frame.GetMethod();
				if (method != null)
				{
					stringBuilder.AppendFormat("{0}.{1}", method.DeclaringType.FullName, method.Name);
					stringBuilder.Append("(");
					ParameterInfo[] parameters = method.GetParameters();
					for (int j = 0; j < parameters.Length; j++)
					{
						if (j > 0)
						{
							stringBuilder.Append(", ");
						}
						Type type = parameters[j].ParameterType;
						bool isByRef = type.IsByRef;
						if (isByRef)
						{
							type = type.GetElementType();
						}
						if (type.IsClass && type.Namespace != string.Empty)
						{
							stringBuilder.Append(type.Namespace);
							stringBuilder.Append(".");
						}
						stringBuilder.Append(type.Name);
						if (isByRef)
						{
							stringBuilder.Append(" ByRef");
						}
						stringBuilder.AppendFormat(" {0}", parameters[j].Name);
					}
					stringBuilder.Append(")");
				}
				else
				{
					stringBuilder.Append(text2);
				}
				if (this.debug_info)
				{
					string secureFileName = frame.GetSecureFileName();
					if (secureFileName != "<filename unknown>")
					{
						stringBuilder.AppendFormat(text3, secureFileName, frame.GetFileLineNumber());
					}
				}
			}
			return stringBuilder.ToString();
		}

		public const int METHODS_TO_SKIP = 0;

		private StackFrame[] frames;

		private bool debug_info;
	}
}
