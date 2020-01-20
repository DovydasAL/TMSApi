using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TMSEntities
{
    public class TMSScraper
    {
        public List<Term> Terms { get; set; } = new List<Term>();
        public Term LastTerm { get; set; }
        public List<Listing> Listings { get; set; } = new List<Listing>();
        public long New { get; set; }
        public long Updated { get; set; }
        public async Task<bool> Execute()
        {
            var listings = new List<Listing>();
            var termNodes = await GetTermNodes();
            foreach (var termNode in termNodes)
            {
                var collegePaths = await GetCollegePaths(termNode);
                foreach (var collegePath in collegePaths)
                {
                    var departmentPaths = await GetDepartmentPaths(collegePath);
                    foreach (var departmentPath in departmentPaths)
                    {
                        var classNodes = await GetClassNodes(departmentPath);
                        foreach (var classNode in classNodes)
                        {
                            await PopulateListings(classNode);
                        }
                    }
                }
            }
            SaveListings();
            return true;
        }

        public async Task<HtmlNodeCollection> GetTermNodes()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(Constants.HOME_URL);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get term nodes (Step 1)");
                }
                var content = await response.Content.ReadAsStringAsync();
                var homeDocument = new HtmlDocument();
                homeDocument.LoadHtml(content);
                return homeDocument.DocumentNode.SelectNodes(Constants.TERM_PATHS);
            }
        }

        public async Task<IEnumerable<string>> GetCollegePaths(HtmlNode termNode)
        {
            using (var client = new HttpClient())
            {
                Term term;
                using (var ctx = new TMSDAL())
                {
                    term = ctx.Terms.FirstOrDefault(x => x.LookupLabel == termNode.InnerText.Replace(" ", ""));
                    if (term == null)
                    {
                        term = new Term()
                        {
                            TermName = termNode.InnerText.Trim(),
                            LookupLabel = termNode.InnerText.Replace(" ", "")
                        };
                        Terms.Add(term);
                        LastTerm = term;
                    }
                }
                var termPath = termNode.Attributes.First(x => x.Name == "href").Value.Replace("amp;", "");
                var termListing = await client.GetAsync(Constants.BASE_PATH + termPath);
                if (!termListing.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get College Paths (Step 2)");
                }
                var termContent = await termListing.Content.ReadAsStringAsync();
                var listingDocument = new HtmlDocument();
                listingDocument.LoadHtml(termContent);
                var collegeNodes = listingDocument.DocumentNode.SelectNodes(Constants.COLLEGE_PATHS);
                return collegeNodes.Select(x => x.Attributes.First(x => x.Name == "href").Value.Replace("amp;", ""));
            }
        }

        public async Task<IEnumerable<string>> GetDepartmentPaths(string collegePath)
        {
            using (var client = new HttpClient())
            {
                var collegeListing = await client.GetAsync(Constants.BASE_PATH + collegePath);
                if (!collegeListing.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get Department Paths (Step 3)");
                }
                var collegeContent = await collegeListing.Content.ReadAsStringAsync();
                var collegeDocument = new HtmlDocument();
                collegeDocument.LoadHtml(collegeContent);
                var departmentNodes = collegeDocument.DocumentNode.SelectNodes(Constants.DEPARTMENT_PATHS);
                return departmentNodes.Select(x => x.Attributes.First(x => x.Name == "href").Value.Replace("amp;", ""));
            }
        }

        public async Task<HtmlNodeCollection> GetClassNodes(string departmentPath)
        {
            using (var client = new HttpClient())
            {
                var classListing = await client.GetAsync(Constants.BASE_PATH + departmentPath);
                if (!classListing.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to get Class Nodes (Step 4)");
                }
                var classContent = (await classListing.Content.ReadAsStringAsync());
                var classDocument = new HtmlDocument();
                classDocument.LoadHtml(classContent);
                return classDocument.DocumentNode.SelectNodes(Constants.CLASS_PATHS);
            }
        }

        public async Task PopulateListings(HtmlNode individualClassNode)
        {
            var children = individualClassNode.ChildNodes.Where(x => x.Name == "td").ToList();
            if (children.Count != 9)
            {
                return;
            }
            var enrollRegex = new Regex("\\d+");
            var matches = enrollRegex.Matches(children[5].ChildNodes[0].Attributes.First(x => x.Name == "title").Value);
            string enroll, maxEnroll, crn;
            if (matches.Count != 2)
            {
                enroll = "CLOSED";
                maxEnroll = "FULL";
                crn = children[5].ChildNodes[0].InnerText;
            }
            else
            {
                enroll = matches[1].Value;
                maxEnroll = matches[0].Value;
                crn = children[5].InnerText;
            }
            var listing = new Listing()
            {
                Subject = children[0].InnerText.Trim(),
                CourseNumber = children[1].InnerText.Trim(),
                InstructionType = children[2].InnerText.Trim().Replace("amp;", ""),
                InstructionMethod = children[3].InnerText.Trim(),
                Section = children[4].InnerText.Trim(),
                CRN = crn.Trim(),
                MaxEnroll = maxEnroll.Trim(),
                Enroll = enroll.Trim(),
                CourseTitle = children[6].InnerText.Trim().Replace("amp;", ""),
                Times = children[7].InnerText.Trim().Replace("\r", "").Replace("\t", ""),
                Instructor = children[8].InnerText.Trim(),
                Term = LastTerm,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            Listings.Add(listing);
        }

        public async Task SaveListings()
        {
            using (TMSDAL ctx = new TMSDAL())
            {
                foreach (var listing in Listings)
                {
                    var oldListing = ctx.Listings.FirstOrDefault(x => x.CRN == listing.CRN && x.Term.TermID == listing.Term.TermID);
                    if (oldListing != null && !oldListing.IsEqual(listing))
                    {

                        oldListing.Subject = listing.Subject;
                        oldListing.CourseNumber = listing.CourseNumber;
                        oldListing.InstructionType = listing.InstructionType;
                        oldListing.InstructionMethod = listing.InstructionMethod;
                        oldListing.Section = listing.Section;
                        oldListing.MaxEnroll = listing.MaxEnroll;
                        oldListing.Enroll = listing.Enroll;
                        oldListing.CourseTitle = listing.CourseTitle;
                        oldListing.Times = listing.Times;
                        oldListing.Instructor = listing.Instructor;
                        oldListing.ModifiedDate = DateTime.Now;
                        Updated += 1;
                    }
                    else
                    {
                        New += 1;
                        ctx.Add(listing);
                    }
                }
                foreach (var term in Terms)
                {
                    if (ctx.Terms.Any(x => x.LookupLabel == term.LookupLabel))
                    {
                        var oldTerm = ctx.Terms.First(x => x.LookupLabel == term.LookupLabel);
                        ctx.Entry(oldTerm).CurrentValues.SetValues(term);
                    }
                    else
                    {
                        ctx.Add(term);
                    }
                }
                ctx.SaveChanges();
            }
        }
    }
}
