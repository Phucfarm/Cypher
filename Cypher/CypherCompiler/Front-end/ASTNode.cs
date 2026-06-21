using System;
using System.Collections.Generic;
namespace CypherCompiler
{
    #region Base Nodes
    public abstract class AstNode
    {

    }
    public abstract class ExpressionNode : AstNode
    {

    }
    public abstract class StatementNode : AstNode
    {

    }
    public abstract class DeclarationNode : StatementNode
    {

    }
    // Thằng này chuyên bọc thông tin về kiểu dữ liệu (ví dụ: 'int', 'float', 'Point'). LLVM cực kỳ cần cái này để định hình vùng nhớ.
    public class TypeSyntax : AstNode
    {
        public Token TypeToken { get; init; }
        public TypeSyntax(Token TypeTokenParam)
        {
            TypeToken = TypeTokenParam;
        }
    }
    #endregion
    #region Declaration Nodes
    // Thằng này để chứa cái đống 'use System;' ở đầu file nhằm lôi thư viện vào xài.
    public class UseDeclarationNode : DeclarationNode
    {
        public Token UseToken { get; init; }
        public Token IdentifierToken { get; init; }
        public UseDeclarationNode(Token UseTokenParam, Token IdentifierTokenParam)
        {
            UseToken = UseTokenParam;
            IdentifierToken = IdentifierTokenParam;
        }
    }
    // Thằng này dành riêng cho kiểu viết tắt 'space Name;', có hiệu lực gán phân vùng cho toàn bộ file code phía sau nó.
    public class FileSpaceDeclarationNode : DeclarationNode
    {
        public Token SpaceToken { get; init; }
        public Token IdentifierToken { get; init; }
        public FileSpaceDeclarationNode(Token SpaceTokenParam, Token IdentifierTokenParam)
        {
            SpaceToken = SpaceTokenParam;
            IdentifierToken = IdentifierTokenParam;
        }
    }
    // Thằng này chuyên trị kiểu bọc ngoặc nhọn 'space Name { ... }'. Thằng LLVM cực cần cái List<AstNode> Members bên trong để gom nhóm symbol chính xác.
    public class BlockSpaceDeclarationNode : DeclarationNode
    {
        public Token SpaceToken { get; init; }
        public Token IdentifierToken { get; init; }
        public List<AstNode> Members { get; init; }
        public BlockSpaceDeclarationNode(Token SpaceTokenParam, Token IdentifierTokenParam, List<AstNode> MembersParam)
        {
            SpaceToken = SpaceTokenParam;
            IdentifierToken = IdentifierTokenParam;
            Members = MembersParam;
        }
    }
    // Thằng đa năng bọc class, struct, interface. Lưu KeywordToken để phân biệt loại thực thể (Class/Struct/Interface) xuống LLVM sinh mã vùng nhớ tương ứng.
    public class TypeDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token KeywordToken { get; init; } // Bắt trúng KeywordClass, KeywordStruct, KeywordInterface
        public Token IdentifierToken { get; init; }
        public List<TypeSyntax> BaseTypes { get; init; }
        public List<AstNode> Members { get; init; }
        public TypeDeclarationNode(List<Token> ModifiersParam, Token KeywordTokenParam, Token IdentifierTokenParam, List<TypeSyntax> BaseTypesParam, List<AstNode> MembersParam)
        {
            Modifiers = ModifiersParam;
            KeywordToken = KeywordTokenParam;
            IdentifierToken = IdentifierTokenParam;
            BaseTypes = BaseTypesParam;
            Members = MembersParam;
        }
    }
    // Thằng riêng biệt dành cho Enum. Bắt cả EnumToken và FromToken để LLVM định hình UnderlyingType tĩnh.
    public class EnumDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token EnumToken { get; init; }
        public Token IdentifierToken { get; init; }
        public Token FromToken { get; init; } // Bắt KeywordFrom phục vụ cú pháp 'from int'
        public TypeSyntax UnderlyingType { get; init; }
        public List<EnumMemberSyntax> Members { get; init; }
        public EnumDeclarationNode(List<Token> ModifiersParam, Token EnumTokenParam, Token IdentifierTokenParam, Token FromTokenParam, TypeSyntax UnderlyingTypeParam, List<EnumMemberSyntax> MembersParam)
        {
            Modifiers = ModifiersParam;
            EnumToken = EnumTokenParam;
            IdentifierToken = IdentifierTokenParam;
            FromToken = FromTokenParam;
            UnderlyingType = UnderlyingTypeParam;
            Members = MembersParam;
        }
    }
    // Thằng lính lác bọc từng phần tử trong Enum (ví dụ: 'Idle', 'Running'). Có thể kèm giá trị gán cứng nếu cần.
    public class EnumMemberSyntax : AstNode
    {
        public Token IdentifierToken { get; init; }
        public ExpressionNode Value { get; init; }
        public EnumMemberSyntax(Token IdentifierTokenParam, ExpressionNode ValueParam)
        {
            IdentifierToken = IdentifierTokenParam;
            Value = ValueParam;
        }
    }
    // Thằng này chuyên bọc hàm khởi tạo (Constructor). Tên trùng tên Class, không có kiểu trả về, LLVM rất cần để chạy hàm setup vùng nhớ.
    public class ConstructorDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token IdentifierToken { get; init; }
        public List<ParameterSyntax> Parameters { get; init; }
        public AstNode Body { get; init; }
        public ConstructorDeclarationNode(List<Token> ModifiersParam, Token IdentifierTokenParam, List<ParameterSyntax> ParametersParam, AstNode BodyParam)
        {
            Modifiers = ModifiersParam;
            IdentifierToken = IdentifierTokenParam;
            Parameters = ParametersParam;
            Body = BodyParam;
        }
    }
    // Thằng này để bọc mấy cái hàm (method). ReturnType được chuyển thành TypeSyntax (có thể null nếu void) để LLVM gen hàm chính xác.
    public class MethodDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token FnToken { get; init; }
        public TypeSyntax ReturnType { get; init; }
        public Token IdentifierToken { get; init; }
        public List<ParameterSyntax> Parameters { get; init; }
        public AstNode Body { get; init; }
        public MethodDeclarationNode(List<Token> ModifiersParam, Token FnTokenParam, TypeSyntax ReturnTypeParam, Token IdentifierTokenParam, List<ParameterSyntax> ParametersParam, AstNode BodyParam)
        {
            Modifiers = ModifiersParam;
            FnToken = FnTokenParam;
            ReturnType = ReturnTypeParam;
            IdentifierToken = IdentifierTokenParam;
            Parameters = ParametersParam;
            Body = BodyParam;
        }
    }
    // Thằng bọc thông tin từng tham số truyền vào hàm. Giờ dùng TypeSyntax chuẩn chỉ cho LLVM tạo chữ ký hàm (Function Signature).
    public class ParameterSyntax : AstNode
    {
        public TypeSyntax Type { get; init; }
        public Token IdentifierToken { get; init; }
        public ParameterSyntax(TypeSyntax TypeParam, Token IdentifierTokenParam)
        {
            Type = TypeParam;
            IdentifierToken = IdentifierTokenParam;
        }
    }
    // Thằng chuyên trị biến thành viên (Field). Lưu chặt Type và biểu thức khởi tạo (Initializer) để LLVM tính toán offset bộ nhớ khi cấp phát struct/class.
    public class FieldDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public TypeSyntax Type { get; init; }
        public Token IdentifierToken { get; init; }
        public ExpressionNode Initializer { get; init; }
        public FieldDeclarationNode(List<Token> ModifiersParam, TypeSyntax TypeParam, Token IdentifierTokenParam, ExpressionNode InitializerParam)
        {
            Modifiers = ModifiersParam;
            Type = TypeParam;
            IdentifierToken = IdentifierTokenParam;
            Initializer = InitializerParam;
        }
    }
    // Thằng property. Giữ danh sách Accessors để tách biệt rõ ràng khối hàm get/set ẩn, khớp mượt với KeywordGet và KeywordSet.
    public class PropertyDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token PropertyToken { get; init; }
        public TypeSyntax Type { get; init; }
        public Token IdentifierToken { get; init; }
        public List<AccessorDeclarationSyntax> Accessors { get; init; }
        public PropertyDeclarationNode(List<Token> ModifiersParam, Token PropertyTokenParam, TypeSyntax TypeParam, Token IdentifierTokenParam, List<AccessorDeclarationSyntax> AccessorsParam)
        {
            Modifiers = ModifiersParam;
            PropertyToken = PropertyTokenParam;
            Type = TypeParam;
            IdentifierToken = IdentifierTokenParam;
            Accessors = AccessorsParam;
        }
    }
    // Thằng bọc riêng từng khối get hoặc set bên trong Property. Khớp chính xác với KeywordGet và KeywordSet.
    public class AccessorDeclarationSyntax : AstNode
    {
        public List<Token> Modifiers { get; init; }
        public Token KeywordToken { get; init; } // Nhận KeywordGet hoặc KeywordSet
        public BlockStatementNode Body { get; init; }
        public AccessorDeclarationSyntax(List<Token> ModifiersParam, Token KeywordTokenParam, BlockStatementNode BodyParam)
        {
            Modifiers = ModifiersParam;
            KeywordToken = KeywordTokenParam;
            Body = BodyParam;
        }
    }
    #endregion
    #region Expression Nodes
    // Thằng bọc các hằng số, chuỗi, ký tự, boolean hoặc null (ví dụ: 42, 3.14, "hello", true, null). LLVM sinh mã hằng (constant) cực nhanh qua thằng này.
    public class LiteralExpressionNode : ExpressionNode
    {
        public Token LiteralToken { get; init; }
        public LiteralExpressionNode(Token LiteralTokenParam)
        {
            LiteralToken = LiteralTokenParam;
        }
    }
    // Thằng bọc tên biến, tên hàm hoặc thực thể (ví dụ: x, MyVariable). Dùng để tra cứu địa chỉ ô nhớ trong Symbol Table khi phát sinh mã IR.
    public class IdentifierExpressionNode : ExpressionNode
    {
        public Token IdentifierToken { get; init; }
        public IdentifierExpressionNode(Token IdentifierTokenParam)
        {
            IdentifierToken = IdentifierTokenParam;
        }
    }
    // Thằng xử lý toán tử một ngôi (ví dụ: -x, !flag, ~mask). Giữ token toán tử để biết lệnh LLVM tương ứng (neg, not).
    public class UnaryExpressionNode : ExpressionNode
    {
        public Token OperatorToken { get; init; }
        public ExpressionNode Operand { get; init; }
        public UnaryExpressionNode(Token OperatorTokenParam, ExpressionNode OperandParam)
        {
            OperatorToken = OperatorTokenParam;
            Operand = OperandParam;
        }
    }
    // Thằng xử lý toán tử hậu tố / tiền tố tăng giảm (ví dụ: i++, --count). Tách biệt hẳn để sinh mã tối ưu hóa thanh ghi hoặc bước lặp.
    public class UpdateExpressionNode : ExpressionNode
    {
        public Token OperatorToken { get; init; }
        public ExpressionNode Operand { get; init; }
        public bool IsPostfix { get; init; }
        public UpdateExpressionNode(Token OperatorTokenParam, ExpressionNode OperandParam, bool IsPostfixParam)
        {
            OperatorToken = OperatorTokenParam;
            Operand = OperandParam;
            IsPostfix = IsPostfixParam;
        }
    }
    // Thằng gánh team tính toán nhị phân (ví dụ: a + b, x * y, i == 0). Gom hai vế Left/Right để Binder check ép kiểu và LLVM gen lệnh toán hạng.
    public class BinaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; init; }
        public Token OperatorToken { get; init; }
        public ExpressionNode Right { get; init; }
        public BinaryExpressionNode(ExpressionNode LeftParam, Token OperatorTokenParam, ExpressionNode RightParam)
        {
            Left = LeftParam;
            OperatorToken = OperatorTokenParam;
            Right = RightParam;
        }
    }
    // Thằng xử lý các biểu thức gán (ví dụ: x = 10, gán phức hợp x += 5). Tách biệt vế trái (thường là LValue) để LLVM biết địa chỉ cần Store dữ liệu vào.
    public class AssignmentExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; init; }
        public Token OperatorToken { get; init; }
        public ExpressionNode Right { get; init; }
        public AssignmentExpressionNode(ExpressionNode LeftParam, Token OperatorTokenParam, ExpressionNode RightParam)
        {
            Left = LeftParam;
            OperatorToken = OperatorTokenParam;
            Right = RightParam;
        }
    }
    // Thằng bọc biểu thức gọi hàm (ví dụ: Print(x, 1), Calculate()). Giữ Target (có thể là tên hàm hoặc con trỏ hàm) và danh sách tham số truyền vào.
    public class InvocationExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; init; }
        public List<ExpressionNode> Arguments { get; init; }
        public InvocationExpressionNode(ExpressionNode TargetParam, List<ExpressionNode> ArgumentsParam)
        {
            Target = TargetParam;
            Arguments = ArgumentsParam;
        }
    }
    // Thằng truy cập thành viên qua dấu chấm (ví dụ: obj.Member, math.PI). Cần thiết để LLVM tính toán offset hoặc xuất lệnh trích xuất phần tử.
    public class MemberAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; init; }
        public Token OperatorToken { get; init; }
        public Token MemberToken { get; init; }
        public MemberAccessExpressionNode(ExpressionNode ExpressionParam, Token OperatorTokenParam, Token MemberTokenParam)
        {
            Expression = ExpressionParam;
            OperatorToken = OperatorTokenParam;
            MemberToken = MemberTokenParam;
        }
    }
    // Thằng truy cập mảng bằng chỉ mục (ví dụ: array[i], map["key"]). Giữ biểu thức gốc và biểu thức nằm trong ngoặc vuông để sinh lệnh GEP của LLVM.
    public class IndexAccessExpressionNode : ExpressionNode
    {
        public ExpressionNode Target { get; init; }
        public Token OpenBracketToken { get; init; }
        public ExpressionNode Index { get; init; }
        public Token CloseBracketToken { get; init; }
        public IndexAccessExpressionNode(ExpressionNode TargetParam, Token OpenBracketTokenParam, ExpressionNode IndexParam, Token CloseBracketTokenParam)
        {
            Target = TargetParam;
            OpenBracketToken = OpenBracketTokenParam;
            Index = IndexParam;
            CloseBracketToken = CloseBracketTokenParam;
        }
    }
    // Thằng xử lý ép kiểu tường minh dạng an toàn không ném lỗi (ví dụ: obj as Point). Trả về đối tượng hoặc null nếu fail.
    public class AsExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; init; }
        public Token AsToken { get; init; }
        public TypeSyntax TargetType { get; init; }
        public AsExpressionNode(ExpressionNode ExpressionParam, Token AsTokenParam, TypeSyntax TargetTypeParam)
        {
            Expression = ExpressionParam;
            AsToken = AsTokenParam;
            TargetType = TargetTypeParam;
        }
    }
    // Thằng kiểm tra kiểu dữ liệu (ví dụ: obj is Point). Trả về kiểu boolean (true/false) để LLVM tối ưu hóa rẽ nhánh rập khuôn.
    public class IsExpressionNode : ExpressionNode
    {
        public ExpressionNode Expression { get; init; }
        public Token IsToken { get; init; }
        public TypeSyntax TargetType { get; init; }
        public IsExpressionNode(ExpressionNode ExpressionParam, Token IsTokenParam, TypeSyntax TargetTypeParam)
        {
            Expression = ExpressionParam;
            IsToken = IsTokenParam;
            TargetType = TargetTypeParam;
        }
    }
    // Thằng xử lý ép kiểu tường minh truyền thống (ví dụ: (int)myFloat). Giúp tầng Binder xác thực ép kiểu an toàn trước khi hạ xuống LLVM sinh lệnh conversion.
    public class CastExpressionNode : ExpressionNode
    {
        public Token OpenParenToken { get; init; }
        public TypeSyntax TargetType { get; init; }
        public Token CloseParenToken { get; init; }
        public ExpressionNode Expression { get; init; }
        public CastExpressionNode(Token OpenParenTokenParam, TypeSyntax TargetTypeParam, Token CloseParenTokenParam, ExpressionNode ExpressionParam)
        {
            OpenParenToken = OpenParenTokenParam;
            TargetType = TargetTypeParam;
            CloseParenToken = CloseParenTokenParam;
            Expression = ExpressionParam;
        }
    }
    // Thằng bọc biểu thức khởi tạo vùng nhớ mới (ví dụ: new Point(10, 20)). Giữ Type để biết size cần cấp phát và Arguments để gọi trúng Constructor.
    public class NewExpressionNode : ExpressionNode
    {
        public Token NewToken { get; init; }
        public TypeSyntax Type { get; init; }
        public List<ExpressionNode> Arguments { get; init; }
        public NewExpressionNode(Token NewTokenParam, TypeSyntax TypeParam, List<ExpressionNode> ArgumentsParam)
        {
            NewToken = NewTokenParam;
            Type = TypeParam;
            Arguments = ArgumentsParam;
        }
    }
    // Thằng bọc từ khóa ngữ cảnh đại diện đối tượng hiện tại (this) hoặc lớp cha (base).
    public class ThisOrBaseExpressionNode : ExpressionNode
    {
        public Token KeywordToken { get; init; }
        public ThisOrBaseExpressionNode(Token KeywordTokenParam)
        {
            KeywordToken = KeywordTokenParam;
        }
    }
    // Thằng bọc biểu thức nằm trong dấu ngoặc đơn (ví dụ: (a + b)). Giúp Parser giữ đúng độ ưu tiên toán tử trước khi hạ cây xuống LLVM.
    public class ParenthesizedExpressionNode : ExpressionNode
    {
        public Token OpenParenToken { get; init; }
        public ExpressionNode Expression { get; init; }
        public Token CloseParenToken { get; init; }
        public ParenthesizedExpressionNode(Token OpenParenTokenParam, ExpressionNode ExpressionParam, Token CloseParenTokenParam)
        {
            OpenParenToken = OpenParenTokenParam;
            Expression = ExpressionParam;
            CloseParenToken = CloseParenTokenParam;
        }
    }
    #endregion
    #region Statement Nodes
    // Cặp ngoặc nhọn thần thánh { ... } gom cụm các lệnh cục bộ. LLVM nhìn vào đây để quản lý scope (phạm vi sóng) của các biến.
    public class BlockStatementNode : StatementNode
    {
        public Token OpenBraceToken { get; init; }
        public List<StatementNode> Statements { get; init; }
        public Token CloseBraceToken { get; init; }
        public BlockStatementNode(Token OpenBraceTokenParam, List<StatementNode> StatementsParam, Token CloseBraceTokenParam)
        {
            OpenBraceToken = OpenBraceTokenParam;
            Statements = StatementsParam;
            CloseBraceToken = CloseBraceTokenParam;
        }
    }
    // Khai báo biến cục bộ trong hàm (ví dụ: int x = 5; hoặc const float PI = 3.14;). LLVM sẽ dùng lệnh 'alloca' để cấp phát trên Stack.
    public class LocalVariableDeclarationStatementNode : StatementNode
    {
        public List<Token> Modifiers { get; init; } // Cho phép bắt 'const', 'readonly' cục bộ
        public TypeSyntax Type { get; init; }
        public Token IdentifierToken { get; init; }
        public ExpressionNode Initializer { get; init; }
        public LocalVariableDeclarationStatementNode(List<Token> ModifiersParam, TypeSyntax TypeParam, Token IdentifierTokenParam, ExpressionNode InitializerParam)
        {
            Modifiers = ModifiersParam;
            Type = TypeParam;
            IdentifierToken = IdentifierTokenParam;
            Initializer = InitializerParam;
        }
    }
    // Biến một biểu thức thành một câu lệnh hợp lệ (ví dụ: x = 5; hoặc DoSomething();). Đuôi chấm phẩy biến Expression thành Statement.
    public class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; init; }
        public Token SemicolonToken { get; init; }
        public ExpressionStatementNode(ExpressionNode ExpressionParam, Token SemicolonTokenParam)
        {
            Expression = ExpressionParam;
            SemicolonToken = SemicolonTokenParam;
        }
    }
    // Câu lệnh điều kiện rẽ nhánh (if - else). Gồm biểu thức điều kiện (Condition), nhánh đúng (ThenStatement) và nhánh sai tùy chọn (ElseStatement). LLVM cần để sinh các nhãn nhảy (Branch Instructions).
    public class IfStatementNode : StatementNode
    {
        public Token IfToken { get; init; }
        public ExpressionNode Condition { get; init; }
        public StatementNode ThenStatement { get; init; }
        public Token ElseToken { get; init; }
        public StatementNode ElseStatement { get; init; }
        public IfStatementNode(Token IfTokenParam, ExpressionNode ConditionParam, StatementNode ThenStatementParam, Token ElseTokenParam, StatementNode ElseStatementParam)
        {
            IfToken = IfTokenParam;
            Condition = ConditionParam;
            ThenStatement = ThenStatementParam;
            ElseToken = ElseTokenParam;
            ElseStatement = ElseStatementParam;
        }
    }
    // 1. Vòng lặp vô tận: for { ... }
    // LLVM chỉ cần bắn lệnh nhảy vô điều kiện (br) lặp lại chính nó.
    public class InfiniteLoopStatementNode : StatementNode
    {
        public Token ForToken { get; init; }
        public StatementNode Body { get; init; }
        public InfiniteLoopStatementNode(Token ForTokenParam, StatementNode BodyParam)
        {
            ForToken = ForTokenParam;
            Body = BodyParam;
        }
    }
    // 2. Vòng lặp While-like: for (true) { ... } hoặc for (x < 5) { ... }
    // Chỉ chứa một biểu thức điều kiện duy nhất.
    public class WhileLoopStatementNode : StatementNode
    {
        public Token ForToken { get; init; }
        public ExpressionNode Condition { get; init; }
        public StatementNode Body { get; init; }
        public WhileLoopStatementNode(Token ForTokenParam, ExpressionNode ConditionParam, StatementNode BodyParam)
        {
            ForToken = ForTokenParam;
            Condition = ConditionParam;
            Body = BodyParam;
        }
    }
    // 3. Vòng lặp For truyền thống: for (int i = 0; i < 5; i++) { ... }
    // Tách rõ 2 dạng khởi tạo để đảm bảo type-safety: hoặc khai báo biến mới, hoặc dùng biểu thức có sẵn (vd: for (i = 0; ...)). Chỉ một trong hai field có giá trị, field còn lại là null.
    public class ForLoopStatementNode : StatementNode
    {
        public Token ForToken { get; init; }
        public LocalVariableDeclarationStatementNode DeclarationInitializer { get; init; }
        public ExpressionStatementNode ExpressionInitializer { get; init; }
        public ExpressionNode Condition { get; init; }
        public ExpressionNode Iterator { get; init; }
        public StatementNode Body { get; init; }
        public ForLoopStatementNode(Token ForTokenParam, LocalVariableDeclarationStatementNode DeclarationInitializerParam, ExpressionStatementNode ExpressionInitializerParam, ExpressionNode ConditionParam, ExpressionNode IteratorParam, StatementNode BodyParam)
        {
            ForToken = ForTokenParam;
            DeclarationInitializer = DeclarationInitializerParam;
            ExpressionInitializer = ExpressionInitializerParam;
            Condition = ConditionParam;
            Iterator = IteratorParam;
            Body = BodyParam;
        }
    }
    // 4. Vòng lặp Duyệt mảng/Bộ sưu tập (Foreach-like): for (var item in collection) { ... }
    // Cần giữ biến chạy (VariableType + Identifier) và tập hợp đích để sinh mã duyệt qua con trỏ hoặc GetEnumerator.
    public class ForeachLoopStatementNode : StatementNode
    {
        public Token ForToken { get; init; }
        public TypeSyntax VariableType { get; init; } // Có thể là 'var' hoặc kiểu tường minh 'int'
        public Token IdentifierToken { get; init; }
        public Token InToken { get; init; }
        public ExpressionNode Collection { get; init; }
        public StatementNode Body { get; init; }
        public ForeachLoopStatementNode(Token ForTokenParam, TypeSyntax VariableTypeParam, Token IdentifierTokenParam, Token InTokenParam, ExpressionNode CollectionParam, StatementNode BodyParam)
        {
            ForToken = ForTokenParam;
            VariableType = VariableTypeParam;
            IdentifierToken = IdentifierTokenParam;
            InToken = InTokenParam;
            Collection = CollectionParam;
            Body = BodyParam;
        }
    }
    // Câu lệnh kiểm soát try-catch-finally. LLVM sẽ dựa vào cấu trúc này để sinh các khối đáp ứng Landing Pad của cơ chế Zero-cost Exception Handling (giống Clang C++).
    public class TryStatementNode : StatementNode
    {
        public Token TryToken { get; init; }
        public BlockStatementNode TryBlock { get; init; }
        public List<CatchClauseSyntax> CatchClauses { get; init; }
        public Token FinallyToken { get; init; }
        public BlockStatementNode FinallyBlock { get; init; } // Có thể null nếu không khai báo block finally
        public TryStatementNode(Token TryTokenParam, BlockStatementNode TryBlockParam, List<CatchClauseSyntax> CatchClausesParam, Token FinallyTokenParam, BlockStatementNode FinallyBlockParam)
        {
            TryToken = TryTokenParam;
            TryBlock = TryBlockParam;
            CatchClauses = CatchClausesParam;
            FinallyToken = FinallyTokenParam;
            FinallyBlock = FinallyBlockParam;
        }
    }
    // Mệnh đề bắt lỗi trong cụm Try. Theo cú pháp Cypher, CatchType và IdentifierToken hoàn toàn có thể null (Ví dụ case: catch { ... } gom toàn bộ lỗi).
    public class CatchClauseSyntax : AstNode
    {
        public Token CatchToken { get; init; }
        public TypeSyntax CatchType { get; init; } // Null nếu ghi 'catch { ... }'
        public Token IdentifierToken { get; init; } // Null nếu ghi 'catch { ... }' hoặc catch không gán biến
        public BlockStatementNode Block { get; init; }
        public CatchClauseSyntax(Token CatchTokenParam, TypeSyntax CatchTypeParam, Token IdentifierTokenParam, BlockStatementNode BlockParam)
        {
            CatchToken = CatchTokenParam;
            CatchType = CatchTypeParam;
            IdentifierToken = IdentifierTokenParam;
            Block = BlockParam;
        }
    }
    // Câu lệnh ném ra một Exception. Hỗ trợ ném bất kỳ kiểu dữ liệu nào (int, float, string...). 
    public class ThrowStatementNode : StatementNode
    {
        public Token ThrowToken { get; init; }
        public ExpressionNode Expression { get; init; } // Chứa biểu thức giá trị bị ném ra
        public ThrowStatementNode(Token ThrowTokenParam, ExpressionNode ExpressionParam)
        {
            ThrowToken = ThrowTokenParam;
            Expression = ExpressionParam;
        }
    }
    // Câu lệnh giải phóng bộ nhớ thủ công cho đối tượng/vùng nhớ con trỏ. Khớp hoàn toàn với KeywordDelete.
    public class DeleteStatementNode : StatementNode
    {
        public Token DeleteToken { get; init; } // Bắt KeywordDelete
        public ExpressionNode Expression { get; init; }
        public DeleteStatementNode(Token DeleteTokenParam, ExpressionNode ExpressionParam)
        {
            DeleteToken = DeleteTokenParam;
            Expression = ExpressionParam;
        }
    }
    // Lệnh nhảy thoát khỏi hàm (return) kèm giá trị trả về tùy chọn. LLVM dựa vào đây để bắn lệnh 'ret'.
    public class ReturnStatementNode : StatementNode
    {
        public Token ReturnToken { get; init; }
        public ExpressionNode Expression { get; init; }
        public ReturnStatementNode(Token ReturnTokenParam, ExpressionNode ExpressionParam)
        {
            ReturnToken = ReturnTokenParam;
            Expression = ExpressionParam;
        }
    }
    // Lệnh bẻ gãy vòng lặp (break). LLVM sẽ phát lệnh nhảy không điều kiện tới nhãn ngay sau vòng lặp hiện tại.
    public class BreakStatementNode : StatementNode
    {
        public Token BreakToken { get; init; }
        public BreakStatementNode(Token BreakTokenParam)
        {
            BreakToken = BreakTokenParam;
        }
    }
    // Lệnh nhảy qua bước (next - tương đương continue trong C#). LLVM sẽ nhảy về nhãn kiểm tra điều kiện hoặc nhãn tăng biến đếm của vòng lặp.
    public class NextStatementNode : StatementNode
    {
        public Token NextToken { get; init; }
        public NextStatementNode(Token NextTokenParam)
        {
            NextToken = NextTokenParam;
        }
    }
    #endregion
}