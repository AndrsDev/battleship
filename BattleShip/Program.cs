using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

enum GamePhase { player, machine, finished };

namespace BattleShip
{
    class Program
    {

        private static int[,] table1;
        private static int[,] table2;
        private static int turn = 1;
        private static GamePhase activePhase = GamePhase.player;
        private static List<int[]> machineMoves = new List<int[]>();


        static void PrintTable(int[,] table, bool showShips = true)
        {
            //Build the table header.
            Console.Write("\t");
            for (int i = 0; i < table.GetLength(0); i++)
            {
                Console.Write("  " + (i + 1).ToString() + "  \t");
            }
            Console.WriteLine();


            //Build the table rows.
            for (int i = 0; i < table.GetLength(1); i++)
            {
                Console.Write((i + 1).ToString() + "\t");
                for (int j = 0; j < table.GetLength(0); j++)
                {

                    switch (table[j,i])
                    {
                        case 0:
                            Console.Write("  -  " + "\t");
                            break;
                        case 1:
                            if (showShips)
                            {
                                Console.Write("barco" + "\t");
                            } else
                            {
                                Console.Write("  -  " + "\t");
                            }
                            break;
                        default:
                            Console.Write("  X  " + "\t");
                            break;
                    }
                }
                Console.WriteLine();
            }


        }

        static void SetupBoards()
        {
            //Read the configuratino file.
            var path = "/Users/andrs/Documents/Tutoring/BattleShip/BattleShip/data.txt";
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            int[] dimensions = Array.ConvertAll(lines[0].Split(','), int.Parse);

            table1 = new int[dimensions[1], dimensions[0]];
            table2 = new int[dimensions[0], dimensions[1]];

            //Initialize [table1].
            for (int i = 2; i < lines.Length; i++)
            {
                string[] ship = lines[i].Split(',');

                int x = int.Parse(ship[1]) - 1;
                int y = int.Parse(ship[0]) - 1;
                string shipOrientation = ship[2];
                int shipLength = int.Parse(ship[3]);

                for (int j = 0; j < shipLength; j++)
                {
                    if (shipOrientation == "H")
                    {
                        table1[x + j, y] = 1;
                    } else
                    {
                        table1[x, y + j] = 1;
                    }
                }

            }

            //Initalize [table2] and save [machine] possible moves.
            for (int i = 0; i < dimensions[0]; i++)
            {
                for (int j = 0; j < dimensions[1]; j++)
                {
                    table2[i, j] = table1[j, i];
                    int[] move = { j, i };
                    machineMoves.Add(move);
                }
            }
        }


        public static void ShuffleMoves()
        {
            Random rng = new Random();
            int n = machineMoves.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int[] value = machineMoves[k];
                machineMoves[k] = machineMoves[n];
                machineMoves[n] = value;
            }
        }

        static void InitGameFlow()
        {
            while (activePhase != GamePhase.finished)
            {
                int[] move;

                Console.Clear();
                Console.WriteLine("JUGADOR");
                PrintTable(table1, true);
                Console.WriteLine();
                Console.WriteLine("OPONENTE");
                PrintTable(table2, false);
                Console.WriteLine();

                switch (activePhase)
                {
                    case GamePhase.player:
                        Console.WriteLine("Turno " + turn.ToString());
                        Console.Write("Su turno, ingrese coordenada: ");
                        move = Array.ConvertAll(Console.ReadLine().Split(','), int.Parse);
                        table2[move[1] - 1, move[0] - 1] = 2;
                        activePhase = GamePhase.machine;

                        break;
                    case GamePhase.machine:
                        move = machineMoves[0];
                        machineMoves.RemoveAt(0);
                        table1[move[0], move[1]] = 2;
                        Console.WriteLine("Turno del oponente, ha seleccionado la coordenada: {0},{1}", move[1] + 1, move[0] + 1);
                        activePhase = GamePhase.player;
                        break;
                }

                turn++;

            }
        }

        static void Main(string[] args)
        {
            SetupBoards();
            ShuffleMoves();
            InitGameFlow();

        }

    }
}
