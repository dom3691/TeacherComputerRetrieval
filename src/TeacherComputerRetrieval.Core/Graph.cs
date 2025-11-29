using System;
using System.Collections.Generic;
using System.Linq;

namespace TeacherComputerRetrieval.Core
{
    /// <summary>
    /// Directed, weighted graph of academies and routes between them.
    /// </summary>
    public sealed class Graph
    {
        private readonly Dictionary<char, List<Edge>> _adjacency = new();

        /// <summary>
        /// Adds a directed edge (route) to the graph.
        /// </summary>
        public void AddEdge(char from, char to, int distance)
        {
            if (distance <= 0)
                throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be positive.");

            if (from == to)
                throw new ArgumentException("From and To academies must be different.", nameof(to));

            if (!_adjacency.TryGetValue(from, out var edgesFrom))
            {
                edgesFrom = new List<Edge>();
                _adjacency[from] = edgesFrom;
            }

            if (edgesFrom.Any(e => e.To == to))
            {
                throw new InvalidOperationException($"Route {from}{to} already exists.");
            }

            edgesFrom.Add(new Edge(to, distance));

            // Ensure destination vertex is known even if it has no outgoing edges.
            if (!_adjacency.ContainsKey(to))
            {
                _adjacency[to] = new List<Edge>();
            }
        }

        /// <summary>
        /// Computes the distance of an explicit route (sequence of academies),
        /// or returns null if the route does not exist.
        /// </summary>
        public int? GetRouteDistance(IReadOnlyList<char> route)
        {
            if (route == null) throw new ArgumentNullException(nameof(route));
            if (route.Count < 2) return 0;

            var distance = 0;

            for (var i = 0; i < route.Count - 1; i++)
            {
                var from = route[i];
                var to = route[i + 1];

                if (!_adjacency.TryGetValue(from, out var edges))
                    return null;

                var edge = edges.FirstOrDefault(e => e.To == to);
                if (edge == null)
                    return null;

                distance += edge.Distance;
            }

            return distance;
        }

        /// <summary>
        /// Counts trips from start to end with at most maxStops edges.
        /// </summary>
        public int CountTripsWithMaxStops(char start, char end, int maxStops)
        {
            if (maxStops < 1) return 0;
            return CountTripsWithMaxStopsInternal(start, end, 0, maxStops);
        }

        private int CountTripsWithMaxStopsInternal(char current, char end, int stopsSoFar, int maxStops)
        {
            if (stopsSoFar > maxStops) return 0;

            var count = 0;
            if (stopsSoFar > 0 && current == end)
            {
                count++;
            }

            if (!_adjacency.TryGetValue(current, out var edges)) return count;

            foreach (var edge in edges)
            {
                count += CountTripsWithMaxStopsInternal(edge.To, end, stopsSoFar + 1, maxStops);
            }

            return count;
        }

        /// <summary>
        /// Counts trips from start to end with exactly exactStops edges.
        /// </summary>
        public int CountTripsWithExactStops(char start, char end, int exactStops)
        {
            if (exactStops < 1) return 0;
            return CountTripsWithExactStopsInternal(start, end, 0, exactStops);
        }

        private int CountTripsWithExactStopsInternal(char current, char end, int stopsSoFar, int exactStops)
        {
            if (stopsSoFar > exactStops) return 0;

            if (stopsSoFar == exactStops)
            {
                return current == end ? 1 : 0;
            }

            if (!_adjacency.TryGetValue(current, out var edges)) return 0;

            var count = 0;
            foreach (var edge in edges)
            {
                count += CountTripsWithExactStopsInternal(edge.To, end, stopsSoFar + 1, exactStops);
            }

            return count;
        }

        /// <summary>
        /// Finds the length of the shortest route from start to end (in distance).
        /// Returns null if no route exists. For start == end, finds shortest cycle.
        /// </summary>
        public int? FindShortestDistance(char start, char end)
        {
            if (!_adjacency.ContainsKey(start) || !_adjacency.ContainsKey(end))
                return null;

            //  handling when we want the shortest cycle starting and ending at the same node.
            if (start == end)
            {
                if (!_adjacency.TryGetValue(start, out var outEdges) || outEdges.Count == 0)
                {
                    return null;
                }

                var shortest = int.MaxValue;

                foreach (var edge in outEdges)
                {
                    // Leave 'start' via edge, then find shortest path back to 'start'.
                    var backDistance = FindShortestDistanceInternal(edge.To, start);
                    if (backDistance.HasValue)
                    {
                        var total = edge.Distance + backDistance.Value;
                        if (total < shortest)
                        {
                            shortest = total;
                        }
                    }
                }

                return shortest == int.MaxValue ? (int?)null : shortest;
            }

            return FindShortestDistanceInternal(start, end);
        }

        /// <summary>
        /// Standard Dijkstra (O(V^2)) between different start and end vertices.
        /// </summary>
        private int? FindShortestDistanceInternal(char start, char end)
        {
            var vertices = _adjacency.Keys.ToList();
            var distances = vertices.ToDictionary(v => v, _ => int.MaxValue);
            var visited = new HashSet<char>();

            distances[start] = 0;

            while (visited.Count < vertices.Count)
            {
                char? currentOpt = null;
                var minDistance = int.MaxValue;

                // find the unvisited vertex with
                // the smallest current distance.
                foreach (var v in vertices)
                {
                    if (visited.Contains(v)) continue;

                    if (distances[v] < minDistance)
                    {
                        minDistance = distances[v];
                        currentOpt = v;
                    }
                }

                if (currentOpt == null || minDistance == int.MaxValue)
                {
                    // remaining vertices are unreachable.
                    break;
                }

                var current = currentOpt.Value;
                visited.Add(current);

                if (current == end)
                {
                    return minDistance;
                }

                if (!_adjacency.TryGetValue(current, out var edges)) continue;

                foreach (var edge in edges)
                {
                    if (visited.Contains(edge.To)) continue;

                    var candidate = minDistance + edge.Distance;
                    if (candidate < distances[edge.To])
                    {
                        distances[edge.To] = candidate;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Counts trips from start to end with total distance strictly less than maxDistance.
        /// </summary>
        public int CountTripsWithMaxDistance(char start, char end, int maxDistance)
        {
            if (maxDistance <= 0) return 0;
            return CountTripsWithMaxDistanceInternal(start, end, 0, maxDistance);
        }

        private int CountTripsWithMaxDistanceInternal(char current, char end, int distanceSoFar, int maxDistance)
        {
            if (distanceSoFar >= maxDistance) return 0;

            var count = 0;

            if (distanceSoFar > 0 && current == end)
            {
                count++;
                //continue exploring, as longer cycles may still be < maxDistance.
            }

            if (!_adjacency.TryGetValue(current, out var edges)) return count;

            foreach (var edge in edges)
            {
                var newDistance = distanceSoFar + edge.Distance;
                if (newDistance < maxDistance)
                {
                    count += CountTripsWithMaxDistanceInternal(edge.To, end, newDistance, maxDistance);
                }
            }

            return count;
        }

        private sealed class Edge
        {
            public char To { get; }
            public int Distance { get; }

            public Edge(char to, int distance)
            {
                To = to;
                Distance = distance;
            }
        }
    }
}
