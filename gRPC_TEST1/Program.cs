using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityComm;
using UDP_Arduino;

namespace gRPC_TEST1
{
    //class GreeterImpl : Greeter.GreeterBase
    //{
    //    // Server side handler of the SayHello RPC
    //    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    //    {
    //        return Task.FromResult(new HelloReply { Message = "Hello Grande " + request.Name });
    //    }
    //}

    class Program
    {
        const int ListenPort = 30051;

        static void Main(string[] args)
        {



            Server server = new Server
            {
                Services = { ArmComm.BindService(new ArmCommImpl()) },
                Ports = {new ServerPort("localhost", ListenPort, ServerCredentials.Insecure)}
            };

            //Server server = new Server
            //{
            //    Services = { Greeter.BindService(new GreeterImpl()) },
            //    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            //};
            server.Start();

            Console.WriteLine("Greeter server listening on port " + ListenPort);



            string read;
            do
            {


                read = Console.ReadLine();

                //client.Send(read);
            } while (read != "quit");


        }
    }

    class ArmCommImpl : ArmComm.ArmCommBase
    {
        // Server side handler of the SayHello RPC
        ServoValues _lastRequest;

        uint m1;
        uint m2;
        uint m3;
        uint m4;
        uint m5;
        uint m6;

        private bool NotDuplicate(ServoValues request)
        {
            return (m1 != request.M1 || m2 != request.M2 || m3 != request.M3 || m4 != request.M4 || m5 != request.M5 || m6 != request.M6);
        }

        //Establish UDP Client to handle requests to arduino via IP/UDP
        UDPClient UDPClientArduino = new UDPClient();

        public override Task<Recieved> SendArmComm(ServoValues request, ServerCallContext context)
        {
            bool finishedCmd = false;

            if (NotDuplicate(request))
            {
                m1 = request.M1;
                m2 = request.M2;
                m3 = request.M3;
                m4 = request.M4;
                m5 = request.M5;
                m6 = request.M6;
                //Console.WriteLine("message recieved" + request.ToString());

                //Add calls to Arduino via UDP
                var reply = UDPClientArduino.Send(m1, m2, m3, m4, m5, m6, request.Step);
                if (reply)
                {
                    finishedCmd = true;
                    _lastRequest = request;
                }
            }

            return Task.FromResult(new Recieved { Replymessage = finishedCmd }); ;
        }
        

        
        //    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        //    {
        //        return Task.FromResult(new HelloReply { Message = "Hello Grande " + request.Name });
        //    }

    }
}
