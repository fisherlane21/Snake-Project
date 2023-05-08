using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake_Project
{
    // This class represents the current state of the game
    public class GameState
    {
        // Public properties that can be accessed from other classes
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
      // Private fields that are used internally

        public bool GameOver { get; private set; }
        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        // constructor that takes amount of rows and cols as parameters
        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[rows, cols];
            Dir = Direction.Right;
            AddSnake(); // Add the initial snake to the grid
            AddFood();  // Add the initial food to the grid


        }

        // Add the initial snake to the grid
        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));

            }
        }
        // Returns an enumerable of all the empty positions on the grid
        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {

                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }


                }


            }
        }
        // Add food to the grid at a random empty position

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());
            if (empty.Count == 0)
            {
                return;
            }
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }
        // Get the position of the snake's head
        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }
        // Get the position of the snake's tail

        public Position TailPosition() 
        {
            return snakePositions.Last.Value;

        }
        // Get an enumerable of all the positions occupied by the snake

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }
        // Add a new head to the snake

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }
        // Remove the tail of the snake

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePositions.RemoveLast();    
        }

        // Get the direction of the last change made to the snake's direction

        private Direction GetLastDirection()
        {
            if (dirChanges.Count == 0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }
        // Check if the snake can change direction to the specified direction

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }
            Direction lastDir  = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        // This method changes the direction of the snake to the specified direction, 
        // if it is allowed based on the game rules.
        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);

            }
        }
        // This private method checks whether the specified position is outside the grid.

        private bool OutsideGrid(Position pos)
        {
            return pos.Row <0 || pos.Row >= Rows ||pos.Col <0 || pos.Col >= Cols;
        }


        // This private method checks what the snake will hit if it moves to the specified 
        // position. Returns a GridValue enum representing the grid cell value.

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPosition()) 
            {
                return GridValue.Empty;
            } 
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }
        


        public void Move()
        {
            // Check if there are any direction changes queued up

            if (dirChanges.Count > 0)
            {
                // Change the direction of the snake to the first direction in the queue and remove it from the queue
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            // Calculate the new head position based on the current direction of the snake
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);
            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                // Set the GameOver flag to true if the snake hits the wall or itself
                GameOver = true;

            }
            else if (hit == GridValue.Empty) 
            {
                // If the new head position is empty, remove the tail and add the new head
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if (hit == GridValue.Food) 
            {
                // If the new head position contains food, add the new head and update the score, then add new food
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }

        }


    }

}
