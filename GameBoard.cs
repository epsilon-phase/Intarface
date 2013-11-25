using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
namespace IntarFace
{
    public enum cellstate
    {
        empty = 0,
        red = 1,
        black = 2
    }

    public class GameBoard
    {
        int rscore = 0, bscore = 0;
         byte[][] byt;
        byte[] filledto = new byte[7];
        public GameBoard()
        {
            //initialize all to empty;
            byt = new byte[6][];
            for (int i = 0; i < 6; i++)
            {
                byt[i] = new byte[7];
                byt[i].Initialize();
            }

        }
        [System.Diagnostics.Contracts.Pure]
        
        private cellstate getCell(byte column, byte row)
        {
            return (cellstate)byt[row][column];
        }
        private void setCell(byte column, byte row, cellstate nstate)
        {
            byt[row][column] = (byte)nstate;
        }
        public bool canPlacePiece(int column)
        {
            return filledto[column] != 6;
        }
        [System.Diagnostics.Contracts.Pure]
        /// <summary>
        ///Provides a safe, if not interesting method for placing players as needed, does
        ///not mutate the original instance
        /// </summary>
        /// <param name="column">The column of the board(as indexed from 0)</param>
        /// <param name="player">The player to put here.</param>
        /// <returns></returns>
        public GameBoard placePiece(int column, cellstate player)
        {
            if (filledto[column] == 6)
                return null;
            GameBoard a = this;
            //don't do it, it's a trap.
            if (player != cellstate.empty)
            {
                a.setCell((byte)column, filledto[column], player);
                a.eval_parallel();
                a.filledto[column]++;
            }
            return a;

        }
        public int Red_Score
        {
            get
            {
                return rscore;
            }
        }
        public int Black_Score
        {
            get
            {
                return bscore;
            }
        }
        [System.Diagnostics.Contracts.Pure]
        /**
         * adapted from http://bretm.home.comcast.net/~bretm/hash/6.html
         * I obtained permission from the author.
         * 
         **/
        public override int GetHashCode()
        {
            byte[] data = new byte[42];
            //copy the array into one dimension
            for (int i = 0; i < 6; i++)
                byt[i].CopyTo(data, 7 * i);
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
        public bool isline(byte[] cell, out cellstate player)
        {
            bool found = false;

            byte col = cell[0], row = cell[1];
            cellstate initial = (cellstate)byt[col][row];
            if (initial == cellstate.empty)
            {
                player = cellstate.empty;
                return false;
            }
            bool up = row < 5 - 4,
                right = col <= 3,
                left = col >= 3;
            bool rightdiag = up && right;
            bool leftdiag = up && left;
            for (int i = 0; i < 4; i++)
            {
                if (up)
                {
                    if (initial != (cellstate)byt[i + row][col])
                        up = false;
                }
                if (left)
                {
                    if (initial != (cellstate)byt[row][col - i])
                        left = false;
                }
                if (right)
                    if (initial != (cellstate)byt[row][col + i])
                        right = false;
                if (rightdiag)
                    if (initial != (cellstate)byt[row + i][col + i])
                        rightdiag = false;
                if (leftdiag)
                    if (initial != (cellstate)byt[row + i][col - i])
                        leftdiag = false;
            }
            found = leftdiag || rightdiag || right || left || up;
            player = initial;
            return found;
        }
        /// <summary>
        /// Attempt to find the truth as to which player wins.
        /// </summary>
        public void eval_parallel()
        {
            System.Threading.Tasks.Parallel.For(0,3, j =>
            {
                for (byte i = 0; i < 7; i++)
                {
                    cellstate g;
                    var c = isline(new byte[] { (byte)j, i }, out g);
                    if (c)
                    {
                        if (g == cellstate.black)
                        {
                            bscore = 1000;
                            rscore = -5000;
                        }
                        else
                        {
                            rscore = 1000;
                            bscore = -5000;
                        }
                        return;
                    }
                    else
                    {
                        this.bscore = 5;
                        this.rscore = 5;
                    }
                }
            }
            );
        }

    }
}
