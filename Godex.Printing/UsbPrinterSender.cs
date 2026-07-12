using System.Runtime.InteropServices;
using System.Text;

namespace Godex.Printing;

// Отправка EZPL-команд на принтер, подключённый как локальный порт Windows (USB).
// printerName — это то же имя, что видно в "Устройства и принтеры" (например "GoDEX GE330").
public sealed class UsbPrinterSender
{
    static UsbPrinterSender()
    {
        // Windows-1251 не входит в .NET по умолчанию, в отличие от .NET Framework —
        // без этой регистрации Encoding.GetEncoding(1251) бросит NotSupportedException.
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public void Send(string printerName, string ezplText)
    {
        // Кодируем той же кодовой страницей, что задаём принтеру командой ^XSET,CODEPAGE,16
        // в EzplLabelGenerator — иначе кириллица на этикетке превратится в кракозябры.
        var bytes = Encoding.GetEncoding(1251).GetBytes(ezplText);

        var unmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
        try
        {
            Marshal.Copy(bytes, 0, unmanagedBytes, bytes.Length);

            var success = RawPrinterHelper.SendBytesToPrinter(printerName, unmanagedBytes, bytes.Length);
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"Не удалось отправить данные на принтер «{printerName}» (код ошибки Windows: {errorCode}).");
            }
        }
        finally
        {
            Marshal.FreeCoTaskMem(unmanagedBytes);
        }
    }
}
