using CommandLiner.Application.Commands.Serialize;
using CommandLiner.Application.Enums;
using CommandLiner.Common.Utilities;
using Microsoft.Extensions.Logging;
using static CommandLiner.Application.Enums.FileType;

namespace CommandLiner.Application.Tests.Unit.Commands;

public sealed class SerializeCommandTests
{
    private const string test = nameof(test);
    private readonly SerializeCommand _sut;

    public SerializeCommandTests()
    {
        var loggerMock = Substitute.For<ILogger<SerializeCommand>>();
        _sut = new SerializeCommand(loggerMock);
    }

    [Fact]
    public void Handle_ShouldCreateNewJsonFile_WhenYamlFileIsProvided()
    {
        //Arrange
        var directoryInfo = new DirectoryInfo($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{test}");
        directoryInfo.Create();

        var fileInfo = new FileInfo($"{directoryInfo.FullName}{Path.DirectorySeparatorChar}{Path.GetRandomFileName().Replace('.', 'a')}{Yaml.GetExtensionWithDot()}");
        using var fileStream = fileInfo.Create();

        fileStream.WriteAndClose(GetYamlContent());

        //Act
        _sut.Handle(test, fileInfo.Name, Yaml, Json);

        //Assert
        var newFileName = fileInfo.FullName.Replace(Yaml.GetExtensionWithDot(), Json.GetExtensionWithDot());
        var resultFileInfo = new FileInfo(newFileName);
        resultFileInfo.Exists.Should().BeTrue();
        directoryInfo.ForceDelete();
    }

    [Fact]
    public void Handle_ShouldCreateNewYamlFile_WhenJsonFileIsProvided()
    {
        //Arrange
        var directoryInfo = new DirectoryInfo($"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{test}");
        directoryInfo.Create();

        var fileInfo = new FileInfo($"{directoryInfo.FullName}{Path.DirectorySeparatorChar}{Path.GetRandomFileName().Replace('.', 'a')}{Json.GetExtensionWithDot()}");
        using var fileStream = fileInfo.Create();

        fileStream.WriteAndClose(GetJsonContent());

        //Act
        _sut.Handle(test, fileInfo.Name, Json, Yaml);

        //Assert
        var newFileName = fileInfo.FullName.Replace(Json.GetExtensionWithDot(), Yaml.GetExtensionWithDot());
        var resultFileInfo = new FileInfo(newFileName);
        resultFileInfo.Exists.Should().BeTrue();
        directoryInfo.ForceDelete();
    }

    public string GetYamlContent()
    {
        return """
            doe: "a deer, a female deer"
            ray: "a drop of golden sun"
            pi: 3.14159
            xmas: true
            french-hens: 3
            calling-birds:
              - huey
              - dewey
              - louie
              - fred
            xmas-fifth-day:
              calling-birds: four
              french-hens: 3
              golden-rings: 5
              partridges:
                count: 1
                location: "a pear tree"
              turtle-doves: two
            """;
    }

    public string GetJsonContent()
    {
        return """
            {
              "doe": "a deer, a female deer",
              "ray": "a drop of golden sun",
              "pi": "3.14159",
              "xmas": "true",
              "french-hens": "3",
              "calling-birds": [
                "huey",
                "dewey",
                "louie",
                "fred"
              ],
              "xmas-fifth-day": {
                "calling-birds": "four",
                "french-hens": "3",
                "golden-rings": "5",
                "partridges": {
                  "count": "1",
                  "location": "a pear tree"
                },
                "turtle-doves": "two"
              }
            }
            """;
    }
}