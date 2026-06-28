using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public class ChannelGroup : ChannelControl
	{
		public ChannelGroup(IntPtr raw)
			: base(raw)
		{
		}

		public RESULT release()
		{
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_Release(base.getRaw());
			if (result == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return result;
		}

		public RESULT addGroup(ChannelGroup group, bool propagatedspclock, out DSPConnection connection)
		{
			connection = null;
			IntPtr intPtr;
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_AddGroup(base.getRaw(), group.getRaw(), propagatedspclock, out intPtr);
			connection = new DSPConnection(intPtr);
			return result;
		}

		public RESULT getNumGroups(out int numgroups)
		{
			return ChannelGroup.FMOD5_ChannelGroup_GetNumGroups(base.getRaw(), out numgroups);
		}

		public RESULT getGroup(int index, out ChannelGroup group)
		{
			group = null;
			IntPtr intPtr;
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_GetGroup(base.getRaw(), index, out intPtr);
			group = new ChannelGroup(intPtr);
			return result;
		}

		public RESULT getParentGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr intPtr;
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_GetParentGroup(base.getRaw(), out intPtr);
			group = new ChannelGroup(intPtr);
			return result;
		}

		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_GetName(base.getRaw(), intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumChannels(out int numchannels)
		{
			return ChannelGroup.FMOD5_ChannelGroup_GetNumChannels(base.getRaw(), out numchannels);
		}

		public RESULT getChannel(int index, out Channel channel)
		{
			channel = null;
			IntPtr intPtr;
			RESULT result = ChannelGroup.FMOD5_ChannelGroup_GetChannel(base.getRaw(), index, out intPtr);
			channel = new Channel(intPtr);
			return result;
		}

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_Release(IntPtr channelgroup);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propogatedspclocks, out IntPtr connection);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumGroups(IntPtr channelgroup, out int numgroups);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetParentGroup(IntPtr channelgroup, out IntPtr group);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetName(IntPtr channelgroup, IntPtr name, int namelen);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumChannels(IntPtr channelgroup, out int numchannels);

		[DllImport("fmodstudiol")]
		private static extern RESULT FMOD5_ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel);
	}
}
