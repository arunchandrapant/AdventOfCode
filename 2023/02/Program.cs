// <copyright file="Program.cs" company="Arun Pant">
// Copyright (c) Arun Pant. All rights reserved.
// </copyright>

using System.Reflection;

// Get the current assembly's location
string path = Assembly.GetExecutingAssembly().Location;

// Get the directory of the current assembly
string? directory = Path.GetDirectoryName(path);

// Combine the directory with the relative path of the file
string filePath = Path.Combine(directory ?? string.Empty, "./data/input");

string[] games = File.ReadAllLines(filePath);

var sumOfIdsOfValidGames = 0;
var sumOfPowerOfCubes = 0;

foreach (var game in games)
{
    var gameId = int.Parse(
        game.Split(":")
            .First()
            .Split(" ")
            .Last());

    var draws = game
        .Split(":")
        .Last()
        .Split(";");

    // [[(1, "red"), (5, "green)], [(2, "blue"), (3, "yellow")]]
    var gameDraws = draws
    .Select(draw => draw
        .Split(",")
        .Select(cubes => cubes
            .Trim()
            .Split(" "))
        .Select(cubes =>
            (int.Parse(cubes.First()),
            cubes.Last())));

    if (IsGameValid(gameDraws))
    {
        sumOfIdsOfValidGames += gameId;
    }

    var powerofCubes = GetPowerOfCubes(gameDraws);
    sumOfPowerOfCubes += powerofCubes;
}

Console.WriteLine($"Sum of ids of valid games: {sumOfIdsOfValidGames}");
Console.WriteLine($"Sum of power of cubes: {sumOfPowerOfCubes}");

static int GetPowerOfCubes(IEnumerable<IEnumerable<(int, string)>> gameDraws)
{
    var maxCountOfRed = 0;
    var maxCountOfGreen = 0;
    var maxCountOfBlue = 0;

    foreach (var draw in gameDraws)
    {
        foreach ((int count, string color) in draw)
        {
            if (color == "red")
            {
                maxCountOfRed = int.Max(maxCountOfRed, count);
            }
            else if (color == "green")
            {
                maxCountOfGreen = int.Max(maxCountOfGreen, count);
            }
            else
            {
                maxCountOfBlue = int.Max(maxCountOfBlue, count);
            }
        }
    }

    return maxCountOfBlue * maxCountOfGreen * maxCountOfRed;
}

static bool IsGameValid(IEnumerable<IEnumerable<(int, string)>> gameDraws)
{
    var gameValid = true;

    foreach (var draw in gameDraws)
    {
        if (!IsDrawValid(draw))
        {
            gameValid = false;
            break;
        }
    }

    return gameValid;
}

static bool IsDrawValid(IEnumerable<(int, string)> draw)
{
    Dictionary<string, int> cubesInBag = new ()
    {
        { "red", 12 },
        { "green", 13 },
        { "blue", 14 },
    };

    var drawValid = true;

    foreach ((int count, string color) in draw)
    {
        if (cubesInBag[color] < count)
        {
            drawValid = false;
            break;
        }
    }

    return drawValid;
}