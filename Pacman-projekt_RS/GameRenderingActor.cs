using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pacman_projekt_RS.Properties;
using Akka.Actor;
using Messages;
using System.Windows.Forms;
using System.Drawing;

namespace Pacman_projekt_RS
{
    class GameRenderingActor : ReceiveActor
    {
        private List<PictureBox> _upperBorders;  
        private List<PictureBox> _sideBorders;
        private List<PictureBox> _walls; 
        private List<PictureBox> _allWalls;

        
        private int x_move_red; 
        private int x_move_orange; 
        private int x_move_pink;
        private int y_move_pink;

        public GameRenderingActor()
        {
            x_move_red = 8;
            x_move_orange = 8;
            x_move_pink = 8;
            y_move_pink = 8;
            #region walls
            _walls = new List<PictureBox>();
            _walls.Add((PictureBox)Program.myForm.Controls.Find(key: "pb5", searchAllChildren: false).FirstOrDefault());
            _walls.Add((PictureBox)Program.myForm.Controls.Find(key: "pb6", searchAllChildren: false).FirstOrDefault());
            _walls.Add((PictureBox)Program.myForm.Controls.Find(key: "pb7", searchAllChildren: false).FirstOrDefault());
            _walls.Add((PictureBox)Program.myForm.Controls.Find(key: "pb8", searchAllChildren: false).FirstOrDefault());

            _sideBorders = new List<PictureBox>();
            _sideBorders.Add((PictureBox)Program.myForm.Controls.Find(key: "pb1", searchAllChildren: false).FirstOrDefault());
            _sideBorders.Add((PictureBox)Program.myForm.Controls.Find(key: "pb3", searchAllChildren: false).FirstOrDefault());
            _upperBorders = new List<PictureBox>();
            _upperBorders.Add((PictureBox)Program.myForm.Controls.Find(key: "pb2", searchAllChildren: false).FirstOrDefault());
            _upperBorders.Add((PictureBox)Program.myForm.Controls.Find(key: "pb4", searchAllChildren: false).FirstOrDefault());

            _allWalls = new List<PictureBox>();
            _allWalls.AddRange(_upperBorders);
            _allWalls.AddRange(_sideBorders);
            _allWalls.AddRange(_walls);
            #endregion

            Receive<MakeMove>(x => HandleMakeMove(x)); //kretanje pacmana
            Receive<StartGame>(x => HandleStartGame(x)); //dodavavnje avatara 
            Receive<MoveGhosts>(x => HandleMoveGhosts()); //kretanje duhova
            Receive<CheckCollisionGhosts>(x => HandleCheckCollisionGhosts(x)); //sudar s duhom
            Receive<CheckCollisionCoin>(x => HandleCheckCollisionCoin(x)); //dodir novčića
            Receive<AnnounceWinnerAndStopGame>(x => { MessageBox.Show($"pobjednik je {x.Winner}"); }); 
            Receive<MsgPlayers>(x => HandleMsgPlayers(x)); //objava avatara
            Receive<UpdateScore>(x => HandleUpdateScore(x));//mijenjanje score-a
            Receive<Unsubscribe>(x => HandleUnsubscribe(x));
        }

        private void HandleUnsubscribe(Unsubscribe x)
        {
            PictureBox avatar = (PictureBox)Program.myForm.Controls.Find(key: x.Avatar, searchAllChildren: false).FirstOrDefault();
            Program.myForm.Controls.Remove(avatar);
            Label lbl = (Label)Program.myForm.Controls.Find(key: $"Lbl{x.Avatar}", searchAllChildren: false).FirstOrDefault();
            Program.myForm.Controls.Remove(lbl);


        }

        private void HandleUpdateScore(UpdateScore x)
        {

            Label lbl = (Label)Program.myForm.Controls.Find(key: $"Lbl{x.Avatar}", searchAllChildren: false).FirstOrDefault();
            lbl.Text = $"{x.Avatar}:{x.Score}";
            
            var c = (PictureBox)Program.myForm.Controls.Find(key: x.CoinName, searchAllChildren: false).FirstOrDefault();
            Program.myForm.Controls.Remove(c); 
        }

        private void HandleCheckCollisionCoin(CheckCollisionCoin x)
        {
            List<Control> nov = new List<Control>();

            foreach (Control coin in Program.myForm.Controls)
            {
                if ((string)coin.Tag == "coin")
                {
                    nov.Add(coin);
                }
            }
            var pacman = (PictureBox)Program.myForm.Controls.Find(key: x.AvatarName, searchAllChildren: false).FirstOrDefault();

            if (nov.Count == 0)
            {
                Sender.Tell(new GameOver(x.Id)); 
            }
            else
            {
                foreach (var coin in nov)
                {
                    if (pacman.Bounds.IntersectsWith(coin.Bounds))
                    {

                        Sender.Tell(new UpdateMyScore(x.Id, x.AvatarName, x.Score, coin.Name));  
                    }
                }
            }
        }

        private void HandleMsgPlayers(MsgPlayers x)
        {
            MessageBox.Show(x.Poruka);
        }

        private void HandleCheckCollisionGhosts(CheckCollisionGhosts x)
        {
            List<PictureBox> ghostList = new List<PictureBox>();
            ghostList.Add((PictureBox)Program.myForm.Controls.Find(key: "redGhost", searchAllChildren: false).FirstOrDefault());
            ghostList.Add((PictureBox)Program.myForm.Controls.Find(key: "orangeGhost", searchAllChildren: false).FirstOrDefault());
            ghostList.Add((PictureBox)Program.myForm.Controls.Find(key: "pinkGhost", searchAllChildren: false).FirstOrDefault());

            var pacman = (PictureBox)Program.myForm.Controls.Find(key: x.AvatarName, searchAllChildren: false).FirstOrDefault();
            foreach (var ghost in ghostList)
            {
                if (ghost.Bounds.IntersectsWith(pacman.Bounds))
                {
                    Sender.Tell(new GameOver(x.Id)); 
                }
            }
        }

