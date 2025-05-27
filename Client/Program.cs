using System.Net.Sockets;


namespace Client;
internal static class Client {

    public static void Main() {
        Connect("127.0.0.1");
    }

    static void Connect(String server) {
        try {
            const int port = 5000;
            using TcpClient client = new TcpClient(server, port);
            Console.Write("Connected to server. Type your messages below (type 'exit' to quit):\n");
            while (true) {
                string message = Console.ReadLine() ?? string.Empty;
                if (message.ToLower() == "exit")
                {
                    Console.WriteLine("Exiting...");
                    break;
                }
                var thread1 = new Thread(() => SendMessage(client, message));
                var thread2 = new Thread(() => ReceiveMessage(client));
                thread1.Start();
                thread2.Start();
            }
        }
        
        catch (ArgumentNullException e) {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        
        catch (SocketException e) {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }

    private static void SendMessage(TcpClient client, string message) {
        var stream = client.GetStream();
        var data = System.Text.Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
        Console.WriteLine("Sent: {0}", message);
    }

    private static void ReceiveMessage(TcpClient client) {
        var stream = client.GetStream();
        var data = new byte[256];
        while (true) {
            int bytes;
            try {
                bytes = stream.Read(data, 0, data.Length);
                if (bytes == 0) break; // Connection closed
                var responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine("\n{0}", responseData);
            } catch {
                break;
            }
        }
    }
}