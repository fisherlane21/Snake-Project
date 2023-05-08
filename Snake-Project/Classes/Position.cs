
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Snake_Project
{
    public class Position
    {
        //Row: an int property that represents the row index of the position
        //Col: an int property that represents the column index of the position
        public int Row { get; set; }
        public int Col { get; set; }

        // constructor that uses row and column as parameters
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        // a method that returns a new Position object that is offset from the current position by the row and column offsets specified in the Direction object dir.

        public Position Translate(Direction dir)
        {
            return new Position(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        // checks if two Position objects are equal based on their row and column indices.
        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   Row == position.Row &&
                   Col == position.Col;
        }

        // method that returns a hash code for the Position object based on its row and column indices
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(Position left, Position right)
        {
            return EqualityComparer<Position>.Default.Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
