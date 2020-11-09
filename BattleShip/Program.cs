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
        private static int turn = 0;
        private static int failedAttempts = 0;
        private static int totalShipSlots = 0;
        private static int totalPlayerHits = 0;
        private static int totalMachineHits = 0;
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
                    totalShipSlots++;
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

        static bool IsGameFinished()
        {
            int playerShipSlots = 0;
            int machineShipSlots = 0;

            //Check failed attempts.
            if(failedAttempts >= 3)
            {
                return true;
            }

            //Check if there is no more ships on the player table.
            for (int i = 0; i < table1.GetLength(1); i++)
            {
                for (int j = 0; j < table1.GetLength(0); j++)
                {
                    if(table1[j, i] == 1)
                    {
                        playerShipSlots++;
                    }
                }
            }
            if(playerShipSlots == 0)
            {
                return true;
            }

            //Check if there is no more ships on the machine table.
            for (int i = 0; i < table2.GetLength(1); i++)
            {
                for (int j = 0; j < table2.GetLength(0); j++)
                {
                    if (table2[j, i] == 1)
                    {
                        machineShipSlots++;
                    }
                }
            }
            if (machineShipSlots == 0)
            {
                return true;
            }

            return false;
        }

        static void ShowResults()
        {
            //Show the final status of the tables
            Console.Clear();
            Console.WriteLine("JUEGO FINALIZADO");
            Console.WriteLine("JUGADOR");
            PrintTable(table1, true);
            Console.WriteLine();
            Console.WriteLine("OPONENTE");
            PrintTable(table2, true);
            Console.WriteLine();

            //Show the results
            Console.WriteLine("RESULTADOS");
            Console.WriteLine("Turnos: {0}", turn);
            double playerHitsPercentage = Math.Round(Convert.ToDouble(totalPlayerHits) / Convert.ToDouble(totalShipSlots) * 100, 2);
            double machineHitsPercentage = Math.Round(Convert.ToDouble(totalMachineHits) / Convert.ToDouble(totalShipSlots) * 100, 2);

            Console.WriteLine("Aciertos jugador: {0}%", playerHitsPercentage);
            Console.WriteLine("Aciertos oponente: {0}%", machineHitsPercentage);
        }

        static void InitGameFlow()
        {
            while (activePhase != GamePhase.finished)
            {
                turn++;
                int[] move;
                int value;

                //Show the status of the tables
                Console.Clear();
                Console.WriteLine("JUGADOR");
                PrintTable(table1, true);
                Console.WriteLine();
                Console.WriteLine("OPONENTE");
                PrintTable(table2, false);
                Console.WriteLine();

                //Decide which action to perform based on the active phase
                switch (activePhase)
                {
                    case GamePhase.player:
                        Console.WriteLine("Turno " + turn.ToString());
                        Console.Write("Su turno, ingrese coordenada: ");

                        string input = Console.ReadLine();

                        if(input.ToUpper() == "X")
                        {
                            activePhase = GamePhase.finished;
                        } else
                        {

                            try
                            {
                                move = Array.ConvertAll(input.Split(','), int.Parse);
                                value = table2[move[1] - 1, move[0] - 1];
                                switch (value)
                                {
                                    case 0:
                                        failedAttempts++;
                                        break;
                                    case 1:
                                        totalPlayerHits++;
                                        break;
                                }

                                //Validate the slot is not already used.
                                if (value <= 1)
                                {
                                    table2[move[1] - 1, move[0] - 1] = 2;

                                    if (!IsGameFinished())
                                    {
                                        activePhase = GamePhase.machine;
                                    }
                                    else
                                    {
                                        activePhase = GamePhase.finished;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Esta coordenada ya ha sido ocupada.");
                                    turn--;
                                }
                            } catch
                            {
                                Console.WriteLine("No ha ingresado una coordenada válida.");
                                turn--;
                            }
                            

        
                        }

                        break;
                    case GamePhase.machine:
                        move = machineMoves[0];
                        machineMoves.RemoveAt(0);
                        value = table1[move[0], move[1]];
                        if (value == 1)
                        {
                            totalMachineHits++;
                        }
                        table1[move[0], move[1]] = 2;

                        Console.WriteLine("Turno del oponente, ha seleccionado la coordenada: {0},{1}", move[1] + 1, move[0] + 1);

                        if (!IsGameFinished())
                        {
                            activePhase = GamePhase.player;
                        }
                        else
                        {
                            activePhase = GamePhase.finished;
                        }

                        activePhase = GamePhase.player;
                        break;
                }
            }

            ShowResults();
        }

        static void Main(string[] args)
        {
            SetupBoards();
            ShuffleMoves();
            InitGameFlow();
        }

    }
}
