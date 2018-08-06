using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SkDAL.Model;

namespace SkladMe.API.Methods
{
    public class Chapters
    {
        public static Dictionary<string, string> XPaths { get; set; }
        #region private fields

        private SkladchikApiClient _skladchikApiClient;

        #endregion

        public Chapters(SkladchikApiClient skladchikApiClient)
        {
            _skladchikApiClient = skladchikApiClient;
        }

        private static string GetNodeValue(HtmlDocument doc, string xpath, string attr = "")
        {
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            var value = node?.GetAttributeValue(attr, node.InnerText);
            return value;
        }

        public async Task<List<Product>> GetPartialProductsFromSubchapterAsync(string url, int prefixId,
            int pageMax = int.MaxValue)
        {
            var firstPageProducts = await _skladchikApiClient.CallAsync(url + $"?prefix_id={prefixId}").ConfigureAwait(false);

            if (string.IsNullOrEmpty(firstPageProducts))
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(firstPageProducts);

            var pageCount = Convert.ToInt32(GetNodeValue(doc, XPaths["PageCount"]) ?? "0");
            var lastPageIndex = pageCount >= pageMax ? pageMax : pageCount;
            if (lastPageIndex <= 0)
            {
                lastPageIndex = 1;
            }
            var links = new List<string>();

            for (var i = 1; i <= lastPageIndex; i++)
            {
                _skladchikApiClient.ThrowIfCancellationRequested();
                var productListUrl = url + $"/page-{i}?prefix_id={prefixId}";
                links.Add(productListUrl);
            }

            var products = await ParseProductFromPage(links).ConfigureAwait(false);

            return products;
        }

        public async Task<List<Product>> GetUserProducts()
        {
            var url = SkladchikApiClient.Domain + "shares/?status=involved";
            var firstPageProducts = await _skladchikApiClient.CallAsync(url + "&page=1").ConfigureAwait(false);

            if (string.IsNullOrEmpty(firstPageProducts))
            {
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(firstPageProducts);

            var pageCount = Convert.ToInt32(GetNodeValue(doc, XPaths["PageCount"]) ?? "1");

            var links = new List<string>();

            for (var i = 1; i <= pageCount; i++)
            {
                var productListUrl = url + $"&page={i}";
                links.Add(productListUrl);
            }

            var products = await ParseProductFromPage(links).ConfigureAwait(false);

            return products;
        }

        private async Task<List<Product>> ParseProductFromPage(List<string> links)
        {
            var pagesTasks = links.Select(_skladchikApiClient.CallAsync).ToList();
            await Task.WhenAll(pagesTasks).ConfigureAwait(false);

            var pagesList = pagesTasks.Select(u => u.Result).ToList();

            var products = new List<Product>();
            for (int i = 0; i < pagesList.Count; i++)
            {
                _skladchikApiClient.ThrowIfCancellationRequested();
                var page = pagesList.ElementAt(i);
                if (string.IsNullOrEmpty(page))
                {
                    continue;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(page);

                var nodes = doc.DocumentNode.SelectNodes(XPaths["ProductsList"]);

                if (nodes == null)
                    break;

                foreach (var node in nodes)
                {
                    _skladchikApiClient.ThrowIfCancellationRequested();
                    //TODO: Литералы
                    //Проверка префикса
                    var prefix = node.SelectSingleNode(XPaths["Prefix"])?.InnerText;
                    if (string.IsNullOrEmpty(prefix) ||
                        (prefix != "Закрыто" && prefix != "Активно" && prefix != "Открыто" && prefix != "Доступно"))
                    {
                        continue;
                    }
                    //Ссылка на складчину
                    var productUrl = node.SelectSingleNode(XPaths["ProductUrl"])?.GetAttributeValue("href", "");

                    //Количество просмотров
                    var viewCountString = node.SelectSingleNode(XPaths["ViewCount"])?.InnerText;
                    int.TryParse(viewCountString?.Replace(".", ""), out int viewCount);

                    //Важная ли складчина
                    var importantString = node.SelectSingleNode(XPaths["Important"])?.InnerText;
                    var important = !string.IsNullOrWhiteSpace(importantString);

                    //Последние изменения
                    var lastChangesString = node.SelectSingleNode(XPaths["LastChanges"])?.InnerText;

                    //Id складчины
                    var productId = int.Parse(Regex.Match(productUrl, @"\d+(?=/)").Value);

                    //Название
                    var title = node.SelectSingleNode(XPaths["Title"])?.InnerText;

                    //Рейтинг
                    var ratingStr = node.SelectSingleNode(XPaths["Rating"])?.GetAttributeValue("title", "");
                    double? raiting = null;
                    bool successParse = double.TryParse(ratingStr, out double r);
                    if (successParse)
                    {
                        raiting = r;
                    }

                    //Взнос
                    var feeStr = node.SelectSingleNode(XPaths["Fee"])?.InnerText;
                    int? fee = null;
                    if (!string.IsNullOrEmpty(feeStr))
                    {
                        var feeReg = Regex.Match(feeStr, @"\d+").Value;

                        if (!string.IsNullOrEmpty(feeReg))
                        {
                            fee = int.Parse(feeReg);
                        }
                    }

                    //Дата создания
                    var dateStr = node.SelectSingleNode(XPaths["DateCreate"])?.InnerText
                                  ?? node.SelectSingleNode(XPaths["DateCreateNew"])?.GetAttributeValue("data-datestring", "");
                    var dateCreate = Products.ConvertDate(dateStr);

                    //Топик стартер
                    var author = node.SelectSingleNode(XPaths["Author"])?.InnerText;

                    //Дата сбора взносов
                    var dateFeeStr = node.SelectSingleNode(XPaths["DateFee"])?.InnerText.Trim();
                    DateTime? dateFee = null;
                    if (!string.IsNullOrEmpty(dateFeeStr))
                    {
                        var dateFeeReg = Regex.Match(dateFeeStr, @"\d+\ [a-zA-Zа-яА-Я]+\ \d+$").Value;
                        dateFee = Products.ConvertDate(dateFeeReg);
                    }

                    var product = new Product()
                    {
                        Id = productId,
                        Prefix = new Prefix() { Title = prefix.ToLower() },
                        Title = title,
                        DateUpdate = Products.ConvertDate(lastChangesString),
                        ViewCount = viewCount,
                        Important = important,
                        Rating = raiting,
                        Fee = fee,
                        DateOfCreation = dateCreate,
                        Creator = new User() { Nickname = author },
                        DateFee = dateFee
                    };
                    products.Add(product);
                }
            }
            return products;
        }
    }
}