using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ResAcc
{
  class Hooks
  {
    // ————————
    // Этот делегает и есть подключаемая процедура.
    private delegate IntPtr keyboardHookProc(int code, IntPtr wParam, IntPtr lParam);
    // ————————
    const int WH_KEYBOARD_LL = 13; // Номер глобального LowLevel-хука на клавиатуру
    const int WM_KEYDOWN = 0x100; // Сообщения нажатия клавиши
    // ————————
    // Создаем один экземпляр процедуры, назначаем ф-цию для делегата
    private static keyboardHookProc _proc = hookProc;
 
    // Объект хука
    private static IntPtr hhook = IntPtr.Zero;
 
    // Это окно, через которое будем показывать флаги
    //private static ShowFlag WindowShowFlag;
 
    // Идентификатор активного потока
    private static int _ProcessId;
 
    // Получаем все установленные в систему языки
    private static InputLanguageCollection _InstalledInputLanguages;
 
    // Текущий язык ввода
    private static string _CurrentInputLanguage;
    // ————————
    public static void SetHook()
    {
      // Получаем handle нужной библиотеки
      IntPtr hInstance = LoadLibrary("User32");
      // Ставим LowLevel-hook, обработки клавиатуры, на все потоки
      hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
    }
    // ————————
    public static void UnHook()
    {
      // Снятие хука
      UnhookWindowsHookEx(hhook);
    }
    // ————————
    // Получение раскладки клавиатуры активного окна
    private static string GetKeyboardLayoutId()
    {
      _InstalledInputLanguages = InputLanguage.InstalledInputLanguages;
 
      // Получаем хендл активного окна
      IntPtr hWnd = GetForegroundWindow();
      // Получаем номер потока активного окна
      int WinThreadProcId = GetWindowThreadProcessId(hWnd, out _ProcessId);
      // Получаем раскладку
      IntPtr KeybLayout = GetKeyboardLayout(WinThreadProcId);
      // Циклом перебираем все установленные языки для проверки идентификатора
      for (int i = 0; i < _InstalledInputLanguages.Count; i++)
      {
        if (KeybLayout == _InstalledInputLanguages[i].Handle)
        {
          _CurrentInputLanguage = _InstalledInputLanguages[i].Culture.ThreeLetterWindowsLanguageName.ToString();
        }
      }
      return _CurrentInputLanguage;
    }
    // —————————
    // Вот и наша подключаемая процедура.
    // Здесь мы будем обрабатывать сообщения клавиатуры
    public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0 && wParam == (IntPtr)0x0101/*WM_KEYUP*/)
      {
        // Получаем handle активного окна
        IntPtr hWnd = GetForegroundWindow();
 
        // Получаем раскладку активного окна
        string NowLanguage = GetKeyboardLayoutId();
        // Совмещаем путь, для получения изображения флага
        //string Path = Settings.FlagsPath + GetKeyboardLayoutId() + ".GIF";
 
        //// Если уже идет показ флага, просто обновляем все значения
        //if (Settings.NowShowFlag)
        //{
        //  // Если сейчас показывается флаг, просто обновляем значение
        //  Settings.LastInputLanguage = NowLanguage;
        //}
        //if (Settings.LastInputLanguage != NowLanguage)
        //{
        //  Settings.NowShowFlag = true;
        //  // Создаем экземпляр нового окна, в котором будем показывать флаг
        //  WindowShowFlag = new ShowFlag(Path, hWnd);
        //  // Изменяем последний полученный язык
        //  Settings.LastInputLanguage = NowLanguage;
 
        //  // Показ флага
        //  WindowShowFlag.Show();
        //}
      }
      return CallNextHookEx(hhook, code, (int)wParam, lParam);
    }
    // —————————
    #region Импорт нужных WinApi-функций
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);
 
    [DllImport("user32.dll")]
    static extern bool UnhookWindowsHookEx(IntPtr hInstance);
 
    [DllImport("user32.dll")]
    static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);
 
    [DllImport("kernel32.dll")]
    static extern IntPtr LoadLibrary(string lpFileName);
 
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int GetWindowThreadProcessId(IntPtr handleWindow, out int lpdwProcessID);
 
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetKeyboardLayout(int WindowsThreadProcessID);
 
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetForegroundWindow();
 
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
    #endregion
  }}
