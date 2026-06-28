using System;

namespace FMOD
{
	public class Error
	{
		public static string String(RESULT errcode)
		{
			string text;
			switch (errcode)
			{
			case RESULT.OK:
				text = "No errors.";
				break;
			case RESULT.ERR_BADCOMMAND:
				text = "Tried to call a function on a data type that does not allow this type of functionality (ie calling Sound::lock on a streaming sound).";
				break;
			case RESULT.ERR_CHANNEL_ALLOC:
				text = "Error trying to allocate a channel.";
				break;
			case RESULT.ERR_CHANNEL_STOLEN:
				text = "The specified channel has been reused to play another sound.";
				break;
			case RESULT.ERR_DMA:
				text = "DMA Failure.  See debug output for more information.";
				break;
			case RESULT.ERR_DSP_CONNECTION:
				text = "DSP connection error.  Connection possibly caused a cyclic dependency or connected dsps with incompatible buffer counts.";
				break;
			case RESULT.ERR_DSP_DONTPROCESS:
				text = "DSP return code from a DSP process query callback.  Tells mixer not to call the process callback and therefore not consume CPU.  Use this to optimize the DSP graph.";
				break;
			case RESULT.ERR_DSP_FORMAT:
				text = "DSP Format error.  A DSP unit may have attempted to connect to this network with the wrong format, or a matrix may have been set with the wrong size if the target unit has a specified channel map.";
				break;
			case RESULT.ERR_DSP_INUSE:
				text = "DSP is already in the mixer's DSP network. It must be removed before being reinserted or released.";
				break;
			case RESULT.ERR_DSP_NOTFOUND:
				text = "DSP connection error.  Couldn't find the DSP unit specified.";
				break;
			case RESULT.ERR_DSP_RESERVED:
				text = "DSP operation error.  Cannot perform operation on this DSP as it is reserved by the system.";
				break;
			case RESULT.ERR_DSP_SILENCE:
				text = "DSP return code from a DSP process query callback.  Tells mixer silence would be produced from read, so go idle and not consume CPU.  Use this to optimize the DSP graph.";
				break;
			case RESULT.ERR_DSP_TYPE:
				text = "DSP operation cannot be performed on a DSP of this type.";
				break;
			case RESULT.ERR_FILE_BAD:
				text = "Error loading file.";
				break;
			case RESULT.ERR_FILE_COULDNOTSEEK:
				text = "Couldn't perform seek operation.  This is a limitation of the medium (ie netstreams) or the file format.";
				break;
			case RESULT.ERR_FILE_DISKEJECTED:
				text = "Media was ejected while reading.";
				break;
			case RESULT.ERR_FILE_EOF:
				text = "End of file unexpectedly reached while trying to read essential data (truncated?).";
				break;
			case RESULT.ERR_FILE_ENDOFDATA:
				text = "End of current chunk reached while trying to read data.";
				break;
			case RESULT.ERR_FILE_NOTFOUND:
				text = "File not found.";
				break;
			case RESULT.ERR_FORMAT:
				text = "Unsupported file or audio format.";
				break;
			case RESULT.ERR_HEADER_MISMATCH:
				text = "There is a version mismatch between the FMOD header and either the FMOD Studio library or the FMOD Low Level library.";
				break;
			case RESULT.ERR_HTTP:
				text = "A HTTP error occurred. This is a catch-all for HTTP errors not listed elsewhere.";
				break;
			case RESULT.ERR_HTTP_ACCESS:
				text = "The specified resource requires authentication or is forbidden.";
				break;
			case RESULT.ERR_HTTP_PROXY_AUTH:
				text = "Proxy authentication is required to access the specified resource.";
				break;
			case RESULT.ERR_HTTP_SERVER_ERROR:
				text = "A HTTP server error occurred.";
				break;
			case RESULT.ERR_HTTP_TIMEOUT:
				text = "The HTTP request timed out.";
				break;
			case RESULT.ERR_INITIALIZATION:
				text = "FMOD was not initialized correctly to support this function.";
				break;
			case RESULT.ERR_INITIALIZED:
				text = "Cannot call this command after System::init.";
				break;
			case RESULT.ERR_INTERNAL:
				text = "An error occurred that wasn't supposed to.  Contact support.";
				break;
			case RESULT.ERR_INVALID_FLOAT:
				text = "Value passed in was a NaN, Inf or denormalized float.";
				break;
			case RESULT.ERR_INVALID_HANDLE:
				text = "An invalid object handle was used.";
				break;
			case RESULT.ERR_INVALID_PARAM:
				text = "An invalid parameter was passed to this function.";
				break;
			case RESULT.ERR_INVALID_POSITION:
				text = "An invalid seek position was passed to this function.";
				break;
			case RESULT.ERR_INVALID_SPEAKER:
				text = "An invalid speaker was passed to this function based on the current speaker mode.";
				break;
			case RESULT.ERR_INVALID_SYNCPOINT:
				text = "The syncpoint did not come from this sound handle.";
				break;
			case RESULT.ERR_INVALID_THREAD:
				text = "Tried to call a function on a thread that is not supported.";
				break;
			case RESULT.ERR_INVALID_VECTOR:
				text = "The vectors passed in are not unit length, or perpendicular.";
				break;
			case RESULT.ERR_MAXAUDIBLE:
				text = "Reached maximum audible playback count for this sound's soundgroup.";
				break;
			case RESULT.ERR_MEMORY:
				text = "Not enough memory or resources.";
				break;
			case RESULT.ERR_MEMORY_CANTPOINT:
				text = "Can't use FMOD_OPENMEMORY_POINT on non PCM source data, or non mp3/xma/adpcm data if FMOD_CREATECOMPRESSEDSAMPLE was used.";
				break;
			case RESULT.ERR_NEEDS3D:
				text = "Tried to call a command on a 2d sound when the command was meant for 3d sound.";
				break;
			case RESULT.ERR_NEEDSHARDWARE:
				text = "Tried to use a feature that requires hardware support.";
				break;
			case RESULT.ERR_NET_CONNECT:
				text = "Couldn't connect to the specified host.";
				break;
			case RESULT.ERR_NET_SOCKET_ERROR:
				text = "A socket error occurred.  This is a catch-all for socket-related errors not listed elsewhere.";
				break;
			case RESULT.ERR_NET_URL:
				text = "The specified URL couldn't be resolved.";
				break;
			case RESULT.ERR_NET_WOULD_BLOCK:
				text = "Operation on a non-blocking socket could not complete immediately.";
				break;
			case RESULT.ERR_NOTREADY:
				text = "Operation could not be performed because specified sound/DSP connection is not ready.";
				break;
			case RESULT.ERR_OUTPUT_ALLOCATED:
				text = "Error initializing output device, but more specifically, the output device is already in use and cannot be reused.";
				break;
			case RESULT.ERR_OUTPUT_CREATEBUFFER:
				text = "Error creating hardware sound buffer.";
				break;
			case RESULT.ERR_OUTPUT_DRIVERCALL:
				text = "A call to a standard soundcard driver failed, which could possibly mean a bug in the driver or resources were missing or exhausted.";
				break;
			case RESULT.ERR_OUTPUT_FORMAT:
				text = "Soundcard does not support the specified format.";
				break;
			case RESULT.ERR_OUTPUT_INIT:
				text = "Error initializing output device.";
				break;
			case RESULT.ERR_OUTPUT_NODRIVERS:
				text = "The output device has no drivers installed.  If pre-init, FMOD_OUTPUT_NOSOUND is selected as the output mode.  If post-init, the function just fails.";
				break;
			case RESULT.ERR_PLUGIN:
				text = "An unspecified error has been returned from a plugin.";
				break;
			case RESULT.ERR_PLUGIN_MISSING:
				text = "A requested output, dsp unit type or codec was not available.";
				break;
			case RESULT.ERR_PLUGIN_RESOURCE:
				text = "A resource that the plugin requires cannot be found. (ie the DLS file for MIDI playback)";
				break;
			case RESULT.ERR_PLUGIN_VERSION:
				text = "A plugin was built with an unsupported SDK version.";
				break;
			case RESULT.ERR_RECORD:
				text = "An error occurred trying to initialize the recording device.";
				break;
			case RESULT.ERR_REVERB_CHANNELGROUP:
				text = "Reverb properties cannot be set on this channel because a parent channelgroup owns the reverb connection.";
				break;
			case RESULT.ERR_REVERB_INSTANCE:
				text = "Specified instance in FMOD_REVERB_PROPERTIES couldn't be set. Most likely because it is an invalid instance number or the reverb doesn't exist.";
				break;
			case RESULT.ERR_SUBSOUNDS:
				text = "The error occurred because the sound referenced contains subsounds when it shouldn't have, or it doesn't contain subsounds when it should have.  The operation may also not be able to be performed on a parent sound.";
				break;
			case RESULT.ERR_SUBSOUND_ALLOCATED:
				text = "This subsound is already being used by another sound, you cannot have more than one parent to a sound.  Null out the other parent's entry first.";
				break;
			case RESULT.ERR_SUBSOUND_CANTMOVE:
				text = "Shared subsounds cannot be replaced or moved from their parent stream, such as when the parent stream is an FSB file.";
				break;
			case RESULT.ERR_TAGNOTFOUND:
				text = "The specified tag could not be found or there are no tags.";
				break;
			case RESULT.ERR_TOOMANYCHANNELS:
				text = "The sound created exceeds the allowable input channel count.  This can be increased using the 'maxinputchannels' parameter in System::setSoftwareFormat.";
				break;
			case RESULT.ERR_TRUNCATED:
				text = "The retrieved string is too long to fit in the supplied buffer and has been truncated.";
				break;
			case RESULT.ERR_UNIMPLEMENTED:
				text = "Something in FMOD hasn't been implemented when it should be! contact support!";
				break;
			case RESULT.ERR_UNINITIALIZED:
				text = "This command failed because System::init or System::setDriver was not called.";
				break;
			case RESULT.ERR_UNSUPPORTED:
				text = "A command issued was not supported by this object.  Possibly a plugin without certain callbacks specified.";
				break;
			case RESULT.ERR_VERSION:
				text = "The version number of this file format is not supported.";
				break;
			case RESULT.ERR_EVENT_ALREADY_LOADED:
				text = "The specified bank has already been loaded.";
				break;
			case RESULT.ERR_EVENT_LIVEUPDATE_BUSY:
				text = "The live update connection failed due to the game already being connected.";
				break;
			case RESULT.ERR_EVENT_LIVEUPDATE_MISMATCH:
				text = "The live update connection failed due to the game data being out of sync with the tool.";
				break;
			case RESULT.ERR_EVENT_LIVEUPDATE_TIMEOUT:
				text = "The live update connection timed out.";
				break;
			case RESULT.ERR_EVENT_NOTFOUND:
				text = "The requested event, bus or vca could not be found.";
				break;
			case RESULT.ERR_STUDIO_UNINITIALIZED:
				text = "The Studio::System object is not yet initialized.";
				break;
			case RESULT.ERR_STUDIO_NOT_LOADED:
				text = "The specified resource is not loaded, so it can't be unloaded.";
				break;
			case RESULT.ERR_INVALID_STRING:
				text = "An invalid string was passed to this function.";
				break;
			case RESULT.ERR_ALREADY_LOCKED:
				text = "The specified resource is already locked.";
				break;
			case RESULT.ERR_NOT_LOCKED:
				text = "The specified resource is not locked, so it can't be unlocked.";
				break;
			case RESULT.ERR_RECORD_DISCONNECTED:
				text = "The specified recording driver has been disconnected.";
				break;
			case RESULT.ERR_TOOMANYSAMPLES:
				text = "The length provided exceed the allowable limit.";
				break;
			default:
				text = "Unknown error.";
				break;
			}
			return text;
		}
	}
}
