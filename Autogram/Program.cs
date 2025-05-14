// See https://aka.ms/new-console-template for more information
Console.OutputEncoding = System.Text.Encoding.UTF8;

const int AlphabetSize = 25;
var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();
var autogram = new Autogram.Autogram(alphabet);

Console.WriteLine("Starting: " + autogram.ToString());

int i = 0;

while (true)
{
    i++;
    var status = autogram.Iterate();

    if (i % 100000 == 0 || status.Success)
    {
        Console.WriteLine("Iteration: " + i + "\tHistory: " + status.HistoryCount + "\t" + status.CurrentString);

        var diffs = status.GuessError;

        foreach (var y in diffs)
        {
            if (y == 0)
            {
                Console.Write("-");
            }
            else if (y < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Math.Abs(y));
                Console.ResetColor();
            }
            else if (y > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(Math.Abs(y));
                Console.ResetColor();
            }
        }

        Console.ResetColor();

        Console.WriteLine("\tMismatches: " + diffs.Count(p => p != 0) + "\tTotal distance: " + diffs.Sum(Math.Abs) + (status.RandomReset ? "\tRandomized 🎲" : ""));
    }

    if (status.Success)
    {
        break;
    }
}

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Finished: " + autogram.ToString());
Console.ResetColor();

Console.ReadKey();