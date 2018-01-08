using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace KeyExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if we can find ViewActions in C:\
            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No input file given. ");
                Console.ResetColor();
                Console.ReadLine();
                
               
                
            }
            else
            {
                Console.WriteLine(args[0]);
                // XML file path
                ParseXML(args[0]);
                Console.ReadLine();
            }
        }



        static void ParseXML(string fileName)
        {
            // Totalkeys is 0
            int totalKeys = 0;

            //Delimiter for splitting errors
            char[] delim = { '\n' };


            XmlDocument xmlDocument = new XmlDocument();
            // Store results
            StringBuilder result = new StringBuilder("");
            // Store errors
            StringBuilder errors = new StringBuilder("");

            // Load the XML file
            xmlDocument.Load(fileName);

            // Get XML nodes
            XmlNodeList actions = xmlDocument.DocumentElement.SelectNodes("//textInput");
            // Totalkeys = count how many keys found 
            totalKeys += FindKeys(actions, result, errors);
            actions = xmlDocument.DocumentElement.SelectNodes("//textInputService");
            totalKeys += FindKeys(actions, result, errors);
            actions = xmlDocument.DocumentElement.SelectNodes("//values");
            totalKeys += FindKeys(actions, result, errors);
            actions = xmlDocument.DocumentElement.SelectNodes("//domainObjectList");
            totalKeys += FindKeys(actions, result, errors);
            actions = xmlDocument.DocumentElement.SelectNodes("//selectService");
            totalKeys += FindKeys(actions, result, errors);
            actions = xmlDocument.DocumentElement.SelectNodes("//type");
            totalKeys += FindKeys(actions, result, errors);
            // Display how many keys that was found
            Console.WriteLine("Keys found: " + totalKeys + "\n");
            System.Threading.Thread.Sleep(2000);

            // Create and write to the file
            System.IO.File.WriteAllText(@"C:\ci_web.properties", result.ToString());

            // Check if there is any errors
            if (errors.Length > 0)
            {
                // Create error log
                System.IO.File.WriteAllText(@"c:\ci_web_errorlog.txt", "\n ");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\ci_web_errorlog.txt", true))
                {
                    file.WriteLine(" ================================");
                    file.WriteLine(" // XML Key Extractor Error Log  \\\\");
                    file.WriteLine("//          XML Nodes:            \\\\");
                    file.WriteLine("====================================");
                    file.WriteLine("======= ERROR: Missing key  ========");
                    file.WriteLine("===== ci_web.properties info: ======");
                    file.WriteLine("|          Keys found: " + totalKeys.ToString() + "         |");
                    file.WriteLine("====================================");
                    //  file.WriteLine(errors.ToString());
                    foreach (string line in errors.ToString().Split(delim))
                        file.WriteLine(line);

                    Console.Write("An error has occoured, check the log file\n\n");
                }
                //Finishing messages
                Console.Write("\nci_web.properties has been created with " + totalKeys + " keys\n");
                Console.Write("XML Key Extractor by Tobias\n");
                Console.Write("Press Enter button to exit...");
            }
        }


        static public int FindKeys(XmlNodeList actions, StringBuilder result, StringBuilder errors)
        {
            // Attriute name
            string[] atrs = { "label", "groupId", "hintText", "prompt", "help", "description" };

            int Count = 0;
            foreach (XmlNode action in actions)
            {
                // Searches for attributes
                foreach (string atrName in atrs)
                {

                    XmlAttribute atr = action.Attributes[atrName];
                    if (atr != null)
                    {
                        XmlAttribute key = action.Attributes[atrName + "Key"];
                        // Check if Key is missing and display error message
                        if (key == null)
                        {
                            errors.AppendLine("ERROR: A key is missing");
                            // XML formating for error log
                            StringWriter wr = new StringWriter();
                            XmlTextWriter writer = new XmlTextWriter(wr);
                            writer.Formatting = Formatting.Indented;
                            action.WriteTo(writer);
                            errors.AppendLine("\n" + "\n" + "\n" + wr.ToString());
                        }
                        else
                        {
                            // Key = Value format
                            result.Append(key.Value + "=" + atr.Value + "\n");
                            Count++;

                        }
                    }

                }
            }
            return Count;
        }

        // Get the value of the attributes
        static string GetValue(XmlAttribute atr)
        {

            if (atr != null && atr.Value != null)
                return atr.Value;
            return "";
        }
    }
}