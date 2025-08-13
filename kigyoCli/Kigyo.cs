using Timer = System.Timers.Timer;

namespace kigyoCli;

public class Kigyo
{
    private const int X = 50;
    private const int Y = 20;
    private const int InitialLength = 5;
    private const double InitialSpeed = 200;

    private readonly int[,] _gameArea;
    private readonly Timer _timer;
    private (int y, int x) _food;

    private (int y, int x) _head;

    private ConsoleKey _keyPressed;

    public Kigyo()
    {
        _gameArea = IntializeGameArea(Y, X);
        _food = (-1, -1);
        _timer = new Timer(InitialSpeed);
        _timer.Elapsed += (_, __) => Refresh();
    }

    private void Refresh()
    {
        Console.Clear();

        var isGameGoing = AddFood();

        if (!isGameGoing) EndGame(true);

        ProcessMovement();

        var score = $"\u256d Score: {_gameArea[_head.y, _head.x]} Speed: {_timer.Interval} ";
        score = score.PadRight(X, '\u2501');
        score = score.PadRight(X+1, '\u256e');
        Console.WriteLine(score);
        for (var i = 0; i < Y - 1; i++)
        {
            Console.Write("\u2503");
            for (var ii = 0; ii < X - 1; ii++)
            {
                var cellValue = _gameArea[i, ii];

                switch (cellValue)
                {
                    case 0:
                        Console.Write(" ");
                        break;
                    case -1:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\u254b");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\u2588");
                        break;
                }

                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.Write("\u2503");
            Console.WriteLine();
        }
        
        var footer = $"\u2570 Food is at {_food.y}, {_food.x} ";
        footer = footer.PadRight(X, '\u2501');
        footer = footer.PadRight(X+1, '\u256f');
        Console.WriteLine(footer);
    }

    private bool AddFood()
    {
        if (_food.x != -1 && _food.y != -1)
            return true;
        var freeCells = new List<(int y, int x)>();

        for (var j = 0; j < Y - 1; j++)
        for (var k = 0; k < X - 1; k++)
            if (_gameArea[j, k] == 0)
                freeCells.Add((j, k));

        if (freeCells.Count == 0)
            return false;

        _food = freeCells.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();

        _gameArea[_food.y, _food.x] = -1;

        return true;
    }

    private void ProcessMovement()
    {
        var newHead = (_head.y, _head.x);
        switch (_keyPressed)
        {
            case ConsoleKey.LeftArrow:
                newHead.x--;
                break;
            case ConsoleKey.RightArrow:
                newHead.x++;
                break;
            case ConsoleKey.UpArrow:
                newHead.y--;
                break;
            case ConsoleKey.DownArrow:
                newHead.y++;
                break;
            default:
                newHead.x--;
                break;
        }

        if (newHead.x < 0 || newHead.x == X - 1 || newHead.y < 0 || newHead.y == Y - 1)
        {
            EndGame(false);
            return;
        }

        if (_gameArea[newHead.y, newHead.x] > 0)
        {
            EndGame(false);
            return;
        }

        var isSnakeEating = _gameArea[newHead.y, newHead.x] == -1;

        _gameArea[newHead.y, newHead.x] = _gameArea[_head.y, _head.x] + 1;
        _head.x = newHead.x;
        _head.y = newHead.y;

        if (isSnakeEating)
        {
            _food = (-1, -1);
            _timer.Interval *= .9;
        }
        else
        {
            DevaluateMatrix();
        }
    }

    private void EndGame(bool isWon)
    {
        _timer.Stop();
        Console.Clear();
        if (isWon)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You won. High score: {_gameArea[_head.y, _head.x]}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Game over. High score: {_gameArea[_head.y, _head.x]}");
        }
    }

    private void DevaluateMatrix()
    {
        for (var j = 0; j < Y; j++)
        for (var k = 0; k < X; k++)
        {
            if (_gameArea[j, k] < 1) continue;

            _gameArea[j, k]--;
        }
    }

    private int[,] IntializeGameArea(int i, int ii)
    {
        var matrix = new int[i, ii];
        for (var j = 0; j < i; j++)
        for (var k = 0; k < ii; k++)
            matrix[j, k] = 0;

        var centerY = i / 2;
        var centerX = ii / 2;

        for (var snakeBody = 0; snakeBody <= InitialLength; snakeBody++) matrix[centerY, centerX + snakeBody] = InitialLength - snakeBody;

        _head = (centerY, centerX);

        return matrix;
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Move(ConsoleKey key)
    {
        _keyPressed = key;
        Refresh();
        _timer.Stop();
        _timer.Start();
    }
}