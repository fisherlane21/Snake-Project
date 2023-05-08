

using System;
using System.Collections.Generic;
using System.Security.Policy;

namespace Snake_Project
{
    //// This class represents the direction the snake can move in

    public class Direction
    {
        // Predefined directions
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(1, 0);

        // These fields represent the offsets for the row and column when moving in this direction
        public int RowOffset { get; set; }
        public int ColOffset { get; set; }

        // Constructor to prevent creation of new directions outside of the predefined ones
        private Direction(int rowOffset, int colOffset) 
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }

        // Returns the opposite direction
        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColOffset);
        }
        // generated equals/hash code methods
        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }
        // Overloads == and != operators to compare directions
        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
 
}
