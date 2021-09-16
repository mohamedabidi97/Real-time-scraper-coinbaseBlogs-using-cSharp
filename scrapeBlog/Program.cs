using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.IO;
using System.Linq;

namespace scrapeBlog

{
    internal static class Program
    {
        // Function : Add the new titles to the first line in text file 
        private static void WriteToFile(string path, IEnumerable<string> text)
        {
            var content = File.ReadAllText(path);
            var newContent = "";
            foreach (var line in text)
            {
                newContent = line + "\n";
            }

            content = newContent + content;
            File.WriteAllText(path, content);
        }

        private static void Main(string[] args)
        {
            List<string> newList = new List<string>();
            var enviroment = System.Environment.CurrentDirectory;
            var directoryInfo = Directory.GetParent(enviroment)?.Parent;
            var path = Path.Combine(directoryInfo.FullName, "list.txt");
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
            var proxy = new Proxy();
            proxy.HttpProxy = "us.proxiware.com:2000";
            options.Proxy = proxy;
            WebDriver driver = new ChromeDriver(options);
            
            for (;;)
            {
                driver.Navigate().GoToUrl("https://blog.coinbase.com/latest");
                Thread.Sleep(1000);
                var blogTitles = driver.FindElements(By.CssSelector(".graf--title"));
                if (File.Exists(path))
                {
                    foreach (var blogTitle in blogTitles)
                    {
                        try
                        {
                            newList.Add(blogTitle.GetAttribute("innerHTML"));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }

                    var newArray = newList.ToArray();
                    if (new FileInfo(path).Length == 0)
                    {
                        try
                        {
                            using (var sw = File.AppendText(path))
                            {
                                foreach (var title in newArray)
                                {
                                    sw.WriteLine(title);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    else
                    {
                        var initialArray = File.ReadLines(path).ToArray();
                        IEnumerable<string> newTitles = newArray.Except(initialArray);
                        WriteToFile(path, newTitles);
                    }
                }
                else
                {
                    Console.WriteLine("Error : There is No file, Please create list.txt");
                }
            }
        }
    }
}