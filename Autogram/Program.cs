// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.Diagnostics;
using Autogram;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.CancelKeyPress += (sender, args) =>
{
    args.Cancel = true;
    Console.Write("\x1b]9;4;0;\x07");
    Environment.Exit(0);
};

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

    var alphabet = Enumerable.Range(0, AlphabetSize).Select(p => (char)('a' + p)).ToArray();

    if (seed == null)
    {
        seed = new Random().Next();
    }

    var config = new AutogramConfigFactory().MakeAutogramConfig(new string(alphabet), template, conjunction, "'s");

    Console.WriteLine("Pre-run summary");
    Console.WriteLine("---------------");
    Console.WriteLine("Index\tChar\tBase\tMin\tFixed\tVIndex\tVBase\tVMin");

    foreach (var letterConfig in config.Letters)
    {
        Console.WriteLine($"{letterConfig.Index}\t" +
            $"{letterConfig.Char}\t" +
            $"{letterConfig.BaselineCount}\t" +
            $"{letterConfig.MinimumCount}\t" +
            $"{(letterConfig.IsVariable ? "N" : "Y")}\t" +
            $"{letterConfig.VariableIndex}\t" +
            $"{letterConfig.VariableBaselineCount}\t" +
            ""
            );
    }

    var autogram = new Autogram.AutogramBytesNoStringsV2(new string(alphabet), config, seed);

    Console.WriteLine("Starting: " + autogram.ToString());

    int i = 0;
    int randomized = 0;
    var sw = new Stopwatch();
    sw.Start();

    while (true)
    {
        i++;
        var status = autogram.Iterate();

        if (status.Randomized)
        {
            randomized++;
        }

        if (i % 1000000 == 0 || status.Success)
        {
            LogProgress(i, status.HistoryCount, sw.Elapsed, randomized);
        }

        if (status.Success)
        {
            if (status.Reordered)
            {
                Console.WriteLine("Reordered guess");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"🎉 Finished 🎉");
            Console.WriteLine($"⏱️ Duration:\t\t\t{sw.Elapsed:hh\\:mm\\:ss}");
            Console.WriteLine($"🔁 Iterations:\t\t\t{i:N0}");
            Console.WriteLine($"🔁/⏱️ Iterations per second:\t{(i / sw.Elapsed.TotalSeconds).Humanize()}");
            Console.WriteLine($"🎲 Randomized:\t\t\t{randomized / (double)i:P}");
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine(autogram);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine($"{Environment.ProcessPath} --template \"{template}\" --conjunction \"{conjunction}\" --alphabet {AlphabetSize} --seed {seed}");

            Console.ResetColor();
            Console.Write("\x1b]9;4;0\x07");
            break;
        }
    }
}

void LogProgress(int i, int historyCount, TimeSpan ts, int randomized)
{
    var itspersecond = (1000 * (double)i / ts.TotalMilliseconds);
    Console.WriteLine($"{ts:hh\\:mm\\:ss}\tIteration: {i.Humanize()}\tHistory: {historyCount.Humanize()}\t{itspersecond.Humanize()} iterations/s\tRandomized: {randomized / (double)i:P}");
}

