using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RegExtract;

namespace AdventToolkit.Utilities;

public class StringMatcher
{
    private List<IStringMatcher> _matchers = new();

    // If true, exception is thrown if no match.
    public bool Strict = false;

    public StringMatcher Add(IStringMatcher matcher)
    {
        _matchers.Add(matcher);
        return this;
    }

    public IStringMatcher Match(string s)
    {
        bool Condition(IStringMatcher matcher) => matcher.TryMatch(s);
        
        return Strict ? _matchers.First(Condition) : _matchers.FirstOrDefault(Condition);
    }
}

public interface IStringMatcher
{
    bool TryMatch(string s);
}

public interface IStringHandler
{
    void Handle(string s);
}

public static class StringMatcherExtensions
{
    public static bool Handle(this StringMatcher matcher, string s)
    {
        var m = matcher.Match(s);
        if (m is not IStringHandler handler) return false;
        handler.Handle(s);
        return true;
    }

    public static StringMatcher AddRegex<T>(this StringMatcher matcher, string pattern, Action<T> action)
    {
        var handler = new RegexStringExtractor<T>(pattern, action);
        return matcher.Add(handler);
    }
    
    public static StringMatcher AddRegex<T1, T2>(this StringMatcher matcher, string pattern, Action<T1, T2> action)
    {
        return matcher.AddRegex<(T1, T2)>(pattern, tuple => action(tuple.Item1, tuple.Item2));
    }
    
    public static StringMatcher AddRegex<T1, T2, T3>(this StringMatcher matcher, string pattern, Action<T1, T2, T3> action)
    {
        return matcher.AddRegex<(T1, T2, T3)>(pattern, tuple => action(tuple.Item1, tuple.Item2, tuple.Item3));
    }

    public static StringMatcher AddPrefix<T>(this StringMatcher matcher, string prefix, string pattern, Action<T> action)
    {
        var handler = new RegexStringExtractor<T>(pattern, action);
        var m = new PrefixStringMatcher(prefix, handler);
        return matcher.Add(m);
    }

    public static StringMatcher AddPrefix<T1, T2>(this StringMatcher matcher, string prefix, string pattern, Action<T1, T2> action)
    {
        return matcher.AddPrefix<(T1, T2)>(prefix, pattern, tuple => action(tuple.Item1, tuple.Item2));
    }
    
    public static StringMatcher AddPrefix<T1, T2, T3>(this StringMatcher matcher, string prefix, string pattern, Action<T1, T2, T3> action)
    {
        return matcher.AddPrefix<(T1, T2, T3)>(prefix, pattern, tuple => action(tuple.Item1, tuple.Item2, tuple.Item3));
    }

    public static StringMatcher AddPrefixType<T>(this StringMatcher matcher, string prefix, Action<T> action, bool trim = true)
    {
        var t = typeof(T);

        var length = prefix.Length;
        Func<string, string> getInput = trim ? s => s[length..].Trim() : s => s[length..];

        Func<string, TC> Converter<TC>(Func<string, TC> parser) => s => parser(getInput(s));

        if (t == typeof(int) && action is Action<int> ai) return matcher.Add(new PrefixTypeHandler<int>(prefix, Converter(int.Parse), ai, trim));
        if (t == typeof(char) && action is Action<char> ac) return matcher.Add(new PrefixTypeHandler<char>(prefix, Converter(char.Parse), ac, trim));
        throw new Exception("Unsupported type");
    }
}

public class PrefixStringMatcher : IStringMatcher, IStringHandler
{
    public readonly string Prefix;
    public readonly IStringHandler Handler;

    public PrefixStringMatcher(string prefix, IStringHandler handler = null)
    {
        Prefix = prefix;
        Handler = handler;
    }

    public bool TryMatch(string s) => s.StartsWith(Prefix);

    public virtual void Handle(string s) => Handler?.Handle(s);
}

public class RegexStringExtractor<T> : IStringMatcher, IStringHandler
{
    private Regex _regex;
    private ExtractionPlan<T> _plan;
    
    private Action<T> _action;

    public RegexStringExtractor(string pattern, Action<T> action)
    {
        _regex = new Regex(pattern, RegexOptions.Compiled);
        _plan = ExtractionPlan<T>.CreatePlan(_regex);
        _action = action;
    }

    public bool TryMatch(string s) => _regex.IsMatch(s);

    public void Handle(string s) => _action?.Invoke(_plan.Extract(s));
}

public class PrefixTypeHandler<T> : PrefixStringMatcher
{
    public readonly Func<string, T> Converter;
    public readonly Action<T> Action;

    public PrefixTypeHandler(string prefix, Func<string, T> converter, Action<T> action, bool trim = true) : base(prefix)
    {
        Converter = converter;
        Action = action;
    }

    public override void Handle(string s)
    {
        Action(Converter(s));
    }
}

