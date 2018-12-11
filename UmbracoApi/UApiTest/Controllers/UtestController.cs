using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace UApiTest.Controllers
{
    public class UtestController : UmbracoApiController
    {
        [HttpPost] // Api -> Post new content "newsitem" under "news"
        public string PostNews([FromBody] News data)
        {
            //string result = "start";
            var contentService = Services.ContentService;
            // Change page id and item according to what needs to be created where.
            var item = contentService.CreateContent(data.Title, 1066, "NewsItem");
            // Set values for what is to be created
            item.SetValue("headline", data.Title);
            item.SetValue("bodyText", data.BodyText);
            // Save the item
            var isSaved = contentService.SaveAndPublishWithStatus(item);
            string result = isSaved ? "OK" : "ERROR";
            return result;
        }

        // /umbraco/api/utest/DeleteNews
        [HttpPost] // Api -> Find newsitem by id and delete it
        public string DeleteNews([FromBody] News data)
        {
            var contentService = ApplicationContext.Services.ContentService;
            var pages = contentService.GetChildren(1066).Where(x => x.Id == data.Id);
            foreach (var item in pages)
            {
                contentService.Delete(item);
            }

            contentService.EmptyRecycleBin(); // Research docs about this...

            return "ok";
        }

        // /umbraco/api/utest/News
        [HttpGet] // Api -> Get a list of all the news items -> headline & bodyText
        public List<News> News()
        {
            List<News> result = new List<News>();
            //get the content service
            var cs = Services.ContentService;
            //get the list of news
            var newsContainer = cs.GetById(1066); //Id of the page that contains the news
            var news = newsContainer.Children();

            foreach (var item in news)
            {
                result.Add(new News()
                {
                    Id = item.Id,
                    Title = item.GetValue("headline").ToString(),
                    BodyText = item.GetValue("bodyText").ToString()
                });
            }
            return result;
        }
    }


    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BodyText { get; set; }
    }
}
