using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class Subscribe
    {
        public Guid GuidId { get; }

        public Subscribe(Guid guidId)
        {
            GuidId = guidId;
        }
    }
    public class UpdateLocation
    {
        public Guid Id { get; }
        public string Avatar { get; }
        public int NewX { get; }
        public int NewY { get; }
        public string Direction { get; }

        public UpdateLocation(Guid id, string avatar, int newX, int newY, string direction)
        {
            Direction = direction;
            Id = id;
            Avatar = avatar;
            NewX = newX;
            NewY = newY;
        }
    }
    public class MsgPlayers
    {
        public string Poruka { get; }
        public Guid Id { get; }
        public MsgPlayers(string poruka, Guid id)
        {
            Id = id;
            Poruka = poruka;
        }
    }

    public class StartGame
    {
        public Dictionary<Guid, AddAvatar> Avatars { get; }
        public StartGame(Dictionary<Guid, AddAvatar> avatars)
        {
            Avatars = avatars;
        }
    }
    public class UpdateScore
    {
        public Guid Id { get; }
        public string Avatar { get; }
        public int Score { get; }
        public string CoinName { get; }
        public UpdateScore(Guid id, string avatar, int score, string coinName)
        {
            CoinName = coinName;
            Id = id;
            Score = score;
            Avatar = avatar;
        }
    }
    public class AnnounceWinnerAndStopGame
    {
        public string Winner { get; }
        public AnnounceWinnerAndStopGame(string winner)
        {
            Winner = winner;
        }
    }
    public class Unsubscribe
    {
        public Guid Id { get; }
        public string Avatar { get; }
        public Unsubscribe(Guid id, string avatar)
        {
            Avatar = avatar;
            Id = id;
        }
    }
    public class AddAvatar
    {
        public Guid Id { get; }
        public string AvatarName { get; }
        public int X;
        public int Y;
        public AddAvatar(Guid id, string avatarName, int x, int y)
        {
            Id = id;
            AvatarName = avatarName;
            X = x;
            Y = y;
        }
    }
}
