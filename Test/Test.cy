use System;
space CypherCompiler;//Space nghĩa giống namespace trong C#
public interface ICalculable
{
    fn float Compute();
}
abstract class BaseCalculator
{
    protected float Multiplier = 1.5;
    public abstract fn Log();//ko cần ghi void, bỏ trống là tự động hiểu là void
}
struct Point
{
    public int X;
    public int Y;
}
enum State from int
{
    Idle,
    Running
}
public class AdvancedCalculator from BaseCalculator, ICalculable
{
    private readonly int CoreId = 42;
    private static string EngineName = "CypherCore";
    private bool IsActive;
    private char GroupChar;
    public property float TaxRate//thêm từ khoá vô, một tính năng khác là nếu viết property mà ko mở ngoặc nhọn để viết lại get set thì tự động tạo getter và setter và field ẩn
    {
        get { return this.Multiplier; }
        set { this.Multiplier = value; }
    }
    public override fn Log()//bỏ trống ko ghi void
    {
        int Total = 0;
        float Delta = 0.0;
        string Message = "Hello \"World\"\n";
        char Letter = 'A';
        char Escape = '\n';
        this.IsActive = true;
        this.IsActive = false;
        if (Total == 0 && !this.IsActive)
        {
            Total = 100;
        }
        else
        {
            Total = -1;
        }
        
        for (int i = 0; i < 10; i++)
        {
            if (i == 2)
            {
                next;
            }
            if (i == 4)
            {
                break;
            }
            Total++;
            Total--;
        }
        Total += 10;
        Total -= 5;
        Total *= 2;
        Total /= 2;
        Total %= 3;
        bool Condition = Total != 5 || Delta < 1.0 || Total > 0 || Delta <= 0.5 || Total >= 10;//chỗ này ko cần ngoặc cũng dc
        int Bitwise = 1 & 2 | 3 ^ 4;
        int Shift = (1 << 2) >> 1;
        int Mask = ~0;
        Bitwise &= 1;
        Bitwise |= 2;
        Bitwise ^= 3;
        Shift <<= 1;
        Shift >>= 1;
        Point P;
        P.X = 10;
        string Path = "C:\\Cypher";
    }
    public virtual fn float Compute()
    {
        return base.Multiplier * 2.0;
    }
}
