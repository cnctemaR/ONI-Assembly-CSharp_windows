using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Utilities/DiagnosticSwitch.h")]
	[NativeAsStruct]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.TextCoreTextEngineModule", "UnityEngine.IMGUIModule" })]
	[NativeClass("DiagnosticSwitch", "struct DiagnosticSwitch;")]
	[StructLayout(LayoutKind.Sequential)]
	internal class DiagnosticSwitch
	{
		private DiagnosticSwitch(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		public string name
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					DiagnosticSwitch.get_name_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public string description
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					DiagnosticSwitch.get_description_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		[NativeName("OwningModuleName")]
		public string owningModule
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					DiagnosticSwitch.get_owningModule_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public DiagnosticSwitch.Flags flags
		{
			get
			{
				IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DiagnosticSwitch.get_flags_Injected(intPtr);
			}
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
		public object defaultValue
		{
			get
			{
				IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DiagnosticSwitch.get_defaultValue_Injected(intPtr);
			}
		}

		[NativeName("ScriptingMinValue")]
		public object minValue
		{
			get
			{
				IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DiagnosticSwitch.get_minValue_Injected(intPtr);
			}
		}

		[NativeName("ScriptingMaxValue")]
		public object maxValue
		{
			get
			{
				IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DiagnosticSwitch.get_maxValue_Injected(intPtr);
			}
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
		public EnumInfo enumInfo
		{
			get
			{
				IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return DiagnosticSwitch.get_enumInfo_Injected(intPtr);
			}
		}

		private object GetScriptingValue()
		{
			IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return DiagnosticSwitch.GetScriptingValue_Injected(intPtr);
		}

		private object GetScriptingPersistentValue()
		{
			IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return DiagnosticSwitch.GetScriptingPersistentValue_Injected(intPtr);
		}

		[NativeThrows]
		private void SetScriptingValue(object value, bool setPersistent)
		{
			IntPtr intPtr = DiagnosticSwitch.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			DiagnosticSwitch.SetScriptingValue_Injected(intPtr, value, setPersistent);
		}

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

		public override string ToString()
		{
			string text = ((this.value == null) ? "<null>" : this.value.ToString());
			return this.name + " = " + text;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_name_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_description_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_owningModule_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern DiagnosticSwitch.Flags get_flags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object get_defaultValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object get_minValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object get_maxValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern EnumInfo get_enumInfo_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetScriptingValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetScriptingPersistentValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScriptingValue_Injected(IntPtr _unity_self, object value, bool setPersistent);

		private IntPtr m_Ptr;

		[Flags]
		internal enum Flags
		{
			None = 0,
			CanChangeAfterEngineStart = 1,
			PropagateToAssetImportWorkerProcess = 2
		}

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(DiagnosticSwitch diagnosticSwitch)
			{
				return diagnosticSwitch.m_Ptr;
			}

			public static DiagnosticSwitch ConvertToManaged(IntPtr ptr)
			{
				return new DiagnosticSwitch(ptr);
			}
		}
	}
}
