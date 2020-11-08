using System;
using System.IO;
using System.Text;

namespace BattleShip
{
    class Program
    {

        private static int[,] table1;
        private static int[,] table2;


        static void PrintTable(int[,] table)
        {
            //Build the table header.
            Console.WriteLine();
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
                            Console.Write("barco" + "\t");
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

            table1[5, 5] = 2;

            //Initalize [table2].
            for (int i = 0; i < dimensions[0]; i++)
            {
                for (int j = 0; j < dimensions[1]; j++)
                {
                    table2[i, j] = table1[j, i];
                }
            }

            PrintTable(table1);
            PrintTable(table2);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("-------");
            SetupBoards();
        }

    }
}
