using System.Net.Sockets;
using System.IO;

namespace Client;
internal static class Client
{
    public static void Main() {
        Connect("127.0.0.1");
    }

    static void Connect(string server) {
        try {
            const int port = 5000;
            using TcpClient client = new TcpClient(server, port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream) { AutoFlush = true };
            var reader = new StreamReader(stream);

            while (true) {
                string? serverMsg = reader.ReadLine();
                if (serverMsg == null) break;
                Console.WriteLine(serverMsg);

                if (serverMsg.Contains("register") || serverMsg.Contains("login") || serverMsg.Contains("Enter username") || serverMsg.Contains("Enter password")) {
                    string input = Console.ReadLine() ?? "";
                    writer.WriteLine(input);
                }
                if (serverMsg.Contains("Welcome to the chat") || serverMsg.Contains("Registration successful")) {
                    break;
                }
                if (serverMsg.Contains("Authentication failed")) {
                    Main();
                }
            }
            var receiveThread = new Thread(() => ReceiveMessages(reader));
            receiveThread.Start();

            while (true) {
                string message = Console.ReadLine() ?? "";
                if (message.ToLower() == "exit") {
                    Console.WriteLine("Exiting...");
                    break;
                }
                writer.WriteLine(message);
            }
        }
        catch (Exception e) {
            Console.WriteLine("Exception: {0}", e);
        }
        Console.WriteLine("\nPress Enter to continue...");
        Console.Read();
    }

    private static void ReceiveMessages(StreamReader reader) {
        try {
            string? line;
            while ((line = reader.ReadLine()) != null) {
                Console.WriteLine(line);
            }
        }
        catch { }
    }
}