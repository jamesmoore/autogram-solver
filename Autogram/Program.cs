// See https://aka.ms/new-console-template for more information
var autogram = new Autogram.Autogram();

Console.WriteLine(autogram.ToString());

int i = 0;

while (autogram.Evaluate() == false)
{
    var diffs = autogram.GetGuessError();

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

    Console.WriteLine("\t" + diffs.Count(p => p != 0) + "\t" + diffs.Sum(p => Math.Abs(p)));

    autogram.Iterate();

    Console.WriteLine((i++) + "\t" + autogram.ToString());

    // Console.ReadKey();
}