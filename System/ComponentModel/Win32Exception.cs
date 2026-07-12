using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[SuppressUnmanagedCodeSecurity]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[Serializable]
	public class Win32Exception : ExternalException, ISerializable
	{
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Win32Exception()
			: this(Marshal.GetLastWin32Error())
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Win32Exception(int error)
			: this(error, Win32Exception.GetErrorMessage(error))
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Win32Exception(int error, string message)
			: base(message)
		{
			this.nativeErrorCode = error;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Win32Exception(string message)
			: this(Marshal.GetLastWin32Error(), message)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public Win32Exception(string message, Exception innerException)
			: base(message, innerException)
		{
			this.nativeErrorCode = Marshal.GetLastWin32Error();
		}

		protected Win32Exception(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.nativeErrorCode = info.GetInt32("NativeErrorCode");
		}

		public int NativeErrorCode
		{
			get
			{
				return this.nativeErrorCode;
			}
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("NativeErrorCode", this.nativeErrorCode);
			base.GetObjectData(info, context);
		}

		[DllImport("Kernel32", CharSet = CharSet.Unicode)]
		private static extern int FormatMessage(int dwFlags, IntPtr lpSource, uint dwMessageId, int dwLanguageId, [Out] char[] lpBuffer, int nSize, IntPtr[] arguments);

		internal static string GetErrorMessage(int error)
		{
			if (!Environment.IsRunningOnWindows)
			{
				if (error <= 6200)
				{
					if (error <= 4006)
					{
						if (error <= 2109)
						{
							if (error <= 487)
							{
								switch (error)
								{
								case 0:
									return "Success";
								case 1:
									return "Invalid function";
								case 2:
									return "Cannot find the specified file";
								case 3:
									return "Cannot find the specified file";
								case 4:
									return "Too many open files";
								case 5:
									return "Access denied";
								case 6:
									return "Invalid handle";
								case 7:
									return "Arena trashed";
								case 8:
									return "Not enough memory";
								case 9:
									return "Invalid block";
								case 10:
									return "Bad environment";
								case 11:
									return "Bad format";
								case 12:
									return "Invalid access";
								case 13:
									return "Invalid data";
								case 14:
									return "Out of memory";
								case 15:
									return "Invalid drive";
								case 16:
									return "Current directory";
								case 17:
									return "Not same device";
								case 18:
									return "No more files";
								case 19:
									return "Write protect";
								case 20:
									return "Bad unit";
								case 21:
									return "Not ready";
								case 22:
									return "Bad command";
								case 23:
									return "CRC";
								case 24:
									return "Bad length";
								case 25:
									return "Seek";
								case 26:
									return "Not DOS disk";
								case 27:
									return "Sector not found";
								case 28:
									return "Out of paper";
								case 29:
									return "Write fault";
								case 30:
									return "Read fault";
								case 31:
									return "General failure";
								case 32:
									return "Sharing violation";
								case 33:
									return "Lock violation";
								case 34:
									return "Wrong disk";
								case 35:
								case 37:
								case 40:
								case 41:
								case 42:
								case 43:
								case 44:
								case 45:
								case 46:
								case 47:
								case 48:
								case 49:
								case 73:
								case 74:
								case 75:
								case 76:
								case 77:
								case 78:
								case 79:
								case 81:
								case 90:
								case 91:
								case 92:
								case 93:
								case 94:
								case 95:
								case 96:
								case 97:
								case 98:
								case 99:
								case 115:
								case 116:
								case 163:
								case 165:
								case 166:
								case 168:
								case 169:
								case 171:
								case 172:
								case 175:
								case 176:
								case 177:
								case 178:
								case 179:
								case 181:
								case 184:
								case 185:
								case 204:
								case 211:
								case 213:
								case 217:
								case 218:
								case 219:
								case 220:
								case 221:
								case 222:
								case 223:
								case 224:
								case 225:
								case 226:
								case 227:
								case 228:
								case 229:
								case 235:
								case 236:
								case 237:
								case 238:
								case 239:
								case 241:
								case 242:
								case 243:
								case 244:
								case 245:
								case 246:
								case 247:
								case 248:
								case 249:
								case 250:
								case 251:
								case 252:
								case 253:
								case 256:
								case 257:
								case 260:
								case 261:
								case 262:
								case 263:
								case 264:
								case 265:
								case 268:
								case 269:
								case 270:
								case 271:
								case 272:
								case 273:
								case 274:
								case 279:
								case 280:
								case 281:
								case 283:
								case 284:
								case 285:
								case 286:
								case 287:
								case 289:
								case 290:
								case 291:
								case 292:
								case 293:
								case 294:
								case 295:
								case 296:
								case 297:
								case 304:
								case 305:
								case 306:
								case 307:
								case 308:
								case 309:
								case 310:
								case 311:
								case 312:
								case 313:
								case 314:
								case 315:
								case 316:
									break;
								case 36:
									return "Sharing buffer exceeded";
								case 38:
									return "Handle EOF";
								case 39:
									return "Handle disk full";
								case 50:
									return "Operation not supported";
								case 51:
									return "Rem not list";
								case 52:
									return "Duplicate name";
								case 53:
									return "Bad netpath";
								case 54:
									return "Network busy";
								case 55:
									return "Device does not exist";
								case 56:
									return "Too many commands";
								case 57:
									return "ADAP HDW error";
								case 58:
									return "Bad net response";
								case 59:
									return "Unexpected net error";
								case 60:
									return "Bad rem adap";
								case 61:
									return "Print queue full";
								case 62:
									return "No spool space";
								case 63:
									return "Print cancelled";
								case 64:
									return "Netname deleted";
								case 65:
									return "Network access denied";
								case 66:
									return "Bad device type";
								case 67:
									return "Bad net name";
								case 68:
									return "Too many names";
								case 69:
									return "Too many sessions";
								case 70:
									return "Sharing paused";
								case 71:
									return "Req not accep";
								case 72:
									return "Redir paused";
								case 80:
									return "File exists";
								case 82:
									return "Cannot make";
								case 83:
									return "Fail i24";
								case 84:
									return "Out of structures";
								case 85:
									return "Already assigned";
								case 86:
									return "Invalid password";
								case 87:
									return "Invalid parameter";
								case 88:
									return "Net write fault";
								case 89:
									return "No proc slots";
								case 100:
									return "Too many semaphores";
								case 101:
									return "Exclusive semaphore already owned";
								case 102:
									return "Semaphore is set";
								case 103:
									return "Too many semaphore requests";
								case 104:
									return "Invalid at interrupt time";
								case 105:
									return "Semaphore owner died";
								case 106:
									return "Semaphore user limit";
								case 107:
									return "Disk change";
								case 108:
									return "Drive locked";
								case 109:
									return "Broken pipe";
								case 110:
									return "Open failed";
								case 111:
									return "Buffer overflow";
								case 112:
									return "Disk full";
								case 113:
									return "No more search handles";
								case 114:
									return "Invalid target handle";
								case 117:
									return "Invalid category";
								case 118:
									return "Invalid verify switch";
								case 119:
									return "Bad driver level";
								case 120:
									return "Call not implemented";
								case 121:
									return "Semaphore timeout";
								case 122:
									return "Insufficient buffer";
								case 123:
									return "Invalid name";
								case 124:
									return "Invalid level";
								case 125:
									return "No volume label";
								case 126:
									return "Module not found";
								case 127:
									return "Process not found";
								case 128:
									return "Wait no children";
								case 129:
									return "Child not complete";
								case 130:
									return "Direct access handle";
								case 131:
									return "Negative seek";
								case 132:
									return "Seek on device";
								case 133:
									return "Is join target";
								case 134:
									return "Is joined";
								case 135:
									return "Is substed";
								case 136:
									return "Not joined";
								case 137:
									return "Not substed";
								case 138:
									return "Join to join";
								case 139:
									return "Subst to subst";
								case 140:
									return "Join to subst";
								case 141:
									return "Subst to join";
								case 142:
									return "Busy drive";
								case 143:
									return "Same drive";
								case 144:
									return "Directory not root";
								case 145:
									return "Directory not empty";
								case 146:
									return "Is subst path";
								case 147:
									return "Is join path";
								case 148:
									return "Path busy";
								case 149:
									return "Is subst target";
								case 150:
									return "System trace";
								case 151:
									return "Invalid event count";
								case 152:
									return "Too many muxwaiters";
								case 153:
									return "Invalid list format";
								case 154:
									return "Label too long";
								case 155:
									return "Too many TCBs";
								case 156:
									return "Signal refused";
								case 157:
									return "Discarded";
								case 158:
									return "Not locked";
								case 159:
									return "Bad thread ID addr";
								case 160:
									return "Bad arguments";
								case 161:
									return "Bad pathname";
								case 162:
									return "Signal pending";
								case 164:
									return "Max thrds reached";
								case 167:
									return "Lock failed";
								case 170:
									return "Busy";
								case 173:
									return "Cancel violation";
								case 174:
									return "Atomic locks not supported";
								case 180:
									return "Invalid segment number";
								case 182:
									return "Invalid ordinal";
								case 183:
									return "Already exists";
								case 186:
									return "Invalid flag number";
								case 187:
									return "Sem not found";
								case 188:
									return "Invalid starting codeseg";
								case 189:
									return "Invalid stackseg";
								case 190:
									return "Invalid moduletype";
								case 191:
									return "Invalid exe signature";
								case 192:
									return "Exe marked invalid";
								case 193:
									return "Bad exe format";
								case 194:
									return "Iterated data exceeds 64k (and that should be enough for anybody!)";
								case 195:
									return "Invalid minallocsize";
								case 196:
									return "Dynlink from invalid ring";
								case 197:
									return "IOPL not enabled";
								case 198:
									return "Invalid segdpl";
								case 199:
									return "Autodataseg exceeds 64k";
								case 200:
									return "Ring2seg must be movable";
								case 201:
									return "Reloc chain exceeds seglim";
								case 202:
									return "Infloop in reloc chain";
								case 203:
									return "Env var not found";
								case 205:
									return "No signal sent";
								case 206:
									return "Filename exceeds range";
								case 207:
									return "Ring2 stack in use";
								case 208:
									return "Meta expansion too long";
								case 209:
									return "Invalid signal number";
								case 210:
									return "Thread 1 inactive";
								case 212:
									return "Locked";
								case 214:
									return "Too many modules";
								case 215:
									return "Nesting not allowed";
								case 216:
									return "Exe machine type mismatch";
								case 230:
									return "Bad pipe";
								case 231:
									return "Pipe busy";
								case 232:
									return "No data";
								case 233:
									return "Pipe not connected";
								case 234:
									return "More data";
								case 240:
									return "VC disconnected";
								case 254:
									return "Invalid EA name";
								case 255:
									return "EA list inconsistent";
								case 258:
									return "Wait timeout";
								case 259:
									return "No more items";
								case 266:
									return "Cannot copy";
								case 267:
									return "Is a directory";
								case 275:
									return "EAS didnt fit";
								case 276:
									return "EA file corrupt";
								case 277:
									return "EA table full";
								case 278:
									return "Invalid EA handle";
								case 282:
									return "EAs not supported";
								case 288:
									return "Not owner";
								case 298:
									return "Too many posts";
								case 299:
									return "Partial copy";
								case 300:
									return "Oplock not granted";
								case 301:
									return "Invalid oplock protocol";
								case 302:
									return "Disk too fragmented";
								case 303:
									return "Delete pending";
								case 317:
									return "Mr Mid not found";
								default:
									if (error == 487)
									{
										return "Invalid address";
									}
									break;
								}
							}
							else
							{
								switch (error)
								{
								case 534:
									return "Arithmetic overflow";
								case 535:
									return "Pipe connected";
								case 536:
									return "Pipe listening";
								default:
									switch (error)
									{
									case 994:
										return "EA access denied";
									case 995:
										return "Operation aborted";
									case 996:
										return "IO incomplete";
									case 997:
										return "IO pending";
									case 998:
										return "No access";
									case 999:
										return "Swap error";
									case 1001:
										return "Stack overflow";
									case 1002:
										return "Invalid message";
									case 1003:
										return "Can not complete";
									case 1004:
										return "Invalid flags";
									case 1005:
										return "Unrecognised volume";
									case 1006:
										return "File invalid";
									case 1007:
										return "Full screen mode";
									case 1008:
										return "No token";
									case 1009:
										return "Bad DB";
									case 1010:
										return "Bad key";
									case 1011:
										return "Can't open";
									case 1012:
										return "Can't read";
									case 1013:
										return "Can't write";
									case 1014:
										return "Registry recovered";
									case 1015:
										return "Registry corrupt";
									case 1016:
										return "Registry IO failed";
									case 1017:
										return "Not registry file";
									case 1018:
										return "Key deleted";
									case 1019:
										return "No log space";
									case 1020:
										return "Key has children";
									case 1021:
										return "Child must be volatile";
									case 1022:
										return "Notify enum dir";
									case 1051:
										return "Dependent services running";
									case 1052:
										return "Invalid service control";
									case 1053:
										return "Service request timeout";
									case 1054:
										return "Service no thread";
									case 1055:
										return "Service database locked";
									case 1056:
										return "Service already running";
									case 1057:
										return "Invalid service account";
									case 1058:
										return "Service disabled";
									case 1059:
										return "Circular dependency";
									case 1060:
										return "Service does not exist";
									case 1061:
										return "Service cannot accept ctrl";
									case 1062:
										return "Service not active";
									case 1063:
										return "Failed service controller connect";
									case 1064:
										return "Exception in service";
									case 1065:
										return "Database does not exist";
									case 1066:
										return "Service specific error";
									case 1067:
										return "Process aborted";
									case 1068:
										return "Service dependency fail";
									case 1069:
										return "Service logon failed";
									case 1070:
										return "Service start hang";
									case 1071:
										return "Invalid service lock";
									case 1072:
										return "Service marked for delete";
									case 1073:
										return "Service exists";
									case 1074:
										return "Already running lkg";
									case 1075:
										return "Service dependency deleted";
									case 1076:
										return "Boot already accepted";
									case 1077:
										return "Service never started";
									case 1078:
										return "Duplicate service name";
									case 1079:
										return "Different service account";
									case 1080:
										return "Cannot detect driver failure";
									case 1081:
										return "Cannot detect process abort";
									case 1082:
										return "No recovery program";
									case 1083:
										return "Service not in exe";
									case 1084:
										return "Not safeboot service";
									case 1100:
										return "End of media";
									case 1101:
										return "Filemark detected";
									case 1102:
										return "Beginning of media";
									case 1103:
										return "Setmark detected";
									case 1104:
										return "No data detected";
									case 1105:
										return "Partition failure";
									case 1106:
										return "Invalid block length";
									case 1107:
										return "Device not partitioned";
									case 1108:
										return "Unable to lock media";
									case 1109:
										return "Unable to unload media";
									case 1110:
										return "Media changed";
									case 1111:
										return "Bus reset";
									case 1112:
										return "No media in drive";
									case 1113:
										return "No unicode translation";
									case 1114:
										return "DLL init failed";
									case 1115:
										return "Shutdown in progress";
									case 1116:
										return "No shutdown in progress";
									case 1117:
										return "IO device";
									case 1118:
										return "Serial IO device";
									case 1119:
										return "IRQ busy";
									case 1120:
										return "More writes";
									case 1121:
										return "Counter timeout";
									case 1122:
										return "Floppy ID mark not found";
									case 1123:
										return "Floppy wrong cylinder";
									case 1124:
										return "Floppy unknown error";
									case 1125:
										return "Floppy bad registers";
									case 1126:
										return "Disk recalibrate failed";
									case 1127:
										return "Disk operation failed";
									case 1128:
										return "Disk reset failed";
									case 1129:
										return "EOM overflow";
									case 1130:
										return "Not enough server memory";
									case 1131:
										return "Possible deadlock";
									case 1132:
										return "Mapped alignment";
									case 1140:
										return "Set power state vetoed";
									case 1141:
										return "Set power state failed";
									case 1142:
										return "Too many links";
									case 1150:
										return "Old win version";
									case 1151:
										return "App wrong OS";
									case 1152:
										return "Single instance app";
									case 1153:
										return "Rmode app";
									case 1154:
										return "Invalid DLL";
									case 1155:
										return "No association";
									case 1156:
										return "DDE fail";
									case 1157:
										return "DLL not found";
									case 1158:
										return "No more user handles";
									case 1159:
										return "Message sync only";
									case 1160:
										return "Source element empty";
									case 1161:
										return "Destination element full";
									case 1162:
										return "Illegal element address";
									case 1163:
										return "Magazine not present";
									case 1164:
										return "Device reinitialization needed";
									case 1165:
										return "Device requires cleaning";
									case 1166:
										return "Device door open";
									case 1167:
										return "Device not connected";
									case 1168:
										return "Not found";
									case 1169:
										return "No match";
									case 1170:
										return "Set not found";
									case 1171:
										return "Point not found";
									case 1172:
										return "No tracking service";
									case 1173:
										return "No volume ID";
									case 1175:
										return "Unable to remove replaced";
									case 1176:
										return "Unable to move replacement";
									case 1177:
										return "Unable to move replacement 2";
									case 1178:
										return "Journal delete in progress";
									case 1179:
										return "Journal not active";
									case 1180:
										return "Potential file found";
									case 1181:
										return "Journal entry deleted";
									case 1200:
										return "Bad device";
									case 1201:
										return "Connection unavail";
									case 1202:
										return "Device already remembered";
									case 1203:
										return "No net or bad path";
									case 1204:
										return "Bad provider";
									case 1205:
										return "Cannot open profile";
									case 1206:
										return "Bad profile";
									case 1207:
										return "Not container";
									case 1208:
										return "Extended error";
									case 1209:
										return "Invalid group name";
									case 1210:
										return "Invalid computer name";
									case 1211:
										return "Invalid event name";
									case 1212:
										return "Invalid domain name";
									case 1213:
										return "Invalid service name";
									case 1214:
										return "Invalid net name";
									case 1215:
										return "Invalid share name";
									case 1216:
										return "Invalid password name";
									case 1217:
										return "Invalid message name";
									case 1218:
										return "Invalid message dest";
									case 1219:
										return "Session credential conflict";
									case 1220:
										return "Remote session limit exceeded";
									case 1221:
										return "Dup domain name";
									case 1222:
										return "No network";
									case 1223:
										return "Cancelled";
									case 1224:
										return "User mapped file";
									case 1225:
										return "Connection refused";
									case 1226:
										return "Graceful disconnect";
									case 1227:
										return "Address already associated";
									case 1228:
										return "Address not associated";
									case 1229:
										return "Connected invalid";
									case 1230:
										return "Connection active";
									case 1231:
										return "Network unreachable";
									case 1232:
										return "Host unreachable";
									case 1233:
										return "Protocol unreachable";
									case 1234:
										return "Port unreachable";
									case 1235:
										return "Request aborted";
									case 1236:
										return "Connection aborted";
									case 1237:
										return "Retry";
									case 1238:
										return "Connection count limit";
									case 1239:
										return "Login time restriction";
									case 1240:
										return "Login wksta restriction";
									case 1241:
										return "Incorrect address";
									case 1242:
										return "Already registered";
									case 1243:
										return "Service not found";
									case 1244:
										return "Not authenticated";
									case 1245:
										return "Not logged on";
									case 1246:
										return "Continue";
									case 1247:
										return "Already initialised";
									case 1248:
										return "No more devices";
									case 1249:
										return "No such site";
									case 1250:
										return "Domain controller exists";
									case 1251:
										return "Only if connected";
									case 1252:
										return "Override no changes";
									case 1253:
										return "Bad user profile";
									case 1254:
										return "Not supported on SBS";
									case 1255:
										return "Server shutdown in progress";
									case 1256:
										return "Host down";
									case 1257:
										return "Non account sid";
									case 1258:
										return "Non domain sid";
									case 1259:
										return "Apphelp block";
									case 1260:
										return "Access disabled by policy";
									case 1261:
										return "Reg nat consumption";
									case 1262:
										return "CSC share offline";
									case 1263:
										return "PK init failure";
									case 1264:
										return "Smartcard subsystem failure";
									case 1265:
										return "Downgrade detected";
									case 1266:
										return "Smartcard cert revoked";
									case 1267:
										return "Issuing CA untrusted";
									case 1268:
										return "Revocation offline";
									case 1269:
										return "PK init client failure";
									case 1270:
										return "Smartcard cert expired";
									case 1271:
										return "Machine locked";
									case 1273:
										return "Callback supplied invalid data";
									case 1274:
										return "Sync foreground refresh required";
									case 1275:
										return "Driver blocked";
									case 1276:
										return "Invalid import of non DLL";
									case 1300:
										return "Not all assigned";
									case 1301:
										return "Some not mapped";
									case 1302:
										return "No quotas for account";
									case 1303:
										return "Local user session key";
									case 1304:
										return "Null LM password";
									case 1305:
										return "Unknown revision";
									case 1306:
										return "Revision mismatch";
									case 1307:
										return "Invalid owner";
									case 1308:
										return "Invalid primary group";
									case 1309:
										return "No impersonation token";
									case 1310:
										return "Can't disable mandatory";
									case 1311:
										return "No logon servers";
									case 1312:
										return "No such logon session";
									case 1313:
										return "No such privilege";
									case 1314:
										return "Privilege not held";
									case 1315:
										return "Invalid account name";
									case 1316:
										return "User exists";
									case 1317:
										return "No such user";
									case 1318:
										return "Group exists";
									case 1319:
										return "No such group";
									case 1320:
										return "Member in group";
									case 1321:
										return "Member not in group";
									case 1322:
										return "Last admin";
									case 1323:
										return "Wrong password";
									case 1324:
										return "Ill formed password";
									case 1325:
										return "Password restriction";
									case 1326:
										return "Logon failure";
									case 1327:
										return "Account restriction";
									case 1328:
										return "Invalid logon hours";
									case 1329:
										return "Invalid workstation";
									case 1330:
										return "Password expired";
									case 1331:
										return "Account disabled";
									case 1332:
										return "None mapped";
									case 1333:
										return "Too many LUIDs requested";
									case 1334:
										return "LUIDs exhausted";
									case 1335:
										return "Invalid sub authority";
									case 1336:
										return "Invalid ACL";
									case 1337:
										return "Invalid SID";
									case 1338:
										return "Invalid security descr";
									case 1340:
										return "Bad inheritance ACL";
									case 1341:
										return "Server disabled";
									case 1342:
										return "Server not disabled";
									case 1343:
										return "Invalid ID authority";
									case 1344:
										return "Allotted space exceeded";
									case 1345:
										return "Invalid group attributes";
									case 1346:
										return "Bad impersonation level";
									case 1347:
										return "Can't open anonymous";
									case 1348:
										return "Bad validation class";
									case 1349:
										return "Bad token type";
									case 1350:
										return "No security on object";
									case 1351:
										return "Can't access domain info";
									case 1352:
										return "Invalid server state";
									case 1353:
										return "Invalid domain state";
									case 1354:
										return "Invalid domain role";
									case 1355:
										return "No such domain";
									case 1356:
										return "Domain exists";
									case 1357:
										return "Domain limit exceeded";
									case 1358:
										return "Internal DB corruption";
									case 1359:
										return "Internal error";
									case 1360:
										return "Generic not mapped";
									case 1361:
										return "Bad descriptor format";
									case 1362:
										return "Not logon process";
									case 1363:
										return "Logon session exists";
									case 1364:
										return "No such package";
									case 1365:
										return "Bad logon session state";
									case 1366:
										return "Logon session collision";
									case 1367:
										return "Invalid logon type";
									case 1368:
										return "Cannot impersonate";
									case 1369:
										return "Rxact invalid state";
									case 1370:
										return "Rxact commit failure";
									case 1371:
										return "Special account";
									case 1372:
										return "Special group";
									case 1373:
										return "Special user";
									case 1374:
										return "Members primary group";
									case 1375:
										return "Token already in use";
									case 1376:
										return "No such alias";
									case 1377:
										return "Member not in alias";
									case 1378:
										return "Member in alias";
									case 1379:
										return "Alias exists";
									case 1380:
										return "Logon not granted";
									case 1381:
										return "Too many secrets";
									case 1382:
										return "Secret too long";
									case 1383:
										return "Internal DB error";
									case 1384:
										return "Too many context IDs";
									case 1385:
										return "Logon type not granted";
									case 1386:
										return "NT cross encryption required";
									case 1387:
										return "No such member";
									case 1388:
										return "Invalid member";
									case 1389:
										return "Too many SIDs";
									case 1390:
										return "LM cross encryption required";
									case 1391:
										return "No inheritance";
									case 1392:
										return "File corrupt";
									case 1393:
										return "Disk corrupt";
									case 1394:
										return "No user session key";
									case 1395:
										return "Licence quota exceeded";
									case 1396:
										return "Wrong target name";
									case 1397:
										return "Mutual auth failed";
									case 1398:
										return "Time skew";
									case 1399:
										return "Current domain not allowed";
									case 1400:
										return "Invalid window handle";
									case 1401:
										return "Invalid menu handle";
									case 1402:
										return "Invalid cursor handle";
									case 1403:
										return "Invalid accel handle";
									case 1404:
										return "Invalid hook handle";
									case 1405:
										return "Invalid DWP handle";
									case 1406:
										return "TLW with wschild";
									case 1407:
										return "Cannot find WND class";
									case 1408:
										return "Window of other thread";
									case 1409:
										return "Hotkey already registered";
									case 1410:
										return "Class already exists";
									case 1411:
										return "Class does not exist";
									case 1412:
										return "Class has windows";
									case 1413:
										return "Invalid index";
									case 1414:
										return "Invalid icon handle";
									case 1415:
										return "Private dialog index";
									case 1416:
										return "Listbox ID not found";
									case 1417:
										return "No wildcard characters";
									case 1418:
										return "Clipboard not open";
									case 1419:
										return "Hotkey not registered";
									case 1420:
										return "Window not dialog";
									case 1421:
										return "Control ID not found";
									case 1422:
										return "Invalid combobox message";
									case 1423:
										return "Window not combobox";
									case 1424:
										return "Invalid edit height";
									case 1425:
										return "DC not found";
									case 1426:
										return "Invalid hook filter";
									case 1427:
										return "Invalid filter proc";
									case 1428:
										return "Hook needs HMOD";
									case 1429:
										return "Global only hook";
									case 1430:
										return "Journal hook set";
									case 1431:
										return "Hook not installed";
									case 1432:
										return "Invalid LB message";
									case 1433:
										return "Setcount on bad LB";
									case 1434:
										return "LB without tabstops";
									case 1435:
										return "Destroy object of other thread";
									case 1436:
										return "Child window menu";
									case 1437:
										return "No system menu";
									case 1438:
										return "Invalid msgbox style";
									case 1439:
										return "Invalid SPI value";
									case 1440:
										return "Screen already locked";
									case 1441:
										return "HWNDs have different parent";
									case 1442:
										return "Not child window";
									case 1443:
										return "Invalid GW command";
									case 1444:
										return "Invalid thread ID";
									case 1445:
										return "Non MDI child window";
									case 1446:
										return "Popup already active";
									case 1447:
										return "No scrollbars";
									case 1448:
										return "Invalid scrollbar range";
									case 1449:
										return "Invalid showwin command";
									case 1450:
										return "No system resources";
									case 1451:
										return "Nonpaged system resources";
									case 1452:
										return "Paged system resources";
									case 1453:
										return "Working set quota";
									case 1454:
										return "Pagefile quota";
									case 1455:
										return "Commitment limit";
									case 1456:
										return "Menu item not found";
									case 1457:
										return "Invalid keyboard handle";
									case 1458:
										return "Hook type not allowed";
									case 1459:
										return "Requires interactive windowstation";
									case 1460:
										return "Timeout";
									case 1461:
										return "Invalid monitor handle";
									case 1500:
										return "Eventlog file corrupt";
									case 1501:
										return "Eventlog can't start";
									case 1502:
										return "Log file full";
									case 1503:
										return "Eventlog file changed";
									case 1601:
										return "Install service failure";
									case 1602:
										return "Install userexit";
									case 1603:
										return "Install failure";
									case 1604:
										return "Install suspend";
									case 1605:
										return "Unknown product";
									case 1606:
										return "Unknown feature";
									case 1607:
										return "Unknown component";
									case 1608:
										return "Unknown property";
									case 1609:
										return "Invalid handle state";
									case 1610:
										return "Bad configuration";
									case 1611:
										return "Index absent";
									case 1612:
										return "Install source absent";
									case 1613:
										return "Install package version";
									case 1614:
										return "Product uninstalled";
									case 1615:
										return "Bad query syntax";
									case 1616:
										return "Invalid field";
									case 1617:
										return "Device removed";
									case 1618:
										return "Install already running";
									case 1619:
										return "Install package open failed";
									case 1620:
										return "Install package invalid";
									case 1621:
										return "Install UI failure";
									case 1622:
										return "Install log failure";
									case 1623:
										return "Install language unsupported";
									case 1624:
										return "Install transform failure";
									case 1625:
										return "Install package rejected";
									case 1626:
										return "Function not called";
									case 1627:
										return "Function failed";
									case 1628:
										return "Invalid table";
									case 1629:
										return "Datatype mismatch";
									case 1630:
										return "Unsupported type";
									case 1631:
										return "Create failed";
									case 1632:
										return "Install temp unwritable";
									case 1633:
										return "Install platform unsupported";
									case 1634:
										return "Install notused";
									case 1635:
										return "Patch package open failed";
									case 1636:
										return "Patch package invalid";
									case 1637:
										return "Patch package unsupported";
									case 1638:
										return "Product version";
									case 1639:
										return "Invalid command line";
									case 1640:
										return "Install remote disallowed";
									case 1641:
										return "Success reboot initiated";
									case 1642:
										return "Patch target not found";
									case 1643:
										return "Patch package rejected";
									case 1644:
										return "Install transform rejected";
									case 1700:
										return "RPC S Invalid string binding";
									case 1701:
										return "RPC S Wrong kind of binding";
									case 1702:
										return "RPC S Invalid binding";
									case 1703:
										return "RPC S Protseq not supported";
									case 1704:
										return "RPC S Invalid RPC protseq";
									case 1705:
										return "RPC S Invalid string UUID";
									case 1706:
										return "RPC S Invalid endpoint format";
									case 1707:
										return "RPC S Invalid net addr";
									case 1708:
										return "RPC S No endpoint found";
									case 1709:
										return "RPC S Invalid timeout";
									case 1710:
										return "RPC S Object not found";
									case 1711:
										return "RPC S Already registered";
									case 1712:
										return "RPC S Type already registered";
									case 1713:
										return "RPC S Already listening";
									case 1714:
										return "RPC S Not protseqs registered";
									case 1715:
										return "RPC S Not listening";
									case 1716:
										return "RPC S Unknown mgr type";
									case 1717:
										return "RPC S Unknown IF";
									case 1718:
										return "RPC S No bindings";
									case 1719:
										return "RPC S Not protseqs";
									case 1720:
										return "RPC S Can't create endpoint";
									case 1721:
										return "RPC S Out of resources";
									case 1722:
										return "RPC S Server unavailable";
									case 1723:
										return "RPC S Server too busy";
									case 1724:
										return "RPC S Invalid network options";
									case 1725:
										return "RPC S No call active";
									case 1726:
										return "RPC S Call failed";
									case 1727:
										return "RPC S Call failed DNE";
									case 1728:
										return "RPC S Protocol error";
									case 1730:
										return "RPC S Unsupported trans syn";
									case 1732:
										return "RPC S Unsupported type";
									case 1733:
										return "RPC S Invalid tag";
									case 1734:
										return "RPC S Invalid bound";
									case 1735:
										return "RPC S No entry name";
									case 1736:
										return "RPC S Invalid name syntax";
									case 1737:
										return "RPC S Unsupported name syntax";
									case 1739:
										return "RPC S UUID no address";
									case 1740:
										return "RPC S Duplicate endpoint";
									case 1741:
										return "RPC S Unknown authn type";
									case 1742:
										return "RPC S Max calls too small";
									case 1743:
										return "RPC S String too long";
									case 1744:
										return "RPC S Protseq not found";
									case 1745:
										return "RPC S Procnum out of range";
									case 1746:
										return "RPC S Binding has no auth";
									case 1747:
										return "RPC S Unknown authn service";
									case 1748:
										return "RPC S Unknown authn level";
									case 1749:
										return "RPC S Invalid auth identity";
									case 1750:
										return "RPC S Unknown authz service";
									case 1751:
										return "EPT S Invalid entry";
									case 1752:
										return "EPT S Can't perform op";
									case 1753:
										return "EPT S Not registered";
									case 1754:
										return "RPC S Nothing to export";
									case 1755:
										return "RPC S Incomplete name";
									case 1756:
										return "RPC S Invalid vers option";
									case 1757:
										return "RPC S No more members";
									case 1758:
										return "RPC S Not all objs unexported";
									case 1759:
										return "RPC S Interface not found";
									case 1760:
										return "RPC S Entry already exists";
									case 1761:
										return "RPC S Entry not found";
									case 1762:
										return "RPC S Name service unavailable";
									case 1763:
										return "RPC S Invalid naf ID";
									case 1764:
										return "RPC S Cannot support";
									case 1765:
										return "RPC S No context available";
									case 1766:
										return "RPC S Internal error";
									case 1767:
										return "RPC S Zero divide";
									case 1768:
										return "RPC S Address error";
									case 1769:
										return "RPC S FP div zero";
									case 1770:
										return "RPC S FP Underflow";
									case 1771:
										return "RPC S Overflow";
									case 1772:
										return "RPC X No more entries";
									case 1773:
										return "RPC X SS char trans open fail";
									case 1774:
										return "RPC X SS char trans short file";
									case 1775:
										return "RPC S SS in null context";
									case 1777:
										return "RPC X SS context damaged";
									case 1778:
										return "RPC X SS handles mismatch";
									case 1779:
										return "RPC X SS cannot get call handle";
									case 1780:
										return "RPC X Null ref pointer";
									case 1781:
										return "RPC X enum value out of range";
									case 1782:
										return "RPC X byte count too small";
									case 1783:
										return "RPC X bad stub data";
									case 1784:
										return "Invalid user buffer";
									case 1785:
										return "Unrecognised media";
									case 1786:
										return "No trust lsa secret";
									case 1787:
										return "No trust sam account";
									case 1788:
										return "Trusted domain failure";
									case 1789:
										return "Trusted relationship failure";
									case 1790:
										return "Trust failure";
									case 1791:
										return "RPC S call in progress";
									case 1792:
										return "Error netlogon not started";
									case 1793:
										return "Account expired";
									case 1794:
										return "Redirector has open handles";
									case 1795:
										return "Printer driver already installed";
									case 1796:
										return "Unknown port";
									case 1797:
										return "Unknown printer driver";
									case 1798:
										return "Unknown printprocessor";
									case 1799:
										return "Invalid separator file";
									case 1800:
										return "Invalid priority";
									case 1801:
										return "Invalid printer name";
									case 1802:
										return "Printer already exists";
									case 1803:
										return "Invalid printer command";
									case 1804:
										return "Invalid datatype";
									case 1805:
										return "Invalid environment";
									case 1806:
										return "RPC S no more bindings";
									case 1807:
										return "Nologon interdomain trust account";
									case 1808:
										return "Nologon workstation trust account";
									case 1809:
										return "Nologon server trust account";
									case 1810:
										return "Domain trust inconsistent";
									case 1811:
										return "Server has open handles";
									case 1812:
										return "Resource data not found";
									case 1813:
										return "Resource type not found";
									case 1814:
										return "Resource name not found";
									case 1815:
										return "Resource lang not found";
									case 1816:
										return "Not enough quota";
									case 1817:
										return "RPC S no interfaces";
									case 1818:
										return "RPC S Call cancelled";
									case 1819:
										return "RPC S Binding incomplete";
									case 1820:
										return "RPC S Comm failure";
									case 1821:
										return "RPC S Unsupported authn level";
									case 1822:
										return "RPC S No princ name";
									case 1823:
										return "RPC S Not RPC error";
									case 1824:
										return "RPC U UUID local only";
									case 1825:
										return "RPC S Sec pkg error";
									case 1826:
										return "RPC S Not cancelled";
									case 1827:
										return "RPC X Invalid ES action";
									case 1828:
										return "RPC X Wrong ES version";
									case 1829:
										return "RPC X Wrong stub version";
									case 1830:
										return "RPC X Invalid pipe object";
									case 1831:
										return "RPC X Wrong pipe order";
									case 1832:
										return "RPC X Wrong pipe version";
									case 1898:
										return "RPC S group member not found";
									case 1899:
										return "EPT S Can't create";
									case 1900:
										return "RPC S Invalid object";
									case 1901:
										return "Invalid time";
									case 1902:
										return "Invalid form name";
									case 1903:
										return "Invalid form size";
									case 1904:
										return "Already waiting";
									case 1905:
										return "Printer deleted";
									case 1906:
										return "Invalid printer state";
									case 1907:
										return "Password must change";
									case 1908:
										return "Domain controller not found";
									case 1909:
										return "Account locked out";
									case 1910:
										return "OR Invalid OXID";
									case 1911:
										return "OR Invalid OID";
									case 1912:
										return "OR Invalid set";
									case 1913:
										return "RPC S Send incomplete";
									case 1914:
										return "RPC S Invalid async handle";
									case 1915:
										return "RPC S Invalid async call";
									case 1916:
										return "RPC X Pipe closed";
									case 1917:
										return "RPC X Pipe discipline error";
									case 1918:
										return "RPC X Pipe empty";
									case 1919:
										return "No sitename";
									case 1920:
										return "Can't access file";
									case 1921:
										return "Can't resolve filename";
									case 1922:
										return "RPC S Entry type mismatch";
									case 1923:
										return "RPC S Not all objs exported";
									case 1924:
										return "RPC S Interface not exported";
									case 1925:
										return "RPC S Profile not added";
									case 1926:
										return "RPC S PRF ELT not added";
									case 1927:
										return "RPC S PRF ELT not removed";
									case 1928:
										return "RPC S GRP ELT not added";
									case 1929:
										return "RPC S GRP ELT not removed";
									case 1930:
										return "KM driver blocked";
									case 1931:
										return "Context expired";
									case 2000:
										return "Invalid pixel format";
									case 2001:
										return "Bad driver";
									case 2002:
										return "Invalid window style";
									case 2003:
										return "Metafile not supported";
									case 2004:
										return "Transform not supported";
									case 2005:
										return "Clipping not supported";
									case 2010:
										return "Invalid CMM";
									case 2011:
										return "Invalid profile";
									case 2012:
										return "Tag not found";
									case 2013:
										return "Tag not present";
									case 2014:
										return "Duplicate tag";
									case 2015:
										return "Profile not associated with device";
									case 2016:
										return "Profile not found";
									case 2017:
										return "Invalid colorspace";
									case 2018:
										return "ICM not enabled";
									case 2019:
										return "Deleting ICM xform";
									case 2020:
										return "Invalid transform";
									case 2021:
										return "Colorspace mismatch";
									case 2022:
										return "Invalid colorindex";
									case 2108:
										return "Connected other password";
									case 2109:
										return "Connected other password default";
									}
									break;
								}
							}
						}
						else if (error <= 2250)
						{
							if (error == 2202)
							{
								return "Bad username";
							}
							if (error == 2250)
							{
								return "Not connected";
							}
						}
						else
						{
							switch (error)
							{
							case 2401:
								return "Open files";
							case 2402:
								return "Active connections";
							case 2403:
								break;
							case 2404:
								return "Device in use";
							default:
								switch (error)
								{
								case 3000:
									return "Unknown print monitor";
								case 3001:
									return "Printer driver in use";
								case 3002:
									return "Spool file not found";
								case 3003:
									return "SPL no startdoc";
								case 3004:
									return "SPL no addjob";
								case 3005:
									return "Print processor already installed";
								case 3006:
									return "Print monitor already installed";
								case 3007:
									return "Invalid print monitor";
								case 3008:
									return "Print monitor in use";
								case 3009:
									return "Printer has jobs queued";
								case 3010:
									return "Success reboot required";
								case 3011:
									return "Success restart required";
								case 3012:
									return "Printer not found";
								case 3013:
									return "Printer driver warned";
								case 3014:
									return "Printer driver blocked";
								default:
									switch (error)
									{
									case 4000:
										return "Wins internal";
									case 4001:
										return "Can not del local wins";
									case 4002:
										return "Static init";
									case 4003:
										return "Inc backup";
									case 4004:
										return "Full backup";
									case 4005:
										return "Rec not existent";
									case 4006:
										return "RPL not allowed";
									}
									break;
								}
								break;
							}
						}
					}
					else if (error <= 4500)
					{
						if (error <= 4214)
						{
							if (error == 4100)
							{
								return "DHCP address conflict";
							}
							switch (error)
							{
							case 4200:
								return "WMU GUID not found";
							case 4201:
								return "WMI instance not found";
							case 4202:
								return "WMI ItemID not found";
							case 4203:
								return "WMI try again";
							case 4204:
								return "WMI DP not found";
							case 4205:
								return "WMI unresolved instance ref";
							case 4206:
								return "WMU already enabled";
							case 4207:
								return "WMU GUID disconnected";
							case 4208:
								return "WMI server unavailable";
							case 4209:
								return "WMI DP failed";
							case 4210:
								return "WMI invalid MOF";
							case 4211:
								return "WMI invalid reginfo";
							case 4212:
								return "WMI already disabled";
							case 4213:
								return "WMI read only";
							case 4214:
								return "WMI set failure";
							}
						}
						else
						{
							switch (error)
							{
							case 4300:
								return "Invalid media";
							case 4301:
								return "Invalid library";
							case 4302:
								return "Invalid media pool";
							case 4303:
								return "Drive media mismatch";
							case 4304:
								return "Media offline";
							case 4305:
								return "Library offline";
							case 4306:
								return "Empty";
							case 4307:
								return "Not empty";
							case 4308:
								return "Media unavailable";
							case 4309:
								return "Resource disabled";
							case 4310:
								return "Invalid cleaner";
							case 4311:
								return "Unable to clean";
							case 4312:
								return "Object not found";
							case 4313:
								return "Database failure";
							case 4314:
								return "Database full";
							case 4315:
								return "Media incompatible";
							case 4316:
								return "Resource not present";
							case 4317:
								return "Invalid operation";
							case 4318:
								return "Media not available";
							case 4319:
								return "Device not available";
							case 4320:
								return "Request refused";
							case 4321:
								return "Invalid drive object";
							case 4322:
								return "Library full";
							case 4323:
								return "Medium not accessible";
							case 4324:
								return "Unable to load medium";
							case 4325:
								return "Unable to inventory drive";
							case 4326:
								return "Unable to inventory slot";
							case 4327:
								return "Unable to inventory transport";
							case 4328:
								return "Transport full";
							case 4329:
								return "Controlling ieport";
							case 4330:
								return "Unable to eject mounted media";
							case 4331:
								return "Cleaner slot set";
							case 4332:
								return "Cleaner slot not set";
							case 4333:
								return "Cleaner cartridge spent";
							case 4334:
								return "Unexpected omid";
							case 4335:
								return "Can't delete last item";
							case 4336:
								return "Message exceeds max size";
							case 4337:
								return "Volume contains sys files";
							case 4338:
								return "Indigenous type";
							case 4339:
								return "No supporting drives";
							case 4340:
								return "Cleaner cartridge installed";
							case 4341:
							case 4342:
							case 4343:
							case 4344:
							case 4345:
							case 4346:
							case 4347:
							case 4348:
							case 4349:
							case 4353:
							case 4354:
							case 4355:
							case 4356:
							case 4357:
							case 4358:
							case 4359:
							case 4360:
							case 4361:
							case 4362:
							case 4363:
							case 4364:
							case 4365:
							case 4366:
							case 4367:
							case 4368:
							case 4369:
							case 4370:
							case 4371:
							case 4372:
							case 4373:
							case 4374:
							case 4375:
							case 4376:
							case 4377:
							case 4378:
							case 4379:
							case 4380:
							case 4381:
							case 4382:
							case 4383:
							case 4384:
							case 4385:
							case 4386:
							case 4387:
							case 4388:
							case 4389:
								break;
							case 4350:
								return "Fill offline";
							case 4351:
								return "Remote storage not active";
							case 4352:
								return "Remote storage media error";
							case 4390:
								return "Not a reparse point";
							case 4391:
								return "Reparse attribute conflict";
							case 4392:
								return "Invalid reparse data";
							case 4393:
								return "Reparse tag invalid";
							case 4394:
								return "Reparse tag mismatch";
							default:
								if (error == 4500)
								{
									return "Volume not sis enabled";
								}
								break;
							}
						}
					}
					else if (error <= 5900)
					{
						switch (error)
						{
						case 5001:
							return "Dependent resource exists";
						case 5002:
							return "Dependency not found";
						case 5003:
							return "Dependency already exists";
						case 5004:
							return "Resource not online";
						case 5005:
							return "Host node not available";
						case 5006:
							return "Resource not available";
						case 5007:
							return "Resource not found";
						case 5008:
							return "Shutdown cluster";
						case 5009:
							return "Can't evict active node";
						case 5010:
							return "Object already exists";
						case 5011:
							return "Object in list";
						case 5012:
							return "Group not available";
						case 5013:
							return "Group not found";
						case 5014:
							return "Group not online";
						case 5015:
							return "Host node not resource owner";
						case 5016:
							return "Host node not group owner";
						case 5017:
							return "Resmon create failed";
						case 5018:
							return "Resmon online failed";
						case 5019:
							return "Resource online";
						case 5020:
							return "Quorum resource";
						case 5021:
							return "Not quorum capable";
						case 5022:
							return "Cluster shutting down";
						case 5023:
							return "Invalid state";
						case 5024:
							return "Resource properties stored";
						case 5025:
							return "Not quorum class";
						case 5026:
							return "Core resource";
						case 5027:
							return "Quorum resource online failed";
						case 5028:
							return "Quorumlog open failed";
						case 5029:
							return "Clusterlog corrupt";
						case 5030:
							return "Clusterlog record exceeds maxsize";
						case 5031:
							return "Clusterlog exceeds maxsize";
						case 5032:
							return "Clusterlog chkpoint not found";
						case 5033:
							return "Clusterlog not enough space";
						case 5034:
							return "Quorum owner alive";
						case 5035:
							return "Network not available";
						case 5036:
							return "Node not available";
						case 5037:
							return "All nodes not available";
						case 5038:
							return "Resource failed";
						case 5039:
							return "Cluster invalid node";
						case 5040:
							return "Cluster node exists";
						case 5041:
							return "Cluster join in progress";
						case 5042:
							return "Cluster node not found";
						case 5043:
							return "Cluster local node not found";
						case 5044:
							return "Cluster network exists";
						case 5045:
							return "Cluster network not found";
						case 5046:
							return "Cluster netinterface exists";
						case 5047:
							return "Cluster netinterface not found";
						case 5048:
							return "Cluster invalid request";
						case 5049:
							return "Cluster invalid network provider";
						case 5050:
							return "Cluster node down";
						case 5051:
							return "Cluster node unreachable";
						case 5052:
							return "Cluster node not member";
						case 5053:
							return "Cluster join not in progress";
						case 5054:
							return "Cluster invalid network";
						case 5055:
							break;
						case 5056:
							return "Cluster node up";
						case 5057:
							return "Cluster ipaddr in use";
						case 5058:
							return "Cluster node not paused";
						case 5059:
							return "Cluster no security context";
						case 5060:
							return "Cluster network not internal";
						case 5061:
							return "Cluster node already up";
						case 5062:
							return "Cluster node already down";
						case 5063:
							return "Cluster network already online";
						case 5064:
							return "Cluster network already offline";
						case 5065:
							return "Cluster node already member";
						case 5066:
							return "Cluster last internal network";
						case 5067:
							return "Cluster network has dependents";
						case 5068:
							return "Invalid operation on quorum";
						case 5069:
							return "Dependency not allowed";
						case 5070:
							return "Cluster node paused";
						case 5071:
							return "Node can't host resource";
						case 5072:
							return "Cluster node not ready";
						case 5073:
							return "Cluster node shutting down";
						case 5074:
							return "Cluster join aborted";
						case 5075:
							return "Cluster incompatible versions";
						case 5076:
							return "Cluster maxnum of resources exceeded";
						case 5077:
							return "Cluster system config changed";
						case 5078:
							return "Cluster resource type not found";
						case 5079:
							return "Cluster restype not supported";
						case 5080:
							return "Cluster resname not found";
						case 5081:
							return "Cluster no RPC packages registered";
						case 5082:
							return "Cluster owner not in preflist";
						case 5083:
							return "Cluster database seqmismatch";
						case 5084:
							return "Resmon invalid state";
						case 5085:
							return "Cluster gum not locker";
						case 5086:
							return "Quorum disk not found";
						case 5087:
							return "Database backup corrupt";
						case 5088:
							return "Cluster node already has DFS root";
						case 5089:
							return "Resource property unchangeable";
						default:
							switch (error)
							{
							case 5890:
								return "Cluster membership invalid state";
							case 5891:
								return "Cluster quorumlog not found";
							case 5892:
								return "Cluster membership halt";
							case 5893:
								return "Cluster instance ID mismatch";
							case 5894:
								return "Cluster network not found for IP";
							case 5895:
								return "Cluster property data type mismatch";
							case 5896:
								return "Cluster evict without cleanup";
							case 5897:
								return "Cluster parameter mismatch";
							case 5898:
								return "Node cannot be clustered";
							case 5899:
								return "Cluster wrong OS version";
							case 5900:
								return "Cluster can't create dup cluster name";
							}
							break;
						}
					}
					else
					{
						switch (error)
						{
						case 6000:
							return "Encryption failed";
						case 6001:
							return "Decryption failed";
						case 6002:
							return "File encrypted";
						case 6003:
							return "No recovery policy";
						case 6004:
							return "No EFS";
						case 6005:
							return "Wrong EFS";
						case 6006:
							return "No user keys";
						case 6007:
							return "File not encryped";
						case 6008:
							return "Not export format";
						case 6009:
							return "File read only";
						case 6010:
							return "Dir EFS disallowed";
						case 6011:
							return "EFS server not trusted";
						case 6012:
							return "Bad recovery policy";
						case 6013:
							return "ETS alg blob too big";
						case 6014:
							return "Volume not support EFS";
						case 6015:
							return "EFS disabled";
						case 6016:
							return "EFS version not support";
						default:
							if (error == 6118)
							{
								return "No browser servers found";
							}
							if (error == 6200)
							{
								return "Sched E service not localsystem";
							}
							break;
						}
					}
				}
				else if (error <= 9753)
				{
					if (error <= 9505)
					{
						if (error <= 8601)
						{
							switch (error)
							{
							case 7001:
								return "Ctx winstation name invalid";
							case 7002:
								return "Ctx invalid PD";
							case 7003:
								return "Ctx PD not found";
							case 7004:
								return "Ctx WD not found";
							case 7005:
								return "Ctx cannot make eventlog entry";
							case 7006:
								return "Ctx service name collision";
							case 7007:
								return "Ctx close pending";
							case 7008:
								return "Ctx no outbuf";
							case 7009:
								return "Ctx modem inf not found";
							case 7010:
								return "Ctx invalid modemname";
							case 7011:
								return "Ctx modem response error";
							case 7012:
								return "Ctx modem response timeout";
							case 7013:
								return "Ctx modem response no carrier";
							case 7014:
								return "Ctx modem response no dial tone";
							case 7015:
								return "Ctx modem response busy";
							case 7016:
								return "Ctx modem response voice";
							case 7017:
								return "Ctx TD error";
							case 7018:
							case 7019:
							case 7020:
							case 7021:
							case 7026:
							case 7027:
							case 7028:
							case 7029:
							case 7030:
							case 7031:
							case 7032:
							case 7033:
							case 7034:
							case 7036:
							case 7039:
							case 7043:
							case 7046:
							case 7047:
							case 7048:
								break;
							case 7022:
								return "Ctx winstation not found";
							case 7023:
								return "Ctx winstation already exists";
							case 7024:
								return "Ctx winstation busy";
							case 7025:
								return "Ctx bad video mode";
							case 7035:
								return "Ctx graphics invalid";
							case 7037:
								return "Ctx logon disabled";
							case 7038:
								return "Ctx not console";
							case 7040:
								return "Ctx client query timeout";
							case 7041:
								return "Ctx console disconnect";
							case 7042:
								return "Ctx console connect";
							case 7044:
								return "Ctx shadow denied";
							case 7045:
								return "Ctx winstation access denied";
							case 7049:
								return "Ctx invalid WD";
							case 7050:
								return "Ctx shadow invalid";
							case 7051:
								return "Ctx shadow disabled";
							case 7052:
								return "Ctx client licence in use";
							case 7053:
								return "Ctx client licence not set";
							case 7054:
								return "Ctx licence not available";
							case 7055:
								return "Ctx licence client invalid";
							case 7056:
								return "Ctx licence expired";
							case 7057:
								return "Ctx shadow not running";
							case 7058:
								return "Ctx shadow ended by mode change";
							default:
								switch (error)
								{
								case 8001:
									return "FRS err invalid API sequence";
								case 8002:
									return "FRS err starting service";
								case 8003:
									return "FRS err stopping service";
								case 8004:
									return "FRS err internal API";
								case 8005:
									return "FRS err internal";
								case 8006:
									return "FRS err service comm";
								case 8007:
									return "FRS err insufficient priv";
								case 8008:
									return "FRS err authentication";
								case 8009:
									return "FRS err parent insufficient priv";
								case 8010:
									return "FRS err parent authentication";
								case 8011:
									return "FRS err child to parent comm";
								case 8012:
									return "FRS err parent to child comm";
								case 8013:
									return "FRS err sysvol populate";
								case 8014:
									return "FRS err sysvol populate timeout";
								case 8015:
									return "FRS err sysvol is busy";
								case 8016:
									return "FRS err sysvol demote";
								case 8017:
									return "FRS err invalid service parameter";
								case 8200:
									return "DS not installed";
								case 8201:
									return "DS membership evaluated locally";
								case 8202:
									return "DS no attribute or value";
								case 8203:
									return "DS invalid attribute syntax";
								case 8204:
									return "DS attribute type undefined";
								case 8205:
									return "DS attribute or value exists";
								case 8206:
									return "DS busy";
								case 8207:
									return "DS unavailable";
								case 8208:
									return "DS no rids allocated";
								case 8209:
									return "DS no more rids";
								case 8210:
									return "DS incorrect role owner";
								case 8211:
									return "DS ridmgr init error";
								case 8212:
									return "DS obj class violation";
								case 8213:
									return "DS can't on non leaf";
								case 8214:
									return "DS can't on rnd";
								case 8215:
									return "DS can't mod obj class";
								case 8216:
									return "DS cross dom move error";
								case 8217:
									return "DS GC not available";
								case 8218:
									return "Shared policy";
								case 8219:
									return "Policy object not found";
								case 8220:
									return "Policy only in DS";
								case 8221:
									return "Promotion active";
								case 8222:
									return "No promotion active";
								case 8224:
									return "DS operations error";
								case 8225:
									return "DS protocol error";
								case 8226:
									return "DS timelimit exceeded";
								case 8227:
									return "DS sizelimit exceeded";
								case 8228:
									return "DS admin limit exceeded";
								case 8229:
									return "DS compare false";
								case 8230:
									return "DS compare true";
								case 8231:
									return "DS auth method not supported";
								case 8232:
									return "DS strong auth required";
								case 8233:
									return "DS inappropriate auth";
								case 8234:
									return "DS auth unknown";
								case 8235:
									return "DS referral";
								case 8236:
									return "DS unavailable crit extension";
								case 8237:
									return "DS confidentiality required";
								case 8238:
									return "DS inappropriate matching";
								case 8239:
									return "DS constraint violation";
								case 8240:
									return "DS no such object";
								case 8241:
									return "DS alias problem";
								case 8242:
									return "DS invalid dn syntax";
								case 8243:
									return "DS is leaf";
								case 8244:
									return "DS alias deref problem";
								case 8245:
									return "DS unwilling to perform";
								case 8246:
									return "DS loop detect";
								case 8247:
									return "DS naming violation";
								case 8248:
									return "DS object results too large";
								case 8249:
									return "DS affects multiple dsas";
								case 8250:
									return "DS server down";
								case 8251:
									return "DS local error";
								case 8252:
									return "DS encoding error";
								case 8253:
									return "DS decoding error";
								case 8254:
									return "DS filter unknown";
								case 8255:
									return "DS param error";
								case 8256:
									return "DS not supported";
								case 8257:
									return "DS no results returned";
								case 8258:
									return "DS control not found";
								case 8259:
									return "DS client loop";
								case 8260:
									return "DS referral limit exceeded";
								case 8261:
									return "DS sort control missing";
								case 8262:
									return "DS offset range error";
								case 8301:
									return "DS root must be nc";
								case 8302:
									return "DS and replica inhibited";
								case 8303:
									return "DS att not def in schema";
								case 8304:
									return "DS max obj size exceeded";
								case 8305:
									return "DS obj string name exists";
								case 8306:
									return "DS no rdn defined in schema";
								case 8307:
									return "DS rdn doesn't match schema";
								case 8308:
									return "DS no requested atts found";
								case 8309:
									return "DS user buffer too small";
								case 8310:
									return "DS att is not on obj";
								case 8311:
									return "DS illegal mod operation";
								case 8312:
									return "DS obj too large";
								case 8313:
									return "DS bad instance type";
								case 8314:
									return "DS masterdsa required";
								case 8315:
									return "DS object class required";
								case 8316:
									return "DS missing required att";
								case 8317:
									return "DS att not def for class";
								case 8318:
									return "DS att already exists";
								case 8320:
									return "DS can't add att values";
								case 8321:
									return "DS single value constraint";
								case 8322:
									return "DS range constraint";
								case 8323:
									return "DS att val already exists";
								case 8324:
									return "DS can't rem missing att";
								case 8325:
									return "DS can't rem missing att val";
								case 8326:
									return "DS root can't be subref";
								case 8327:
									return "DS no chaining";
								case 8328:
									return "DS no chained eval";
								case 8329:
									return "DS no parent object";
								case 8330:
									return "DS parent is an alias";
								case 8331:
									return "DS can't mix master and reps";
								case 8332:
									return "DS children exist";
								case 8333:
									return "DS obj not found";
								case 8334:
									return "DS aliased obj missing";
								case 8335:
									return "DS bad name syntax";
								case 8336:
									return "DS alias points to alias";
								case 8337:
									return "DS can't redef alias";
								case 8338:
									return "DS out of scope";
								case 8339:
									return "DS object being removed";
								case 8340:
									return "DS can't delete dsa obj";
								case 8341:
									return "DS generic error";
								case 8342:
									return "DS dsa must be int master";
								case 8343:
									return "DS class not dsa";
								case 8344:
									return "DS insuff access rights";
								case 8345:
									return "DS illegal superior";
								case 8346:
									return "DS attribute owned by sam";
								case 8347:
									return "DS name too many parts";
								case 8348:
									return "DS name too long";
								case 8349:
									return "DS name value too long";
								case 8350:
									return "DS name unparseable";
								case 8351:
									return "DS name type unknown";
								case 8352:
									return "DS not an object";
								case 8353:
									return "DS sec desc too short";
								case 8354:
									return "DS sec desc invalid";
								case 8355:
									return "DS no deleted name";
								case 8356:
									return "DS subref must have parent";
								case 8357:
									return "DS ncname must be nc";
								case 8358:
									return "DS can't add system only";
								case 8359:
									return "DS class must be concrete";
								case 8360:
									return "DS invalid dmd";
								case 8361:
									return "DS obj GUID exists";
								case 8362:
									return "DS not on backlink";
								case 8363:
									return "DS no crossref for nc";
								case 8364:
									return "DS shutting down";
								case 8365:
									return "DS unknown operation";
								case 8366:
									return "DS invalid role owner";
								case 8367:
									return "DS couldn't contact fsmo";
								case 8368:
									return "DS cross nc dn rename";
								case 8369:
									return "DS can't mod system only";
								case 8370:
									return "DS replicator only";
								case 8371:
									return "DS obj class not defined";
								case 8372:
									return "DS obj class not subclass";
								case 8373:
									return "DS name reference invalid";
								case 8374:
									return "DS cross ref exists";
								case 8375:
									return "DS can't del master crossref";
								case 8376:
									return "DS subtree notify not nc head";
								case 8377:
									return "DS notify filter too complex";
								case 8378:
									return "DS dup rdn";
								case 8379:
									return "DS dup oid";
								case 8380:
									return "DS dup mapi ID";
								case 8381:
									return "DS dup schema ID GUID";
								case 8382:
									return "DS dup LDAP display name";
								case 8383:
									return "DS semantic att test";
								case 8384:
									return "DS syntax mismatch";
								case 8385:
									return "DS exists in must have";
								case 8386:
									return "DS exists in may have";
								case 8387:
									return "DS nonexistent may have";
								case 8388:
									return "DS nonexistent must have";
								case 8389:
									return "DS aux cls test fail";
								case 8390:
									return "DS nonexistent poss sup";
								case 8391:
									return "DS sub cls test fail";
								case 8392:
									return "DS bad rdn att ID syntax";
								case 8393:
									return "DS exists in aux cls";
								case 8394:
									return "DS exists in sub cls";
								case 8395:
									return "DS exists in poss sup";
								case 8396:
									return "DS recalcschema failed";
								case 8397:
									return "DS tree delete not finished";
								case 8398:
									return "DS can't delete";
								case 8399:
									return "DS att schema req ID";
								case 8400:
									return "DS bad att schema syntax";
								case 8401:
									return "DS can't cache att";
								case 8402:
									return "DS can't cache class";
								case 8403:
									return "DS can't remove att cache";
								case 8404:
									return "DS can't remove class cache";
								case 8405:
									return "DS can't retrieve DN";
								case 8406:
									return "DS missing supref";
								case 8407:
									return "DS can't retrieve instance";
								case 8408:
									return "DS code inconsistency";
								case 8409:
									return "DS database error";
								case 8410:
									return "DS governsid missing";
								case 8411:
									return "DS missing expected att";
								case 8412:
									return "DS ncname missing cr ref";
								case 8413:
									return "DS security checking error";
								case 8414:
									return "DS schema not loaded";
								case 8415:
									return "DS schema alloc failed";
								case 8416:
									return "DS att schema req syntax";
								case 8417:
									return "DS gcverify error";
								case 8418:
									return "DS dra schema mismatch";
								case 8419:
									return "DS can't find dsa obj";
								case 8420:
									return "DS can't find expected nc";
								case 8421:
									return "DS can't find nc in cache";
								case 8422:
									return "DS can't retrieve child";
								case 8423:
									return "DS security illegal modify";
								case 8424:
									return "DS can't replace hidden rec";
								case 8425:
									return "DS bad hierarchy file";
								case 8426:
									return "DS build hierarchy table failed";
								case 8427:
									return "DS config param missing";
								case 8428:
									return "DS counting ab indices failed";
								case 8429:
									return "DS hierarchy table malloc failed";
								case 8430:
									return "DS internal failure";
								case 8431:
									return "DS unknown error";
								case 8432:
									return "DS root requires class top";
								case 8433:
									return "DS refusing fmso roles";
								case 8434:
									return "DS missing fmso settings";
								case 8435:
									return "DS unable to surrender roles";
								case 8436:
									return "DS dra generic";
								case 8437:
									return "DS dra invalid parameter";
								case 8438:
									return "DS dra busy";
								case 8439:
									return "DS dra bad dn";
								case 8440:
									return "DS dra bad nc";
								case 8441:
									return "DS dra dn exists";
								case 8442:
									return "DS dra internal error";
								case 8443:
									return "DS dra inconsistent dit";
								case 8444:
									return "DS dra connection failed";
								case 8445:
									return "DS dra bad instance type";
								case 8446:
									return "DS dra out of mem";
								case 8447:
									return "DS dra mail problem";
								case 8448:
									return "DS dra ref already exists";
								case 8449:
									return "DS dra ref not found";
								case 8450:
									return "DS dra obj is rep source";
								case 8451:
									return "DS dra db error";
								case 8452:
									return "DS dra no replica";
								case 8453:
									return "DS dra access denied";
								case 8454:
									return "DS dra not supported";
								case 8455:
									return "DS dra RPC cancelled";
								case 8456:
									return "DS dra source disabled";
								case 8457:
									return "DS dra sink disabled";
								case 8458:
									return "DS dra name collision";
								case 8459:
									return "DS dra source reinstalled";
								case 8460:
									return "DS dra missing parent";
								case 8461:
									return "DS dra preempted";
								case 8462:
									return "DS dra abandon sync";
								case 8463:
									return "DS dra shutdown";
								case 8464:
									return "DS dra incompatible partial set";
								case 8465:
									return "DS dra source is partial replica";
								case 8466:
									return "DS dra extn connection failed";
								case 8467:
									return "DS install schema mismatch";
								case 8468:
									return "DS dup link ID";
								case 8469:
									return "DS name error resolving";
								case 8470:
									return "DS name error not found";
								case 8471:
									return "DS name error not unique";
								case 8472:
									return "DS name error no mapping";
								case 8473:
									return "DS name error domain only";
								case 8474:
									return "DS name error no syntactical mapping";
								case 8475:
									return "DS constructed att mod";
								case 8476:
									return "DS wrong om obj class";
								case 8477:
									return "DS dra repl pending";
								case 8478:
									return "DS ds required";
								case 8479:
									return "DS invalid LDAP display name";
								case 8480:
									return "DS non base search";
								case 8481:
									return "DS can't retrieve atts";
								case 8482:
									return "DS backlink without link";
								case 8483:
									return "DS epoch mismatch";
								case 8484:
									return "DS src name mismatch";
								case 8485:
									return "DS src and dst nc identical";
								case 8486:
									return "DS dst nc mismatch";
								case 8487:
									return "DS not authoritive for dst nc";
								case 8488:
									return "DS src GUID mismatch";
								case 8489:
									return "DS can't move deleted object";
								case 8490:
									return "DS pdc operation in progress";
								case 8491:
									return "DS cross domain cleanup reqd";
								case 8492:
									return "DS illegal xdom move operation";
								case 8493:
									return "DS can't with acct group membershps";
								case 8494:
									return "DS nc must have nc parent";
								case 8496:
									return "DS dst domain not native";
								case 8497:
									return "DS missing infrastructure container";
								case 8498:
									return "DS can't move account group";
								case 8499:
									return "DS can't move resource group";
								case 8500:
									return "DS invalid search flag";
								case 8501:
									return "DS no tree delete above nc";
								case 8502:
									return "DS couldn't lock tree for delete";
								case 8503:
									return "DS couldn't identify objects for tree delete";
								case 8504:
									return "DS sam init failure";
								case 8505:
									return "DS sensitive group violation";
								case 8506:
									return "DS can't mod primarygroupid";
								case 8507:
									return "DS illegal base schema mod";
								case 8508:
									return "DS nonsafe schema change";
								case 8509:
									return "DS schema update disallowed";
								case 8510:
									return "DS can't create under schema";
								case 8511:
									return "DS install no src sch version";
								case 8512:
									return "DS install no sch version in inifile";
								case 8513:
									return "DS invalid group type";
								case 8514:
									return "DS no nest globalgroup in mixeddomain";
								case 8515:
									return "DS no nest localgroup in mixeddomain";
								case 8516:
									return "DS global can't have local member";
								case 8517:
									return "DS global can't have universal member";
								case 8518:
									return "DS universal can't have local member";
								case 8519:
									return "DS global can't have crossdomain member";
								case 8520:
									return "DS local can't have crossdomain local member";
								case 8521:
									return "DS have primary members";
								case 8522:
									return "DS string sd conversion failed";
								case 8523:
									return "DS naming master gc";
								case 8524:
									return "DS lookup failure";
								case 8525:
									return "DS couldn't update spns";
								case 8526:
									return "DS can't retrieve sd";
								case 8527:
									return "DS key not unique";
								case 8528:
									return "DS wrong linked att syntax";
								case 8529:
									return "DS sam need bootkey password";
								case 8530:
									return "DS sam need bootkey floppy";
								case 8531:
									return "DS can't start";
								case 8532:
									return "DS init failure";
								case 8533:
									return "DS no pkt privacy on connection";
								case 8534:
									return "DS source domain in forest";
								case 8535:
									return "DS destination domain not in forest";
								case 8536:
									return "DS destination auditing not enabled";
								case 8537:
									return "DS can't find dc for src domain";
								case 8538:
									return "DS src obj not group or user";
								case 8539:
									return "DS src sid exists in forest";
								case 8540:
									return "DS src and dst object class mismatch";
								case 8541:
									return "Sam init failure";
								case 8542:
									return "DS dra schema info ship";
								case 8543:
									return "DS dra schema conflict";
								case 8544:
									return "DS dra earlier schema conflict";
								case 8545:
									return "DS dra obj nc mismatch";
								case 8546:
									return "DS nc still has dsas";
								case 8547:
									return "DS gc required";
								case 8548:
									return "DS local member of local only";
								case 8549:
									return "DS no fpo in universal groups";
								case 8550:
									return "DS can't add to gc";
								case 8551:
									return "DS no checkpoint with pdc";
								case 8552:
									return "DS source auditing not enabled";
								case 8553:
									return "DS can't create in nondomain nc";
								case 8554:
									return "DS invalid name for spn";
								case 8555:
									return "DS filter uses constructed attrs";
								case 8556:
									return "DS unicodepwd not in quotes";
								case 8557:
									return "DS machine account quota exceeded";
								case 8558:
									return "DS must be run on dst dc";
								case 8559:
									return "DS src dc must be sp4 or greater";
								case 8560:
									return "DS can't tree delete critical obj";
								case 8561:
									return "DS init failure console";
								case 8562:
									return "DS sam init failure console";
								case 8563:
									return "DS forest version too high";
								case 8564:
									return "DS domain version too high";
								case 8565:
									return "DS forest version too low";
								case 8566:
									return "DS domain version too low";
								case 8567:
									return "DS incompatible version";
								case 8568:
									return "DS low dsa version";
								case 8569:
									return "DS no behaviour version in mixeddomain";
								case 8570:
									return "DS not supported sort order";
								case 8571:
									return "DS name not unique";
								case 8572:
									return "DS machine account created prent4";
								case 8573:
									return "DS out of version store";
								case 8574:
									return "DS incompatible controls used";
								case 8575:
									return "DS no ref domain";
								case 8576:
									return "DS reserved link ID";
								case 8577:
									return "DS link ID not available";
								case 8578:
									return "DS ag can't have universal member";
								case 8579:
									return "DS modifydn disallowed by instance type";
								case 8580:
									return "DS no object move in schema nc";
								case 8581:
									return "DS modifydn disallowed by flag";
								case 8582:
									return "DS modifydn wrong grandparent";
								case 8583:
									return "DS name error trust referral";
								case 8584:
									return "Not supported on standard server";
								case 8585:
									return "DS can't access remote part of ad";
								case 8586:
									return "DS cr impossible to validate";
								case 8587:
									return "DS thread limit exceeded";
								case 8588:
									return "DS not closest";
								case 8589:
									return "DS can't derive spn without server ref";
								case 8590:
									return "DS single user mode failed";
								case 8591:
									return "DS ntdscript syntax error";
								case 8592:
									return "DS ntdscript process error";
								case 8593:
									return "DS different repl epochs";
								case 8594:
									return "DS drs extensions changed";
								case 8595:
									return "DS replica set change not allowed on disabled cr";
								case 8596:
									return "DS no msds intid";
								case 8597:
									return "DS dup msds intid";
								case 8598:
									return "DS exists in rdnattid";
								case 8599:
									return "DS authorisation failed";
								case 8600:
									return "DS invalid script";
								case 8601:
									return "DS remote crossref op failed";
								}
								break;
							}
						}
						else
						{
							switch (error)
							{
							case 9001:
								return "DNS error rcode format error";
							case 9002:
								return "DNS error rcode server failure";
							case 9003:
								return "DNS error rcode name error";
							case 9004:
								return "DNS error rcode not implemented";
							case 9005:
								return "DNS error rcode refused";
							case 9006:
								return "DNS error rcode yxdomain";
							case 9007:
								return "DNS error rcode yxrrset";
							case 9008:
								return "DNS error rcode nxrrset";
							case 9009:
								return "DNS error rcode notauth";
							case 9010:
								return "DNS error rcode notzone";
							case 9011:
							case 9012:
							case 9013:
							case 9014:
							case 9015:
								break;
							case 9016:
								return "DNS error rcode badsig";
							case 9017:
								return "DNS error rcode badkey";
							case 9018:
								return "DNS error rcode badtime";
							default:
								switch (error)
								{
								case 9501:
									return "DNS info no records";
								case 9502:
									return "DNS error bad packet";
								case 9503:
									return "DNS error no packet";
								case 9504:
									return "DNS error rcode";
								case 9505:
									return "DNS error unsecure packet";
								}
								break;
							}
						}
					}
					else if (error <= 9621)
					{
						switch (error)
						{
						case 9551:
							return "DNS error invalid type";
						case 9552:
							return "DNS error invalid IP address";
						case 9553:
							return "DNS error invalid property";
						case 9554:
							return "DNS error try again later";
						case 9555:
							return "DNS error not unique";
						case 9556:
							return "DNS error non RFC name";
						case 9557:
							return "DNS status FQDN";
						case 9558:
							return "DNS status dotted name";
						case 9559:
							return "DNS status single part name";
						case 9560:
							return "DNS error invalid name char";
						case 9561:
							return "DNS error numeric name";
						case 9562:
							return "DNS error not allowed on root server";
						default:
							switch (error)
							{
							case 9601:
								return "DNS error zone does not exist";
							case 9602:
								return "DNS error not zone info";
							case 9603:
								return "DNS error invalid zone operation";
							case 9604:
								return "DNS error zone configuration error";
							case 9605:
								return "DNS error zone has not SOA record";
							case 9606:
								return "DNS error zone has no NS records";
							case 9607:
								return "DNS error zone locked";
							case 9608:
								return "DNS error zone creation failed";
							case 9609:
								return "DNS error zone already exists";
							case 9610:
								return "DNS error autozone already exists";
							case 9611:
								return "DNS error invalid zone type";
							case 9612:
								return "DNS error secondary requires master IP";
							case 9613:
								return "DNS error zone not secondary";
							case 9614:
								return "DNS error need secondary addresses";
							case 9615:
								return "DNS error wins init failed";
							case 9616:
								return "DNS error need wins servers";
							case 9617:
								return "DNS error nbstat init failed";
							case 9618:
								return "DNS error SOA delete invalid";
							case 9619:
								return "DNS error forwarder already exists";
							case 9620:
								return "DNS error zone requires master IP";
							case 9621:
								return "DNS error zone is shutdown";
							}
							break;
						}
					}
					else
					{
						switch (error)
						{
						case 9651:
							return "DNS error primary requires datafile";
						case 9652:
							return "DNS error invalid datafile name";
						case 9653:
							return "DNS error datafile open failure";
						case 9654:
							return "DNS error file writeback failed";
						case 9655:
							return "DNS error datafile parsing";
						default:
							switch (error)
							{
							case 9701:
								return "DNS error record does not exist";
							case 9702:
								return "DNS error record format";
							case 9703:
								return "DNS error node creation failed";
							case 9704:
								return "DNS error unknown record type";
							case 9705:
								return "DNS error record timed out";
							case 9706:
								return "DNS error name not in zone";
							case 9707:
								return "DNS error CNAME loop";
							case 9708:
								return "DNS error node is CNAME";
							case 9709:
								return "DNS error CNAME collision";
							case 9710:
								return "DNS error record only at zone root";
							case 9711:
								return "DNS error record already exists";
							case 9712:
								return "DNS error secondary data";
							case 9713:
								return "DNS error no create cache data";
							case 9714:
								return "DNS error name does not exist";
							case 9715:
								return "DNS warning PTR create failed";
							case 9716:
								return "DNS warning domain undeleted";
							case 9717:
								return "DNS error ds unavailable";
							case 9718:
								return "DNS error ds zone already exists";
							case 9719:
								return "DNS error no bootfile if ds zone";
							default:
								switch (error)
								{
								case 9751:
									return "DNS info AXFR complete";
								case 9752:
									return "DNS error AXFR";
								case 9753:
									return "DNS info added local wins";
								}
								break;
							}
							break;
						}
					}
				}
				else if (error <= 9904)
				{
					if (error <= 9851)
					{
						if (error == 9801)
						{
							return "DNS status continue needed";
						}
						if (error == 9851)
						{
							return "DNS error no TCPIP";
						}
					}
					else
					{
						if (error == 9852)
						{
							return "DNS error no DNS servers";
						}
						switch (error)
						{
						case 9901:
							return "DNS error dp does not exist";
						case 9902:
							return "DNS error dp already exists";
						case 9903:
							return "DNS error dp not enlisted";
						case 9904:
							return "DNS error dp already enlisted";
						}
					}
				}
				else if (error <= 11031)
				{
					switch (error)
					{
					case 10004:
						return "interrupted";
					case 10005:
					case 10006:
					case 10007:
					case 10008:
					case 10010:
					case 10011:
					case 10012:
					case 10015:
					case 10016:
					case 10017:
					case 10018:
					case 10019:
					case 10020:
					case 10021:
					case 10023:
					case 10025:
					case 10026:
					case 10027:
					case 10028:
					case 10029:
					case 10030:
					case 10031:
					case 10032:
					case 10033:
					case 10034:
					case 10072:
					case 10073:
					case 10074:
					case 10075:
					case 10076:
					case 10077:
					case 10078:
					case 10079:
					case 10080:
					case 10081:
					case 10082:
					case 10083:
					case 10084:
					case 10085:
					case 10086:
					case 10087:
					case 10088:
					case 10089:
					case 10090:
					case 10094:
					case 10095:
					case 10096:
					case 10097:
					case 10098:
					case 10099:
					case 10100:
						break;
					case 10009:
						return "Bad file number";
					case 10013:
						return "Access denied";
					case 10014:
						return "Bad address";
					case 10022:
						return "Invalid arguments";
					case 10024:
						return "Too many open files";
					case 10035:
						return "Operation on non-blocking socket would block";
					case 10036:
						return "Operation in progress";
					case 10037:
						return "Operation already in progress";
					case 10038:
						return "The descriptor is not a socket";
					case 10039:
						return "Destination address required";
					case 10040:
						return "Message too long";
					case 10041:
						return "Protocol wrong type for socket";
					case 10042:
						return "Protocol option not supported";
					case 10043:
						return "Protocol not supported";
					case 10044:
						return "Socket not supported";
					case 10045:
						return "Operation not supported";
					case 10046:
						return "Protocol family not supported";
					case 10047:
						return "An address incompatible with the requested protocol was used";
					case 10048:
						return "Address already in use";
					case 10049:
						return "The requested address is not valid in this context";
					case 10050:
						return "Network subsystem is down";
					case 10051:
						return "Network is unreachable";
					case 10052:
						return "Connection broken, keep-alive detected a problem";
					case 10053:
						return "An established connection was aborted in your host machine.";
					case 10054:
						return "Connection reset by peer";
					case 10055:
						return "Not enough buffer space is available";
					case 10056:
						return "Socket is already connected";
					case 10057:
						return "The socket is not connected";
					case 10058:
						return "The socket has been shut down";
					case 10059:
						return "Too many references: cannot splice";
					case 10060:
						return "Connection timed out";
					case 10061:
						return "Connection refused";
					case 10062:
						return "Too many symbolic links encountered";
					case 10063:
						return "File name too long";
					case 10064:
						return "Host is down";
					case 10065:
						return "No route to host";
					case 10066:
						return "Directory not empty";
					case 10067:
						return "EPROCLIM";
					case 10068:
						return "Too many users";
					case 10069:
						return "Quota exceeded";
					case 10070:
						return "Stale NFS file handle";
					case 10071:
						return "Object is remote";
					case 10091:
						return "SYSNOTREADY";
					case 10092:
						return "VERNOTSUPPORTED";
					case 10093:
						return "Winsock not initialised";
					case 10101:
						return "EDISCON";
					case 10102:
						return "ENOMORE";
					case 10103:
						return "Operation canceled";
					case 10104:
						return "EINVALIDPROCTABLE";
					case 10105:
						return "EINVALIDPROVIDER";
					case 10106:
						return "EPROVIDERFAILEDINIT";
					case 10107:
						return "System call failed";
					case 10108:
						return "SERVICE_NOT_FOUND";
					case 10109:
						return "TYPE_NOT_FOUND";
					case 10110:
						return "E_NO_MORE";
					case 10111:
						return "E_CANCELLED";
					case 10112:
						return "EREFUSED";
					default:
						switch (error)
						{
						case 11001:
							return "No such host is known";
						case 11002:
							return "A temporary error occurred on an authoritative name server.  Try again later.";
						case 11003:
							return "No recovery";
						case 11004:
							return "No data";
						case 11005:
							return "QOS receivers";
						case 11006:
							return "QOS senders";
						case 11007:
							return "QOS no senders";
						case 11008:
							return "QOS no receivers";
						case 11009:
							return "QOS request confirmed";
						case 11010:
							return "QOS admission failure";
						case 11011:
							return "QOS policy failure";
						case 11012:
							return "QOS bad style";
						case 11013:
							return "QOS bad object";
						case 11014:
							return "QOS traffic ctrl error";
						case 11015:
							return "QOS generic error";
						case 11016:
							return "QOS eservicetype";
						case 11017:
							return "QOS eflowspec";
						case 11018:
							return "QOS eprovspecbuf";
						case 11019:
							return "QOS efilterstyle";
						case 11020:
							return "QOS efiltertype";
						case 11021:
							return "QOS efiltercount";
						case 11022:
							return "QOS eobjlength";
						case 11023:
							return "QOS eflowcount";
						case 11024:
							return "QOS eunknownpsobj";
						case 11025:
							return "QOS epolicyobj";
						case 11026:
							return "QOS eflowdesc";
						case 11027:
							return "QOS epsflowspec";
						case 11028:
							return "QOS epsfilterspec";
						case 11029:
							return "QOS esdmodeobj";
						case 11030:
							return "QOS eshaperateobj";
						case 11031:
							return "QOS reserved petype";
						}
						break;
					}
				}
				else
				{
					switch (error)
					{
					case 13000:
						return "IPSEC qm policy exists";
					case 13001:
						return "IPSEC qm policy not found";
					case 13002:
						return "IPSEC qm policy in use";
					case 13003:
						return "IPSEC mm policy exists";
					case 13004:
						return "IPSEC mm policy not found";
					case 13005:
						return "IPSEC mm policy in use";
					case 13006:
						return "IPSEC mm filter exists";
					case 13007:
						return "IPSEC mm filter not found";
					case 13008:
						return "IPSEC transport filter exists";
					case 13009:
						return "IPSEC transport filter not found";
					case 13010:
						return "IPSEC mm auth exists";
					case 13011:
						return "IPSEC mm auth not found";
					case 13012:
						return "IPSEC mm auth in use";
					case 13013:
						return "IPSEC default mm policy not found";
					case 13014:
						return "IPSEC default mm auth not found";
					case 13015:
						return "IPSEC default qm policy not found";
					case 13016:
						return "IPSEC tunnel filter exists";
					case 13017:
						return "IPSEC tunnel filter not found";
					case 13018:
						return "IPSEC mm filter pending deletion";
					case 13019:
						return "IPSEC transport filter pending deletion";
					case 13020:
						return "IPSEC tunnel filter pending deletion";
					case 13021:
						return "IPSEC mm policy pending deletion";
					case 13022:
						return "IPSEC mm auth pending deletion";
					case 13023:
						return "IPSEC qm policy pending deletion";
					default:
						switch (error)
						{
						case 13801:
							return "IPSEC IKE auth fail";
						case 13802:
							return "IPSEC IKE attrib fail";
						case 13803:
							return "IPSEC IKE negotiation pending";
						case 13804:
							return "IPSEC IKE general processing error";
						case 13805:
							return "IPSEC IKE timed out";
						case 13806:
							return "IPSEC IKE no cert";
						case 13807:
							return "IPSEC IKE sa deleted";
						case 13808:
							return "IPSEC IKE sa reaped";
						case 13809:
							return "IPSEC IKE mm acquire drop";
						case 13810:
							return "IPSEC IKE qm acquire drop";
						case 13811:
							return "IPSEC IKE queue drop mm";
						case 13812:
							return "IPSEC IKE queue drop no mm";
						case 13813:
							return "IPSEC IKE drop no response";
						case 13814:
							return "IPSEC IKE mm delay drop";
						case 13815:
							return "IPSEC IKE qm delay drop";
						case 13816:
							return "IPSEC IKE error";
						case 13817:
							return "IPSEC IKE crl failed";
						case 13818:
							return "IPSEC IKE invalid key usage";
						case 13819:
							return "IPSEC IKE invalid cert type";
						case 13820:
							return "IPSEC IKE no private key";
						case 13821:
						case 13823:
						case 13880:
							break;
						case 13822:
							return "IPSEC IKE dh fail";
						case 13824:
							return "IPSEC IKE invalid header";
						case 13825:
							return "IPSEC IKE no policy";
						case 13826:
							return "IPSEC IKE invalid signature";
						case 13827:
							return "IPSEC IKE kerberos error";
						case 13828:
							return "IPSEC IKE no public key";
						case 13829:
							return "IPSEC IKE process err";
						case 13830:
							return "IPSEC IKE process err sa";
						case 13831:
							return "IPSEC IKE process err prop";
						case 13832:
							return "IPSEC IKE process err trans";
						case 13833:
							return "IPSEC IKE process err ke";
						case 13834:
							return "IPSEC IKE process err ID";
						case 13835:
							return "IPSEC IKE process err cert";
						case 13836:
							return "IPSEC IKE process err cert req";
						case 13837:
							return "IPSEC IKE process err hash";
						case 13838:
							return "IPSEC IKE process err sig";
						case 13839:
							return "IPSEC IKE process err nonce";
						case 13840:
							return "IPSEC IKE process err notify";
						case 13841:
							return "IPSEC IKE process err delete";
						case 13842:
							return "IPSEC IKE process err vendor";
						case 13843:
							return "IPSEC IKE invalid payload";
						case 13844:
							return "IPSEC IKE load soft sa";
						case 13845:
							return "IPSEC IKE soft sa torn down";
						case 13846:
							return "IPSEC IKE invalid cookie";
						case 13847:
							return "IPSEC IKE no peer cert";
						case 13848:
							return "IPSEC IKE peer CRL failed";
						case 13849:
							return "IPSEC IKE policy change";
						case 13850:
							return "IPSEC IKE no mm policy";
						case 13851:
							return "IPSEC IKE notcbpriv";
						case 13852:
							return "IPSEC IKE secloadfail";
						case 13853:
							return "IPSEC IKE failsspinit";
						case 13854:
							return "IPSEC IKE failqueryssp";
						case 13855:
							return "IPSEC IKE srvacqfail";
						case 13856:
							return "IPSEC IKE srvquerycred";
						case 13857:
							return "IPSEC IKE getspifail";
						case 13858:
							return "IPSEC IKE invalid filter";
						case 13859:
							return "IPSEC IKE out of memory";
						case 13860:
							return "IPSEC IKE add update key failed";
						case 13861:
							return "IPSEC IKE invalid policy";
						case 13862:
							return "IPSEC IKE unknown doi";
						case 13863:
							return "IPSEC IKE invalid situation";
						case 13864:
							return "IPSEC IKE dh failure";
						case 13865:
							return "IPSEC IKE invalid group";
						case 13866:
							return "IPSEC IKE encrypt";
						case 13867:
							return "IPSEC IKE decrypt";
						case 13868:
							return "IPSEC IKE policy match";
						case 13869:
							return "IPSEC IKE unsupported ID";
						case 13870:
							return "IPSEC IKE invalid hash";
						case 13871:
							return "IPSEC IKE invalid hash alg";
						case 13872:
							return "IPSEC IKE invalid hash size";
						case 13873:
							return "IPSEC IKE invalid encrypt alg";
						case 13874:
							return "IPSEC IKE invalid auth alg";
						case 13875:
							return "IPSEC IKE invalid sig";
						case 13876:
							return "IPSEC IKE load failed";
						case 13877:
							return "IPSEC IKE rpc delete";
						case 13878:
							return "IPSEC IKE benign reinit";
						case 13879:
							return "IPSEC IKE invalid responder lifetime notify";
						case 13881:
							return "IPSEC IKE invalid cert keylen";
						case 13882:
							return "IPSEC IKE mm limit";
						case 13883:
							return "IPSEC IKE negotiation disabled";
						case 13884:
							return "IPSEC IKE neg status end";
						default:
							if (error == 100001)
							{
								return "Device not configured";
							}
							break;
						}
						break;
					}
				}
				return string.Format("mono-io-layer-error ({0})", error);
			}
			char[] array = new char[256];
			if (Win32Exception.FormatMessage(4608, IntPtr.Zero, (uint)error, 0, array, 256, null) == 0)
			{
				return "Error looking up error string";
			}
			return new string(array);
		}

		private readonly int nativeErrorCode;

		private const int MAX_MESSAGE_LENGTH = 256;
	}
}
