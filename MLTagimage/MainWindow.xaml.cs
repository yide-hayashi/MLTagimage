using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Drawing;
//winAPI
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;
using mainWin;

namespace MLTagimage
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region dim user32
        /// <summary>
        /// get mouse position
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static System.Windows.Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new System.Windows.Point(w32Mouse.X, w32Mouse.Y);
        }

        /*keyboard hook*/
        const int WH_KEYBOARD_LL = 13;
        private static int m_HookHandle = 0;    // Hook handle
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private HookProc m_KbdHookProc;            // 鍵盤掛鉤函式指標
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        // 設置掛鉤.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);


        // 將之前設置的掛鉤移除。記得在應用程式結束前呼叫此函式.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 呼叫下一個掛鉤處理常式（若不這麼做，會令其他掛鉤處理常式失效）.
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();
        #endregion
        public static System.Windows.Point Mp;
        delegate void MLBC(string x);
        MLBC Mlbc;
        bool Formclose = false;


        int spaceispressed = 0;
        int index = 0;
        public MainWindow()
        {
            InitializeComponent();
            Mlbc = new MLBC(MPLBcontain);
            KeyboardWatching();
            Thread th = new Thread(CatchMousePosition);
            th.Start();
            for (var i = 0; i < 30; i++)
            {
                Tag.Items.Add(i.ToString());
            }
            Tag.SelectedIndex = 0;
            showimg.Focusable = false;
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.SelectedPath =  "D:\\素材\\https\\H";
            //AppDomain.CurrentDomain.BaseDirectory
            path.Description = "請選擇圖片所在資料夾";
            path.ShowDialog();
            imgdir.Content = path.SelectedPath;
            PathFilesRead();
        

        }
        private void PathFilesRead()
        {
            try
            {
                imglist.Items.Clear();
                string[] filed = Directory.GetFiles(imgdir.Content.ToString());
                foreach (var file in filed)
                {
                    FileInfo f = new FileInfo(file);
                    ListBoxItem lbi = new ListBoxItem();
                    imgsubName imgsubName = new imgsubName();
                    foreach(var i in imgsubName.subName)
                    {
                        if (f.Extension ==i)
                        {
                            lbi.Content = f.Name;
                            //new string[] { f.Name, f.Name, f.Extension };
                            imglist.Items.Add(lbi);
                            break;
                        }
                    }
                    

                }
            }
            catch (Exception ex) { }
        }

        private void imglist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileStream fstream = new FileStream(imgdir.Content.ToString() + "\\" + (imglist.SelectedItem as ListBoxItem).Content.ToString(), FileMode.Open);
            BitmapImage bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.StreamSource = fstream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            

            if (bitmap.PixelHeight != bitmap.Height && bitmap.PixelWidth != bitmap.Height)
            {
                MemoryStream ms = new MemoryStream();
                fstream.CopyTo(ms);
                Bitmap bmp = CreateThumbnail((Stream)ms, bitmap.PixelWidth, bitmap.PixelHeight);
                showimg.BeginInit(); //要開啟
                showimg.Stretch = Stretch.None;
                showimg.Width = bitmap.PixelWidth;
                showimg.Height = bitmap.PixelHeight;
                showimg.Source = BitmapToBitmapSource(bmp);
                showimg.EndInit(); //關閉
            }
            else
            {
                showimg.BeginInit(); //要開啟
                showimg.Stretch = Stretch.None;
                showimg.Width = bitmap.PixelWidth;
                showimg.Height = bitmap.PixelHeight;
                showimg.Source = bitmap;
                showimg.EndInit(); //關閉
            }
            fstream.Close();


            //            WPF引入了統一資源標識Uri(Unified Resource Identifier)來標識和訪問資源。
            //其中較為常見的情況是用Uri加載圖像。Uri表達式的一般形式為：協議 + 授權 + 路徑
            //協議：pack://
            //            授權：有兩種。一種用於訪問編譯時已經知道的文件，用application:///
            //            一種用於訪問編譯時不知道、運行時才知道的文件，用siteoforigin:///

        }
        /// 重新改變圖片大小
        /// </summary>
        /// <param name="BitmapStream">原始圖片串流</param>
        /// <param name="lnWidth">希望寬度範圍</param>
        /// <param name="lnHeight">希望高度範圍</param>
        /// <returns>長寬改變後的新圖片</returns>
        public static Bitmap CreateThumbnail(Stream BitmapStream, int lnWidth, int lnHeight)
        {

            System.Drawing.Bitmap bmpOut = null;
            try
            {
                Bitmap loBMP = new Bitmap(BitmapStream);
                System.Drawing.Imaging.ImageFormat loFormat = loBMP.RawFormat;
                decimal lnRatio;
                int lnNewWidth = 0;
                int lnNewHeight = 0;

                //*** If the image is smaller than a thumbnail just return it
                if (loBMP.Width < lnWidth && loBMP.Height < lnHeight)
                    return loBMP;

                if (loBMP.Width > loBMP.Height)
                {
                    lnRatio = (decimal)lnWidth / loBMP.Width;
                    lnNewWidth = lnWidth;
                    decimal lnTemp = loBMP.Height * lnRatio;
                    lnNewHeight = (int)lnTemp;
                }
                else
                {
                    lnRatio = (decimal)lnHeight / loBMP.Height;
                    lnNewHeight = lnHeight;
                    decimal lnTemp = loBMP.Width * lnRatio;
                    lnNewWidth = (int)lnTemp;
                }

                // System.Drawing.Image imgOut = 
                //      loBMP.GetThumbnailImage(lnNewWidth,lnNewHeight,
                //                              null,IntPtr.Zero);
                // *** This code creates cleaner (though bigger) thumbnails and properly
                // *** and handles GIF files better by generating a white background for
                // *** transparent images (as opposed to black)

                bmpOut = new Bitmap(lnNewWidth, lnNewHeight);
                Graphics g = Graphics.FromImage(bmpOut);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, lnNewWidth, lnNewHeight);
                g.DrawImage(loBMP, 0, 0, lnNewWidth, lnNewHeight);

                loBMP.Dispose();
            }
            catch
            {
                return null;
            }
            return bmpOut;
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")] public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// bitmap to windwos.media.stream
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private BitmapSource BitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr ptr = bitmap.GetHbitmap();
            BitmapSource result =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //release resource
            DeleteObject(ptr);

            return result;
        }

        /// <summary>
        /// 顯示座標
        /// </summary>
        /// <param name="x">滑鼠座標傳入</param>
        public void MPLBcontain(string x)
        {
            if(this.WindowState==WindowState.Maximized)
            {
                showlbl.Content = (Convert.ToInt32(x.Substring(0, x.IndexOf(",")))+Convert.ToInt32(sv.ContentHorizontalOffset)).ToString() + "," +
               (Convert.ToInt32(x.Substring(x.IndexOf(",") + 1, x.Length - x.IndexOf(",") - 1)) -(imgdir.Height * 2)+Convert.ToInt32(sv.ContentVerticalOffset)).ToString();
            }
            else
            {
                showlbl.Content = (Convert.ToInt32(x.Substring(0, x.IndexOf(","))) - Left-4 + Convert.ToInt32(sv.ContentHorizontalOffset)).ToString() + "," 
                +(Convert.ToInt32(x.Substring(x.IndexOf(",") + 1, x.Length - x.IndexOf(",") - 1)) - Top - (imgdir.Height * 2+10)+Convert.ToInt32(sv.ContentVerticalOffset)).ToString()
               ;
            }
           
        }
        public void CatchMousePosition()
        {
            while (!Formclose)
            {
                Dispatcher.Invoke(Mlbc, new string[] { GetMousePosition().X.ToString() + "," + GetMousePosition().Y.ToString() });

                Thread.Sleep(100);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           Formclose = true;
        }

        #region キーボードイベント処理

        public void KeyboardWatching()
        {
            //0 設置鍵盤掛勾 
            if (m_HookHandle == 0)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    m_KbdHookProc = new HookProc(KeyboardHookProc);
                    m_HookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, m_KbdHookProc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
            else
            {
                bool ret = UnhookWindowsHookEx(m_HookHandle);
                if (ret == false)
                {
                    /*呼叫失敗*/
                }
                m_HookHandle = 0;

            }
        }
        public int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {

            
            try
            {
                //if (Keyboard.IsKeyUp(autoFkey))
                //{
                //    autoF = false;
                //}
                // 當按鍵按下及鬆開時都會觸發此函式好幾次，這裡只處理鍵盤按下的情形。
                bool isPressed = (lParam.ToInt64() & 0x800000000000) == 0;
                bool check = false;
                //// 取得欲攔截之按鍵狀態
                KeyStateInfo escKey = KeyboardInfo.GetKeyState(Keys.Escape);
                KeyStateInfo f10Key = KeyboardInfo.GetKeyState(Keys.F10);
                KeyStateInfo SpaceKey = KeyboardInfo.GetKeyState(Keys.Space);
                if (nCode < 0 || !isPressed)
                {

                    return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);

                }
                  if (!Keyboard.IsKeyUp(Key.Delete))
                {
                    removePointList();
                }
                //抓到F10被按了之後 開始設定矩陣
                if (!Keyboard.IsKeyUp(Key.F10))
                {
                    sv.Focus();
                    addPoint();
                }
                if (!Keyboard.IsKeyUp(Key.Escape))
                {
                    clearP1P2();
                   
                }
                 if (!Keyboard.IsKeyUp(Key.Space))
                {
                    sv.Focus();
                    if (spaceispressed % 2 == 0)
                    {
                        P1.Content = "P1:" + showlbl.Content;
                    }
                    else
                    {
                        P2.Content = "P2:" + showlbl.Content;
                    }
                    spaceispressed++;
                }
                 if(!Keyboard.IsKeyUp(Key.NumPad0))
                {
                    Tag.SelectedIndex = 0;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad1))
                {
                    Tag.SelectedIndex = 1;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad2))
                {
                    Tag.SelectedIndex = 2;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad3))
                {
                    Tag.SelectedIndex = 3;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad4))
                {
                    Tag.SelectedIndex = 4;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad5))
                {
                    Tag.SelectedIndex = 5;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad6))
                {
                    Tag.SelectedIndex = 6;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad7))
                {
                    Tag.SelectedIndex = 7;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad8))
                {
                    Tag.SelectedIndex = 8;
                }
                if (!Keyboard.IsKeyUp(Key.NumPad9))
                {
                    Tag.SelectedIndex = 9;
                }
                //if (!Keyboard.IsKeyDown(Key.Space) && !autoF && AutotypeCB.SelectedIndex == 1)
                //{
                //    autoFkey = Key.Space;
                //    autoF = true;
                //    catchposistion();
                //}
                //if (Keyboard.IsKeyDown(Key.Escape))
                //{
                //    if (autowebsideNew != null)
                //    {
                //        autowebsideNew.evtstop = true;
                //        cvt.LockMouse(false);
                //    }
                //}
            }
            catch (Exception ex) { }
            return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
        }
        private void clearP1P2_Click(object sender, RoutedEventArgs e)
        {
            clearP1P2();
        }
        private void clearP1P2()
        {
            P1.Content = "P1:";
            P2.Content = "P2:";
            spaceispressed = 0;
        }
        private void addPoint()
        {
            bool check = false;
            try
            {
                if (P1.Content.ToString() != "P1:" && P2.Content.ToString() != "P2:" && imglist.SelectedItem as ListBoxItem != null)
                {
                    for (index = 0; index < PointList.Items.Count; index++)
                    {
                        if (PointList.Items[index].ToString().IndexOf((imglist.SelectedItem as ListBoxItem).Content.ToString()) >= 0)
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        PointList.Items[index] = PointList.Items[index] + " " + P1.Content.ToString().Replace("P1:", "") + "," +
                            P2.Content.ToString().Replace("P2:", "") + "," + Tag.SelectedValue.ToString();
                    }
                    else
                    {
                        PointList.Items.Add((imglist.SelectedItem as ListBoxItem).Content.ToString() + " " + P1.Content.ToString().Replace("P1:", "") +
                            "," + P2.Content.ToString().Replace("P2:", "") + "," + Tag.SelectedValue.ToString());
                    }
                    clearP1P2();
                }
            }
            catch (Exception ex) { }
        }

        private void addList_Click(object sender, RoutedEventArgs e)
        {
            addPoint();
        }

        private void removePointList()
        {
            try
            {
                if (PointList.IsKeyboardFocusWithin && PointList.Items.Count > 0 && PointList.SelectedIndex > 0)
                {
                    PointList.Items.RemoveAt(PointList.SelectedIndex);
                }
            }
            catch (Exception ex) { }
        }

        private void RemovePointListitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(PointList.Items.Count>0 && PointList.SelectedIndex >0)
                {
                    PointList.Items.RemoveAt(PointList.SelectedIndex);
                }
               
            }
            catch (Exception ex) { }
            
        }
        #endregion
        #region  KeyStateInfo
        public enum MouseEventTFlags
        {
            //左鍵
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            //中鍵
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            //右鍵
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,


            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,              //滾輪移動
            VirtualDesk = 0x4000,        //虛擬桌面
            Absolute = 0x8000
        }
        [TypeConverter(typeof(ByteConverter))]
        public enum keyboard_code : byte
        {
            Enter = 0x0D,
            Shift = 0x10,
            LeftShif = 0xA0,
            RightShif = 0xA1,
            Ctrl = 0x11,
            LeftCtrl = 0xA2,
            RighCtrl = 0xA3,
            Alt = 0x12,
            Pause = 0x13,
            IME_Kana_mode = 0x15,
            IME_Kanji_mode = 0x19,
            Esc = 0x1B,
            /// <summary>
            /// 空白鍵、スペースキー
            /// </summary>
            SPACEBAR = 0x20,
            PageUp = 0x21,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            LeftArrow = 0x25,
            UpArrow = 0x26,
            RightArrow = 0x27,
            DownArrow = 0x28,
            PrintScreen = 0x2C,
            Del = 0x2E,
            /// <summary>
            /// 鍵盤左上方數字0
            /// </summary>
            knum0 = 0x30,
            knum1 = 0x31,
            knum2 = 0x32,
            knum3 = 0x33,
            knum4 = 0x34,
            knum5 = 0x35,
            knum6 = 0x36,
            knum7 = 0x37,
            knum8 = 0x38,
            knum9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            /// <summary>
            /// 數字鍵0
            /// </summary>
            Numeric_keypad_0 = 0x60,
            Numeric_keypad_1 = 0x61,
            Numeric_keypad_2 = 0x62,
            Numeric_keypad_3 = 0x63,
            Numeric_keypad_4 = 0x64,
            Numeric_keypad_5 = 0x65,
            Numeric_keypad_6 = 0x66,
            Numeric_keypad_7 = 0x67,
            Numeric_keypad_8 = 0x68,
            Numeric_keypad_9 = 0x69,
            /// <summary>
            /// 鍵盤上的*
            /// </summary>
            Multiply = 0x6A,
            /// <summary>
            /// 鍵盤上的+
            /// </summary>
            Add = 0x6B,
            /// <summary>
            /// 
            /// </summary>
            Separator = 0x6C,
            /// <summary>
            /// 鍵盤上的-
            /// </summary>
            Subtract = 0x6D,
            /// <summary>
            /// 鍵盤上的.
            /// </summary>
            Decimal = 0x6E,
            /// <summary>
            /// 鍵盤上的/
            /// </summary>
            Divide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
        }
        public class KeyboardInfo
        {
            private KeyboardInfo() { }
            [DllImport("user32")]
            private static extern short GetKeyState(int vKey);
            public static KeyStateInfo GetKeyState(Keys key)
            {
                short keyState = GetKeyState((int)key);
                byte[] bits = BitConverter.GetBytes(keyState);
                bool toggled = bits[0] > 0, pressed = bits[1] > 0;
                return new KeyStateInfo(key, pressed, toggled);
            }
        }

        #region dataclass
        public class RunThreadDate
        {
            public string looptimesText, AutotypeCBText;
            public int x, y, DelayTime;
        }
        #endregion
        /******************************************************/
        /*          NULLFX FREE SOFTWARE LICENSE              */
        /******************************************************/
        /*  GetKeyState Utility                               */
        /*  by: Steve Whitley                                 */
        /*  © 2005 NullFX Software                            */
        /*                                                    */
        /* NULLFX SOFTWARE DISCLAIMS ALL WARRANTIES,          */
        /* RESPONSIBILITIES, AND LIABILITIES ASSOCIATED WITH  */
        /* USE OF THIS CODE IN ANY WAY, SHAPE, OR FORM        */
        /* REGARDLESS HOW IMPLICIT, EXPLICIT, OR OBSCURE IT   */
        /* IS. IF THERE IS ANYTHING QUESTIONABLE WITH REGARDS */
        /* TO THIS SOFTWARE BREAKING AND YOU GAIN A LOSS OF   */
        /* ANY NATURE, WE ARE NOT THE RESPONSIBLE PARTY. USE  */
        /* OF THIS SOFTWARE CREATES ACCEPTANCE OF THESE TERMS */
        /*                                                    */
        /* USE OF THIS CODE MUST RETAIN ALL COPYRIGHT NOTICES */
        /* AND LICENSES (MEANING THIS TEXT).                  */
        /*                                                    */
        /******************************************************/

        public struct KeyStateInfo
        {
            Keys _key;
            bool _isPressed,
                _isToggled;
            public KeyStateInfo(Keys key,
                            bool ispressed,
                            bool istoggled)
            {
                _key = key;
                _isPressed = ispressed;
                _isToggled = istoggled;
            }
            public static KeyStateInfo Default
            {
                get
                {
                    return new KeyStateInfo(Keys.None,
                                                false,
                                                false);
                }
            }
            public Keys Key
            {
                get { return _key; }
            }
            public bool IsPressed
            {
                get { return _isPressed; }
            }
            public bool IsToggled
            {
                get { return _isToggled; }
            }
        }

        #endregion

        private void outputdata_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savepath = new SaveFileDialog();
            savepath.DefaultExt = ".txt";
            savepath.Filter = "Text documents (.txt)|*.txt";
            
            if (savepath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Open document
                string filename = savepath.FileName;
                fileio fio = new fileio();
                List<string> strings = new List<string>();
                foreach (var a in PointList.Items)
                {
                    strings.Add(a.ToString());
                }
                fio.fileDataCreatUTF8(strings,savepath.FileName);
            }
        }

        private void PointList_GotFocus(object sender, RoutedEventArgs e)
        {

        }

    }
    class imgsubName
    {
        public List<string> subName = new List<string>() { ".JPG",".jpg",".jpeg", ".jpe",".jfif", ".jfi", ".jif",
            ".JPEG",".gif",".png", ".bmp", };
        
    }

}
