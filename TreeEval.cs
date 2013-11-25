using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntarFace
{
    /// <summary>
    /// A class used to generate as many boardstates as possible and to maximize the score;
    /// </summary>
    public class TreeEval
    {
        /**
         * The weakreference wrapper allows for the garbage collector to obtain it without fear of reprisal,
         * so essentially, the table's memory will be all that is left
         * */
        Dictionary<GameBoard, WeakReference> collected_nodes;
        node current = null;
        bool isplayingred = false;
        public TreeEval(bool isred)
        {
            collected_nodes = new Dictionary<GameBoard, WeakReference>();
            isplayingred = isred;
            current = new node(new GameBoard(), 3, ref collected_nodes,
                //due to the overwhelmingly simple logic of my function, the node before the first move must be black.
                cellstate.black,//if the player is red, then say so.
                isred ? cellstate.red : cellstate.black);


        }
        public void movetoGameBoard(GameBoard c)
        {
            if (collected_nodes.ContainsKey(c))
            {
                current = (node)collected_nodes[c].Target;
            }
            else
                throw new KeyNotFoundException("No Node found with state");
        }
        public int getBestMove()
        {
            return current.bestMove;
        }
        class node
        {
            /// <summary>
            /// The player that has just made a move.
            /// </summary>
            cellstate currentplayer;
            /// <summary>
            /// The player which the score is maximized for.
            /// </summary>
            readonly cellstate pas;
            long rscore = 0, bscore = 0;
            List<node> children;
            GameBoard ac;
            node maxscore = null;
            int movetomax = 0;
            public int bestMove
            {
                get
                {
                    return movetomax;
                }
            }
            public void recalculateAtDepth(int gendepth, ref Dictionary<GameBoard, WeakReference> h)
            {
                h = this.GenerateChildren(this.ac, gendepth, h);
            }

            public node(GameBoard g, int generation_depth, ref Dictionary<GameBoard, WeakReference> h, cellstate d = cellstate.red, cellstate play = cellstate.black)
            {
                ac = g;
                currentplayer = d;
                rscore = g.Red_Score;
                bscore = g.Black_Score;
                children = new List<node>(7);
                pas = play;
                h = GenerateChildren(g, generation_depth, h);
            }

            private Dictionary<GameBoard, WeakReference> GenerateChildren(GameBoard g, int generation_depth, Dictionary<GameBoard, WeakReference> h)
            {
                if (g.Black_Score < 1000 && g.Black_Score > -1000 && generation_depth > 0)
                    for (byte c = 0; c < 7; c++)
                    {
                        if (ac.canPlacePiece(c))
                        {
                            GameBoard quod;
                            if (currentplayer == cellstate.red)
                            {
                                quod = ac.placePiece(c, cellstate.black);
                                children.Add(new node(quod, generation_depth - 1, ref h, cellstate.black, pas));
                            }
                            else
                            {
                                quod = ac.placePiece(c, cellstate.red);

                                children.Add(new node(quod, generation_depth - 1, ref h, cellstate.red, pas));
                            }
                            //if the current player is making the move, then it seems more relevant.
                            if (pas != currentplayer)
                                h.Add(quod, new WeakReference(this));

                            rscore += children[c].rscore;
                            bscore += children[c].bscore;
                            if (maxscore == null)
                                maxscore = children[c];
                            if (pas == cellstate.black)
                            {
                                if (children[c].bscore > maxscore.bscore)
                                {
                                    maxscore = children[c];
                                    movetomax = c;
                                }

                            }
                            else
                                if (children[c].rscore > maxscore.rscore)
                                {
                                    maxscore = children[c];
                                    movetomax = c;
                                }
                        }
                        else
                        {
                            children.Add(null);
                        }
                    }
                return h;
            }
        }
    }
}
