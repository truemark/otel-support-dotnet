#r "nuget: Semver, 2.1.0"
using System;
using Semver;

string version1 = Environment.GetEnvironmentVariable("VERSION1") ?? "0.0.0";
string version2 = Environment.GetEnvironmentVariable("VERSION2") ?? "0.0.0";

var v1 = SemVersion.Parse(version1);
var v2 = SemVersion.Parse(version2);

Console.WriteLine(v1.CompareTo(v2)); // Outputs -1, 0, or 1
