using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[Serializable]
	public struct SteamInputActionEvent_t
	{
		public InputHandle_t controllerHandle;

		public ESteamInputActionEventType eEventType;

		public SteamInputActionEvent_t.OptionValue m_val;

		[Serializable]
		public struct AnalogAction_t
		{
			public InputAnalogActionHandle_t actionHandle;

			public InputAnalogActionData_t analogActionData;
		}

		[Serializable]
		public struct DigitalAction_t
		{
			public InputDigitalActionHandle_t actionHandle;

			public InputDigitalActionData_t digitalActionData;
		}

		[Serializable]
		[StructLayout(LayoutKind.Explicit)]
		public struct OptionValue
		{
			[FieldOffset(0)]
			public SteamInputActionEvent_t.AnalogAction_t analogAction;

			[FieldOffset(0)]
			public SteamInputActionEvent_t.DigitalAction_t digitalAction;
		}
	}
}
