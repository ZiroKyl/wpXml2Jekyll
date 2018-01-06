using System;
using System.IO;
using System.Xml;

namespace wpXml2Jekyll
{
    public class PostWriter
    {
        private readonly String _postTypeAttachment = "attachment";

        public int WritePost(XmlDocument xmlDocumentToWrite, string outputFolder)
        {
            var items = xmlDocumentToWrite.SelectNodes("//item");
            int postCount = 0;

            var namespaceManager = new XmlNamespaceManager(xmlDocumentToWrite.NameTable);
			namespaceManager.AddNamespace("excerpt", "http://wordpress.org/export/1.2/excerpt/");
			namespaceManager.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");
			namespaceManager.AddNamespace("wfw",     "http://wellformedweb.org/CommentAPI/");
			namespaceManager.AddNamespace("dc",      "http://purl.org/dc/elements/1.1/");
			namespaceManager.AddNamespace("wp",      "http://wordpress.org/export/1.2/");

            foreach(XmlNode item in items){
                string postType = item.SelectSingleNode("wp:post_type", namespaceManager).InnerText;

                //check if the item is post,page or attachment
                //attachments shouldn't be saved as a post
                if(!String.Equals(postType, _postTypeAttachment)){
					string title   = item.SelectSingleNode("title").InnerText;
					DateTime date  = DateTime.Parse(item.SelectSingleNode("wp:post_date", namespaceManager).InnerText);
					string content = item.SelectSingleNode("content:encoded", namespaceManager).InnerText;
					string url     = item.SelectSingleNode("wp:post_name", namespaceManager).InnerText;
					string author  = item.SelectSingleNode("dc:creator", namespaceManager).InnerText;

                    var categories = item.SelectNodes("category[@domain='category']", namespaceManager);
                    var tags = item.SelectNodes("category[@domain='post_tag']", namespaceManager);

                    string postStatus = item.SelectSingleNode("wp:status", namespaceManager).InnerText;
					string folderPath = AppendStatusToOutputFolder(outputFolder, postStatus);
                    CreateDirectoryIfDoesntExist(folderPath);

					// Fix too expensive... Just rewrite in Go.

					// .NET 2.0 Workaround for PathTooLongException https://www.codeproject.com/Articles/22013/NET-Workaround-for-PathTooLongException
					// URI decode (use System.Uri.UnescapeDataString):
					// - https://blogs.msdn.microsoft.com/yangxind/2006/11/08/dont-use-net-system-uri-unescapedatastring-in-url-decoding/
					// - https://gist.github.com/xl1/2d0b2890fa4a70142a41afd4c5bbe215
					// IsBadFileSystemCharacter https://msdn.microsoft.com/en-us/library/system.uri.isbadfilesystemcharacter(v=vs.110).aspx
					using(TextWriter tw = new StreamWriter(folderPath + Path.DirectorySeparatorChar + date.ToString("yyyy-MM-dd-") + url + ".html")){
                        tw.WriteLine("---");
                        tw.Write("layout: ");
                        tw.WriteLine(postType);//different layout for pages
                        tw.WriteLine("title: \"" + title.Replace("\"", "&quot;") + "\"");
                        tw.WriteLine("date: " + date.ToString("yyyy-MM-dd HH:mm"));
                        tw.WriteLine("author: " + author);
                        tw.WriteLine("comments: true");
                        tw.Write("categories: [");

                        for(int i=0; i<categories.Count; i++){
                            tw.Write(categories[i].InnerText);
                            if(i+1 < categories.Count) tw.Write(", ");//TODO: проверять присутствие "," в исходной строке
                        }

                        tw.WriteLine("]");
                        tw.Write("tags: [");

                        for(int i=0; i<tags.Count; i++){
                            tw.Write(tags[i].InnerText);
                            if(i+1 < tags.Count) tw.Write(", ");
                        }

                        tw.WriteLine("]");
                        tw.WriteLine("---");
                        tw.WriteLine(content);

                        postCount++;
                    }
                }
            }

            return postCount;
        }

        private static void CreateDirectoryIfDoesntExist(string folderPath)
        {
            if(!Directory.Exists(folderPath)){
                Directory.CreateDirectory(folderPath);
            }
        }

        private static string AppendStatusToOutputFolder(string outputFolder, string postStatus)
        {
            string folderPath = outputFolder;
            if(!String.IsNullOrWhiteSpace(postStatus)){
                folderPath = outputFolder + Path.DirectorySeparatorChar + postStatus;
            }

            return folderPath;
        }
    }
}