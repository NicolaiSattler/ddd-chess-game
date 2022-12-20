using Chess.Core.Match.ValueObjects;

namespace Chess.Domain.ValueObjects;

public static class Extensions
{
    public static File ChangeFile(this File file, int num)
    {
        if (num < -7 && num > 7) throw new ArgumentException("Cannot change more the 7 files");

        if ((num < 0 && file == File.A) || (num > 0 && file == File.H)) return File.Undefined;

        var fileAsInt = (int)file;
        var newFile = fileAsInt + num;

        return (File)newFile;
    }
}