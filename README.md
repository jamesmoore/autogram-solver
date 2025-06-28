# Autogram solver
See https://en.wikipedia.org/wiki/Autogram

Uses basic heuristics, plus random variations to escape from infinite loops.

Supports
* Counting letters (a-z by default) but configurable in the alphabet parameter
* Special characters - commas, apostrophes, hyphens, spaces if present in the alphabet parameter
* Max 100 counts per character

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
  -?, -h, --help     Show help and usage information
  --version          Show version information
  -t, --template     The template of the autogram to search for. Must contain a {0}. [default: This sentence is an autogram and it contains {0}.]
  -c, --conjunction  The conjunction to add to the list of letters, appearing before the final one. This is typically " and " but you could leave it empty or use " and lastly ", " and last but not
                     least" etc. [default:  and ]
  --separator        The separator between the itemised letter counts. This defaults to ", " [default: , ]
  -s, --seed         The seed to use in the random number generator, to create repeatable runs. Leave undefined to allow the system to choose []
  -a, --alphabet     A regex defining the letters of the alphabet to use. Eg, [a-y\.]. [default: [a-z]]
  -f, --forced       A regex defining the letters that should be present in the count even if they aren't in the template. Eg, [kqz]. []
  -r, --reset        Reset (clear history and increment random seed) after N iterations []
  -q, --quiet        If true then only a final success will be shown [default: False]
```
## Run in docker

```sh
docker run --rm ghcr.io/jamesmoore/autogram-solver:main
```

### with parameters
```sh
docker run --rm ghcr.io/jamesmoore/autogram-solver:main --template "This is an autogram with punctuation and it has {0}, one left brace, one right brace, and one full stop (aka period)." --conj
unction ", " --alphabet "[a-z\-', ]" --seed 1002327068
```

## Default
```powershell
.\Autogram.exe
```
This will run using the template `This sentence is an autogram and it contains {0}.`, the full alphabet and ` and ` as the conjunctive.

After a number of iterations you will get an output like
```
Autogram\bin\Release\net9.0\Autogram.exe --seed 673268347
🎉 Finished 🎉
⏱️ Duration:                    00:00:04
🔁 Iterations:                  4,879,276
🔁/⏱️ Iterations per second:    1.177M
🎲 Randomized:                  12.67%
This sentence is an autogram and it contains seven a's, three c's, three d's, twenty-six e's, seven f's, four g's, six h's, fourteen i's, one l, two m's, twenty n's, nine o's, eight r's, twenty-nine s's, eighteen t's, six u's, five v's, five w's, four x's and four y's.
```

On a reasonable PC you should get 1M+ iterations per second, but as the history builds up that will reduce, and memory pressure will build. To combat that you can set the --restart number which will reset the history after a certain number of iterations. The downside to this is that a reduced history will be less effective in preventing repetitive cycles. It's a trade off.

# Examples

## Wikipedia example
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "This sentence employs {0}." --forced z --seed 1423524136
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:05
🔁 Iterations:                  5,199,158
🔁/⏱️ Iterations per second:    945.474k
🎲 Randomized:                  31.96%
This sentence employs two a's, two c's, two d's, twenty-eight e's, five f's, three g's, eight h's, eleven i's, three l's, two m's, thirteen n's, nine o's, two p's, five r's, twenty-five s's, twenty-three t's, six v's, ten w's, two x's, five y's and one z.
```

## Wikipedia example (alternate solution)
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "This sentence employs {0}." --forced z --seed 812627953
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:07
🔁 Iterations:                  5,756,558
🔁/⏱️ Iterations per second:    817.31k
🎲 Randomized:                  33.16%
This sentence employs two a's, two c's, two d's, thirty-three e's, three f's, two g's, seven h's, seven i's, three l's, two m's, seventeen n's, eleven o's, two p's, five r's, twenty-seven s's, twenty t's, one u, eight v's, nine w's, one x, five y's and one z.
```

## Pangrammatic autogram
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "the quick brown fox jumped over the lazy dog and yelled 'this sentence contains {0}.'" --seed 56262793
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:00
🔁 Iterations:                  51,240
🔁/⏱️ Iterations per second:    390.177k
🎲 Randomized:                  3.47%
the quick brown fox jumped over the lazy dog and yelled 'this sentence contains five a's, two b's, four c's, six d's, thirty-four e's, ten f's, three g's, eight h's, fifteen i's, two j's, two k's, five l's, two m's, twenty-one n's, seventeen o's, two p's, two q's, nine r's, thirty-five s's, twenty-seven t's, six u's, nine v's, twelve w's, four x's, seven y's and two z's.'
```

