// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Diagnostics;
using Autogram;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var template = args.Length > 0 ? args[0] : null;
const int AlphabetSize = 26;
const string defaultTemplate = "This sentence is an autogram and it contains {0}."; // from https://en.wikipedia.org/wiki/Autogram
const string defaultConjunction = " and ";

var templateOption = new Option<string?>(
    aliases: ["--template", "-t"],
    description: "The template of the autogram to search for. Must contain a {0}.",
    getDefaultValue: () => defaultTemplate
    );

var conjunctionOption = new Option<string?>(
    aliases: ["--conjunction", "-c"],
    description: "The conjunction to add to the list of letters, appearing before the final one. This is typically \" and \" but you could leave it empty or use \" and lastly \", \" and last but not least\" etc.",
    getDefaultValue: () => defaultConjunction
    );

var seedOption = new Option<int?>(
    aliases: ["--seed", "-s"],
    description: "The seed to use in the random number generator, to create repeatable runs. Leave undefined to allow the system to choose",
    getDefaultValue: () => null
    );

var alphabetSizeOption = new Option<int>(
    aliases: ["--alphabet", "-a"],
    description: "The number of letters of the alphabet to use. Eg, you may want to skip z. [This may be improved]",
    getDefaultValue: () => AlphabetSize
    );

var rootCommand = new RootCommand("Autogram searcher")
{
    templateOption,
    conjunctionOption,
    seedOption,
    alphabetSizeOption,
};

rootCommand.SetHandler((template, conjunction, seed, alphabetSize) =>
{
    DoAutogramSearch(alphabetSize, seed, template, conjunction);
},
templateOption, conjunctionOption, seedOption, alphabetSizeOption);

return rootCommand.InvokeAsync(args).Result;

void DoAutogramSearch(int AlphabetSize, int? seed, string template, string conjunction)
{
    Console.Write("\x1b]9;4;3\x07"); // https://learn.microsoft.com/en-us/windows/terminal/tutorials/progress-bar-sequences

    var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToList();

    var autogram = new Autogram.AutogramBytesNoStrings(alphabet, template, conjunction, seed);

    Console.WriteLine("Starting: " + autogram.ToString());

    int i = 0; 

    var sw = new Stopwatch();
    sw.Start();

    while (true)
    {
        i++;
        var status = autogram.Iterate();

        if (i % 100000 == 0 || status.Success)
        {
            LogProgress(i, status, status.GuessError, status.TotalDistance, sw);
        }

        if (status.Success)
        {
            if (status.Reordered)
            {
                Console.WriteLine("Reordered guess");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Finished @ iteration {i:N0}: {autogram}");
            Console.ResetColor();
            Console.Write("\x1b]9;4;0\x07");
            break;
        }
    }
}

void LogProgress(int i, Status status, int[] diffs, int totalDistance, Stopwatch sw)
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

