using System;
using System.Collections.Generic;
namespace CypherCompiler
{
    public class Lexer
    {
        private string Source;
        private int Index = 0;
        private int Line = 1;
        private int Column = 1;
        private List<Token> Tokens = new List<Token>();
        public List<Diagnostic> Diagnostics { get; private set; } = new List<Diagnostic>();
        public List<Token> Tokenize(string SourceParam)
        {
            Source = SourceParam;
            Index = 0;
            Line = 1;
            Column = 1;
            Tokens = new List<Token>();
            Diagnostics = new List<Diagnostic>();
            while (Index < Source.Length)
            {
                char CurrentChar = Source[Index];
                if (CheckAndHandleNewLine())
                {
                    continue;
                }
                if (char.IsWhiteSpace(CurrentChar))
                {
                    Column++;
                    Index++;
                    continue;
                }
                if (CurrentChar == '/' && (Index + 1 < Source.Length && (Source[Index + 1] == '/' || Source[Index + 1] == '*')))
                {
                    HandleComments();
                    continue;
                }
                if (CurrentChar == '"')
                {
                    ReadString();
                    continue;
                }
                if (CurrentChar == '\'')
                {
                    ReadChar();
                    continue;
                }
                if (char.IsDigit(CurrentChar))
                {
                    ReadNumber();
                    continue;
                }
                if (char.IsLetter(CurrentChar) || CurrentChar == '_')
                {
                    ReadIdentifierOrKeyword();
                    continue;
                }
                ReadOperator();
                continue;
            }
            Tokens.Add(new Token(TokenType.EndOfFile, "", Line, Column));
            return Tokens;
        }
        private bool CheckAndHandleNewLine()
        {
            ReadOnlySpan<char> Span = Source.AsSpan(Index);
            if (MemoryExtensions.StartsWith(Span, "\r\n"))
            {
                Line++;
                Column = 1;
                Index += 2;
                return true;
            }
            if (MemoryExtensions.StartsWith(Span, "\n") || MemoryExtensions.StartsWith(Span, "\r"))
            {
                Line++;
                Column = 1;
                Index++;
                return true;
            }
            return false;
        }
        private void AddDiagnostic(int StartLine, int StartCol, int EndLine, int EndCol, string Message, string ErrorCode)
        {
            var NewRange = new Range(new Position(StartLine - 1, StartCol - 1), new Position(EndLine - 1, EndCol - 1));
            Diagnostics.Add(new Diagnostic(NewRange, Message, code: ErrorCode));
        }
        private bool IsValidEscapeChar(char C)
        {
            return C == 'n' || C == 't' || C == 'r' || C == '"' || C == '\\' || C == '\'';
        }
        private void ReadOperator()
        {
            char CurrentChar = Source[Index];
            if (CurrentChar == '+')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '+')
                {
                    Tokens.Add(new Token(TokenType.PlusPlus, "++", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.PlusAssign, "+=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Plus, "+", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '-')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '-')
                {
                    Tokens.Add(new Token(TokenType.MinusMinus, "--", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.MinusAssign, "-=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Minus, "-", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '*')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.MultiplyAssign, "*=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Asterisk, "*", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '/')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.DivideAssign, "/=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.ForwardSlash, "/", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '%')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.PercentAssign, "%=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Percent, "%", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '=')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.Equal, "==", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Assign, "=", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '!')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.NotEqual, "!=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.LogicalNot, "!", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '&')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '&')
                {
                    Tokens.Add(new Token(TokenType.LogicalAnd, "&&", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.BitwiseAndAssign, "&=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Ampersand, "&", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '|')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '|')
                {
                    Tokens.Add(new Token(TokenType.LogicalOr, "||", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.BitwiseOrAssign, "|=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Bar, "|", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '^')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.BitwiseXorAssign, "^=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else
                {
                    Tokens.Add(new Token(TokenType.Caret, "^", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '~')
            {
                Tokens.Add(new Token(TokenType.Tilde, "~", Line, Column));
                Index++;
                Column++;
                return;
            }
            if (CurrentChar == '<')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.LessThanOrEqual, "<=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '<')
                {
                    if (Index + 2 < Source.Length && Source[Index + 2] == '=')
                    {
                        Tokens.Add(new Token(TokenType.LeftShiftAssign, "<<=", Line, Column));
                        Index += 3;
                        Column += 3;
                    }
                    else
                    {
                        Tokens.Add(new Token(TokenType.LeftShift, "<<", Line, Column));
                        Index += 2;
                        Column += 2;
                    }
                }
                else
                {
                    Tokens.Add(new Token(TokenType.LessThan, "<", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            if (CurrentChar == '>')
            {
                if (Index + 1 < Source.Length && Source[Index + 1] == '=')
                {
                    Tokens.Add(new Token(TokenType.GreaterThanOrEqual, ">=", Line, Column));
                    Index += 2;
                    Column += 2;
                }
                else if (Index + 1 < Source.Length && Source[Index + 1] == '>')
                {
                    if (Index + 2 < Source.Length && Source[Index + 2] == '=')
                    {
                        Tokens.Add(new Token(TokenType.RightShiftAssign, ">>=", Line, Column));
                        Index += 3;
                        Column += 3;
                    }
                    else
                    {
                        Tokens.Add(new Token(TokenType.RightShift, ">>", Line, Column));
                        Index += 2;
                        Column += 2;
                    }
                }
                else
                {
                    Tokens.Add(new Token(TokenType.GreaterThan, ">", Line, Column));
                    Index++;
                    Column++;
                }
                return;
            }
            TokenType? Type = CurrentChar switch
            {
                '(' => TokenType.OpenParen,
                ')' => TokenType.CloseParen,
                '{' => TokenType.OpenBrace,
                '}' => TokenType.CloseBrace,
                '[' => TokenType.OpenBracket,
                ']' => TokenType.CloseBracket,
                ';' => TokenType.Semicolon,
                ',' => TokenType.Comma,
                '.' => TokenType.Dot,
                '\\' => TokenType.BackSlash,
                _ => null
            };
            if (Type != null)
            {
                Tokens.Add(new Token(Type.Value, CurrentChar.ToString(), Line, Column));
            }
            else
            {
                AddDiagnostic(Line, Column, Line, Column + 1, $"Unexpected character: '{CurrentChar}'", "CY0001");
                Tokens.Add(new Token(TokenType.Unknown, CurrentChar.ToString(), Line, Column));
            }
            Index++;
            Column++;
        }
        private void HandleComments()
        {
            if (Source[Index + 1] == '/')
            {
                while (Index < Source.Length)
                {
                    ReadOnlySpan<char> Span = Source.AsSpan(Index);
                    if (MemoryExtensions.StartsWith(Span, "\r\n") || MemoryExtensions.StartsWith(Span, "\n") || MemoryExtensions.StartsWith(Span, "\r"))
                    {
                        break;
                    }
                    Index++;
                    Column++;
                }
            }
            else if (Source[Index + 1] == '*')
            {
                int StartLine = Line;
                int StartColumn = Column;
                Index += 2;
                Column += 2;
                bool Closed = false;
                while (Index < Source.Length)
                {
                    if (Source[Index] == '*' && (Index + 1 < Source.Length && Source[Index + 1] == '/'))
                    {
                        Index += 2;
                        Column += 2;
                        Closed = true;
                        break;
                    }
                    if (CheckAndHandleNewLine())
                    {
                        continue;
                    }
                    Index++;
                    Column++;
                }
                if (!Closed)
                {
                    AddDiagnostic(StartLine, StartColumn, Line, Column, "Unterminated block comment; expected '*/'", "CY0002");
                    Tokens.Add(new Token(TokenType.Unknown, "/*", StartLine, StartColumn));
                }
            }
        }
        private void ReadString()
        {
            string Value = "";
            int StartLine = Line;
            int StartColumn = Column;
            Index++;
            Column++;
            bool IsClosed = false;
            while (Index < Source.Length)
            {
                char CurrentChar = Source[Index];
                if (CurrentChar == '"')
                {
                    IsClosed = true;
                    Index++;
                    Column++;
                    break;
                }
                if (CurrentChar == '\\')
                {
                    if (Index + 1 < Source.Length)
                    {
                        char EscapeChar = Source[Index + 1];
                        if (!IsValidEscapeChar(EscapeChar))
                        {
                            AddDiagnostic(Line, Column, Line, Column + 2, $"Invalid escape sequence: '\\{EscapeChar}'", "CY0005");
                        }
                        Value += EscapeChar switch
                        {
                            'n' => "\n",
                            't' => "\t",
                            'r' => "\r",
                            '"' => "\"",
                            '\\' => "\\",
                            _ => "\\" + EscapeChar
                        };
                        Index += 2;
                        Column += 2;
                        continue;
                    }
                }
                ReadOnlySpan<char> Span = Source.AsSpan(Index);
                if (MemoryExtensions.StartsWith(Span, "\r\n"))
                {
                    Value += "\r\n";
                    Line++;
                    Column = 1;
                    Index += 2;
                    continue;
                }
                if (MemoryExtensions.StartsWith(Span, "\n") || MemoryExtensions.StartsWith(Span, "\r"))
                {
                    Value += "\n";
                    Line++;
                    Column = 1;
                    Index++;
                    continue;
                }
                Value += Source[Index];
                Column++;
                Index++;
            }
            if (IsClosed)
            {
                Tokens.Add(new Token(TokenType.StringLiteral, Value, StartLine, StartColumn));
            }
            else
            {
                AddDiagnostic(StartLine, StartColumn, Line, Column, "Unterminated string literal; expected '\"'", "CY0003");
                Tokens.Add(new Token(TokenType.Unknown, Value, StartLine, StartColumn));
            }
        }
        private void ReadChar()
        {
            string Value = "";
            int StartLine = Line;
            int StartColumn = Column;
            Index++;
            Column++;
            while (Index < Source.Length && Source[Index] != '\'' && Source[Index] != '\n' && Source[Index] != '\r')
            {
                Value += Source[Index];
                Index++;
                Column++;
            }
            if (Index < Source.Length && Source[Index] == '\'')
            {
                Index++;
                Column++;
                bool IsValidEscape = Value.Length == 2 && Value[0] == '\\' && IsValidEscapeChar(Value[1]);
                bool IsValidNormalChar = Value.Length == 1 && Value[0] != '\\';
                if (IsValidEscape || IsValidNormalChar)
                {
                    Tokens.Add(new Token(TokenType.CharLiteral, Value, StartLine, StartColumn));
                }
                else
                {
                    if (Value.Length == 2 && Value[0] == '\\')
                    {
                        AddDiagnostic(StartLine, StartColumn, Line, Column, $"Invalid escape sequence: '\\{Value[1]}'", "CY0005");
                    }
                    else
                    {
                        AddDiagnostic(StartLine, StartColumn, Line, Column, "Invalid character literal; must contain exactly one character or a valid escape sequence", "CY0004");
                    }
                    Tokens.Add(new Token(TokenType.Unknown, Value, StartLine, StartColumn));
                }
            }
            else
            {
                AddDiagnostic(StartLine, StartColumn, Line, Column, "Unterminated character literal; expected '\''", "CY0006");
                Tokens.Add(new Token(TokenType.Unknown, Value, StartLine, StartColumn));
            }
        }
        private void ReadNumber()
        {
            string Value = "";
            int StartColumn = Column;
            bool IsFloat = false;
            while (Index < Source.Length && char.IsDigit(Source[Index]))
            {
                Value += Source[Index];
                Index++;
                Column++;
            }
            if (Index < Source.Length && Source[Index] == '.' && Index + 1 < Source.Length && char.IsDigit(Source[Index + 1]))
            {
                IsFloat = true;
                Value += Source[Index];
                Index++;
                Column++;
                while (Index < Source.Length && char.IsDigit(Source[Index]))
                {
                    Value += Source[Index];
                    Index++;
                    Column++;
                }
            }
            TokenType Type = IsFloat ? TokenType.FloatLiteral : TokenType.IntLiteral;
            Tokens.Add(new Token(Type, Value, Line, StartColumn));
        }
        private void ReadIdentifierOrKeyword()
        {
            string Value = "";
            int StartColumn = Column;
            while (Index < Source.Length && (char.IsLetterOrDigit(Source[Index]) || Source[Index] == '_'))
            {
                Value += Source[Index];
                Index++;
                Column++;
            }
            TokenType Type = Value switch
            {
                "fn" => TokenType.KeywordFn,
                "struct" => TokenType.KeywordStruct,
                "class" => TokenType.KeywordClass,
                "enum" => TokenType.KeywordEnum,
                "int" => TokenType.KeywordInt,
                "float" => TokenType.KeywordFloat,
                "string" => TokenType.KeywordString,
                "bool" => TokenType.KeywordBool,
                "char" => TokenType.KeywordChar,
                "void" => TokenType.KeywordVoid,
                "if" => TokenType.KeywordIf,
                "else" => TokenType.KeywordElse,
                "for" => TokenType.KeywordFor,
                "static" => TokenType.KeywordStatic,
                "public" => TokenType.KeywordPublic,
                "private" => TokenType.KeywordPrivate,
                "return" => TokenType.KeywordReturn,
                "break" => TokenType.KeywordBreak,
                "next" => TokenType.KeywordNext,
                "true" => TokenType.KeywordTrue,
                "false" => TokenType.KeywordFalse,
                "use" => TokenType.KeywordUse,
                "space" => TokenType.KeywordSpace,
                "in" => TokenType.KeywordIn,
                "const" => TokenType.KeywordConst,
                "readonly" => TokenType.KeywordReadonly,
                "abstract" => TokenType.KeywordAbstract,
                "interface" => TokenType.KeywordInterface,
                "from" => TokenType.KeywordFrom,
                "virtual" => TokenType.KeywordVirtual,
                "override" => TokenType.KeywordOverride,
                "base" => TokenType.KeywordBase,
                "this" => TokenType.KeywordThis,
                "internal" => TokenType.KeywordInternal,
                "protected" => TokenType.KeywordProtected,
                "property" => TokenType.KeywordProperty,
                "get" => TokenType.KeywordGet,
                "set" => TokenType.KeywordSet,
                "delete" => TokenType.KeywordDelete,
                "null" => TokenType.KeywordNull,
                "new" => TokenType.KeywordNew,
                _ => TokenType.Identifier
            };
            Tokens.Add(new Token(Type, Value, Line, StartColumn));
        }
    }
}