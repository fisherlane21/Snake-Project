using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Media;

namespace Snake_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Dictionary that maps GridValue to ImageSource for displaying the game grid

        private readonly Dictionary<GridValue, ImageSource> gridValtoImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake,Images.Body},
            {GridValue.Food, Images.Food },
            
        };

        // Dictionary that maps Direction to rotation angle for rotating the snake head
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0 },
            {Direction.Right,90 },
            {Direction.Down,180},
            {Direction.Left,270}

        };

        // Constants for number of rows and columns in the game grid
        private readonly int rows = 17, cols = 17;

        // 2D array to hold the Image controls for the game grid
        private readonly Image[,] gridImages;

        // Instance of the game state
        private GameState gameState;

        // Boolean flag to indicate if the game is running
        private bool gameRunning;

        // Instance of the SoundPlayer for playing audio
        MediaPlayer player = new MediaPlayer();

        // Boolean flag to indicate if audio is enabled
        public bool audioON = false;


        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid(); // Set up the game grid and store the Image controls in gridImages
            gameState = new GameState(rows,cols); // Initialize the game state with the number of rows and columns
        }

        private async Task RunGame()
        {
            Draw(); // Draw the initial state of the game
            await ShowCountdown(); // Show the countdown before the game starts
            Overlay.Visibility = Visibility.Hidden; // Hide the overlay once the countdown is over
            await GameLoop(); // start the game loop
            await ShowGameOver(); // Show the game over screen once the game is over
            gameState = new GameState(rows,cols); // Reset the game state for a new game
        }

        // plays thunderstruck if the radio button is checked
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (audioON == false)
            {
                player.Position= new TimeSpan(0,0,0);
                player.Open(new Uri($"Assets/DC - Thunderstruck (Official Video).wav", UriKind.Relative));
                player.Play();
                audioON = true;
            }
            else
            {
                player.Stop();
                audioON = false;
                MusicBtn.IsChecked = false;
            }
        }

       
        // It checks if the overlay is visible and sets the Handled property to true to indicate that the event has been handled. 
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
               
                await RunGame();
                gameRunning = false;
            }
        }

        // method is an event handler that responds to the KeyDown event of the window. It checks if the game is over and returns if it is.
        // Otherwise, it changes the direction of the snake based on the key that was pressed.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;  
            }
        }


        // This method runs the game loop until the game is over

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }


        // This method sets up the initial image grid for the game board
        private Image[,]SetupGrid()
        {
            Image[,] images = new Image[rows, cols];   
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            // Loop through each row and column of the grid and create a new image for each cell

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image()
                    {
                        // Create a new image with the empty image source and set the render transform origin
                        Source = Images.Empty,
                      RenderTransformOrigin = new Point(0.5, 0.5),
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            // Return the array of images
            return images;
        }


        // This method draws the current game state on the screen
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }


        // This method draws the grid on the screen
        private void DrawGrid()
        {
            // Loop through each row in the grid
            for (int r = 0;r < rows; r++)
            {
                // Loop through each column in the grid
                for (int c =0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValtoImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
               

            }
        }

        // This method draws the snake's head on the screen

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            // Determine the rotation angle based on the snake's current direction
            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        // This method draws the dead snake on the screen
        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            // Loop through each position in the body and animate the death sequence
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }


        // This method shows a countdown on the screen before starting the game
        private async Task ShowCountdown()
        {
            for(int i = 3; i>=1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }

        }


        // This method shows the game over screen on the screen
        private async Task ShowGameOver()
        {
            // draws dead snake
            await DrawDeadSnake();
            await Task.Delay(1000);

            // Make the overlay visible
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}