        private void HandleMoveGhosts()
        {
            var ghostRed = (PictureBox)Program.myForm.Controls.Find(key: "redGhost", searchAllChildren: false).FirstOrDefault();
            var ghostOrange = (PictureBox)Program.myForm.Controls.Find(key: "orangeGhost", searchAllChildren: false).FirstOrDefault();

            int x_coor_red = ghostRed.Location.X;
            int y_coor_red = ghostRed.Location.Y;

            int x_coor_orange = ghostOrange.Location.X;
            int y_coor_orange = ghostOrange.Location.Y;

            foreach (var wall in _allWalls)
            {
                if (ghostRed.Bounds.IntersectsWith(wall.Bounds))
                {
                    x_move_red = -x_move_red; 
                }
                if (ghostOrange.Bounds.IntersectsWith(wall.Bounds))
                {
                    x_move_orange = -x_move_orange; 
                }
            }
            Point newPointRed = new Point(x_coor_red + x_move_red, y_coor_red);
            Point newPointOrange = new Point(x_coor_orange + x_move_orange, y_coor_orange);
            ghostRed.Location = newPointRed;
            ghostOrange.Location = newPointOrange;

            var ghostPink = (PictureBox)Program.myForm.Controls.Find(key: "pinkGhost", searchAllChildren: false).FirstOrDefault();
            int x_coor_pink = ghostPink.Location.X;
            int y_coor_pink = ghostPink.Location.Y;

            foreach (var wall in _upperBorders)
            {
                if (ghostPink.Bounds.IntersectsWith(wall.Bounds))
                {
                    y_move_pink = -y_move_pink;
                }
            }
            foreach (var wall in _walls)
            {
                if (ghostPink.Bounds.IntersectsWith(wall.Bounds))
                {
                    y_move_pink = -y_move_pink;
                }
            }
            foreach (var wall in _sideBorders)
            {
                if (ghostPink.Bounds.IntersectsWith(wall.Bounds))
                {
                    x_move_pink = -x_move_pink;
                }
            }
            Point newPointPink = new Point(x_coor_pink + x_move_pink, y_coor_pink + y_move_pink);
            ghostPink.Location = newPointPink;

        }

        private void HandleStartGame(StartGame x)
        {
            Label lbl = (Label)Program.myForm.Controls.Find(key: "čeka", searchAllChildren: false).FirstOrDefault();
            Program.myForm.Controls.Remove(lbl);

            Dictionary<Guid, AddAvatar> avatars = x.Avatars;
            int i = 0;
            foreach (var one in avatars)
            {
                Image pic = Resources.right2;
                Point location = new Point(one.Value.X, one.Value.Y);
                Point lblLocation;

                if (i % 2 == 0)
                {
                    pic = Resources.left;
                    location = new Point(12, 72);
                    lblLocation = new Point(490, 1);
                }
                else
                {
                    pic = Resources.right2;
                    location = new Point(561, 531);
                    lblLocation = new Point(490, 25);
                }

                Label player = new Label
                {
                    Name = $"Lbl{one.Value.AvatarName}",
                    Size = new System.Drawing.Size(80, 20),
                    Location = lblLocation,
                    Text = $"{one.Value.AvatarName}: 0"
                };
                Program.myForm.Controls.Add(player);

                PictureBox avatar = new PictureBox
                {
                    Name = one.Value.AvatarName,
                    Size = new System.Drawing.Size(40, 40),
                    Location = location,
                    Image = pic,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = $"pacman{i}"
                };
                Program.myForm.Controls.Add(avatar);
                Sender.Tell(new UpdateStartCoordinates(one.Value.Id, one.Value.AvatarName, avatar.Location.X, avatar.Location.Y));
                i++;
            }

        }


        private void HandleMakeMove(MakeMove x)
        {
            PictureBox avatar = (PictureBox)Program.myForm.Controls.Find(key: x.AvatarName, searchAllChildren: false).FirstOrDefault(); 
            int x_old = avatar.Location.X;
            int y_old = avatar.Location.Y;
            string direction = x.Direction;
            Image pic = null;
            var location = new System.Drawing.Point(x.NewX, x.NewY);
            if ((string)avatar.Tag == "pacman0")
            {
                switch (direction)
                {
                    case "left":
                        pic = Resources.right;
                        break;
                    case "right":
                        pic = Resources.left;
                        break;
                    case "up":
                        pic = Resources.up;
                        break;
                    case "down":
                        pic = Resources.down;
                        break;
                }
            }
            else
            {
                switch (direction)
                {
                    case "left":
                        pic = Resources.right2;
                        break;
                    case "right":
                        pic = Resources.left2;
                        break;
                    case "up":
                        pic = Resources.up2;
                        break;
                    case "down":
                        pic = Resources.down2;
                        break;
                }
            }
            avatar.Image = pic;
            avatar.Location = location; 

            foreach (var wall in _allWalls)
            {
                if (avatar.Bounds.IntersectsWith(wall.Bounds)) 
                {
                    location = new System.Drawing.Point(x_old, y_old);
                    avatar.Location = location;
                }
            }
        }
    }
}
