using AdventOfCode2023.Puzzles;
using AdventToolkit2;
using AdventToolkit2.Parsing.Core;

// Run puzzles using the old toolkit
// PuzzleBase.Run<Day1New>();

// Run puzzles using the new toolkit
DefaultContext.AddCommonTypes();
// DefaultContext.AddToolkitTypes();
PuzzleRunner.Run<Day1New>();