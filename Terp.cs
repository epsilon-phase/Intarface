using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntarFace
{

    class Terp
    {
        public void run()
        {
            TreeEval g;
            string current;
            Random e = new Random();
            var player = 0;
            int.TryParse(Console.ReadLine(), out player);
            GameBoard gamestate = new GameBoard();
            if (player == 1)
            {
                g = new TreeEval(true);
                Console.WriteLine(g.getBestMove() + 1);
                gamestate = gamestate.placePiece(g.getBestMove(), cellstate.red);
            }
            else
            {
                g = new TreeEval(false);
                Console.WriteLine("?");

            }
            cellstate opponent = player == 1 ? cellstate.red : cellstate.black;
            while (true)
            {
                current = Console.ReadLine();
                int i;
                int.TryParse(current, out i);
                gamestate = gamestate.placePiece(i - 1, opponent);
                g.movetoGameBoard(gamestate);
            }

        }
    }
}
