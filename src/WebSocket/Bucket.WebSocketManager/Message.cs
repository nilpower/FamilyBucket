﻿namespace Bucket.WebSocketManager
{
    public enum MessageType
    {
        Text,
        ConnectionEvent,
        Error
    }

    public class Message
    {
        public MessageType MessageType { get; set; }
        public string Data { get; set; }
    }
}
