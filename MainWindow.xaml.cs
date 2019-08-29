using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Game2
{
    /// Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        public ObservableCollection<Player> Members { get; set; }
        private Ellipse ellipse = new Ellipse();
        private static Random random = new Random();
        private Game Spiel { get; set; }
        bool equalRank = false;
        public MainWindow()
        {
            InitializeComponent();

            ellipse.Width = 20;
            ellipse.Height = 20;
            ellipse.StrokeThickness = 2;

            TheImage = new Image();
            TheImage.Source = new BitmapImage(new Uri(@"C:\Users\mniers\Desktop\test.png"));
            TheImage.Stretch = Stretch.None;
            TheImage.HorizontalAlignment = HorizontalAlignment.Left;
            TheImage.Height = 400;
            TheImage.Width = 625;
            // The farthest left the dot can be

            Members = new ObservableCollection<Player>();
          

            this.Spiel = new Game(setRandom());
            Load();

            var result = this.Members.OrderBy(p => p.Clicks).ToList();
            this.Members.Clear();
            foreach(var p in result) this.Members.Add(p);
            
            DataContext = this;
            
        }
        private Point setRandom()
        {
            int minLeft = 0;
            // The farthest right the dot can be without it going off the screen
            int maxLeft = Convert.ToInt32(TheImage.Width);
            // The farthest up the dot can be
            int minTop = 0;
            // The farthest down the dot can be without it going off the screen
            int maxTop = Convert.ToInt32(TheImage.Height);

            int left = RandomBetween(minLeft, maxLeft);
            int top = RandomBetween(minTop, maxTop);

            return new Point(left, top);
        }
        private Image _theImage;
        public Image TheImage
        {
            get
            {
                return _theImage;
            }

            set
            {
                _theImage = value;
                OnPropertyChanged("TheImage");
            }
        }

        public object Interaction { get; private set; }

        #region INotifiedProperty Block
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void ContentControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //how to get the coordinates of the mouse click here on the image?
            if (Spiel.running == false) return;

            Spiel.erhoeheClicks();
            Clicks.Text = Spiel.current.Clicks.ToString();
            Point p = Spiel.point;
            Point b = e.GetPosition(TheImage);
            Point diagonalh = new Point(this.TheImage.Width, this.TheImage.Height);
            double diagonal = Point.Subtract(new Point(0, 0), diagonalh).Length;
            double distance = Point.Subtract(b, p).Length;
            double num = (ellipse.Width / 2) / diagonal;
            distance = distance / diagonal;
            if (distance <= num)
            {
                Ellipse final = new Ellipse();
                final.Width = ellipse.Width;
                final.Height = ellipse.Height;
                final.StrokeThickness = ellipse.StrokeThickness;
                final.Fill = Brushes.Black;
                Cnv.Children.Add(final);

                Canvas.SetLeft(final, p.X);
                Canvas.SetTop(final, p.Y);

                Login.Visibility = Visibility.Visible;

                int index = 0;

                for (var i = 0; i < Members.Count; i++)
                {
                    if (Members[i].Clicks > Spiel.current.Clicks)
                    {
                        break;
                    }
                    index += 1;
                }
                Spiel.current.Rank = index + 1;
                for (var i = 0; i < Members.Count; i++)
                {
                    if (Members[i].Clicks == Spiel.current.Clicks)
                    {
                        Spiel.current.Rank = Members[i].Rank;
                        equalRank = true;
                    }
              
                }

                Members.Insert(index, Spiel.current);
                if (equalRank == false)
                {
                    for (var i = index + 1; i < Members.Count; i++)
                    {
                        Members[i].Rank += 1;
                    }
                }
                CollectionViewSource.GetDefaultView(this.Members).Refresh();
                Spiel.setFinal(setRandom());
                Cnv.Children.RemoveRange(4, Cnv.Children.Count - 3);

                Spiel.running = false;
                //MessageBox.Show("Hi");
            }
            else
            {

                Color c = colour.GetColor(distance);
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = c;

                Ellipse newEll = new Ellipse();
                newEll.Width = ellipse.Width;
                newEll.Height = ellipse.Height;
                newEll.StrokeThickness = ellipse.StrokeThickness;
                newEll.Fill = mySolidColorBrush;
                Cnv.Children.Add(newEll);

                Canvas.SetLeft(newEll, e.GetPosition(TheImage).X);
                Canvas.SetTop(newEll, e.GetPosition(TheImage).Y);

            }
        }

        private int RandomBetween(int min, int max)
        {
            return random.Next(min, max);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string name = this.Player.Text;
            this.Spiel.current = new Player(name);
            Spiel.running = true;
            Clicks.Text = Spiel.current.Clicks.ToString();
            Login.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Save();
        }
        
        private void Load()
        {
            var Scores = new Ranking();
            if (File.Exists(@"C:\Users\mniers\Documents\Visual Studio 2017\Projects\Game2\Game2\Highscore.xml"))
            {
                Type outType = typeof(Ranking);

                XmlSerializer serializer = new XmlSerializer(outType);
                using (var reader = new StreamReader(@"C:\Users\mniers\Documents\Visual Studio 2017\Projects\Game2\Game2\Highscore.xml"))
                {
                    Scores = (Ranking)serializer.Deserialize(reader);
                }
            }

            foreach (var p in Scores.Members)
            { Members.Add(p); } 
        }

        public void Save()
        {
            var Scores = new Ranking();
            foreach (var p in Members)
            { Scores.Members.Add(p); }
            XmlSerializer serializer = new XmlSerializer(typeof(Ranking));
            using (StreamWriter streamWriter = File.CreateText(@"C:\Users\mniers\Documents\Visual Studio 2017\Projects\Game2\Game2\Highscore.xml"))
            {
                serializer.Serialize(streamWriter, Scores);
            }
        }

    }
}

