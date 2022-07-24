using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
//origin ver at 20170703
//ver 1.2 at 20220724
namespace mainWin
{
    class fileio
    {
        public string  error,FileReadToEndInfo="";
        string [] answer=new string[]{""};
        int processNum = 0;
        /*-----------------------------------------------------------read-----------------------------------------------------------*/
        /// <summary>
        /// 用字元(UTF-8)的方式讀取
        /// </summary>
        /// <param name="pathstr">路徑</param>
        /// <returns>回傳字串陣列</returns>
        public string[] fileDataReadCharUTF8(string pathstr)
        {

            string[] re = new string[] { "" };
            int count = 0;
            char []x=new char[]{'\0'} ;
            bool add = false;
            using (StreamReader SrList = new StreamReader(pathstr, Encoding.UTF8))
            {
                while (SrList.EndOfStream != true)
                {
                    SrList.Read(x,0,1);
                    if (x[0] == '\r')
                    {
                        System.Array.Resize(ref re, re.Length + 1);
                        count++;
                        add = true;
                    }
                    else if (x[0] == '\n')
                    { }
                    else
                        re[count] += x[0];
                }
                if(add && re[re.Length-1]==null)
                System.Array.Resize(ref re, re.Length - 1);
                SrList.Close();
            }
            return re;
        }
        public string[] fileDateReadUTF8(string pathstr)
        {
            string [] re=new string[]{""};
            int count = 0;
            using (StreamReader SrList = new StreamReader(pathstr, Encoding.UTF8))
            {
                while (SrList.EndOfStream != true)
                {
                    re[count] = SrList.ReadLine();
                    System.Array.Resize(ref re, re.Length + 1);
                    count++;
                }
                System.Array.Resize(ref re, re.Length - 1);
                SrList.Close();
            }
            return re;
        }

        public string fileDateReadAppointIndexUTF8(string pathstr,int IndexStart)
        {
            string temp = "",re="";
            int count = 0;
            using (StreamReader SrList = new StreamReader(pathstr, Encoding.UTF8))
            {
                while (SrList.EndOfStream != true)
                {
                    if (count < IndexStart)
                    {
                        temp = SrList.ReadLine();
                    }
                    else
                    {
                        re += SrList.ReadLine();
                    }
                    count++;
                }
                SrList.Close();
            }
            return re;
        }

        public string fileDateReadToEndUTF8(string pathstr)
        {
            string re = "" ;
            using (StreamReader SrList = new StreamReader(pathstr, Encoding.UTF8))
            {
                re = SrList.ReadToEnd();
                SrList.Close();
            }
            return re;
        }