## Pangrammatic autogram #2
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "jackdaws love my big sphinx of quartz and they also love this sentence which has {0}." --seed 1632955351
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:00
🔁 Iterations:                  277,555
🔁/⏱️ Iterations per second:    525.136k
🎲 Randomized:                  7.27%
jackdaws love my big sphinx of quartz and they also love this sentence which has eight a's, two b's, four c's, four d's, thirty e's, ten f's, five g's, thirteen h's, eighteen i's, two j's, two k's, five l's, two m's, sixteen n's, fifteen o's, two p's, two q's, eight r's, thirty-seven s's, twenty-seven t's, five u's, nine v's, twelve w's, four x's, six y's and two z's.
```

## Custom conjunction (for unknown reasons this seems to solve rapidly)
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --conjunction " and lastly " --seed 526894180
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:00
🔁 Iterations:                  657,766
🔁/⏱️ Iterations per second:    808.284k
🎲 Randomized:                  15.41%
This sentence is an autogram and it contains eight a's, three c's, three d's, thirty-one e's, seven f's, five g's, ten h's, fifteen i's, three l's, two m's, eighteen n's, seven o's, eight r's, twenty-nine s's, twenty-four t's, three u's, six v's, five w's, two x's and lastly five y's.
```

# Second Wikipedia Example (Lee Sallows)
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "Only the fool would take trouble to verify that his sentence was composed of {0} and, last but not least, a single !" --conjunction ", " --alphabet "[a-z\-',]" --seed 1868293385
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:06
🔁 Iterations:                  5,709,923
🔁/⏱️ Iterations per second:    880.881k
🎲 Randomized:                  13.96%
Only the fool would take trouble to verify that his sentence was composed of ten a's, three b's, four c's, four d's, forty-six e's, sixteen f's, four g's, thirteen h's, fifteen i's, two k's, nine l's, four m's, twenty-five n's, twenty-four o's, five p's, sixteen r's, forty-one s's, thirty-seven t's, ten u's, eight v's, eight w's, four x's, eleven y's, twenty-three apostrophes, twenty-seven commas, seven hyphens and, last but not least, a single !
```

# Including spaces in the count
```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "Only a fool would take the time to verify that this sentence is composed of {0} and one exclamation mark!" --conjunction ", " --alphabet "[a-z\-', ]" --seed 1080824275
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:09
🔁 Iterations:                  7,147,180
🔁/⏱️ Iterations per second:    790.647k
🎲 Randomized:                  13.48%
Only a fool would take the time to verify that this sentence is composed of eleven a's, six c's, four d's, fifty-four e's, thirteen f's, three g's, fifteen h's, twenty-one i's, three k's, eight l's, seven m's, twenty-nine n's, nineteen o's, six p's, thirteen r's, thirty-nine s's, thirty-seven t's, five u's, eleven v's, eight w's, four x's, twelve y's, seventy spaces, twenty-two apostrophes, twenty-five commas, seven hyphens and one exclamation mark!
```

```powershell
Autogram\bin\Release\net9.0\Autogram.exe --template "This is an autogram with punctuation and it has {0}, one left brace, one right brace, and one full stop (aka period)." --conjunction ", " --alphabet "[a-z\-', ]" --seed 1002327068
```
```
🎉 Finished 🎉
⏱️ Duration:                    00:00:03
🔁 Iterations:                  3,770,502
🔁/⏱️ Iterations per second:    1.211M
🎲 Randomized:                  11.08%
This is an autogram with punctuation and it has fifteen a's, three b's, six c's, four d's, fifty-one e's, twelve f's, seven g's, seventeen h's, twenty-one i's, two k's, five l's, four m's, thirty n's, nineteen o's, eight p's, sixteen r's, forty-one s's, thirty-eight t's, eight u's, eight v's, seven w's, three x's, ten y's, seventy-four spaces, twenty-three apostrophes, twenty-nine commas, seven hyphens, one left brace, one right brace, and one full stop (aka period).
```
