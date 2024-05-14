using System.Text.RegularExpressions;
using AdventToolkit.New.Collections;
using AdventToolkit.New.Parsing.Builtin;
using AdventToolkit.New.Parsing.Interface;
using AdventToolkit.New.Reflect;

namespace AdventToolkit.New.Parsing.Context;

/// <summary>
/// Allows parsing a string using a regex.
/// </summary>
public class RegexParse : IParserLookup
{
    /// <summary>
    /// Attempt to convert the input value to a regex parser.
    /// </summary>
    /// <param name="value">Parse format value.</param>
    /// <param name="regex">Result regex.</param>
    /// <param name="matchExtract">Instance of <see cref="MatchExtract{TResult}"/></param>
    /// <param name="resultType">Output type of the regex parse.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool AsRegex<T>(T value, out Regex regex, out IParser matchExtract, out Type resultType)
    {
        // string or regex can be used as regex
        if (value is not Regex r)
        {
            if (value is not string s)
            {
                regex = default!;
                matchExtract = default!;
                resultType = default!;
                return false;
            }

            r = new Regex(s, RegexOptions.Compiled);
        }

        regex = r;

        // Output type will have one enumerable for each group in the regex
        var groups = regex.GetGroupNumbers().Length - 1;
        var start = groups > 0 ? 1 : 0;
        groups = Math.Max(groups, 1);
        
        resultType = groups > 1 ? Types.CreateTupleType(typeof(IEnumerable<string>), groups) : typeof(IEnumerable<string>);
        matchExtract = typeof(MatchExtract<>).NewParserGeneric([resultType], start, groups);
        return true;
    }

    /// <summary>
    /// Read the options that will be applied to the regex parse.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool ReadOptions(string data)
    {
        return data.Contains('+');
    }
    
    public bool TryLookup<T>(Type inputType, T value, string extra, IParseContext context, out IParser parser)
    {
        // Regex parse can be used when the input type is a string
        if (inputType == typeof(string) && AsRegex(value, out var regex, out var extract, out var resultType))
        {
            var collectionMode = ReadOptions(extra);
            var parserType = collectionMode ? typeof(MatchMany<>) : typeof(MatchSingle<>);
            parser = parserType.NewParserGeneric([resultType], regex, extract);
            return true;
        }

        // Regex parse can be used if the input type is a sequence of strings
        if (ParseUtil.TryGetInnerType(inputType, context, out var inner, out var selector)
            && inner == typeof(string)
            && AsRegex(value, out regex, out extract, out resultType))
        {
            var collectionMode = ReadOptions(extra);
            var parserType = collectionMode ? typeof(MatchMany<>) : typeof(MatchSingle<>);
            var matcher = parserType.NewParserGeneric([resultType], regex, extract);
            parser = ParseAdapt.MaybeJoin(selector, SelectAdapter.Create(matcher));
            return true;
        }

        parser = default!;
        return false;
    }

    /// <summary>
    /// Parser which extracts the groups out of a regex match.
    ///
    /// The output will be IEnumerable&lt;string&gt; if the count is 1.
    /// The output will be a tuple of IEnumerable&lt;string&gt; if count > 1.
    /// </summary>
    /// <param name="start">First group to extract.</param>
    /// <param name="count">Number of groups to extract.</param>
    /// <typeparam name="TResult"></typeparam>
    public class MatchExtract<TResult>(int start, int count) : IParser<Match, TResult>, IParser<MatchCollection, IEnumerable<TResult>>
    {
        public IEnumerable<string> Captures(Match match, int group)
        {
            foreach (Capture capture in match.Groups[group].Captures)
            {
                yield return capture.Value;
            }
        }
        
        public TResult Parse(Match input)
        {
            if (count == 1) return (TResult) Captures(input, start);
            
            using Arr<object?> args = new(count);
            for (var i = 0; i < count; i++)
            {
                args[i] = Captures(input, i + start);
            }
            return (TResult) Types.CreateTupleFrom(typeof(TResult), args);
        }

        public IEnumerable<TResult> Parse(MatchCollection input)
        {
            foreach (Match m in input)
            {
                yield return Parse(m);
            }
        }
    }

    /// <summary>
    /// A match extract that finds a single match.
    /// </summary>
    /// <seealso cref="Regex.Match(string)"/>
    /// <param name="regex"></param>
    /// <param name="extract"></param>
    /// <typeparam name="TResult"></typeparam>
    public class MatchSingle<TResult>(Regex regex, MatchExtract<TResult> extract) : IParser<string, TResult>
    {
        public TResult Parse(string input) => extract.Parse(regex.Match(input));

        public IEnumerable<IParser> GetChildren()
        {
            yield return extract;
        }
    }

    /// <summary>
    /// A match extract that finds all non-overlapping matches.
    /// </summary>
    /// <seealso cref="Regex.Matches(string)"/>
    /// <param name="regex"></param>
    /// <param name="extract"></param>
    /// <typeparam name="TResult"></typeparam>
    public class MatchMany<TResult>(Regex regex, MatchExtract<TResult> extract) : IParser<string, IEnumerable<TResult>>
    {
        public IEnumerable<TResult> Parse(string input) => extract.Parse(regex.Matches(input));

        public IEnumerable<IParser> GetChildren()
        {
            yield return extract;
        }
    }
}