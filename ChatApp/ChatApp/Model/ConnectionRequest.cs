namespace ChatApp.Model
{
    public class ConnectionRequest
    {
        public string MessageType { get; set; } = "connectRequest";
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
    }

    public class ConnectionAccept
    {
        public string MessageType { get; set; } 
        public string Name { get; set; }       
    }
}