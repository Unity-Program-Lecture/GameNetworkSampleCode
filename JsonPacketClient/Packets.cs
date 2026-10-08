namespace JsonPacket
{
    public enum PacketType : ushort
    {
        Unknown = 0,
        
        Chat = 1,
        System = 2
    }

    public abstract class GamePacket
    {
        internal abstract PacketType Type { get; }
    }
}

namespace JsonPacket
{
    public class ChatPacket : GamePacket
    {
        internal override PacketType Type => PacketType.Chat;

        public string Message { get; set; }
    }

    public class SystemPacket : GamePacket
    {
        internal override PacketType Type => PacketType.System;

        public string Command { get; set; }
    }
}

namespace JsonPacket
{
    using System.Buffers.Binary;
    using System.Net.Sockets;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class PacketJsonConverter
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public static string Serialize<TGamePacket>(this TGamePacket packet)
            where TGamePacket : GamePacket
        {
            return JsonSerializer.Serialize(packet, JsonOptions);
        }

        public static TGamePacket? Deserialize<TGamePacket>(string json)
            where TGamePacket : GamePacket
        {
            TGamePacket? packet = JsonSerializer.Deserialize<TGamePacket>(json, JsonOptions);
            if (packet is null)
            {
                throw new InvalidOperationException("Failed to deserialize the packet.");
            }

            return packet;
        }
    }

    public class RecievedPacket
    {
        public PacketType Type { get; set; }
        public string? Json { get; set; }
    }

    public static class PacketTransfer
    {
        private const int HeaderSize = 2 + 4;
        private static readonly System.Text.Encoding Utf8 = new System.Text.UTF8Encoding(false, true);

        public static async Task SendPacketAsync<TGamePacket>(this TGamePacket packet, NetworkStream stream)
            where TGamePacket : GamePacket
        {
            string json = packet.Serialize();

            byte[] body = Utf8.GetBytes(json);
            byte[] header = new byte[HeaderSize];

            BinaryPrimitives.WriteUInt16BigEndian(header.AsSpan(0, 2), (ushort)packet.Type);
            BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(2, 4), (uint)body.Length);

            await stream.WriteAsync(header, 0, header.Length);
            await stream.WriteAsync(body, 0, body.Length);
        }

        public static async Task<RecievedPacket?> ReceivePacketAsync(NetworkStream stream)
        {
            byte[] header = new byte[HeaderSize];
            int bytesRead = await stream.ReadAsync(header, 0, HeaderSize);
            if (bytesRead < HeaderSize)
            {
                return null;
            }

            ushort packetTypeValue = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(0, 2));
            uint bodyLength = BinaryPrimitives.ReadUInt32BigEndian(header.AsSpan(2, 4));

            byte[] body = new byte[bodyLength];
            bytesRead = await stream.ReadAsync(body, 0, (int)bodyLength);
            if (bytesRead < bodyLength)
            {
                return null;
            }

            string json = Utf8.GetString(body);

            return new RecievedPacket
            {
                Type = (PacketType)packetTypeValue,
                Json = json
            };
        }
    }
}