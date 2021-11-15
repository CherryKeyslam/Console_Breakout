using System;

namespace Ping_Pong
{
    class Program
    {
        public static int width = 119;
        public static int height = 29;
        public static string[,] matrix = new string[height, width];
        static void Main(string[] args)
        {
            bool lost = false;
            for (int i = 0; i < height; i++)
            {
                for (int y = 0; y < width; y++)
                {
                    matrix[i, y] = " ";
                }
            }

            for (int i = 0; i < 8; i++)
            {
                matrix[i, 0] = "█";
            }

            for (int i = 0; i < height; i++)
            {
                matrix[i, (width - 1)] = "█";
            }

            Controller controller = new Controller();
            string key;
            key = "na";

            while (true)
            {
                Console.Clear();
                controller.Update(key);
                display();
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        key = "up";
                        break;

                    case ConsoleKey.DownArrow:
                        key = "dn";
                        break;

                    default:
                        key = "na";
                        break;
                }
            }

            Console.ReadKey();
        }

        public static void display()
        {
            for (int i = 0; i < height; i++)
            {
                string line = "";
                for (int y = 0; y < width; y++)
                {
                    line += matrix[i, y];
                }
                Console.WriteLine(line);
            }
        }
        public class Controller
        {
            int start = 0;
            int end = 7;

            public void Update(string key)
            {
                switch(key)
                {
                    case "up":
                        if((start - 1) < 0)
                        {
                            return;
                        }
                        matrix[end, 0] = " ";
                        matrix[(start - 1), 0] = "█";
                        start--;
                        end--;
                    break;

                    case "dn":
                        if ((end + 1) > (height-1))
                        {
                            return;
                        }
                        matrix[start, 0] = " ";
                        matrix[(end + 1), 0] = "█";
                        start++;
                        end++;
                    break;
                }
            }
        }
    }
}
