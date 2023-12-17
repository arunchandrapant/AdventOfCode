// <copyright file="Program.cs" company="Arun Pant">
// Copyright (c) Arun Pant. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Reflection;

// Get the current assembly's location
string path = Assembly.GetExecutingAssembly().Location;

// Get the directory of the current assembly
string? directory = Path.GetDirectoryName(path);

// Combine the directory with the relative path of the file
string filePath = Path.Combine(directory ?? string.Empty, "./data/input");

string[] lines = File.ReadAllLines(filePath);

List<(ImmutableHashSet<int>, ImmutableHashSet<int>)> cards = new ();

Dictionary<int, int> cardMatchingNumber = new ();

cards = GetCards(lines);

// Part 1: Get total card points.
var totalCardPoints = TotalCardPoints(cards);

Console.WriteLine($"Total card points: {totalCardPoints}");

var cardMatchingNumbers = new Dictionary<int, int>();

var totalCardCount = GetTotalCardCount();

Console.WriteLine($"Total card count: {totalCardCount}");

int GetTotalCardCount()
{
    var totalCardCount = 0;
    for (int i = 0; i < cards.Count; i++)
    {
        totalCardCount++;
        totalCardCount += GetCardCount(i);
    }

    return totalCardCount;
}

int GetCardCount(int cardIndex)
{
    var card = cards[cardIndex];
    var cardCount = 0;

    if (cardMatchingNumbers.TryGetValue(cardIndex, out int nextMatchingCardCount))
    {
    }
    else
    {
        nextMatchingCardCount = GetCardMatchingNumbers(card);
        cardMatchingNumbers.Add(cardIndex, nextMatchingCardCount);
    }

    for (int i = 1; i <= nextMatchingCardCount; i++)
    {
        cardCount++;
        if (cardIndex + i < cards.Count)
        {
            cardCount += GetCardCount(cardIndex + i);
        }
    }

    return cardCount;
}

int GetCardMatchingNumbers((ImmutableHashSet<int>, ImmutableHashSet<int>) card)
{
    var (winningNums, cardNums) = card;
    var winningCardNums = winningNums.Intersect(cardNums);
    var winningCount = winningCardNums.Count;

    return winningCount;
}

int TotalCardPoints(List<(ImmutableHashSet<int>, ImmutableHashSet<int>)> cards)
{
    var totalCardPoints = 0;
    foreach (var card in cards)
    {
        totalCardPoints += GetCardPoint(card);
    }

    return totalCardPoints;
}

int GetCardPoint((ImmutableHashSet<int>, ImmutableHashSet<int>) card)
{
    var winningCount = GetCardMatchingNumbers(card);

    if (winningCount == 0)
    {
        return 0;
    }

    return (int)Math.Pow(2, winningCount - 1);
}

List<(ImmutableHashSet<int>, ImmutableHashSet<int>)> GetCards(string[] lines)
{
    var cards = new List<(ImmutableHashSet<int>, ImmutableHashSet<int>)>();
    foreach (var line in lines)
    {
        var card = line
            .Split(":")
            .Last()
            .Split("|")
            .Select(numList => numList
                .Trim()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(num =>
                    int.Parse(num))
                .ToImmutableHashSet());

        cards.Add((card.First(), card.Last()));
    }

    return cards;
}