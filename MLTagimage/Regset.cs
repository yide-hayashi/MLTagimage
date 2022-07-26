using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*ver 1.0.1*/
namespace WpfApp1bot
{
    public class regset
    {
        /// <summary>
        /// 正規式引入，取出符合值 (回傳第一個符合值)
        /// </summary>
        /// <param name="href">字串</param>
        /// <param name="pax">正規式符號</param>
        /// <returns></returns>
        public string reg(string href, string pax)
        {
            Regex testreg = new Regex(pax);
            MatchCollection regsee = testreg.Matches(href);
            foreach (Match x in regsee)
            {
                href = x.Groups[0].Value;
                break;
            }
            return href;
        }
        /// <summary>
        /// 正規式引入，取出符合值(回傳單多個string)
        /// </summary>
        /// <param name="href"></param>
        /// <param name="pax"></param>
        /// <returns></returns>
        public List<string> RegSet(string href, string pax)
        {
            List<string> Setlist = new List<string>();
            Regex testreg = new Regex(pax);
            MatchCollection regsee = testreg.Matches(href);
            foreach (Match x in regsee)
            {
                if(x.Groups[0].Value!="")
                    Setlist.Add(x.Groups[0].Value);
            }
            return Setlist;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="href">原始文字</param>
        /// <param name="WasReplaceWord">取代成的字</param>
        /// <param name="pax">正規式符號</param>
        /// <returns></returns>
        public string RegReplace(string href, string WasReplaceWord, string pax)
        {
            Regex testreg = new Regex(pax);
            href = testreg.Replace(href, WasReplaceWord);
            return href;
        }


        /*
        * https://regexr.com/ 正規式測試網頁
        * 
        * replace $使用方式 https://developer.mozilla.org/ja/docs/Web/JavaScript/Reference/Global_Objects/RegExp/n(在javascript上)
        * https://docs.microsoft.com/ja-jp/dotnet/standard/base-types/substitutions-in-regular-expressions?redirectedfrom=MSDN#Anchor_0 (C#)
        */
    }

}
