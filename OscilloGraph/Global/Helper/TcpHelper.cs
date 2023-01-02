using System.Net.Sockets;
using System.Net;

namespace OscilloGraph.Global.Helper
{
    internal class TcpHelper
    {
        internal static int GetAvailablePort() => GetAvailablePort(IPAddress.Loopback);

        internal static int GetAvailablePort(IPAddress ip)
        {
            var listener = new TcpListener(ip, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
