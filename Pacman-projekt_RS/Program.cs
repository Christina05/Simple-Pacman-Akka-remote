using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;
namespace Pacman_projekt_RS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static Form1 myForm;
        public static ActorSystem actorSystem;

        [STAThread]
        static void Main()
        {
            var imeKlijenta = "GameClient";
            actorSystem = ActorSystem.Create(imeKlijenta);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(myForm = new Form1());
        }
    }
}
