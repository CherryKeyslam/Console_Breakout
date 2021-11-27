/// C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /t:exe /out:atari.exe atari.cs

using System;

namespace Breakout {

	class Program {

		public static int width = 120;
        public static int height = 31;
        public static bool running = true;
        public static char[,] matrix = new char[height, width];
        private static readonly object guard = new object();

        static void Main(string[] args)
        {
        	Console.CursorVisible = false;

        	Console.SetWindowSize(Console.WindowWidth + 1, Console.WindowHeight);

        	for (int i = 0; i < height; i++)
            {
                for (int y = 0; y < width; y++)
                {
                    matrix[i, y] = ' ';
                }
            }

            for (int i = (0 + 53); i < (15 + 53); i++)
            {
                matrix[(height - 1), i] = '█';
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

            // Level builder:

            int[] colours = {12,12,9,9,10,10,14,14};
            bool block_pattern = true;
            for(int i = 0; i < 8; i++) {

            	char fill1;
            	char fill2;

            	if(block_pattern) {
            		fill1 = '░';
            		fill2 = '▒';
            		block_pattern = false;
            	} else {
            		fill1 = '▒';
            		fill2 = '░';
            		block_pattern = true;	
            	}

            	int blocks = -1;
            	for(int x = 0; x < width; x++) {
            		blocks++;
            		Console.SetCursorPosition(x,i + 5);
            		Console.ForegroundColor = (ConsoleColor)colours[i];
            		if(blocks < 8) {
            			matrix[i + 5,x] = fill1;
            			Console.Write(fill1);
            		} else {
            			matrix[i + 5,x] = fill2;
            			Console.Write(fill2);
            		}

            		if(blocks == 15) {
            			blocks = -1;
            		}
            	}
            }

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.SetCursorPosition(0,0);

            System.Threading.Thread ball = new System.Threading.Thread(Program.Ball);
            ball.Start();

            int start = 0 + 53;
            int end = 14 + 53;
            int flr = height - 1;

            // Controller: 

            while(running) {
            	switch (Console.ReadKey(true).Key)
	                {
	                    case ConsoleKey.LeftArrow:
	                        if((start - 1) < 0)
                        	{
                            	continue;
                        	}
	                        display(end, flr, ' ');
	                        display((start - 1), flr, '█');
	                        start--;
	                        end--;
	                    break;

	                    case ConsoleKey.RightArrow:
	                        if ((end + 1) > (width-1))
	                        {
	                            continue;
	                        }
	                        display(start, flr, ' ');
	                        display((end + 1),flr, '█');
	                        start++;
	                        end++;
	                    break;
	                }
            }

        	while(true) { Console.ReadKey(true); };
		}

		public static void display(int x, int y, char z) {
			matrix[y,x] = z;
        	lock(guard) {
	        	Console.SetCursorPosition(x,y);
	            Console.Write(z);
	            // To fix blue line glitch:
	            Console.SetCursorPosition(0,30);
        	}
        }
        public static void DeleteBlock(int x, int y) {
			int in_x = 0;
			bool walking = true;
			while(walking) {
				if((x + in_x) > (width - 1)) {
					walking = false;
				}
				else if(matrix[y , (x + in_x)] == matrix[y, x]) {
					in_x += 1;
				} else {
					walking = false;
				}
			}

			in_x += x - 1;

			for (int i = 0; i < 8; i++) {
				display((in_x - i),y,' ');
			}
		}
        
        public static void Ball() {
        	int score = 0;

        	for(int turns = 0; turns < 3; turns++) {
	        	int x = width / 2;
	        	int y = height / 2;

	        	display(x, y, 'O');


	        	int angle = 0;
	        	int riglef = 0;
				int store = 0;
				int direction = 1;

				int speed = 50;

	        	int forceX = direction;
	        	int forceY = riglef;

	        	System.Collections.Generic.Dictionary<int,int> score_lookup = new System.Collections.Generic.Dictionary<int,int>() {
					{5,7}, {6,7},
					{7,5}, {8,5},
					{9,3}, {10,3},
					{11,1}, {12,1}
				};

				System.Collections.Generic.Dictionary<int,int> angle_lookup = new System.Collections.Generic.Dictionary<int,int>() {
					{0,0},
					{1,1},
					{2,1},
					{3,1},
					{4,1},
					{5,1},
					{6,1},
					{7,2},
					{8,2}
				};

				System.Threading.Thread.Sleep(1500);

	        	while(running) {

	        		display(x, y, ' ');
	        		
	        		// This giant if condition checks that the next move is not out of bounds and if the ball is in a corner or on a diagonal.

	        		if(!(((x + forceX) < 0) || ((x + forceX) == width) || ((y + forceY) < 0) || ((y + forceY) == height)) && (((matrix[y + forceY,x + forceX] != ' ') && (matrix[y,x + forceX] != ' ') && (matrix[y + forceY,x] != ' ') || ((matrix[y,x + forceX] == ' ') && (matrix[y + forceY,x] == ' ') && (matrix[y + forceY,x + forceX] != ' '))))) {
	        			riglef *= -1;
	        			direction *= -1;
	        			store = 0;

	        			if((matrix[(y + forceY),(x + forceX)] != ' ') && ((matrix[y,(x + forceX)] != ' ') && (matrix[(y + forceY),x] != ' '))) {
	        				DeleteBlock((x + forceX),y);

	    					DeleteBlock(x,(y + forceY));

	    					DeleteBlock((x + forceX),(y + forceY));
	        			} else {
	        				DeleteBlock((x + forceX),(y + forceY));
	        			}


	        		} else {
		        		if(((x + forceX) < 1) || (x + forceX) == width) {
		        			riglef *= -1;
		        		} else {
			        		switch(matrix[y,(x + forceX)]) {
			        			case '█':
			        				angle = 3;
			        				store = 0;
			        				direction = -1;
			        				riglef *= -1;

			        			break;

			        			case '░':
			        			case '▒':

			        			score += score_lookup[y];

			    				DeleteBlock((x + forceX),y);



			    				riglef *= -1;

			        			break;
			        		}
		        		}

		        		if((y + forceY) == height && direction != -1) {
		        			break;
		        		} else if((y + forceY) < 1) {
		        			store = 0;
		        			direction *= -1;
		        		} else {
			        		switch(matrix[y + forceY, x]) {
			        			case '█':
			        				float dep = 0f;
			        				bool walking = true;
			        				while(walking) {
			        					if((x + (int)dep) == width) {
			        						walking = false;
			        					}
			        					else if(matrix[(y + forceY) , (x + (int)dep)] == '█') {
			        						dep += 1f;
			        					} else {
			        						walking = false;
			        					}
			        				}

			        				int centreDist = Math.Abs((int)((15f - dep) - 7f));

			        				angle = angle_lookup[centreDist];

			        				if(angle == 0) {
			        					riglef = 0;
			        					speed = 50;
			        				} else {

			        					if(angle == 1) {
			        						speed = 80;
			        					} else {
			        						speed = 50;
			        					}
			        					if(dep < 8) {
			        						riglef = 1;
			        					} else {
			        						riglef = -1;
			        					}
			        				}

			        				store = 0;
			        				direction *= -1;
			        			break;

			        			case '░':
			        			case '▒':

			        				score += score_lookup[y + forceY];

			        				DeleteBlock(x,(y + forceY));

			        				store = 0;
			        				direction *= -1;
			        			break;
			        		}
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
	        }

	        running = false;

			Console.Clear();
			Console.SetCursorPosition(0,0);
			string losing_text = @"







					 ██████╗  █████╗ ███╗   ███╗███████╗
					██╔════╝ ██╔══██╗████╗ ████║██╔════╝
					██║  ██╗ ███████║██╔████╔██║█████╗  
					██║  ╚██╗██╔══██║██║╚██╔╝██║██╔══╝  
					╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗
					 ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝
					
					  █████╗ ██╗   ██╗███████╗██████╗ 
					 ██╔══██╗██║   ██║██╔════╝██╔══██╗
					 ██║  ██║╚██╗ ██╔╝█████╗  ██████╔╝
					 ██║  ██║ ╚████╔╝ ██╔══╝  ██╔══██╗
					 ╚█████╔╝  ╚██╔╝  ███████╗██║  ██║
					  ╚════╝    ╚═╝   ╚══════╝╚═╝  ╚═╝

					  	    (Score = " + score + @")
			";
			Console.SetCursorPosition(0,0);
			Console.WriteLine(losing_text);
        }
	}
}
