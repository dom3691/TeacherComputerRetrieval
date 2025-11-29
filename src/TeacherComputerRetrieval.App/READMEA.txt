1. Design overview
Problem modelling

The school network is modelled as a directed, weighted graph:

Each academy is a node (represented as a char, e.g. A, B, C).

Each route between academies is a directed edge with a positive integer distance, e.g. "AB5" = from A to B with distance 5.

The graph is stored using an adjacency list:
private readonly Dictionary<char, List<Edge>> _adjacency;


-	TeacherComputerRetrieval.Core
i.	Pure domain logic:
ii.	Graph: graph representation and algorithms.
iii.	GraphParser: parses input like "AB5, BC4, ..." into a Graph.

-	TeacherComputerRetrieval.App
Console application that:
i. Reads the route definition from the user (or uses a default one).
ii. Provides an interactive menu to run queries on the graph.

-	TeacherComputerRetrieval.Tests
i.	NUnit test project that verifies all required outputs from the sample network.
ii.	This separation keeps the core algorithms reusable, testable, and independent from the console UI.

HOW TO RUN:

1.	Build the solution in Visual Studio or using dotnet CLI.
2.	Run the TeacherComputerRetrieval.App project.
3.	Follow the prompts to enter the route definitions or use the default.
4.	Select queries from the menu to see results based on the sample network.
5.	To run tests, execute the TeacherComputerRetrieval.Tests project using your test runner of choice.
This structure ensures a clean separation of concerns, making the codebase maintainable and extensible for future enhancements.
