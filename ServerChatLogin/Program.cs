using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace ServerChatLogin;
internal static class ServerChat {
    static ConcurrentDictionary<TcpClient, string> clients = new();
    
    public static void Main() {
        TcpListener? server = null;
        var rand = new Random();
        
        try {
            const int port = 5000;
            var localAddr = IPAddress.Parse("0.0.0.0");
            server = new TcpListener(localAddr, port);
            server.Start();

            while(true) {
                Console.Write("Waiting for a connection... ");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                string name = "User" + rand.Next(1000, 9999);
                if (clients.TryAdd(client, name)) {
                    Thread thread1 = new Thread(() => Listen(client, name));
                    thread1.Start();
                    Console.WriteLine("{0} has joined the chat.", name);
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
    
    static void Listen(TcpClient client, string name) {
        var bytes = new byte[256];
        NetworkStream stream = client.GetStream();
        
        int i;
        while((i = stream.Read(bytes, 0, bytes.Length))!=0) {
            var data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            var message = $"{name}: {data}";
            Console.WriteLine("Received: {0}", message);
            Broadcast(message, client);
        }
    }
    
    static void Broadcast(string message, TcpClient? sender) {
        byte[] msg = System.Text.Encoding.UTF8.GetBytes(message);
        foreach (var client in clients.Keys) {
            if (sender != null && client == sender) continue;
            try {
                if (client.Connected) {
                    NetworkStream stream = client.GetStream();
                    stream.Write(msg, 0, msg.Length);
                }
            } catch {}
        }
    }
    }


internal static class Login {

    static void login() {
        
    }
    
}



internal static class Register {
    
    
    
    
}