﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using YoutubeSearchApi.Net.Exceptions;
using YoutubeSearchApi.Net.Models.Youtube;
using YoutubeSearchApi.Net.Modules;

namespace YoutubeSearchApi.Net.Services
{
    public class YoutubeSearchClient : ISearchClient<YoutubeSearchResult>
    {
        private static readonly string startFeature = "ytInitialData";
        private static readonly string BASE_URL = "https://www.youtube.com/results?search_query=";
        private readonly HttpClient httpClient;

        public YoutubeSearchClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public YoutubeSearchResult Search(string query, int retry = 3)
        {
            throw new NotImplementedException();
        }

        public async Task<YoutubeSearchResult> SearchAsync(string query, int retry = 3)
        {
            string encodedKeywords = HttpUtility.UrlEncode(query);
            string searchUrl = $"{BASE_URL}{encodedKeywords}";

            string pageContent = "";
            bool foundFeatureFlag = false;

            for (int i = 0; i < retry; i++)
            {
                pageContent = await Utils.GetSourceFromUrl(httpClient, searchUrl);
                if (pageContent.Contains(startFeature))
                {
                    foundFeatureFlag = true;
                    break;
                }
            }
            if (foundFeatureFlag == false)
            {
                throw new NoResultFoundException("What you searched was unfortunately not found or doesn't exist. query: " + query);
            }
            var videos = ParseData(pageContent);

            return new YoutubeSearchResult()
            {
                Url = searchUrl,
                Query = query,
                Results = videos
            };
        }

        internal List<YoutubeVideo> ParseData(string pageContent)
        {
            int startIndex = Regex.Match(pageContent, "{\\s*\"responseContext\"").Index;
            var match = Regex.Match(pageContent, "\"targetId\":\\s*\"search-page\"\\s*}", RegexOptions.RightToLeft);
            int endIndex = match.Index + match.Length;
            string jsonString = pageContent.Substring(startIndex, endIndex - startIndex);

            JObject jsonObject = JObject.Parse(jsonString);

            JArray contents = jsonObject["contents"]["twoColumnSearchResultsRenderer"]
                ["primaryContents"]["sectionListRenderer"]
                ["contents"][0]
                ["itemSectionRenderer"]["contents"].Value<JArray>();

            var videos = new List<YoutubeVideo>();

            foreach (JObject element in contents)
            {
                if (element.ContainsKey("videoRenderer"))
                {
                    JObject videoRenderer = element["videoRenderer"].Value<JObject>();

                    string videoId = videoRenderer["videoId"].Value<string>();

                    string videoUri = "https://www.youtube.com/watch?v=" + videoId;

                    string videoTitle = videoRenderer["title"]["runs"][0]["text"].Value<string>();

                    string videoThumbnailUrl = videoRenderer["thumbnail"]["thumbnails"][0]["url"].Value<string>();

                    string videoDuration = "";
                    if (videoRenderer.ContainsKey("lengthText"))
                        videoDuration = videoRenderer["lengthText"]["simpleText"].Value<string>().Replace(".", ":");

                    string videoAuthor = videoRenderer["longBylineText"]["runs"][0]["text"].Value<string>();

                    var videoDescription = videoRenderer["detailedMetadataSnippets"] != null
                        ? string.Join("; ", videoRenderer["detailedMetadataSnippets"][0]["snippetText"]["runs"].Select( run => run["text"].Value<string>()))
                        : "";

                    YoutubeVideo youtubeVideo = new YoutubeVideo(videoId, videoUri, videoTitle, videoThumbnailUrl, videoDuration, videoAuthor, videoDescription);
                    videos.Add(youtubeVideo);
                }
            }

            return videos;
        }
    }
}