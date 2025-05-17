// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using Autogram;

Console.OutputEncoding = System.Text.Encoding.UTF8;

const int AlphabetSize = 25;
int? seed = 1001;

var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();
var autogram = new Autogram.AutogramBytesNoStrings(alphabet, seed);

Console.WriteLine("Starting: " + autogram.ToString());

int i = 0;

// var topGuesses = new List<Status>();

var sw = new Stopwatch();
sw.Start();

while (true)
{
    i++;
    var status = autogram.Iterate();

    //var diffs = status.GuessError;

    //var totalDistance = status.TotalDistance;

    //if (topGuesses.Count < 1000)
    //{
    //    topGuesses.Add(status);
    //}
    //else
    //{
    //    Status worst = null;
    //    foreach (var item in topGuesses)
    //    {
    //        if (item.TotalDistance > status.TotalDistance)
    //        {
    //            worst = item; break;
    //        }
    //    }
    //    if (worst != null)
    //    {
    //        topGuesses.Remove(worst);
    //        topGuesses.Add(status);
    //    }
    //}

    if (i % 100000 == 0 || status.Success)
    {
        LogProgress(i, status, status.GuessError, status.TotalDistance);
    }

    //if (i % 1000000 == 0)
    //{
    //    var average = new double[AlphabetSize];
    //    var sd = new double[AlphabetSize];

    //    for (int j = 0; j < AlphabetSize; j++)
    //    {
    //        average[j] = topGuesses.Average(p => (double)p.currentGuess[j]);
    //        sd[j] = topGuesses.Select(p => (double)p.currentGuess[j]).ToArray().StandardDeviation();
    //        var median = topGuesses.Select(p => p.currentGuess[j]).OrderBy(p => p).ElementAt(500);
    //        var min = topGuesses.Min(p => p.currentGuess[j]);
    //        var max = topGuesses.Max(p => p.currentGuess[j]);
    //        Console.WriteLine((char)('a' + j) + "\t" + average[j] + "\t" + sd[j] + "\t" + min + "\t" + median + "\t" + max);
    //    }
    //}

    if (status.Success)
    {
        if (status.Reordered)
        {
            Console.WriteLine("Reordered guess");
        }
        break;
    }
}

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("Finished: " + autogram.ToString());
Console.ResetColor();

void LogProgress(int i, Status status, int[] diffs, int totalDistance)
{
    Console.WriteLine(sw.Elapsed.ToString(@"hh\:mm\:ss") + "\tIteration: " + i.Humanize() + "\tHistory: " + status.HistoryCount.Humanize() + "\t" + (1000 * (double)i / sw.ElapsedMilliseconds).Humanize() + " iterations/s");

    Console.Write("Current iteration:\t");
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

    Console.WriteLine("\tMismatches: " + diffs.Count(p => p != 0) + "\tTotal distance: " + totalDistance + (status.RandomReset ? "\tRandomized 🎲" : ""));
}