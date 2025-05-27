using System.Net;
using System.Net.Sockets;




namespace Server;
internal static class Server {
    public static void Main() {
        TcpListener? server = null;
        try {
            const int port = 5000;
            var localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);

            server.Start();

            var bytes = new byte[256];

            while(true) {
                Console.Write("Waiting for a connection... ");
                using TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                NetworkStream stream = client.GetStream();
                int i;
                while((i = stream.Read(bytes, 0, bytes.Length))!=0) {
                    var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);
                    data = Console.ReadLine() ?? string.Empty;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }
            }
        }
        catch(SocketException e) {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally {
            server?.Stop();
        }
        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }
}