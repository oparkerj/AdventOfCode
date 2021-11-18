using System;
using System.Collections.Generic;
using System.Linq;
using AdventToolkit.Extensions;

namespace AdventToolkit.Utilities.Automata
{
    public class StateMachine<TUpdate>
    {
        private int _nextState;
        internal readonly Dictionary<int, Links<TUpdate>> Table = new();
        internal readonly HashSet<int> AcceptingStates = new();

        public Links<TUpdate> this[int id] => Table.GetOrSetValue(id, () => new Links<TUpdate>());

        public int InitialState => default;

        public int Length => _nextState;

        public void RecalculateNext()
        {
            if (Table.Count == 0)
            {
                _nextState = 0;
            }
            else
            {
                _nextState = Math.Max(Table.Keys.Max(), Table.Values.SelectMany(links => links.Possible.Select(link => link.To)).Max()) + 1;
            }
        }

        public int NewState()
        {
            var state = _nextState;
            _nextState++;
            return state;
        }

        public void IncludeState(int state)
        {
            if (state >= _nextState) _nextState = state + 1;
        }

        /// <summary>
        /// Try to match the given inputs.
        /// </summary>
        /// <param name="input">Inputs to match against.</param>
        /// <param name="path">The path used to match the input. Contains each index matched and which
        /// state matched that index. Will be non-empty if there is a match. Last element is the index after
        /// the last index matched, with the associated accepting state.</param>
        /// <param name="backtrack">A point to continue matching from. If the value is null, will start
        /// matching from the beginning.</param>
        /// <param name="start">Index in the input to start matching from.</param>
        /// <returns>True if the input matches this machine, false otherwise.</returns>
        public bool Match(TUpdate[] input, out List<(int state, int index)> path, ref Backtrack<TUpdate> backtrack, int start = 0)
        {
            StateList<(int state, int index)> list;
            // If starting a new search, set up
            if (backtrack == default)
            {
                list = new StateList<(int state, int index)>();
                backtrack = new Backtrack<TUpdate>(list);
                if (Table.TryGetValue(InitialState, out var startLinks) && startLinks.Count > 0)
                {
                    list.Save();
                    for (var index = startLinks.Possible.Count - 1; index >= 0; index--)
                    {
                        var link = startLinks.Possible[index];
                        backtrack.Next.Push(new MatchPoint<TUpdate>(InitialState, link, start));
                    }
                    backtrack.Groups.Push(startLinks.Count);
                }
            }
            else list = backtrack.Path;
            path = list;
            // Explore paths in order
            while (backtrack.HasNext)
            {
                // Next point to try and match from, also decrement group count
                var matchPoint = backtrack.Next.Pop();
                var groupCount = backtrack.Groups.Pop() - 1;
                if (groupCount > 0) backtrack.Groups.Push(groupCount);
                var linkBacktrack = matchPoint.Backtrack;
                if (matchPoint.Link == null || !matchPoint.Link.TryMatch(input, matchPoint.Index, out var end, ref linkBacktrack))
                {
                    // Don't backtrack until we explore all possibilities at the current state
                    if (groupCount > 0) continue;
                    if (AcceptingStates.Contains(matchPoint.State))
                    {
                        list.Add((matchPoint.State, matchPoint.Index));
                        return true;
                    }
                    // Explore a different branch
                    list.Restore();
                    continue;
                }
                // If the link left a place to continue from, add it to the backtrack points
                if (linkBacktrack is {HasNext: true})
                {
                    if (groupCount > 0) backtrack.Groups.Push(backtrack.Groups.Pop() + 1);
                    else backtrack.Groups.Push(1);
                    backtrack.Next.Push(matchPoint.WithBacktrack(linkBacktrack));
                }
                // Add characters that were consumed by accepting this link
                for (var i = matchPoint.Index; i < end; i++)
                {
                    list.Add((matchPoint.Index, i));
                }
                list.Save();
                if (Table.TryGetValue(matchPoint.Link.To, out var links) && links.Count > 0)
                {
                    // Add possibilities of state we just moved to
                    for (var i = links.Possible.Count - 1; i >= 0; i--)
                    {
                        var nextLink = links.Possible[i];
                        backtrack.Next.Push(new MatchPoint<TUpdate>(matchPoint.Link.To, nextLink, end));
                    }
                    backtrack.Groups.Push(links.Count);
                }
                else
                {
                    // Next state is a dead end, so check if we are done
                    if (AcceptingStates.Contains(matchPoint.Link.To))
                    {
                        list.Add((matchPoint.Link.To, end));
                        return true;
                    }
                    list.Release();
                }
            }
            // Ran out of possibilities to check
            if (list.Count == 0) list.Add((InitialState, start));
            return false;
        }

        public bool Step(int state, TUpdate update, out int newState)
        {
            newState = default;
            if (!Table.TryGetValue(state, out var links)) return false;
            var matched = links.FirstOrDefault(link => link.TryMatchOne(update));
            if (matched == null) return false;
            newState = matched.To;
            return true;
        }
    }
}