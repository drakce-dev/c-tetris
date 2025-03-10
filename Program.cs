using System.Diagnostics;
using System.Threading;
using Tetris;

static void WaitForInput(Game game)
{
    TimeSpan dropTime = TimeSpan.FromSeconds(Math.Max(3 - game.Lines / 10, 0.25));
    Stopwatch stopwatch = Stopwatch.StartNew();
    bool breakLoop = false;
    while (stopwatch.Elapsed < dropTime)
    {
        if (!Console.KeyAvailable) Thread.Sleep(25);
        else
        {
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.DownArrow:
                    game.MoveDown();
                    game.PrintState();
                    breakLoop = true;
                    break;
                case ConsoleKey.LeftArrow:
                    game.MoveLeft();
                    game.PrintState();
                    break;
                case ConsoleKey.RightArrow:
                    game.MoveRight();
                    game.PrintState();
                    break;
                case ConsoleKey.Spacebar:
                    game.FallDown();
                    breakLoop = true;
                    break;
                case ConsoleKey.UpArrow:
                    game.Rotate();
                    game.PrintState();
                    break;
                case ConsoleKey.C:
                    if (game.Hold())
                        breakLoop = true;
                    break;
            }
            if (breakLoop) break;
        }
    }
    stopwatch.Stop();
    game.NextState(!breakLoop);
}

Game game = new Game();
game.Print();

while (!game.IsGameOver())
{
    game.PrintState();
    WaitForInput(game);
}
