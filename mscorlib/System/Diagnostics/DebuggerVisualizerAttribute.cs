using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerVisualizerAttribute : Attribute
	{
		public DebuggerVisualizerAttribute(string visualizerTypeName)
		{
			this.visualizerName = visualizerTypeName;
		}

		public DebuggerVisualizerAttribute(Type visualizer)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
		}

		public DebuggerVisualizerAttribute(string visualizerTypeName, string visualizerObjectSourceTypeName)
		{
			this.visualizerName = visualizerTypeName;
			this.visualizerSourceName = visualizerObjectSourceTypeName;
		}

		public DebuggerVisualizerAttribute(string visualizerTypeName, Type visualizerObjectSource)
		{
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			this.visualizerName = visualizerTypeName;
			this.visualizerSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}

		public DebuggerVisualizerAttribute(Type visualizer, string visualizerObjectSourceTypeName)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
			this.visualizerSourceName = visualizerObjectSourceTypeName;
		}

		public DebuggerVisualizerAttribute(Type visualizer, Type visualizerObjectSource)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			this.visualizerName = visualizer.AssemblyQualifiedName;
			this.visualizerSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public Type Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
				this.targetTypeName = this.target.AssemblyQualifiedName;
			}
		}

		public string TargetTypeName
		{
			get
			{
				return this.targetTypeName;
			}
			set
			{
				this.targetTypeName = value;
			}
		}

		public string VisualizerObjectSourceTypeName
		{
			get
			{
				return this.visualizerSourceName;
			}
		}

		public string VisualizerTypeName
		{
			get
			{
				return this.visualizerName;
			}
		}

		private string description;

		private string visualizerSourceName;

		private string visualizerName;

		private string targetTypeName;

		private Type target;
	}
}