        public string fileDateReadToEndHtml(string pathstr)
        {
            string re = "";
            using (StreamReader SrList = new StreamReader(pathstr, Encoding.UTF8))
            {
                
                while(SrList.EndOfStream!=true)
                {
                    re += SrList.ReadLine()+"<br/>";
                }
                re.Replace("<br/><br/>", "<br/>");
                SrList.Close();
            }
            return re;
        }
        /*-----------------------------Create------------------------------------------------------*/
        /// <summary>
        /// 不會有換行輸出 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pathstr"></param>
        /// <param name="FileName"></param>
        public void fileDataCreatNoNewLineUTF8(string text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (StreamWriter fs = File.CreateText(strSub))
            {
                fs.Write(text);
                fs.Close();
            }
        }
        public void fileDataCreatNoNewLineUTF8(string[] text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (StreamWriter fs = File.CreateText(strSub))
            {
                int i = 0;
                while (i < text.Length)
                {
                    fs.Write(text[i]);
                    i++;
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text">內容</param>
        /// <param name="pathstr">資料夾位址</param>
        /// <param name="FileName">檔案名稱(包括副檔名)</param>
        public void fileDataCreatUTF8(string text,string pathstr,string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (StreamWriter fs = File.CreateText(strSub))
            {
                fs.WriteLine(text);
                fs.Close();
            }
        }
        /// <summary>
        /// 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text">內容</param>
        /// <param name="pathstr">資料夾位址+檔案名稱(包括副檔名)</param>
        public void fileDataCreatUTF8(string text, string pathstr)
        {
            using (StreamWriter fs = File.CreateText(pathstr))
            {
                fs.WriteLine(text);
                fs.Close();
            }
        }
        /// <summary>
        /// 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text">內容(字串陣列格式 一列一行)</param>
        /// <param name="pathstr">資料夾位址</param>
        /// <param name="FileName">檔案名稱(包括副檔名)</param>
        public void fileDataCreatUTF8(string [] text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (StreamWriter fs = File.CreateText(strSub))
            {
                int i = 0;
                while (i < text.Length)
                {
                    fs.WriteLine(text[i]);
                    i++;
                }
                fs.Close();
            }
        }
        /// <summary>
        /// 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text">內容(字串陣列格式 一列一行)</param>
        /// <param name="pathstr">資料夾位址</param>
        /// <param name="FileName">檔案名稱(包括副檔名)</param>
        public void fileDataCreatUTF8(List<string> text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (StreamWriter fs = File.CreateText(strSub))
            {
                foreach(var i in text)
                {
                    fs.WriteLine(i);
                }
                fs.Close();
            }
        }
        /// <summary>
        /// 建立檔案(UTF-8編碼)
        /// </summary>
        /// <param name="text">內容(List<string>格式 一個string一行 但裡面有/r/n就會換行)</param>
        /// <param name="pathstr_FileName">資料夾位址+檔案名稱(包括副檔名)</param>
        public void fileDataCreatUTF8(List<string> text, string pathstr_FileName)
        {
            using (StreamWriter fs = File.CreateText(pathstr_FileName))
            {
                foreach (var i in text)
                {
                    fs.WriteLine(i);
                }
                fs.Close();
            }
        }
        /// <summary>
        /// 建立檔案(ANSI編碼 依系統目前是什麼語系編碼就是什麼語系 跨語系時會變成亂碼)
        /// </summary>
        /// <param name="text">內容</param>
        /// <param name="pathstr">資料夾位址</param>
        /// <param name="FileName">檔案名稱(包括副檔名)</param>
        public void fileDataCreatANSI(string text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (System.IO.FileStream fs = File.Create(strSub))
            {
                Byte[] outstr = Encoding.Default.GetBytes(text);
                fs.Write(outstr, 0, outstr.Length);
                fs.Close();
            }
        }
        public void fileDataCreatANSI(string[] text, string pathstr, string FileName)
        {
            string strSub = System.IO.Path.Combine(pathstr, FileName);
            using (System.IO.FileStream fs = File.Create(strSub))
            {
                Byte[] outstr;
                int i = 0;
                while (i < text.Length)
                {
                    outstr = Encoding.Default.GetBytes(text[i]);
                    fs.Write(outstr,0,outstr.Length);
                    i++;
                }
                fs.Close();
            }
        }

        /*-----------------------------run Exe------------------------------------------------------*/
        /// <summary>
        /// 執行外部程式檔案
        /// </summary>
        /// <param name="path">位址</param>
        /// <param name="input">是否要輸入</param>
        /// <param name="inputstr">輸入資料 非陣列 </param>
        /// <param name="EOFstring">確認是否有EOF</param>
        /// <returns></returns>
        public string[] runFile(string path,bool input,string inputstr,bool EOFstring) //inpustr 非陣列 確認是否有EOF
        { //用char讀取 轉成string輸出
            try
            {
                char[] s = new char []{ '\0' };
                Process runSoft = new Process();
                if (answer.Length == 0)
                    System.Array.Resize(ref answer, answer.Length + 1);
                else
                {
                    System.Array.Resize(ref answer, 1);
                    answer[0] = "";
                }
                runSoft.StartInfo.FileName = path;
                runSoft.StartInfo.CreateNoWindow = true;
                runSoft.StartInfo.RedirectStandardError = true;
                runSoft.StartInfo.UseShellExecute = false;
                runSoft.StartInfo.RedirectStandardInput = true;
                runSoft.StartInfo.RedirectStandardOutput = true;
                runSoft.Start();
                if (input == true)
                {
                    runSoft.StandardInput.WriteLine(inputstr);
                    if (EOFstring == true)
                        runSoft.StandardInput.WriteLine((char)26);
                }
                while (runSoft.StandardOutput.EndOfStream != true)
                {
                    runSoft.StandardOutput.Read(s, 0, 1);
                    answer[answer.Length - 1] += s[0];
                    if (s[0] == '\n')
                    {
                        if (answer[answer.Length - 1].Equals("続行するには何かキーを押してください . . . \r\n") ||
                            answer[answer.Length - 1].Equals("請案任意鍵繼續 . . . \r\n")|| 
                            answer[answer.Length - 1].Equals("Press any key to continue . . . \r\n"))
                        {
                            answer[answer.Length - 1] = "";
                            runSoft.StandardInput.WriteLine("A");
                        }
                        else
                            System.Array.Resize(ref answer, answer.Length + 1);
                    }
                }
                if (s[0] == '\n')
                    System.Array.Resize(ref answer, answer.Length - 1);
                error = runSoft.StandardError.ReadToEnd();
                runSoft.Close();
                
            }catch(Exception ex){}
            return answer;
        }

        public string[] runFile(string path, bool input, string inputstr) //inpustr 非陣列
        { //用char讀取 轉成string輸出
            try
            {
                char[] s = new char[] { '\0' };
                Process runSoft = new Process();
                if (answer.Length == 0)
                    System.Array.Resize(ref answer, answer.Length + 1);
                runSoft.StartInfo.FileName = path;
                runSoft.StartInfo.CreateNoWindow = true;
                runSoft.StartInfo.RedirectStandardError = true;
                runSoft.StartInfo.UseShellExecute = false;
                runSoft.StartInfo.RedirectStandardInput = true;
                runSoft.StartInfo.RedirectStandardOutput = true;
                runSoft.Start();
                if (input == true)
                    runSoft.StandardInput.WriteLine(inputstr);
                while (runSoft.StandardOutput.EndOfStream != true)
                {
                    runSoft.StandardOutput.Read(s, 0, 1);
                    answer[answer.Length - 1] += s[0];
                    if (s[0] == '\n')
                    {
                        if (answer[answer.Length - 1].Equals("続行するには何かキーを押してください . . . \r\n") ||
                            answer[answer.Length - 1].Equals("請案任意鍵繼續 . . . \r\n") ||
                            answer[answer.Length - 1].Equals("Press any key to continue . . . \r\n"))
                        {
                            answer[answer.Length - 1] = "";
                            runSoft.StandardInput.WriteLine("A");
                        }
                        else
                            System.Array.Resize(ref answer, answer.Length + 1);
                    }
                }
                if (s[0] == '\n')
                    System.Array.Resize(ref answer, answer.Length - 1);
                error = runSoft.StandardError.ReadToEnd();
                runSoft.Close();

            }
            catch (Exception ex) { }
            return answer;
        }

        public string[] runFile(string path, string [] inputstr) //inpustr 陣列
        {
          char [] sn=new char []{'\0'};
          int count = 0;
            Process runSoft = new Process();
            if (answer.Length == 0)
                System.Array.Resize(ref answer, answer.Length + 1);
            runSoft.StartInfo.FileName = path;
            runSoft.StartInfo.CreateNoWindow = true;
            runSoft.StartInfo.RedirectStandardError = true;
            runSoft.StartInfo.UseShellExecute = false;
            runSoft.StartInfo.RedirectStandardInput = true;
            runSoft.StartInfo.RedirectStandardOutput = true;
            runSoft.Start();
            count=0;
            while(count<inputstr.Length)
            {
                runSoft.StandardInput.WriteLine(inputstr[count]);
                count++;
            }
            count=0;
            while (runSoft.StandardOutput.EndOfStream != true)
            {
                answer[count] = runSoft.StandardOutput.ReadLine();
                System.Array.Resize(ref answer, answer.Length + 1);
                count++;
            }
            System.Array.Resize(ref answer, answer.Length - 1);
            error = runSoft.StandardError.ReadToEnd();
            runSoft.Close();
            return answer;
        }
        public void runFileReadToEnd(string path, string[] inputstr)
        {
          try
            {
                int count = 0;
                Process runSoft = new Process();
                
                if (answer.Length == 0)
                    System.Array.Resize(ref answer, answer.Length + 1);
                runSoft.StartInfo.FileName = path;
                runSoft.StartInfo.CreateNoWindow = true;
                runSoft.StartInfo.RedirectStandardError = true;
                runSoft.StartInfo.UseShellExecute = false;
                runSoft.StartInfo.RedirectStandardInput = true;
                runSoft.StartInfo.RedirectStandardOutput = true;
                runSoft.Start();

                processNum = runSoft.Id;
                count = 0;
                while (count < inputstr.Length)
                {
                    runSoft.StandardInput.WriteLine(inputstr[count]);
                    count++;
                }
                Thread watchgcc=new Thread(new ThreadStart(this.watchExe));
                watchgcc.Start();

                FileReadToEndInfo = runSoft.StandardOutput.ReadToEnd();
                System.Array.Resize(ref answer, answer.Length - 1);
                error = runSoft.StandardError.ReadToEnd();
                runSoft.Close();
            }
            catch (Exception e) { error = e.ToString(); }
        }
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        public void watchExe()
        {
            try
            {
                int i = 0, j = 0, countTime = 1;
                bool ExeTure = false;
                Process[] processs = Process.GetProcesses();


                Process runSoft = Process.GetProcessById(processNum);
                for (i = 0; i <= countTime; i++)
                {
                    for (j = 0; j < processs.Length; j++)
                    {
                        if (processs[j].Id == processNum)
                        {

                            ExeTure = true;
                            break;
                        }
                        else if (j == processs.Length - 1)
                        {
                            ExeTure = false;
                        }
                    }
                    if (countTime == i && ExeTure)
                    {
                        runSoft.CloseMainWindow();
                    }
                    else
                        Thread.Sleep(1000);
                }
            }
            catch (Exception e) { }

        }
    }
    //public string runFileUseCharRead()


   
}
