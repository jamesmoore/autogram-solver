# Autogram solver
See https://en.wikipedia.org/wiki/Autogram

Uses basic heuristics, plus random variations to escape from infinite loops.

# Usage

## Help
```powershell
.\Autogram.exe --help
```
```
Description:
  Autogram searcher

Usage:
  Autogram [options]

Options:
  -t, --template <template>        The template of the autogram to search for. Must contain a {0}. [default: This sentence is an
                                   autogram and it contains {0}.]
  -c, --conjunction <conjunction>  The conjunction to add to the list of letters, appearing before the final one. This is typically "
                                   and " but you could leave it empty or use " and lastly ", " and last but not least" etc. [default:
                                   and ]
  -s, --seed <seed>                The seed to use in the random number generator, to create repeatable runs. Leave undefined to allow
                                   the system to choose []
  -a, --alphabet <alphabet>        A regex defining the letters of the alphabet to use. Eg, [a-y\.]. [default: [a-z]]
  -f, --forced <forced>            A regex defining the letters that should be present in the count even if they aren't in the template.
                                   Eg, [kqz]. []
  --version                        Show version information
  -?, -h, --help                   Show help and usage information
```

## Default
```powershell
.\Autogram.exe
```
This will run using the template `This sentence is an autogram and it contains {0}.`, the full alphabet and ` and ` as the conjunctive.

After a number of iterations you will get an output like
```Finished @ iteration 14,010,783: This sentence is an autogram and it contains seven a's, three c's, three d's, twenty-six e's, seven f's, four g's, six h's, fourteen i's, one l, two m's, twenty n's, nine o's, eight r's, twenty-nine s's, eighteen t's, six u's, five v's, five w's, four x's and four y's.```

On a reasonable PC you should get ~500K iterations per second.

# Examples

## Wikipedia example
```powershell
.\Autogram.exe --template "This sentence employs {0}." --forced z --seed 1745959527
```
```🎉 Finished 🎉
⏱️ Duration:                    00:00:23
🔁 Iterations:                  19,585,004
🔁/⏱️ Iterations per second:    840.804k
🎲 Randomized:                  26.53%
This sentence employs two a's, two c's, two d's, twenty-eight e's, five f's, three g's, eight h's, eleven i's, three l's, two m's, thirteen n's, nine o's, two p's, five r's, twenty-five s's, twenty-three t's, six v's, ten w's, two x's, five y's and one z.
```

## Wikipedia example (alternate solution)
```powershell
.\Autogram.exe --template "This sentence employs {0}." --forced z --seed 956257669
```
```🎉 Finished 🎉
⏱️ Duration:                    00:01:41
🔁 Iterations:                  69,211,766
🔁/⏱️ Iterations per second:    680.373k
🎲 Randomized:                  30.93%
-----------------------------------------------------------------------------------------------------------------------------------------
This sentence employs two a's, two c's, two d's, thirty-three e's, three f's, two g's, seven h's, seven i's, three l's, two m's, seventeen n's, eleven o's, two p's, five r's, twenty-seven s's, twenty t's, one u, eight v's, nine w's, one x, five y's and one z.
```

## Pangrammatic autogram
```powershell
.\Autogram.exe --template "the quick brown fox jumped over the lazy dog and yelled 'this sentence contains {0}.'" --seed 35875715
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:02
🔁 Iterations:                  1,940,414
🔁/⏱️ Iterations per second:    903.342k
🎲 Randomized:                  22.69%
the quick brown fox jumped over the lazy dog and yelled 'this sentence contains five a's, two b's, four c's, six d's, thirty-four e's, ten f's, three g's, eight h's, fifteen i's, two j's, two k's, five l's, two m's, twenty-one n's, seventeen o's, two p's, two q's, nine r's, thirty-five s's, twenty-seven t's, six u's, nine v's, twelve w's, four x's, seven y's and two z's.''
```

## Pangrammatic autogram #2
```powershell
.\Autogram.exe --template "jackdaws love my big sphinx of quartz and they also love this sentence which has {0}." --seed 713122846
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:28
🔁 Iterations:                  22,654,450
🔁/⏱️ Iterations per second:    801.678k
🎲 Randomized:                  29.12%
jackdaws love my big sphinx of quartz and they also love this sentence which has eight a's, two b's, four c's, four d's, thirty e's, ten f's, five g's, thirteen h's, eighteen i's, two j's, two k's, five l's, two m's, sixteen n's, fifteen o's, two p's, two q's, eight r's, thirty-seven s's, twenty-seven t's, five u's, nine v's, twelve w's, four x's, six y's and two z's.
```


## Custom conjunction (for unknown reasons this seems to solve rapidly)
```powershell
.\Autogram.exe --conjunction " and lastly " --seed 943828433
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:00
🔁 Iterations:                  542,822
🔁/⏱️ Iterations per second:    1.033M
🎲 Randomized:                  15.64%
This sentence is an autogram and it contains eight a's, three c's, three d's, thirty-one e's, seven f's, five g's, ten h's, fifteen i's, three l's, two m's, eighteen n's, seven o's, eight r's, twenty-nine s's, twenty-four t's, three u's, six v's, five w's, two x's and lastly five y's.
```
