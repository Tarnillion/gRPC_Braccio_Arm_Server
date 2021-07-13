using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace UDP_Arduino
{

    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }

    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();
            return new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }
    }

    //Server
    class UdpListener : UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123))
        {
        }

        public UdpListener(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new UdpClient(_listenOn);
        }

        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length, endpoint);
        }

    }

    //Client
    class UdpUser : UdpBase
    {
        private UdpUser() { }

        public static UdpUser ConnectTo(string hostname, int port)
        {
            var connection = new UdpUser();
            connection.Client.Connect(hostname, port);
            return connection;
        }

        public void Send(byte[] message)
        {
            var datagram = message;
            //var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }
        public void Send(string message)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            Client.Send(datagram, datagram.Length);
        }
    }



    public class UDPClient
    {
        UdpUser client;
        public UDPClient()
        {
            client = UdpUser.ConnectTo("192.168.1.167", 2390);
        }

        public bool Send(uint m1, uint m2, uint m3, uint m4, uint m5, uint m6, uint step)
        {
            bool cmd = false;
            Task.Factory.StartNew(async () =>
            {
                byte[] msg = {
                Convert.ToByte(0),
                Convert.ToByte(step),
                Convert.ToByte(m1),
                Convert.ToByte(m2),
                Convert.ToByte(m3),
                Convert.ToByte(m4),
                Convert.ToByte(m5),
                Convert.ToByte(m6)
                 };
                client.Send(msg);
                var received = await client.Receive();

                if (received.Message.Contains("acknowledged"))
                {
                    cmd = true;
                }
            }
            );
            return true;
        }
        


    }


}
