These are my solutions to [Advent of Code](https://adventofcode.com/) puzzles.

Many of the solutions have been improved over time as I have discovered better techniques
or made improvements to the toolkit. Although most of the solutions have roughly remained the same.

## AdventToolkit
This is the collection of utilities, extensions, wrappers, and algorithms that I have accumulated
throughout my time working on the puzzles. The overall goal of the toolkit is to have algorithms
available for common data manipulation, so that I can spend more time focusing on the problem itself
rather than the underlying implementations used to solve the problem. The second focus is to be able
to write solutions in a more concise way.

### AdventToolkit v2
Slightly before AoC 2023 began, I started on the first major overhaul to the toolkit
(by overhaul I mean remaking it from scratch). I started with a few goals in mind:
* Rework any code patterns that I think could be improved.
  * This could be anything from just trying a new design to aligning the code with the other goals in this list.
* Use new language features that been released since the toolkit was made.
  * Generic math just to name one. Previously, many methods needed to be copied with separate `int` and `long`
  versions. Now, there can be one method that supports pretty much any number type.
* Increase the performance where possible.
  * This is just a passive rule to keep in mind while developing the helpful methods. An example
  of this is processing data directly rather than trying to write every operation using LINQ methods.
  * Preconditions are checked using `Debug.Assert`. If anything goes wrong, a quick switch over to Debug
  configuration will reveal where the data was not as expected. Once everything is correct, the checks
  will be omitted when it comes time to run benchmarks.
* Add documentation so myself or others can come back and understand what everything does.

### Organization
The top level folders organize the toolkit into sections for each capability.
* **&lt;Top Level&gt;**: Contains the base classes used to create puzzles.
* **Calc**: Extensions and algorithms that don't fit in other sections.
* **Collections**: Implementations of containers for storing data.
* **Data**: Wrapper types.
* **Debugging**: Utilities for error handling and debugging.
* **Extensions**: For extensions on builtin types. Extensions on toolkit types should be placed
within the corresponding section folder.
* **Graphing**: Types for creating graphs.
* **Parsing**: Framework designed to allow a concise way to extract data from inputs.
* **Reflect**: Utilities to aid with reflection.
* **Space**: Utilities to work with various spacial dimensions (2D, 3D, etc.).
* **Util**: Uncategorized utilities.