using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace ServerChatLogin;
internal static class ServerChat {
    static ConcurrentDictionary<TcpClient, string> clients = new();

    private static Dictionary<string, string> users = new() {
        { "admin", "admin123" },
        {"user1", "password1" },
        { "user2", "password2" }
    };
    
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
                Thread authThread = new Thread(() => Authenticate(client));
                authThread.Start();
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

    static void Authenticate(TcpClient client) {
        var stream = client.GetStream();
        var writer = new StreamWriter(stream) { AutoFlush = true };
        var reader = new StreamReader(stream);

        writer.WriteLine("Welcome! Type 'register' or 'login':");
        string? action = reader.ReadLine();

        string? username = null;
        bool authenticated = false;

        if (action == "register") {
            writer.WriteLine("Enter username:");
            username = reader.ReadLine();
            writer.WriteLine("Enter password:");
            string? password = reader.ReadLine();

            lock (users) {
                if (username != null && !users.ContainsKey(username)) {
                    users[username] = password ?? "";
                    writer.WriteLine("Registration successful. You are now logged in.");
                    authenticated = true;
                } else {
                    writer.WriteLine("Username already exists.");
                }
            }
        } 
        else if (action == "login") {
            writer.WriteLine("Enter username:");
            username = reader.ReadLine();
            writer.WriteLine("Enter password:");
            string? password = reader.ReadLine();

            lock (users) {
                if (username != null && users.TryGetValue(username, out var storedPw) && storedPw == password) {
                    writer.WriteLine("Login successful.");
                    authenticated = true;
                } else {
                    writer.WriteLine("Invalid credentials.");
                }
            }
        } 
        else {
            writer.WriteLine("Unknown command.");
        }

        if (authenticated && username != null) {
            if (clients.TryAdd(client, username)) {
                writer.WriteLine($"Welcome to the chat, {username}!");
                Thread thread1 = new Thread(() => Listen(client, username));
                thread1.Start();
                Console.WriteLine("{0} has joined the chat.", username);
            }
        } 
        else {
            writer.WriteLine("Authentication failed.");
            client.Close();
        }
    }

    static void Listen(TcpClient client, string name) {
    var bytes = new byte[256];
    NetworkStream stream = client.GetStream();

    try {
        int i;
        while((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
            var data = System.Text.Encoding.UTF8.GetString(bytes, 0, i);
            var message = $"{name}: {data}";
            Console.WriteLine("Received: {0}", message);
            Broadcast(message, client);
        }
    } catch (Exception ex) {
        Console.WriteLine($"Error with client {name}: {ex.Message}");
    } finally {
        // Remove client and close connection
        clients.TryRemove(client, out _);
        client.Close();
        Console.WriteLine($"{name} has disconnected.");
        Broadcast($"{name} has left the chat.", null);
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
            } 
            catch{}
        }
    }
}