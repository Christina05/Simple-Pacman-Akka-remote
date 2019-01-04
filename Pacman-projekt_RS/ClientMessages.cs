using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacman_projekt_RS
{
    class MoveGhosts { }

    class UpdateMyScore
    {
        public Guid Id { get; }
        public string AvatarName { get; }
        public int Score { get; }
        public string CoinName { get; }
        public UpdateMyScore(Guid id, string avatar, int score, string coinName)
        {
            CoinName = coinName;
            AvatarName = avatar;
            Score = score;
            Id = id;
        }
    }
    class CheckCollisionGhosts
    {
        public string AvatarName { get; }
        public Guid Id { get; }
        public CheckCollisionGhosts(Guid id, string avatarName)
        {
            AvatarName = avatarName;
            Id = id;
        }
    }
    class CheckCollisionCoin
    {
        public string AvatarName { get; }
        public Guid Id { get; }
        public int Score { get; }
        public CheckCollisionCoin(Guid id, string avatarName, int score)
        {
            Score = score;
            AvatarName = avatarName;
            Id = id;
        }
    }
    class GameOver
    {
        public Guid Id { get; }
        public GameOver(Guid id)
        {
            Id = id;
        }
    }
    class MakeMove
    {
        public string AvatarName { get; }
        public int NewX { get; }
        public int NewY { get; }
        public string Direction { get; }
        public MakeMove(string avatarName, int newX, int newY, string direction)
        {
            Direction = direction;
            AvatarName = avatarName;
            NewX = newX;
            NewY = newY;
        }
    }

    class UpdateStartCoordinates
    {
        public Guid Id { get; }
        public string Avatar { get; }
        public int NewX { get; }
        public int NewY { get; }

        public UpdateStartCoordinates(Guid id, string avatar, int newX, int newY)
        {
            Id = id;
            Avatar = avatar;
            NewX = newX;
            NewY = newY;
        }
    }

    public class Move
    {
        public class Left { }
        public class Right { }
        public class Up { }
        public class Down { }
    }

    public class Quit
    {

    }
}