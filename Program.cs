// C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:exe /out:pong.exe pong.cs

using System;

namespace Ping_Pong
{
    class Program
    {
        public static int width = 119;
        public static int height = 29;
        public static bool notended = true;
        public static string[,] matrix = new string[height, width];
        static void Main(string[] args)
        {
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
                matrix[i, (width - 1)] = "░";
            }

            
            System.Threading.Thread keyReader = new System.Threading.Thread(Program.Input);
            keyReader.Start();

            System.Threading.Thread ball = new System.Threading.Thread(Program.Ball);
            ball.Start();

            while (notended)
            {
                Console.Clear();
                display();
                System.Threading.Thread.Sleep(500);
            }
            Console.ReadKey();
        }

        //-----------------------------------------------------------------------------

        public static void Ball() {
        	int x = width / 2;
        	int y = height / 2;

        	int forceX = 0;
        	int forceY = 0;
        	int angle = 1;
        	int updown = -1;
			int store = 0;
			int direction = -1;

        	while(true) {
        		matrix[y,x] = " ";


        		if((x + forceX) < 0) {
        			notended = false;
        		} else if((x + forceX) > (width - 1)) {
        			direction *= 1;
        		} else {
	        		switch(matrix[y,x + forceX]) {
	        			case "█":
	        				direction *= -1;
	        			break;

	        			case "░":
	        				matrix[y,x + forceX] = " ";
	        				direction *= -1;
	        			break;
	        		}
        		}

        		if((y + forceY) < 0 || (y + forceY) > (height - 1)) {
        			store = 0;
        			updown *= -1;
        		} else {
	        		switch(matrix[y + forceY, x]) {
	        			case "█":
	        				direction *= -1;
	        			break;

	        			case "░":
	        				matrix[y + forceY, x] = " ";
	        				updown *= -1;
	        			break;
	        		}
        		}
        		
        		if(Math.Abs(store) > 0) {
					forceY = updown;
					forceX = 0;
					if(store == (angle*updown)) {
						store = 0;
					} else {
						store += updown;
					}
				} else {
					forceX = direction;
					forceY = 0;
					store += updown;
				}

        		x = x + forceX;
        		y = y + forceY;
	        	matrix[y, x] = "O";
	        	System.Threading.Thread.Sleep(200);
        	}
        }

        //-----------------------------------------------------------------------------

        public static void display()
        {
        	string compiled = "";
            for (int i = 0; i < height; i++)
            {
                string line = "";
                for (int y = 0; y < width; y++)
                {
                    line += matrix[i, y];
                }
                compiled += line + "\n";
            }
            Console.WriteLine(compiled);
        }

        public static void Input() {
        	    Controller controller = new Controller();
            	string key = "na";

            	while(true) {
	                switch (Console.ReadKey(true).Key)
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
	                controller.Update(key);
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
