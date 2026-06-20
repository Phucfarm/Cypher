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
    // Thằng đa năng bọc class, struct, interface. Lưu đầy đủ modifier, danh sách cha (BaseTypes) để sau này LLVM tạo cấu trúc bảng ảo (VTable).
    public class TypeDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token KeywordToken { get; init; }
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
    // Thằng riêng biệt dành cho Enum. LLVM bắt buộc phải biết kiểu nền (UnderlyingType, ví dụ 'int' trong 'from int') để tạo kiểu dữ liệu nguyên thể tương ứng.
    public class EnumDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token EnumToken { get; init; }
        public Token IdentifierToken { get; init; }
        public TypeSyntax UnderlyingType { get; init; }
        public List<EnumMemberSyntax> Members { get; init; }
        public EnumDeclarationNode(List<Token> ModifiersParam, Token EnumTokenParam, Token IdentifierTokenParam, TypeSyntax UnderlyingTypeParam, List<EnumMemberSyntax> MembersParam)
        {
            Modifiers = ModifiersParam;
            EnumToken = EnumTokenParam;
            IdentifierToken = IdentifierTokenParam;
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
    // Thằng property. Giữ nguyên cấu trúc để tầng phân tích ngữ nghĩa (Binder) sau này tự động tách thành các hàm getter/setter ẩn cho LLVM thực thi.
    public class PropertyDeclarationNode : DeclarationNode
    {
        public List<Token> Modifiers { get; init; }
        public Token PropertyToken { get; init; }
        public TypeSyntax Type { get; init; }
        public Token IdentifierToken { get; init; }
        public AstNode Body { get; init; }
        public PropertyDeclarationNode(List<Token> ModifiersParam, Token PropertyTokenParam, TypeSyntax TypeParam, Token IdentifierTokenParam, AstNode BodyParam)
        {
            Modifiers = ModifiersParam;
            PropertyToken = PropertyTokenParam;
            Type = TypeParam;
            IdentifierToken = IdentifierTokenParam;
            Body = BodyParam;
        }
    }
    #endregion
}