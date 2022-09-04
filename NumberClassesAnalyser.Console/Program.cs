using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Immutable;

if (args.Length == 0)
{
    throw new ArgumentException("Must provide an absolut sln path");
}

var solutionPath = args[0];

List<string> projectPaths = await GetProjectPaths(solutionPath);

AnalyzerManager manager = new(solutionPath);

var projectAnalysers = projectPaths.Select(x => manager.GetProject(x));

Dictionary<string, List<string>> projectClassesDict = new();
foreach (var projectAnalyser in projectAnalysers)
{
    var visitor = new ClassVisitor();
    AdhocWorkspace workspace = new();
    var proj = projectAnalyser.AddToWorkspace(workspace);

    foreach (var doc in proj.Documents)
    {
        var root = await doc.GetSyntaxRootAsync();
        visitor.Visit(root);
    }
    projectClassesDict.Add(proj.AssemblyName, visitor.Classes);
}

PrintProjectAndClasses(projectClassesDict);

async Task<List<string>> GetProjectPaths(string solutionPath)
{
    var workSpace = MSBuildWorkspace.Create();
    var sln = await workSpace.OpenSolutionAsync(solutionPath);
    List<string> projectPaths = new();
    foreach (var proj in sln.Projects)
    {
        projectPaths.Add(proj.FilePath ?? string.Empty);        
    }
    return projectPaths;
}

static void PrintProjectAndClasses(Dictionary<string, List<string>> projectClassesDict)
{
    foreach (var entry in projectClassesDict.Keys)
    {
        Console.WriteLine($"Project: {entry}, Number of classes: {projectClassesDict[entry].Count}");
        //foreach (var className in projectClassesDict[entry])
        //{
        //    Console.WriteLine($"\tClass: ${className}");
        //}
    }
}
