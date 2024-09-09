using Microsoft.Extensions.Configuration;
using Code.Extensions;
using Math;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("configuration.json", optional: false, reloadOnChange: false)
    .Build();

var errorProbability = configuration.GetErrorProbability();
var order = configuration.GetDivisionRingOrder();
var code = configuration.GetCodeConfiguration();

Console.WriteLine(code.Length);
Console.WriteLine(code.Dimension);
Console.WriteLine(code.Matrix);
