using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;


namespace ConsoleApp2
{
    internal class Program
    {
        public static List<TopicClass> AllTopic = new List<TopicClass>();
        static void Main(string[] args)
        {
            int correct = 0;
            int total = 0;
            string pdfFilePath = "./DataSource/金融市場常識-113.pdf";
            string textContent = ReadPdfContent(pdfFilePath);

            foreach(var item in AllTopic.Where(x=>x.Topic.Contains("(1)") && x.Topic.Contains("(2)") && x.Topic.Contains("(3)") && x.Topic.Contains("(4)"))) 
            {
                item.Topic = item.Topic.Trim();
                item.Topic = item.Topic.Replace("　", "");
                item.Topic = item.Topic.Substring(0, 3) + "\t" + item.Topic.Substring(3);
                item.Topic = item.Topic.Replace("(1)", "\t\n(1) ");
                item.Topic = item.Topic.Replace("(2)", "\t\n(2) ");
                item.Topic = item.Topic.Replace("(3)", "\t\n(3) ");
                item.Topic = item.Topic.Replace("(4)", "\t\n(4) ");
            }


            bool isFirstTime = true;
            while (true) 
            {
                if (isFirstTime) 
                {
                    Console.WriteLine("輸入C關閉，輸入.清空，Enter開始");
                    var input = Console.ReadLine();
                    if (input == "C")
                        break;
                    if (input == ".")
                    {
                        Console.Clear();
                        Console.WriteLine("輸入C關閉，輸入.清空，Enter繼續");
                    }
                    isFirstTime = false;
                }
                
                    
                Random rd = new Random();
                
                var topic = AllTopic.Find(x => x.Topic.Contains($"{rd.Next(0, 500)} "));
                if (topic != null && topic.Topic.Contains("(1)") && topic.Topic.Contains("(2)") && topic.Topic.Contains("(3)") && topic.Topic.Contains("(4)"))
                {
                    Console.WriteLine($"題目:{topic.Topic}");
                }
                else
                {
                    continue;
                }

                Console.WriteLine("---------------------------");
                Console.WriteLine("輸入答案: 1 2 3 4 \t\t\t\t輸入C關閉，輸入.清空，Enter繼續");
                var inputAnswer = Console.ReadLine();
                if (inputAnswer == "C") 
                {
                    break;
                }
                if (inputAnswer == ".")
                {
                    Console.Clear();
                    Console.WriteLine("輸入C關閉，輸入.清空，Enter繼續");
                }
                total++;
                if (topic.Answer.Contains(inputAnswer))
                {
                    correct++;
                    Console.WriteLine("正確");
                    Console.WriteLine("---------------------------\r\n");
                }
                else 
                {
                    Console.WriteLine($"錯誤，正確答案 {topic.Answer}");
                    Console.WriteLine("---------------------------\r\n");
                }
            }
            Console.WriteLine($"結束，結果({correct}/{total})");
        }

        static string ReadPdfContent(string pdfFilePath)
        {
            StringWriter text = new StringWriter();
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string pageContent = PdfTextExtractor.GetTextFromPage(reader, i);
                    var allParagraph = pageContent.Split('\n');
                    TopicClass currentTopic = new TopicClass();
                    bool isNewTopic = true;
                    for (int j = 0; j < allParagraph.Length; j++)
                    {
                        var current = allParagraph[j];
                        if (current.Contains("Part I"))
                            continue;
                        if(string.IsNullOrEmpty(current.Trim()))
                            continue;
                        if (current.Trim().Replace(" ", "") == "答案題號題目")
                            continue;


                        if (current.StartsWith("(1) ")|| current.StartsWith("(2) ") || current.StartsWith("(3) ") || current.StartsWith("(4) ")) 
                        {       
                            if(currentTopic != null && currentTopic.SetDone) 
                            {
                                AllTopic.Add(currentTopic);
                                currentTopic = new TopicClass();
                            }
                            var answer = current.Substring(0, 3);
                            var topic = current.Substring(3);
                            currentTopic.Answer = answer;
                            currentTopic.Topic = topic;
                            if(currentTopic.Topic.Contains("(1)") && currentTopic.Topic.Contains("(2)") && currentTopic.Topic.Contains("(3)") && currentTopic.Topic.Contains("(4)"))
                                currentTopic.SetDone = true;
                        }
                        else 
                        {
                            currentTopic.Topic += current;
                            currentTopic.SetDone = true;
                        }



                    }
                    text.Write(pageContent);
                }
            }
            return text.ToString();
        }
    }
    public class TopicClass
    {
        public string Topic { get; set; }
        public string Answer { get; set; }
        public bool SetDone { get; set; } = false;
    }
}
