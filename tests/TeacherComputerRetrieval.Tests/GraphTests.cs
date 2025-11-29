using NUnit.Framework;
using TeacherComputerRetrieval.Core;

namespace TeacherComputerRetrieval.Tests
{
    [TestFixture]
    public class GraphTests
    {
        private Graph _graph;

        [SetUp]
        public void SetUp()
        {
            _graph = GraphParser.ParseEdges("AB5, BC4, CD8, DC8, DE6, AD5, CE2, EB3, AE7");
        }

        [Test]
        public void DistanceOfRoute_ABC_Is9()
        {
            var route = new[] { 'A', 'B', 'C' };
            var distance = _graph.GetRouteDistance(route);

            Assert.That(distance, Is.EqualTo(9));
        }

        [Test]
        public void DistanceOfRoute_AEBCD_Is22()
        {
            var route = new[] { 'A', 'E', 'B', 'C', 'D' };
            var distance = _graph.GetRouteDistance(route);

            Assert.That(distance, Is.EqualTo(22));
        }

        [Test]
        public void DistanceOfRoute_AED_IsNoSuchRoute()
        {
            var route = new[] { 'A', 'E', 'D' };
            var distance = _graph.GetRouteDistance(route);

            Assert.That(distance, Is.Null);
        }

        [Test]
        public void TripsFromCToC_WithMax3Stops_Is2()
        {
            var result = _graph.CountTripsWithMaxStops('C', 'C', 3);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void TripsFromAToC_WithExactly4Stops_Is3()
        {
            var result = _graph.CountTripsWithExactStops('A', 'C', 4);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void ShortestRouteFromAToC_Is9()
        {
            var result = _graph.FindShortestDistance('A', 'C');

            Assert.That(result, Is.EqualTo(9));
        }

        [Test]
        public void ShortestRouteFromBToB_Is9()
        {
            var result = _graph.FindShortestDistance('B', 'B');

            Assert.That(result, Is.EqualTo(9));
        }

        [Test]
        public void RoutesFromCToC_WithDistanceLessThan30_Is7()
        {
            var result = _graph.CountTripsWithMaxDistance('C', 'C', 30);

            Assert.That(result, Is.EqualTo(7));
        }
    }
}
