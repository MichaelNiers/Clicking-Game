using System.Windows;

namespace Game2
{
    internal class Game
    {
        public Player current { get; set; } 
        public bool running;
        public Point point;
        public Game(Point x)
        {
            running = false;
            point = x;
        }
       

        public void Run()
        {

        }

        public void erhoeheClicks()
        {
            current.Clicks += 1;
        }

        public void setPlayer(string newName)
        {
            current = new Player(newName);
        }

        public void setFinal(Point x)
        {
            current = null;
            point = x;
        }


    }
}