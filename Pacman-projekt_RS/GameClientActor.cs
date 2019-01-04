using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Messages;
using System.Configuration;
namespace Pacman_projekt_RS
{
    class GameClientActor :ReceiveActor
    {
        private IActorRef gameRenderingActor;
        private ActorSelection serverActor;
        private int currentX;
        private int currentY;
        private int score;
        private Guid id;
        private string avatarName;
        private Dictionary<Guid, ICancelable> cancelableGhostMsg;
        private Dictionary<Guid, ICancelable> cancelableCollisionMsg;
        private Dictionary<Guid, ICancelable> cancelableCoinMsg;
        public GameClientActor(IActorRef gameRenderingActor, Guid id, int currentX, int currentY)
        {
            score = 0;
            cancelableCoinMsg = new Dictionary<Guid, ICancelable>();
            cancelableCollisionMsg = new Dictionary<Guid, ICancelable>();
            cancelableGhostMsg = new Dictionary<Guid, ICancelable>();
            var serverAddress = ConfigurationManager.AppSettings["serverActorAddress"];
            serverActor = Context.ActorSelection(serverAddress);
            this.gameRenderingActor = gameRenderingActor;
            this.id = id;
            this.currentX = currentX;
            this.currentY = currentY;
            this.avatarName = "pacman" + id.GetHashCode().ToString().Substring(id.GetHashCode().ToString().Length - 2);

            //poruke od ServerActor-a
            Receive<UpdateLocation>(x => HandleUpdateLocation(x));
            Receive<StartGame>(x => HandleStartGame(x));
            Receive<MsgPlayers>(x => { if (id == x.Id) { gameRenderingActor.Tell(x); } });
            Receive<UpdateScore>(x => HandleUpdateScore(x));
            Receive<AnnounceWinnerAndStopGame>(x => HandleAnnounceWinnerAndStopGame(x));
            Receive<Unsubscribe>(x => HandleUnsubscribe(x));

            //poruke od GameRenderActor-a
            Receive<UpdateStartCoordinates>(x => HandleUpdateStartCoordinates(x)); 
            Receive<GameOver>(x => HandleGameOver(x));
            Receive<UpdateMyScore>(x => HandleUpdateMyScore(x));

            //poruke o kretanju
            Receive<Move.Left>(x => HandleMoveLeft());
            Receive<Move.Right>(x => HandleMoveRight());
            Receive<Move.Up>(x => HandleMoveUp());
            Receive<Move.Down>(x => HandleMoveDown());



        }

        private void HandleUnsubscribe(Unsubscribe x)
        {
            gameRenderingActor.Tell(new Unsubscribe(id, avatarName));
        }

        private void HandleQuit(Quit x)
        {
            score = score - 1000;
            serverActor.Tell(new UpdateScore(id, avatarName, score, ""));
            serverActor.Tell(new AnnounceWinnerAndStopGame(""));

        }

        private void HandleUpdateScore(UpdateScore x)
        {
            gameRenderingActor.Tell(new UpdateScore(x.Id, x.Avatar, x.Score, x.CoinName));
        }

        private void HandleUpdateMyScore(UpdateMyScore x)
        {
            score += 5;
            serverActor.Tell(new UpdateScore(x.Id, x.AvatarName, score, x.CoinName));
        }

        private void HandleAnnounceWinnerAndStopGame(AnnounceWinnerAndStopGame x)
        {
            cancelableGhostMsg[id].Cancel();
            cancelableGhostMsg.Remove(id);

            cancelableCoinMsg[id].Cancel();
            cancelableCoinMsg.Remove(id);

            cancelableCollisionMsg[id].Cancel();
            cancelableCollisionMsg.Remove(id);
            gameRenderingActor.Tell(x);

            serverActor.Tell(new Unsubscribe(id, avatarName));
            
        }

        private void HandleGameOver(GameOver x)
        {
            serverActor.Tell(new AnnounceWinnerAndStopGame("")); 
        }
        
        private void HandleUpdateStartCoordinates(UpdateStartCoordinates x)
        {
            if (id == x.Id)
            {
                currentX = x.NewX;
                currentY = x.NewY;
            }
        }

        //poruka od servera
        private void HandleStartGame(StartGame x)
        {
            gameRenderingActor.Tell(new StartGame(x.Avatars));
            var cancelableCollision = Program.actorSystem.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.1), gameRenderingActor, new CheckCollisionGhosts(id, avatarName), Self);
            var cancelableGhost = Program.actorSystem.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.1), gameRenderingActor, new MoveGhosts(), Self);
            var cancelableCoin = Program.actorSystem.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.1), gameRenderingActor, new CheckCollisionCoin(id, avatarName, score), Self);
            cancelableGhostMsg.Add(id, cancelableGhost);
            cancelableCollisionMsg.Add(id, cancelableCollision);
            cancelableCoinMsg.Add(id, cancelableCoin);
        }

        //poruka od servera
        private void HandleUpdateLocation(UpdateLocation x)
        {
            var msg = new MakeMove(x.Avatar, x.NewX, x.NewY, x.Direction);
            gameRenderingActor.Tell(msg);
        }

        protected override void PreStart()
        {
            // kažemo serveru da smo se priključili
            var addMsg = new AddAvatar(id, avatarName, currentX, currentY);
            serverActor.Tell(addMsg);
            var msg = new Subscribe(id);
            serverActor.Tell(msg);

            base.PreStart();
        }

        private void SendLocationUpdate(int x, int y, string direction)
        {
            var msg = new UpdateLocation(id, avatarName, x, y, direction);
            serverActor.Tell(msg);
        }

        #region kopirano
        private void MoveAndSendLocationUpdate(Action move, string direction)
        {
            var oldX = currentX;
            var oldY = currentY;
            // pozivamo akciju
            move();

            SendLocationUpdate(oldX, oldY, direction);
        }

        public void HandleMoveLeft()
        {
            // akcija pomaka lijevo odgovara smanjenju X koordinate za 1
            MoveAndSendLocationUpdate(() => currentX -= 2, "left");
        }

        public void HandleMoveRight()
        {
            MoveAndSendLocationUpdate(() => currentX += 2, "right");
        }

        public void HandleMoveUp()
        {
            // akcija pomaka gore odgovara smanjenju Y koordinate za 1
            MoveAndSendLocationUpdate(() => currentY -= 2, "up");
        }

        public void HandleMoveDown()
        {
            // akcija pomaka lijevo odgovara smanjenju X koordinate za 1
            MoveAndSendLocationUpdate(() => currentY += 2, "down");
        }
        #endregion
    }


}
