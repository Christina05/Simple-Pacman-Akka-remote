using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Akka.Actor;

namespace Pacman_projekt_RS
{
    public partial class Form1 : Form
    {
        IActorRef actor;
        public Form1()
        {
            InitializeComponent();
            var id = Guid.NewGuid();
            Props props1 = Props.Create(() => new GameRenderingActor())
                .WithDispatcher("akka.actor.synchronized-dispatcher");
            var renderActor = Program.actorSystem.ActorOf(props1);
            Props props2 = Props.Create(() => new GameClientActor(renderActor, id, 12, 12));
            actor = Program.actorSystem.ActorOf(props2);
            Label lbl = new Label
            {
                Name = "čeka",
                Text = "Molim pričekajte!",
                Location = new System.Drawing.Point(190, 234),
                Font = new Font("Arial", 20),
                Size = new System.Drawing.Size(300, 200)
            };
            Controls.Add(lbl);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if (key == Keys.Down)
            {
                actor.Tell(new Move.Down());

            }
            else if (key == Keys.Left)
            {
                actor.Tell(new Move.Left());
            }
            else if (key == Keys.Up)
            {
                actor.Tell(new Move.Up());
            }
            else if (key == Keys.Right)
            {
                actor.Tell(new Move.Right());
            }
            else if (key == Keys.Escape)
            {
                actor.Tell(new Quit());
            }
        }
    }
}
