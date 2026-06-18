using System;
using System.Collections.Generic;
using System.Text;

namespace CypherCompiler
{
    public enum TokenType : byte
    {
        KeywordFn, KeywordStruct, KeywordClass, KeywordEnum,
        KeywordInt, KeywordFloat, KeywordString, KeywordBool, KeywordChar,
        KeywordVoid, KeywordTrue, KeywordFalse,
        KeywordIf, KeywordElse, KeywordFor, KeywordIn,
        KeywordReturn, KeywordBreak, KeywordNext,
        KeywordPublic, KeywordPrivate, KeywordProtected, KeywordInternal,
        KeywordAbstract, KeywordInterface, KeywordForm,
        KeywordVirtual, KeywordOverride, KeywordBase, KeywordThis,
        KeywordStatic, KeywordConst, KeywordReadonly,
        KeywordUse, KeywordSpace,

        Identifier, IntLiteral, FloatLiteral, StringLiteral, CharLiteral,

        Plus, Minus, Asterisk, ForwardSlash, Percent,
        PlusAssign, MinusAssign, MultiplyAssign, DivideAssign, PercentAssign,
        Assign, Equal, NotEqual, LessThan, GreaterThan,
        LessThanOrEqual, GreaterThanOrEqual,
        PlusPlus, MinusMinus, LogicalAnd, LogicalOr, LogicalNot,
        Ampersand, Bar, Caret, Tilde, LeftShift, RightShift,
        BitwiseAndAssign, BitwiseOrAssign, BitwiseXorAssign,
        LeftShiftAssign, RightShiftAssign,

        OpenParen, CloseParen, OpenBrace, CloseBrace, OpenBracket, CloseBracket,
        Semicolon, Comma, Dot, BackSlash, EndOfFile, Unknown
    }
    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public Token(TokenType TypeParam, string ValueParam, int LineParam, int ColumnParam)
        {
            Type = TypeParam;
            Value = ValueParam;
            Line = LineParam;
            Column = ColumnParam;
        }
        public override string ToString()
        {
            return $"[{Type}] '{Value}' at Line {Line}, Column {Column}";
        }
    }
}
