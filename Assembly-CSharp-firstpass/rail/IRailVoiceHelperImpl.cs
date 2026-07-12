using System;

namespace rail
{
	public class IRailVoiceHelperImpl : RailObject, IRailVoiceHelper
	{
		internal IRailVoiceHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailVoiceHelperImpl()
		{
		}

		public virtual IRailVoiceChannel AsyncCreateVoiceChannel(CreateVoiceChannelOption options, string channel_name, string user_data, out RailResult result)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateVoiceChannelOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			IRailVoiceChannel railVoiceChannel;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailVoiceHelper_AsyncCreateVoiceChannel(this.swigCPtr_, intPtr, channel_name, user_data, out result);
				railVoiceChannel = ((intPtr2 == IntPtr.Zero) ? null : new IRailVoiceChannelImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateVoiceChannelOption(intPtr);
			}
			return railVoiceChannel;
		}

		public virtual IRailVoiceChannel OpenVoiceChannel(RailVoiceChannelID voice_channel_id, string channel_name, out RailResult result)
		{
			IntPtr intPtr = ((voice_channel_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceChannelID__SWIG_0());
			if (voice_channel_id != null)
			{
				RailConverter.Csharp2Cpp(voice_channel_id, intPtr);
			}
			IRailVoiceChannel railVoiceChannel;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailVoiceHelper_OpenVoiceChannel(this.swigCPtr_, intPtr, channel_name, out result);
				railVoiceChannel = ((intPtr2 == IntPtr.Zero) ? null : new IRailVoiceChannelImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceChannelID(intPtr);
			}
			return railVoiceChannel;
		}

		public virtual EnumRailVoiceChannelSpeakerState GetSpeakerState()
		{
			return (EnumRailVoiceChannelSpeakerState)RAIL_API_PINVOKE.IRailVoiceHelper_GetSpeakerState(this.swigCPtr_);
		}

		public virtual RailResult MuteSpeaker()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_MuteSpeaker(this.swigCPtr_);
		}

		public virtual RailResult ResumeSpeaker()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_ResumeSpeaker(this.swigCPtr_);
		}

		public virtual RailResult SetupVoiceCapture(RailVoiceCaptureOption options, RailCaptureVoiceCallback callback)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetupVoiceCapture__SWIG_0(this.swigCPtr_, intPtr, callback);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceCaptureOption(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetupVoiceCapture(RailVoiceCaptureOption options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureOption__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetupVoiceCapture__SWIG_1(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailVoiceCaptureOption(intPtr);
			}
			return railResult;
		}

		public virtual RailResult StartVoiceCapturing(uint duration_milliseconds, bool repeat)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_0(this.swigCPtr_, duration_milliseconds, repeat);
		}

		public virtual RailResult StartVoiceCapturing(uint duration_milliseconds)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_1(this.swigCPtr_, duration_milliseconds);
		}

		public virtual RailResult StartVoiceCapturing()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StartVoiceCapturing__SWIG_2(this.swigCPtr_);
		}

		public virtual RailResult StopVoiceCapturing()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_StopVoiceCapturing(this.swigCPtr_);
		}

		public virtual RailResult GetCapturedVoiceData(byte[] buffer, uint buffer_length, out uint encoded_bytes_written)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_GetCapturedVoiceData(this.swigCPtr_, buffer, buffer_length, out encoded_bytes_written);
		}

		public virtual RailResult DecodeVoice(byte[] encoded_buffer, uint encoded_length, byte[] pcm_buffer, uint pcm_buffer_length, out uint pcm_buffer_written)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_DecodeVoice(this.swigCPtr_, encoded_buffer, encoded_length, pcm_buffer, pcm_buffer_length, out pcm_buffer_written);
		}

		public virtual RailResult GetVoiceCaptureSpecification(RailVoiceCaptureSpecification spec)
		{
			IntPtr intPtr = ((spec == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailVoiceCaptureSpecification__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_GetVoiceCaptureSpecification(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (spec != null)
				{
					RailConverter.Cpp2Csharp(intPtr, spec);
				}
				RAIL_API_PINVOKE.delete_RailVoiceCaptureSpecification(intPtr);
			}
			return railResult;
		}

		public virtual RailResult EnableInGameVoiceSpeaking(bool can_speaking)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_EnableInGameVoiceSpeaking(this.swigCPtr_, can_speaking);
		}

		public virtual RailResult SetPlayerNicknameInVoiceChannel(string nickname)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetPlayerNicknameInVoiceChannel(this.swigCPtr_, nickname);
		}

		public virtual RailResult SetPushToTalkKeyInVoiceChannel(uint push_to_talk_hot_key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetPushToTalkKeyInVoiceChannel(this.swigCPtr_, push_to_talk_hot_key);
		}

		public virtual uint GetPushToTalkKeyInVoiceChannel()
		{
			return RAIL_API_PINVOKE.IRailVoiceHelper_GetPushToTalkKeyInVoiceChannel(this.swigCPtr_);
		}

		public virtual RailResult ShowOverlayUI(bool show)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_ShowOverlayUI(this.swigCPtr_, show);
		}

		public virtual RailResult SetMicroVolume(uint volume)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetMicroVolume(this.swigCPtr_, volume);
		}

		public virtual RailResult SetSpeakerVolume(uint volume)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailVoiceHelper_SetSpeakerVolume(this.swigCPtr_, volume);
		}
	}
}
