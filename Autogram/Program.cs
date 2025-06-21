// See https://aka.ms/new-console-template for more information
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Autogram;
using Autogram.Extensions;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.CancelKeyPress += (sender, args) =>
{
    args.Cancel = true;
    Console.Write("\x1b]9;4;0;\x07");
    Environment.Exit(0);
};

var template = args.Length > 0 ? args[0] : null;
const string defaultAlphabetRegex = "[a-z]";
const string defaultTemplate = "This sentence is an autogram and it contains {0}."; // from https://en.wikipedia.org/wiki/Autogram
const string defaultConjunction = " and ";
const string defaultForced = "";

var templateOption = new Option<string>("--template", "-t")
{
    Description = "The template of the autogram to search for. Must contain a {0}.",
    DefaultValueFactory = (ArgumentResult r) => defaultTemplate,
};

var conjunctionOption = new Option<string>("--conjunction", "-c")
{
    Description = "The conjunction to add to the list of letters, appearing before the final one. This is typically \" and \" but you could leave it empty or use \" and lastly \", \" and last but not least\" etc.",
    DefaultValueFactory = (ArgumentResult r) => defaultConjunction,
};

var seedOption = new Option<int?>("--seed", "-s")
{
    Description = "The seed to use in the random number generator, to create repeatable runs. Leave undefined to allow the system to choose",
    DefaultValueFactory = (ArgumentResult r) => null
};

var resetOption = new Option<int?>("--reset", "-r")
{
    Description = "Reset (clear history and increment random seed) after N iterations",
    DefaultValueFactory = (ArgumentResult r) => null
};

var alphabetRegexOption = new Option<string>("--alphabet", "-a")
{
    Description = @"A regex defining the letters of the alphabet to use. Eg, [a-y\.].",
    DefaultValueFactory = (ArgumentResult r) => defaultAlphabetRegex
};
alphabetRegexOption.Validators.Add(result =>
{
    var value = result.GetValue(alphabetRegexOption);

    if (value == null || value.IsValidRegex() == false)
    {
        result.AddError($"Alphabet regex: {value} is not a valid regex.");
    }
});

var forcedRegexOption = new Option<string>("--forced", "-f")
{
    Description = @"A regex defining the letters that should be present in the count even if they aren't in the template. Eg, [kqz].",
    DefaultValueFactory = (ArgumentResult r) => defaultForced
};

forcedRegexOption.Validators.Add(result =>
{
    var value = result.GetValue(forcedRegexOption);

    if (string.IsNullOrWhiteSpace(value) == false && value.IsValidRegex() == false)
    {
        result.AddError($"Forced regex: {value} is not a valid regex.");
    }
});

var quietOption = new Option<bool>("--quiet", "-q")
{
    Description = "If true then only a final success will be shown",
    DefaultValueFactory = (ArgumentResult r) => false
};

var rootCommand = new RootCommand("Autogram searcher")
{
    templateOption,
    conjunctionOption,
    seedOption,
    alphabetRegexOption,
    forcedRegexOption,
    resetOption,
    quietOption,
};

rootCommand.SetAction(parseResult =>
{
    DoAutogramSearch(
        parseResult.GetValue(alphabetRegexOption),
        parseResult.GetValue(seedOption),
        parseResult.GetValue(templateOption),
        parseResult.GetValue(conjunctionOption),
        parseResult.GetValue(forcedRegexOption),
        parseResult.GetValue(resetOption),
        parseResult.GetValue(quietOption)
        );
    return 0;
});

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();

