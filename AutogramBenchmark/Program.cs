﻿// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

// var summary = BenchmarkRunner.Run(typeof(Program).Assembly);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
