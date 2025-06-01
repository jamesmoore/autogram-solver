dotnet build --configuration Release

$p = ".\Autogram\bin\Release\net9.0\"

. $p\Autogram.exe -r 1000000 -q
. $p\Autogram.exe --template "This sentence employs {0}." --forced z -r 100000000 -q
. $p\Autogram.exe --template "This sentence employs {0}." --forced z -r 100000000 -q
. $p\Autogram.exe --template "the quick brown fox jumped over the lazy dog and yelled 'this sentence contains {0}.'" -r 1000000 -q
. $p\Autogram.exe --template "jackdaws love my big sphinx of quartz and they also love this sentence which has {0}." -r 1000000 -q
. $p\Autogram.exe --conjunction " and lastly " -r 1000000 -q
