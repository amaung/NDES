using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;
//using System.Web.Services;
//using System.Web.UI.WebControls;
using System.Threading;
using System.Threading.Tasks;

namespace JSON
{
    public class Json
    {
        private string jFileName;
        private static Mutex mux = new Mutex();
        //**************************************************************************************
        //* Json() - Constructor.
        //**************************************************************************************
        public Json(string Jfilename)
        {
            jFileName = Jfilename;
            mux.WaitOne();
            string s1 = "Json file check";
            string s2 = string.Empty;
            string s3 = string.Empty;
            if (!File.Exists(jFileName))
            {
                s2 = "Going to create";
                createFile(jFileName);
                s3 = "Created";
            }
            string fname = "C:\\Users\\auzma\\source\\VS2012\\PaliScriptConverterWeb-V1Alpha\\PaliScriptConverterWeb\\log.txt";
            string[] s = { s1, s2, jFileName, s3, "Completed" };
            System.IO.File.AppendAllLines(fname, s);
            mux.ReleaseMutex();
        }
        private void createFile(string fn)
        {
            string Date = "  \"DATE\": \"";
            string d = DateTime.Today.ToString();
            int p = d.IndexOf(' ');
            if (p != -1) d = d.Substring(0, p);
            Date += d;
            Date += "\"";
            string[] lines = { "{", Date, "}" };
            // WriteAllLines creates a file
            // writes all lines to the file,
            try
            {
                System.IO.File.WriteAllLines(fn, lines);
            }
            catch (IOException e)
            {
                if (e.Source != null)
                    Console.WriteLine("IOException source: {0}", e.Source);
                throw;
            }
        }
        //**************************************************************************************
        //* GetData() - To retrieve key, value pairs from json file and format into strings
        //              as comma separated values.
        //**************************************************************************************
        public string[] GetData()
        {
            List<string> s = new List<string>();
            mux.WaitOne();
            var json = File.ReadAllText(jFileName);
            mux.ReleaseMutex();
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    foreach (var j in jObject)
                    {
                        string name = j.Key;
                        int value = j.Value.Value<int>();
                        s.Add(name + "," + value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
            return s.ToArray();
        }
        //**************************************************************************************
        //* GetData() - To retrieve key, value pairs from json file and format into strings
        //              as comma separated values.
        //**************************************************************************************
        public string GetReport()
        {
            mux.WaitOne();
            var json = File.ReadAllText(jFileName);
            mux.ReleaseMutex();
            string s = string.Empty;
            try
            {
                var jObject = JObject.Parse(json);
                if (jObject != null)
                {
                    foreach (var j in jObject)
                    {
                        string name = j.Key;
                        string value;
                        if (name == "DATE") value = j.Value.Value<string>();
                        else value = j.Value.Value<int>().ToString();
                        s += "<br>" + getLineMsg(name) + value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Add Error : " + ex.Message.ToString());
            }
            return s;
        }
        private string getLineMsg(string key)
        {
            string rc = string.Empty;
            switch (key)
            {
                case "DATE":
                    rc = "Date started = ";
                    break;
                case "VISITORS":
                    rc = "Total number of visitors = ";
                    break;
                case "CONVERSIONS":
                    rc = "Total number of conversions = ";
                    break;
                default:
                    if (key == string.Empty) rc = "Undefined";
                    else
                    {
                        if (key.IndexOf("PLATFORM") != -1 || key.IndexOf("REGION") != -1)
                            rc = key + " users = ";
                        else
                        {
                            if (key.IndexOf(".ZIP") >= 0 || key.IndexOf(".PDF") >= 0)
                                rc = key + " downloads = ";
                            else
                                rc = key + " conversions = ";
                        }
                    }
                    break;
            }
            return rc;
        }
        //**************************************************************************************
        //* UpdateData() - To update the record matching the key json file. It adds a new
        //                 record if the key doesn't exist.
        //**************************************************************************************
        public void UpdateData(string key)
        {
            try
            {
                mux.WaitOne();
                string json = File.ReadAllText(jFileName);
                mux.ReleaseMutex();
                key = key.ToUpper();
                var jObject = JObject.Parse(json);

                JToken j = jObject[key];
                if (j != null)
                {
                    int count = j.Value<int>();
                    jObject[key] = count + 1;
                }
                else
                    // add new key
                    jObject.Add(key, 1);

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jFileName, output);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Json Update Error : " + ex.Message.ToString());
            }
        }
        //**************************************************************************************
        //* UpdateData() - To update the record matching the key json file. It adds a new
        //                 record if the key doesn't exist.
        //**************************************************************************************
        public void UpdateSessionStats(string[] keys)
        {
            try
            {
                mux.WaitOne();
                string json = File.ReadAllText(jFileName);
                mux.ReleaseMutex();
                var jObject = JObject.Parse(json);

                foreach (string k in keys)
                {
                    string key = k.ToUpper();
                    JToken j = jObject[key];
                    if (j != null)
                    {
                        int count = j.Value<int>();
                        jObject[key] = count + 1;
                    }
                    else
                        // add new key
                        jObject.Add(key, 1);
                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jFileName, output);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Json Update Error : " + ex.Message.ToString());
            }
        }
    }
}