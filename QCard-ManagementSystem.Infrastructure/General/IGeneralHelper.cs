

using QCard.ManagementSystem.Domain.Object;

namespace QCard.ManagementSystem.Infrastructure.General
{
    public interface IGeneralHelper
    {
        ConfigObject? GetDataConfig();
        string GenerateID();
        string RemoveSpecialCharacters(string value);
        bool OnlyNumberCharacter(string value);
        bool OnlyNumber(string value);
        string NumberToRupiah(decimal angka);
        string NumberToRupiahWithoutPrefix(decimal angka);
        string RemoveSpecialChar(string input);
    }
}

