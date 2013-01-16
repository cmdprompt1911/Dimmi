using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using Dimmi.Models.Domain;
using System.Collections;
using Lucene.Net.Store;
using Lucene.Net.QueryParsers;

namespace Dimmi.Search
{
    public class IndexManager
    {

        private static StandardAnalyzer _analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        private static object my_lock = new object();
        static readonly IReviewableRepository repository = new ReviewableRepository();

        private static MultiFieldQueryParser multiParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, new string[] { "content", "description1", "description2", "parentname" }, _analyzer);
        private static QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "content", _analyzer);
        private static Query query = null;
        private static Searcher searcher = null;

        private readonly static String _indexPath = HostingEnvironment.ApplicationPhysicalPath + "App_Data\\SearchIndex";

        public static void build_lucene_index(System.Web.HttpApplicationState app)
        {
            System.Threading.Thread thread = new System.Threading.Thread(threadproc_build);
            thread.Start(app);
        }

        static void threadproc_build(object obj)
        {
            lock (my_lock)
            {
                try
                {
                    
                    FSDirectory d = FSDirectory.Open(_indexPath);
                    IndexWriter writer = new IndexWriter(d, _analyzer, true, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);

                    List<ReviewableData> reviewables = (List<ReviewableData>)repository.Get();
                    foreach (ReviewableData r in reviewables)
                    {
                        writer.AddDocument(create_doc(r.id.ToString(), r.reviewableType, r.name, r.description, r.description2, r.parentName));

                    }
     
                    writer.Optimize();
                    writer.Dispose();
                }
                catch (Exception e)
                {
                    
                }
            }
        }


        static Lucene.Net.Documents.Document create_doc(string reviewableId, string reviewableType, string text, string description1, string description2, string parentName)
        {
            Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();

            doc.Add(new Lucene.Net.Documents.Field(
                "content",
                text,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "reviewabletype",
                reviewableType,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "description1",
                description1,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "description2",
                description2,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "reviewableid",
                reviewableId,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            doc.Add(new Lucene.Net.Documents.Field(
                "parentname",
                parentName,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            //// For the highlighter, store the raw text
            //doc.Add(new Lucene.Net.Documents.Field(
            //    "raw_text",
            //    text,
            //    Lucene.Net.Documents.Field.Store.YES,
            //    Lucene.Net.Documents.Field.Index.NOT_ANALYZED));

            return doc;
        }


        

        public static void threadproc_update(object obj)
        {
            lock (my_lock) // If a thread is updating the index, no other thread should be doing anything with it.
            {
                try
                {
                    if (searcher != null)
                    {
                        try
                        {
                            searcher.Dispose();
                        }
                        catch (Exception e)
                        {
                            
                        }
                        searcher = null;
                    }
                    Lucene.Net.Store.FSDirectory d = Lucene.Net.Store.FSDirectory.Open(_indexPath);

                    IndexWriter writer = new IndexWriter(d, _analyzer, Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);

                    // same as build, but uses "modifier" instead of write.
                    // uses additional "where" clause for bugid

                    ReviewableData r = (ReviewableData)obj;

                    writer.DeleteDocuments(new Lucene.Net.Index.Term("reviewableid", r.id.ToString()));

                    //ReviewableData r = (ReviewableData)repository.Get(reviewableId, Guid.Empty);

                    writer.AddDocument(create_doc(r.id.ToString(), r.reviewableType, r.name, r.description, r.description2, r.parentName));





                    writer.Flush(true, true, true);
                    writer.Dispose();

                }
                catch (Exception e)
                {
                    
                }
            }
        }

        //private static Query CreateFilteredByTypeQuery(string reviewableType, string criteria)
        //{
        //    criteria = criteria.Trim();
        //    reviewableType = reviewableType.Trim().ToLower();
        //    //BooleanQuery bq = new BooleanQuery();
        //    //bq.Add(new TermQuery(new Lucene.Net.Index.Term("reviewableType", reviewableType)), Occur.MUST);
        //    //bq.Add(criteria, Occur.MUST);
        //    //return bq;

        //    BooleanQuery mainQuery = new BooleanQuery();

        //    TermQuery contentFilter = new TermQuery(new Term("reviewabletype", reviewableType.Trim()));
        //    mainQuery.Add(contentFilter, Occur.MUST);

        //    WildcardQuery criteriaQ = new WildcardQuery(new Term("content", criteria + "*"));
        //    mainQuery.Add(criteriaQ, Occur.MUST);

        //    //BooleanQuery moduleFilter = new BooleanQuery();
        //    //moduleFilter.MinimumNumberShouldMatch = 1;
        //    //moduleFilter.Add(new TermQuery(new Term("text", criteria)), Occur.SHOULD);
        //    //moduleFilter.Add(new TermQuery(new Term("description1", criteria)), Occur.SHOULD);
        //    //moduleFilter.Add(new TermQuery(new Term("description2", criteria)), Occur.SHOULD);
        //    //mainQuery.Add(moduleFilter, Occur.MUST);
        //    return mainQuery;
           
        //}


        public static Hashtable searchReviewablesByReviewablesType(string searchText, string reviewableType)
        {
            if (!searchText.Contains(" "))
            {
                searchText = searchText + "*";
            }
           

            query = multiParser.Parse(searchText);

            Filter f = new PrefixFilter(new Term("reviewabletype",reviewableType));

            FilteredQuery fq = new FilteredQuery(query,f);
            Hashtable results = new Hashtable();
            lock (my_lock)
            {
    
                //Lucene.Net.Search.TopDocs hits = null;
                Lucene.Net.Search.TopDocs hits = null;
                try
                {
                    if (searcher == null)
                    {
                        Lucene.Net.Store.FSDirectory d = Lucene.Net.Store.FSDirectory.Open(_indexPath);
                        searcher = new Lucene.Net.Search.IndexSearcher(d);
                    }

                    hits = searcher.Search(fq, 200);

                }
                catch (Exception e)
                {
                    
                }
                
                for (int i = 0; i <= hits.ScoreDocs.Count() - 1; i++)
                {
                    Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                    string id = doc.GetField("reviewableid").StringValue;
                    
                    //double score = hits[i]
                    if (!results.Contains(id))
                        results.Add(id, hits.ScoreDocs[i].Score);
                    
                    float x = hits.MaxScore;

                }
            }
            return results;

        }


        public static Hashtable searchReviewables(string searchText)
        {
            query = parser.Parse(searchText + "*");


            Hashtable results = new Hashtable();
            lock (my_lock)
            {

                //Lucene.Net.Search.TopDocs hits = null;
                Lucene.Net.Search.TopDocs hits = null;
                try
                {
                    if (searcher == null)
                    {
                        Lucene.Net.Store.FSDirectory d = Lucene.Net.Store.FSDirectory.Open(_indexPath);
                        searcher = new Lucene.Net.Search.IndexSearcher(d);
                    }

                    hits = searcher.Search(query, 200);

                }
                catch (Exception e)
                {

                }

                for (int i = 0; i < hits.ScoreDocs.Count() - 1; i++)
                {
                    Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
                    string id = doc.GetField("reviewableid").StringValue;
                    if (!results.Contains(id))
                        results.Add(id, hits.ScoreDocs[i].Score);

                    float x = hits.MaxScore;
                }
            }
            return results;

        }
    
    
    }
}