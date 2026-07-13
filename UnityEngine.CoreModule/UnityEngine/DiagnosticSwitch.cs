using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeClass("DiagnosticSwitch", "struct DiagnosticSwitch;")]
	[NativeHeader("Runtime/Utilities/DiagnosticSwitch.h")]
	[NativeAsStruct]
	[StructLayout(LayoutKind.Sequential)]
	internal class DiagnosticSwitch
	{
		private DiagnosticSwitch()
		{
		}

		public extern string name
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string description
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("OwningModuleName")]
		public extern string owningModule
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern DiagnosticSwitch.Flags flags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public object value
		{
			get
			{
				return this.GetScriptingValue();
			}
			set
			{
				this.SetScriptingValue(value, false);
			}
		}

		[NativeName("ScriptingDefaultValue")]
		public extern object defaultValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("ScriptingMinValue")]
		public extern object minValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeName("ScriptingMaxValue")]
		public extern object maxValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public object persistentValue
		{
			get
			{
				return this.GetScriptingPersistentValue();
			}
			set
			{
				this.SetScriptingValue(value, true);
			}
		}

		[NativeName("ScriptingEnumInfo")]
		public extern EnumInfo enumInfo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetScriptingValue();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetScriptingPersistentValue();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetScriptingValue(object value, bool setPersistent);

		public bool isSetToDefault
		{
			get
			{
				return object.Equals(this.persistentValue, this.defaultValue);
			}
		}

		public bool needsRestart
		{
			get
			{
				return !object.Equals(this.value, this.persistentValue);
			}
		}

		private IntPtr m_Ptr;

		[Flags]
		internal enum Flags
		{
			None = 0,
			CanChangeAfterEngineStart = 1,
			PropagateToAssetImportWorkerProcess = 2
		}
	}
}
