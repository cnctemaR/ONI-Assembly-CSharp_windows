using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.SqlClient.SNI
{
	internal class SSRP
	{
		internal static int GetPortByInstanceName(string browserHostName, string instanceName)
		{
			byte[] array = SSRP.CreateInstanceInfoRequest(instanceName);
			byte[] array2 = null;
			try
			{
				array2 = SSRP.SendUDPRequest(browserHostName, 1434, array);
			}
			catch (SocketException ex)
			{
				throw new Exception(SQLMessage.SqlServerBrowserNotAccessible(), ex);
			}
			if (array2 == null || array2.Length <= 3 || array2[0] != 5 || (int)BitConverter.ToUInt16(array2, 1) != array2.Length - 3)
			{
				throw new SocketException();
			}
			string[] array3 = Encoding.ASCII.GetString(array2, 3, array2.Length - 3).Split(new char[] { ';' });
			int num = Array.IndexOf<string>(array3, "tcp");
			if (num < 0 || num == array3.Length - 1)
			{
				throw new SocketException();
			}
			return (int)ushort.Parse(array3[num + 1]);
		}

		private static byte[] CreateInstanceInfoRequest(string instanceName)
		{
			instanceName += "\0";
			byte[] array = new byte[Encoding.ASCII.GetByteCount(instanceName) + 1];
			array[0] = 4;
			Encoding.ASCII.GetBytes(instanceName, 0, instanceName.Length, array, 1);
			return array;
		}

		internal static int GetDacPortByInstanceName(string browserHostName, string instanceName)
		{
			byte[] array = SSRP.CreateDacPortInfoRequest(instanceName);
			byte[] array2 = SSRP.SendUDPRequest(browserHostName, 1434, array);
			if (array2 == null || array2.Length <= 4 || array2[0] != 5 || BitConverter.ToUInt16(array2, 1) != 6 || array2[3] != 1)
			{
				throw new SocketException();
			}
			return (int)BitConverter.ToUInt16(array2, 4);
		}

		private static byte[] CreateDacPortInfoRequest(string instanceName)
		{
			instanceName += "\0";
			byte[] array = new byte[Encoding.ASCII.GetByteCount(instanceName) + 2];
			array[0] = 15;
			array[1] = 1;
			Encoding.ASCII.GetBytes(instanceName, 0, instanceName.Length, array, 2);
			return array;
		}

		private static byte[] SendUDPRequest(string browserHostname, int port, byte[] requestPacket)
		{
			IPAddress ipaddress = null;
			bool flag = IPAddress.TryParse(browserHostname, out ipaddress);
			byte[] array = null;
			using (UdpClient udpClient = new UdpClient((!flag) ? AddressFamily.InterNetwork : ipaddress.AddressFamily))
			{
				Task<UdpReceiveResult> task;
				if (udpClient.SendAsync(requestPacket, requestPacket.Length, browserHostname, port).Wait(1000) && (task = udpClient.ReceiveAsync()).Wait(1000))
				{
					array = task.Result.Buffer;
				}
			}
			return array;
		}

		private const char SemicolonSeparator = ';';

		private const int SqlServerBrowserPort = 1434;
	}
}
