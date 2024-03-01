using AdventToolkit.New.Parsing;
using static Testing.TestExtensions;

namespace Testing;

public class ParserTest
{
    public const string IntCsv = "1,2,3,4,5,6";

    public const string CharCsv = "a,b,c,d,e,f";
    
    public const string StrCsv = "one,two,three,four,five";

    public static List<int> IntList => [-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5];

    public static IEnumerable<object[]> Ints => Expand([..IntList]);

    public static IEnumerable<object[]> Csv => Expand(IntCsv, CharCsv, StrCsv);

    public static IEnumerable<object[]> All => [..Csv];

    public static Type Str => typeof(string);

    public static Type Int => typeof(int);
    
    [Theory]
    [MemberData(nameof(All))]
    public void Empty_Identity(string input)
    {
        var result = input.Parse<string>($"");
        Assert.Equal(input, result);
    }

    [Theory]
    [MemberData(nameof(All))]
    public void NullSection_Identity(string input)
    {
        var result = input.Parse<string>($"{null}");
        Assert.Equal(input, result);
    }

    [Fact]
    public void AnchorOnly_Error()
    {
        Assert.Throws<ArgumentException>(() => IntCsv.Parse<string>($","));
    }

    [Theory]
    [MemberData(nameof(Csv))]
    public void TrailAnchor_Split(string input)
    {
        var expected = input[..input.IndexOf(',')];
        var result = input.Parse<string>($"{null},");
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void LeadAnchor_Split(string input)
    {
        var expected = input[(input.IndexOf(',') + 1)..];
        var result = input.Parse<string>($",{null}");
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void StartEndAnchor_Split(string input)
    {
        var parts = input.Split(',');
        var expected = parts[1];
        
        var result = input.Parse<string>($",{null},");
        
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MiddleAnchor_Split(string input)
    {
        var index = input.IndexOf(',');
        var leftExpected = input[..index];
        var rightExpected = input[(index + 1)..];
        
        var (left, right) = input.Parse<(string, string)>($"{null},{null}");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MiddleTrailAnchor_Split(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[0];
        var rightExpected = parts[1];
        
        var (left, right) = input.Parse<(string, string)>($"{null},{null},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void LeadMiddleAnchor_Split(string input)
    {
        var first = input.IndexOf(',');
        var second = input.IndexOf(',', first + 1);
        var leftExpected = input[(first + 1)..second];
        var rightExpected = input[(second + 1)..];
        
        var (left, right) = input.Parse<(string, string)>($",{null},{null}");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void LeadMiddleTrailAnchor_Split(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[1];
        var rightExpected = parts[2];
        
        var (left, right) = input.Parse<(string, string)>($",{null},{null},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiNull_Repeat(string input)
    {
        var (left, right) = input.Parse<(string, string)>($"{null}{null}");
        
        Assert.Equal(input, left);
        Assert.Equal(input, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiNullStart_Repeat(string input)
    {
        var parts = input.Split(',');
        var expected = parts[0];
        
        var (left, right) = input.Parse<(string, string)>($"{null}{null},");
        
        Assert.Equal(expected, left);
        Assert.Equal(expected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiNullEnd_Repeat(string input)
    {
        var expected = input[(input.IndexOf(',') + 1)..];
        
        var (left, right) = input.Parse<(string, string)>($",{null}{null}");
        
        Assert.Equal(expected, left);
        Assert.Equal(expected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void NullThenSection_Identity(string input)
    {
        var result = input.Parse<string>($"{null}{Str}");
        Assert.Equal(input, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void SectionThenNull_Repeat(string input)
    {
        var (left, right) = input.Parse<(string, string)>($"{Str}{null}");
        
        Assert.Equal(input, left);
        Assert.Equal(input, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void SectionNullSection_Repeat(string input)
    {
        var (left, right) = input.Parse<(string, string)>($"{Str}{null}{Str}");
        
        Assert.Equal(input, left);
        Assert.Equal(input, right);
    }

    [Theory]
    [MemberData(nameof(All))]
    public void SingleSection_Identity(string input)
    {
        var result = input.Parse<string>($"{Str}");
        Assert.Equal(input, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void LeadSection_Section(string input)
    {
        var expected = input[..input.IndexOf(',')];
        var result = input.Parse<string>($"{Str},");
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void TrailSection_Section(string input)
    {
        var expected = input[(input.IndexOf(',') + 1)..];
        var result = input.Parse<string>($",{Str}");
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MiddleSection_Section(string input)
    {
        var parts = input.Split(',');
        var expected = parts[1];
        
        var result = input.Parse<string>($",{Str},");
        
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiSectionLead_Sections(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[0];
        var rightExpected = parts[1];
        
        var (left, right) = input.Parse<(string, string)>($"{Str},{Str},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiSectionTrail_Sections(string input)
    {
        var first = input.IndexOf(',');
        var second = input.IndexOf(',', first + 1);
        var leftExpected = input[(first + 1)..second];
        var rightExpected = input[(second + 1)..];
        
        var (left, right) = input.Parse<(string, string)>($",{Str},{Str}");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void MultiSectionMiddle_Sections(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[1];
        var rightExpected = parts[2];
        
        var (left, right) = input.Parse<(string, string)>($",{Str},{Str},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void NullThenSection_Split(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[1];
        var rightExpected = parts[2];
        
        var (left, right) = input.Parse<(string, string)>($",{null},{Str},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }
    
    [Theory]
    [MemberData(nameof(Csv))]
    public void SectionThenNull_Split(string input)
    {
        var parts = input.Split(',');
        var leftExpected = parts[1];
        var rightExpected = parts[2];
        
        var (left, right) = input.Parse<(string, string)>($",{Str},{null},");
        
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
    }

    [Theory]
    [MemberData(nameof(Ints))]
    public void OutputType_Converted(int value)
    {
        var result = value.ToString().Parse<int>($"");
        Assert.Equal(value, result);
    }
    
    [Fact]
    public void OutputTypesSame_Converted()
    {
        var input = string.Join(',', IntList);
        var firstExpected = IntList[0];
        var secondExpected = IntList[1];
        
        var (first, second) = input.Parse<(int, int)>($"{null},{null},");
        
        Assert.Equal(firstExpected, first);
        Assert.Equal(secondExpected, second);
    }
    
    [Fact]
    public void OutputTypesDifferent_Converted()
    {
        var input = string.Join(',', IntList);
        var firstExpected = IntList[0].ToString();
        var secondExpected = IntList[1];
        
        var (first, second) = input.Parse<(string, int)>($"{null},{null},");
        
        Assert.Equal(firstExpected, first);
        Assert.Equal(secondExpected, second);
    }
}