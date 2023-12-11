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

string[] lines = File.ReadAllLines(filePath);

static char[,] GetEngineSchematic(string[] lines)
{
    // Adding a guard in all sides consisting of only '.' to ease boundary condition check logic.
    var x_length = lines[0].Length + 2;
    var y_length = lines.Length + 2;

    var engineSchematic = new char[x_length, y_length];

    for (int y = 0; y < y_length; y++)
    {
        engineSchematic[y, 0] = '.';
        engineSchematic[y, x_length - 1] = '.';
    }

    for (int x = 0; x < x_length; x++)
    {
        engineSchematic[0, x] = '.';
        engineSchematic[y_length - 1, x] = '.';
    }

    for (int y = 0; y < lines.Length; y++)
    {
        for (int x = 0; x < lines[y].Length; x++)
        {
            engineSchematic[x + 1, y + 1] = lines[y][x];
        }
    }

    return engineSchematic;
}

var engineSchematic = GetEngineSchematic(lines);

// Part 1: Get sum of all part numbers.
var partNumbers = GetAllPartNumbers(engineSchematic);
var sumOfPartNumbers = partNumbers.Sum();

Console.WriteLine($"Sum of part numbers: {sumOfPartNumbers}");

// Part 2: Get sum of all gear ratios.
var gearRatios = GetAllGearRatios(engineSchematic);
var sumOfGearRatios = gearRatios.Sum();

Console.WriteLine($"Sum of gear ratios: {sumOfGearRatios}");

static List<int> GetAllPartNumbers(char[,] engineSchematic)
{
    var partNumbers = new List<int>();

    for (int y = 0; y < engineSchematic.GetLength(1); y++)
    {
        bool foundNumStart = false;
        int numStart = 0;
        int numEnd;
        for (int x = 0; x < engineSchematic.GetLength(0); x++)
        {
            var currentChar = engineSchematic[x, y];
            if (char.IsDigit(currentChar))
            {
                if (!foundNumStart)
                {
                    foundNumStart = true;
                    numStart = x;
                }
            }
            else
            {
                if (foundNumStart)
                {
                    numEnd = x - 1;
                    if (IsPartNumber((numStart, y), (numEnd, y), engineSchematic))
                    {
                        partNumbers.Add(GetPartNumber((numStart, y), (numEnd, y), engineSchematic));
                    }

                    foundNumStart = false;
                }
            }
        }
    }

    return partNumbers;
}

static List<int> GetAllGearRatios(char[,] engineSchematic)
{
    var gears = new Dictionary<string, List<int>>();
    var gearRatios = new List<int>();

    for (int y = 0; y < engineSchematic.GetLength(1); y++)
    {
        bool foundNumStart = false;
        int numStart = 0;
        int numEnd;
        for (int x = 0; x < engineSchematic.GetLength(0); x++)
        {
            var currentChar = engineSchematic[x, y];
            if (char.IsDigit(currentChar))
            {
                if (!foundNumStart)
                {
                    foundNumStart = true;
                    numStart = x;
                }
            }
            else
            {
                if (foundNumStart)
                {
                    numEnd = x - 1;
                    var (gearCoordinates, partNumber) = GetGears((numStart, y), (numEnd, y), engineSchematic);
                    if (gearCoordinates.Count > 0)
                    {
                        foreach (var gearCoordinate in gearCoordinates)
                        {
                            if (gears.ContainsKey(gearCoordinate))
                            {
                                gears[gearCoordinate].Add(partNumber);
                            }
                            else
                            {
                                gears.Add(gearCoordinate, new List<int> { partNumber });
                            }
                        }
                    }

                    foundNumStart = false;
                }
            }
        }
    }

    foreach (var gear in gears)
    {
        if (gear.Value.Count == 2)
        {
            gearRatios.Add(gear.Value[0] * gear.Value[1]);
        }
    }

    return gearRatios;
}

static (List<string>, int) GetGears((int, int) numStart, (int, int) numEnd, char[,] engineSchematic)
{
    var gearCoordinates = new List<string>();

    var surroundingCoordinates = GetSurroundingCoordinates(numStart, numEnd);
    var isPart = false;
    var isGearPart = false;

    foreach (var coordinate in surroundingCoordinates)
    {
        var (x, y) = coordinate;
        var surroundingChar = engineSchematic[x, y];
        if (surroundingChar != '.' && !char.IsDigit(surroundingChar))
        {
            isPart = true;
        }

        if (surroundingChar == '*')
        {
            isGearPart = true;
            gearCoordinates.Add($"{x},{y}");
        }
    }

    if (isPart && isGearPart)
    {
        return (gearCoordinates, GetPartNumber(numStart, numEnd, engineSchematic));
    }

    return (new List<string>(), 0);
}

static int GetPartNumber((int, int) numStart, (int, int) numEnd, char[,] engineSchematic)
{
    var (x_start, _) = numStart;
    var (x_end, y) = numEnd;

    var numString = string.Empty;

    for (int x = x_start; x <= x_end; x++)
    {
        var currentChar = engineSchematic[x, y];
        numString += currentChar;
    }

    return int.Parse(numString);
}

static bool IsPartNumber((int, int) numStart, (int, int) numEnd, char[,] engineSchematic)
{
    var surroundingCoordinates = GetSurroundingCoordinates(numStart, numEnd);
    foreach (var coordinate in surroundingCoordinates)
    {
        var (x, y) = coordinate;
        var surroundingChar = engineSchematic[x, y];
        if (surroundingChar != '.' && !char.IsDigit(surroundingChar))
        {
            return true;
        }
    }

    return false;
}

static List<(int, int)> GetSurroundingCoordinates((int, int) startCoord, (int, int) endCoord)
{
    var (x_start, _) = startCoord;
    var (x_end, y) = endCoord;

    var surroundingCoordinates = new List<(int, int)>();

    for (int x = x_start - 1; x <= x_end + 1; x++)
    {
        surroundingCoordinates.Add((x, y - 1));
        surroundingCoordinates.Add((x, y + 1));
    }

    surroundingCoordinates.Add((x_start - 1, y));
    surroundingCoordinates.Add((x_end + 1, y));

    return surroundingCoordinates;
}