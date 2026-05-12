namespace Lms.Application.Common.Interfaces
{
    public interface IPasswordGenerator
    {
        string Generate(int length=8);
    }
}
