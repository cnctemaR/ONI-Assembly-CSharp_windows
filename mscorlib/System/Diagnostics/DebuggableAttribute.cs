using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
	[ComVisible(true)]
	public sealed class DebuggableAttribute : Attribute
	{
		public DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
		{
			this.JITTrackingEnabledFlag = isJITTrackingEnabled;
			this.JITOptimizerDisabledFlag = isJITOptimizerDisabled;
			if (isJITTrackingEnabled)
			{
				this.debuggingModes |= DebuggableAttribute.DebuggingModes.Default;
			}
			if (isJITOptimizerDisabled)
			{
				this.debuggingModes |= DebuggableAttribute.DebuggingModes.DisableOptimizations;
			}
		}

		public DebuggableAttribute(DebuggableAttribute.DebuggingModes modes)
		{
			this.debuggingModes = modes;
			this.JITTrackingEnabledFlag = (this.debuggingModes & DebuggableAttribute.DebuggingModes.Default) != DebuggableAttribute.DebuggingModes.None;
			this.JITOptimizerDisabledFlag = (this.debuggingModes & DebuggableAttribute.DebuggingModes.DisableOptimizations) != DebuggableAttribute.DebuggingModes.None;
		}

		public DebuggableAttribute.DebuggingModes DebuggingFlags
		{
			get
			{
				return this.debuggingModes;
			}
		}

		public bool IsJITTrackingEnabled
		{
			get
			{
				return this.JITTrackingEnabledFlag;
			}
		}

		public bool IsJITOptimizerDisabled
		{
			get
			{
				return this.JITOptimizerDisabledFlag;
			}
		}

		private bool JITTrackingEnabledFlag;

		private bool JITOptimizerDisabledFlag;

		private DebuggableAttribute.DebuggingModes debuggingModes;

		[Flags]
		[ComVisible(true)]
		public enum DebuggingModes
		{
			None = 0,
			Default = 1,
			IgnoreSymbolStoreSequencePoints = 2,
			EnableEditAndContinue = 4,
			DisableOptimizations = 256
		}
	}
}
