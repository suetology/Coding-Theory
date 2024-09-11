using Microsoft.Extensions.Configuration;
using Code.Extensions;
using Code;
using Math;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("configuration.json", optional: false, reloadOnChange: false)
    .Build();

var errorProbability = configuration.GetErrorProbability();
var message = configuration.GetMessage();
var code = configuration.GetCodeConfiguration();

Console.WriteLine(code.GeneratorMatrix);

code.GeneratorMatrix.CreateParityCheckMatrix();