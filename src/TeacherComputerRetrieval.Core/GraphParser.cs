using System;

namespace TeacherComputerRetrieval.Core
{
    /// <summary>
    /// Parses the input string into a Graph instance.
    /// Example input: "AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7"
    /// </summary>
    public static class GraphParser
    {
        public static Graph ParseEdges(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input must not be empty.", nameof(input));

            var graph = new Graph();
            var segments = input.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (segment.Length < 3)
                    throw new FormatException($"Invalid segment '{segment}'. Expected form AB5.");

                var from = segment[0];
                var to = segment[1];

                var distanceText = segment.Substring(2);
                if (!int.TryParse(distanceText, out var distance))
                {
                    throw new FormatException($"Invalid distance in segment '{segment}'.");
                }

                graph.AddEdge(from, to, distance);
            }

            return graph;
        }
    }
}
