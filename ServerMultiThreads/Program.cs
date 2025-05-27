using System.Net;
using System.Net.Sockets;

namespace ServerAsync;
internal static class ServerAsync {
    public static void Main() {
        TcpListener? server = null;
        try {
            const int port = 5000;
            var localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);
            server.Start();

            while(true) {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                Thread thread1 = new Thread(Listen);
                Random rnd = new Random();
                thread1.Start(client, rnd.Next(1, 1000));
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
    
    static void Listen(object? param) {
        if (param is not TcpClient client) return;
        var bytes = new byte[256];
        NetworkStream stream = client.GetStream();
        int i;
        while((i = stream.Read(bytes, 0, bytes.Length))!=0) {
            var data = client + " : " + System.Text.Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("Received: {0}", data);
            data = data.ToUpper();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", data);
        }
    }
    
}




    




