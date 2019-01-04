using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var imeSustava = "GameServer";
            Console.Title = imeSustava;
            using (var system = ActorSystem.Create(imeSustava))
            {
                Props props = Props.Create(() => new ServerActor());
                var serverActor = system.ActorOf(props, "serverActor");
                system.WhenTerminated.Wait();
            }
        }
    }
}
