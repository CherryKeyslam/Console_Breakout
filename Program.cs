// C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:exe /out:atari.exe atari.cs

using System;

namespace Breakout {

	class Program {

	public static int width = 119;
        public static int height = 30;
        public static bool running = true;
        public static char[,] matrix = new char[height, width];
        private static readonly object guard = new object();

        public static char fill = '█';

        static void Main(string[] args)
        {
        	Console.CursorVisible = false;

        	for (int i = 0; i < height; i++)
            {
                for (int y = 0; y < width; y++)
                {
                    matrix[i, y] = ' ';
                }
            }

            for (int i = (0 + 50); i < (17 + 50); i++)
            {
                matrix[(height - 1), i] = fill;
                //█
            }

            string compiled = "";
            for (int i = 0; i < height; i++)
            {
                string line = "";
                for (int y = 0; y < width; y++)
                {
                    line += matrix[i, y];
                }
                compiled += line + '\n';
            }
            Console.WriteLine(compiled);

            int[] colours = {12,6,6,14,10,1};
            for(int i = 0; i < 6; i++) {
            	for(int x = 0; x < width; x++) {
            		Console.SetCursorPosition(x,i + 5);
            		Console.ForegroundColor = (ConsoleColor)colours[i];
            		Console.Write('█');
            		matrix[i + 5,x] = '░';
            	}
            }

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.SetCursorPosition(0,0);

            System.Threading.Thread keyReader = new System.Threading.Thread(Program.Input);
            keyReader.Start();

            System.Threading.Thread ball = new System.Threading.Thread(Program.Ball);
            ball.Start();

        	while(true) { Console.ReadKey(true); };
		}

		public static void display(int x, int y, char z) {
        	lock(guard) {
	        	matrix[y,x] = z;
	        	Console.SetCursorPosition(x,y);
	            Console.Write(z);
        	}
        }

        public static void Input() {
        	    Controller controller = new Controller();
            	string key = "na";

            	while(running) {
	                switch (Console.ReadKey(true).Key)
	                {
	                    case ConsoleKey.LeftArrow:
	                        key = "up";
	                        break;

	                    case ConsoleKey.RightArrow:
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
            int start = 0 + 50;
            int end = 16 + 50;
            int flr = height - 1;

            public void Update(string key)
            {
                switch(key)
                {
                    case "up":
                        if((start - 1) < 0)
                        {
                            return;
                        }
                        display(end, flr , ' ');
                        display((start - 1), flr, fill);
                        start--;
                        end--;
                    break;

                    case "dn":
                        if ((end + 1) > (width-1))
                        {
                            return;
                        }
                        display(start, flr, ' ');
                        display((end + 1),flr, fill);
                        start++;
                        end++;
                    break;
                }
            }
        }
        
        public static void Ball() {
        	int x = width / 2;
        	int y = height / 2;

        	int angle = 1;
        	int riglef = 1;
			int store = 0;
			int direction = -1;
			bool victory = false;

			int speed = 150;

        	int forceX = direction;
        	int forceY = riglef;

        	System.Collections.Generic.Dictionary<int,int> lookup = new System.Collections.Generic.Dictionary<int,int>() {
				{0,0},
				{1,0},
				{2,1},
				{3,1},
				{4,1},
				{5,1},
				{6,1},
				{7,2},
				{8,2},
			};

        	while(running) {

        		display(x, y, ' ');

        		if(((x + forceX) < 0) || (x + forceX) > (width - 1)) {
        			riglef *= -1;
        		} else {
	        		switch(matrix[y,(x + forceX)]) {
	        			case '█':
	        				riglef *= -1;

	        			break;

	        			case '░':
	        				//matrix[y,x + forceX] = ' ';
	        				display((x + forceX), y, ' ');
	        				riglef *= -1;
	        			break;
	        		}
        		}

        		if((y + forceY) > (height - 1)) {
        			running = false;
        			break;
        		} else if((y + forceY) < 0) {
        			store = 0;
        			direction *= -1;
        		} else {
	        		switch(matrix[y + forceY, x]) {
	        			case '█':
	        				float dep = 0f;
	        				bool walking = true;
	        				while(walking) {
	        					if((x + (int)dep) > (width - 1)) {
	        						walking = false;
	        					}
	        					else if(matrix[(y + forceY) , (x + (int)dep)] == '█') {
	        						dep += 1f;
	        					} else {
	        						walking = false;
	        					}
	        				}

	        				int centreDist = Math.Abs((int)((17f - dep) - 8f));

	        				angle = lookup[centreDist];

	        				if(angle == 0) {
	        					riglef = 0;
	        					speed = 50;
	        				} else {

	        					if(angle == 1) {
	        						speed = 150;
	        					} else {
	        						speed = 100;
	        					}
	        					if(dep < 9) {
	        						riglef = 1;
	        					} else {
	        						riglef = -1;
	        					}
	        				}

	        				store = 0;
	        				direction *= -1;
	        			break;

	        			case '░':
	        				display(x, (y + forceY), ' ');
	        				store = 0;
	        				direction *= -1;
	        			break;
	        		}
        		}
        		
        		//Angle-inator:

        		if(store == (angle*riglef)) {
					store = 0;
				}
				if(Math.Abs(store) > 0) {
					forceX = riglef;
					forceY = 0;
					store += riglef;
				} else {
					forceY = direction;
					forceX = riglef;
					store += riglef;
				}

        		x = x + forceX;
        		y = y + forceY;
	        	display(x, y, 'O');
	        	System.Threading.Thread.Sleep(speed);
        	}

        	switch(victory) {
        		case false:
        			Console.Clear();
        			Console.SetCursorPosition(0,0);
        			string losing_text = @"








                    ██╗   ██╗ █████╗ ██╗   ██╗                ██╗      █████╗  ██████╗████████╗██╗
                    ╚██╗ ██╔╝██╔══██╗██║   ██║                ██║     ██╔══██╗██╔════╝╚══██╔══╝██║
                     ╚████╔╝ ██║  ██║██║   ██║                ██║     ██║  ██║╚█████╗    ██║   ██║
                      ╚██╔╝  ██║  ██║██║   ██║                ██║     ██║  ██║ ╚═══██╗   ██║   ╚═╝
                       ██║   ╚█████╔╝╚██████╔╝                ███████╗╚█████╔╝██████╔╝   ██║   ██╗
                       ╚═╝    ╚════╝  ╚═════╝                 ╚══════╝ ╚════╝ ╚═════╝    ╚═╝   ╚═╝
        			";
					Console.SetCursorPosition(0,0);
					Console.WriteLine(losing_text);
        		break;

        	}
        }
	}
}
