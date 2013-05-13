using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Dynamic;

namespace EmailerTemplate
{
    public class Emailer
    {
        private string emailPath;
        public dynamic data = new ExpandoObject();
        private string rawEmail;
        public string RawEmail { get { return rawEmail; } }

        public static string VarOpenBracket = "[[";
        public static string VarCloseBracket = "]]";
        public static string BlockOpenBracket = "[{";
        public static string BlockCloseBracket = "}]";
        public static string ConditionalOpenBracket = "[%";
        public static string ConditionalCloseBracket = "%]";


        public Emailer(string emailPath)
        {
            this.emailPath = emailPath;
            getEmailFile();
        }

        private void getEmailFile()
        {
            using (StreamReader sReader = File.OpenText(emailPath))
            {
                rawEmail = sReader.ReadToEnd();
            }
        }

        public string render()
        {
            return renderBlock(rawEmail, data);
        }

        #region rendering

        private string renderBlock(string str, ExpandoObject dataObj)
        {
            // four variable types
            var expandoList = new List<KeyValuePair<string, object>>();
            var expandoListList = new List<KeyValuePair<string, object>>();
            var conditionalList = new List<KeyValuePair<string, object>>();
            var otherList = new List<KeyValuePair<string, object>>();

            // split the data into template types
            foreach (var obj in dataObj)
            {
                if (obj.Value is ExpandoObject)
                    expandoList.Add(obj);
                else if (obj.Value is List<ExpandoObject>)
                    expandoListList.Add(obj);
                else if (obj.Value is bool)
                    conditionalList.Add(obj);
                else
                    otherList.Add(obj);
            }

            // take a block and map it to each item in the expando list
            foreach (var list in expandoListList)
            {
                string block = getCompleteBlock(str, str.IndexOf(BlockOpenBracket + list.Key),
                                                BlockOpenBracket, BlockCloseBracket);
                if (block != String.Empty)
                {
                    string trimBlock = trim(block, list.Key, BlockOpenBracket, BlockCloseBracket);
                    str = str.Replace(block, renderList(trimBlock, list.Value as List<ExpandoObject>));
                }
            }

            // take a block and apply it to a single expando
            foreach (var expando in expandoList)
            {
                // find the pattern that matches this object name
                string block = getCompleteBlock(str, str.IndexOf(BlockOpenBracket + expando.Key),
                                                BlockOpenBracket, BlockCloseBracket);

                // call render using the pattern and obj
                if (block != String.Empty)
                {
                    string trimBlock = trim(block, expando.Key, BlockOpenBracket, BlockCloseBracket);
                    str = str.Replace(block, renderBlock(trimBlock, (expando.Value as ExpandoObject)));
                }
            }

            // show or hide a block given the value of the boolean
            foreach (var condition in conditionalList)
            {
                // find the pattern that matches this object name
                string block = getCompleteBlock(str, str.IndexOf(ConditionalOpenBracket + condition.Key),
                                                ConditionalOpenBracket, ConditionalCloseBracket);
                if (block != String.Empty)
                {
                    if (!Convert.ToBoolean(condition.Value))
                        str = str.Replace(block, "");
                    else
                        str = str.Replace(block, trim(block, condition.Key, ConditionalOpenBracket, ConditionalCloseBracket));
                }                
            }

            // replace template variable with the matching dynamic variable
            foreach (var val in otherList)
            {
                // replace [[name]] with data value
                string pattern = "[[" + val.Key + "]]";

                // return the string
                str = str.Replace(pattern, val.Value.ToString());
            }

            return str;
        }

        private string renderList(string str, List<ExpandoObject> dataObj)
        {
            string completeBlock = "";

            foreach (ExpandoObject obj in dataObj)
                completeBlock += renderBlock(str, obj);

            return completeBlock;
        }

        private string getCompleteBlock(string str, int openIndex, string openPattern, string closePattern)
        {
            if (openIndex != -1)
            {
                int closeIndex = str.IndexOf(closePattern, openIndex);
                int currentIndex = str.IndexOf(openPattern, openIndex + openPattern.Length, closeIndex - openIndex);

                while (currentIndex != -1)
                {
                    int oldCloseIndex = closeIndex;
                    closeIndex = str.IndexOf(closePattern, closeIndex + closePattern.Length);
                    currentIndex = str.IndexOf(openPattern, oldCloseIndex, closeIndex - oldCloseIndex);
                }

                return str.Substring(openIndex, closeIndex - openIndex + closePattern.Length);
            }
            return "";
        }

        private string trim(string str, string name, string openPattern, string closePattern)
        {
            str = str.Substring(openPattern.Length + name.Length);
            str = str.Substring(0, str.Length - closePattern.Length);
            return str;
        }

        #endregion
    }
}
