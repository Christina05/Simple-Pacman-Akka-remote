using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Messages;

namespace Server
{
    class ServerActor:ReceiveActor
    {
        private Dictionary<Guid, IActorRef> _subscribers;
        private Dictionary<Guid, AddAvatar> _avatars;
        private Dictionary<string, int> _scores;
        private bool start;
        private int unsuscribed;
        public ServerActor()
        {
            unsuscribed = 0;
            start = true;
            _subscribers = new Dictionary<Guid, IActorRef>();
            _avatars = new Dictionary<Guid, AddAvatar>();
            _scores = new Dictionary<string, int>();

            Receive<Subscribe>(x => HandleSubscribe(x));
            Receive<AddAvatar>(x => HandleAddAvatar(x));
            Receive<UpdateLocation>(x => HandleUpdateLocation(x));
            Receive<AnnounceWinnerAndStopGame>(x => HandleStopGame());
            Receive<UpdateScore>(x => HandleUpdateScore(x));
            Receive<Unsubscribe>(x => HandleUnsubscribe(x));
        }

        private void HandleUpdateScore(UpdateScore x)
        {
            _scores[x.Avatar] = x.Score;
            Broadcast(new UpdateScore(x.Id, x.Avatar, x.Score, x.CoinName));
        }

        private void HandleStopGame() 
        {
            var winner = _scores.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            Broadcast(new AnnounceWinnerAndStopGame(winner));
        }

        private void HandleUpdateLocation(UpdateLocation x)
        {
            Broadcast(x);
        }

        private void HandleAddAvatar(AddAvatar x)
        {
            if (!_avatars.ContainsKey(x.Id) && !_scores.ContainsKey(x.AvatarName))
            {
                _avatars.Add(x.Id, x);
                _scores.Add(x.AvatarName, 0);
            }
        }

        private void HandleSubscribe(Subscribe x)
        {
            Console.WriteLine($"Actor: {Sender.Path} subscribed with guidId: {x.GuidId.ToString()}");
            if (!_subscribers.ContainsKey(x.GuidId))
            {
                _subscribers.Add(x.GuidId, Sender);
            }
            string avatar = (_subscribers.Count % 2 == 0) ? "male" : "female";
            string num = $"pacman{x.GuidId.GetHashCode().ToString().Substring(x.GuidId.GetHashCode().ToString().Length - 2)}";
            var msg = new MsgPlayers($"Vi ste igrač {num}, avatar: {avatar}", x.GuidId);

            Broadcast(msg);
            StartOrWait();

        }

        private void StartOrWait()
        {
            if (_subscribers.Count>=2  && start)
            {
                Broadcast(new StartGame(_avatars));
                start = false;
                unsuscribed = 0;
            }
        }

        private void HandleUnsubscribe(Unsubscribe x)
        {
            unsuscribed++;
            if (_subscribers.ContainsKey(x.Id))
            {
                _subscribers.Remove(x.Id);
            }

            if (_scores.ContainsKey(x.Avatar))
            {
                _scores.Remove(x.Avatar);
            }
            if (_avatars.ContainsKey(x.Id))
            {
                _avatars.Remove(x.Id);
            }

            Broadcast(x);

            if (unsuscribed == 2)
            { start = true; }
            else { start = false; }

            StartOrWait();

        }

        private void Broadcast<T>(T message)
        {
            foreach (var actor in _subscribers.Values)
            {
                actor.Tell(message);
            }
        }
    }
}
