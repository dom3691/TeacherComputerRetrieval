using System;
using System.Collections.Generic;
using System.Linq;
using TeacherComputerRetrieval.Core;

namespace TeacherComputerRetrieval.App
{
    internal static class Program
    {
        private const string DefaultInput = "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7";

        private static void Main(string[] args)
        {
            Console.WriteLine("=== Teacher Computer Retrieval ===");
            Console.WriteLine("Enter routes in the form AB5, BC4, ...");
            Console.WriteLine($"Press ENTER to use default: {DefaultInput}");
            Console.Write("Routes: ");

            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                input = DefaultInput;
            }

            Graph graph;
            try
            {
                graph = GraphParser.ParseEdges(input);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing routes: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Graph loaded successfully.");
            RunMenu(graph);
        }

        private static void RunMenu(Graph graph)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Distance of a specific route");
                Console.WriteLine("2. Count trips with maximum number of stops");
                Console.WriteLine("3. Count trips with exact number of stops");
                Console.WriteLine("4. Shortest distance between two academies");
                Console.WriteLine("5. Count trips with maximum total distance");
                Console.WriteLine("0. Exit");
                Console.Write("Choice: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        HandleRouteDistance(graph);
                        break;

                    case "2":
                        HandleTripsWithMaxStops(graph);
                        break;

                    case "3":
                        HandleTripsWithExactStops(graph);
                        break;

                    case "4":
                        HandleShortestDistance(graph);
                        break;

                    case "5":
                        HandleTripsWithMaxDistance(graph);
                        break;

                    case "0":
                        Console.WriteLine("Goodbye.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void HandleRouteDistance(Graph graph)
        {
            Console.WriteLine("Enter route (e.g. A-B-C or ABC): ");
            Console.Write("Route: ");
            var routeInput = Console.ReadLine();

            try
            {
                var route = ParseRoute(routeInput);
                var distance = graph.GetRouteDistance(route);

                var formatted = distance.HasValue ? distance.Value.ToString() : "NO SUCH ROUTE";
                Console.WriteLine($"Distance: {formatted}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void HandleTripsWithMaxStops(Graph graph)
        {
            var start = ReadAcademy("Start academy (e.g. A): ");
            var end = ReadAcademy("End academy (e.g. C): ");
            var maxStops = ReadPositiveInt("Maximum number of stops: ");

            var count = graph.CountTripsWithMaxStops(start, end, maxStops);
            Console.WriteLine($"Number of trips from {start} to {end} with at most {maxStops} stops: {count}");
        }

        private static void HandleTripsWithExactStops(Graph graph)
        {
            var start = ReadAcademy("Start academy (e.g. A): ");
            var end = ReadAcademy("End academy (e.g. C): ");
            var exactStops = ReadPositiveInt("Exact number of stops: ");

            var count = graph.CountTripsWithExactStops(start, end, exactStops);
            Console.WriteLine($"Number of trips from {start} to {end} with exactly {exactStops} stops: {count}");
        }

        private static void HandleShortestDistance(Graph graph)
        {
            var start = ReadAcademy("Start academy (e.g. A): ");
            var end = ReadAcademy("End academy (e.g. C): ");

            var distance = graph.FindShortestDistance(start, end);

            if (distance.HasValue)
            {
                Console.WriteLine($"Shortest distance from {start} to {end}: {distance.Value}");
            }
            else
            {
                Console.WriteLine($"No route from {start} to {end}.");
            }
        }

        private static void HandleTripsWithMaxDistance(Graph graph)
        {
            var start = ReadAcademy("Start academy (e.g. C): ");
            var end = ReadAcademy("End academy (e.g. C): ");
            var maxDistance = ReadPositiveInt("Maximum total distance (strictly less than): ");

            var count = graph.CountTripsWithMaxDistance(start, end, maxDistance);
            Console.WriteLine($"Number of trips from {start} to {end} with distance < {maxDistance}: {count}");
        }

        private static IReadOnlyList<char> ParseRoute(string routeString)
        {
            if (string.IsNullOrWhiteSpace(routeString))
                throw new ArgumentException("Route string cannot be empty.", nameof(routeString));

            routeString = routeString.Trim();

            // Support "A-B-C" or "ABC"
            string[] parts;
            if (routeString.Contains('-'))
            {
                parts = routeString.Split('-', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                parts = routeString.Select(c => c.ToString()).ToArray();
            }

            var result = new List<char>();
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (trimmed.Length == 0)
                    continue;

                var ch = trimmed[0];
                if (!char.IsLetter(ch))
                    throw new ArgumentException($"Invalid academy identifier: '{trimmed}'");

                result.Add(char.ToUpperInvariant(ch));
            }

            if (result.Count < 2)
                throw new ArgumentException("Route must contain at least two academies.");

            return result;
        }

        private static char ReadAcademy(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    var ch = input.Trim()[0];
                    if (char.IsLetter(ch))
                    {
                        return char.ToUpperInvariant(ch);
                    }
                }

                Console.WriteLine("Invalid academy. Please enter a letter like A, B, C...");
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();

                if (int.TryParse(input, out var value) && value > 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid number. Please enter a positive integer.");
            }
        }
    }
}
