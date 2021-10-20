namespace AdventToolkit.Utilities
{
    public readonly struct Token
    {
        public readonly string Content;
        public readonly TokenType Type;

        public Token(string content, TokenType type)
        {
            Content = content;
            Type = type;
        }

        public void Deconstruct(out string content, out TokenType type)
        {
            content = Content;
            type = Type;
        }
    }
    
    public enum TokenType
    {
        None,
        Number,
        Word,
        Symbol,
        Whitespace,
    }
}