void DoAutogramSearch(
    string alphabetRegexString,
    int? seed,
    string template,
    string conjunction,
    string forcedRegexString,
    int? reset,
    bool quiet)
{
    Console.Write("\x1b]9;4;3\x07"); // https://learn.microsoft.com/en-us/windows/terminal/tutorials/progress-bar-sequences

    var fullAlphabet = Enumerable.Range(0, 256).Select(p => (char)p).ToArray();

    var alphabetRegex = new Regex(alphabetRegexString);
    var alphabet = fullAlphabet.Where(p => alphabetRegex.IsMatch(p.ToString())).ToArray();

    var forced = defaultForced.ToCharArray();
    if (string.IsNullOrWhiteSpace(forcedRegexString) == false)
    {
        var forcedRegex = new Regex(forcedRegexString);
        forced = fullAlphabet.Where(p => forcedRegex.IsMatch(p.ToString())).ToArray();
    }

    var rootRandom = new Random();

    if (seed == null)
    {
        seed = rootRandom.Next();
    }

    var config = new AutogramConfigFactory().MakeAutogramConfig(new string(alphabet), template, conjunction, "'s", new string(forced));

    if (quiet == false)
    {
        Console.WriteLine("Pre-run summary");
        Console.WriteLine("---------------");
        Console.WriteLine("Index\tChar\tBase\tMin\tFixed\tVIndex\tVBase\tVMin\tIncludeSelf");

        foreach (var letterConfig in config.AllChars)
        {
            Console.WriteLine($"{letterConfig.Index}\t" +
                $"{letterConfig.Char}\t" +
                $"{letterConfig.BaselineCount}\t" +
                $"{letterConfig.MinimumCount}\t" +
                $"{(letterConfig.IsVariable ? "N" : "Y")}\t" +
                $"{letterConfig.VariableIndex}\t" +
                $"{letterConfig.VariableBaselineCount}\t" +
                $"{(letterConfig.IsVariable ? letterConfig.MinimumCount : String.Empty)}\t" +
                $"{(letterConfig.IncludeSelfInCount ? "Y" : "N")}\t" +
                ""
                );
        }
    }

    var autogram = new Autogram.AutogramBytesNoStringsV4(config, seed);

    if (quiet == false)
    {
        Console.WriteLine("Starting: " + autogram.ToString());
    }

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

        if (quiet == false && (i % 1000000 == 0 || status.Success))
        {
            LogProgress(i, autogram.HistoryCount, sw.Elapsed, randomized, seed.Value);
        }

        if (status.Success)
        {
            var commandLine = GetCommandLine(seed, template, conjunction, forcedRegexString, alphabetRegex);
            ReportSuccess(quiet, autogram.ToString(), i, randomized, sw, status, commandLine);
            break;
        }

        if (reset.HasValue && i % reset == 0)
        {
            seed++;
            autogram = new Autogram.AutogramBytesNoStringsV4(config, seed);
        }
    }
}

void LogProgress(int i, int historyCount, TimeSpan ts, int randomized, int seed)
{
    var itspersecond = (1000 * (double)i / ts.TotalMilliseconds);
    Console.WriteLine($"{ts:hh\\:mm\\:ss}\tIteration: {i.Humanize()}\tHistory: {historyCount.Humanize()}\t{itspersecond.Humanize()} iterations/s\tRandomized: {randomized / (double)i:P}\tSeed:\t{seed}");
}
static void ReportSuccess(bool quiet, string autogramString, int i, int randomized, Stopwatch sw, Status status, string commandLine)
{
    if (status.Reordered && quiet == false)
    {
        Console.WriteLine("Reordered guess");
    }

    Console.WriteLine(commandLine);
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"🎉 Finished 🎉");
    Console.WriteLine($"⏱️ Duration:\t\t\t{sw.Elapsed:hh\\:mm\\:ss}");
    Console.WriteLine($"🔁 Iterations:\t\t\t{i:N0}");
    Console.WriteLine($"🔁/⏱️ Iterations per second:\t{(i / sw.Elapsed.TotalSeconds).Humanize()}");
    Console.WriteLine($"🎲 Randomized:\t\t\t{randomized / (double)i:P}");
    Console.WriteLine(new string('-', Console.WindowWidth));
    Console.WriteLine(autogramString);
    Console.WriteLine(new string('-', Console.WindowWidth));

    var verified = autogramString.IsAutogram();
    if (verified == false)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Verification FAILED ❌");
    }

    Console.ResetColor();

    Console.Write("\x1b]9;4;0\x07");
}

static string GetCommandLine(int? seed, string template, string conjunction, string forcedRegexString, Regex alphabetRegex)
{
    return $"{GetCurrentExe()}" +
        (template != defaultTemplate ? $" --template \"{template}\"" : "") +
        (conjunction != defaultConjunction ? $" --conjunction \"{conjunction}\"" : "") +
        (alphabetRegex.ToString() != defaultAlphabetRegex ? $" --alphabet \"{alphabetRegex}\"" : "") +
        (forcedRegexString.ToString() != defaultForced ? $" --forced {forcedRegexString}" : "") +
        $" --seed {seed}";
}

static string GetCurrentExe()
{
    var processPath = Environment.ProcessPath ?? throw new ApplicationException("No process path");
    return new DirectoryInfo(Directory.GetCurrentDirectory()).GetRelativePathTo(new FileInfo(processPath));
}
