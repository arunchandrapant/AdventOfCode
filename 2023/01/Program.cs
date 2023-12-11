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

string[] caliberationLines = File.ReadAllLines(filePath);

var caliberationValueSum = 0;
var caliberationValueWithNumWordsSum = 0;

foreach (var caliberationLine in caliberationLines)
{
    int? caliberationValue = GetCaliberationValue(caliberationLine);
    int caliberationValueWithNumWords = GetCaliberationValueWithNumWords(caliberationLine);
    caliberationValueSum += caliberationValue ?? 0;
    caliberationValueWithNumWordsSum += caliberationValueWithNumWords;
}

Console.WriteLine($"Sum of caliberation values: {caliberationValueSum}");
Console.WriteLine($"Sum of caliberation values with num words: {caliberationValueWithNumWordsSum}");

static int? GetCaliberationValue(string caliberationLine)
{
    var firstDigit = caliberationLine.FirstOrDefault(char.IsDigit);
    var secondDigit = caliberationLine.LastOrDefault(char.IsDigit);

    if (firstDigit == default || secondDigit == default)
    {
        return null;
    }
    else
    {
        return (10 * int.Parse(firstDigit.ToString())) + int.Parse(secondDigit.ToString());
    }
}

static int GetCaliberationValueWithNumWords(string caliberationLine)
{
    int minPosition = caliberationLine.Length + 1, maxPosition = -1;
    int firstDigit = 0, lastDigit = 0;

    var digitTuple = caliberationLine
        .Select((value, index) => (value, index))
        .Where(tuple => char.IsDigit(tuple.value));

    minPosition = digitTuple.First().index;
    firstDigit = int.Parse(digitTuple.First().value.ToString());

    maxPosition = digitTuple.Last().index;
    lastDigit = int.Parse(digitTuple.Last().value.ToString());

    var numWordMap = new Dictionary<int, string>
    {
        { 0, "zero" },
        { 1, "one" },
        { 2, "two" },
        { 3, "three" },
        { 4, "four" },
        { 5, "five" },
        { 6, "six" },
        { 7, "seven" },
        { 8, "eight" },
        { 9, "nine" },
    };

    foreach (var numWord in numWordMap)
    {
        var num = numWord.Key;
        var word = numWord.Value;

        var firstPosition = caliberationLine.IndexOf(word);
        if (firstPosition != -1)
        {
            if (firstPosition < minPosition)
            {
                minPosition = firstPosition;
                firstDigit = num;
            }
        }

        var lastPosition = caliberationLine.LastIndexOf(word);
        if (lastPosition != -1)
        {
            if (lastPosition > maxPosition)
            {
                maxPosition = lastPosition;
                lastDigit = num;
            }
        }
    }

    return (10 * firstDigit) + lastDigit;
}